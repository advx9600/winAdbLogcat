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
                comboBoxDevices.DataSource = new String[] { "没有设备" };
                return;
            }


            buttonSwitch.Text = "Stop";

            comboBoxDebugLevel.DataSource = new String[] { "Verbose", "Info", "Warning", "Err" };

            adb.logcat(this);

        }

        private int mReMainNum;
        /* 回调函数 
         *  aa | bb 显示包含aa或bb的数据
         *  aaaa n 3为 包含aaa接下来的3行显示
         */
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

                mReMainNum--;

                // 过滤字符串
                if (mReMainNum > 0 || !String.IsNullOrEmpty(searchData.filterStr))
                {
                    String filteStr = searchData.filterStr.Trim();
                    if (filteStr.Contains(" | ")) //正则表达
                    {
                        String[] strs = filteStr.Split(new char[3] { ' ', '|', ' ' });
                        bool isReturn = true;
                        for (int i = 0; i < strs.Length; i++)
                        {
                            if (String.IsNullOrEmpty(strs[i].Trim())) continue;

                            if (log.Tag.Contains(strs[i]) || log.Message.Contains(strs[i]))
                            {
                                isReturn = false;
                                break;
                            }
                        }
                        if (isReturn) return;
                    }
                    else if (filteStr.Contains(" n ")) // aaaa n 3为 包含aaa接下来的3行显示
                    { // 包含空格
                        String[] strs = filteStr.Split(new char[3] { ' ', 'n', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (!log.Tag.Contains(strs[0]) && !log.Message.Contains(strs[0]))
                        {
                            return;
                        }
                        else
                        {
                            if (strs.Length > 1)
                                mReMainNum = Int32.Parse(strs[1]);
                        }
                    }
                    else if (!log.Tag.Contains(searchData.filterStr) && !log.Message.Contains(searchData.filterStr))
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

        private Timer timerTxtDelay;

        private void textBoxFilter_TextChanged(object sender, EventArgs e)
        {
            if (timerTxtDelay != null)
            {
                timerTxtDelay.Stop();
                timerTxtDelay = null;
            }

            timerTxtDelay = new Timer();
            timerTxtDelay.Interval = 700;
            timerTxtDelay.Tick += new EventHandler(timer_Tick);
            timerTxtDelay.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (timerTxtDelay != null)
            {
                timerTxtDelay.Stop();
                timerTxtDelay = null;
            }
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
