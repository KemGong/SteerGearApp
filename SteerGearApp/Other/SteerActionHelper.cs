using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SteerGearApp.SerialPorts;
using SteerGearApp.Common;

namespace SteerGearApp.Other
{
    public class SteerActionHelper
    {
        /// <summary>
        /// 控制速度
        /// </summary>
        public static PortResult StartSpeed(int speed)
        {
            List<byte> sendData = new List<byte>();
            sendData.Add(0XFE); //帧头
            sendData.Add(0X09); //数据长度
            sendData.Add(0X01); //功能码
            sendData.Add(0X01); //功能码
            sendData.Add(0X01); //舵机ID
            Byte[] speedBytes = BitConverter.GetBytes(speed);
            sendData.Add(speedBytes[0]);  //左舵机速度
            sendData.Add(speedBytes[1]);
            sendData.Add(0X00);           //右舵机旋转角度
            sendData.Add(0X00);
            sendData.Add(speedBytes[0]);  //右舵机速度
            sendData.Add(speedBytes[1]);
            sendData.Add(0X00);           //右舵机旋转角度  
            sendData.Add(0X00);
            return ProtSendData(ByteVerify.GetByteVerifyArray(sendData.ToArray()));
        }

        /// <summary>
        /// 控制速度和角度
        /// </summary>
        public static PortResult ChangeSpeedAndAngle(int speed, int angle)
        {
            List<byte> sendData = new List<byte>();
            sendData.Add(0XFE); //帧头
            sendData.Add(0X09); //数据长度
            sendData.Add(0X01); //功能码
            sendData.Add(0X01); //功能码
            sendData.Add(0X01); //舵机ID
            Byte[] speedBytes = BitConverter.GetBytes(speed);
            byte[] angleBytes = BitConverter.GetBytes(angle);
            sendData.Add(speedBytes[0]);  //左舵机速度
            sendData.Add(speedBytes[1]);
            sendData.Add(angleBytes[0]);  //右舵机旋转角度
            sendData.Add(angleBytes[1]);
            sendData.Add(speedBytes[0]);  //右舵机速度
            sendData.Add(speedBytes[1]);
            sendData.Add(angleBytes[0]);  //右舵机旋转角度  
            sendData.Add(angleBytes[1]);
            return ProtSendData(ByteVerify.GetByteVerifyArray(sendData.ToArray()));
        }


        /// <summary>
        /// 没有成功的时候，多发送一次
        /// </summary>
        public static PortResult ThreeTimesWithAngle(int times, int angle)
        {
            PortResult result = new PortResult();

            int runTimes = 0;

            while (!result.IsSuccess && runTimes < times)
            {
                result = StartAngle(angle);

                runTimes++;
            }

            return result;
        }

        /// <summary>
        /// 控制角度
        /// </summary>
        public static PortResult StartAngle(int angle)
        {
            List<byte> sendData = new List<byte>();
            sendData.Add(0XFE); //帧头
            sendData.Add(0X09); //数据长度
            sendData.Add(0X01); //功能码
            sendData.Add(0X01); //功能码
            sendData.Add(0X01); //舵机ID
            Byte[] speedBytes = BitConverter.GetBytes(angle);
            sendData.Add(0X00);           //左舵机速度  
            sendData.Add(0X00);
            sendData.Add(speedBytes[0]);  //左舵机旋转角度
            sendData.Add(speedBytes[1]);
            sendData.Add(0X00);           //右舵机速度
            sendData.Add(0X00);
            sendData.Add(speedBytes[0]);  //右舵机旋转角度
            sendData.Add(speedBytes[1]);
            return ProtSendData(ByteVerify.GetByteVerifyArray(sendData.ToArray()));
        }

        /// <summary>
        /// 获取舵机的虚位
        /// </summary>
        public static PortResult GetDiastemaData()
        {
            List<byte> sendData = new List<byte>();
            sendData.Add(0XFE); //帧头
            sendData.Add(0X09); //数据长度
            sendData.Add(0X01); //功能码
            sendData.Add(0X01); //功能码
            sendData.Add(0X01); //舵机ID
            sendData.Add(0X02); //左舵机速度  
            sendData.Add(0X00);
            sendData.Add(0X00); //左舵机旋转角度
            sendData.Add(0X00);
            sendData.Add(0X02); //右舵机速度
            sendData.Add(0X00);
            sendData.Add(0X00); //右舵机旋转角度
            sendData.Add(0X00);
            return ProtSendData(ByteVerify.GetByteVerifyArray(sendData.ToArray()));
        }
        
        /// <summary>
        /// 停止运行
        /// </summary>
        public static PortResult StopRun()
        {
            List<byte> sendData = new List<byte>();
            sendData.Add(0XFE); //帧头
            sendData.Add(0X09); //数据长度
            sendData.Add(0X01); //功能码
            sendData.Add(0X01); //功能码
            sendData.Add(0X01); //舵机ID
            sendData.Add(0X00); //左舵机速度  
            sendData.Add(0X00);
            sendData.Add(0X00); //左舵机旋转角度
            sendData.Add(0X00);
            sendData.Add(0X00); //右舵机速度
            sendData.Add(0X00);
            sendData.Add(0X00); //右舵机旋转角度
            sendData.Add(0X00);
            return ProtSendData(ByteVerify.GetByteVerifyArray(sendData.ToArray()));
        }


        public static PortResult ProtSendData(byte[] datas)
        {
            return ZEROMP.Instance.SendRead(datas);
        }
    }
}
