using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace ImgurScraper
{
    public static class Extensions
    {
        public static void FadingText(this Label label, string text,int delay = 40)
        {
            Task.Run(() =>
            {
                if(label.Text!="")
                    label.Text = "";
                label.Text = text;
                for (int i = 0; i < 52; ++i)
                {
                    int c = i - 1;
                    int a = 240 - i * 4;
                    
                       
                    label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(a)))), ((int)(((byte)(a)))), ((int)(((byte)(a)))));
                    Thread.Sleep(delay);
                    
                }
                label.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(34)))), ((int)(((byte)(34)))), ((int)(((byte)(38)))));
                label.Text = "";
            });
           
        }
    }
}
