﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;

namespace SteerGearApp.Tools
{
    public class INI
    {

        #region API函数声明

        [DllImport("kernel32")]//返回0表示失败，非0为成功
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);
        [DllImport("kernel32")]//返回取得字符串缓冲区的长度
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);

        #endregion

        #region 读Ini文件
        /// <summary>
        /// 读Ini文件
        /// </summary>
        /// <param name="Section">字节名字</param>
        /// <param name="Key">参数名</param>
        /// <param name="NoText">默认名字</param>
        /// <param name="iniFilePath">文件地址</param>
        /// <returns></returns>
        public static string ReadIniData(string Section, string Key, string NoText, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        #endregion

        #region 写Ini文件
        /// <summary>
        /// 写Ini文件
        /// </summary>
        /// <param name="Section">字节名字</param>
        /// <param name="Key">参数名</param>
        /// <param name="Value">值</param>
        /// <param name="iniFilePath">文件地址</param>
        /// <returns></returns>
        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
