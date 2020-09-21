using System;
using Microsoft.Extensions.FileProviders;
namespace FileProviderDemo
{
    class Program
    {
        static void Main(string[] args)
        {
            //1 获取物理文件
            IFileProvider provider1 = new PhysicalFileProvider(AppDomain.CurrentDomain.BaseDirectory);

            var contentsp = provider1.GetDirectoryContents("/");


            foreach (var item in contentsp)
            {
                //var stream = item.CreateReadStream();
                Console.WriteLine(item.Name);
            }


            //2 获取编译时构建到应用程序集中的文件
            IFileProvider provider2 = new EmbeddedFileProvider(typeof(Program).Assembly);
            //var dd = provider2.GetDirectoryContents("/");
            //2 将emb.html设置为嵌入的资源，而不是应用文件
            //var html = provider2.GetFileInfo("emb.html");

            //3 获取组合的各种文件
            IFileProvider provider = new CompositeFileProvider(provider1, provider2);



            var contents = provider.GetDirectoryContents("/");


            foreach (var item in contents)
            {
                Console.WriteLine(item.Name);
            }

            //Console.ReadKey();
        }
    }
}
