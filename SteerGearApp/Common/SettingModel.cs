using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace SteerGearApp.Common
{
    public class SettingModel
    {
        /// <summary>
        /// 端口的名称
        /// </summary>
        public string PortName { set; get; }

        /// <summary>
        /// 串口波特率
        /// </summary>
        public int BaudNum { set; get; }

        /// <summary>
        /// 测试选项
        /// </summary>
        public string TestItem { set; get; }

        /// <summary>
        /// 速度
        /// </summary>
        public int SpeedValue { set; get; }

        /// <summary>
        /// 正角度
        /// </summary>
        public int PosAngleValue { set; get; }

        /// <summary>
        /// 负角度
        /// </summary>
        public int NegAngleValue { set; get; }

        /// <summary>
        /// KP的值
        /// </summary>
        public decimal KPValue { set; get; }

        /// <summary>
        /// KI的值
        /// </summary>
        public decimal KIValue { set; get; }

        /// <summary>
        /// KD的值
        /// </summary>
        public decimal KDValue { set; get; }

        /// <summary>
        /// 自定义角度
        /// </summary>
        public int CustomAngleValue { set; get; }

        private static SettingModel _instance = null;

        private static readonly object oLock = new object();

        public static SettingModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (oLock)
                    {
                        if (_instance == null)
                        {
                            _instance = new SettingModel();

                            ReadUserModelFromXML();
                        }
                    }
                }

                return _instance;
            }
        }

        public SettingModel()
        {
            PortName = "COM1";

            BaudNum = 9600;

            TestItem = "1";

            SpeedValue = 0;

            PosAngleValue = 0;

            NegAngleValue = 0;

            CustomAngleValue = 0;

            KPValue = 0;

            KIValue = 0;

            KDValue = 0;
        }

        /// <summary>
        /// 保存的地址
        /// </summary>
        private static string fileAddress = System.Environment.CurrentDirectory + "\\Setting.xml";

        /// <summary>
        /// 写数据到XML
        /// </summary>
        public static void WriteUserModelToXML()
        {
            System.Xml.Serialization.XmlSerializer writer =
            new System.Xml.Serialization.XmlSerializer(typeof(SettingModel));
            
            System.IO.FileStream file = System.IO.File.Create(fileAddress);

            writer.Serialize(file, SettingModel.Instance);

            file.Close();
        }

        public static void ReadUserModelFromXML()
        {
            if (File.Exists(fileAddress))
            {
                System.Xml.Serialization.XmlSerializer reader =
           new System.Xml.Serialization.XmlSerializer(typeof(SettingModel));

                System.IO.StreamReader file = new System.IO.StreamReader(fileAddress);

                _instance = (SettingModel)reader.Deserialize(file);

                file.Close();
            }
        }
    }
}
