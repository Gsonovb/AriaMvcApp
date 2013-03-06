Aria2 Web App
====
Web前端 和 一个文件管理的网页程序。


使用方法
-----
1. 下载必要组件。
> aria2  http://sourceforge.net/projects/aria2/
>
> IIS Express http://www.microsoft.com/zh-cn/download/details.aspx?id=34679
>

2. 打开 Aria2.config 文件，修改以下内容
>修改会话保存位置
>input-file=C:\aria2\aria2.session
>save-session=C:\aria2\aria2.session
>
>修改下载保存位置
>dir=c:\Downloads

3. 打开applicationhost.config 文件，修改以下内容
>       <site name="WebSite1" id="1" serverAutoStart="true">
>               <application path="/">
>                    <virtualDirectory path="/" physicalPath="c:\aria2\webui" />
>                    Web程序所在位置
>                    <virtualDirectory path="/downloads" physicalPath="c:\aria2\Downloads" />
>                    默认下载位置
>                </application>
>                <bindings>
>                     将 Localhost 改成机器名
>                    <binding protocol="http" bindingInformation=":36080:localhost" />
>                </bindings>


4.修改 runiis.bat 和 runaria2.bat 文件以匹配实际路径。


5.分别启动 runiis.bat 和 runaria2.bat 。 然后在浏览器里输入 http://机器名:36080
