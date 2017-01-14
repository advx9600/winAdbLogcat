using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowADBLogcat
{
    class Device
    {
        public String name;
    }

    class LogCatLog
    {

        public LogCatLog(string data)
        {
            // First char indicates error type:
            // W - warning
            // E - error
            // D - debug
            // I - info
            // V - verboseb
            // 编码转换            
            //byte[] bytes = System.Text.Encoding.Default.GetBytes(data);            
            //data = System.Text.Encoding.GetEncoding("UTF-8").GetString(bytes);

            originData = data;

            // 小米的log格式
            {
                if (data.StartsWith("D/") || data.StartsWith("I/") || data.StartsWith("W/") || data.StartsWith("V/") || data.StartsWith("E/") || data.StartsWith("F/"))
                {
                    Type = data.Substring(0, 1).ToCharArray()[0];
                    Tag = data.Split(':')[0].Substring(2);
                    Message = data.Substring(data.Split(':')[0].Length + 1);
                    CreationDate = string.Format("{0:MM-dd HH:mm:ss.fff}", DateTime.Now);
                    return;
                }
            }

            // 模拟器的log格式
            {

                try
                {
                    string[] strs = data.Split(' ');

                    CreationDate = strs[0] + " " + strs[1];
                    Type = strs[6].ToCharArray()[0];
                    Tag = strs[7].Replace(":", "");
                    {
                        String removeDateStr = data.Substring(20);
                        Message = removeDateStr.Substring(removeDateStr.Split(':')[0].Length + 2);
                    }
                }
                catch (Exception e)
                {

                }
            }

        }

        public String Tag;
        public string CreationDate;
        public char Type;
        public string Message;
        public string originData;

        public Color GetBgColor()
        {
            switch (Type)
            {
                case 'W':
                    return Color.FromArgb(174, 108, 108);
                case 'I':
                    return Color.Green;

                case 'E':
                    return Color.Red;

                case 'D':
                    return Color.Blue;

                case 'V':
                default:
                    return Color.Gray;
            }
        }

        public bool contains(String msg)
        {
            if (!String.IsNullOrEmpty(msg))
            {
                if (!String.IsNullOrEmpty(Tag) && Tag.Contains(msg))
                {
                    return true;
                }
                if (!String.IsNullOrEmpty(Message) && Message.Contains(msg))
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
            return false;
        }
    }



    class AdbCmd
    {
        private String CMD = "adb";
        private Process logCatProcess;
        private String curDev;

        public List<Device> adbDevices()
        {
            Process p = new Process();
            p.StartInfo.FileName = CMD;           //设定程序名  
            p.StartInfo.Arguments = "devices";    //设定程式执行參數  
            p.StartInfo.UseShellExecute = false;        //关闭Shell的使用  
            p.StartInfo.RedirectStandardInput = true;   //重定向标准输入  
            p.StartInfo.RedirectStandardOutput = true;  //重定向标准输出  
            p.StartInfo.RedirectStandardError = true;   //重定向错误输出  
            p.StartInfo.CreateNoWindow = true;          //设置不显示窗口  
            p.Start();
            //this.label1.Text = p.StandardOutput.ReadToEnd();
            String info = p.StandardOutput.ReadToEnd();
            p.Close();

            List<Device> list = new List<Device>();
            if (info != null && info.Length > 0)
            {
                String[] devices = info.Replace("\r\n", "@").Split('@');
                for (int i = 1; i < devices.Length; i++)
                {
                    if (devices[i] == null || devices[i].Trim().Length == 0) continue;

                    if (devices[i].Contains("daemon")) continue;

                    String[] item = devices[i].Split('\t');
                    if (item != null && item[0].Trim().Length > 0)
                    {
                        String name = item[0];
                        String val = item[1].Trim();
                        if (val.Equals("device"))
                        {
                            Device dev = new Device();
                            dev.name = name;
                            list.Add(dev);
                        }
                    }
                }
            }
            if (list.Count == 0)
            {
                return null;
            }
            return list;
        }


        private void clearLogcat(Form1 form)
        {
            Process p = new Process();
            p.StartInfo.FileName = CMD;           //设定程序名  
            p.StartInfo.Arguments = "-s " + form.getLogcatParams() + " logcat -c";    //设定程式执行參數             
            p.StartInfo.UseShellExecute = false;        //关闭Shell的使用            
            p.StartInfo.CreateNoWindow = true;          //设置不显示窗口   
            p.Start();
            p.WaitForExit();
            p.Close();
        }
        public void logcat(Form1 form)
        {
            clearLogcat(form);
            curDev = form.getLogcatParams();
            Process p = new Process();
            p.StartInfo.FileName = CMD;           //设定程序名  
            p.StartInfo.Arguments = "-s " + form.getLogcatParams() + " logcat";    //设定程式执行參數             
            p.StartInfo.UseShellExecute = false;        //关闭Shell的使用  
            p.StartInfo.RedirectStandardInput = true;   //重定向标准输入  
            p.StartInfo.RedirectStandardOutput = true;  //重定向标准输出  
            p.StartInfo.RedirectStandardError = true;   //重定向错误输出  
            p.StartInfo.CreateNoWindow = true;          //设置不显示窗口  
            p.StartInfo.StandardOutputEncoding = Encoding.UTF8; // 一定要事先设置编码

            p.OutputDataReceived += (sender, outputLine) =>
            {
                if (outputLine.Data != null && outputLine.Data.Length > 2)
                {
                    String data = outputLine.Data;
                    if (data.StartsWith("---"))
                    {
                        return;
                    }

                    LogCatLog log = new LogCatLog(data);
                    form.receive_data(log);

                }
            };

            p.Start();
            p.BeginOutputReadLine();

            killLogcatProcess();

            logCatProcess = p;
        }

        private void killLogcatProcess()
        {
            if (logCatProcess != null && !logCatProcess.HasExited)
            {
                logCatProcess.Kill();
                logCatProcess = null;
            }
        }

        public void Close()
        {
            killLogcatProcess();
        }

        public void onKeyDown(Keys keyCode)
        {
            int key = ConvertKeyCode(keyCode);
            Debug.WriteLine(keyCode);


            if (key > 0)
            {
                Process p = new Process();
                p.StartInfo.FileName = CMD;           //设定程序名  
                p.StartInfo.Arguments = "-s " + curDev + " shell input keyevent " + key;    //设定程式执行參數             
                p.StartInfo.UseShellExecute = false;        //关闭Shell的使用            
                p.StartInfo.CreateNoWindow = true;          //设置不显示窗口   
                p.Start();
            }
        }

        public static int ConvertKeyCode(Keys keyCode)
        {
            int key = 0;
            switch (keyCode)
            {
                case Keys.Apps:
                case Keys.F2:
                    key = 1;
                    break;
                case Keys.Home:
                    key = 3;
                    break;
                case Keys.Escape:
                    key = 4;
                    break;
                case Keys.D0:
                case Keys.NumPad0:
                    key = 7;
                    break;
                case Keys.D1:
                case Keys.NumPad1:
                    key = 8;
                    break;
                case Keys.D2:
                case Keys.NumPad2:
                    key = 9;
                    break;
                case Keys.D3:
                case Keys.NumPad3:
                    key = 10;
                    break;
                case Keys.D4:
                case Keys.NumPad4:
                    key = 11;
                    break;
                case Keys.D5:
                case Keys.NumPad5:
                    key = 12;
                    break;
                case Keys.D6:
                case Keys.NumPad6:
                    key = 13;
                    break;
                case Keys.D7:
                case Keys.NumPad7:
                    key = 14;
                    break;
                case Keys.D8:
                case Keys.NumPad8:
                    key = 15;
                    break;
                case Keys.D9:
                case Keys.NumPad9:
                    key = 16;
                    break;
                case Keys.Up:
                    key = 19;
                    break;
                case Keys.Down:
                    key = 20;
                    break;
                case Keys.Left:
                    key = 21;
                    break;
                case Keys.Right:
                    key = 22;
                    break;
                case Keys.Enter:
                    key = 23;
                    break;
                case Keys.A:
                    key = 29;
                    break;                
                case Keys.B:
                    key = 30;
                    break;
                case Keys.C:
                    key = 31;
                    break;
                case Keys.D:
                    key = 32;
                    break;
                case Keys.E:
                    key = 33;
                    break;
                case Keys.F:
                    key = 34;
                    break;
                case Keys.G:
                    key = 35;
                    break;
                case Keys.H:
                    key = 36;
                    break;
                case Keys.I:
                    key = 37;
                    break;
                case Keys.J:
                    key = 38;
                    break;
                case Keys.K:
                    key = 39;
                    break;
                case Keys.L:
                    key = 40;
                    break;
                case Keys.M:
                    key = 41;
                    break;
                case Keys.N:
                    key = 42;
                    break;
                case Keys.O:
                    key = 43;
                    break;
                case Keys.P:
                    key = 44;
                    break;
                case Keys.Q:
                    key = 45;
                    break;
                case Keys.R:
                    key = 46;
                    break;
                case Keys.S:
                    key = 47;
                    break;
                case Keys.T:
                    key = 48;
                    break;
                case Keys.U:
                    key = 49;
                    break;
                case Keys.V:
                    key = 50;
                    break;
                case Keys.W:
                    key = 51;
                    break;
                case Keys.X:
                    key = 52;
                    break;
                case Keys.Y:
                    key = 53;
                    break;
                case Keys.Z:
                    key = 54;
                    break;
                case Keys.Tab:
                    key = 61;
                    break;
                case Keys.Space:
                    key = 62;
                    break;
            }
            return key;
        }
    }
}
