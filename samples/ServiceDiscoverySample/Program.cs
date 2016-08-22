using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace ServiceDiscoverySample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseUrls("http://+:5000/")
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
