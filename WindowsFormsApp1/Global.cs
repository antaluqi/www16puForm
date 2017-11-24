using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using WWW16pu;
using RespInfo;
namespace Global
{
    class GlobalV
    {
        public static List<User> userList = new List<User>();
        public static List<PdetailInfo> pidList = new List<PdetailInfo>();

        
        public GlobalV()
        {
            userList = null;
            pidList = null;

        }
           

    }

    public class User
    {
        private int id;
        private PdetailInfo pid;
        private www16pu obj;
        private double money;
        private ListViewItem item=null;
        private Thread BuyThread;

        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                if (Item!=null)
                {
                    Item.SubItems[0].Text = value.ToString();
                }
                id = value;
            }
        }

        public PdetailInfo PID
        {
            get
            {
                return pid;
            }
            set
            {
                if (Item != null)
                {
                    Item.SubItems[3].Text = value.ID.ToString();
                    if(Obj!=null)
                    {
                        double bal = Math.Floor(Obj.Member().balance/100)*100;
                        double max = value.Maximum;
                        double min = value.Minimum;
                        double amount = value.Amount;
                        if(bal<min)
                        {
                            Money = 0;
                        }
                        else
                        {
                            Money= Math.Min(bal, Math.Min(max, amount));
                        }

                    }
                }
                pid = value;
            }
        }

        public www16pu Obj
        {
            get
            {
                return obj;
            }
            set
            {
                if (Item != null)
                {
                    Item.SubItems[1].Text = value.username;
                    MemberInfo memberInfo = value.Member();
                    Item.SubItems[2].Text = memberInfo.balance.ToString();

                }
                obj = value;
            }
        }

        public double Money
        {
            get
            {
                return money;
            }
            set
            {
                if (Item != null)
                {
                    Item.SubItems[4].Text = value.ToString();
                }
                money = value;
            }
        }

        public ListViewItem Item
        {
            get
            {
                if(item==null)
                {
                    item = new ListViewItem();
                    item.SubItems.Clear();
                    item.SubItems[0].Text = (ID == 0) ? "" : ID.ToString();
                    item.SubItems.Add((Obj == null) ? "" : Obj.username);
                    item.SubItems.Add((Obj == null) ? "" : Obj.Member().balance.ToString());
                    item.SubItems.Add((PID == null) ? "" : PID.ID);
                    item.SubItems.Add((Money == 0) ? "" : Money.ToString());
                    item.SubItems.Add("");
                    
                }
                return item;
            }
            set
            {

            }

        }

        public void reLogein()
        {
            string reStr="";
            Obj.Login(ref reStr);
            Item.SubItems[5].Text =DateTime.Now.ToLongTimeString()+":  "+ Obj.username + "执行登陆";
        }

        public void BuyThreadStartFun(string pid)
        {
            if (Money>0)
            {
                BuyThread?.Abort();
                BuyThread = new Thread(delegate () { test(pid); });
                BuyThread.Start();
            }
            
        }

        public void  test(string pid)
        {
            Item.SubItems[5].Text = DateTime.Now.ToLongTimeString()+":  " + Obj.username + "启动" + pid + "标的购买。";
            int reTry = 10;
            while (reTry > 0)
            {
                string reStr = Obj.Buy(Money, pid);
                Item.SubItems[5].Text = DateTime.Now.ToLongTimeString() + ":  重试第" + (10 - reTry).ToString() + "次：" + reStr;
                if (reStr == "OK")
                {
                    Item.SubItems[5].Text = DateTime.Now.ToLongTimeString() + "购买成功";
                    break;
                }
                reTry = reTry - 1;
                Thread.Sleep(300);

            }


        }
    }

}