using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDemo.ConfigDemo
{
    /// <summary>
    /// 从数据库读取配置键值对的基本配置提供程序。
    /// </summary>
    /// 
    /// 
    /// 定义用于在数据库中存储配置值的 EFConfigurationValue 实体
    public class EFConfigurationValue
    {
        public string Id { get; set; }
        public string Value { get; set; }
    }


    //添加 EFConfigurationContext 以存储和访问配置的值
    public class EFConfigurationContext : DbContext
    {
        public EFConfigurationContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<EFConfigurationValue> Values { get; set; }
    }

    //1 创建用于实现 IConfigurationSource 的类
    public class EFConfigurationSource : IConfigurationSource
    {
        private readonly Action<DbContextOptionsBuilder> _optionsAction;

        public EFConfigurationSource(Action<DbContextOptionsBuilder> optionsAction)
        {
            _optionsAction = optionsAction;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EFConfigurationProvider(_optionsAction);
        }
    }

    /// <summary>
    ///2 实现IConfigurationProvider
    /// </summary>
    public class EFConfigurationProvider : ConfigurationProvider
    {
        public EFConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
        {
            OptionsAction = optionsAction;
        }

        Action<DbContextOptionsBuilder> OptionsAction { get; }

        // Load config data from EF DB.
        public override void Load()
        {
            var builder = new DbContextOptionsBuilder<EFConfigurationContext>();

            OptionsAction(builder);

            using (var dbContext = new EFConfigurationContext(builder.Options))
            {
                dbContext.Database.EnsureCreated();

                Data = !dbContext.Values.Any()
                    ? CreateAndSaveDefaultValues(dbContext)
                    : dbContext.Values.ToDictionary(c => c.Id, c => c.Value);
            }
        }

        private static IDictionary<string, string> CreateAndSaveDefaultValues(EFConfigurationContext dbContext)
        {
            // Quotes (c)2005 Universal Pictures: Serenity
            // https://www.uphe.com/movies/serenity
            var configValues =
                new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                { "quote1", "I aim to misbehave." },
                { "quote2", "I swallowed a bug." },
                { "quote3", "You can't stop the signal, Mal." }
                };

            dbContext.Values.AddRange(configValues
                .Select(kvp => new EFConfigurationValue
                {
                    Id = kvp.Key,
                    Value = kvp.Value
                })
                .ToArray());

            dbContext.SaveChanges();

            return configValues;
        }
    }

    public static class EntityFrameworkExtensions
    {
        public static IConfigurationBuilder AddEFConfiguration(
            this IConfigurationBuilder builder,
            Action<DbContextOptionsBuilder> optionsAction)
        {
            return builder.Add(new EFConfigurationSource(optionsAction));
        }
    }
}
