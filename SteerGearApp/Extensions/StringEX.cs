using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteerGearApp.Extensions
{
    public static class StringEX
    {
        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="str">需要判断的字符串</param>
        /// <returns>判定的结果</returns>
        public static bool judgeStringIsNotNullAndEmpty(this string str)
        {
            bool isNotNullAndEmpty = false;

            if (str != null && str != "" && str.Length > 0)
            {
                isNotNullAndEmpty = true;
            }
            return isNotNullAndEmpty;
        }
    }
}
