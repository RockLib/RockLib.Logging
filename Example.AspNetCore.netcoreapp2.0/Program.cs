using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RockLib.Configuration;
using RockLib.Logging;
using RockLib.Logging.AspNetCore;

namespace Example.AspNetCore.netcoreapp2._0
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var webHost = BuildWebHost(args);
            
            try
            {
                webHost.Run();
            }
            finally
            {
                LoggerFactory.ShutDown();
            }
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseRockLibLogging()
                .Build();
    }
}
