/*
using CoreWCF.Configuration;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;


namespace #NAMESPACEPLACEHOLDER#
{
	public class Program
	{
		public static void Main(string[] args)
		{
      //All Ports set are default.
			IWebHost host = CreateWebHostBuilder(args).Build();
      host.Run();
		}

    public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
      WebHost.CreateDefaultBuilder(args)
				 .UseKestrel(options => { })
				 .UseStartup<Startup>();
	}
}
*/