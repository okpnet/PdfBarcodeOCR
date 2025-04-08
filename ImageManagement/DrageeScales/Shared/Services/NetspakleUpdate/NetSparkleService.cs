using DrageeScales.Shared.Services.NetspakleUpdate.Args;
using Microsoft.Extensions.Logging;
using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.SignatureVerifiers;
using Serilog.Core;
using System;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace DrageeScales.Shared.Services.NetspakleUpdate
{
    public class NetSparkleService : IDisposable
    {
        readonly ILogger _logger;
        readonly Subject<UpdateEventArg> _subject;
        /// <summary>
        /// Sparkleインスタンス
        /// </summary>
        SparkleUpdater _sparkle;
        /// <summary>
        /// アップデート情報
        /// </summary>
        UpdateInfo _info;
        /// <summary>
        /// イベントハンドラDispose
        /// </summary>
        CompositeDisposable _disposables = new CompositeDisposable();
        /// <summary>
        /// ダウンロードファイルパス
        /// </summary>
        FileInfo _downloadPath = default;
        /// <summary>
        /// AppcastURL
        /// </summary>
        Uri _appcastUrl = default;
        /// <summary>
        /// 公開鍵パス
        /// </summary>
        FileInfo _publicKeyPath = default;
        /// <summary>
        /// Sparkleインスタンス
        /// </summary>
        public SparkleUpdater Sparkle { get => _sparkle; }
        /// <summary>
        /// クローズアクション
        /// </summary>
        public Action AppCloseAction { get; init; }
        /// <summary>
        /// 準備完了
        /// </summary>
        public bool UpdateReady { get; protected set; }
        /// <summary>
        /// 準備完了通知
        /// </summary>
        public IObservable<UpdateEventArg> UpdateReadyEvent { get; }
        /// <summary>
        /// アップデートステータスチェックイベント
        /// </summary>
        public IObservable<UpdateEventArg> UpdateCheckFinishedEvent { get; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NetSparkleService(Action appclose,FileInfo publicKeyPath,Uri appcastUrl, FileInfo downloadPath)
        {
            
            _appcastUrl = appcastUrl;
            _downloadPath = downloadPath;
            AppCloseAction = appclose;
            UpdateReady = false;
            _sparkle = new SparkleUpdater
                (
                    _appcastUrl.AbsoluteUri,
                    new Ed25519Checker(SecurityMode.Unsafe, publicKeyPath.FullName)
                )
            {
                UIFactory = null,
            };

            UpdateReadyEvent = _subject.Where(t => t.State == UpdateState.UpdateStandby).AsObservable();//準備完了イベント
            UpdateCheckFinishedEvent = _subject.Where(t => t.IsCheckFinished).AsObservable();//チェック完了イベント

            AddEvent();

            _sparkle.StartLoop(true);
            _info = _sparkle.CheckForUpdatesQuietly().Result;
        }

        public NetSparkleService(ILogger<NetSparkleService> logger, Action appclose, FileInfo publicKeyPath, Uri appcastUrl, FileInfo downloadPath) : this(appclose, publicKeyPath, appcastUrl, downloadPath)
        {
            _logger = logger;
        }
        /// <summary>
        /// イベント追加
        /// </summary>
        private void AddEvent()
        {
            _disposables.Add(//チェック完了イベント
                Observable.FromEventPattern<object, UpdateStatus>(_sparkle, nameof(UpdateCheckFinished))
                .Subscribe((t) =>
                {

                    if (_sparkle is null || _info is null || !_info.Updates.Any()) return;
                    var arg = t.EventArgs == UpdateStatus.UpdateAvailable ?
                            UpdateEventArg.AvailableUpdate() : UpdateEventArg.NotAvailableUpdate();
                    _subject.OnNext(arg);
                    if (t.EventArgs != UpdateStatus.UpdateAvailable || arg.IsCancel)
                    {
                        return;
                    }
                    var updateDate = _info.Updates.Last();
                    _sparkle.InitAndBeginDownload(updateDate);

                })
            );

            _disposables.Add(//準備完了イベント
                Observable.FromEventPattern<object, string>(_sparkle, nameof(_sparkle.DownloadFinished)).
                Subscribe((t) =>
                {
                    if (_sparkle is null || _info is null) return;
                    var updater = _info.Updates.LastOrDefault();
                    if (updater is null) return;

                    _downloadPath =new(t.EventArgs);
                    _subject.OnNext(UpdateEventArg.StandbyUpdate());
                    UpdateReady = true;
                })
            );

            _disposables.Add(
                Observable.FromEvent<CloseApplication, Unit>(
                t => () => t(Unit.Default),
                t => _sparkle.CloseApplication += t, t => _sparkle.CloseApplication -= t).
                Subscribe((_) => AppCloseAction.Invoke())
                );
        }

        /// <summary>
        /// Appcast情報
        /// </summary>
        /// <returns></returns>
        //public async Task<UpdateInfo> InitAsync()
        //{
        //    if (_sparkle is null) return null;

        //    //_sparkle.LogWriter=new LogWriter(true);
        //    await _sparkle.StartLoop(true);

        //    _info = await _sparkle.CheckForUpdatesQuietly();
        //    //if (_info.Status == UpdateStatus.UpdateAvailable)
        //    //{
        //    //    UpdateStateSubject.OnNext(_info.Status);
        //    //    var updateDate = _info.Updates.Last();
        //    //    await _sparkle.InitAndBeginDownload(updateDate);
        //    //}
        //    return _info;
        //}
        /// <summary>
        /// アップデート実行
        /// </summary>
        public void Update()
        {
            if (_sparkle is null || _info is null || _downloadPath is null) return;
            var updateDate = _info.Updates.FirstOrDefault();
            if (updateDate is not null && updateDate.IsWindowsUpdate)
            {
                var exepath = GetNewFileName(_downloadPath.FullName, updateDate.Version);
                try
                {
                    System.IO.File.Move(_downloadPath.FullName, exepath, true);
                    _sparkle.InstallUpdate(updateDate, exepath);
                }
                catch (Exception ex)
                {
                    _logger?.LogWarning(ex, "CL:{class} M:{method} EXCEPTION", GetType().Name, nameof(Update));
                }
            }
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _disposables.Clear();
        }
        /// <summary>
        /// インストーラーファイル名
        /// </summary>
        /// <param name="oldfileName"></param>
        /// <returns></returns>
        string GetNewFileName(string oldfileName, string version)
        {
            var exepath = oldfileName + ".exe";
            var fileinfo = new System.IO.FileInfo(oldfileName);
            var assemblyNames = Assembly.GetExecutingAssembly().GetName();
            if (fileinfo is not null && assemblyNames is not null)
            {
                var dir = System.IO.Path.GetDirectoryName(oldfileName);
                exepath = System.IO.Path.Combine(dir!, assemblyNames.Name ?? DEFAULT_FILE_NAME)
                    + "_"
                    + version
                    + ".exe";
            }
            return exepath;
        }
    }
}
