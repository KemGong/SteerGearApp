using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SteerGearApp.Tools
{
    public class Delay
    {
        public static void MS(int milliSecond)
        {
            if (milliSecond == 0)
            {
                return;
            }
            int start = Environment.TickCount;

            while (Math.Abs(Environment.TickCount - start) < milliSecond)
            {
                Application.DoEvents();
            }
        }

        public static void DelayMs(int milliSecond)
        {
            if (milliSecond == 0)
            {
                return;
            }

            int start = Environment.TickCount;

            while (Math.Abs(Environment.TickCount - start) < milliSecond) { }
        }
    }
}
