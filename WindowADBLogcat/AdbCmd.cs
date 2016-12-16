using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            p.StartInfo.Arguments = "-s " + form.getLogcatParams() + "logcat -c";    //设定程式执行參數             
            p.StartInfo.UseShellExecute = false;        //关闭Shell的使用            
            p.StartInfo.CreateNoWindow = true;          //设置不显示窗口   
            p.Start();
            p.WaitForExit();
            p.Close();
        }
        public void logcat(Form1 form)
        {
            clearLogcat(form);            
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
    }
}
