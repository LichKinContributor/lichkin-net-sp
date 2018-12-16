using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LichKin.IO.Ports
{
    /// <summary>
    ///     串口扩展 - 关闭串口
    /// </summary>
    public partial class SerialPortX
    {
        /// <summary>
        ///     关闭串口
        /// </summary>
        internal void Close()
        {
            IsClosed = true;
            if (OpenStatusListenerTimer != null)
            {
                try
                {
                    OpenStatusListenerTimer.Dispose();
                }
                catch (Exception ex)
                {
                    System.Console.Write(ex);
                    OpenStatusListenerTimer = null;
                }
            }

            if (AfterOpenEventHandlerListenerTimer != null)
            {
                try
                {
                    AfterOpenEventHandlerListenerTimer.Dispose();
                }
                catch (Exception ex)
                {
                    System.Console.Write(ex);
                    AfterOpenEventHandlerListenerTimer = null;
                }
            }

            if (CMDQueue != null)
            {
                try
                {
                    CMDQueue.Dispose();
                }
                catch (Exception ex)
                {
                    System.Console.Write(ex);
                    CMDQueue = null;
                }
            }

            if (serialPort == null)
            {
                return;
            }

            try
            {
                serialPort.Close();// 尝试关闭
            }
            catch (Exception ex)
            {
                System.Console.Write(ex);
                serialPort = null;
            }
        }
    }
}
