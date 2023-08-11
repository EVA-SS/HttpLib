# HttpLib 便捷的Http库

如果你喜欢 HttpLib 项目，请为本项点亮一颗星 ⭐！

<a href="https://www.nuget.org/packages/Tom.HttpLib.Client/" target="_blank"> 
    <img src="https://img.shields.io/nuget/vpre/tom.httpLib.client?style=flat-square&logo=nuget&label=HttpLib"> 
							  <img src="https://img.shields.io/nuget/dt/Tom.HttpLib.Client?style=flat-square">
							  </a>

## 🖥支持环境
- .NET 6.0及以上。
- .NET Core3.1及以上。
- .NET Standard2.0及以上。

## 🌴支持

#### multipart/form-data

既可以上传文件等二进制数据，也可以上传表单键值对

#### 上传与下载进度回调

上传与下载的进度监控

#### 支持缓存

类似图片加载场景，同一个id的图片通过磁盘存储减少网络开支

#### 工厂创建

>默认关闭
使用 `HttpClientFactory` 池，可以最大程度上节省系统重复请求开支
``` csharp
Config.UsePool = true;
```


****

## ✨示例

* [创建请求](#创建请求)
* [添加参数](#添加参数)
* [添加请求头](#添加请求头)
* [启用重定向](#启用重定向)
* [设置超时时长](#设置超时时长)
* [设置编码](#设置编码)
* [设置缓存](#设置缓存)
* [请求之前处理](#请求之前处理)
* [请求之后处理](#请求之后处理)
* [注入回调获取进度](#注入回调获取进度)
    * [上传](#上传)
    * [下载](#下载)
* [请求](#请求)
* [实例1](#实例1)
* [实例2](#实例2)
* [实例下载文件](#实例下载文件)
* [实例多线程下载文件](#实例多线程下载文件)
* [实例获取域名IP](#实例获取域名ip)
* [WebResult介绍](#webresult介绍)


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
## 添加参数
>GET请求参数会自动注入到地址
``` csharp
data(new { test1 = "测试1", test2 = "测试2" })
data(new { wd = new string[] { "GitHub - Haku-Men HttpLib", "POST数组参数" } })
query(new { test = "POST下继续传递URL参数" })
query(new Val("test", "POST下继续传递URL参数1"))
```
>支持Class模型 `POST Json 需要自己编程`
``` csharp
data(new MyModel{ id = "id参数", file=new Files(@"文件地址") })
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
string json = "{\"JSON\":\"json data\"}";
datastr(json,"application/json")
```
``` csharp
data(new Files("文件地址"))
file(@"文件地址")
```
## 添加请求头
``` csharp
header(new { Accept = "*/*", Token = "test" })
```
``` csharp
header(new Val("Accept","*/*"), new Val("User-Agent","Chrome"))
```
## 启用重定向
>默认禁止
``` csharp
redirect(true)
```
## 设置超时时长
>`毫秒`（默认100秒）
``` csharp
timeout(3000)
```
## 设置编码
>默认`utf-8`
``` csharp
encoding("utf-8")
```

## 设置缓存
>先配置`Config.CacheFolder`缓存文件夹
``` csharp
cache("缓存id")
```

>或者设定有效期 1分钟
``` csharp
cache("缓存id",1)
```

## 请求之前处理
``` csharp
before((HttpCore r) =>
{
	return true; //继续请求
})
```

## 请求之后处理
``` csharp
after((HttpCore r, HttpResponseMessage msg) =>
{
	return true; //继续下载数据
})
```

## 注入回调获取进度
>字节大小
### 上传
``` csharp
requestProgres(prog => {
	Console.Write("{0}% 上传", prog);
})
```
### 下载
``` csharp
responseProgres((bytesSent, totalBytes) => {
	if (totalBytes.HasValue)
	{
		double prog = (bytesSent * 1.0) / (totalBytes.Value * 1.0);
		Console.Write("{0}% 下载", Math.Round(prog * 100.0, 1).ToString("N1"));
	}
})
```

# 请求
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
Http.Get("https://dldir1.qq.com/qqfile/qq/PCQQ9.7.3/QQ9.7.3.28946.exe")
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

# 实例多线程下载文件
``` csharp
Http.Get("https://dldir1.qq.com/qqfile/qq/PCQQ9.7.3/QQ9.7.3.28946.exe")
       .redirect(true)
       .DownLoad(new HttpDownOption(@"C:\Users\admin\Desktop\文档下载")
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
|ContentLength|内容长度||
|Exception|错误异常||
|Data|响应内容||