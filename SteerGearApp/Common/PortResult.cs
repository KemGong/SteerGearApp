using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteerGearApp.Common
{
    public class PortResult
    {
        public PortResult()
        {
            IsSuccess = false;

            RecDatas = null;

            ErrorMsg = "";
        }

        public PortResult(bool success, byte[] datas, string errorMsg) : this()
        {
            IsSuccess = success;
            RecDatas = datas;
            ErrorMsg = errorMsg;
        }
        /// <summary>
        /// 是否发送成功
        /// </summary>
        public bool IsSuccess { set; get; }

        /// <summary>
        /// 接收的数据
        /// </summary>
        public byte[] RecDatas { set; get; }

        /// <summary>
        /// 错误的消息
        /// </summary>
        public string ErrorMsg { set; get; }
    }
}
