using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using WWW16pu;
using RespInfo;
using Global;

namespace WindowsFormsApp1
{
    public partial class main_Form : Form
    {
        public delegate void TimeUpHandler(string pid);
        public event TimeUpHandler TimeUpEvent;  // 倒计时结束事件

        //public delegate void UserLogHandler();
        public event Action UserLogEvent;  // 用户重新登陆事件

        private Thread timeCountThread;

        public main_Form()
        {
            InitializeComponent();

            //----------------------------------------------------
            user_listView.GridLines = true;  // 表格是否显示网格线
            user_listView.FullRowSelect = true;  //  是否选中整行
            user_listView.Scrollable = true;  // 是否自动显示滚动条
            user_listView.MultiSelect = false;  // 是否可以选择多行

            user_listView.Columns.Add("ID","序号");
            user_listView.Columns.Add("TEL","手机");
            user_listView.Columns.Add("BALACE","可用余额");
            user_listView.Columns.Add("PID", "投资标的");
            user_listView.Columns.Add("MONEY", "购买金额");
            user_listView.Columns.Add("INFO", "最后信息");


            user_listView.Columns["ID"].Width = 50;   // -2:根据标题设置宽度,-1:根据内容设置宽度
            user_listView.Columns["TEL"].Width = 100; 
            user_listView.Columns["BALACE"].Width = 80;
            user_listView.Columns["PID"].Width = 80;
            user_listView.Columns["MONEY"].Width = 80;
            user_listView.Columns["INFO"].Width = 200;

            user_listView.View = View.Details; // 显示方式
            //----------------------------------------------------
            pid_listView.GridLines = true;  // 表格是否显示网格线
            pid_listView.FullRowSelect = true;  //  是否选中整行
            pid_listView.Scrollable = true;  // 是否自动显示滚动条
            pid_listView.MultiSelect = false;  // 是否可以选择多行

            pid_listView.Columns.Add("ID", "序号");
            pid_listView.Columns.Add("TimeDown", "倒计时");
            pid_listView.Columns.Add("Title", "标的编号");
            pid_listView.Columns.Add("Rate", "年化收益");
            pid_listView.Columns.Add("Term", "期限");
            pid_listView.Columns.Add("StartTime", "开始时间");
            pid_listView.Columns.Add("EndTime", "结束时间");
            pid_listView.Columns.Add("Minimum", "最低起投");
            pid_listView.Columns.Add("Increase", "递增金额");
            pid_listView.Columns.Add("Maxnum", "最高可投");
            pid_listView.Columns.Add("Amount", "剩余可投");


            pid_listView.Columns["ID"].Width = 50;   // -2:根据标题设置宽度,-1:根据内容设置宽度
            pid_listView.Columns["TimeDown"].Width = 50;
            pid_listView.Columns["Title"].Width = 120;
            pid_listView.Columns["Rate"].Width = 80;
            pid_listView.Columns["Term"].Width = 50;
            pid_listView.Columns["StartTime"].Width = 120;
            pid_listView.Columns["EndTime"].Width = 100;
            pid_listView.Columns["Minimum"].Width = 80;
            pid_listView.Columns["Increase"].Width = 80;
            pid_listView.Columns["Maxnum"].Width = 80;
            pid_listView.Columns["Amount"].Width = 80;

            pid_listView.View = View.Details; // 显示方式
            //----------------------------------------------------
        }

        private void test_button_Click(object sender, EventArgs e)
        {
             www16pu a = new www16pu("1234567789", "123456");
           // List<PdetailInfo> pdetailInfoList = a.Touzi2();
            List<PdetailInfo> pdetailInfoList = a.Touzi();
            if (pdetailInfoList.Count>0)
            {
                //pdetailInfoList[0].StartTime = DateTime.Now.AddSeconds(10); //测试用

                flashPidList(pdetailInfoList);
                timeCountThread?.Abort();
                timeCountThread = new Thread(delegate () { timeCount(pdetailInfoList[0]); });
                timeCountThread.Start();
            }

        }

        private void add_button1_Click(object sender, EventArgs e)
        {
            addUser_Form addUser_Form = new addUser_Form();
            addUser_Form.ShowDialog(this);
        }

        private void delete_button_Click(object sender, EventArgs e)
        {
            if(user_listView.SelectedItems.Count>0)
            {
                int id = Convert.ToInt16(user_listView.SelectedItems[0].SubItems[0].Text);
                var objs = GlobalV.userList.Where(u => { return u.ID == id; });
                removeUserList(objs.First());
            }
        }

        private void main_Form_Load(object sender, EventArgs e)
        {

        }

        public void addUserList(www16pu www16puObj)
        {
            User user = new User();
            if (GlobalV.userList.Count == 0)
            {
                user.ID = 1;
            }
            else
            {
                user.ID = (GlobalV.userList.Select(ul => ul.ID).Max() + 1);
            }
            user.Obj = www16puObj;
            GlobalV.userList.Add(user);
            user_listView.Items.Add(user.Item);
            //TimeUpEvent += new TimeUpHandler(user.test); // 加入倒计时结束事件的订阅
            TimeUpEvent += new TimeUpHandler(user.BuyThreadStartFun); // 加入倒计时结束事件的订阅
            UserLogEvent += user.reLogein; // 加入重新登陆事件订阅
        }

        public void removeUserList(User user)
        {
            GlobalV.userList.Remove(user);
            ListViewItem item=user_listView.FindItemWithText(user.Obj.username);
            item.Remove();
        }

        public void removeUserList(www16pu user)
        {
            var objs = GlobalV.userList.Where(u => { return u.Obj.username == user.username;});
            GlobalV.userList.Remove(objs.First());
            ListViewItem item = user_listView.FindItemWithText(user.username);
            item.Remove();
        }

        public void flashPidList(List<PdetailInfo> pidList)
        {
            pid_listView.Items.Clear();

            if (pidList.Count() > 0)
            {
                if(timeCountThread!=null && timeCountThread.ThreadState==0)
                {
                    timeCountThread.Abort();
                }
                MessageBox.Show(pidList[0].ToString());
                foreach (var pid in pidList)
                {
                    ListViewItem item = new ListViewItem();
                    item.SubItems.Clear();
                    item.SubItems[0].Text = pid.ID;
                    item.SubItems.Add("");
                    item.SubItems.Add(pid.Title);
                    item.SubItems.Add(pid.Rate.ToString() + "%");
                    item.SubItems.Add(pid.Term.ToString() + "天");
                    item.SubItems.Add(pid.StartTime.ToString());
                    item.SubItems.Add(pid.EndTime.ToString());
                    item.SubItems.Add(pid.Minimum.ToString());
                    item.SubItems.Add(pid.Increase.ToString());
                    item.SubItems.Add(pid.Maximum.ToString());
                    item.SubItems.Add(pid.Amount.ToString());

                    pid_listView.Items.Add(item);
                }
                if(GlobalV.userList.Count>0)
                {
                    foreach(var user in GlobalV.userList)
                    {
                        user.PID = pidList[0];
                    }
                }
               


            }
            

        }

        public void timeCount(PdetailInfo pdetailInfo)
        {
            
            CheckForIllegalCrossThreadCalls = false;
            UserLogEvent?.Invoke();
            //TimeSpan ts = pdetailInfo.StartTime - DateTime.Now;
            TimeSpan ts = pdetailInfo.StartTime - DateTime.Now-(pdetailInfo.Webtime-pdetailInfo.Systime);
            double seconds = Math.Max(0, Math.Ceiling(ts.TotalSeconds));
            double s = seconds;
            bool isRelogin = false;
            while(s>0)
            {
                s = s - 1;
               ListViewItem item=pid_listView.FindItemWithText(pdetailInfo.ID);
                if(item!=null)
                {
                   double H = Math.Floor(s / 3600);
                   double M = Math.Floor((s-H*3600)/60);
                   double S = s - M * 60 - H * 3600;
                    //item.SubItems[1].Text = s.ToString();
                    item.SubItems[1].Text = H.ToString() + "小时 " + M.ToString() + "分钟 " + S.ToString() + "秒";
                }
                if(s<10 && isRelogin==false)
                {
                    UserLogEvent?.Invoke();
                    isRelogin = true;
                }
               
               Thread.Sleep(1000);
               
            }
            TimeUpEvent?.Invoke(pdetailInfo.ID);// 发布信号
            //CheckForIllegalCrossThreadCalls = true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            timeCountThread?.Abort();
            base.OnClosing(e);
        }

    }

    


}
