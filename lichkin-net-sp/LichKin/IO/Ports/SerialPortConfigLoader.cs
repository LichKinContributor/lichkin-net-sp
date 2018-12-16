using LichKin.Utils;
using System;
using System.Collections.Generic;
using static LichKin.IO.Ports.SerialPortX;

namespace LichKin.IO.Ports
{
    /// <summary>
    ///     串口配置文件加载器
    /// </summary>
    public abstract class SerialPortConfigLoader
    {
        // 串口列表
        private List<SerialPortX> serialPorts = null;

        /// <summary>
        ///     获取配置文件名
        /// </summary>
        protected abstract String GetConfigFileName();

        /// <summary>
        ///     加载串口
        /// </summary>
        /// <param name="SerialDataReceivedEventHandlerX">串口数据接收事件处理器扩展</param>
        /// <param name="BeforeOpen">尝试打开串口前操作</param>
        /// <param name="OpenSuccess">尝试打开串口成功操作</param>
        /// <param name="OpenError">尝试打开串口失败操作</param>
        /// <param name="StillOpen">仍处于打开状态操作</param>
        /// <param name="Disconnect">已经断开操作</param>
        /// <param name="BeforeReOpen">尝试重新打开串口前操作</param>
        /// <param name="ReOpenSuccess">尝试重新打开串口成功操作</param>
        /// <param name="ReOpenError">尝试重新打开串口失败操作</param>
        /// <param name="AfterOpen">串口打开后操作</param>
        public void Load
        (
            SerialDataReceivedEventHandlerX SerialDataReceivedEventHandlerX,
            BeforeOpen BeforeOpen,
            OpenSuccess OpenSuccess,
            OpenError OpenError,
            StillOpen StillOpen,
            Disconnect Disconnect,
            BeforeReOpen BeforeReOpen,
            ReOpenSuccess ReOpenSuccess,
            ReOpenError ReOpenError,
            AfterOpen AfterOpen
        )
        {
            if (serialPorts != null)
            {
                Release();
            }
            List<Dictionary<String, String>> configs = JSONFileReader.ReadArray(GetConfigFileName());
            serialPorts = new List<SerialPortX>(configs.Capacity);
            foreach (Dictionary<String, String> config in configs)
            {
                SerialPortX serialPort = new SerialPortX(config);
                serialPort.Start(
                    SerialDataReceivedEventHandlerX,
                    BeforeOpen,
                    OpenSuccess,
                    OpenError,
                    StillOpen,
                    Disconnect,
                    BeforeReOpen,
                    ReOpenSuccess,
                    ReOpenError,
                    AfterOpen
                );
                serialPorts.Add(serialPort);
            }
        }

        /// <summary>
        ///     获取串口
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <returns>
        ///     串口
        /// </returns>
        public SerialPortX Get(String sid)
        {
            if (serialPorts == null)
            {
                return null;
            }
            foreach (SerialPortX serialPort in serialPorts)
            {
                if (serialPort.sid.Equals(sid))
                {
                    return serialPort;
                }
            }
            return null;
        }

        /// <summary>
        ///     释放串口
        /// </summary>
        public void Release()
        {
            if (serialPorts == null)
            {
                return;
            }
            foreach (SerialPortX serialPort in serialPorts)
            {
                serialPort.Close();
            }
            serialPorts = null;
        }
    }
}
