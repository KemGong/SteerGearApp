using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using SteerGearApp.Common;
using SteerGearApp.Other;

namespace SteerGearApp.SerialPorts
{
    public class ZEROMP
    {
        /// <summary>
        /// 等待结果返回时间
        /// </summary>
        private const int MaxWaitTime = 100;

        /// <summary>
        /// 是否能连接上
        /// </summary>
        private bool _isCanConnect = false;

        public bool IsCanConenct
        {
            get { return _isCanConnect; }
        }

        /// <summary>
        /// 串口名称
        /// </summary>
        private string _portName = "COM1";
        public string PortName
        {
            get { return _portName; }
            set 
            {
                _portName = value;

                Open();
            }  
        }

        /// <summary>
        /// 串口波特率
        /// </summary>
        private int _baudNum = 9600;
        public int BaudNum
        {
            get { return _baudNum; } 
            set 
            { 
                _baudNum = value;

                Open();
            }
        }

        /// <summary>
        /// 同步锁
        /// </summary>
        private readonly object sendLk = new object(); 

        /// <summary>
        /// 串口句柄
        /// </summary>
        private SerialPort _COMM = null;


        /// <summary>
        /// 单例
        /// </summary>
        private static ZEROMP _instance = null;

        private static readonly object locker = new object();

        public static ZEROMP Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (locker)
                    {
                        if (_instance == null)
                        {
                            _instance = new ZEROMP();
                        }
                    }
                }

                return _instance;
            }
        }

        public ZEROMP()
        {
            _COMM = new SerialPort();
        }

        /// <summary>
        /// 清除缓冲区的数据
        /// </summary>
        public void ClearBuffer()
        {
            try
            {
                _COMM.DiscardInBuffer();
            }
            catch { };
        }

        public bool Open()
        {
            try
            {
                _isCanConnect = false;

                if (_COMM.IsOpen)
                {
                    _COMM.Close();
                }
                _COMM.PortName = _portName;
                _COMM.BaudRate = _baudNum;
                _COMM.DataBits = 8;
                _COMM.Parity = Parity.None;
                _COMM.StopBits = StopBits.One;
                _COMM.Open();
                //再关闭一下
                _COMM.Close();
                _isCanConnect = true;
                return true;
            }
            catch 
            {
                _isCanConnect = false;
                return false;
            }
        }

        /// <summary>
        /// 打开串口
        /// </summary>
        public bool OpenPort()
        {
            try
            {
                
                if(_isCanConnect && !_COMM.IsOpen)
                {
                    _COMM.Open();
                }

                return _COMM.IsOpen;

            }catch 
            { 
               return false;
            };
        }

        /// <summary>
        /// 关闭串口
        /// </summary>
        public void ClosePort()
        {
            try
            {
                if(_isCanConnect && _COMM.IsOpen)
                {
                    _COMM.Close();
                }

            }catch { };
        }

        /// <summary>
        /// 获取串口的状态
        /// </summary>
        public bool IsOpen()
        {
            try
            {
                if (_COMM.IsOpen)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }


        public PortResult SendRead(byte[] sendDatas)
        {
            lock (sendLk)
            {
                //打开串口
                if(!OpenPort()) return new PortResult(false,null,"打开串口失败");
                //清除缓存
                ClearBuffer();
                //发送数据
                _COMM.Write(sendDatas, 0, sendDatas.Length);
                //读取数据
                PortResult result = recDataMethod();
                //关闭串口
                ClosePort();
                //返回结果
                return result;
            }
        }

        /// <summary>
        /// 开始接收数据
        /// </summary>
        private PortResult recDataMethod()
        {
            byte[] data = null;

            List<byte> recDatas = new List<byte>();

            int start = Environment.TickCount;

            PortResult result = new PortResult();

            byte[] userDatas = null;

            while (Math.Abs(Environment.TickCount - start) < MaxWaitTime)
            {
                try
                {
                    data = new byte[_COMM.BytesToRead];

                    _COMM.Read(data, 0, data.Length);

                }
                catch
                {
                    result.ErrorMsg = "ERROR 201"; 
                }

                recDatas.AddRange(data);

                if (recDatas.Count <= 4) continue;

                userDatas = recDatas.ToArray();

                bool isAviable = ByteVerify.IsAviableData(ref userDatas);

                if (isAviable)
                {
                    result.IsSuccess = true;

                    result.RecDatas = userDatas;

                    return result;
                }

                System.Windows.Forms.Application.DoEvents();
            }

            result.ErrorMsg = "ERROR 202";

            return result;

        }
    }
}
