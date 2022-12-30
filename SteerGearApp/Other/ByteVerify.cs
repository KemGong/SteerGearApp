using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteerGearApp.Other
{
    public class ByteVerify
    {
        /// <summary>
        /// 获取到异或校验字节
        /// </summary>
        public static byte GetByteVerify(byte[] allBytes)
        {
            if(allBytes.Length <= 3) return 0;

            byte byt_xor = allBytes[1];

            for(int i = 2; i < allBytes.Length; i++)
            {
                byt_xor ^= allBytes[i];
            }

            return byt_xor;
        }

        /// <summary>
        /// 获取异或校验后的所有数据
        /// </summary>
        public static byte[] GetByteVerifyArray(byte[] allBytes)
        {
            if (allBytes.Length <= 3) return allBytes;

            byte byt_xor = allBytes[1];

            for (int i = 2; i < allBytes.Length; i++)
            {
                byt_xor ^= allBytes[i];
            }

            List<byte> result = new List<byte>();

            result.AddRange(allBytes);

            result.Add(byt_xor);

            return result.ToArray();
        }

        /// <summary>
        /// 判断是否有效并返回有效数据
        /// </summary>
        public static bool IsAviableData(ref byte[] byteDatas)
        {
            if (byteDatas.Length < 14) return false;

            List<byte> userBytes = new List<byte>();

            bool isFindFisrtByte = false;

            for (int i = 0; i < byteDatas.Length; i++)
            {
                if(byteDatas[i] == 0XFE )
                {
                    isFindFisrtByte = true;
                }

                if (isFindFisrtByte)
                {
                    userBytes.Add(byteDatas[i]);
                }
            }

            if(userBytes.Count >= 14)
            {
                byte byt_xor = userBytes[1];

                for (int i = 2; i < 13; i++)
                {
                    byt_xor ^= userBytes[i];
                }

                if(byt_xor == userBytes[13])
                {
                    byteDatas = userBytes.GetRange(0, 14).ToArray();

                    return true;
                }

               return false;
            }
            else
            {
                return false;
            }
        }
    }
}
