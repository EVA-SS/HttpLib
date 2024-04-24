// See https://aka.ms/new-console-template for more information

using HttpLib;

var savapath = Http.Get("https://dldir1.qq.com/qqfile/qq/QQNT/Windows/QQ_9.9.9_240422_x64_01.exe")
       .redirect()
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
       }).download(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "qq.exe");

if (savapath != null) Console.WriteLine("下载成功保存至:" + savapath);
else Console.WriteLine("下载失败");


Console.ReadLine();

static string CountSize(double Size)
{
    string houzui = "B";
    double FactSize = Size;
    if (FactSize >= 1024)
    {
        houzui = "K";
        FactSize /= 1024.00;
    }
    if (FactSize >= 1024)
    {
        houzui = "M";
        FactSize /= 1024.00;
    }
    if (FactSize >= 1024)
    {
        houzui = "G";
        FactSize /= 1024.00;
    }
    if (FactSize >= 1024)
    {
        houzui = "T";
        FactSize /= 1024.00;
    }
    return string.Format("{0} {1}", Math.Round(FactSize, 2), houzui);
}