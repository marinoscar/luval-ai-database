using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Luval.AI.Database.UnitTests
{
    public class Utils
    {
        public static SecureString GetApiKey()
        {
            return new NetworkCredential("", File.ReadAllText("private.txt")).SecurePassword;
        }

		public static string GetConnStr()
		{
			return @"Server=.\SQLEXPRESS;Database=AdventureWorksDW2019;Trusted_Connection=True;";
		}
        public static string GetDbSchema()
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

		public static DataPrompt CreateDataPrompt()
		{
			return new DataPrompt(GetApiKey(), GetConnStr(), GetDbSchema());
		}


    }
}
