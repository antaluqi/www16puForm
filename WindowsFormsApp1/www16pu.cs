
using System;
using System.Drawing;
using System.Net;
using System.Text.RegularExpressions;
using System.IO;
using Common;
using System.Collections.Generic;
using System.Threading;

using HtmlAgilityPack;
using Http;
using RespInfo;


//-------------------------------------------------

/*                  十六铺接口                    */

//-------------------------------------------------

namespace WWW16pu
{
    public class www16pu
    {
        Requests r = new Requests();
        CookieContainer cookies = new CookieContainer();
        public string username = null;
        private string password = null;
        
        // 构造函数
        public www16pu(string username, string password)
        {
            this.username = username;
            this.password = password;
        }
        // 登陆
        public bool Login(ref string logRturnStr)
        {
            // 获取验证码图片
            string scURL = "https://www.16pu.com/safecode.jhtml?id=0.3594586764922329";
            Response resp = r.Get(scURL, "", ref cookies);
            Bitmap img = resp.getImg();
            // 识别验证码
            SafeCode SCRead = new SafeCode();
            string sc = SCRead.read2dig(img);
            // Post登陆，获取返回页面文本
            string logURL = "https://www.16pu.com/jsonloginzy.jhtml";
            string postDataStr = "username=" + username + "&password=" + password + "&safecode=" + sc;
            resp = r.Post(logURL, postDataStr, ref cookies);
            String reStr = resp.getText();
            // 读取登陆返回的Json字符串，判断是否登陆成功
            dynamic loginRturn = Comm.JsonRead(reStr);
            if (loginRturn["rmsg"] != "ok")
            {
                logRturnStr = loginRturn["rmsg"];
                return false;

            }
            Requests.SaveCookies(cookies, username);//保存成功登陆的Cookies文件
            logRturnStr = username + " 登陆成功";
            return true;
        }
        // 从Cookies登陆 
        public bool LoginFromCookie(ref string logRturnStr)
        {
            // 读取Cookies文件（.符号为上一级）
            string cookieFilePath = @".\Cookies\" + username + ".txt";

            // 如Cookies文件存在，则载入，反之则提示错误
            if (File.Exists(cookieFilePath))
            {
                cookies = Requests.ReadCookies(username);
                logRturnStr = username + "从Cookies登陆成功";
                return true;
            }
            else
            {
                logRturnStr = username + "从Cookies登陆失败，没有此用户的Cookies文件";
                return false;
            }

        }
        // 读取客户信息页面
        public MemberInfo Member()
        {
            string URL = "https://www.16pu.com/member/index.html";
            Response resp = r.Get(URL, "", ref cookies);
            StreamReader trStream = resp.getStream();
            return Read_Member(trStream);

        }
        // 读取客户投资列表
        public List<TzlbInfo> MemberTzlb()
        {
            string URL = "https://www.16pu.com/member/tzlb.jhtml";
            Response resp = r.Get(URL, "", ref cookies);
            StreamReader trStream = resp.getStream();
            return Read_MemberTzlb(trStream);
        }
        // 读取可投标的页面
        public List<PdetailInfo> Touzi()
        {
            string URL = "https://www.16pu.com/touzi.jhtml";
            Response resp = r.Get(URL, "", ref cookies);
            StreamReader trStream = resp.getStream();
            return Read_Touzi(trStream);
        }

        // 读取可投标的页面(读取离线网页，测试用)
        public List<PdetailInfo> Touzi2()
        {
            List<PdetailInfo> list = new List<PdetailInfo>();
            string URL = @"..\..\..\..\测试网页\未登陆状态下.标的网页.html";
            StreamReader streamReader= Comm.xmlRead(URL);
            PdetailInfo pdetailInfo= Read_Pdetail(streamReader);
            list.Add(pdetailInfo);
            return list;
        }


        // 单个标的信息页面读取
        public PdetailInfo Pdetail(string URL)
        {
            string Referer = @"https://www.16pu.com/touzi.jhtml";
            Response resp = r.Get(URL, "", ref cookies, Referer);
            StreamReader trStream = resp.getStream();
            PdetailInfo pdetailInfo = Read_Pdetail(trStream);
            return pdetailInfo;
        }
        // 购买
        public string Buy(double money, string ID)
        {
            string result = null;
            string buyURL = @"https://www.16pu.com/ctorder.jhtml?tzje=" + money.ToString() + "&cpbm=" + ID;
            string Referer = @"https://www.16pu.com/pdetail" + ID + ".jhtml";
            Response resp = r.Get(buyURL, "", ref cookies, Referer);
            string LocationURL = resp.response.Headers["Location"];
            if (LocationURL.IndexOf("success") != -1)
            {
                result = "OK";
            }
            resp = r.Get(LocationURL, "", ref cookies, Referer);
            StreamReader xml = resp.getStream();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(xml);
            HtmlNode node = doc.DocumentNode.SelectSingleNode(@".//div[@id=""MyDiv0""]/div[@class=""tc_cont_box""]/div[@class=""layer-xkzx-cont""]");
            if (node != null)
            {
                result = node.InnerText.Trim();

            }
            return result;
        }
        // 手动循环购买（测试用）
        public void LoopBuy(double money, string ID)
        {

            while (true)
            {
                string buResult = Buy(money, ID);
                Console.WriteLine(buResult);
                Console.WriteLine("按键继续或按'b'退出\n");
                if (buResult == "OK")
                {
                    Console.WriteLine("购买成功，退出循环\n");
                    break;
                }
                ConsoleKeyInfo c = Console.ReadKey();
                if (c.KeyChar == 'b')
                {
                    Console.WriteLine("已退出循环\n");
                    break;
                }
            }
        }
        // 自动购买（检测标的+等待时间+购买成功）
        public bool AutoBuy()
        {

            List<PdetailInfo> pdetailInfo = new List<PdetailInfo>();
            while (true)
            {
                pdetailInfo = Touzi();
                if (pdetailInfo.Count == 0)
                {
                    Console.WriteLine("没有可投标的");
                    Thread.Sleep(5000);
                    continue;

                }
                Console.WriteLine("发现可投标的:\n==========================\n");
                Console.WriteLine(pdetailInfo[0].ToString());

                break;

            }
            PdetailInfo pdetail = pdetailInfo[0];


            //----------------------------------------------------------------测试用
            //PdetailInfo pdetail = new PdetailInfo();
            //pdetail.URL = @"https://www.16pu.com/pdetail13178.jhtml";
            //pdetail.ID = "13178";
            //pdetail.Title = "CG20171010128 ";
            //pdetail.Rate = 0.11;
            //pdetail.Term = 28;
            //pdetail.StartTime = Convert.ToDateTime("2017-10-14 23:20:25");
            //pdetail.EndTime = Convert.ToDateTime("2017-11-10");
            //pdetail.Minimum = 1000;
            //pdetail.Increase = 100;
            //pdetail.Maximum = 50000;
            //pdetail.Balance = 20000;
            //pdetail.Amount = 300000;
            //----------------------------------------------------------------
            if (pdetail.Balance == -1)
            {
                string logInfo = "";

                while (!Login(ref logInfo))
                {
                    Console.WriteLine(logInfo);
                }
                pdetail = Pdetail(pdetail.URL);
                Console.WriteLine(pdetail.ToString());
            }

            if (pdetail.Balance < pdetail.Minimum)
            {
                Console.WriteLine("可用金额小于最小投资金额");
                return false;
            }

            double money = Math.Min(pdetail.Balance, Math.Min(pdetail.Maximum, pdetail.Amount));
            money = Math.Floor(money/100)*100;
            string ID = pdetail.ID;
            DateTime startTime = pdetail.StartTime;
            DateTime webTime = pdetail.Webtime;
            DateTime sysTime = pdetail.Systime;
            TimeSpan ts = startTime - DateTime.Now-(webTime-sysTime);
            double seconds = Math.Max(0, Math.Ceiling(ts.TotalSeconds));
            Console.WriteLine("投资标的{0}，投资金额{1}元，将于{2}秒后投标", ID, money, seconds);

            double s = seconds;
            while (s > 0)
            {
                Console.WriteLine("倒计时{0}", s);
                if (s == 2)
                {
                    string logInfo = "";
                    Login(ref logInfo);
                }
                Thread.Sleep(1000);
                s = s - 1;
            }

            Console.WriteLine("开始购买....");
            int reTry = 5;
            while (reTry > 0)
            {
                string r = Buy(money, ID);
                if (r == "OK")
                {

                    return true;
                }
                else
                {
                    Console.WriteLine(r);
                    reTry = reTry - 1;
                    Thread.Sleep(500);
                }
            }

            return false;
        }


        // <私有> 客户信息页面解析
        private MemberInfo Read_Member(StreamReader xml)
        {
            MemberInfo memberInfo = new MemberInfo();// 返回信息对象
            HtmlDocument doc = new HtmlDocument();   // 需要加载 HtmlAgilityPack.dll
            doc.Load(xml); // 加载页面
            //=========================================================================================
            HtmlNode node = doc.DocumentNode.SelectSingleNode(@".//div[@id=""yesx""]/../../p/em"); // 读取单个节点
            if (node != null)
            {
                string att = node.InnerText.Trim();
                memberInfo.balance = Convert.ToDouble(att);
            }
            //=========================================================================================
            node = doc.DocumentNode.SelectSingleNode(@".//div[@class=""zjgl-jzcze""]/p/em");
            if (node != null)
            {
                string att = node.InnerText.Trim();
                memberInfo.total_net_assets = Convert.ToDouble(att);
            }
            //=========================================================================================
            node = doc.DocumentNode.SelectSingleNode(@".//div[@class=""zjgl-zjxx""]/span[1]/p");
            if (node != null)
            {
                string att = node.InnerText.Trim();
                att = Regex.Replace(att, @"[\u4e00-\u9fa5]+", "").Trim(); // 将中文字符更换为空字符
                memberInfo.capital_on_call = Convert.ToDouble(att);
            }
            //=========================================================================================
            node = doc.DocumentNode.SelectSingleNode(@".//div[@class=""zjgl-zjxx""]/span[2]/p");
            if (node != null)
            {
                string att = node.InnerText;
                att = Regex.Replace(att, @"[\u4e00-\u9fa5]+", "").Trim();
                memberInfo.interest_received = Convert.ToDouble(att);
            }
            //=========================================================================================
            node = doc.DocumentNode.SelectSingleNode(@".//div[@class=""zjgl-zjxx""]/span[3]/p");
            if (node != null)
            {
                string att = node.InnerText;
                att = Regex.Replace(att, @"[\u4e00-\u9fa5]+", "").Trim();
                memberInfo.investment_amount = Convert.ToDouble(att);
            }
            //=========================================================================================
            node = doc.DocumentNode.SelectSingleNode(@".//div[@class=""zjgl-index-main-r""]/p");
            if (node != null)
            {
                string att = node.InnerText;
                att = Regex.Replace(att, @"[\u4e00-\u9fa5]+", "").Trim();
                memberInfo.accumulated_income = Convert.ToDouble(att);
            }

            //=========================================================================================

            return memberInfo;

        }
        // <私有> 客户投资列表页面解析
        private List<TzlbInfo> Read_MemberTzlb(StreamReader xml)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(xml);
            //=========================================================================================
            List<TzlbInfo> tzlbInfo = new List<TzlbInfo>();
            HtmlNodeCollection collection = doc.DocumentNode.SelectNodes(@".//div[contains(@class,""wdtzjl_list"") and @id]");
            if (collection != null)
            {
                foreach (HtmlNode Node in collection)
                {
                    TzlbInfo i_tzlbInfo = new TzlbInfo();
                    //=========================================================================================
                    HtmlNode node = Node.SelectSingleNode(".//ul/li[6]");
                    string att = node.InnerText.Trim();
                    if (att != "待还款")
                    {
                        break;
                    }
                    i_tzlbInfo.Status = att;
                    //=========================================================================================

                    node = Node.SelectSingleNode(".//ul/li[1]/a");
                    att = node.InnerText.Trim();
                    i_tzlbInfo.Title = att;
                    //=========================================================================================
                    node = Node.SelectSingleNode(".//ul/li[2]");
                    att = node.InnerText.Trim();
                    i_tzlbInfo.Amount = Convert.ToDouble(att);
                    //=========================================================================================
                    node = Node.SelectSingleNode(".//ul/li[3]");
                    att = node.InnerText.Trim();
                    i_tzlbInfo.Rate = Convert.ToDouble(att.Substring(0, att.Length - 1));
                    //=========================================================================================
                    node = Node.SelectSingleNode(".//ul/li[4]");
                    att = node.InnerText.Trim();
                    i_tzlbInfo.Term = Convert.ToInt16(att.Substring(0, att.Length - 1));
                    //=========================================================================================
                    node = Node.SelectSingleNode(".//ul/li[5]");
                    att = node.InnerText.Trim();
                    i_tzlbInfo.Interest = Convert.ToDouble(att);
                    //=========================================================================================
                    node = Node.SelectSingleNode(".//ul/li[7]");
                    att = node.InnerText.Trim();
                    i_tzlbInfo.StartTime = Convert.ToDateTime(att);
                    //=========================================================================================
                    node = Node.SelectSingleNode(".//ul/li[8]");
                    att = node.InnerText.Trim();
                    i_tzlbInfo.EndTime = Convert.ToDateTime(att);
                    //=========================================================================================
                    tzlbInfo.Add(i_tzlbInfo);
                }
            }
            return tzlbInfo;
        }
        // <私有> 可投标的页面解析
        private List<PdetailInfo> Read_Touzi(StreamReader xml)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.Load(xml);
            //=========================================================================================
            List<PdetailInfo> pdetailInfoList = new List<PdetailInfo>();
            HtmlNodeCollection collection = doc.DocumentNode.SelectNodes(@".//span[@class=""lb-kbsj""]/../../div/h3/a");
            if (collection != null)
            {
                foreach (HtmlNode node in collection)
                {
                    string att = node.Attributes["href"].Value;
                    string PdetailURL = "https://www.16pu.com/" + att;

                    PdetailInfo pdetailInfo = Pdetail(PdetailURL);
                    pdetailInfoList.Add(pdetailInfo);
                }

            }
            return pdetailInfoList;
        }
        // <私有> 可投标的页面解析
        private PdetailInfo Read_Pdetail(StreamReader xml)
        {
            PdetailInfo pdetailInfo = new PdetailInfo();

            HtmlDocument doc = new HtmlDocument();
            doc.Load(xml);
            //=========================================================================================系统时间
            pdetailInfo.Systime=DateTime.Now;
            //=========================================================================================pid
            HtmlNode Node = doc.DocumentNode.SelectSingleNode(@".//input[@id=""cpbm""]");
            string cpbm = Node.Attributes["value"].Value;
            pdetailInfo.ID = cpbm;
            pdetailInfo.URL = @"https://www.16pu.com/pdetail"+ cpbm + ".jhtml";
            //=========================================================================================标的编号
             Node = doc.DocumentNode.SelectSingleNode(@".//div[@class=""tit""]");
            if (Node != null)
            {
                string InnerText = Node.InnerText.Trim();
                MatchCollection collec = Regex.Matches(InnerText, @"(?<=日聚鑫\-)\w+");
                string str = collec[0].Value;
                pdetailInfo.Title = str;
            }
            //=========================================================================================年化收益
            Node = doc.DocumentNode.SelectSingleNode(@".//input[@id=""llje""]");
            string llje = Node.Attributes["value"].Value;
            Node = doc.DocumentNode.SelectSingleNode(@".//input[@id=""hbll""]");
            string hbll = Node.Attributes["value"].Value;
            pdetailInfo.Rate = Convert.ToDouble(llje) + Convert.ToDouble(hbll);

            //=========================================================================================期限
            Node = doc.DocumentNode.SelectSingleNode(@".//li[@class=""li-tzqx""]//span[@class=""szz""]");
            if (Node != null)
            {
                string InnerText = Node.InnerText.Trim();
                pdetailInfo.Term = Convert.ToInt16(InnerText);
            }

            //=========================================================================================开始时间
            Node = doc.DocumentNode.SelectSingleNode(@".//div[@class=""info-cont""]/ul/li[1]//following-sibling::text()[1]");
            if (Node != null)
            {
                string InnerText = Node.InnerText.Trim();
                pdetailInfo.StartTime = Convert.ToDateTime(InnerText);
            }
            //=========================================================================================结束时间
            Node = doc.DocumentNode.SelectSingleNode(@".//div[@class=""info-cont""]/ul/li[2]//following-sibling::text()[1]");
            if (Node != null)
            {
                string InnerText = Node.InnerText.Trim();
                MatchCollection collec = Regex.Matches(InnerText, @".*?(?=（)");
                string str = collec[0].Value;
                pdetailInfo.EndTime = Convert.ToDateTime(str);
            }
            //=========================================================================================最小投资
            Node = doc.DocumentNode.SelectSingleNode(@".//input[@id=""zdqt""]");
            string zdqt = Node.Attributes["value"].Value;
            pdetailInfo.Minimum = Convert.ToDouble(zdqt);
            //=========================================================================================最小递增
            Node = doc.DocumentNode.SelectSingleNode(@".//input[@id=""dzed""]");
            string dzed = Node.Attributes["value"].Value;
            pdetailInfo.Increase = Convert.ToDouble(dzed);
            //=========================================================================================最大可投
            Node = doc.DocumentNode.SelectSingleNode(@".//input[@id=""zdkt""]");
            string zdkt = Node.Attributes["value"].Value;
            pdetailInfo.Maximum = Convert.ToDouble(zdkt);
            //=========================================================================================可用余额
            Node = doc.DocumentNode.SelectSingleNode(@".//em[@id=""dzzhkyye""]");
            if (Node != null)
            {
                string InnerText = Node.InnerText.Trim();
                MatchCollection collec = Regex.Matches(InnerText, @".*?(?=元)");
                string str = collec[0].Value;
                pdetailInfo.Balance = Convert.ToDouble(str);
            }
            //=========================================================================================剩余可投
            Node = doc.DocumentNode.SelectSingleNode(@".//input[@id=""sykt""]");
            string sykt = Node.Attributes["value"].Value;
            pdetailInfo.Amount = Convert.ToDouble(sykt);
            //=========================================================================================此刻时间（网页）
            Node = doc.DocumentNode.SelectSingleNode(@".//div[@id=""timec""]");
            string webtime = Node.Attributes["curDate"].Value;
            pdetailInfo.Webtime = Convert.ToDateTime(webtime);

            return pdetailInfo;

        }
        // <静态公有> List<> 元素的预览
        public static void InfoListView<T>(List<T> list)
        {
            string str = "";
            foreach (var item in list)
            {
                str = str + "------------------------------------\n"
                      + item.ToString() + "\n";
            }
            Console.WriteLine(str);
        }


    }
}
