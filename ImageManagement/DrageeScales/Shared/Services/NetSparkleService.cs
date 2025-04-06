using Microsoft.Extensions.Logging;
using NetSparkleUpdater;
using NetSparkleUpdater.Enums;
using NetSparkleUpdater.SignatureVerifiers;
using Serilog.Core;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reflection;
using System.Threading.Tasks;

namespace StaffIdentityCard.Updaters
{
    public class NetSparkleService:IDisposable
    {
        const string APPCAST_URL = "https://www.dropbox.com/s/hojfguq50g91fqe/appcast.xml?dl=1";
        const string PUBLIC_KEY_PATH = "NetSparkle_Ed25519.pub";
        const string DEFAULT_FILE_NAME = "Installer";
        readonly ILogger? _logger;
        /// <summary>
        /// Sparkleインスタンス
        /// </summary>
        SparkleUpdater? _sparkle;
        /// <summary>
        /// アップデート情報
        /// </summary>
        UpdateInfo? _info;
        /// <summary>
        /// イベントハンドラDispose
        /// </summary>
        CompositeDisposable _disposables=new CompositeDisposable();
        /// <summary>
        /// ダウンロードファイルパス
        /// </summary>
        string _downloadPath = string.Empty;
        /// <summary>
        /// Sparkleインスタンス
        /// </summary>
        public SparkleUpdater? Sparkle { get=> _sparkle; }
        /// <summary>
        /// アップデートステータスイベント
        /// </summary>
        public ISubject<UpdateStatus> UpdateStateSubject { get;}=new Subject<UpdateStatus>();
        ///// <summary>
        ///// ダウンロード中イベント
        ///// </summary>
        //public ISubject<ItemDownloadProgressEventArgs> DouwnLoadingSubject { get; } = new Subject<ItemDownloadProgressEventArgs>();
        /// <summary>
        /// クローズアクション
        /// </summary>
        public Action AppCloseAction { get; init; }
        /// <summary>
        /// 準備完了
        /// </summary>
        public bool UpdateReady { get;protected set; }
        /// <summary>
        /// 準備完了通知
        /// </summary>
        public ISubject<bool> UpdateReadySubject { get; set; }=new Subject<bool>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NetSparkleService(Action appclose)
        {
            AppCloseAction= appclose;
            UpdateReady = false;
            _sparkle = new SparkleUpdater(APPCAST_URL,
                new Ed25519Checker(NetSparkleUpdater.Enums.SecurityMode.Unsafe, PUBLIC_KEY_PATH))
            {
                UIFactory = null,
                
            };

            _disposables.Add(Observable.FromEventPattern<object, UpdateStatus>(_sparkle, nameof(NetSparkleUpdater.UpdateCheckFinished))
            .Subscribe((t) =>
            {
                if (_sparkle is null || t.EventArgs != UpdateStatus.UpdateAvailable || _info is null || !_info.Updates.Any()) return;
                UpdateStateSubject.OnNext(t.EventArgs);
                var updateDate = _info.Updates.Last();
                _sparkle.InitAndBeginDownload(updateDate);
            }));

            _disposables.Add(Observable.FromEventPattern<object, string>(_sparkle, nameof(_sparkle.DownloadFinished)).Subscribe((t) =>
            {
                if (_sparkle is null || _info is null) return;
                var updater = _info.Updates.LastOrDefault();
                if (updater is null) return;
                
                _downloadPath = t.EventArgs;
                UpdateReady = true;
                UpdateReadySubject.OnNext(true);
            }));

            _disposables.Add(Observable.FromEvent<CloseApplication, Unit>(
                t => () => t(Unit.Default),
                t => _sparkle.CloseApplication += t, t => _sparkle.CloseApplication -= t).
                Subscribe((_) => AppCloseAction.Invoke())
                );
        }

        public NetSparkleService(ILogger<NetSparkleService> logger, Action appclose) : this(appclose)
        {
            _logger = logger;
        }

        /// <summary>
        /// Appcast情報
        /// </summary>
        /// <returns></returns>
        public async Task<UpdateInfo?> InitAsync()
        {
            if (_sparkle is null) return null;

            //_sparkle.LogWriter=new LogWriter(true);
            await _sparkle.StartLoop(true);

            _info =await _sparkle.CheckForUpdatesQuietly();
            if (_info.Status == UpdateStatus.UpdateAvailable)
            {
                UpdateStateSubject.OnNext(_info.Status);
                var updateDate = _info.Updates.Last();
                await _sparkle.InitAndBeginDownload(updateDate);
            }
            return _info;
        }
        /// <summary>
        /// アップデート実行
        /// </summary>
        public void Update()
        {
            if (_sparkle is null || _info is null || string.IsNullOrEmpty(_downloadPath)) return;
            var updateDate = _info.Updates.FirstOrDefault();
            if (updateDate is not null && updateDate.IsWindowsUpdate)
            { 
                var exepath = GetNewFileName(_downloadPath,updateDate.Version);
                try
                {
                    System.IO.File.Move(_downloadPath, exepath,true);
                    _sparkle.InstallUpdate(updateDate, exepath);
                }catch(Exception ex)
                {
                    _logger?.LogWarning(ex, "CL:{class} M:{method} EXCEPTION", this.GetType().Name, nameof(Update));
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
        string GetNewFileName(string oldfileName,string version)
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
