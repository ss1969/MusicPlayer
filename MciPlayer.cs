using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Concurrent;

namespace Null.MciPlayer
{
    public class MciPlayer : IDisposable
    {
        public enum PlaybackState
        {
            Stopped,
            Playing,
            Paused,
            Invalid = -1,
        }

        #region 外部属性
        public Form? notifyForm { get; set; } = null;
        public string DevicePath { get => longpath; }
        public string AliasName { get => aliasName; }
        #endregion

        #region 内部变量
        private string longpath = "";
        private string aliasName = "";
        private BlockingCollection<MciCommand> cmdQueue = new BlockingCollection<MciCommand>();
        private CancellationTokenSource cts = new CancellationTokenSource();
        private readonly Thread t;
        #endregion

        #region 构造函数、核心线程
        public MciPlayer(string path, Form? notifyForm = null)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File not exist.", path);

            longpath = path;
            this.notifyForm = notifyForm;
            // 必须建立STA Thread来处理mci命令
            t = new Thread(ProcessCommand);
            t.SetApartmentState(ApartmentState.STA);
            t.Start(cts.Token);
        }

        private void ProcessCommand(object? obj)
        {
            CancellationToken token = (CancellationToken)obj!;
            while (!token.IsCancellationRequested)
            {
                cmdQueue.TryTake(out MciCommand cmd, -1, cts.Token);
                int err = MciSendString(command: cmd.command, cmd.buffer ?? "", cmd.bufferSize, cmd.callback);
                if (err != 0) throw new MciException(err);
                cmd.are?.Set();
            }
        }
        #endregion

        #region 外部函数
        [DllImport("kernel32.dll", EntryPoint = "GetShortPathNameW", CharSet = CharSet.Unicode)]
        extern static short GetShortPath(string DevicePath, string buffer, int bufferSize);
        [DllImport("winmm.dll", EntryPoint = "mciSendString", CharSet = CharSet.Unicode)]
        extern static int MciSendString(string command, string buffer, int bufferSize, IntPtr callback);
        #endregion

        #region 外部函数Warper
        bool TryGetShortPath(string DevicePath, out string shortPath)
        {
            shortPath = "";
            short reqLen = GetShortPath(DevicePath, null, 0);   // 指定 null与0, 则返回需要的长度
            if (reqLen == 0) return false;

            shortPath = new string('\0', reqLen);   // 声明缓冲
            short rstLen = GetShortPath(DevicePath, shortPath, reqLen);   // 转换
            if (rstLen == 0 || rstLen == reqLen) return false;
            shortPath = shortPath.TrimEnd('\0');
            
            return true;
        }
        private string? MciSendStringWithCheckDirect(string command,
                                                     string? buffer,
                                                     int bufferSize,
                                                     IntPtr callback,
                                                     bool waitResult = false)
        {
            //Console.WriteLine("MciSendStringWithCheck: " + command);
            if (aliasName == "") return "";
            int err = MciSendString(command, buffer ?? "", bufferSize, callback);
            return err != 0 ? throw new MciException(err) : buffer;
        }
        private string? MciSendStringWithCheck(string command,
                                               string? buffer,
                                               int bufferSize,
                                               IntPtr callback,
                                               bool waitResult = false)
        {
            //Console.WriteLine("MciSendStringWithCheck: " + command);
            if (aliasName == "") return "";
            MciCommand cmd = new(command,
                                 buffer,
                                 bufferSize,
                                 callback,
                                 waitResult ? new AutoResetEvent(false) : null);
            this.cmdQueue.Add(cmd);
            if (waitResult) cmd.are!.WaitOne();
            return cmd.buffer;
        }
        #endregion

        #region 接口命令函数
        private string StatusInfo(string info)
        {
            string? buffer = new string('\0', 32);
            buffer = MciSendStringWithCheck($"status {aliasName} {info}", buffer, 32, IntPtr.Zero, true);

            return buffer!.TrimEnd('\0');
        }
        public bool SetDevicePath(string longpath)
        {
            if (aliasName != "") return false;
            this.longpath = longpath;
            return true;
        }
        public void Open()
        {
            if (!TryGetShortPath(longpath, out string shortName))
                throw new Exception("Get short path faield when initializing.");

            aliasName = $"nmci{DateTime.Now.Ticks}";
            MciSendStringWithCheck($"open \"{shortName}\" alias {aliasName}", null, 0, IntPtr.Zero);
            MciSendStringWithCheck($"set {aliasName} time format milliseconds", null, 0, IntPtr.Zero);
        }
        public void Close()
        {
            MciSendStringWithCheck($"close {aliasName}", null, 0, IntPtr.Zero);
            aliasName = "";
        }
        public void Play(bool repeat = false)
        {
            if (notifyForm != null)
            {
                MciSendStringWithCheck(
                    String.Format("play {0} notify{1}", aliasName, repeat ? " repeat" : ""),
                    null,
                    0,
                    notifyForm.Handle);
            }
            else
            {
                MciSendStringWithCheck(
                    String.Format("play {0}{1}", aliasName, repeat ? " repeat" : ""),
                    null,
                    0,
                    IntPtr.Zero);
            }
        }
        public void Resume()
        {
            MciSendStringWithCheck($"resume {aliasName}", null, 0, IntPtr.Zero);
        }
        public void Pause()
        {
            MciSendStringWithCheck($"pause {aliasName}", null, 0, IntPtr.Zero);
        }
        public void Stop()
        {
            MciSendStringWithCheck($"stop {aliasName}", null, 0, IntPtr.Zero);
        }
        public void Volume(int volume)
        {
            MciSendStringWithCheck($"setaudio {aliasName} volume to {volume}", null, 0, IntPtr.Zero);
        }
        public int GetPosition()
        {
            return Convert.ToInt32(StatusInfo("position"));
        }
        public int GetLength()
        {
            return Convert.ToInt32(StatusInfo("length"));
        }
        public PlaybackState GetState()
        {
            return StatusInfo("mode").ToLower() switch
            {
                "playing" => PlaybackState.Playing,
                "paused" => PlaybackState.Paused,
                "stopped" => PlaybackState.Stopped,
                _ => PlaybackState.Invalid,
            };
        }
        public void PlayWait()
        {
            MciSendStringWithCheck($"play {aliasName} wait", null, 0, IntPtr.Zero);
        }
        public void PlayRepeat()
        {
            MciSendStringWithCheck($"play {aliasName} repeat", null, 0, IntPtr.Zero);
        }
        public void Seek(int position)
        {
            MciSendStringWithCheck($"seek {aliasName} to {position}", null, 0, IntPtr.Zero);
        }
        public void SeekToStart()
        {
            MciSendStringWithCheck($"seek {aliasName} to start", null, 0, IntPtr.Zero);
        }
        public void SetSeekMode(bool fExact)
        {
            MciSendStringWithCheck($"set {aliasName} seek exactly {(fExact ? "on" : "off")}", null, 0, IntPtr.Zero);
        }
        public void Dispose()
        {
            if (aliasName != null) Close();
            cts.Cancel();
            t.Join();
        }
        #endregion
    }
}
