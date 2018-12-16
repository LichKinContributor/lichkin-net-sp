using LichKin.Utils;
using System;
using System.Threading;

namespace LichKin.IO.Ports
{
    /// <summary>
    ///     串口扩展 - 打开串口
    /// </summary>
    public partial class SerialPortX
    {
        // 串口是否已经被调用关闭方法
        private Boolean IsClosed;
        // 打开状态
        private bool? OpenStatus;
        // 打开状态监听计时器
        private Timer OpenStatusListenerTimer;
        // 串口打开后操作计时器
        private Timer AfterOpenEventHandlerListenerTimer;

        /// <summary>
        ///     打开串口
        /// </summary>
        /// <returns>
        ///     打开状态
        /// </returns>
        private Boolean Open()
        {
            try
            {
                serialPort.Open();// 尝试打开
                return true;// 打开成功
            }
            catch (Exception ex)
            {
                System.Console.Write(ex);
                return false;// 打开失败
            }
        }

        private String hexString = "";// 部分响应信息拼凑
        private int byteLength = 0;// 部分响应信息字节长度

        /// <summary>
        ///     开始打开串口
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
        internal void Start(
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
            // 设置串口数据接收事件处理器
            serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler((sender, e) =>
            {
                if (IsClosed)
                {
                    return;
                }
                byte[] readedBytes = new byte[serialPort.BytesToRead];
                int byteLength = readedBytes.Length;
                if (byteLength == 0)
                {
                    CMDQueue.CancelCurrentCMD();
                    return;
                }
                serialPort.Read(readedBytes, 0, byteLength);
                String hexString = HexUtils.BytesToHexString(readedBytes, true).Trim();
                if ("".Equals(hexString))
                {
                    CMDQueue.CancelCurrentCMD();
                    return;
                }
                if (ModbusUtils.IsFullCmd(hexString))
                {
                    if (useCMDQueue)
                    {
                        CMDQueue.FinishCurrentCMD(hexString);
                    }
                    SerialDataReceivedEventHandlerX?.Invoke(this.sid, this.portName, hexString);
                    this.hexString = "";
                    this.byteLength = 0;
                }
                else
                {
                    this.hexString += " " + hexString;
                    this.hexString = this.hexString.Trim();
                    this.byteLength += byteLength;
                    if (ModbusUtils.IsFullCmd(this.hexString))
                    {
                        if (useCMDQueue)
                        {
                            CMDQueue.FinishCurrentCMD(hexString);
                        }
                        SerialDataReceivedEventHandlerX?.Invoke(this.sid, this.portName, this.hexString);
                        this.hexString = "";
                        this.byteLength = 0;
                    }
                }
            });

            // 每100毫秒执行一次指令队列
            if (useCMDQueue)
            {
                CMDQueue = new SerialPortCMDQueue(this.sid, this.portName, serialPort);
                CMDQueue.Start();
            }

            BeforeOpen?.Invoke(this.sid, this.portName);// 尝试打开串口前操作
            if (this.Open())// 尝试打开串口
            {
                OpenSuccess?.Invoke(this.sid, this.portName);// 尝试打开串口成功操作
                OpenStatus = true;
                if (AfterOpen != null)
                {
                    AfterOpenEventHandlerListenerTimer = new Timer((obj) =>
                    {
                        AfterOpen(this.sid, this.portName);// 串口打开后操作
                    }, null, 0, this.afterOpenEventHandlerPeriod);
                }
            }
            else
            {
                OpenError?.Invoke(this.sid, this.portName);// 尝试打开串口失败操作
                OpenStatus = null;
                if (AfterOpen != null)
                {
                    AfterOpenEventHandlerListenerTimer = new Timer((obj) =>
                    {
                        if (IsClosed)
                        {
                            return;
                        }
                        AfterOpen(this.sid, this.portName);// 串口打开后操作
                    }, null, Timeout.Infinite, Timeout.Infinite);
                }
            }
            // 无论打开成功与否，后续都需要监听该串口是否处于打开状态。
            OpenStatusListenerTimer = new Timer((o) =>
            {
                if (IsClosed)
                {
                    return;
                }
                switch (OpenStatus)// 上一次的串口打开状态
                {
                    case null:// 首次打开串口失败
                    case false:// 后续打开串口失败
                        {
                            BeforeReOpen?.Invoke(this.sid, this.portName);// 尝试重新打开串口前操作
                            if (this.Open())// 尝试重新打开串口
                            {
                                ReOpenSuccess?.Invoke(this.sid, this.portName);// 尝试重新打开串口成功操作
                                OpenStatus = true;
                                if (AfterOpen != null)
                                {
                                    AfterOpenEventHandlerListenerTimer.Change(0, this.afterOpenEventHandlerPeriod);// 重新开启计时器
                                }
                            }
                            else
                            {
                                ReOpenError?.Invoke(this.sid, this.portName);// 尝试重新打开串口失败
                                OpenStatus = false;
                            }
                        }
                        break;
                    case true:// 首次或后续打开串口成功
                        {
                            if (serialPort.IsOpen)// 仍处于打开状态
                            {
                                StillOpen?.Invoke(this.sid, this.portName);// 仍处于打开状态操作
                                OpenStatus = true;
                            }
                            else// 已经断开
                            {
                                Disconnect?.Invoke(this.sid, this.portName);// 已经断开操作
                                OpenStatus = false;
                                if (AfterOpen != null)
                                {
                                    AfterOpenEventHandlerListenerTimer.Change(Timeout.Infinite, Timeout.Infinite);// 暂停计时器
                                }
                            }
                        }
                        break;
                }
            }, null, this.checkOpenStatusPeriod, this.checkOpenStatusPeriod);
        }
    }
}
