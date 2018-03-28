using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace VncLib.Console.Client
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            VncConnection vnc = new VncConnection();
            vnc.UpdateInterval = 500;
            vnc.AutoUpdate = true;
            vnc.Connect("10.40.1.126", 5901, "Initial1");
            vnc.UpdateScreen();
            for (int i = 0;; ++i)
            {
                var s = vnc.Screenshot;
                if (s != null)
                {
                    s.Save($"image{i}.bmp");
                }

                Thread.Sleep(3000);
            }
        }
    }
}
