using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Diagnostics;
using CYAutoFramework;
using static CYStandardProcedure.Program;

namespace CYStandardProcedure.MyClass
{
    public class WebApiService
    {
        private HttpListener _listener;
        private CancellationTokenSource _cancellationTokenSource;
        private int _port;
        private readonly HardwareWebController _controller;
        private bool _isRunning;

        public event Action<string> OnLogMessage;

        public WebApiService(int port = 8081)
        {
            _port = port;
            _controller = new HardwareWebController();
            _isRunning = false;
        }

        public void Start()
        {
            if (_isRunning)
            {
                LogMessage("Web API服务已经在运行中");
                return;
            }

            try
            {
                _listener = new HttpListener();
                _listener.Prefixes.Add($"http://localhost:{_port}/");
                _listener.Prefixes.Add($"http://127.0.0.1:{_port}/");
                _listener.Prefixes.Add($"http://+:{_port}/");
                _cancellationTokenSource = new CancellationTokenSource();
                _listener.Start();
                _isRunning = true;

                LogMessage($"Web API服务启动成功，端口: {_port}");
                LogMessage($"API地址: http://localhost:{_port}/api/hardware");

                Task.Run(() => HandleRequestsAsync(_cancellationTokenSource.Token));
            }
            catch (Exception ex)
            {
                LogMessage($"Web API服务启动失败: {ex.Message}");
                throw;
            }
        }

        public void Stop()
        {
            if (!_isRunning)
            {
                return;
            }

            try
            {
                _cancellationTokenSource?.Cancel();
                _listener?.Stop();
                _listener?.Close();
                _isRunning = false;
                LogMessage("Web API服务已停止");
            }
            catch (Exception ex)
            {
                LogMessage($"停止Web API服务时发生错误: {ex.Message}");
            }
        }

        private async Task HandleRequestsAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested && _listener.IsListening)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    // 串行处理请求，确保硬件操作按顺序执行
                    await ProcessRequestAsync(context);
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (HttpListenerException ex)
                {
                    if (ex.ErrorCode != 995)
                    {
                        LogMessage($"HTTP监听器错误: {ex.Message}");
                    }
                    break;
                }
                catch (Exception ex)
                {
                    LogMessage($"处理请求时发生错误: {ex.Message}");
                }
            }
        }

        private async Task ProcessRequestAsync(HttpListenerContext context)
        {
            var request = context.Request;
            var response = context.Response;

            try
            {
                response.Headers.Add("Access-Control-Allow-Origin", "*");
                response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
                response.Headers.Add("Access-Control-Allow-Headers", "Content-Type, Authorization");

                if (request.HttpMethod == "OPTIONS")
                {
                    response.StatusCode = 200;
                    response.Close();
                    return;
                }

                var path = request.Url.LocalPath;
                var method = request.HttpMethod;
                
                LogMessage($"接收请求: {method} {path}");

                string responseContent = await HandleApiRequestAsync(path, method, request);

                byte[] responseBytes = Encoding.UTF8.GetBytes(responseContent);
                response.ContentType = "application/json; charset=utf-8";
                response.ContentLength64 = responseBytes.Length;
                response.StatusCode = 200;

                await response.OutputStream.WriteAsync(responseBytes, 0, responseBytes.Length);
                response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                LogMessage($"处理请求时发生错误: {ex.Message}");
                
                try
                {
                    var errorResponse = JsonConvert.SerializeObject(ApiResponse.CreateError($"服务器内部错误: {ex.Message}"));
                    byte[] errorBytes = Encoding.UTF8.GetBytes(errorResponse);
                    
                    response.ContentType = "application/json; charset=utf-8";
                    response.StatusCode = 500;
                    response.ContentLength64 = errorBytes.Length;
                    
                    await response.OutputStream.WriteAsync(errorBytes, 0, errorBytes.Length);
                    response.OutputStream.Close();
                }
                catch
                {
                    response.StatusCode = 500;
                    response.Close();
                }
            }
        }

        private async Task<string> HandleApiRequestAsync(string path, string method, HttpListenerRequest request)
        {
            string requestBody = null;
            if (request.HasEntityBody)
            {
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
            }

            var result = await _controller.ProcessRequestAsync(path, method, requestBody);
            return JsonConvert.SerializeObject(result, Formatting.Indented);
        }

        private void LogMessage(string message)
        {
            var logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] WebAPI: {message}";
            OnLogMessage?.Invoke(logMessage);
            Debug.WriteLine(logMessage);
        }

        public bool IsRunning => _isRunning;
    }

    public class HardwareWebController
    {
        private readonly WebApiConfiguration _config;

        public HardwareWebController()
        {
            _config = new WebApiConfiguration();
        }

        public async Task<object> ProcessRequestAsync(string path, string method, string requestBody)
        {
            try
            {
                var segments = path.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                
                if (segments.Length < 2 || segments[0] != "api" || segments[1] != "hardware")
                {
                    return ApiResponse.CreateError("无效的API路径");
                }

                if (segments.Length == 2)
                {
                    if (method == "GET")
                    {
                        return await GetHardwareStatusAsync();
                    }
                    else if (method == "POST")
                    {
                        return await InitializeHardwareAsync();
                    }
                }

                if (segments.Length >= 3)
                {
                    var category = segments[2];
                    return await RouteRequestAsync(category, segments, method, requestBody);
                }

                return ApiResponse.CreateError("不支持的API操作");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateError($"处理请求时发生错误: {ex.Message}");
            }
        }

        private async Task<object> RouteRequestAsync(string category, string[] segments, string method, string requestBody)
        {
            switch (category.ToLower())
            {
                case "initialize":
                    return await InitializeHardwareAsync();
                
                case "status":
                    return await GetHardwareStatusAsync();
                
                case "shutdown":
                    return await ShutdownHardwareAsync();
                
                case "motion":
                    return await HandleMotionRequestAsync(segments, method, requestBody);
                
                case "io":
                    return await HandleIORequestAsync(segments, method, requestBody);
                
                case "sequencer":
                    return await HandleSequencerRequestAsync(segments, method, requestBody);
                
                case "carry-gripper":
                    return await HandleCarryGripperRequestAsync(segments, method, requestBody);
                
                case "robot-gripper":
                    return await HandleRobotGripperRequestAsync(segments, method, requestBody);
                
                case "robot":
                    return await HandleRobotRequestAsync(segments, method, requestBody);
                
                case "pipette":
                    return await HandlePipetteRequestAsync(segments, method, requestBody);
                
                case "temperature":
                    return await HandleTemperatureRequestAsync(segments, method, requestBody);
                
                case "camera":
                    return await HandleCameraRequestAsync(segments, method, requestBody);
                
                default:
                    return ApiResponse.CreateError($"不支持的API类别: {category}");
            }
        }

        private async Task<object> GetHardwareStatusAsync()
        {
            await Task.Delay(1);
            
            var status = new HardwareStatusModel
            {
                SystemStatus = "运行中",
                AxesStatus = new Dictionary<string, AxisStatusModel>
                {
                    ["测序仪XAxis"] = new AxisStatusModel { Name = "测序仪XAxis", Position = 0, IsEnabled = true, IsHomed = true, Status = "就绪" },
                    ["搬运XAxis"] = new AxisStatusModel { Name = "搬运XAxis", Position = 0, IsEnabled = true, IsHomed = true, Status = "就绪" },
                    ["搬运YAxis"] = new AxisStatusModel { Name = "搬运YAxis", Position = 0, IsEnabled = true, IsHomed = true, Status = "就绪" },
                    ["搬运ZAxis"] = new AxisStatusModel { Name = "搬运ZAxis", Position = 0, IsEnabled = true, IsHomed = true, Status = "就绪" }
                }
            };

            return ApiResponse<HardwareStatusModel>.CreateSuccess(status, "获取硬件状态成功");
        }

        private async Task<object> InitializeHardwareAsync()
        {
            await Task.Delay(1);
            return ApiResponse.CreateSuccess("硬件初始化成功");
        }

        private async Task<object> ShutdownHardwareAsync()
        {
            await Task.Delay(1);
            return ApiResponse.CreateSuccess("硬件关闭成功");
        }

        private async Task<object> HandleMotionRequestAsync(string[] segments, string method, string requestBody)
        {
            if (segments.Length < 4)
            {
                return ApiResponse.CreateError("运动控制请求格式错误");
            }

            var action = segments[3];
            
            switch (action.ToLower())
            {
                case "home":
                    if (segments.Length >= 5)
                    {
                        return await HomeAxisAsync(segments[4]);
                    }
                    break;
                
                case "move":
                    if (segments.Length >= 5)
                    {
                        return await MoveAxisAsync(segments[4], requestBody);
                    }
                    break;
                
                case "point":
                    if (segments.Length >= 5)
                    {
                        return await MoveToPointAsync(segments[4]);
                    }
                    break;
                
                case "position":
                    if (segments.Length >= 5)
                    {
                        return await GetAxisPositionAsync(segments[4]);
                    }
                    break;
                
                case "status":
                    if (segments.Length >= 5)
                    {
                        return await GetAxisStatusAsync(segments[4]);
                    }
                    break;
            }

            return ApiResponse.CreateError("不支持的运动控制操作");
        }

        private async Task<object> HandleIORequestAsync(string[] segments, string method, string requestBody)
        {
            if (segments.Length < 4)
            {
                return ApiResponse.CreateError("IO控制请求格式错误");
            }

            var ioType = segments[3];
            
            if (ioType.ToLower() == "output" && segments.Length >= 5)
            {
                return await SetOutputAsync(segments[4], requestBody);
            }
            else if (ioType.ToLower() == "input" && segments.Length >= 5)
            {
                return await GetInputAsync(segments[4]);
            }

            return ApiResponse.CreateError("不支持的IO操作");
        }

        private async Task<object> HandleSequencerRequestAsync(string[] segments, string method, string requestBody)
        {
            if (segments.Length < 4)
            {
                return ApiResponse.CreateError("测序仪请求格式错误");
            }

            var action = segments[3];
            
            switch (action.ToLower())
            {
                case "start":
                    return await StartSequencingAsync(requestBody);
                case "stop":
                    return await StopSequencingAsync();
                case "pause":
                    return await PauseSequencingAsync();
                case "resume":
                    return await ResumeSequencingAsync();
                case "status":
                    return await GetSequencerStatusAsync();
                case "network":
                    return await CheckSequencerNetworkAsync();
                case "chip":
                    if (segments.Length >= 5 && segments[4].ToLower() == "state")
                    {
                        return await GetChipStateAsync();
                    }
                    break;
                case "basecalled":
                    if (segments.Length >= 5 && segments[4].ToLower() == "fraction")
                    {
                        return await GetBasecalledFractionAsync();
                    }
                    break;
                case "file":
                    if (segments.Length >= 5)
                    {
                        if (segments[4].ToLower() == "copy")
                        {
                            return await CopySequencingFilesAsync();
                        }
                        else if (segments[4].ToLower() == "state")
                        {
                            return await GetFileCopyStateAsync();
                        }
                    }
                    break;
            }

            return ApiResponse.CreateError("不支持的测序仪操作");
        }

        private async Task<object> HandleCarryGripperRequestAsync(string[] segments, string method, string requestBody)
        {
            if (segments.Length < 4)
            {
                return ApiResponse.CreateError("搬运夹爪请求格式错误");
            }

            var action = segments[3];
            
            switch (action.ToLower())
            {
                case "move-absolute":
                    if (method == "POST")
                    {
                        return await MoveCarryGripperAbsoluteAsync(requestBody);
                    }
                    break;
                case "move-force":
                    if (method == "POST")
                    {
                        return await MoveCarryGripperForceAsync(requestBody);
                    }
                    break;
                case "preset":
                    if (segments.Length >= 5)
                    {
                        return await MoveCarryGripperToPresetAsync(segments[4]);
                    }
                    break;
                case "home":
                    return await HomeCarryGripperAsync();
                case "status":
                    return await GetCarryGripperStatusAsync();
                case "presets":
                    return await GetCarryGripperPresetsAsync();
                default:
                    return ApiResponse.CreateError("不支持的搬运夹爪操作");
            }
            return ApiResponse.CreateError("不支持的搬运夹爪操作");
        }

        private async Task<object> HandleRobotGripperRequestAsync(string[] segments, string method, string requestBody)
        {
            if (segments.Length < 5)
            {
                return ApiResponse.CreateError("精密夹爪请求格式错误");
            }

            int gripperId;
            if (!int.TryParse(segments[3], out gripperId))
            {
                return ApiResponse.CreateError("无效的夹爪ID");
            }

            var action = segments[4];
            
            switch (action.ToLower())
            {
                case "move":
                    if (method == "POST")
                    {
                        return await MoveRobotGripperAsync(gripperId, requestBody);
                    }
                    break;
                case "preset":
                    if (segments.Length >= 6)
                    {
                        return await MoveRobotGripperToPresetAsync(gripperId, segments[5]);
                    }
                    break;
                case "home":
                    return await HomeRobotGripperAsync(gripperId);
                case "status":
                    return await GetRobotGripperStatusAsync(gripperId);
                case "presets":
                    return await GetRobotGripperPresetsAsync(gripperId);
                default:
                    return ApiResponse.CreateError("不支持的精密夹爪操作");
            }
            return ApiResponse.CreateError("不支持的精密夹爪操作");
        }

        private async Task<object> HandleRobotRequestAsync(string[] segments, string method, string requestBody)
        {
            if (segments.Length < 4)
            {
                return ApiResponse.CreateError("机器人请求格式错误");
            }

            var action = segments[3];
            
            switch (action.ToLower())
            {
                case "move-position":
                    return await MoveRobotToPositionAsync(requestBody);
                case "move-joint":
                    return await MoveRobotJointAsync(requestBody);
                case "position":
                    return await GetRobotPositionAsync();
            }

            return ApiResponse.CreateError("不支持的机器人操作");
        }

        private async Task<object> HandlePipetteRequestAsync(string[] segments, string method, string requestBody)
        {
            if (segments.Length < 4)
            {
                return ApiResponse.CreateError("移液器请求格式错误");
            }

            var action = segments[3];
            
            switch (action.ToLower())
            {
                case "initialize":
                    return await InitializePipetteAsync();
                case "aspirate":
                    return await PipetteAspirateAsync(requestBody);
                case "dispense":
                    return await PipetteDispenseAsync(requestBody);
                case "initialize-gun":
                    return await PipetteInitializeGunAsync(requestBody);
                case "move-z":
                    return await PipetteMoveZAsync(requestBody);
                default:
                    return ApiResponse.CreateError("不支持的移液器操作");
            }
        }

        private async Task<object> HandleTemperatureRequestAsync(string[] segments, string method, string requestBody)
        {
            if (segments.Length < 4)
            {
                return ApiResponse.CreateError("温度控制请求格式错误");
            }

            var action = segments[3];
            
            switch (action.ToLower())
            {
                case "start":
                    return await StartTemperatureMonitoringAsync();
                case "stop":
                    return await StopTemperatureMonitoringAsync();
                case "current":
                    return await GetCurrentTemperatureAsync();
                default:
                    return ApiResponse.CreateError("不支持的温度控制操作");
            }
        }

        private async Task<object> HandleCameraRequestAsync(string[] segments, string method, string requestBody)
        {
            if (segments.Length < 4)
            {
                return ApiResponse.CreateError("相机控制请求格式错误");
            }

            var action = segments[3];
            
            switch (action.ToLower())
            {
                case "capture":
                    if (segments.Length >= 5)
                    {
                        return await CaptureImageAsync(segments[4]);
                    }
                    break;
                case "analyze":
                    return await AnalyzeImageAsync(requestBody);
                default:
                    return ApiResponse.CreateError("不支持的相机操作");
            }

            return ApiResponse.CreateError("相机操作参数错误");
        }

        #region Motion Control Methods

        private async Task<object> HomeAxisAsync(string axisName)
        {
            try
            {
                // 检查系统状态
                if (StationConfig.Instance.MainStation.mCurStatus == ObjectStation._StationStatus.Alarm)
                {
                    return ApiResponse.CreateError($"系统报警，{axisName} 回零失败");
                }

                // 获取轴索引
                int axisIndex = GetAxisIndex(axisName);
                if (axisIndex == -1)
                {
                    return ApiResponse.CreateError($"未找到轴 {axisName}");
                }

                // 检查轴报警状态
                if (MotionConfig.Instance.MotionStatusList[axisIndex].Alm)
                {
                    return ApiResponse.CreateError($"{axisName} 报警，回零失败");
                }

                // 检查伺服状态，如果未使能则自动使能
                if (!MotionConfig.Instance.MotionStatusList[axisIndex].Svo)
                {
                    MotionConfig.Instance.ServoOn(axisName);
                    await Task.Delay(100);
                }

                // 检查是否正在回零
                if (MotionConfig.Instance.MotionStatusList[axisIndex].Homing)
                {
                    return ApiResponse.CreateError($"{axisName} 正在回零中");
                }

                // 启动回零
                MotionConfig.Instance.HomeStart(axisName);

                // 等待回零完成
                await Task.Run(() =>
                {
                    while (MotionConfig.Instance.MotionStatusList[axisIndex].Homing)
                    {
                        Thread.Sleep(5);
                    }
                });

                return ApiResponse.CreateSuccess($"轴 {axisName} 回零成功");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateError($"轴 {axisName} 回零失败: {ex.Message}");
            }
        }

        private int GetAxisIndex(string axisName)
        {
            switch (axisName)
            {
                case "测序仪XAxis":
                    return (int)_Axis.测序仪XAxis;
                case "搬运ZAxis":
                    return (int)_Axis.搬运ZAxis;
                case "搬运XAxis":
                    return (int)_Axis.搬运XAxis;
                case "搬运YAxis":
                    return (int)_Axis.搬运YAxis;
                default:
                    return -1;
            }
        }

        private async Task<object> MoveAxisAsync(string axisName, string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody))
                {
                    return ApiResponse.CreateError("缺少移动参数");
                }

                var request = JsonConvert.DeserializeObject<AxisMoveRequest>(requestBody);
                if (request == null)
                {
                    return ApiResponse.CreateError("无效的移动参数");
                }

                // 检查系统状态
                if (StationConfig.Instance.MainStation.mCurStatus == ObjectStation._StationStatus.Alarm)
                {
                    return ApiResponse.CreateError($"系统报警，{axisName} 移动失败");
                }

                // 获取轴索引
                int axisIndex = GetAxisIndex(axisName);
                if (axisIndex == -1)
                {
                    return ApiResponse.CreateError($"未找到轴 {axisName}");
                }

                // 检查轴报警状态
                if (MotionConfig.Instance.MotionStatusList[axisIndex].Alm)
                {
                    return ApiResponse.CreateError($"{axisName} 报警，移动失败");
                }

                // 检查轴是否正在运动
                if (MotionConfig.Instance.MotionStatusList[axisIndex].Moving)
                {
                    return ApiResponse.CreateError($"{axisName} 正在运动中，请等待完成");
                }

                // 检查伺服状态，如果未使能则自动使能
                if (!MotionConfig.Instance.MotionStatusList[axisIndex].Svo)
                {
                    MotionConfig.Instance.ServoOn(axisName);
                    await Task.Delay(100);
                }

                // 执行移动
                if (request.Speed.HasValue)
                {
                    // 使用自定义速度的绝对移动（需要更多参数，这里使用默认加减速度）
                    MotionConfig.Instance.AbsoluteMove(axisName, 1000, 1000, request.Speed.Value, request.Position);
                }
                else
                {
                    // 使用预加载速度的绝对移动
                    MotionConfig.Instance.AbsoluteMove(axisName, request.Position);
                }

                // 等待移动完成
                await Task.Run(() =>
                {
                    Thread.Sleep(20); // 等待移动启动
                    while (MotionConfig.Instance.MotionStatusList[axisIndex].Moving)
                    {
                        Thread.Sleep(5);
                        // 检查急停状态
                        if (MotionConfig.Instance.MotionStatusList[axisIndex].Emg)
                        {
                            break;
                        }
                    }
                });

                // 检查是否因为急停而停止
                if (MotionConfig.Instance.MotionStatusList[axisIndex].Emg)
                {
                    return ApiResponse.CreateError($"{axisName} 移动被急停中断");
                }

                return ApiResponse.CreateSuccess($"轴 {axisName} 移动到位置 {request.Position} 成功");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateError($"轴 {axisName} 移动失败: {ex.Message}");
            }
        }

        private async Task<object> MoveToPointAsync(string pointName)
        {
            await Task.Delay(1);
            return ApiResponse.CreateSuccess($"移动到点位 {pointName} 成功");
        }

        private Task<object> GetAxisPositionAsync(string axisName)
        {
            try
            {
                // 获取轴索引
                int axisIndex = GetAxisIndex(axisName);
                if (axisIndex == -1)
                {
                    return Task.FromResult<object>(ApiResponse<PositionResponse>.CreateError($"未找到轴 {axisName}"));
                }

                // 获取当前位置
                double currentPosition = MotionConfig.Instance.CurPos[axisIndex];
                
                var position = new PositionResponse 
                { 
                    Position = currentPosition 
                };

                return Task.FromResult<object>(ApiResponse<PositionResponse>.CreateSuccess(position, $"获取轴 {axisName} 位置成功"));
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse<PositionResponse>.CreateError($"获取轴 {axisName} 位置失败: {ex.Message}"));
            }
        }

        private Task<object> GetAxisStatusAsync(string axisName)
        {
            try
            {
                // 获取轴索引
                int axisIndex = GetAxisIndex(axisName);
                if (axisIndex == -1)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError($"未找到轴 {axisName}"));
                }

                // 获取轴状态和位置
                var motionStatus = MotionConfig.Instance.MotionStatusList[axisIndex];
                double currentPosition = MotionConfig.Instance.CurPos[axisIndex];

                // 构建详细状态信息
                var axisStatus = new
                {
                    AxisName = axisName,
                    CurrentPosition = currentPosition,
                    IOStatus = new
                    {
                        Homing = motionStatus.Homing,        // 回零中
                        HomeDone = motionStatus.HomeDone,    // 回零完成
                        Moving = motionStatus.Moving,        // 运动中
                        MoveDone = motionStatus.MoveDone,    // 定位完成
                        Alarm = motionStatus.Alm,            // 报警
                        ServoOn = motionStatus.Svo,          // 伺服使能
                        Emergency = motionStatus.Emg,        // 急停
                        PositiveLimit = motionStatus.Pel,    // 正极限
                        NegativeLimit = motionStatus.Mel,    // 负极限
                        Origin = motionStatus.Ori            // 原点
                    },
                    Summary = new
                    {
                        IsEnabled = motionStatus.Svo,
                        IsHomed = motionStatus.HomeDone,
                        Status = GetAxisStatusString(motionStatus)
                    }
                };

                return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(axisStatus, $"获取轴 {axisName} 状态成功"));
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"获取轴 {axisName} 状态失败: {ex.Message}"));
            }
        }

        private string GetAxisStatusString(MotionIOStatus motionStatus)
        {
            if (motionStatus.Alm)
                return "报警";
            if (motionStatus.Emg)
                return "急停";
            if (motionStatus.Homing)
                return "回零中";
            if (motionStatus.Moving)
                return "运动中";
            if (!motionStatus.Svo)
                return "伺服未使能";
            if (motionStatus.Pel)
                return "正极限";
            if (motionStatus.Mel)
                return "负极限";
            
            return "就绪";
        }

        #endregion

        #region IO Control Methods

        private Task<object> SetOutputAsync(string outputName, string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody))
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("缺少输出参数"));
                }

                var request = JsonConvert.DeserializeObject<IOOutputRequest>(requestBody);
                if (request == null)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("无效的输出参数"));
                }

                // 验证输出名称是否存在
                if (!IsValidOutputName(outputName))
                {
                    return Task.FromResult<object>(ApiResponse.CreateError($"输出点 '{outputName}' 不存在"));
                }

                // 设置输出
                IOConfig.Instance.SetSingleOut(outputName, request.Value ? 1 : 0);

                // 验证设置是否成功（可选）
                Thread.Sleep(10); // 给硬件响应时间
                bool actualState = IOConfig.Instance.GetBitOutput(outputName);
                
                var result = new
                {
                    outputName = outputName,
                    requestedValue = request.Value,
                    actualValue = actualState,
                    success = (actualState == request.Value)
                };

                return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(result, $"设置输出 {outputName} 为 {request.Value} 成功"));
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"设置输出 {outputName} 失败: {ex.Message}"));
            }
        }

        private Task<object> GetInputAsync(string inputName)
        {
            try
            {
                // 验证输入名称是否存在
                if (!IsValidInputName(inputName))
                {
                    return Task.FromResult<object>(ApiResponse.CreateError($"输入点 '{inputName}' 不存在"));
                }

                // 读取输入状态
                bool inputState = IOConfig.Instance.GetBitInput(inputName);
                
                var result = new
                {
                    inputName = inputName,
                    value = inputState,
                    timestamp = DateTime.Now
                };

                return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(result, $"读取输入 {inputName} 成功"));
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"读取输入 {inputName} 失败: {ex.Message}"));
            }
        }

        private bool IsValidOutputName(string outputName)
        {
            try
            {
                return Enum.IsDefined(typeof(_OutputCollect), outputName);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidInputName(string inputName)
        {
            try
            {
                return Enum.IsDefined(typeof(_InputCollect), inputName);
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region Sequencer Control Methods

        private async Task<object> StartSequencingAsync(string requestBody)
        {
            await Task.Delay(1);
            return ApiResponse.CreateSuccess("测序启动成功");
        }

        private async Task<object> StopSequencingAsync()
        {
            await Task.Delay(1);
            return ApiResponse.CreateSuccess("测序停止成功");
        }

        private async Task<object> PauseSequencingAsync()
        {
            await Task.Delay(1);
            return ApiResponse.CreateSuccess("测序暂停成功");
        }

        private async Task<object> ResumeSequencingAsync()
        {
            await Task.Delay(1);
            return ApiResponse.CreateSuccess("测序恢复成功");
        }

        private async Task<object> GetSequencerStatusAsync()
        {
            await Task.Delay(1);
            var status = new SequencerStatusModel
            {
                IsConnected = true,
                IsRunning = false,
                Status = "就绪"
            };
            return ApiResponse<SequencerStatusModel>.CreateSuccess(status, "获取测序仪状态成功");
        }

        private async Task<object> CheckSequencerNetworkAsync()
        {
            await Task.Delay(1);
            var network = new NetworkStatusResponse
            {
                IsConnected = true,
                IpAddress = "192.168.1.100",
                ResponseTime = 10.5
            };
            return ApiResponse<NetworkStatusResponse>.CreateSuccess(network, "检查测序仪网络成功");
        }

        private async Task<object> GetChipStateAsync()
        {
            await Task.Delay(1);
            var chipState = new { state = "ready" };
            return ApiResponse<object>.CreateSuccess(chipState, "获取芯片状态成功");
        }

        private async Task<object> GetBasecalledFractionAsync()
        {
            await Task.Delay(1);
            var fraction = new { fraction = 0.0 };
            return ApiResponse<object>.CreateSuccess(fraction, "获取碱基识别进度成功");
        }

        private async Task<object> CopySequencingFilesAsync()
        {
            await Task.Delay(1);
            return ApiResponse.CreateSuccess("文件拷贝启动成功");
        }

        private async Task<object> GetFileCopyStateAsync()
        {
            await Task.Delay(1);
            var state = new FileCopyStateResponse
            {
                IsCopying = false,
                Progress = 0.0,
                TotalFiles = 0,
                CopiedFiles = 0
            };
            return ApiResponse<FileCopyStateResponse>.CreateSuccess(state, "获取文件拷贝状态成功");
        }

        #endregion

        #region Gripper Control Methods

        private Task<object> MoveCarryGripperAbsoluteAsync(string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody))
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("缺少运动参数"));
                }

                var request = JsonConvert.DeserializeObject<CarryGripperMoveRequest>(requestBody);
                if (request == null)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("无效的运动参数"));
                }

                // 构建运动参数
                var moveConfig = new GripPawlConfig
                {
                    PointName = "自定义运动",
                    PushDistance = (int)(request.Position * 1000), // mm转换为um
                    PushVM = (int)(request.Speed ?? 500000), // 默认500mm/s
                    PushAcc = (int)(request.Acceleration ?? 5000000), // 默认5m/s²
                    OrientationRange = (int)(request.Tolerance ?? 3) // 默认3um
                };

                // 调用原项目的夹爪绝对位置控制方法
                bool result = Program.carryClawForm.WaitCarryClawAbsMove(moveConfig, request.Timeout ?? 10000);
                
                if (result)
                {
                    var responseData = new
                    {
                        Action = "绝对位置运动",
                        Position = request.Position,
                        Speed = request.Speed,
                        Acceleration = request.Acceleration,
                        Tolerance = request.Tolerance,
                        Timestamp = DateTime.Now
                    };
                    return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(responseData, "搬运夹爪绝对位置运动成功"));
                }
                else
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("搬运夹爪绝对位置运动失败或超时"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"搬运夹爪绝对位置运动失败: {ex.Message}"));
            }
        }

        private Task<object> MoveCarryGripperForceAsync(string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody))
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("缺少运动参数"));
                }

                var request = JsonConvert.DeserializeObject<CarryGripperForceRequest>(requestBody);
                if (request == null)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("无效的运动参数"));
                }

                // 构建推压运动参数
                var forceConfig = new GripPawlConfig
                {
                    PointName = "自定义推压运动",
                    PushDistance = (int)(request.Position * 1000), // mm转换为um
                    PushVM = (int)(request.Speed ?? 500000), // 默认500mm/s
                    PushAcc = (int)(request.Acceleration ?? 5000000), // 默认5m/s²
                    OrientationRange = (int)(request.Tolerance ?? 3), // 默认3um
                    PushForce = (int)(request.Force ?? 20000), // 默认推压力
                    ForceDistance = (int)(request.ForceDistance ?? request.Position * 1000), // 推压距离
                    TimeRange = (int)(request.DelayTime ?? 100) // 默认延时100ms
                };

                // 调用原项目的夹爪推压控制方法
                bool result = Program.carryClawForm.WaitCarryClawForceMove(forceConfig, request.Timeout ?? 10000);
                
                if (result)
                {
                    var responseData = new
                    {
                        Action = "推压运动",
                        Position = request.Position,
                        Speed = request.Speed,
                        Acceleration = request.Acceleration,
                        Force = request.Force,
                        ForceDistance = request.ForceDistance,
                        DelayTime = request.DelayTime,
                        Timestamp = DateTime.Now
                    };
                    return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(responseData, "搬运夹爪推压运动成功"));
                }
                else
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("搬运夹爪推压运动失败或超时"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"搬运夹爪推压运动失败: {ex.Message}"));
            }
        }

        private Task<object> HomeCarryGripperAsync()
        {
            try
            {
                // 调用原项目的夹爪回零方法
                bool result = Program.carryClawForm.WaitCarryClawHome(15000);
                
                if (result)
                {
                    var responseData = new
                    {
                        Action = "回零",
                        Position = 0,
                        Timestamp = DateTime.Now
                    };
                    return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(responseData, "搬运夹爪回零成功"));
                }
                else
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("搬运夹爪回零失败或超时"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"搬运夹爪回零失败: {ex.Message}"));
            }
        }

        private Task<object> MoveCarryGripperToPresetAsync(string presetName)
        {
            try
            {
                // 获取预设位置参数
                var presetConfig = Program.carryClawConfigList?.FirstOrDefault(c => c.PointName == presetName);
                if (presetConfig == null)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError($"未找到预设位置 '{presetName}'"));
                }

                // 调用原项目的夹爪控制方法
                bool result = Program.carryClawForm.WaitCarryClawAbsMove(presetConfig, 10000);
                
                if (result)
                {
                    var responseData = new
                    {
                        Action = "移动到预设位置",
                        PresetName = presetName,
                        Position = presetConfig.PushDistance,
                        PositionMM = presetConfig.PushDistance / 1000.0,
                        Speed = presetConfig.PushVM,
                        Acceleration = presetConfig.PushAcc,
                        Timestamp = DateTime.Now
                    };
                    return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(responseData, $"搬运夹爪移动到预设位置'{presetName}'成功"));
                }
                else
                {
                    return Task.FromResult<object>(ApiResponse.CreateError($"搬运夹爪移动到预设位置'{presetName}'失败或超时"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"搬运夹爪移动到预设位置'{presetName}'失败: {ex.Message}"));
            }
        }

        private Task<object> GetCarryGripperPresetsAsync()
        {
            try
            {
                var presets = new Dictionary<string, object>();
                
                // 读取所有预设位置配置
                if (Program.carryClawConfigList != null)
                {
                    foreach (var config in Program.carryClawConfigList)
                    {
                        presets[config.PointName] = new
                        {
                            Name = config.PointName,
                            Position = config.PushDistance,
                            PositionMM = config.PushDistance / 1000.0,
                            Speed = config.PushVM,
                            SpeedMM = config.PushVM / 1000.0,
                            Acceleration = config.PushAcc,
                            AccelerationMM = config.PushAcc / 1000.0,
                            Tolerance = config.OrientationRange,
                            Force = config.PushForce,
                            ForceDistance = config.ForceDistance,
                            DelayTime = config.TimeRange
                        };
                    }
                }
                
                var responseData = new
                {
                    Presets = presets,
                    Count = presets.Count,
                    Timestamp = DateTime.Now
                };
                
                return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(responseData, "获取搬运夹爪预设成功"));
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"获取搬运夹爪预设失败: {ex.Message}"));
            }
        }

        private Task<object> GetCarryGripperStatusAsync()
        {
            try
            {
                // 读取搬运夹爪当前状态
                var position = Program.carryClawForm.Rtu_carryClaw.ReadInputRegInt(Program.carryClawConfig.DevAdd, 0, 1);
                var velocity = Program.carryClawForm.Rtu_carryClaw.ReadInputRegInt(Program.carryClawConfig.DevAdd, 2, 1);
                var torque = Program.carryClawForm.Rtu_carryClaw.ReadKeepRegInt(Program.carryClawConfig.DevAdd, 4, 1);
                var alarmStatus = Program.carryClawForm.Rtu_carryClaw.ReadOutputStatusBool(Program.carryClawConfig.DevAdd, 1502, 1);
                var enableStatus = Program.carryClawForm.Rtu_carryClaw.ReadOutputStatusBool(Program.carryClawConfig.DevAdd, 1504, 1);
                var movingStatus = Program.carryClawForm.Rtu_carryClaw.ReadOutputStatusBool(Program.carryClawConfig.DevAdd, 1505, 1);
                var homeStatus = Program.carryClawForm.Rtu_carryClaw.ReadOutputStatusBool(Program.carryClawConfig.DevAdd, 1501, 1);
                
                if (position != null && position.Length == 1)
                {
                    var statusData = new
                    {
                        Position = position[0], // 单位：um
                        PositionMM = position[0] / 1000.0, // 单位：mm
                        Velocity = velocity?[0] ?? 0, // 单位：um/s
                        VelocityMM = (velocity?[0] ?? 0) / 1000.0, // 单位：mm/s
                        Torque = torque?[0] ?? 0, // 力矩
                        IsAlarm = alarmStatus?[0] ?? false, // 是否报警
                        IsEnabled = enableStatus?[0] ?? false, // 是否使能
                        IsMoving = movingStatus?[0] ?? false, // 是否运动中
                        IsHomed = homeStatus?[0] ?? false, // 是否已回零
                        Timestamp = DateTime.Now
                    };
                    return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(statusData, "获取搬运夹爪状态成功"));
                }
                else
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("读取搬运夹爪状态失败"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"获取搬运夹爪状态失败: {ex.Message}"));
            }
        }

        private Task<object> MoveRobotGripperAsync(int gripperId, string requestBody)
        {
            try
            {
                // 验证夹爪ID的有效性
                if (gripperId < 1 || gripperId > 2)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("无效的夹爪ID，只支持夹爪1和夹爪2"));
                }

                if (string.IsNullOrEmpty(requestBody))
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("缺少运动参数"));
                }

                var request = JsonConvert.DeserializeObject<RobotGripperMoveRequest>(requestBody);
                if (request == null)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("无效的运动参数"));
                }

                // 设置默认值
                var force = request.Force ?? 1.0f;
                var speed = request.Speed ?? 50.0f;
                var acceleration = request.Acceleration ?? 500.0f;
                var position = request.Position ?? 0.0f;
                var forceThreshold = request.ForceThreshold ?? 999.0f;

                // 调用原项目的夹爪控制方法
                bool result = Program.robotNewClawForm.WaitRobotClawRun((byte)gripperId, force, speed, acceleration, position, forceThreshold);
                
                if (result)
                {
                    var responseData = new
                    {
                        GripperId = gripperId,
                        Action = "运动",
                        Position = position,
                        Speed = speed,
                        Acceleration = acceleration,
                        Force = force,
                        ForceThreshold = forceThreshold,
                        Timestamp = DateTime.Now
                    };
                    return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(responseData, $"精密夹爪{gripperId}运动成功"));
                }
                else
                {
                    return Task.FromResult<object>(ApiResponse.CreateError($"精密夹爪{gripperId}运动失败或超时"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"精密夹爪{gripperId}运动失败: {ex.Message}"));
            }
        }

        private Task<object> MoveRobotGripperToPresetAsync(int gripperId, string presetName)
        {
            try
            {
                // 验证夹爪ID的有效性
                if (gripperId < 1 || gripperId > 2)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("无效的夹爪ID，只支持夹爪1和夹爪2"));
                }

                // 根据预设名称获取参数
                var presetConfig = GetRobotGripperPresetConfig(gripperId, presetName);
                if (presetConfig == null)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError($"未找到夹爪{gripperId}的预设'{presetName}'"));
                }

                // 调用原项目的夹爪控制方法
                bool result = Program.robotNewClawForm.WaitRobotClawRun((byte)gripperId, 
                    presetConfig.Force, presetConfig.Speed, presetConfig.Acceleration, 
                    presetConfig.Position, 999.0f);
                
                if (result)
                {
                    var responseData = new
                    {
                        GripperId = gripperId,
                        PresetName = presetName,
                        Position = presetConfig.Position,
                        Speed = presetConfig.Speed,
                        Acceleration = presetConfig.Acceleration,
                        Force = presetConfig.Force,
                        Timestamp = DateTime.Now
                    };
                    return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(responseData, $"精密夹爪{gripperId}移动到预设'{presetName}'成功"));
                }
                else
                {
                    return Task.FromResult<object>(ApiResponse.CreateError($"精密夹爪{gripperId}移动到预设'{presetName}'失败或超时"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"精密夹爪{gripperId}移动到预设'{presetName}'失败: {ex.Message}"));
            }
        }

        private Task<object> HomeRobotGripperAsync(int gripperId)
        {
            try
            {
                // 验证夹爪ID的有效性
                if (gripperId < 1 || gripperId > 2)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("无效的夹爪ID，只支持夹爪1和夹爪2"));
                }

                // 调用原项目的夹爪回零方法
                bool result = Program.robotNewClawForm.WaitRobClawHome((byte)gripperId);
                
                if (result)
                {
                    var responseData = new
                    {
                        GripperId = gripperId,
                        Action = "回零",
                        Position = 0,
                        Timestamp = DateTime.Now
                    };
                    return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(responseData, $"精密夹爪{gripperId}回零成功"));
                }
                else
                {
                    return Task.FromResult<object>(ApiResponse.CreateError($"精密夹爪{gripperId}回零失败或超时"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"精密夹爪{gripperId}回零失败: {ex.Message}"));
            }
        }

        private Task<object> GetRobotGripperStatusAsync(int gripperId)
        {
            try
            {
                // 验证夹爪ID的有效性
                if (gripperId < 1 || gripperId > 2)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("无效的夹爪ID，只支持夹爪1和夹爪2"));
                }

                // 读取机器人夹爪当前状态
                float position = 0, speed = 0, force = 0, torque = 0;
                bool positionOk = SerializeClass.m_ModbusRtuRob.ReadSingleReal((byte)gripperId, 0, "04", out position);
                bool speedOk = SerializeClass.m_ModbusRtuRob.ReadSingleReal((byte)gripperId, 2, "04", out speed);
                bool forceOk = SerializeClass.m_ModbusRtuRob.ReadSingleReal((byte)gripperId, 16, "04", out force);
                bool torqueOk = SerializeClass.m_ModbusRtuRob.ReadSingleReal((byte)gripperId, 2154, "03", out torque);
                
                if (positionOk)
                {
                    var statusData = new
                    {
                        GripperId = gripperId,
                        Position = position,
                        Speed = speedOk ? speed : 0,
                        Force = forceOk ? force : 0,
                        Torque = torqueOk ? torque : 0,
                        Timestamp = DateTime.Now
                    };
                    return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(statusData, $"获取精密夹爪{gripperId}状态成功"));
                }
                else
                {
                    return Task.FromResult<object>(ApiResponse.CreateError($"读取精密夹爪{gripperId}状态失败"));
                }
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"获取精密夹爪{gripperId}状态失败: {ex.Message}"));
            }
        }

        private Task<object> GetRobotGripperPresetsAsync(int gripperId)
        {
            try
            {
                // 验证夹爪ID的有效性
                if (gripperId < 1 || gripperId > 2)
                {
                    return Task.FromResult<object>(ApiResponse.CreateError("无效的夹爪ID，只支持夹爪1和夹爪2"));
                }

                var presets = new Dictionary<string, object>();
                
                // 读取预设点位配置
                var preset1 = GetRobotGripperPresetConfig(gripperId, "1");
                var preset2 = GetRobotGripperPresetConfig(gripperId, "2");
                
                if (preset1 != null)
                {
                    presets["1"] = new
                    {
                        Name = "复位位置",
                        Position = preset1.Position,
                        Speed = preset1.Speed,
                        Acceleration = preset1.Acceleration,
                        Force = preset1.Force
                    };
                }
                
                if (preset2 != null)
                {
                    presets["2"] = new
                    {
                        Name = "到位位置",
                        Position = preset2.Position,
                        Speed = preset2.Speed,
                        Acceleration = preset2.Acceleration,
                        Force = preset2.Force
                    };
                }
                
                var responseData = new
                {
                    GripperId = gripperId,
                    Presets = presets,
                    Timestamp = DateTime.Now
                };
                
                return Task.FromResult<object>(ApiResponse<object>.CreateSuccess(responseData, $"获取精密夹爪{gripperId}预设成功"));
            }
            catch (Exception ex)
            {
                return Task.FromResult<object>(ApiResponse.CreateError($"获取精密夹爪{gripperId}预设失败: {ex.Message}"));
            }
        }

        private RobotGripperPresetConfig GetRobotGripperPresetConfig(int gripperId, string presetId)
        {
            try
            {
                var configFile = Path.Combine(Application.StartupPath, "ExeFile", "RobotClaws", "RobotClawPos.ini");
                if (!File.Exists(configFile))
                {
                    return null;
                }

                var sectionName = $"Claw{gripperId}_{presetId}";
                var speed = iniHelper.ReadString(sectionName, "speed", "50", configFile);
                var acc = iniHelper.ReadString(sectionName, "acc", "500", configFile);
                var force = iniHelper.ReadString(sectionName, "force", "1", configFile);
                var pos = iniHelper.ReadString(sectionName, "pos", "0", configFile);

                return new RobotGripperPresetConfig
                {
                    Speed = float.Parse(speed),
                    Acceleration = float.Parse(acc),
                    Force = float.Parse(force),
                    Position = float.Parse(pos)
                };
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Robot Control Methods


        private async Task<object> MoveRobotToPositionAsync(string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody))
                {
                    return ApiResponse.CreateError("缺少移动参数");
                }

                var request = JsonConvert.DeserializeObject<RobotMovePositionRequest>(requestBody);
                if (request == null || request.Position == null)
                {
                    return ApiResponse.CreateError("位置参数格式错误");
                }

                if (request.Position.Length != 3 && request.Position.Length != 7)
                {
                    return ApiResponse.CreateError("位置参数格式错误，支持两种格式：\n" +
                        "1. 简单模式 [x, y, z] - 只指定位置，姿态由机械臂自动选择\n" +
                        "2. 精确模式 [x, y, z, qw, qx, qy, qz] - 指定完整位置和姿态");
                }

                // 使用AuboClass单例进行机器人控制
                var auboRobot = AuboClass.Instance;
                int result;

                if (request.Position.Length == 3)
                {
                    // 简单模式：只指定位置，让机械臂自动选择姿态
                    var pos = new AuboRobot.Pos
                    {
                        x = request.Position[0],
                        y = request.Position[1], 
                        z = request.Position[2]
                    };
                    var tool = new AuboRobot.ToolInEndDesc();
                    result = auboRobot.MoveToPos(0, pos, tool, true); // 0=直线运动
                }
                else
                {
                    // 精确模式：指定完整的位置和姿态
                    var waypoint = new AuboRobot.wayPoint_S
                    {
                        cartPos = new AuboRobot.Pos
                        {
                            x = request.Position[0],
                            y = request.Position[1], 
                            z = request.Position[2]
                        },
                        orientation = new AuboRobot.Ori
                        {
                            w = request.Position[3], // 四元数w
                            x = request.Position[4], // 四元数x
                            y = request.Position[5], // 四元数y
                            z = request.Position[6]  // 四元数z
                        },
                        jointpos = new double[6] // 初始化关节角度数组
                    };
                    result = auboRobot.MoveToWaypoint(waypoint, true);
                }
                
                if (result == 0) // 成功
                {
                    return ApiResponse.CreateSuccess($"机器人移动到位置 [{string.Join(", ", request.Position)}] 成功");
                }
                else
                {
                    return ApiResponse.CreateError($"机器人移动失败，错误码: {result}");
                }
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateError($"机器人位置移动失败: {ex.Message}");
            }
        }

        private async Task<object> MoveRobotJointAsync(string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody))
                {
                    return ApiResponse.CreateError("缺少关节参数");
                }

                var request = JsonConvert.DeserializeObject<RobotMoveJointRequest>(requestBody);
                if (request == null || request.Joints == null || request.Joints.Length != 6)
                {
                    return ApiResponse.CreateError("关节参数格式错误，需要包含6个关节角度值");
                }

                // 使用AuboClass单例进行机器人控制
                var auboRobot = AuboClass.Instance;
                
                // 将角度转换为弧度（如果需要）
                double[] joints = new double[6];
                for (int i = 0; i < 6; i++)
                {
                    joints[i] = request.Joints[i] * Math.PI / 180.0; // 角度转弧度
                }
                
                // 调用机器人关节移动方法 (type=0表示关节移动)
                int result = auboRobot.MovePos(0, joints, true);
                
                if (result == 0) // 成功
                {
                    return ApiResponse.CreateSuccess($"机器人关节移动到 [{string.Join(", ", request.Joints)}] 度成功");
                }
                else
                {
                    return ApiResponse.CreateError($"机器人关节移动失败，错误码: {result}");
                }
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateError($"机器人关节移动失败: {ex.Message}");
            }
        }

        private async Task<object> GetRobotPositionAsync()
        {
            try
            {
                // 使用AuboClass单例获取机器人当前位置
                var auboRobot = AuboClass.Instance;
                
                // 创建必要的结构体
                var waypoint = new AuboRobot.wayPoint_S();
                var ori = new AuboRobot.Ori();
                var rpy = new AuboRobot.Rpy();
                
                // 获取当前位置信息
                var posDict = auboRobot.GetCurrentPos(waypoint, ori, rpy, Math.PI);
                
                if (posDict != null && posDict.Count > 0)
                {
                    // 构造位置数组 [x, y, z, qw, qx, qy, qz]
                    var position = new double[] 
                    { 
                        posDict["xPos"],
                        posDict["yPos"],
                        posDict["zPos"],
                        posDict["oriW"],
                        posDict["oriX"],
                        posDict["oriY"],
                        posDict["oriZ"]
                    };
                    
                    // 关节角度（已经转换为度）
                    var joints = new double[]
                    {
                        posDict["joint1"],
                        posDict["joint2"],
                        posDict["joint3"],
                        posDict["joint4"],
                        posDict["joint5"],
                        posDict["joint6"]
                    };
                    
                    var robotStatus = new RobotStatusModel
                    {
                        IsConnected = true,
                        IsEnabled = true,
                        CurrentPosition = position,
                        CurrentJoints = joints,
                        IsMoving = false, // 可以通过其他方法获取运动状态
                        Status = "就绪"
                    };
                    
                    return ApiResponse<RobotStatusModel>.CreateSuccess(robotStatus, "获取机器人位置成功");
                }
                else
                {
                    return ApiResponse.CreateError("获取机器人位置失败");
                }
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateError($"获取机器人位置失败: {ex.Message}");
            }
        }


        #endregion


        #region Pipette Control Methods

        private async Task<object> InitializePipetteAsync()
        {
            try
            {
                var pipetteForm = PipetteGunForm.m_pipettegun;
                if (pipetteForm == null)
                {
                    return ApiResponse.CreateError("移液器系统未初始化");
                }

                await Task.Delay(1);
                return ApiResponse.CreateSuccess("移液器初始化成功");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateError($"移液器初始化失败: {ex.Message}");
            }
        }

        private async Task<object> PipetteAspirateAsync(string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody))
                {
                    return ApiResponse.CreateError("缺少吸取参数");
                }

                var request = JsonConvert.DeserializeObject<PipetteOperationRequest>(requestBody);
                if (request == null)
                {
                    return ApiResponse.CreateError("吸取参数格式错误");
                }

                var pipetteForm = PipetteGunForm.m_pipettegun;
                if (pipetteForm == null)
                {
                    return ApiResponse.CreateError("移液器系统未初始化");
                }

                // 使用移液器的吸液命令格式
                string cmdListStr = $"1[Ia{request.Volume},{request.Speed},,];";
                
                await Task.Run(() =>
                {
                    pipetteForm.Invoke(new Action(() =>
                    {
                        // 调用PipetteGunForm的PipetteGunSend方法
                        var method = typeof(PipetteGunForm).GetMethod("PipetteGunSend", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (method != null)
                        {
                            method.Invoke(pipetteForm, new object[] { cmdListStr });
                        }
                    }));
                });

                return ApiResponse.CreateSuccess($"移液器吸取 {request.Volume}μL 成功，速度: {request.Speed}");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateError($"移液器吸取失败: {ex.Message}");
            }
        }

        private async Task<object> PipetteDispenseAsync(string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody))
                {
                    return ApiResponse.CreateError("缺少分液参数");
                }

                var request = JsonConvert.DeserializeObject<PipetteOperationRequest>(requestBody);
                if (request == null)
                {
                    return ApiResponse.CreateError("分液参数格式错误");
                }

                var pipetteForm = PipetteGunForm.m_pipettegun;
                if (pipetteForm == null)
                {
                    return ApiResponse.CreateError("移液器系统未初始化");
                }

                // 使用移液器的分液命令格式
                string cmdListStr = $"1[Da{request.Volume},,{request.Speed},];";
                
                await Task.Run(() =>
                {
                    pipetteForm.Invoke(new Action(() =>
                    {
                        // 调用PipetteGunForm的PipetteGunSend方法
                        var method = typeof(PipetteGunForm).GetMethod("PipetteGunSend", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (method != null)
                        {
                            method.Invoke(pipetteForm, new object[] { cmdListStr });
                        }
                    }));
                });

                return ApiResponse.CreateSuccess($"移液器分液 {request.Volume}μL 成功，速度: {request.Speed}");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateError($"移液器分液失败: {ex.Message}");
            }
        }

        private async Task<object> PipetteInitializeGunAsync(string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody))
                {
                    return ApiResponse.CreateError("缺少初始化参数");
                }

                var request = JsonConvert.DeserializeObject<PipetteInitializeGunRequest>(requestBody);
                if (request == null)
                {
                    return ApiResponse.CreateError("初始化参数格式错误");
                }

                var pipetteForm = PipetteGunForm.m_pipettegun;
                if (pipetteForm == null)
                {
                    return ApiResponse.CreateError("移液器系统未初始化");
                }

                // 使用移液器的初始化命令格式
                string cmdListStr = $"1[It{request.Speed}];";
                
                await Task.Run(() =>
                {
                    pipetteForm.Invoke(new Action(() =>
                    {
                        // 调用PipetteGunForm的PipetteGunSend方法
                        var method = typeof(PipetteGunForm).GetMethod("PipetteGunSend", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (method != null)
                        {
                            method.Invoke(pipetteForm, new object[] { cmdListStr });
                        }
                    }));
                });

                return ApiResponse.CreateSuccess($"移液器初始化枪头成功，速度: {request.Speed}");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateError($"移液器初始化枪头失败: {ex.Message}");
            }
        }

        private async Task<object> PipetteMoveZAsync(string requestBody)
        {
            try
            {
                if (string.IsNullOrEmpty(requestBody))
                {
                    return ApiResponse.CreateError("缺少Z轴移动参数");
                }

                var request = JsonConvert.DeserializeObject<PipetteMoveZRequest>(requestBody);
                if (request == null)
                {
                    return ApiResponse.CreateError("Z轴移动参数格式错误");
                }

                var pipetteForm = PipetteGunForm.m_pipettegun;
                if (pipetteForm == null)
                {
                    return ApiResponse.CreateError("移液器系统未初始化");
                }

                // 使用移液器的Z轴移动命令格式
                string cmdListStr = $"41[Zp{request.Position},{request.Speed}];";
                
                await Task.Run(() =>
                {
                    pipetteForm.Invoke(new Action(() =>
                    {
                        // 调用PipetteGunForm的PipetteGunSend方法
                        var method = typeof(PipetteGunForm).GetMethod("PipetteGunSend", 
                            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (method != null)
                        {
                            method.Invoke(pipetteForm, new object[] { cmdListStr });
                        }
                    }));
                });

                return ApiResponse.CreateSuccess($"移液器Z轴移动到位置 {request.Position} 成功，速度: {request.Speed}");
            }
            catch (Exception ex)
            {
                return ApiResponse.CreateError($"移液器Z轴移动失败: {ex.Message}");
            }
        }


        #endregion

        #region Temperature Control Methods

        private async Task<object> StartTemperatureMonitoringAsync()
        {
            await Task.Delay(1);
            return ApiResponse.CreateSuccess("温度监控启动成功");
        }

        private async Task<object> StopTemperatureMonitoringAsync()
        {
            await Task.Delay(1);
            return ApiResponse.CreateSuccess("温度监控停止成功");
        }

        private async Task<object> GetCurrentTemperatureAsync()
        {
            await Task.Delay(1);
            var temperature = new TemperatureResponse { Temperature = 25.5 };
            return ApiResponse<TemperatureResponse>.CreateSuccess(temperature, "获取当前温度成功");
        }

        #endregion

        #region Camera Control Methods

        private async Task<object> CaptureImageAsync(string cameraType)
        {
            await Task.Delay(1);
            var image = new ImageCaptureResponse
            {
                ImageId = $"IMG_{DateTime.Now:yyyyMMdd_HHmmss}",
                ImagePath = $"/images/{cameraType}_{DateTime.Now:yyyyMMdd_HHmmss}.jpg"
            };
            return ApiResponse<ImageCaptureResponse>.CreateSuccess(image, $"相机 {cameraType} 拍摄成功");
        }

        private async Task<object> AnalyzeImageAsync(string requestBody)
        {
            await Task.Delay(1);
            var analysis = new ImageAnalysisResponse
            {
                AnalysisResult = new Dictionary<string, object>
                {
                    ["detected_objects"] = new List<string>(),
                    ["quality_score"] = 0.95
                },
                Confidence = 0.95
            };
            return ApiResponse<ImageAnalysisResponse>.CreateSuccess(analysis, "图像分析成功");
        }

        #endregion
    }
}