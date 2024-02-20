using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace HttpLib
{
    partial class HttpDown
    {
        #region 参数

        /// <summary>
        /// 自定义ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 下载地址
        /// </summary>
        public string Url { get; }

        /// <summary>
        /// 保存路径
        /// </summary>
        public string SavePath { get; }
        public string SaveFullName { get; private set; }

        /// <summary>
        /// 超时时长
        /// </summary>
        public int TimeOut = Config.TimeOut;

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount = Config.RetryCount;

        /// <summary>
        /// 下载缓存大小
        /// </summary>
        public int CacheSize = Config.CacheSize;
        public string UserAgent = Config.UserAgent;

        /// <summary>
        /// 下载名称
        /// </summary>
        public string Name { get => _Name; }
        /// <summary>
        /// 下载速度
        /// </summary>
        public double Speed { get => _Speed; }

        /// <summary>
        /// 剩余时间
        /// </summary>
        public string Time { get => _Time; }

        /// <summary>
        /// 当前下载值
        /// </summary>
        public double Value { get => _Value; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public double MaxValue { get => _MaxValue; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public DownState State { get => _State; }

        DownState _State = DownState.Ready;
        long _MaxValue, _Value, _Speed;
        string _Name, _Time;

        public enum DownState
        {
            /// <summary>
            /// 准备中
            /// </summary>
            Ready,
            /// <summary>
            /// 下载中
            /// </summary>
            Downloading,
            /// <summary>
            /// 已停止
            /// </summary>
            Stop,
            /// <summary>
            /// 完成
            /// </summary>
            Complete,
            /// <summary>
            /// 异常
            /// </summary>
            Fail
        }

        #region 非公开

        Dictionary<int, long> ValTemp = new Dictionary<int, long>();
        Dictionary<int, long> MaxValTemp = new Dictionary<int, long>();
        ManualResetEvent resetState = new ManualResetEvent(true);

        #endregion

        #endregion

        #region 任务事件

        public delegate void StateHandler(DownState e, string err);

        #region 文件大小

        int DownCount = 0, TotalCount = 0;
        void SetMaxValue(long value)
        {
            if (_MaxValue != value)
            {
                _MaxValue = value;
                if (_MaxValueChange != null) _MaxValueChange(value);
            }
        }
        void SetMaxValue()
        {
            try { var val = MaxValTemp.Sum(ab => ab.Value); if (TotalCount > 0) { SetMaxValue(val * TotalCount / DownCount); } else { SetMaxValue(val); } } catch { }
        }

        Action<long> _MaxValueChange;
        /// <summary>
        /// 文件大小改变
        /// </summary>
        public HttpDown MaxValueChange(Action<long> action)
        {
            _MaxValueChange = action;
            return this;
        }

        #endregion

        #region 当前下载值

        void SetValue(long value)
        {
            if (_Value != value)
            {
                _Value = value;
                if (_ValueChange != null) _ValueChange(value);
            }
        }
        void SetValue()
        {
            try { SetValue(ValTemp.Sum(ab => ab.Value)); } catch { }
        }

        Action<long> _ValueChange;
        /// <summary>
        /// 当前下载值改变
        /// </summary>
        public HttpDown ValueChange(Action<long> action)
        {
            _ValueChange = action;
            return this;
        }

        #endregion

        #region 下载名称

        void SetName(string value)
        {
            if (_Name != value)
            {
                _Name = value;
                if (_NameChange != null) _NameChange(value);
            }
        }

        Action<string> _NameChange;
        /// <summary>
        /// 下载名称改变
        /// </summary>
        public HttpDown NameChange(Action<string> action)
        {
            _NameChange = action;
            return this;
        }

        #endregion

        #region 下载速度

        bool canSpeed = true;
        void SetSpeed(long value)
        {
            if (!canSpeed) return;
            if (_Speed != value)
            {
                _Speed = value;
                if (_SpeedChange != null) _SpeedChange(value);
            }
        }

        Action<long> _SpeedChange;
        /// <summary>
        /// 下载速度改变
        /// </summary>
        public HttpDown SpeedChange(Action<long> action)
        {
            _SpeedChange = action;
            return this;
        }

        #endregion

        #region 剩余时间

        void SetTime(string value)
        {
            if (!canSpeed) return;
            if (_Time != value)
            {
                _Time = value;
                if (_TimeChange != null) _TimeChange(value);
            }
        }

        Action<string> _TimeChange;
        /// <summary>
        /// 剩余时间改变
        /// </summary>
        public HttpDown TimeChange(Action<string> action)
        {
            _TimeChange = action;
            return this;
        }

        #endregion

        #region 任务状态

        void SetState(DownState value, string err = null)
        {
            if (_State != value)
            {
                _State = value;
                if (_StateChange != null) _StateChange(value, err);
            }
        }

        Action<DownState, string> _StateChange;
        /// <summary>
        /// 下载状态改变
        /// </summary>
        public HttpDown StateChange(Action<DownState, string> action)
        {
            _StateChange = action;
            return this;
        }

        #endregion

        #endregion

        string GetFileName(string Disposition)
        {
            if (Disposition == null) return GetFileName();
            var filename = Disposition.Substring(Disposition.ToUpper().IndexOf("filename=") + 9);
            if (filename.Contains(";"))
            {
                filename = filename.Substring(0, filename.IndexOf(";"));
                if (filename.EndsWith("\"")) filename = filename.Substring(1, filename.Length - 2);
            }
            //filename = filename.Trim('"');
            //byte[] byteArray = Encoding.GetEncoding("ISO-8859-1").GetBytes(filename);
            //string urlFilename = Encoding.GetEncoding("utf-8").GetString(byteArray);
            //urlFilename = Uri.UnescapeDataString(urlFilename);
            //fileName = urlFilename;
            return filename;
        }
        string GetFileName()
        {
            //return Uri.Segments[Uri.Segments.Length - 1];
            if (Uri.Query.Length > 0) return Path.GetFileName(Url.Substring(0, Url.Length - Uri.Query.Length));
            return Path.GetFileName(Url);
        }

        #region 初始化

        public Uri Uri = null;
        public HttpDown(string url, string _savePath)
        {
            Uri = new Uri(url);
            SavePath = _savePath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            Url = url;
        }
        public HttpCore core = null;
        public HttpDown(HttpCore _core, string _savePath)
        {
            core = _core;
            Uri = core.option.uri;
            SavePath = _savePath.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
            Url = core.option.uri.AbsoluteUri;
        }

        #endregion
    }
}