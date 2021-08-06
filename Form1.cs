using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;

namespace ImgurScraper
{

    public partial class Form1 : Form
    {
        static bool isPathDefault;

        private Point mouseOffset;
        private bool isMouseDown = false;

        static string path;
        static int[] INVALID = { -1, 0, 503, 5082, 4939, 4940, 4941, 12003, 5556, 5553, 6323 };

        static int total_downloaded;
        static int total_invalid;

        

        public Form1()
        {
            InitializeComponent();
            textBox1.Text = $"Please, check your internet connection before launch{Environment.NewLine}Logs will be here";

            button1.Click +=  (s, e) => 
            {
                this.button1.Enabled = false;

                string TIME = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";
                textBox1.AppendText($"{Environment.NewLine}[{TIME}]" +
               $"Scraper is Launched{Environment.NewLine}");

                int thread_count = (int)numericUpDown1.Value;
                int delay = (int)numericUpDown2.Value;
                textBox1.AppendText($"[{TIME}]" +
               $"Delay({delay})\nThreads({thread_count}){Environment.NewLine}");

                for (int i = 0; i < thread_count; i++)
                {
                    try
                    {
                        new Thread(() => NewTask(delay)).Start();
                    }
                    catch (Exception ex)
                    {
                        button5.PerformClick();
                    textBox1.AppendText($"{Environment.NewLine}[{TIME}] Exception!{Environment.NewLine}{ex}{Environment.NewLine}");

                    }
                   
                }

                this.button1.BackColor = System.Drawing.SystemColors.Highlight;
                this.button1.Text = "    Launched";
                this.button5.Text = "   Cancel";
                this.button2.Enabled = false;
                numericUpDown1.Enabled = false;
                numericUpDown2.Enabled = false;
                this.button5.Enabled = true;
                this.button5.BackColor = System.Drawing.Color.Crimson;

                using (RegistryKey SOFTWARE = Registry.CurrentUser.OpenSubKey("SOFTWARE", true))
                using (RegistryKey ImgurScraper = SOFTWARE.OpenSubKey("ImgurScraper", true))
                {
                    ImgurScraper.SetValue("path", path);
                    ImgurScraper.SetValue("total_downloaded", total_downloaded);
                    ImgurScraper.SetValue("total_invalid", total_invalid);
                }

            };
            button2.Click += (s, e) => SetFolder();
            button3.Click += (s, e) =>
            {
                using (RegistryKey SOFTWARE = Registry.CurrentUser.OpenSubKey("SOFTWARE", true))
                using (RegistryKey ImgurScraper = SOFTWARE.OpenSubKey("ImgurScraper", true))
                {
                    ImgurScraper.SetValue("path", path);
                    ImgurScraper.SetValue("total_downloaded", total_downloaded);
                    ImgurScraper.SetValue("total_invalid", total_invalid);
                }
                this.Close();
                this.Dispose();
            };

            button4.Click += (s, e) =>
            {
                using (RegistryKey SOFTWARE = Registry.CurrentUser.OpenSubKey("SOFTWARE", true))
                using (RegistryKey ImgurScraper = SOFTWARE.OpenSubKey("ImgurScraper", true))
                {
                    ImgurScraper.SetValue("path", path);
                    ImgurScraper.SetValue("total_downloaded", total_downloaded);
                    ImgurScraper.SetValue("total_invalid", total_invalid);
                }
                this.WindowState = FormWindowState.Minimized;
            };
            linkLabel1.LinkClicked += (s,e) => System.Diagnostics.Process.Start("https://github.com/lexabelkin");
            button5.Click += (s, e) => 
            {
                label7.FadingText("Some threads may still terminate", 100);

                string TIME = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";
                this.button5.BackColor = System.Drawing.Color.DarkSlateBlue;
                this.button5.Enabled = false;
                this.button5.Text = "   Canceled";

                this.button2.Enabled = true;
                numericUpDown1.Enabled = true;
                numericUpDown2.Enabled = true;
                this.button1.Enabled = true;
                this.button1.Text = "    Launch";
                this.button1.BackColor = System.Drawing.Color.LimeGreen;

                textBox1.AppendText($"{Environment.NewLine}[{TIME}]" +
                $"Scraper is Canceled{Environment.NewLine}");

                using (RegistryKey SOFTWARE = Registry.CurrentUser.OpenSubKey("SOFTWARE", true))
                using (RegistryKey ImgurScraper = SOFTWARE.OpenSubKey("ImgurScraper", true))
                {
                    ImgurScraper.SetValue("path", path);
                    ImgurScraper.SetValue("total_downloaded", total_downloaded);
                    ImgurScraper.SetValue("total_invalid", total_invalid);
                }

            };
        }

        private void Form1_Load(object sender, EventArgs e)
        {


            
            ToolTip toolTip1 = new ToolTip();

            toolTip1.SetToolTip(label2, "Sets carefully!");
            toolTip1.SetToolTip(label8, "Sets carefully!");
            toolTip1.SetToolTip(numericUpDown1, "Sets carefully!");
            toolTip1.SetToolTip(button2, "Sets the folder where the photos will be downloaded");
            toolTip1.SetToolTip(button7, "Manually saves the statistics");
            toolTip1.SetToolTip(button6, "Resets the statistics");

            toolTip1.SetToolTip(linkLabel1, "My GitHub :)");
            toolTip1.SetToolTip(button4, "Minimize window");
            toolTip1.SetToolTip(button3, "Close application");

            RegistryKey SOFTWARE = Registry.CurrentUser.OpenSubKey("SOFTWARE", true);

            var test = SOFTWARE.GetSubKeyNames();
            if (!test.Contains("ImgurScraper"))
                using (RegistryKey ImgurScraper = SOFTWARE.CreateSubKey("ImgurScraper"))
                {
                    ImgurScraper.SetValue("path", $@"{Environment.CurrentDirectory}\photos");
                    ImgurScraper.SetValue("total_downloaded", 0);
                    ImgurScraper.SetValue("total_invalid", 0);
                    total_invalid = 0;
                    total_downloaded = 0;
                    path = $@"{Environment.CurrentDirectory}\photos";
                    isPathDefault = true;
                } 
            else
            {

                path = SOFTWARE.OpenSubKey("ImgurScraper").GetValue("path").ToString();
                total_downloaded = (int)SOFTWARE.OpenSubKey("ImgurScraper").GetValue("total_downloaded");
                total_invalid = (int)SOFTWARE.OpenSubKey("ImgurScraper").GetValue("total_invalid");
                isPathDefault = false;
            }

            label6.Text = $"Invalid files received:\t{total_invalid}";
            label5.Text = $"Files downloaded:\t{total_downloaded}";
            textBox2.Text += $"{Environment.NewLine}{path}";
            SOFTWARE.Dispose();
        }

        public void SetFolder()
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            FBD.Description = "Sets the photo destination folder";
            FBD.RootFolder = Environment.SpecialFolder.MyComputer;
            
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                path = FBD.SelectedPath;
                textBox2.Text = $"Selected Folder: {Environment.NewLine}{path}";
                this.button2.BackgroundImage = global::ImgurScraper.Resource1.folder;


                using (RegistryKey SOFTWARE = Registry.CurrentUser.OpenSubKey("SOFTWARE", true))
                using (RegistryKey ImgurScraper = SOFTWARE.OpenSubKey("ImgurScraper", true))
                {
                    ImgurScraper.SetValue("path", path);
                    ImgurScraper.SetValue("total_downloaded", total_downloaded);
                    ImgurScraper.SetValue("total_invalid", total_invalid);
                }
                   
                
            }
            
        }

        public Tuple<string,string> MakeUrl()
        {
            string url = "http://i.imgur.com/";

            Random rand = new Random();
            int length = rand.Next(5, 7);
            

            string name = length == 5
                ? Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(5).ToLower()
                : Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString())).Remove(6).ToLower();
            
            return Tuple.Create($"{name}.jpg", $"{url}{name}.jpg");
        }

        public void DownloadPicture()
        {
            string url = MakeUrl().Item2;
            string name = MakeUrl().Item1;

            WebClient wc = new WebClient();

            wc.DownloadFileCompleted += (s, e) =>
            {
                string TIME = $"{DateTime.Now.Hour}:{DateTime.Now.Minute}:{DateTime.Now.Second}";


                FileInfo info = new FileInfo($@"{path}\{name}");
                if (INVALID.Contains((int)info.Length))
                {
               
                    try
                    {
                        File.Delete($"{path}/{name}");
                    }
                    catch (Exception ex)
                    {
                        
                        textBox1.AppendText($"{Environment.NewLine}[{TIME}] Exception!{Environment.NewLine}{ex}{Environment.NewLine}");
                    }
                    
                    total_invalid += 1;
                    label6.Text = $"Invalid files received:\t{total_invalid}";
                    
                }
                else
                {
                    textBox1.AppendText($"{Environment.NewLine}[{TIME}]File \"{name}\"({((info.Length) / 1024).ToString()}KB) downloaded");

                    total_downloaded += 1;
                    label5.Text = $"Files downloaded:\t{total_downloaded}";
                }

            };
            wc.DownloadFileAsync(new Uri(url), $"{path}/{name}");
            wc.Dispose();

        }
        public void NewTask(int delay)
        {
            if (isPathDefault)
            {
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
            }


            while (button1.Enabled == false)
            {
                try
                {
                    Task.Run(DownloadPicture);
                }
                catch (Exception ex)
                {
                    textBox1.AppendText($"{Environment.NewLine}[] Exception!{Environment.NewLine}{ex}{Environment.NewLine}");

                }
                
                Thread.Sleep(delay);
            }
        }

        

        #region mouse events

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            int xOffset;
            int yOffset;

            if (e.Button == MouseButtons.Left)
            {
                xOffset = -e.X - SystemInformation.FrameBorderSize.Width;
                yOffset = -e.Y - SystemInformation.CaptionHeight -
                    SystemInformation.FrameBorderSize.Height;
                mouseOffset = new Point(xOffset, yOffset);
                isMouseDown = true;
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouseOffset.X, mouseOffset.Y);
                Location = mousePos;
            }
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                isMouseDown = false;
            }
        }

        private void textBox1_MouseDown(object sender, MouseEventArgs e)
        {
            button1.Focus();
            
        }

        private void button3_MouseMove(object sender, MouseEventArgs e)
        {
            this.button3.BackgroundImage = global::ImgurScraper.Resource1.multiply1;
        }

        private void button4_MouseMove(object sender, MouseEventArgs e)
        {
            this.button4.BackgroundImage = global::ImgurScraper.Resource1.minimize_button1;
        }

        private void button3_MouseLeave(object sender, EventArgs e)
        {
            this.button3.BackgroundImage = global::ImgurScraper.Resource1.multiply;
        }

        private void button4_MouseLeave(object sender, EventArgs e)
        {
            this.button4.BackgroundImage = global::ImgurScraper.Resource1.minimize_button;
        }

        private void button7_MouseDown(object sender, MouseEventArgs e)
        {
            this.button7.Size = new System.Drawing.Size(22, 22);
        }

        private void button7_MouseUp(object sender, MouseEventArgs e)
        {
            this.button7.Size = new System.Drawing.Size(25, 25);
        }

        private void button6_MouseDown(object sender, MouseEventArgs e)
        {
            this.button6.Size = new System.Drawing.Size(22, 22);
        }

        private void button6_MouseUp(object sender, MouseEventArgs e)
        {
            this.button6.Size = new System.Drawing.Size(25, 25);
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            this.button1.Size = new System.Drawing.Size(160, 45);
        }

        private void button1_MouseUp(object sender, MouseEventArgs e)
        {
            this.button1.Size = new System.Drawing.Size(170, 50);
        }

        private void button5_MouseDown(object sender, MouseEventArgs e)
        {
            this.button5.Size = new System.Drawing.Size(160, 37);
        }

        private void button5_MouseUp(object sender, MouseEventArgs e)
        {
            this.button5.Size = new System.Drawing.Size(170, 42);
        }

        private void button2_MouseDown(object sender, MouseEventArgs e)
        {
            this.button2.Size = new System.Drawing.Size(37, 30);
        }

        private void button2_MouseUp(object sender, MouseEventArgs e)
        {
            this.button2.Size = new System.Drawing.Size(42, 34);
        }

        private void linkLabel1_MouseEnter(object sender, EventArgs e)
        {
            this.linkLabel1.LinkColor = System.Drawing.Color.MediumPurple;

        }

        private void linkLabel1_MouseLeave(object sender, EventArgs e)
        {
            this.linkLabel1.LinkColor = System.Drawing.Color.Turquoise;
        }

        private void linkLabel1_MouseDown(object sender, MouseEventArgs e)
        {
            this.linkLabel1.Font = new System.Drawing.Font("Consolas", 10.12727F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        }

        private void linkLabel1_MouseUp(object sender, MouseEventArgs e)
        {
            this.linkLabel1.Font = new System.Drawing.Font("Consolas", 11.12727F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
        }

        private void button6_Click(object sender, EventArgs e)
        {
            using (RegistryKey SOFTWARE = Registry.CurrentUser.OpenSubKey("SOFTWARE", true))
            using (RegistryKey ImgurScraper = SOFTWARE.OpenSubKey("ImgurScraper", true))
            {
                ImgurScraper.SetValue("path", path);
                ImgurScraper.SetValue("total_downloaded", 0);
                ImgurScraper.SetValue("total_invalid", 0);
            }
            total_invalid = 0;
            total_downloaded = 0;
            label6.Text = $"Invalid files received:\t{total_invalid}";
            label5.Text = $"Files downloaded:\t{total_downloaded}";
            label7.FadingText("Reseted");
        }

        private void button7_Click(object sender, EventArgs e)
        {
            using (RegistryKey SOFTWARE = Registry.CurrentUser.OpenSubKey("SOFTWARE", true))
            using (RegistryKey ImgurScraper = SOFTWARE.OpenSubKey("ImgurScraper", true))
            {
                ImgurScraper.SetValue("path", path);
                ImgurScraper.SetValue("total_downloaded", total_downloaded);
                ImgurScraper.SetValue("total_invalid", total_invalid);
            }
            label7.FadingText("Saved");
           
        }
    }
    #endregion

}
