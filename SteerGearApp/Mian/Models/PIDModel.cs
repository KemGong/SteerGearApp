using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteerGearApp.Mian.Models
{
    public class PIDModel
    {
        /// <summary>
        /// 比例系数
        /// </summary>
        public double Kp { get; set; }

        /// <summary>
        /// 积分系数
        /// </summary>

        public double Ki { get; set; }

        /// <summary>
        /// 微分系数
        /// </summary>
        public double Kd { set; get; }

        /// <summary>
        /// 当前的误差
        /// </summary>

        public double ErrNow { set; get; }

        /// <summary>
        /// 控制增量输出
        /// </summary>
        public double DCtrOut { set; get; }

        /// <summary>
        /// 控制输出
        /// </summary>
        public double CtrOut { set; get; }

        /// <summary>
        /// 上次的误差值
        /// </summary>
        private double _errOld1 { set; get; }

        /// <summary>
        /// 上上次的误差值
        /// </summary>
        private double _errOld2 { set; get; }


        public PIDModel()
        {
            Kp = 0.0;
            Ki = 0.0;
            Kd = 0.0;
            ErrNow = 0.0;
            DCtrOut = 0.0;
            CtrOut = 0.0;
            _errOld1 = 0.0;
            _errOld2 = 0.0;
        }

        /// <summary>
        /// 增量式PID算法
        /// </summary>
        public void PID_IncrementModel()
        {
            double dErrP = ErrNow - _errOld1;

            double dErri = ErrNow;

            double dErrd = ErrNow - 2 * _errOld1 + _errOld2;

            _errOld2 = _errOld1;

            _errOld1 = ErrNow;

            DCtrOut = Kp * dErrP + Ki * dErri + Kd * dErrd;

            if (Kp == 0 && Ki == 0 && Kd == 0)
            {
                CtrOut = 0.0;
            }
            else
            {
                CtrOut += DCtrOut;
            }

            if(Math.Abs(CtrOut) > 1000)
            {
               if(CtrOut>= 0)
                {
                    CtrOut = 1000;
                }
                else
                {
                    CtrOut = -1000;
                }
            }

            //double dErrP = ErrNow;

            //_errOld2 += ErrNow;

            //double dErrd = ErrNow - _errOld1;

            ////_errOld2 = _errOld1;

            //_errOld1 = ErrNow;

            //DCtrOut = Kp * dErrP + Ki * _errOld2 + Kd * dErrd;

            //if (Kp == 0 && Ki == 0 && Kd == 0)
            //{
            //    CtrOut = 0.0;
            //}
            //else
            //{
            //    CtrOut = DCtrOut;
            //}
        }
    }
}
