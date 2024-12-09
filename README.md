# HttpLib 便捷的Http库 | 多线程下载库

如果你喜欢 HttpLib 项目，请为本项点亮一颗星 ⭐！


[![NuGet](https://img.shields.io/nuget/vpre/tom.httpLib?style=flat-square&logo=nuget&label=HttpLib)](https://www.nuget.org/packages/Tom.HttpLib)
[![Download](https://img.shields.io/nuget/dt/Tom.HttpLib?style=flat-square)](https://www.nuget.org/packages/Tom.HttpLib)

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


****

## 目录
* [示例](#示例)
    * [创建请求](#创建请求)
    * [添加参数](#添加参数)
    * [添加请求头](#添加请求头)
    * [设置代理](#设置代理)
    * [启用重定向](#启用重定向)
    * [设置超时时长](#设置超时时长)
    * [设置编码](#设置编码)
    * [设置缓存](#设置缓存)
    * [请求之前处理](#请求之前处理)
    * [注入回调获取进度](#注入回调获取进度)
        * [上传](#上传)
        * [下载](#下载)
    * [请求](#请求)
        * [异步错误](#异步错误)
        * [异步请求](#异步请求)
        * [同步获取](#同步获取)
* [实例](#实例)
* [实例下载文件](#实例下载文件)
* [实例流式传输](#实例流式传输)
* [实例获取域名IP](#实例获取域名IP)
* [实例全局错误捕获](#实例全局错误捕获)
* [ResultResponse介绍](#ResultResponse介绍)


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

> GET请求参数会自动注入到地址

``` csharp
data("wd", "随便搜一下")
data(new { test1 = "测试1", test2 = "测试2" })
data(new { params_ = "关键字参数" })
data(new { wd = new string[] { "GitHub - Haku-Men HttpLib", "POST数组参数" } })
```

> URL参数（除了GET请求）

``` csharp
query("键", "值对")
query(new { test = "POST下继续传递URL参数" })
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

> 上传字符串 默认 `text/plain`

``` csharp
string json = "{\"JSON\":\"json data\"}";
datastr(json, "application/json")
```

> 上传文件

``` csharp
data(new Files("文件地址"))
```
``` csharp
file(@"文件地址")
```

### 添加请求头
``` csharp
header("Authorization", "abc")
```
``` csharp
header(new { accept = "*/*", userAgent = "Chrome" })
```
``` csharp
header(new Val("accept","*/*"), new Val("user-agent","Chrome"))
```

### 设置代理
``` csharp
proxy("127.0.0.1",1000)
```

### 启用重定向
>默认禁止
``` csharp
redirect()
```

### 设置超时时长
>`毫秒`（默认不超时）
``` csharp
timeout(3000)
```

### 设置编码
>默认`utf-8`
``` csharp
encoding('utf-8')
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

### 请求之前处理
``` csharp
before((HttpWebResponse response, ResultResponse result) =>
{
	return true; //继续请求
})
```

### 注入回调获取进度
>字节大小
#### 上传
``` csharp
requestProgres((bytesSent, totalBytes) => {
	double prog = (bytesSent * 1.0) / (totalBytes * 1.0);
	Console.Write("{0}% 上传", Math.Round(prog * 100.0, 1).ToString("N1"));
})
```
#### 下载
``` csharp
responseProgres((bytesSent, totalBytes) => {
	if (totalBytes > 0)
	{
		double prog = (bytesSent * 1.0) / (totalBytes * 1.0);
		Console.Write("{0}% 下载", Math.Round(prog * 100.0, 1).ToString("N1"));
	}
})
```

## 请求
### 异步错误
``` csharp
fail((ResultResponse result) => {
})
```
### 同步获取
``` csharp
requestNone();//不下载流
request();//返回字符串
requestData();//返回字节
download("保存目录", "保存文件名称（为空自动获取）");//下载文件
```

# 实例

``` csharp
string result = Http.Get("https://www.baidu.com/s")
.data(new { wd = "GitHub - Haku-Men HttpLib", params_ = "关键字参数" })
.redirect()
.request();
Console.Write(result);
```

# 实例下载文件
``` csharp
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
```

# 实例流式传输
``` csharp
Http.Get("https://test.chatgpt.com/local_doc_chat").request(msg =>
{
    if (msg == null) return;
    if (msg.StartsWith("data: ")) msg = msg.Substring(5).Trim();
    System.Diagnostics.Debug.WriteLine(msg);
});
```

# 实例获取域名IP
``` csharp
Http.Get("https://www.baidu.com").IP
```

# 实例全局错误捕获
``` csharp
Config.fail += (HttpCore core, ResultResponse result)=>
{
    if (result.Exception == null) return;
    Console.Write(err.GetType());
    Console.Write(err.Message);
};
```

# ResultResponse介绍

|代码|类型|解释|说明|
|:--|:--|:--:|:--|
|StatusCode|int|状态代码|`200` 为正常 常见的有`404`未找到、`302`重定向、`502`网址报错|
|IsSuccessStatusCode|bool|响应是否成功|range `200`-`299`|
|ServerHeader|string`?`|服务头|HTTP 200 OK BWS/1.1 Ver:1.1|
|Uri|Uri|最终的地址||
|Type|string`?`|服务指示类型|`Content-Type`|
|Header|Dictionary<string, string>|响应头||
|Cookie|Dictionary<string, string>|Cookie||
|OriginalSize|long|流原始大小|动态压缩|
|Size|long|流大小||
|Exception|Exception`?`|异常信息||