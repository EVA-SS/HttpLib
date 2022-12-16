// See https://aka.ms/new-console-template for more information

using HttpLib;

List<Val> headerss = new List<Val> {
    new Val("Accept","text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9"),
    new Val("User-Agent","Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.183 Safari/537.36 Edg/86.0.622.63"),
};

var ip = Http.Get("https://www.baidu.com/s").IP;
Console.WriteLine("IP:baidu.com " + ip);
var str = Http.Get("https://www.baidu.com/s").header(headerss)
    .data("wd", "GitHub - Haku-Men HttpLib")
    .data(new Val("ie", "utf-8"))
       .header(new { Accept = "*/*" })
       .redirect(true)
       .before((HttpCore r) =>
       {
           return true; //继续请求
       }).request().Result;

Console.WriteLine(str.Data);
Console.WriteLine();
Console.ReadLine();