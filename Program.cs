using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace ESPL.NG
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var cert = new X509Certificate2("SelfSignedCertificate1.pfx", "espl@123");

            var host = new WebHostBuilder()
                // .UseKestrel(cfg => cfg.UseHttps(cert))
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseUrls("http://localhost:6060")
                .Build();

            host.Run();
        }
    }
}
