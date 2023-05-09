using System.Net;

namespace BakWeb
{
    public class Program
    {
        public static void Main(string[] args)
            => CreateHostBuilder(args)
                .Build()
                .Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureUmbracoDefaults()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStaticWebAssets();
                    webBuilder.UseStartup<Startup>()
                                .UseKestrel(options =>
                                {
                                    options.Listen(IPAddress.Parse("192.168.0.234"), 5000);
                                });
                });
    }
}
