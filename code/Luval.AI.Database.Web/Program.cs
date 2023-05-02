using Luval.AI.Database.MVM;
using Luval.AI.Database.Web.Data;
using Luval.OpenAI;
using Luval.OpenAI.Completion;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Net;
using System.Runtime.CompilerServices;

namespace Luval.AI.Database.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var apiKey =
            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddServerSideBlazor();
            builder.Services.AddSingleton<WeatherForecastService>();

            var instance = builder.Configuration.GetSection("AppSettings").GetValue<string>("OpenAIInstance");
            var openAIKey = builder.Configuration.GetSection("AppSettings").GetValue<string>("OpenAIApiKey");
            var openAiAuth = new ApiAuthentication(new NetworkCredential("", builder.Configuration.GetSection("AppSettings").GetValue<string>("OpenAIApiKey")).SecurePassword);
            var connStr = builder.Configuration.GetSection("AppSettings").GetValue<string>("ConnectionString");
            var api = new CompletionEndpoint(openAiAuth);
            if (!string.IsNullOrWhiteSpace(instance)) 
                api = CompletionEndpoint.CreateAzure(openAiAuth, instance);

            builder.Services.AddSingleton<ChatMVM>(new ChatMVM(new DataAnalyzer(api, connStr, GetDbSchema())));

            var app = builder.Build();
            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.MapBlazorHub();
            app.MapFallbackToPage("/_Host");

            app.Run();
        }

        private static string GetDbSchema()
        {
            return @"
CREATE TABLE [dbo].[CompanySales](
	[LineId] [bigint] NOT NULL PRIMARY KEY,
	[ProductKey] [int] NOT NULL,
	[ProductName] [nvarchar](50) NOT NULL,
	[CategoryName] [nvarchar](50) NOT NULL,
	[StandardCost] [money] NULL,
	[ListPrice] [money] NULL,
	[ReorderPoint] [smallint] NULL,
	[CustomerKey] [int] NOT NULL,
	[CustomerBirthDate] [date] NULL,
	[CustomerGender] [nvarchar](1) NULL,
	[CustomerCountry] [nvarchar](50) NULL,
	[CustomerCity] [nvarchar](30) NULL,
	[CustomerPostalCode] [nvarchar](15) NULL,
	[PromotionKey] [int] NOT NULL,
	[PromotionName] [nvarchar](255) NULL,
	[PromotionType] [nvarchar](50) NULL,
	[PromotionCategory] [nvarchar](50) NULL,
	[DiscountPercentage] [float] NULL,
	[CurrencyKey] [int] NOT NULL,
	[SalesTerritoryKey] [int] NOT NULL,
	[SalesTerritoryCountry] [nvarchar](50) NULL,
	[SalesTerritoryRegion] [nvarchar](50) NULL,
	[SalesTerritoryGroup] [nvarchar](50) NULL,
	[SalesOrderNumber] [nvarchar](20) NOT NULL,
	[OrderQuantity] [smallint] NOT NULL,
	[UnitPrice] [money] NOT NULL,
	[LineTotalUnitPrice] [money] NOT NULL,
	[UnitPriceDiscountPct] [float] NOT NULL,
	[DiscountAmount] [float] NOT NULL,
	[ProductStandardCost] [money] NOT NULL,
	[TotalProductCost] [money] NOT NULL,
	[SalesAmount] [money] NOT NULL,
	[TaxAmt] [money] NOT NULL,
	[Freight] [money] NOT NULL,
	[OrderDate] [datetime] NULL,
	[DueDate] [datetime] NULL,
	[ShipDate] [datetime] NULL,
)
GO
";
        }
    }
}