using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SteerGearApp.Common;
using SteerGearApp.Other;
using Sunny.UI;
using SteerGearApp.SerialPorts;
using System.Threading;
using SteerGearApp.Tools;
using SteerGearApp.Mian.Models;

namespace SteerGearApp.Mian.Views
{
    public partial class MianView : Form
    {

        private const string _titleString = "舵机控制测试V1.0.0";
        /// <summary>
        /// 虚位实时显示控件
        /// </summary>
        private Label lblDiastema = null;

        /// <summary>
        /// 显示速度的label
        /// </summary>
        private Label lblSpeed = null;
        /// <summary>
        /// 波特率的主题按钮
        /// </summary>
        private ToolStripMenuItem[] baudNumItems = null;

        /// <summary>
        /// 是否在运行测试
        /// </summary>
        private bool _isRunTestting = false;

        /// <summary>
        /// 当前角度
        /// </summary>
        private int _currrentAngle = 0;


        /// <summary>
        /// 运行的速度
        /// </summary>
        private int _speedValue = 0;

        /// <summary>
        /// 正向旋转角度
        /// </summary>
        private int _posAngle = 0;

        /// <summary>
        /// 反向旋转角度
        /// </summary>
        private int _negAngle = 0;

        /// <summary>
        /// 返回的角度值
        /// </summary>
        private List<double> _allBackAngle = null;

        /// <summary>
        /// 当前返回的实时角度
        /// </summary>
        private double _currentBackAngle = 0.0;

        /// <summary>
        /// 当前清零保存的数据
        /// </summary>
        private double _currentSaveZeroData = 0.0;

        /// <summary>
        /// 每次旋转的步进
        /// </summary>
        private int _rorateScale = 200;

        /// <summary>
        /// Kp
        /// </summary>
        private double _settingKp = 0.0;
        /// <summary>
        /// Ki
        /// </summary>
        private double _settingKi = 0.0;
        /// <summary>
        /// Kd
        /// </summary>
        private double _settingKd = 0.0;

        public MianView()
        {
            InitializeComponent();

            initDiastemaLabel();

            initSpeedLabel();

            initAngleChart();

            _allBackAngle = new List<double>();
            //initProgressLabel();

            baudNumItems = new ToolStripMenuItem[7];
            baudNumItems[0] = toolStripMenuItem2;
            baudNumItems[1] = toolStripMenuItem3;
            baudNumItems[2] = toolStripMenuItem4;
            baudNumItems[3] = toolStripMenuItem5;
            baudNumItems[4] = toolStripMenuItem6;
            baudNumItems[5] = toolStripMenuItem7;
            baudNumItems[6] = toolStripMenuItem8;
        }

        //diastema
        private void initDiastemaLabel()
        {
            lblDiastema = new Label();
            lblDiastema.AutoSize = true;
            lblDiastema.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            lblDiastema.Location = new System.Drawing.Point(this.Size.Width -360, 80);
            lblDiastema.Text = "实时角度: 0.0";
            lblDiastema.ForeColor = Color.FromArgb(0, 192, 192);
            lblDiastema.BackColor = Color.Transparent;
            lblDiastema.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            lblDiastema.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ctDiastema.Controls.Add(lblDiastema);
            lblDiastema.BringToFront();
        }

        
        private void initSpeedLabel()
        {
            lblSpeed = new Label();
            lblSpeed.AutoSize = true;
            lblSpeed.Anchor = AnchorStyles.Right | AnchorStyles.Top;
            lblSpeed.Location = new System.Drawing.Point(this.Size.Width - 360, 36);
            lblSpeed.Text = "实时速度(°/s): 0.0";
            lblSpeed.ForeColor = Color.Yellow;
            lblSpeed.BackColor = Color.Transparent;
            lblSpeed.Font = new System.Drawing.Font("宋体", 16F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            lblSpeed.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ctDiastema.Controls.Add(lblSpeed);
            lblSpeed.BringToFront();
        }


        private void initAngleChart()
        {
            ctDiastema.ChartAreas[0].AxisX.Minimum = 0;
            ctDiastema.ChartAreas[0].AxisX.Maximum = 100;
            ctDiastema.ChartAreas[0].AxisX.MajorGrid.Interval = 10;

            ctDiastema.ChartAreas[0].AxisY.Minimum = -1440;
            ctDiastema.ChartAreas[0].AxisY.Maximum = 1440;
            ctDiastema.ChartAreas[0].AxisY.MajorGrid.Interval = 2880 / 10;

            ctDiastema.Series[0].Points.AddXY(0, 360);
            ctDiastema.Series[0].Points.AddXY(100, 360);


            ctDiastema.ChartAreas[0].AxisX2.Minimum = 0;
            ctDiastema.ChartAreas[0].AxisX2.Maximum = 100;
            ctDiastema.ChartAreas[0].AxisX2.MajorGrid.Interval = 10;

            ctDiastema.ChartAreas[0].AxisY2.Minimum = -360;
            ctDiastema.ChartAreas[0].AxisY2.Maximum = 360;
            ctDiastema.ChartAreas[0].AxisY2.MajorGrid.Interval = 720 / 10;

            ctDiastema.Series[1].Points.AddXY(0, 0);
            ctDiastema.Series[1].Points.AddXY(100, 0);
        }

        private void MianView_Load(object sender, EventArgs e)
        {
            this.Text = _titleString;

            initViewSetting();

            tryToConnectSerialPort();
        }


        /// <summary>
        /// 连接串口
        /// </summary>
        private void tryToConnectSerialPort()
        {
            ZEROMP.Instance.PortName = SettingModel.Instance.PortName;

            ZEROMP.Instance.BaudNum = SettingModel.Instance.BaudNum;

            serialPortConnectMes();
        }

        private void initViewSetting()
        {
            SettingModel.ReadUserModelFromXML();

            setttingTestItem();

            tbSpeed.Value = SettingModel.Instance.SpeedValue;

            nudSpeed.Value = SettingModel.Instance.SpeedValue;

            tbPosAngle.Value = SettingModel.Instance.PosAngleValue;

            nudPosAngle.Value = SettingModel.Instance.PosAngleValue;

            tbNegAngle.Value = SettingModel.Instance.NegAngleValue;

            nudNegAngle.Value = SettingModel.Instance.NegAngleValue;

            nudCusAngle.Value = SettingModel.Instance.CustomAngleValue;

            nudKP.Value = SettingModel.Instance.KPValue;

            nudKI.Value = SettingModel.Instance.KIValue;

            nudKD.Value = SettingModel.Instance.KDValue;

            settingBaudNumChecktState();
         }

         private void settingBaudNumChecktState()
        {
            foreach(ToolStripMenuItem item in baudNumItems)
            {
                if(item.Text == SettingModel.Instance.BaudNum.ToString())
                {
                    item.Checked = true;
                }
            }
        }

        private void setttingTestItem()
        {
            switch (SettingModel.Instance.TestItem) 
            {
                case "1":
                    rdbSpeed.Checked = true;
                    break;
                case "2":
                    rdbPosition.Checked = true;
                    break;
                case "3":
                    rdbDiastema.Checked = true;
                    break;
                default:
                    rdbSpeed.Checked = true;
                    break;
            }

        }

        private void 串口设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //获取有效的串口
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();

            if (ports.Length <= 0)
            {

                MessageBox.Show("没有搜索到可用的串口！");

                return;
            }

            TSMIPortNames.HideDropDown();

            TSMIPortNames.DropDownItems.Clear();

            for (int i = 0; i < ports.Length; i++)
            {
                ToolStripMenuItem portNameItem = new ToolStripMenuItem();
                portNameItem.Text = ports[i];
                if (ports[i] == SettingModel.Instance.PortName)
                {
                    portNameItem.Checked = true;
                }
                portNameItem.Click += new System.EventHandler(this.portNameMenuItem_Click);
                TSMIPortNames.DropDownItems.Add(portNameItem);
            }
            TSMIPortNames.ShowDropDown();
         }


        /// <summary>
        /// 窗口名称的点击事件
        /// </summary>
        private void portNameMenuItem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem portItem = (ToolStripMenuItem)sender;

            ZEROMP.Instance.PortName = portItem.Text;

            SettingModel.Instance.PortName = portItem.Text;

            serialPortConnectMes();

        }

        private void serialPortConnectMes()
        {
            if (ZEROMP.Instance.IsCanConenct)
            {
                this.Text = string.Format("{0}  {1}已经开启！波特率={2}", 
                                           _titleString,
                                           ZEROMP.Instance.PortName,
                                           ZEROMP.Instance.BaudNum);
            }
            else
            {
                this.Text = string.Format("{0}  {1}无效或被占用", _titleString,ZEROMP.Instance.PortName);
            }
        }

        private void baudNumMenuItem_Click(object sender, EventArgs e)
        {
            clearAllPortBaudCheckd();

            ToolStripMenuItem bandItem = (ToolStripMenuItem)sender;

            bandItem.Checked = true;

            int baudrate = int.Parse(bandItem.Text);

            ZEROMP.Instance.BaudNum = baudrate;

            SettingModel.Instance.BaudNum = baudrate;

            serialPortConnectMes();
        }
        /// <summary>
        /// 清除波特率的选择状态
        /// </summary>
        private void clearAllPortBaudCheckd()
        {
            for(int i = 0; i < baudNumItems.Length; i++)
            {
                baudNumItems[i].Checked = false;
            }
        }

        private void nudSpeed_ValueChanged(object sender, EventArgs e)
        {
            tbSpeed.Value =(int)nudSpeed.Value;

            _speedValue = tbSpeed.Value;
        }

        private void nudPosAngle_ValueChanged(object sender, EventArgs e)
        {
            tbPosAngle.Value = (int)nudPosAngle.Value;

            _posAngle = 65535 / 360 * tbPosAngle.Value;
        }

        private void nudNegAngle_ValueChanged(object sender, EventArgs e)
        {
            tbNegAngle.Value = (int)nudNegAngle.Value;

            _negAngle = 65535 / 360 * tbNegAngle.Value;
        }

        private void tbSpeed_Scroll(object sender, EventArgs e)
        {
            nudSpeed.Value = (int)tbSpeed.Value;
        }

        private void tbPosAngle_Scroll(object sender, EventArgs e)
        {
            nudPosAngle.Value =(int)tbPosAngle.Value;
        }

        private void tbNegAngle_Scroll(object sender, EventArgs e)
        {
            nudNegAngle.Value =(int)tbNegAngle.Value;
        }

        private void 保存设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingModel.Instance.TestItem = getTestItem();

            SettingModel.Instance.SpeedValue = tbSpeed.Value;

            SettingModel.Instance.PosAngleValue = tbPosAngle.Value;

            SettingModel.Instance.NegAngleValue = tbNegAngle.Value;

            SettingModel.Instance.CustomAngleValue = (int)nudCusAngle.Value;

            SettingModel.Instance.KPValue = nudKP.Value;

            SettingModel.Instance.KIValue = nudKI.Value;

            SettingModel.Instance.KDValue = nudKD.Value;

            SettingModel.WriteUserModelToXML();

            MessageBox.Show("保存成功！");
        }

        public string getTestItem()
        {
            if (rdbSpeed.Checked)
            {
                return "1";
            }

            if (rdbPosition.Checked)
            {
                return "2";
            }

            if (rdbDiastema.Checked)
            {
                return "3";
            }

            return "1";
        }

        /// <summary>
        /// 开始按钮
        /// </summary>
        private void btnStart_Click(object sender, EventArgs e)
        {
            unableAllRadioButton();
            _isRunTestting = true;
            startTest();
           
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            enableAllRadioButton();

            if (rdbSpeed.Checked)
            {
                ////暂停的时候设置速度为1，靠阻力停止
                //SteerActionHelper.StartSpeed(1);
                //btnStart.Text = "开始";
                setStartButtonEnable();
            }
            else if (rdbPosition.Checked)
            {
                setStartButtonEnable();
            }
            else
            {
                setStartButtonEnable();
            }


            _isRunTestting = false;
        }

        /// <summary>
        /// 开始测试
        /// </summary>
        private void startTest()
        {
            if (rdbSpeed.Checked)//速度模式
            {
                //runSpeed();
                runSpeedPID();
                //runSpeedWithAngle();
                //runSpeedWithTimer();

            }
            else if (rdbPosition.Checked)//位置模式
            {
                //runPosition();
                //RunPositionMethod();
                runPositionSpeedMethod();
            }
            else//虚位模式
            {
                runDiastema();
            }
        }

        /// <summary>
        /// 运行速度模式
        /// </summary>
        private bool runSpeedPID()
        {
            //PortResult resultModel = SteerActionHelper.StartSpeed(tbSpeed.Value);

            //return resultModel.IsSuccess;

            setStartButtonUable();

            Task.Run(() =>
            {
                int backAagle = 0;

                int currentTime = Environment.TickCount;

                int lastTime = Environment.TickCount;

                double angleNum = 0.0;

                double lastAngleNum = 0.0;

                double speedValue = 0.0;

                PortResult resultModel = null;

                double rorateAngle = 0.0;

                double ralationTime = 0.0;

                PIDModel _pidModel = new PIDModel 
                { 
                    Kp = _settingKp,
                    Ki = _settingKi,
                    Kd = _settingKd
                };

                bool isFirst = true;

                List<double> speedVlaues = new List<double>();

                while (_isRunTestting)
                {
                    Thread.Sleep(20);

                    _pidModel.ErrNow = _speedValue - speedValue;

                    _pidModel.PID_IncrementModel();

                    resultModel = SteerActionHelper.StartSpeed((int)_pidModel.CtrOut);

                    if (resultModel.IsSuccess)
                    {
                        currentTime = Environment.TickCount;

                        backAagle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

                        angleNum = ((double)backAagle) / 65535 * 360.0;

                        //表示正转
                        if(_speedValue > 0)
                        {
                            if(angleNum >= lastAngleNum)
                            {
                                rorateAngle = angleNum - lastAngleNum;
                            }
                            else
                            {
                                rorateAngle = (360 - lastAngleNum) + angleNum;
                            }
                        }
                        else
                        {
                            if(angleNum <= lastAngleNum)
                            {
                                rorateAngle = lastAngleNum - angleNum;
                            }
                            else
                            {
                                rorateAngle = lastAngleNum + (360 - angleNum);
                            }
                        }

                        ralationTime = ((double)Math.Abs(currentTime - lastTime)) / 1000;

                        lastAngleNum = angleNum;

                        lastTime = currentTime;

                        if (isFirst)
                        {
                            isFirst = false;

                            continue;
                        }
                        //这里是获取到速度
                        speedValue = rorateAngle / ralationTime;

                        if (_speedValue < 0) speedValue = -speedValue;

                        speedVlaues.Add(speedValue);

                        if(speedVlaues.Count > 100)
                        {
                            speedVlaues.RemoveAt(0);
                        }

                        var lineV = ctDiastema.BeginInvoke(new Action(() =>
                        {
                            ctDiastema.Series[0].Points.Clear();

                            for (int i = 0; i < speedVlaues.Count; i++)
                            {
                                ctDiastema.Series[0].Points.AddXY(i, speedVlaues[i]);
                            }
                        }));

                        ctDiastema.EndInvoke(lineV);

                        var vi = lblSpeed.BeginInvoke(new Action(() =>
                        {
                            lblSpeed.Text = string.Format("实时角度(°/s):{0}", speedValue.ToString("0.00"));
                        }));
                        lblSpeed.EndInvoke(vi);
                    }
                }

               SteerActionHelper.StartSpeed(1);
            });

            return true;
        }


        private bool runSpeed()
        {
            setStartButtonUable();

            Task.Run(() =>
            {
                int backAagle = 0;

                int currentTime = Environment.TickCount;

                int lastTime = Environment.TickCount;

                double angleNum = 0.0;

                double lastAngleNum = 0.0;

                double speedValue = 0.0;

                PortResult resultModel = null;

                double rorateAngle = 0.0;

                double ralationTime = 0.0;

                List<double> speedVlaues = new List<double>();

                while (_isRunTestting)
                {
                    resultModel = SteerActionHelper.StartSpeed(_speedValue);

                    if (resultModel.IsSuccess)
                    {
                        backAagle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

                        angleNum = ((double)backAagle) / 65535 * 360.0;

                        currentTime = Environment.TickCount;

                        //表示正转
                        if (_speedValue > 0)
                        {
                            if (angleNum >= lastAngleNum)
                            {
                                rorateAngle = angleNum - lastAngleNum;
                            }
                            else
                            {
                                rorateAngle = (360 - lastAngleNum) + angleNum;
                            }
                        }
                        else
                        {
                            if (angleNum <= lastAngleNum)
                            {
                                rorateAngle = lastAngleNum - angleNum;
                            }
                            else
                            {
                                rorateAngle = lastAngleNum + (360 - angleNum);
                            }
                        }

                        ralationTime = ((double)Math.Abs(currentTime - lastTime)) / 1000;

                        //这里是获取到速度
                        speedValue = rorateAngle / ralationTime;

                        speedVlaues.Add(speedValue);

                        if (speedVlaues.Count > 100)
                        {
                            speedVlaues.RemoveAt(0);
                        }

                        lastAngleNum = angleNum;

                        lastTime = currentTime;

                        var lineChart = ctDiastema.BeginInvoke(new Action(() =>
                        {
                            ctDiastema.Series[0].Points.Clear();

                            for (int i = 0; i < speedVlaues.Count; i++)
                            {
                                ctDiastema.Series[0].Points.AddXY(i, speedVlaues[i]);
                            }
                        }));
                        ctDiastema.EndInvoke(lineChart);

                        var vi = lblSpeed.BeginInvoke(new Action(() =>
                        {
                            lblSpeed.Text = string.Format("实时角度(°/s):{0}", speedValue.ToString("0.00"));

                        }));
                        lblSpeed.EndInvoke(vi);

                        Thread.Sleep(20);
                    }
                }

                SteerActionHelper.StartSpeed(1);
            });

            return true;
        }

        private void runSpeedWithAngle()
        {
            PortResult resultModel = SteerActionHelper.StartSpeed(1);

            if (!resultModel.IsSuccess) return;

            setStartButtonUable();

            int backAngle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

            Task.Run(() =>
            {
                while (_isRunTestting)
                {
                    backAngle += 100;

                    SteerActionHelper.StartAngle(backAngle % 65535);
                }

            });
        }
        private void runPositionSpeedMethod()
        {
            setStartButtonUable();

            Task.Run(() =>
            {
                while (_isRunTestting)
                {
                    forewardAngleSpeedMode();
                    Thread.Sleep(1000);
                    rollBackAngleSpeedMode();
                    Thread.Sleep(1000);
                }

                SteerActionHelper.StartSpeed(-1);
            });
        }

        /// <summary>
        /// 运行角度模式
        /// </summary>
        private bool runPosition()
        {
            setStartButtonUable();
            //创建线程
            Task.Run(() =>
            {
                //开始的是否正向旋转
                bool isPosRotate = true;

                PortResult resultModel = null;

                int backAngle = 0;

                while (_isRunTestting)
                {
                    if (isPosRotate)
                    {
                        resultModel =  SteerActionHelper.StartAngle(_posAngle);

                        if (resultModel.IsSuccess)
                        {
                            backAngle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

                            if(Math.Abs(_posAngle - backAngle) <= 50)
                            {
                                isPosRotate = false;
                            }

                        }
                    }
                    else
                    {
                        resultModel = SteerActionHelper.StartAngle(_negAngle);

                        if (resultModel.IsSuccess)
                        {
                            backAngle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

                            if (Math.Abs(_negAngle - backAngle) <= 50)
                            {
                                isPosRotate = true;
                            }
                        }
                    }

                    Thread.Sleep(100);
                }
            });

            return true;
        }

        //private void 
        private void setStartButtonUable()
        {
            btnStart.Enabled = false;

            btnStart.BackColor = Color.Gray;

            btnSendAngle.Enabled = false;

            btnZeroAngle.Enabled = false;

            btnNintyAngle.Enabled = false;

            btOne_EightAngle.Enabled = false;

            btnTwo_SevenAnlge.Enabled = false;

            btnThree_SixAngle.Enabled = false;
        }


        private void setStartButtonEnable()
        {
            btnStart.Enabled = true;

            btnStart.BackColor = Color.Green;

            btnSendAngle.Enabled = true;

            btnZeroAngle.Enabled = true;

            btnNintyAngle.Enabled = true;

            btOne_EightAngle.Enabled = true;

            btnTwo_SevenAnlge.Enabled = true;

            btnThree_SixAngle.Enabled = true;
        }

        /// <summary>
        /// 运行虚位模式
        /// </summary>
        public bool runDiastema()
        {
            _allBackAngle.Clear();

            setStartButtonUable();
            //创建线程
            Task.Run(() =>
            {
                PortResult resultModel = null;

                int backAngle = 0;

                double showAngle = 0.0;

                while (_isRunTestting)
                {
                    //不断发送运行命令
                    resultModel = SteerActionHelper.StartSpeed(2);

                    if (resultModel.IsSuccess)
                    {
                        backAngle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

                        _currentBackAngle = ((double)backAngle) / 65535 * 360.0;

                        showAngle = _currentBackAngle - _currentSaveZeroData;

                        var vi = lblDiastema.BeginInvoke(new Action(() =>
                        {
                            lblDiastema.Text = String.Format("实时角度:{0}", showAngle.ToString("0.00"));

                        }));
                        lblDiastema.EndInvoke(vi);

                        _allBackAngle.Add(showAngle);

                        if(_allBackAngle.Count > 100)
                        {
                            _allBackAngle.RemoveAt(0);
                        }

                        var lineChart = ctDiastema.BeginInvoke(new Action(() =>
                        {
                            ctDiastema.Series[1].Points.Clear();

                            for(int i= 0; i < _allBackAngle.Count; i++)
                            {
                                ctDiastema.Series[1].Points.AddXY(i, _allBackAngle[i]);
                            }

                        }));

                        ctDiastema.EndInvoke(lineChart);

                        Thread.Sleep(50);
                    }
                }
            });

            return true;
        }


        /// <summary>
        ///禁止所有的选项
        /// </summary>
        private void unableAllRadioButton()
        {
            rdbSpeed.Enabled = false;
            rdbPosition.Enabled = false;
            rdbDiastema.Enabled = false;
        }

        /// <summary>
        /// 打开所有的选项
        /// </summary>
        private void enableAllRadioButton()
        {
            rdbSpeed.Enabled = true;
            rdbPosition.Enabled = true;
            rdbDiastema.Enabled = true;
        }

        private void btnSendAngle_Click(object sender, EventArgs e)
        {
            if (_isRunTestting) return;

            _currrentAngle = 65535 / 360 * ((int)nudCusAngle.Value);

            SteerActionHelper.StartAngle(_currrentAngle);
        }

        private void btnZeroAngle_Click(object sender, EventArgs e)
        {
            if (_isRunTestting) return;

            settingAngle(0);
        }

        private void btnNintyAngle_Click(object sender, EventArgs e)
        {
            if (_isRunTestting) return;

            settingAngle(90);
        }

        private void btOne_EightAngle_Click(object sender, EventArgs e)
        {
            if (_isRunTestting) return;

            settingAngle(180);
        }

        private void btnTwo_SevenAnlge_Click(object sender, EventArgs e)
        {
            if (_isRunTestting) return;

            settingAngle(270);
        }

        private void btnThree_SixAngle_Click(object sender, EventArgs e)
        {
            if (_isRunTestting) return;

            settingAngle(360);
        }

        /// <summary>
        /// 设置角度
        /// </summary>
        private void settingAngle(int angle)
        {
            _currrentAngle = 65535 / 360 * angle;

            SteerActionHelper.StartAngle(_currrentAngle);
        }

        private void MianView_FormClosing(object sender, FormClosingEventArgs e)
        {
            _isRunTestting = false;
        }

        private void btnClearData_Click(object sender, EventArgs e)
        {
            _allBackAngle.Clear();

            ctDiastema.Series[0].Points.Clear();
        }

        /// <summary>
        /// 清零
        /// </summary>
        private void btnClearZero_Click(object sender, EventArgs e)
        {
            _currentSaveZeroData = _currentBackAngle;
        }

        private void btnDspChart_Click(object sender, EventArgs e)
        {
            if(btnDspChart.Tag.ToString() == "ON")
            {
                ctDiastema.ChartAreas[0].Visible = false;

                btnDspChart.Tag = "OFF";

                btnDspChart.Text = "显示表格";
            }
            else
            {
                ctDiastema.ChartAreas[0].Visible = true;

                btnDspChart.Tag = "ON";

                btnDspChart.Text = "隐藏表格";
            }
        }

        /// <summary>
        /// 运行位置
        /// </summary>
        private void RunPositionMethod()
        {
            setStartButtonUable();
            forewardAngleSpeedMode();
            Thread.Sleep(1000);
            rollBackAngleSpeedMode();
            Thread.Sleep(1000);
            _isRunTestting = false;
            enableAllRadioButton();
        }

        /// <summary>
        /// 正转多少度
        /// </summary>
        private void forewardAngle()
        {
            PortResult resultModel =  SteerActionHelper.StartSpeed(1);

            if (!resultModel.IsSuccess) return;
            
            int backAngle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

            int distanceAngle = backAngle + _posAngle;

            int realSendNum = 0;

            int moveAngle = backAngle;

            while (moveAngle <= distanceAngle && _isRunTestting)
            {
                if (distanceAngle - moveAngle < _rorateScale)
                {
                    moveAngle += distanceAngle - moveAngle;
                }
                else
                {
                    moveAngle += _rorateScale;
                }

                realSendNum = moveAngle % 65535;

                SteerActionHelper.StartAngle(realSendNum);

                Thread.Sleep(50);
            }
        }

        /// <summary>
        /// 用速度来旋转角度
        /// </summary>
        private void forewardAngleSpeedMode()
        {
            PortResult resultModel = SteerActionHelper.StartSpeed(1);

            if (!resultModel.IsSuccess) return;

            int backAngle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

            //旋转速度
            int rorateSpeed = 1000;

            //记录转动的角度
            int roratedAngle = 0;

            //上一次的角度
            int lastAngle = backAngle;

            int finishAngle = _posAngle;

            while (roratedAngle < _posAngle && _isRunTestting)
            {
                if(finishAngle >= 30000)
                {
                    resultModel = SteerActionHelper.StartSpeed(rorateSpeed);

                    if (resultModel.IsSuccess)
                    {
                        backAngle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

                        if (backAngle - lastAngle >= 0)
                        {
                            roratedAngle += (backAngle - lastAngle);
                        }
                        else
                        {
                            roratedAngle += ((backAngle - 0) + (65535 - lastAngle));
                        }

                        lastAngle = backAngle;
                    }

                    finishAngle = _posAngle - roratedAngle;
                }
                else
                {
                    finishAngle = (backAngle + finishAngle) % 65535;

                    //SteerActionHelper.StartAngle(finishAngle);
                    SteerActionHelper.ThreeTimesWithAngle(2, finishAngle);

                    break;
                }
            }
        }

        /// <summary>
        /// 反转多少度
        /// </summary>
        private void rollbackAngle()
        {
            PortResult resultModel = SteerActionHelper.StartSpeed(1);

            if (!resultModel.IsSuccess) return;

            int backAngle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

            int realSendNum = backAngle;

            int moveAngle = 0;

            while (moveAngle <= _negAngle && _isRunTestting)
            {
                if (_negAngle - moveAngle < _rorateScale)
                {
                    moveAngle += (_negAngle - moveAngle);
                }
                else
                {
                    moveAngle += _rorateScale;
                }

                realSendNum -= moveAngle;

                if(realSendNum <= 0)
                {
                    realSendNum = 65535 + realSendNum;
                }

                SteerActionHelper.StartAngle(realSendNum);
            }

        }

        private void rollBackAngleSpeedMode()
        {
            PortResult resultModel = SteerActionHelper.StartSpeed(-1);

            if (!resultModel.IsSuccess) return;

            int backAngle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

            //旋转的角度
            int rorateSpeed = -1000;

            //记录转动的角度
            int roratedAngle = 0;

            //上一次的角度
            int lastAngle = backAngle;

            int finishAngle = _negAngle;

            while (roratedAngle < _negAngle && _isRunTestting)
            {
                if(finishAngle > 30000)
                {
                    resultModel = SteerActionHelper.StartSpeed(rorateSpeed);

                    if (resultModel.IsSuccess)
                    {
                        backAngle = BitConverter.ToUInt16(resultModel.RecDatas, 8);

                        if (lastAngle - backAngle >= 0)
                        {
                            roratedAngle += (lastAngle - backAngle);
                        }
                        else
                        {
                            roratedAngle += ((lastAngle - 0) + (65535 - backAngle));
                        }

                        lastAngle = backAngle;
                    }

                    //还未转动的角度
                    finishAngle = _negAngle - roratedAngle;
                }
                else
                {
                    finishAngle = (backAngle - finishAngle) % 65535;

                    //命令发送成功没有生效
                    SteerActionHelper.ThreeTimesWithAngle(2, finishAngle);

                    break;
                }

            }
        }

        private void nudKP_ValueChanged(object sender, EventArgs e)
        {
            _settingKp = (double)nudKP.Value;
        }

        private void nudKI_ValueChanged(object sender, EventArgs e)
        {
            _settingKi = (double)nudKI.Value;
        }

        private void nudKD_ValueChanged(object sender, EventArgs e)
        {
            _settingKd = (double)nudKD.Value;
        }
    }
}
