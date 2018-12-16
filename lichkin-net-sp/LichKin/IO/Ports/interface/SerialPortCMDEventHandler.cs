using System;

namespace LichKin.IO.Ports
{
    /// <summary>
    ///     串口指令事件处理器
    /// </summary>
    public interface SerialPortCMDEventHandler
    {
        /// <summary>
        ///     处理方法
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        /// <param name="eventCode">事件编码</param>
        /// <param name="cmdStatus">指令状态</param>
        /// <param name="receivedData">接收到的数据</param>
        void Handle(String sid, String portName, String eventCode, String cmdStatus, String receivedData);
    }
}
