using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace CYStandardProcedure.MyClass
{
    public class ApiResponse<T>
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public T Data { get; set; }

        public ApiResponse()
        {
            Success = true;
            Message = "操作成功";
        }

        public ApiResponse(T data) : this()
        {
            Data = data;
        }

        public ApiResponse(string message, bool success = true) : this()
        {
            Message = message;
            Success = success;
        }

        public static ApiResponse<T> CreateSuccess(T data, string message = "操作成功")
        {
            return new ApiResponse<T>
            {
                Success = true,
                Message = message,
                Data = data
            };
        }

        public static ApiResponse<T> CreateError(string message)
        {
            return new ApiResponse<T>
            {
                Success = false,
                Message = message,
                Data = default(T)
            };
        }
    }

    public class ApiResponse : ApiResponse<object>
    {
        public ApiResponse() : base() { }
        public ApiResponse(string message, bool success = true) : base(message, success) { }

        public static ApiResponse CreateSuccess(string message = "操作成功")
        {
            return new ApiResponse(message, true);
        }

        public static ApiResponse CreateError(string message)
        {
            return new ApiResponse(message, false);
        }
    }

    public class HardwareStatusModel
    {
        [JsonProperty("system_status")]
        public string SystemStatus { get; set; }

        [JsonProperty("axes_status")]
        public Dictionary<string, AxisStatusModel> AxesStatus { get; set; }

        [JsonProperty("io_status")]
        public IOStatusModel IOStatus { get; set; }

        [JsonProperty("sequencer_status")]
        public SequencerStatusModel SequencerStatus { get; set; }

        [JsonProperty("robot_status")]
        public RobotStatusModel RobotStatus { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        public HardwareStatusModel()
        {
            AxesStatus = new Dictionary<string, AxisStatusModel>();
            IOStatus = new IOStatusModel();
            SequencerStatus = new SequencerStatusModel();
            RobotStatus = new RobotStatusModel();
            Timestamp = DateTime.Now;
        }
    }

    public class AxisStatusModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("position")]
        public double Position { get; set; }

        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("is_moving")]
        public bool IsMoving { get; set; }

        [JsonProperty("is_homed")]
        public bool IsHomed { get; set; }

        [JsonProperty("alarm_code")]
        public int AlarmCode { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class IOStatusModel
    {
        [JsonProperty("inputs")]
        public Dictionary<string, bool> Inputs { get; set; }

        [JsonProperty("outputs")]
        public Dictionary<string, bool> Outputs { get; set; }

        public IOStatusModel()
        {
            Inputs = new Dictionary<string, bool>();
            Outputs = new Dictionary<string, bool>();
        }
    }

    public class SequencerStatusModel
    {
        [JsonProperty("is_connected")]
        public bool IsConnected { get; set; }

        [JsonProperty("is_running")]
        public bool IsRunning { get; set; }

        [JsonProperty("current_sample")]
        public string CurrentSample { get; set; }

        [JsonProperty("progress")]
        public double Progress { get; set; }

        [JsonProperty("chip_state")]
        public string ChipState { get; set; }

        [JsonProperty("basecalled_fraction")]
        public double BasecalledFraction { get; set; }

        [JsonProperty("experiment_duration")]
        public int ExperimentDuration { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class RobotStatusModel
    {
        [JsonProperty("is_connected")]
        public bool IsConnected { get; set; }

        [JsonProperty("is_enabled")]
        public bool IsEnabled { get; set; }

        [JsonProperty("current_position")]
        public double[] CurrentPosition { get; set; }

        [JsonProperty("current_joints")]
        public double[] CurrentJoints { get; set; }

        [JsonProperty("is_moving")]
        public bool IsMoving { get; set; }

        [JsonProperty("gripper_status")]
        public Dictionary<int, GripperStatusModel> GripperStatus { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        public RobotStatusModel()
        {
            CurrentPosition = new double[7];
            CurrentJoints = new double[6];
            GripperStatus = new Dictionary<int, GripperStatusModel>();
        }
    }

    public class GripperStatusModel
    {
        [JsonProperty("gripper_id")]
        public int GripperId { get; set; }

        [JsonProperty("position")]
        public double Position { get; set; }

        [JsonProperty("force")]
        public double Force { get; set; }

        [JsonProperty("is_open")]
        public bool IsOpen { get; set; }

        [JsonProperty("is_moving")]
        public bool IsMoving { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }
    }

    public class AxisMoveRequest
    {
        [JsonProperty("position")]
        public double Position { get; set; }

        [JsonProperty("speed")]
        public double? Speed { get; set; }
    }

    public class RobotMovePositionRequest
    {
        [JsonProperty("position")]
        public double[] Position { get; set; }

        [JsonProperty("speed")]
        public double? Speed { get; set; }
    }

    public class RobotMoveJointRequest
    {
        [JsonProperty("joints")]
        public double[] Joints { get; set; }

        [JsonProperty("speed")]
        public double? Speed { get; set; }
    }

    public class IOOutputRequest
    {
        [JsonProperty("value")]
        public bool Value { get; set; }
    }

    public class SequencerStartRequest
    {
        [JsonProperty("sampleId")]
        public string SampleId { get; set; }

        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters { get; set; }

        public SequencerStartRequest()
        {
            Parameters = new Dictionary<string, object>();
        }
    }

    public class PipetteOperationRequest
    {
        [JsonProperty("volume")]
        public double Volume { get; set; }

        [JsonProperty("speed")]
        public double Speed { get; set; }
    }

    public class PipetteMoveZRequest
    {
        [JsonProperty("position")]
        public double Position { get; set; }

        [JsonProperty("speed")]
        public double Speed { get; set; }
    }

    public class PipetteInitializeGunRequest
    {
        [JsonProperty("speed")]
        public double Speed { get; set; }
    }

    public class ImageAnalysisRequest
    {
        [JsonProperty("imageId")]
        public string ImageId { get; set; }

        [JsonProperty("analysisType")]
        public string AnalysisType { get; set; }
    }

    public class PositionResponse
    {
        [JsonProperty("position")]
        public double Position { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        public PositionResponse()
        {
            Timestamp = DateTime.Now;
        }
    }

    public class TemperatureResponse
    {
        [JsonProperty("temperature")]
        public double Temperature { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        public TemperatureResponse()
        {
            Timestamp = DateTime.Now;
        }
    }

    public class NetworkStatusResponse
    {
        [JsonProperty("is_connected")]
        public bool IsConnected { get; set; }

        [JsonProperty("ip_address")]
        public string IpAddress { get; set; }

        [JsonProperty("response_time")]
        public double ResponseTime { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        public NetworkStatusResponse()
        {
            Timestamp = DateTime.Now;
        }
    }

    public class FileCopyStateResponse
    {
        [JsonProperty("is_copying")]
        public bool IsCopying { get; set; }

        [JsonProperty("progress")]
        public double Progress { get; set; }

        [JsonProperty("current_file")]
        public string CurrentFile { get; set; }

        [JsonProperty("total_files")]
        public int TotalFiles { get; set; }

        [JsonProperty("copied_files")]
        public int CopiedFiles { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        public FileCopyStateResponse()
        {
            Timestamp = DateTime.Now;
        }
    }

    public class ImageCaptureResponse
    {
        [JsonProperty("image_id")]
        public string ImageId { get; set; }

        [JsonProperty("image_path")]
        public string ImagePath { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        public ImageCaptureResponse()
        {
            Timestamp = DateTime.Now;
        }
    }

    public class ImageAnalysisResponse
    {
        [JsonProperty("analysis_result")]
        public Dictionary<string, object> AnalysisResult { get; set; }

        [JsonProperty("confidence")]
        public double Confidence { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        public ImageAnalysisResponse()
        {
            AnalysisResult = new Dictionary<string, object>();
            Timestamp = DateTime.Now;
        }
    }

    public class PipetteStatusResponse
    {
        [JsonProperty("is_initialized")]
        public bool IsInitialized { get; set; }

        [JsonProperty("current_volume")]
        public double CurrentVolume { get; set; }

        [JsonProperty("z_position")]
        public double ZPosition { get; set; }

        [JsonProperty("is_moving")]
        public bool IsMoving { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("timestamp")]
        public DateTime Timestamp { get; set; }

        public PipetteStatusResponse()
        {
            Timestamp = DateTime.Now;
        }
    }

    // 机器人精密夹爪相关模型
    public class RobotGripperMoveRequest
    {
        [JsonProperty("position")]
        public float? Position { get; set; }
        
        [JsonProperty("speed")]
        public float? Speed { get; set; }
        
        [JsonProperty("acceleration")]
        public float? Acceleration { get; set; }
        
        [JsonProperty("force")]
        public float? Force { get; set; }
        
        [JsonProperty("forceThreshold")]
        public float? ForceThreshold { get; set; }
    }

    public class RobotGripperPresetConfig
    {
        [JsonProperty("speed")]
        public float Speed { get; set; }
        
        [JsonProperty("acceleration")]
        public float Acceleration { get; set; }
        
        [JsonProperty("force")]
        public float Force { get; set; }
        
        [JsonProperty("position")]
        public float Position { get; set; }
    }

    // 搬运夹爪相关模型
    public class CarryGripperMoveRequest
    {
        [JsonProperty("position")]
        public double Position { get; set; }        // 目标位置 (mm)
        
        [JsonProperty("speed")]
        public double? Speed { get; set; }          // 运动速度 (mm/s)
        
        [JsonProperty("acceleration")]
        public double? Acceleration { get; set; }   // 加速度 (mm/s²)
        
        [JsonProperty("tolerance")]
        public double? Tolerance { get; set; }      // 定位容差 (um)
        
        [JsonProperty("timeout")]
        public double? Timeout { get; set; }        // 超时时间 (ms)
    }

    public class CarryGripperForceRequest
    {
        [JsonProperty("position")]
        public double Position { get; set; }        // 目标位置 (mm)
        
        [JsonProperty("speed")]
        public double? Speed { get; set; }          // 运动速度 (mm/s)
        
        [JsonProperty("acceleration")]
        public double? Acceleration { get; set; }   // 加速度 (mm/s²)
        
        [JsonProperty("tolerance")]
        public double? Tolerance { get; set; }      // 定位容差 (um)
        
        [JsonProperty("force")]
        public double? Force { get; set; }          // 推压力
        
        [JsonProperty("forceDistance")]
        public double? ForceDistance { get; set; }  // 推压距离 (mm)
        
        [JsonProperty("delayTime")]
        public double? DelayTime { get; set; }      // 延时时间 (ms)
        
        [JsonProperty("timeout")]
        public double? Timeout { get; set; }        // 超时时间 (ms)
    }
}