# HttpLib 便捷的Http库

如果你喜欢 HttpLib 项目，请为本项点亮一颗星 ⭐！

****

|作者|Tom|
|---|---
|QQ|17379620

****
## 目录
* [示例](#示例)
    * [创建请求](#创建请求)
    * [添加参数](#添加参数)
    * [添加请求头](#添加请求头)
    * [启用重定向](#启用重定向)
    * [设置超时时长](#设置超时时长)
    * [设置编码](#设置编码)
    * [请求之前处理](#请求之前处理)
    * [注入回调获取进度](#注入回调获取进度)
        * [上传](#上传)
        * [下载](#下载)
    * [请求](#请求)
* [实例1](#实例1)
* [实例2](#实例2)
* [实例下载文件](#实例下载文件)
* [实例获取域名IP](#实例获取域名IP)
* [WebResult介绍](#WebResult介绍)


# 示例
## 创建请求
``` csharp
Http.Get("https://www.baidu.com")
```
``` csharp
Http.Post("https://www.baidu.com")
```
``` csharp
Http.Put("https://www.baidu.com")
```
``` csharp
Http.Delete("https://www.baidu.com")
```
### 添加参数
>GET请求参数会自动注入到地址
``` csharp
data(new { test1 = "测试1", test2 = "测试2" })
data(new { params_ = "关键字参数" })
data(new { wd = new string[] { "GitHub - Haku-Men HttpLib", "POST数组参数" } })
query(new { test = "POST下继续传递URL参数" })
query(new Val("test", "POST下继续传递URL参数1"))
```
``` csharp
data(new Val("test1", "测试1"), new Val("test2", "测试2"))
```
``` csharp
data(new List<Val> {
	new Val("test1","测试1"),
	new Val("test2","测试2")
})
```
``` csharp
data(new Files("文件地址"))
```
### 添加请求头
``` csharp
header(new { Accept = "*/*", UserAgent = "Chrome" })
```
``` csharp
header(new Val("accept","*/*"), new Val("user-agent","Chrome"))
```
### 启用重定向
>默认禁止
``` csharp
redirect(true)
```
### 设置超时时长
>`毫秒`（默认100秒）
``` csharp
timeout(3000)
```
### 设置编码
>默认`utf-8`
``` csharp
encoding('utf-8')
```

### 请求之前处理
``` csharp
before((HttpCore r) =>
{
	return true; //继续请求
})
```

### 注入回调获取进度
>字节大小
#### 上传
``` csharp
requestProgres(prog => {
	Console.Write("{0}% 上传", prog);
})
```
#### 下载
``` csharp
responseProgres((bytesSent, totalBytes) => {
	if (totalBytes.HasValue)
	{
		double prog = (bytesSent * 1.0) / (totalBytes.Value * 1.0);
		Console.Write("{0}% 下载", Math.Round(prog * 100.0, 1).ToString("N1"));
	}
})
```

## 请求
>方法全异步
``` csharp
requestNone();//仅请求
request();//返回字符串
requestData();//返回字节
download("保存目录", "保存文件名称（为空自动获取）");//下载文件
```

# 实例1
>异步
``` csharp
Config.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1 Edg/108.0.0.0";

Http.Get("https://www.baidu.com/s")
.data(new { wd = "GitHub - Haku-Men HttpLib" })
.redirect(true)
.requestProgres(prog => {
    Console.Write("{0}% 上传", prog);
})
.responseProgres((bytesSent, totalBytes) => {
    if (totalBytes.HasValue)
    {
        double prog = (bytesSent * 1.0) / (totalBytes.Value * 1.0);
        Console.Write("{0}% 下载", Math.Round(prog * 100.0, 1).ToString("N1"));
    }
})
.request().ContinueWith((data) => {
    Console.WriteLine(data.Result.Data);
});
```

# 实例2 
>同步
``` csharp
var html = Http.Get("https://www.baidu.com/s")
.data(new { wd = "GitHub - Haku-Men HttpLib" })
.redirect(true)
.requestProgres(prog => {
	Console.Write("{0}% 上传", prog);
})
.responseProgres((bytesSent, totalBytes) => {
	if (totalBytes.HasValue)
	{
		double prog = (bytesSent * 1.0) / (totalBytes.Value * 1.0);
		Console.Write("{0}% 下载", Math.Round(prog * 100.0, 1).ToString("N1"));
	}
})
.request().Result;
Console.WriteLine(html.Data);
```

# 实例下载文件
``` csharp
Http.Get("https://dldir1.qq.com/qqfile/qq/PCQQ9.6.2/QQ9.6.2.28756.exe")
       .redirect(true)
       .responseProgres((bytesSent, totalBytes) =>
       {
           Console.SetCursorPosition(0, 0);
           if (totalBytes.HasValue)
           {
               double prog = (bytesSent * 1.0) / (totalBytes.Value * 1.0);
               Console.Write("{0}% 下载 {1}/{2}                  ", Math.Round(prog * 100.0, 1).ToString("N1"), CountSize(bytesSent), CountSize(totalBytes.Value));
           }
           else
           {
               Console.Write("{0} 下载            ", CountSize(bytesSent));
           }
       }).download(@"C:\Users\admin\Desktop").ContinueWith(savapath =>
       {
           if (savapath.Result != null)
           {
               Console.WriteLine("下载成功保存至:" + savapath.Result.Data);
           }
           else
           {
               Console.WriteLine("下载失败");
           }
       }).Wait();
```

# 实例获取域名IP
``` csharp
Http.Get("https://www.baidu.com").IP
```

# WebResult介绍

|代码|解释|说明|
|:------------|:---------------:|:------------|
|OK|是否成功响应|`true` 正常响应|
|StatusCode|状态代码|`200` 为正常 常见的有`404`未找到、`302`重定向、`502`网址报错|
|Type|服务指示类型|`Content-Type`|
|Header|响应头||
|HeaderContent|内容响应头||
|Exception|错误异常||
|Data|响应内容||