using System;
using System.IO.Ports;

namespace LichKin.IO.Ports
{
    /// <summary>
    ///     串口指令
    /// </summary>
    internal class SerialPortCMD
    {
        // 设备ID
        private String sid;
        // 串口名
        private String portName;
        // 是否允许丢弃该指令
        private Boolean allowDiscard;
        // 串口指令事件处理器
        private SerialPortCMDEventHandler cmdEventHandler;
        // 发送指令超时时长（毫秒）
        private int sendCMDTimeout;
        // 接收数据超时时长（毫秒）
        private int reciveDataTimeout;
        // 指令
        private byte[] cmd;

        /// <summary>
        ///     构造方法
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        /// <param name="allowDiscard">是否允许丢弃该指令</param>
        /// <param name="cmdEventHandler">串口指令事件处理器</param>
        /// <param name="sendCMDTimeout">发送指令超时时长（毫秒）</param>
        /// <param name="reciveDataTimeout">接收数据超时时长（毫秒）</param>
        /// <param name="cmd">指令</param>
        public SerialPortCMD(String sid, String portName, Boolean allowDiscard, SerialPortCMDEventHandler cmdEventHandler, int sendCMDTimeout, int reciveDataTimeout, byte[] cmd)
        {
            this.sid = sid;
            this.portName = portName;
            this.allowDiscard = allowDiscard;
            this.cmdEventHandler = cmdEventHandler;
            this.sendCMDTimeout = sendCMDTimeout;
            this.reciveDataTimeout = reciveDataTimeout;
            this.cmd = cmd;

            this.createTime = DateTime.Now;
            this.sended = false;
            this.recived = false;
        }

        // 指令创建时间
        private DateTime createTime;
        // 指令是否已经发送
        private Boolean sended;
        // 指令是否已经接收
        private Boolean recived;
        // 指令发送时间
        private DateTime sendTime;

        /// <summary>
        ///     触发串口指令事件
        /// </summary>
        /// <param name="eventCode">事件编码</param>
        /// <param name="cmdStatus">指令状态</param>
        /// <param name="receivedData">接收到的数据</param>
        public void triggerEvent(String eventCode, int? cmdStatus, String receivedData)
        {
            String cmdStatusStr = null;
            switch (cmdStatus)
            {
                case null: break;
                case -3: cmdStatusStr = "Sended -> NotTimeout -> WaitRecive"; break;
                case -2: cmdStatusStr = "Sended -> ReciveTimeout -> WaitRecive"; break;
                case -1: cmdStatusStr = "Sended -> ReciveTimeout -> Discard"; break;
                case 0: cmdStatusStr = "Sended -> Recived -> Finished"; break;
                case 1: cmdStatusStr = "UnSended -> Timeout -> Discard"; break;
                case 2: cmdStatusStr = "UnSended -> Timeout -> WaitSend"; break;
                case 3: cmdStatusStr = "UnSended -> NotTimeout -> WaitSend"; break;
            }
            this.cmdEventHandler.Handle(sid, portName, eventCode, cmdStatusStr, receivedData);
        }

        /// <summary>
        ///     重置指令
        /// </summary>
        public void ReCreate()
        {
            this.allowDiscard = true;// 强制允许丢弃
            this.createTime = DateTime.Now;// 重新创建时间
        }

        /// <summary>
        ///     发送指令
        /// </summary>
        /// <param name="serialPort">串口对象</param>
        public void Send(SerialPort serialPort)
        {
            if (sended)
            {
                return;
            }
            sended = true;
            sendTime = DateTime.Now;
            try
            {
                triggerEvent("Send", null, null);
                serialPort.Write(cmd, 0, cmd.Length);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        /// <summary>
        ///     完成指令
        /// </summary>
        public void Finish()
        {
            recived = true;
        }

        /// <summary>
        ///     获取指令状态
        /// </summary>
        /// <returns>
        ///     -3  : 已发送但接收未超时;
        ///     -2  : 已发送但接收超时不丢弃;
        ///     -1  : 已发送但接收超时丢弃;
        ///      0  : 已发送并已接收;
        ///      1  : 未发送但超时丢弃;
        ///      2  : 未发送但超时不丢弃;
        ///      3  : 未发送且未超时;
        /// </returns>
        public int GetStatus()
        {
            if (sended)// 已发送
            {
                if (recived)// 已接收
                {
                    return 0;
                }
                else// 未接收
                {
                    if ((DateTime.Now - sendTime).TotalMilliseconds > reciveDataTimeout)// 超时
                    {
                        if (allowDiscard)// 允许丢弃
                        {
                            return -1;
                        }
                        else// 不允许丢弃
                        {
                            return -2;
                        }
                    }
                    else// 未超时
                    {
                        return -3;
                    }
                }
            }
            else// 未发送
            {
                if ((DateTime.Now - createTime).TotalMilliseconds > sendCMDTimeout)// 超时
                {
                    if (allowDiscard)// 允许丢弃
                    {
                        return 1;
                    }
                    else// 不允许丢弃
                    {
                        return 2;
                    }
                }
                else// 未超时
                {
                    return 3;
                }
            }
        }
    }
}
