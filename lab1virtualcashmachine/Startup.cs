using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace lab1virtualcashmachine
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }


        private static float cash = 1000.0f;

        private static void Withdraw(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                cash -= 500;
                await next.Invoke();
                await context.Response.WriteAsync($"Cash after withdrawal: {cash}");
            });

            app.Run(async context =>
            {
                if (cash < 0)
                {
                    cash += 500;
                    await context.Response.WriteAsync("Not enough money!\n");
                }
                else
                {
                    await context.Response.WriteAsync("Withdraw 500$\n");
                }
            });
        }

        private static void Deposit(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await next.Invoke();
                await context.Response.WriteAsync($"Cash after deposit: {cash}");
            });

            app.Run(async context =>
            {
                cash += 200;
                await context.Response.WriteAsync("Deposited 200$\n");
            });
        }

        private static void Balance(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync($"Current balance: {cash}");
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Map added MapMiddleware to the pipeline
            app.Map("/cash", cashApp => {
                cashApp.Map("/balance", Balance);
                cashApp.Map("/withdraw", Withdraw);
                cashApp.Map("/deposit", Deposit);
            });
            
            // app.Use adds delegate defined in-line to the application's request pipeline
            app.Use(async (context, next) =>
            {
                await context.Response.WriteAsync("\n\tSite Map\n----------------------------------------\n/cash/balance - view current balance\n/cash/withdraw - withdraw 500$\n/cash/deposit - deposit 200$\n----------------------------------------\n");
            });

            // app.Run adds a terminal middleware to the application's request pipeline
            app.Run(async context =>
            {
                
            });
        }

        public delegate Task RequestDelegate(HttpContext context);
    }
}
