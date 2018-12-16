
namespace LichKin.IO.Ports
{
    /// <summary>
    ///     串口指令事件处理器 - 部分逻辑实现
    /// </summary>
    public abstract class SerialPortCMDEventHandlerImpl : SerialPortCMDEventHandler
    {
        /// <summary>
        ///     处理方法
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        /// <param name="eventCode">事件编码</param>
        /// <param name="cmdStatus">指令状态</param>
        /// <param name="receivedData">接收到的数据</param>
        public void Handle(string sid, string portName, string eventCode, string cmdStatus, string receivedData)
        {
            switch (eventCode)
            {
                case "Enqueue":// 入队一条指令
                    // 通常什么都不用做
                    return;
                case "Dequeue":// 出队一条指令
                    {
                        switch (cmdStatus)
                        {
                            case "UnSended -> Timeout -> Discard":// 未发送但超时丢弃
                                UnSendedTimeoutDiscard(sid, portName);
                                return;
                            case "UnSended -> Timeout -> WaitSend":// 未发送但超时不丢弃
                                // 通常什么都不用做
                                return;
                            case "UnSended -> NotTimeout -> WaitSend":// 未发送且未超时
                                // 通常什么都不用做
                                return;
                        }
                    }
                    return;
                case "Send":// 发送指令
                    // 通常什么都不用做
                    return;
                case "CMDQueueExecuteTimer":// 每100毫秒执行一次指令队列
                    {
                        switch (cmdStatus)
                        {
                            case "Sended -> NotTimeout -> WaitRecive":// 已发送但接收未超时
                                // 通常什么都不用做
                                return;
                            case "Sended -> ReciveTimeout -> WaitRecive":// 已发送但接收超时不丢弃
                                // 通常什么都不用做
                                return;
                            case "Sended -> ReciveTimeout -> Discard":// 已发送但接收超时丢弃
                                SendedReciveTimeoutDiscard(sid, portName);
                                return;
                            case "Sended -> Recived -> Finished":// 已发送并已接收
                                // 通常什么都不用做
                                return;
                        }
                    }
                    return;
                case "CMDQueueExecuteTimer -> Discard":// 每100毫秒执行一次指令队列，未处理的状态，理论上不可能出现。
                    // TODO 记录异常日志
                    return;
                case "Dequeue -> Discard":// 出队一条指令，未处理的状态，理论上不可能出现。
                    // TODO 记录异常日志
                    return;
                case "FinishCurrentCMD":// 完成当前指令
                    Finished(sid, portName, receivedData);
                    return;
                case "CancelCurrentCMD":// 结束当前指令
                    // TODO 记录异常日志
                    Canceled(sid, portName);
                    return;
            }
        }

        /// <summary>
        ///     未发送 -> 超时 -> 丢弃
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        protected abstract void UnSendedTimeoutDiscard(string sid, string portName);

        /// <summary>
        ///     已发送 -> 接收超时 -> 丢弃
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        protected abstract void SendedReciveTimeoutDiscard(string sid, string portName);

        /// <summary>
        ///     已完成
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        /// <param name="receivedData">接收到的数据</param>
        protected abstract void Finished(string sid, string portName, string receivedData);

        /// <summary>
        ///     已结束
        /// </summary>
        /// <param name="sid">设备ID</param>
        /// <param name="portName">串口名</param>
        protected abstract void Canceled(string sid, string portName);
    }
}
