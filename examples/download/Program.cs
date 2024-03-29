﻿// See https://aka.ms/new-console-template for more information

using HttpLib;

Http.Get("https://dldir1.qq.com/qqfile/qq/PCQQ9.6.2/QQ9.6.2.28756.exe")
       .redirect(true)
       .responseProgres((bytesSent, totalBytes) =>
       {
           Console.SetCursorPosition(0, 0);
           if (totalBytes > 0)
           {
               double prog = (bytesSent * 1.0) / (totalBytes * 1.0);
               Console.Write("{0}% 下载 {1}/{2}                  ", Math.Round(prog * 100.0, 1).ToString("N1"), CountSize(bytesSent), CountSize(totalBytes));
           }
           else
           {
               Console.Write("{0} 下载            ", CountSize(bytesSent));
           }
       }).download(@"C:\Users\admin\Desktop", "qq.exe").ContinueWith(savapath =>
       {
           if (savapath.Result != null)
           {
               Console.WriteLine("下载成功保存至:" + savapath.Result);
           }
           else
           {
               Console.WriteLine("下载失败");
           }
       }).Wait();

Console.ReadLine();

static string CountSize(double Size)
{
    string houzui = "B";
    double FactSize = Size;
    if (FactSize >= 1024)
    {
        houzui = "K";
        FactSize = (FactSize / 1024.00);
    }
    if (FactSize >= 1024)
    {
        houzui = "M";
        FactSize = (FactSize / 1024.00);
    }
    if (FactSize >= 1024)
    {
        houzui = "G";
        FactSize = (FactSize / 1024.00);
    }
    if (FactSize >= 1024)
    {
        houzui = "T";
        FactSize = (FactSize / 1024.00);
    }
    return string.Format("{0} {1}", Math.Round(FactSize, 2), houzui);
}