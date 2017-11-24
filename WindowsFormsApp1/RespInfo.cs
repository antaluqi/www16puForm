using System;


namespace RespInfo
{

    // 客户信息返回类
    public class MemberInfo
    {
        public double balance=-1;             //账户余额
        public double total_net_assets=-1;    //净资产总额
        public double capital_on_call=-1;     //代收本金
        public double interest_received=-1;   //代收利息
        public double investment_amount=-1;   //投资金额
        public double accumulated_income=-1;  //累计收益

        public override string ToString()
        {
            string str = "账户余额：" + balance + "\n" +
                         "净资产总额：" + total_net_assets + "\n" +
                         "代收本金：" + capital_on_call + "\n" +
                         "代收利息：" + interest_received + "\n" +
                         "投资金额：" + investment_amount + "\n" +
                         "累计收益：" + accumulated_income + "\n";
            return str;
        }
        
    }
    // 客户投资列表信息返回类（单个标的）
    public class TzlbInfo
    {
        public string Title;         //标的编号
        public double Amount=-1;        //投资金额
        public double Rate=-1;          //年化收益
        public int Term=-1;             //期限
        public double Interest=-1;      //待收利息
        public string Status;        //标的状态
        public DateTime StartTime;   //开始时间
        public DateTime EndTime;     //结束时间

        public override string ToString()
        {
            string str = "标的编号：" + Title + "\n" +
                         "投资金额：" + Amount + "\n" +
                         "年化收益：" + Rate + "\n" +
                         "期限：" + Term + "\n" +
                         "待收利息：" + Interest + "\n" +
                         "标的状态:" + Status + "\n" +
                         "开始时间：" + StartTime + "\n" +
                         "结束时间：" + EndTime;
            return str;
        }
    }
    // 可投标的信息返回类（单个标的）
    public class PdetailInfo
    {
        public string ID;           //标的购买码
        public string URL;          //标的网页
        public string Title;        //标的编号
        public double Rate=-1;         //年化收益
        public int Term=-1;            //期限
        public DateTime StartTime;  //开始时间
        public DateTime EndTime;    //结束时间
        public double Minimum=-1;      //最低起投
        public double Increase=-1;     //递增金额
        public double Maximum=-1;      //最高可投
        public double Balance=-1;      //可用余额
        public double Amount=-1;       //剩余可投
        public DateTime Webtime;         //网页显示时间
        public DateTime Systime;         //系统时间

        public override string ToString()
        {
            string str = "ID:" + ID + "\n" +
                       "URL：" + URL + "\n" +
                       "标的编号：" + Title + "\n" +
                       "年化收益：" + Rate + "\n" +
                       "期限：" + Term + "\n" +
                       "开始时间：" + StartTime + "\n" +
                       "结束时间：" + EndTime + "\n" +
                       "最低起投：" + Minimum + "\n" +
                       "递增金额：" + Increase + "\n" +
                       "最高可投：" + Maximum + "\n" +
                       "可用余额：" + Balance + "\n" +
                       "网页时间：" + Webtime + "\n" +
                       "系统时间：" + Systime + "\n" +
                       "剩余可投：" + Amount;

            return str;
        }
    }
     
    

}