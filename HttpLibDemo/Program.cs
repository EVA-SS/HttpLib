using HttpLib;
using System;
using System.Collections.Generic;
using System.Net;

namespace HttpLibDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            List<Val> headerss = new List<Val> {
                new Val("accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"),
                new Val("user-agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36 Edg/86.0.622.63"),
            };
            Http.Get("http://www.baidu.com").header(headerss)
                .header(new { userAgent = "测试1", accept = "*/*" })
                   //.redirect(true)
                   .requestProgress((bytesSent, totalBytes) =>
                   {
                       double dsa = (bytesSent * 1.0) / (totalBytes * 1.0);

                       int top = 0;
                       Console.SetCursorPosition(0, top);
                       Console.Write("{0}% 上传 {1}/{2}            ", (Math.Round(dsa, 2) * 100.0), CountSize(bytesSent), CountSize(totalBytes));

                   })
                   .responseProgress((bytesSent, totalBytes) =>
                   {
                       int top = 2;
                       Console.SetCursorPosition(0, top);
                       if (totalBytes.HasValue)
                       {
                           double dsa = (bytesSent * 1.0) / (totalBytes.Value * 1.0);
                           Console.Write("{0}% 下载 {1}/{2}                  ", (Math.Round(dsa, 2) * 100.0), CountSize(bytesSent), CountSize(totalBytes.Value));
                       }
                       else
                       {
                           Console.Write("{0} 下载            ", CountSize(bytesSent));
                       }
                   })
                   .success((WebResult r,string a) =>
                   {
                       Console.SetCursorPosition(0, 5);
                       Console.Write(a);
                   }).fail((Exception ez) =>
                {
                    Console.Write(ez.GetType());
                    Console.Write(ez.Message);
                }).RequestAsync();

            Console.ReadLine();
            return;

            List<Val> headers = new List<Val> {
                new Val("accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"),
                new Val("user-agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36 Edg/86.0.622.63"),
            };
            Http.Post("http://localhost:61489/api/values").header(headers)
                .data(new List<Files> { new Files(@"C:\Users\ttgx\Desktop\Flowing.exe") })
                .data(new List<Files> { new Files(@"C:\Users\ttgx\Desktop\ABT44B9D51AC6E96DA9220E39EED30D1945666375E8401AD7DE974193FD48211E72.jfif") })
                .data(new Val("abc", "123"))
                .data(new Val("abc1", "1234"))
                .requestProgress((bytesSent, totalBytes) =>
                {
                    double dsa = (bytesSent * 1.0) / (totalBytes * 1.0);

                    int top = 0;
                    Console.SetCursorPosition(0, top);
                    Console.Write("{0}% 上传 {1}/{2}            ", (Math.Round(dsa, 2) * 100.0), CountSize(bytesSent), CountSize(totalBytes));

                })
                .responseProgress((bytesSent, totalBytes) =>
                {
                    int top = 2;
                    Console.SetCursorPosition(0, top);
                    if (totalBytes.HasValue)
                    {
                        double dsa = (bytesSent * 1.0) / (totalBytes.Value * 1.0);
                        Console.Write("{0}% 下载 {1}/{2}                  ", (Math.Round(dsa, 2) * 100.0), CountSize(bytesSent), CountSize(totalBytes.Value));
                    }
                    else
                    {
                        Console.Write("{0} 下载            ", CountSize(bytesSent));
                    }
                })
                .success((WebResult r, string a) =>
                {
                    Console.SetCursorPosition(0, 5);
                    Console.Write(a);
                }).fail((Exception ez) =>
                {
                });
            Console.ReadLine();
        }
        public static string CountSize(double Size)
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
    }
}
