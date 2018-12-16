using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;

namespace LichKin.IO.Ports
{
    /// <summary>
    ///     串口指令队列
    /// </summary>
    internal class SerialPortCMDQueue
    {
        // 设备ID
        private String sid;
        // 串口名
        private String portName;
        // 串口对象
        private SerialPort serialPort;

        /// <summary>
        ///     构造方法
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        /// <param name="serialPort">串口对象</param>
        public SerialPortCMDQueue(String sid, String portName, SerialPort serialPort)
        {
            this.sid = sid;
            this.portName = portName;
            this.serialPort = serialPort;
        }

        // 指令队列
        private Queue<SerialPortCMD> CMDQueue = new Queue<SerialPortCMD>();
        // 当前指令
        private SerialPortCMD CurrentCMD;
        // 指令队列执行计时器
        private Timer CMDQueueExecuteTimer;

        /// <summary>
        ///     开始执行
        /// </summary>
        public void Start()
        {
            // 每100毫秒执行一次指令队列
            CMDQueueExecuteTimer = new Timer((obj) =>
            {
                if (CurrentCMD == null)
                {
                    CurrentCMD = Dequeue();
                    if (CurrentCMD != null)
                    {
                        CurrentCMD.Send(serialPort);
                    }
                }
                else
                {
                    int status = CurrentCMD.GetStatus();
                    switch (status)
                    {
                        case -3:
                            {
                                CurrentCMD.triggerEvent("CMDQueueExecuteTimer", status, null);
                            }
                            break;
                        case -2:
                            {
                                CurrentCMD.triggerEvent("CMDQueueExecuteTimer", status, null);
                            }
                            break;
                        case -1:
                            {
                                CurrentCMD.triggerEvent("CMDQueueExecuteTimer", status, null);
                                CurrentCMD = null;
                            }
                            break;
                        case 0:
                            {
                                CurrentCMD.triggerEvent("CMDQueueExecuteTimer", status, null);
                                CurrentCMD = null;
                            }
                            break;
                        default:
                            CurrentCMD.triggerEvent("CMDQueueExecuteTimer -> Discard", status, null);
                            CurrentCMD = null;
                            break;
                    }
                }
            }, null, 0, 100);
        }

        /// <summary>
        ///     入队一条指令
        /// </summary>
        /// <param name="allowDiscard">是否允许丢弃该指令</param>
        /// <param name="cmdEventHandler">串口指令事件处理器</param>
        /// <param name="sendCMDTimeout">发送指令超时时长（毫秒）</param>
        /// <param name="reciveDataTimeout">接收数据超时时长（毫秒）</param>
        /// <param name="cmd">指令</param>
        public void Enqueue(Boolean allowDiscard, SerialPortCMDEventHandler cmdEventHandler, int sendCMDTimeout, int reciveDataTimeout, byte[] cmd)
        {
            SerialPortCMD serialPortCMD = new SerialPortCMD(sid, portName, allowDiscard, cmdEventHandler, sendCMDTimeout, reciveDataTimeout, cmd);
            serialPortCMD.triggerEvent("Enqueue", null, null);
            CMDQueue.Enqueue(serialPortCMD);
        }

        /// <summary>
        ///     出队一条指令
        /// </summary>
        /// <returns>
        ///     指令对象
        /// </returns>
        private SerialPortCMD Dequeue()
        {
            if (CMDQueue.Count == 0)
            {
                return null;
            }

            SerialPortCMD cmd = CMDQueue.Dequeue();
            int status = cmd.GetStatus();
            switch (status)
            {
                case 1:
                    {
                        cmd.triggerEvent("Dequeue", status, null);
                        return Dequeue();
                    }
                case 2:
                    {
                        cmd.triggerEvent("Dequeue", status, null);
                        cmd.ReCreate();
                        return cmd;
                    }
                case 3:
                    {
                        cmd.triggerEvent("Dequeue", status, null);
                        return cmd;
                    }
                default:
                    cmd.triggerEvent("Dequeue -> Discard", status, null);
                    return null;
            }
        }

        /// <summary>
        ///     完成当前指令
        /// </summary>
        /// <param name="receivedData">接收到的数据</param>
        public void FinishCurrentCMD(String receivedData)
        {
            CurrentCMD.triggerEvent("FinishCurrentCMD", null, receivedData);
            CurrentCMD.Finish();
        }

        /// <summary>
        ///     结束当前指令
        /// </summary>
        public void CancelCurrentCMD()
        {
            CurrentCMD.triggerEvent("CancelCurrentCMD", null, null);
            CurrentCMD.Finish();
        }

        /// <summary>
        ///     销毁队列
        /// </summary>
        public void Dispose()
        {
            if (CMDQueueExecuteTimer != null)
            {
                try
                {
                    CMDQueueExecuteTimer.Dispose();
                }
                catch (Exception ex)
                {
                    System.Console.Write(ex);
                    CMDQueueExecuteTimer = null;
                }
            }

            this.sid = null;
            this.portName = null;
            this.serialPort = null;

            CMDQueue = null;
            CurrentCMD = null;
        }
    }
}
