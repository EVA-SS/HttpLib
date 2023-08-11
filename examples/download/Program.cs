// See https://aka.ms/new-console-template for more information

using HttpLib;


Http.Get("https://dldir1.qq.com/qqfile/qq/PCQQ9.7.3/QQ9.7.3.28946.exe")
       .redirect(true).DownLoad(new HttpDownOption(@"C:\Users\admin\Desktop\文档下载")
       {
           progres = (bytesSent, totalBytes) =>
           {
               Console.SetCursorPosition(0, 0);
               double prog = (bytesSent * 1.0) / (totalBytes * 1.0);
               Console.Write("{0}% 下载 {1}/{2}                  ", Math.Round(prog * 100.0, 1).ToString("N1"), CountSize(bytesSent), CountSize(totalBytes));
           }
       }).ContinueWith(savapath =>
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

//Http.Get("https://dldir1.qq.com/qqfile/qq/PCQQ9.7.3/QQ9.7.3.28946.exe")
//       .redirect(true)
//       .responseProgres((bytesSent, totalBytes) =>
//       {
//           Console.SetCursorPosition(0, 0);
//           if (totalBytes.HasValue)
//           {
//               double prog = (bytesSent * 1.0) / (totalBytes.Value * 1.0);
//               Console.Write("{0}% 下载 {1}/{2}                  ", Math.Round(prog * 100.0, 1).ToString("N1"), CountSize(bytesSent), CountSize(totalBytes.Value));
//           }
//           else
//           {
//               Console.Write("{0} 下载            ", CountSize(bytesSent));
//           }
//       }).download(@"C:\Users\admin\Desktop").ContinueWith(savapath =>
//       {
//           if (savapath.Result != null)
//           {
//               Console.WriteLine("下载成功保存至:" + savapath.Result.Data);
//           }
//           else
//           {
//               Console.WriteLine("下载失败");
//           }
//       }).Wait();

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