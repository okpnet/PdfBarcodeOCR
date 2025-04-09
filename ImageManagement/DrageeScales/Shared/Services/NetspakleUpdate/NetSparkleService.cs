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
using System.Reflection;
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
        /// AppcastURL
        /// </summary>
        Uri _appcastUrl = default;
        /// <summary>
        /// 公開鍵パス
        /// </summary>
        FileInfo _publicKeyPath = default;
        /// <summary>
        /// ダウンロードしたアップデートファイル
        /// </summary>
        FileInfo? _downloadFile;
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
        public IObservable<UpdateEventArg> UpdateStandbyEvent { get; }
        /// <summary>
        /// アップデートステータスチェックイベント
        /// </summary>
        public IObservable<UpdateEventArg> UpdateCheckFinishedEvent { get; }
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NetSparkleService(Action appclose,FileInfo publicKeyPath,Uri appcastUrl)
        {
            
            _appcastUrl = appcastUrl;
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

            UpdateStandbyEvent = _subject.Where(t => t.State == UpdateState.UpdateStandby).AsObservable();//準備完了イベント
            UpdateCheckFinishedEvent = _subject.Where(t => t.IsCheckFinished).AsObservable();//チェック完了イベント

            AddEvent();

            _sparkle.StartLoop(true);
            _info = _sparkle.CheckForUpdatesQuietly().Result;
        }

        public NetSparkleService(ILogger<NetSparkleService> logger, Action appclose, FileInfo publicKeyPath, Uri appcastUrl, FileInfo downloadPath) : this(appclose, publicKeyPath, appcastUrl)
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

                    _downloadFile =new(t.EventArgs);
                    _subject.OnNext(UpdateEventArg.StandbyUpdate(_downloadFile,updater.Version));
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
        /// アップデート実行
        /// </summary>
        public void Update(Action<UpdateEventArg> action)
        {
            if(_sparkle is null || _info is null)
            {
                throw new NullReferenceException($"NetSparkleUpdater requierd for update is null.");
            }
            var updateDate = _info.Updates.FirstOrDefault();
            var arg = _downloadFile is null ?
                UpdateEventArg.NotAvailableUpdate() : UpdateEventArg.StandbyUpdate(_downloadFile, updateDate.Version);

            action(arg);
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _disposables.Clear();
        }
    }
}
