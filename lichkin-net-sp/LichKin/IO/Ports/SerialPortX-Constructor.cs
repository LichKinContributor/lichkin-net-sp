using LichKin.Utils;
using System;
using System.Collections.Generic;
using System.IO.Ports;

namespace LichKin.IO.Ports
{
    /// <summary>
    ///     串口扩展
    /// </summary>
    public partial class SerialPortX
    {
        // 串口
        private System.IO.Ports.SerialPort serialPort = new System.IO.Ports.SerialPort();

        // 串口名
        private String portName;

        // 设备ID
        internal String sid;
        // 是否使用指令队列
        private Boolean useCMDQueue;
        // 验证打开状态间隔时间（毫秒）
        private int checkOpenStatusPeriod;
        // 打开后操作间隔时间（毫秒）
        private int afterOpenEventHandlerPeriod;
        // 发送指令超时时长（毫秒）
        private int sendCMDTimeout;
        // 接收数据超时时长（毫秒）
        private int reciveDataTimeout;

        /// <summary>
        ///     构造方法
        /// </summary>
        /// <param name="config">配置值</param>
        internal SerialPortX(Dictionary<String, String> config)
        {
            // System.IO.Ports.SerialPort定义参数
            // 串口名
            serialPort.PortName = this.portName = DictionaryUtils.GetString(config, "PortName");
            // 波特率
            serialPort.BaudRate = Convert.ToInt32(DictionaryUtils.GetString(config, "BaudRate"));
            // 数据位
            serialPort.DataBits = Convert.ToInt32(DictionaryUtils.GetString(config, "DataBits"));
            // 校验规则
            String parity = DictionaryUtils.GetString(config, "Parity");
            foreach (Parity PARITY in Enum.GetValues(typeof(Parity)))
            {
                if (Enum.GetName(typeof(Parity), PARITY).Equals(parity))
                {
                    serialPort.Parity = PARITY;
                }
            }
            // 停止位
            String stopBits = DictionaryUtils.GetString(config, "StopBits");
            foreach (StopBits STOP_BITS in Enum.GetValues(typeof(StopBits)))
            {
                if (Enum.GetName(typeof(StopBits), STOP_BITS).Equals(stopBits))
                {
                    serialPort.StopBits = STOP_BITS;
                }
            }

            // LichKin.IO.Ports.SerialPortX定义参数
            // 设备ID
            this.sid = DictionaryUtils.GetString(config, "sid");
            // 是否使用指令队列
            this.useCMDQueue = "true".Equals(DictionaryUtils.GetString(config, "useCMDQueue"));
            // 验证打开状态间隔时间（毫秒）
            this.checkOpenStatusPeriod = Convert.ToInt32(DictionaryUtils.GetString(config, "checkOpenStatusPeriod"));
            // 打开后操作间隔时间（毫秒）
            this.afterOpenEventHandlerPeriod = Convert.ToInt32(DictionaryUtils.GetString(config, "afterOpenEventHandlerPeriod"));
            // 发送指令超时时长（毫秒）
            this.sendCMDTimeout = Convert.ToInt32(DictionaryUtils.GetString(config, "sendCMDTimeout"));
            // 接收数据超时时长（毫秒）
            this.reciveDataTimeout = Convert.ToInt32(DictionaryUtils.GetString(config, "reciveDataTimeout"));
        }

        // 指令队列
        private SerialPortCMDQueue CMDQueue;

        /// <summary>
        ///     串口数据接收事件处理器扩展
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        /// <param name="readedHexString">读取到的数据</param>
        public delegate void SerialDataReceivedEventHandlerX(String sid, String portName, String readedHexString);

        /// <summary>
        ///     尝试打开串口前操作
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        public delegate void BeforeOpen(String sid, String portName);

        /// <summary>
        ///     尝试打开串口成功操作
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        public delegate void OpenSuccess(String sid, String portName);

        /// <summary>
        ///     尝试打开串口失败操作
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        public delegate void OpenError(String sid, String portName);

        /// <summary>
        ///     仍处于打开状态操作
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        public delegate void StillOpen(String sid, String portName);

        /// <summary>
        ///     已经断开操作
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        public delegate void Disconnect(String sid, String portName);

        /// <summary>
        ///     尝试重新打开串口前操作
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        public delegate void BeforeReOpen(String sid, String portName);

        /// <summary>
        ///     尝试重新打开串口成功操作
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        public delegate void ReOpenSuccess(String sid, String portName);

        /// <summary>
        ///     尝试重新打开串口失败操作
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        public delegate void ReOpenError(String sid, String portName);

        /// <summary>
        ///     串口打开后操作
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        public delegate void AfterOpen(String sid, String portName);
    }
}
