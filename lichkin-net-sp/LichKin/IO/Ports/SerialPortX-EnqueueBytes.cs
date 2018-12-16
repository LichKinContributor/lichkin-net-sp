using LichKin.Utils;
using System;

namespace LichKin.IO.Ports
{
    /// <summary>
    ///     串口扩展 - 写指令（采用队列方式）
    /// </summary>
    public partial class SerialPortX
    {
        /// <summary>
        ///     入队一条指令
        /// </summary>
        /// <param name="allowDiscard">是否允许丢弃该指令</param>
        /// <param name="cmdEventHandler">串口指令事件处理器</param>
        /// <param name="sendCMDTimeout">发送指令超时时长（毫秒）</param>
        /// <param name="reciveDataTimeout">接收数据超时时长（毫秒）</param>
        /// <param name="cmd">指令</param>
        public void EnqueueBytes(Boolean allowDiscard, SerialPortCMDEventHandler cmdEventHandler, int sendCMDTimeout, int reciveDataTimeout, byte[] cmd)
        {
            if (!useCMDQueue)
            {
                System.Console.WriteLine("不使用队列的串口设备不能调用此方法");
                return;
            }
            CMDQueue.Enqueue(allowDiscard, cmdEventHandler, sendCMDTimeout, reciveDataTimeout, cmd);
        }

        /// <summary>
        ///     入队一条指令
        /// </summary>
        /// <param name="allowDiscard">是否允许丢弃该指令</param>
        /// <param name="cmdEventHandler">串口指令事件处理器</param>
        /// <param name="cmd">指令</param>
        public void EnqueueBytes(Boolean allowDiscard, SerialPortCMDEventHandler cmdEventHandler, byte[] cmd)
        {
            EnqueueBytes(allowDiscard, cmdEventHandler, sendCMDTimeout, reciveDataTimeout, cmd);
        }

        /// <summary>
        ///     入队一条指令
        /// </summary>
        /// <param name="allowDiscard">是否允许丢弃该指令</param>
        /// <param name="cmdEventHandler">串口指令事件处理器</param>
        /// <param name="sendCMDTimeout">发送指令超时时长（毫秒）</param>
        /// <param name="reciveDataTimeout">接收数据超时时长（毫秒）</param>
        /// <param name="cmd">指令</param>
        public void EnqueueHexString(Boolean allowDiscard, SerialPortCMDEventHandler cmdEventHandler, int sendCMDTimeout, int reciveDataTimeout, String cmd)
        {
            EnqueueBytes(allowDiscard, cmdEventHandler, sendCMDTimeout, reciveDataTimeout, HexUtils.HexStringToBytes(cmd));
        }

        /// <summary>
        ///     入队一条指令
        /// </summary>
        /// <param name="allowDiscard">是否允许丢弃该指令</param>
        /// <param name="cmdEventHandler">串口指令事件处理器</param>
        /// <param name="cmd">指令</param>
        public void WriteHexString(Boolean allowDiscard, SerialPortCMDEventHandler cmdEventHandler, String cmd)
        {
            EnqueueHexString(allowDiscard, cmdEventHandler, sendCMDTimeout, reciveDataTimeout, cmd);
        }

        /// <summary>
        ///     入队一条指令（MODBUS协议）
        /// </summary>
        /// <param name="allowDiscard">是否允许丢弃该指令</param>
        /// <param name="cmdEventHandler">串口指令事件处理器</param>
        /// <param name="sendCMDTimeout">发送指令超时时长（毫秒）</param>
        /// <param name="reciveDataTimeout">接收数据超时时长（毫秒）</param>
        /// <param name="address">指令-地址码</param>
        /// <param name="func">指令-功能码</param>
        /// <param name="data">指令-数据</param>
        public void EnqueueModbus(Boolean allowDiscard, SerialPortCMDEventHandler cmdEventHandler, int sendCMDTimeout, int reciveDataTimeout, String address, String func, String data)
        {
            EnqueueHexString(allowDiscard, cmdEventHandler, sendCMDTimeout, reciveDataTimeout, ModbusUtils.CRC(address, func, data));
        }

        /// <summary>
        ///     入队一条指令（MODBUS协议）
        /// </summary>
        /// <param name="allowDiscard">是否允许丢弃该指令</param>
        /// <param name="cmdEventHandler">串口指令事件处理器</param>
        /// <param name="address">指令-地址码</param>
        /// <param name="func">指令-功能码</param>
        /// <param name="data">指令-数据</param>
        public void EnqueueModbus(Boolean allowDiscard, SerialPortCMDEventHandler cmdEventHandler, String address, String func, String data)
        {
            EnqueueModbus(allowDiscard, cmdEventHandler, sendCMDTimeout, reciveDataTimeout, address, func, data);
        }
    }
}
