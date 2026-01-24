using System;
using System.Threading;
using System.Threading.Tasks;
using CYStandardProcedure.MyClass;

namespace CYStandardProcedure.Station
{
    public class WebApiStation
    {
        private WebApiService _webApiService;
        private Thread _webApiThread;
        private bool _isRunning;
        private readonly object _lockObject = new object();

        public event Action<string> OnLogMessage;

        public WebApiStation()
        {
            _isRunning = false;
        }

        public void Start()
        {
            lock (_lockObject)
            {
                if (_isRunning)
                {
                    LogMessage("Web API Station已经在运行中");
                    return;
                }

                try
                {
                    var config = new WebApiConfiguration();
                    _webApiService = new WebApiService(config.Port);
                    _webApiService.OnLogMessage += LogMessage;

                    _webApiThread = new Thread(WebApiThreadMain)
                    {
                        Name = "WebApiThread",
                        IsBackground = true
                    };

                    _webApiThread.Start();
                    _isRunning = true;

                    LogMessage($"Web API Station启动成功，监听端口: {config.Port}");
                    LogMessage($"API访问地址: {config.GetApiBaseUrl()}");
                }
                catch (Exception ex)
                {
                    LogMessage($"Web API Station启动失败: {ex.Message}");
                    throw;
                }
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                if (!_isRunning)
                {
                    return;
                }

                try
                {
                    LogMessage("正在停止Web API Station...");
                    
                    _webApiService?.Stop();
                    _isRunning = false;

                    if (_webApiThread != null && _webApiThread.IsAlive)
                    {
                        if (!_webApiThread.Join(5000))
                        {
                            LogMessage("Web API线程未能正常结束，强制终止");
                            _webApiThread.Abort();
                        }
                    }

                    LogMessage("Web API Station已停止");
                }
                catch (Exception ex)
                {
                    LogMessage($"停止Web API Station时发生错误: {ex.Message}");
                }
            }
        }

        private void WebApiThreadMain()
        {
            try
            {
                LogMessage("Web API服务线程启动");
                _webApiService.Start();

                while (_isRunning)
                {
                    Thread.Sleep(1000);
                }
            }
            catch (ThreadAbortException)
            {
                LogMessage("Web API服务线程被终止");
            }
            catch (Exception ex)
            {
                LogMessage($"Web API服务线程发生错误: {ex.Message}");
            }
            finally
            {
                LogMessage("Web API服务线程结束");
            }
        }

        public void Restart()
        {
            LogMessage("重启Web API Station");
            Stop();
            Thread.Sleep(2000);
            Start();
        }

        public bool IsRunning
        {
            get 
            { 
                lock (_lockObject)
                {
                    return _isRunning && _webApiService != null && _webApiService.IsRunning;
                }
            }
        }

        public string GetStatusInfo()
        {
            lock (_lockObject)
            {
                if (!_isRunning || _webApiService == null)
                {
                    return "Web API服务: 未运行";
                }

                var config = new WebApiConfiguration();
                return $"Web API服务: 运行中 (端口: {config.Port})";
            }
        }

        private void LogMessage(string message)
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] WebApiStation: {message}";
            OnLogMessage?.Invoke(logMessage);
            System.Diagnostics.Debug.WriteLine(logMessage);
        }

        public void Dispose()
        {
            Stop();
        }
    }
}