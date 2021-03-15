# HttpLib
便捷的Http库
===========================
如果你喜欢 HttpLib 项目，请为本项点亮一颗星 ⭐！
****
已上传NuGet：https://www.nuget.org/packages/Tom.HttpLib
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
    * [设置代理](#设置代理)
    * [启用重定向](#启用重定向)
    * [设置超时时长](#设置超时时长)
    * [设置编码](#设置编码)
    * [请求之前处理](#请求之前处理)
    * [注入回调获取进度](#注入回调获取进度)
        * [上传](#上传)
        * [下载](#下载)
    * [请求](#请求)
        * [异步错误](#异步错误)
        * [异步请求](#异步请求)
        * [同步获取](#同步获取)
* [实例1](#实例1)
* [实例2](#实例2)
* [WebResult介绍](#WebResult介绍)
* [版权和协议](#版权和协议)


# 示例
## 创建请求
```
Http.Get("https://www.baidu.com")
```
### 添加参数
>GET请求参数会自动注入到地址
```
data(new { test1 = "测试1", test2 = "测试2" })
data(new { params_ = "关键字参数" })
data(new { wd = new string[] { "GitHub - Haku-Men HttpLib", "POST数组参数" } })
query(new { test = "POST下继续传递URL参数" })
```
```
data(new List<Val> {
	new Val("test1","测试1"),
	new Val("test2","测试2")
})
```
```
data(new Files("文件地址"))
```
### 添加请求头
```
header(new { accept = "*/*", userAgent = "Chrome" })
```
```
header(new List<Val> {
	new Val("accept","*/*"),
	new Val("user-agent","Chrome")
})
```
### 设置代理
```
proxy("127.0.0.1",1000)
```
### 启用重定向
>默认禁止
```
redirect(true)
```
### 设置超时时长
>`毫秒`（默认不超时）
```
timeout(3000)
```
### 设置编码
>默认`utf-8`
```
encoding('utf-8')
```

### 请求之前处理
```
requestBefore((WebResult r) =>
{
	return true; //继续请求
})
```

### 注入回调获取进度
>字节大小
#### 上传
```
requestProgress((bytesSent, totalBytes) => {
	double 进度 = (bytesSent * 1.0) / (totalBytes * 1.0);
	Console.Write("{0}% 上传", (Math.Round(进度, 2) * 100.0));
})
```
#### 下载
```
responseProgress((bytesSent, totalBytes) => {
	if (totalBytes.HasValue)
	{
		double 进度 = (bytesSent * 1.0) / (totalBytes.Value * 1.0);
		Console.Write("{0}% 下载", (Math.Round(进度, 2) * 100.0));
	}
})
```

## 请求
### 异步错误
```
fail((Exception e) => {
})
```
### 异步请求
```
success((WebResult web,string result) => {
	//放在最后
});

requestAsync();//主动调用异步方法
```
### 同步获取
```
requestNone();//不下载流
request();//返回字符串
requestData();//返回字节
```

# 实例1
>异步
```
Config.UserAgent = "测试的UserAgent";

Http.Get("https://www.baidu.com/s")
.data(new { wd = "GitHub - Haku-Men HttpLib", params_ = "关键字参数" })
.redirect(true)
.requestProgress((bytesSent, totalBytes) => {
	double 进度 = (bytesSent * 1.0) / (totalBytes * 1.0);
	Console.Write("{0}% 上传", (Math.Round(进度, 2) * 100.0));
})
.responseProgress((bytesSent, totalBytes) => {
	if (totalBytes.HasValue)
	{
		double 进度 = (bytesSent * 1.0) / (totalBytes.Value * 1.0);
		Console.Write("{0}% 下载", (Math.Round(进度, 2) * 100.0));
	}
})
.fail((Exception e) => {
	Console.Write(e.GetType());
	Console.Write(e.Message);
})
.success((WebResult web,string result) => {
	Console.Write(result);
});
```

# 实例2 
>同步
```
string result = Http.Get("https://www.baidu.com/s")
.data(new { wd = "GitHub - Haku-Men HttpLib", params_ = "关键字参数" })
.redirect(true)
.fail((Exception e) => {
	Console.Write(e.GetType());
	Console.Write(e.Message);
})
.request();
Console.Write(result);
```


# WebResult介绍

|代码|解释|说明|
|:------------|:---------------:|:------------|
|StatusCode|状态代码|`200` 为正常 常见的有`404`未找到、`302`重定向、`502`网址报错|
|ServerHeader|服务头|HTTP 200 OK BWS/1.1 Ver:1.1|
|IP|请求域的IP地址||
|AbsoluteUri|最终的地址||
|Type|服务指示类型|`Content-Type`|
|Header|响应头||
|Cookie|Cookie||
|SetCookie|Set-Cookie||
|OriginalSize|流原始大小|动态压缩|
|Size|流大小||
|FileName|文件名称|返回文件类型才有`Content-Disposition`|
|Location|重定向网址|302|


# 版权和协议
HttpLib 项目基于 LGPL-3.0 开源协议开放项目源代码。本项目版权由项目发起人、开发者Tom所有。

依照 LGPL-3.0 协议规定：

您可以在任何商业软件中引用 HttpLib 的二进制库而无需支付任何与版权相关的费用;
如果您的项目使用并修改了 HttpLib 的源代码，那么您的项目也需要使用 LGPL 协议进行开源，并且在您的衍生项目中保留 HttpLib 的版权信息：Powered by HttpLib。
如果您需要在非开源的应用程序中使用 HttpLib 的源代码，为了保障您的合法权益，请考虑向项目作者购买商业授权。
