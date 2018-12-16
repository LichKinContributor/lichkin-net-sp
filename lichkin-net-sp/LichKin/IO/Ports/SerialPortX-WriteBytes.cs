using LichKin.Utils;
using System;

namespace LichKin.IO.Ports
{
    /// <summary>
    ///     串口扩展 - 写指令
    /// </summary>
    public partial class SerialPortX
    {
        /// <summary>
        ///     写指令
        /// </summary>
        /// <param name="cmd">指令</param>
        public void WriteBytes(byte[] cmd)
        {
            if (useCMDQueue)
            {
                System.Console.WriteLine("使用队列的串口设备不能调用此方法");
                return;
            }
            try
            {
                serialPort.Write(cmd, 0, cmd.Length);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
        }

        /// <summary>
        ///     写指令
        /// </summary>
        /// <param name="cmd">指令</param>
        public void WriteHexString(String cmd)
        {
            WriteBytes(HexUtils.HexStringToBytes(cmd));
        }

        /// <summary>
        ///     写指令（MODBUS协议）
        /// </summary>
        /// <param name="address">指令-地址码</param>
        /// <param name="func">指令-功能码</param>
        /// <param name="data">指令-数据</param>
        public void WriteModbus(String address, String func, String data)
        {
            WriteHexString(ModbusUtils.CRC(address, func, data));
        }
    }
}
