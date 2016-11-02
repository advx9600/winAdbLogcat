using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowADBLogcat
{
    public partial class Form1 : Form
    {
        public delegate void LogAppendDelegate(Color color, string text);

        public void LogAppend(Color color, string text)
        {
            txtLogcat.AppendText("\r\n");
            txtLogcat.SelectionColor = color;
            txtLogcat.AppendText(text);
        }

        class SearchData
        {
            public String filterStr;
            public String logLevel;
            public bool isStop = false;

            public void Reset()
            {
                filterStr = null;
                logLevel = null;
                isStop = false;
            }
        }

        private AdbCmd adb;
        private SearchData searchData = new SearchData();

        private List<LogCatLog> listLogs = new List<LogCatLog>();

        public Form1()
        {
            InitializeComponent();
            adb = new AdbCmd();
            ResetAdb();
        }

        private void ResetAdb(bool isResetDevice = true)
        {
            listLogs.Clear();
            txtLogcat.Clear();
            textBoxFilter.Clear();
            searchData.Reset();

            List<Device> list = adb.adbDevices();
            if (list != null && list.Count > 0)
            {
                if (isResetDevice)
                {
                    List<string> comString = new List<string>();
                    foreach (Device dev in list)
                    {
                        comString.Add(dev.name);
                    }

                    comboBoxDevices.DataSource = comString;
                }
            }
            else
            {
                comboBoxDevices.DataSource = new String[]{"没有设备"};
                return;
            }


            buttonSwitch.Text = "Stop";

            comboBoxDebugLevel.DataSource = new String[] { "Verbose", "Info", "Warning", "Err" };

            adb.logcat(this);

        }

        /* 回调函数 */
        public void receive_data(Object data, Boolean isAddList = true)
        {
            LogCatLog log = (LogCatLog)data;

            if (searchData != null && searchData.isStop && isAddList) return;

            if (isAddList)
            {
                if (isAddList) listLogs.Add(log);
                if (listLogs.Count > 2000)
                {
                    listLogs.RemoveAt(0);
                }
            }

            if (searchData != null)
            {
                // 过滤字符串
                if (!String.IsNullOrEmpty(searchData.filterStr))
                {
                    if (!log.Tag.Contains(searchData.filterStr) && !log.Message.Contains(searchData.filterStr))
                    {
                        return;
                    }                    

                }

                // 过滤debug level
                if (!String.IsNullOrEmpty(searchData.logLevel))
                {
                    switch (searchData.logLevel.ToCharArray()[0])
                    {
                        case 'E':
                            if (log.Type != 'E') return;
                            break;
                        case 'W':
                            if (log.Type != 'E' && log.Type != 'W') return;
                            break;
                        case 'I':
                            if (log.Type == 'V') return;
                            break;
                    }
                }
            }
            LogAppendDelegate la = new LogAppendDelegate(LogAppend);
            txtLogcat.Invoke(la, log.GetBgColor(), log.CreationDate + " " + log.Tag + " " + log.Message);
        }
        /* 回调函数 */
        public String getLogcatParams()
        {
            return comboBoxDevices.Text;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            txtLogcat.Width = Width - 30;
            txtLogcat.Height = Height - 110;
        }

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            adb.Close();
        }

        private void buttonReset_Click(object sender, EventArgs e)
        {
            ResetAdb();
        }

        private void ResetLogcat()
        {
            searchData.logLevel = comboBoxDebugLevel.Text;
            searchData.filterStr = textBoxFilter.Text;
            txtLogcat.Clear();
            for (int i = 0; i < listLogs.Count; i++)
            {
                receive_data(listLogs[i], false);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            txtLogcat.Text = "";
            listLogs.Clear();
        }

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            ResetLogcat();
        }

        private void comboBoxDebugLevel_SelectedValueChanged(object sender, EventArgs e)
        {
            ResetLogcat();
        }


        private void comboBoxDevices_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ResetAdb(false);
        }

        private void buttonSwitch_Click(object sender, EventArgs e)
        {
            searchData.isStop = !searchData.isStop;
            if (searchData.isStop)
            {
                buttonSwitch.Text = "Start";
            }
            else
            {
                buttonSwitch.Text = "Stop";
            }
        }
    }
}
