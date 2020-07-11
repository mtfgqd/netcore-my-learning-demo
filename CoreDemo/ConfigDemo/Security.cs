using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDemo.ConfigDemo
{
    /// <summary>
    /// 配置数据指南：
    //请勿在配置提供程序代码或纯文本配置文件中存储密码或其他敏感数据。 机密管理器可用于存储开发环境中的机密。
    //不要在开发或测试环境中使用生产机密。
    //请在项目外部指定机密，避免将其意外提交到源代码存储库。
    //默认情况下，机密管理器会在 appsettings.json 和 appsettings.Environment.json 之后读取配置设置 。
    //有关存储密码或其他敏感数据的详细信息：
    //在 ASP.NET Core 中使用多个环境
    //ASP.NET Core 中的开发中安全存储应用机密：包含有关如何使用环境变量来存储敏感数据的建议。 机密管理器使用文件配置提供程序将用户机密存储在本地系统上的 JSON 文件中。
    //Azure Key Vault 安全存储 ASP.NET Core 应用的应用机密。
    /// </summary>
    public class Security
    {
        //ASP.NET Core 中的开发中安全存储应用机密
        //https://docs.microsoft.com/zh-cn/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows
    }


    /// <summary> 
    /// 使用默认配置，EnvironmentVariablesConfigurationProvider 
    /// 会在读取 appsettings.json、appsettings.Environment.json 和机密管理器后从环境变量键值对加载配置 。
    /// 因此，从环境中读取的键值会替代从 appsettings.json、appsettings.Environment.json 和机密管理器中读取的值 。
    /// </summary>
    /// <returns></returns>
    public class EnvSecurity
    {
        // 设置环境变量分层键使用 __（双下划线）


        //.NET Core CLI
        //set MyKey="My key from Environment"
        //set Position__Title = Environment_Editor
        //set Position__Name = Environment_Rick
        //dotnet run

        //windows cmd
        //setx MyKey "My key from setx Environment" /M
        //setx Position__Title Setx_Environment_Editor /M
        //setx Position__Name Environment_Rick /M



        //若 ConfigureAppConfiguration设置了环境变量的prefix那么上面的Key都要加前缀
        //.ConfigureAppConfiguration((hostingContext, config) =>
        //    {
        //        config.AddEnvironmentVariables(prefix: "MyCustomPrefix_");
        //    })


        //set MyCustomPrefix_MyKey = "My key with MyCustomPrefix_ Environment"
        //set MyCustomPrefix_Position__Title = Editor_with_customPrefix
        //set MyCustomPrefix_Position__Name = Environment_Rick_cp
        //dotnet run

        //默认配置会加载前缀为 DOTNET_ 和 ASPNETCORE_ 的环境变量和命令行参数。
        //DOTNET_ 和 ASPNETCORE_ 前缀会由 ASP.NET Core 用于主机和应用配置，但不用于用户配置。
    }
}
