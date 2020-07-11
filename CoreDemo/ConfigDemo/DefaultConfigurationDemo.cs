using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Linq;

namespace CoreDemo.ConfigDemo
{
    public class PositionOptions
    {
        public string Title { get; set; }
        public string Name { get; set; }
    }

    public class DefaultConfigurationDemo : PageModel
    {
        private IConfigurationRoot ConfigRoot;

        // requires using Microsoft.Extensions.Configuration;
        private readonly IConfiguration Configuration;

        public DefaultConfigurationDemo(IConfiguration config)
        {
            ConfigRoot = (IConfigurationRoot)config;//所有配置项的基础
            Configuration = config;
        }

        /// <summary>
        /// 按添加顺序显示了已启用的配置提供程序
        /// 默认的 JsonConfigurationProvider 会按以下顺序加载配置：
        /// appsettings.json
        /// appsettings.Environment.json ：例如，appsettings.Production.json 和 appsettings.Development.json 文件。 
        /// 文件的环境版本是根据 IHostingEnvironment.EnvironmentName 加载的。
        /// appsettings.Environment.json 值将替代 appsettings.json 中的密钥 。
        /// </summary>
        /// <returns></returns>
        public ContentResult OnGetConfigProviders()
        {
            string str = "";
            foreach (var provider in ConfigRoot.Providers.ToList())
            {
                str += provider.ToString() + "\n";
            }

            return Content(str);
        }

        /// <summary>
        /// 显示了appsettings.json一些配置设置 分层配置数据
        /// </summary>
        /// <returns></returns>
        public ContentResult OnGetConfigItemFormAppsettings()
        {
            var myKeyValue = Configuration["MyKey"];
            var title = Configuration["Position:Title"];
            var name = Configuration["Position:Name"];
            var defaultLogLevel = Configuration["Logging:LogLevel:Default"];
            return Content($"MyKey value: {myKeyValue} \n" +
                           $"Title: {title} \n" +
                           $"Name: {name} \n" +
                           $"Default Log Level: {defaultLogLevel}");
        }

        /// <summary>
        /// 使用选项模式绑定分层配置数据appsettings.json
        /// </summary>
        /// <returns></returns>
        public ContentResult OnGetConfigObjUseBind()
        {
            var positionOptions = new PositionOptions();// mapping to configration file

            //Position in appsettings.json
            //调用 ConfigurationBinder.Bind 将 PositionOptions 类绑定到 Position 部分
            //绑定类型的所有公共读写属性  不会绑定字段
            Configuration.GetSection("Position").Bind(positionOptions);
            
            //显示 Position 配置数据
            return Content($"Title: {positionOptions.Title} \n" +
                           $"Name: {positionOptions.Name}");
        }


        public PositionOptions positionOptions { get; private set; }
        /// <summary>
        /// ConfigurationBinder.Get<T> 绑定并返回指定的类型。
        /// 使用 ConfigurationBinder.Get<T> 可能比使用 ConfigurationBinder.Bind 更方便。
        /// </summary>
        /// <returns></returns>
        public ContentResult OnGetConfigObjUseGet()
        {
            positionOptions = Configuration.GetSection("Position").Get<PositionOptions>();

            return Content($"Title: {positionOptions.Title} \n" +
                           $"Name: {positionOptions.Name}");
        }

        /// <summary>
        /// ConfigurationBinder.GetValue<T> 从配置中提取一个具有指定键的值，并将它转换为指定的类型：
        /// </summary>
        /// <returns></returns>
        public ContentResult OnGetGetValue()
        {
            var number = Configuration.GetValue<int>("NumberKey", 99);
            return Content($"{number}");
        }

        /// <summary>
        /// IConfiguration.GetSection 会返回具有指定子节键的配置子节。
        /// </summary>
        /// <returns></returns>
        public ContentResult OnGetGetSection()
        {
            var Config = Configuration.GetSection("section1");
            return Content(
                    $"section1:key0: '{Config["key0"]}'\n" +
                    $"section1:key1: '{Config["key1"]}'");
        }

        public ContentResult OnGetGetChildrenAndExists()
        {
            string s = null;
            var selection = Configuration.GetSection("section2");
            if (!selection.Exists())
            {
                throw new System.Exception("section2 does not exist.");
            }
            var children = selection.GetChildren();

            foreach (var subSection in children)
            {
                int i = 0;
                var key1 = subSection.Key + ":key" + i++.ToString();
                var key2 = subSection.Key + ":key" + i.ToString();
                s += key1 + " value: " + selection[key1] + "\n";
                s += key2 + " value: " + selection[key2] + "\n";
            }
            return Content(s);
        }
        /// <summary>
        /// ConfigurationBinder.Bind 支持使用配置键中的数组索引将数组绑定到对象。 公开数值键段的任何数组格式都能够与 POCO 类数组进行数组绑定。
        /// </summary>
        /// <returns></returns>
        public ArrayExample _array { get; private set; }
        public ContentResult OnGet()
        {
            _array = Configuration.GetSection("array").Get<ArrayExample>();
            string s = null;

            for (int j = 0; j < _array.Entries.Length; j++)
            {
                s += $"Index: {j}  Value:  {_array.Entries[j]} \n";
            }

            return Content(s);
        }
    }

    /// <summary>
    /// add service in Startup
    /// ConfigureServices.services.Configure<Model.ConfigurationDemo.PositionOptions>(Configuration.GetSection("Position"));
    /// 使用默认配置，会通过 reloadOnChange: true 启用 appsettings.json 和 appsettings.Environment.json 文件 。
    /// 应用启动后，对 appsettings.json 和 appsettings.Environment.json 文件做出的更改将由 JSON 配置提供程序读取 。
    /// </summary>
    public class ConfigurationDemo2 : PageModel
    {
        private readonly PositionOptions _options;

        public ConfigurationDemo2(IOptions<PositionOptions> options)
        {
            _options = options.Value;
        }

        public ContentResult OnGetOptions()
        {
            return Content($"Title: {_options.Title} \n" +
                           $"Name: {_options.Name}");
        }
    }
}