IF  EXISTS (SELECT * FROM sys.fn_listextendedproperty(N'MS_Description' , N'SCHEMA',N'dbo', N'TABLE',N'Products', N'COLUMN',N'InVentario'))
EXEC sys.sp_dropextendedproperty @name=N'MS_Description' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Products', @level2type=N'COLUMN',@level2name=N'InVentario'
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [FK_Users_Users_LastUpdateBy]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [FK_Users_Users_CreatedBy]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [FK_Users_Groups]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [FK_Users_Company]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sales]') AND type in (N'U'))
ALTER TABLE [dbo].[Sales] DROP CONSTRAINT IF EXISTS [FK_Sales_Users]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sales]') AND type in (N'U'))
ALTER TABLE [dbo].[Sales] DROP CONSTRAINT IF EXISTS [FK_Sales_Customers]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sales]') AND type in (N'U'))
ALTER TABLE [dbo].[Sales] DROP CONSTRAINT IF EXISTS [FK_Sale_Company]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleDetails]') AND type in (N'U'))
ALTER TABLE [dbo].[SaleDetails] DROP CONSTRAINT IF EXISTS [FK_SaleDetails_Products]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleDetails]') AND type in (N'U'))
ALTER TABLE [dbo].[SaleDetails] DROP CONSTRAINT IF EXISTS [FK_Sale_SaleDetails]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rentas]') AND type in (N'U'))
ALTER TABLE [dbo].[Rentas] DROP CONSTRAINT IF EXISTS [FK_Rentas_Users]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Rentas]') AND type in (N'U'))
ALTER TABLE [dbo].[Rentas] DROP CONSTRAINT IF EXISTS [FK_Rentas_Company]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
ALTER TABLE [dbo].[Products] DROP CONSTRAINT IF EXISTS [FK_Products_ProductCategory]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
ALTER TABLE [dbo].[Products] DROP CONSTRAINT IF EXISTS [FK_Products_Company]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ProductCategory]') AND type in (N'U'))
ALTER TABLE [dbo].[ProductCategory] DROP CONSTRAINT IF EXISTS [FK_ProductCategory_Company]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Groups]') AND type in (N'U'))
ALTER TABLE [dbo].[Groups] DROP CONSTRAINT IF EXISTS [FK_Groups_Company]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Customers]') AND type in (N'U'))
ALTER TABLE [dbo].[Customers] DROP CONSTRAINT IF EXISTS [FK_Customer_Company]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Company]') AND type in (N'U'))
ALTER TABLE [dbo].[Company] DROP CONSTRAINT IF EXISTS [FK_Company_Users_LastUpdateBy]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Company]') AND type in (N'U'))
ALTER TABLE [dbo].[Company] DROP CONSTRAINT IF EXISTS [FK_Company_Users_CreatedBy]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Company]') AND type in (N'U'))
ALTER TABLE [dbo].[Company] DROP CONSTRAINT IF EXISTS [FK_Company_PlanRentas]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [DF__Users__StatusId__5AEE82B9]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Users]') AND type in (N'U'))
ALTER TABLE [dbo].[Users] DROP CONSTRAINT IF EXISTS [DF__Users__IsEnabled__4316F928]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sales]') AND type in (N'U'))
ALTER TABLE [dbo].[Sales] DROP CONSTRAINT IF EXISTS [DF__Sales__CompanyId__367C1819]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sales]') AND type in (N'U'))
ALTER TABLE [dbo].[Sales] DROP CONSTRAINT IF EXISTS [DF__Sales__Status__3587F3E0]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Sales]') AND type in (N'U'))
ALTER TABLE [dbo].[Sales] DROP CONSTRAINT IF EXISTS [DF__Sales__SaleDate__3493CFA7]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleDetails]') AND type in (N'U'))
ALTER TABLE [dbo].[SaleDetails] DROP CONSTRAINT IF EXISTS [DF__SaleDetai__BuyPr__339FAB6E]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
ALTER TABLE [dbo].[Products] DROP CONSTRAINT IF EXISTS [DF__Products__DateMo__32AB8735]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Products]') AND type in (N'U'))
ALTER TABLE [dbo].[Products] DROP CONSTRAINT IF EXISTS [DF__Products__DateAd__31B762FC]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PlanRentas]') AND type in (N'U'))
ALTER TABLE [dbo].[PlanRentas] DROP CONSTRAINT IF EXISTS [DF_PlanRentas_StatusId]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PlanRentas]') AND type in (N'U'))
ALTER TABLE [dbo].[PlanRentas] DROP CONSTRAINT IF EXISTS [DF_PlanRentas_Price]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Company]') AND type in (N'U'))
ALTER TABLE [dbo].[Company] DROP CONSTRAINT IF EXISTS [DF__Company__StatusI__2EDAF651]
GO
/****** Object:  Index [IX_Users_UserName]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Users_UserName] ON [dbo].[Users]
GO
/****** Object:  Index [IX_Users_LastUpdateBy]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Users_LastUpdateBy] ON [dbo].[Users]
GO
/****** Object:  Index [IX_Users_GroupId]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Users_GroupId] ON [dbo].[Users]
GO
/****** Object:  Index [IX_Users_CreatedBy]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Users_CreatedBy] ON [dbo].[Users]
GO
/****** Object:  Index [IX_Users_CompanyId]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Users_CompanyId] ON [dbo].[Users]
GO
/****** Object:  Index [IX_Sales_UserId]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Sales_UserId] ON [dbo].[Sales]
GO
/****** Object:  Index [IX_Sales_CustomerId]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Sales_CustomerId] ON [dbo].[Sales]
GO
/****** Object:  Index [IX_Sales_CompanyId]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Sales_CompanyId] ON [dbo].[Sales]
GO
/****** Object:  Index [IX_SaleDetails_ProductId]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_SaleDetails_ProductId] ON [dbo].[SaleDetails]
GO
/****** Object:  Index [IX_Products_CompanyId]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Products_CompanyId] ON [dbo].[Products]
GO
/****** Object:  Index [IX_Products_Barcode_SKU]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Products_Barcode_SKU] ON [dbo].[Products]
GO
/****** Object:  Index [IX_Products]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Products] ON [dbo].[Products]
GO
/****** Object:  Index [IX_Groups_CompanyId]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Groups_CompanyId] ON [dbo].[Groups]
GO
/****** Object:  Index [IX_Customer_Phone]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Customer_Phone] ON [dbo].[Customers]
GO
/****** Object:  Index [IX_Customer_Name]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Customer_Name] ON [dbo].[Customers]
GO
/****** Object:  Index [IX_Customer_Email]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Customer_Email] ON [dbo].[Customers]
GO
/****** Object:  Index [IX_Customer_CompanyId]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Customer_CompanyId] ON [dbo].[Customers]
GO
/****** Object:  Index [IX_Company_LastUpdateBy]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Company_LastUpdateBy] ON [dbo].[Company]
GO
/****** Object:  Index [IX_Company_CreatedBy]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP INDEX IF EXISTS [IX_Company_CreatedBy] ON [dbo].[Company]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP TABLE IF EXISTS [dbo].[Users]
GO
/****** Object:  Table [dbo].[Sales]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP TABLE IF EXISTS [dbo].[Sales]
GO
/****** Object:  Table [dbo].[SaleDetails]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP TABLE IF EXISTS [dbo].[SaleDetails]
GO
/****** Object:  Table [dbo].[Rentas]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP TABLE IF EXISTS [dbo].[Rentas]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP TABLE IF EXISTS [dbo].[Products]
GO
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP TABLE IF EXISTS [dbo].[ProductCategory]
GO
/****** Object:  Table [dbo].[PlanRentas]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP TABLE IF EXISTS [dbo].[PlanRentas]
GO
/****** Object:  Table [dbo].[Groups]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP TABLE IF EXISTS [dbo].[Groups]
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP TABLE IF EXISTS [dbo].[Customers]
GO
/****** Object:  Table [dbo].[Company]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP TABLE IF EXISTS [dbo].[Company]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2/21/2025 7:20:12 AM ******/
DROP TABLE IF EXISTS [dbo].[__EFMigrationsHistory]
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 2/21/2025 7:20:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Company]    Script Date: 2/21/2025 7:20:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Company](
	[CompanyId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Address] [varchar](250) NULL,
	[CreatedBy] [int] NULL,
	[CreationDate] [datetime] NULL,
	[LastUpdateBy] [int] NULL,
	[PaymentsJson] [xml] NULL,
	[StatusId] [int] NOT NULL,
	[ExpirationDate] [datetime] NULL,
	[PhoneNumber] [varchar](10) NULL,
	[CelphoneNumber] [varchar](10) NULL,
	[Email] [varchar](30) NULL,
	[OwnerInfo] [nvarchar](500) NULL,
	[PlanId] [int] NULL,
 CONSTRAINT [PK_Company] PRIMARY KEY CLUSTERED 
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Customers]    Script Date: 2/21/2025 7:20:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Customers](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[Address] [varchar](300) NULL,
	[Notes] [varchar](500) NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName1] [varchar](50) NOT NULL,
	[LastName2] [varchar](50) NULL,
	[WorkPhoneNumber] [varchar](15) NULL,
	[CellPhoneNumber] [varchar](15) NULL,
	[Email] [varchar](35) NULL,
	[FullName]  AS (concat([FirstName],' ',[LastName1],coalesce(' '+[LastName2],''))),
	[CompanyId] [int] NOT NULL,
 CONSTRAINT [PK_Customer_Id] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Groups]    Script Date: 2/21/2025 7:20:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Groups](
	[GroupId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](250) NULL,
	[Permissions] [xml] NOT NULL,
	[CompanyId] [int] NOT NULL,
 CONSTRAINT [PK_Groups] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanRentas]    Script Date: 2/21/2025 7:20:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanRentas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Notes] [nvarchar](500) NULL,
	[Price] [decimal](10, 0) NOT NULL,
	[StatusId] [int] NOT NULL,
	[NumUsers] [int] NOT NULL,
 CONSTRAINT [PK_PlanRentas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ProductCategory]    Script Date: 2/21/2025 7:20:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductCategory](
	[CategoryId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Description] [varchar](500) NULL,
	[CompanyId] [int] NOT NULL,
 CONSTRAINT [PK_ProductCategory] PRIMARY KEY CLUSTERED 
(
	[CategoryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Products]    Script Date: 2/21/2025 7:20:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Products](
	[ProductId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](250) NOT NULL,
	[Description] [varchar](500) NULL,
	[CategoryId] [int] NULL,
	[SupplierId] [int] NULL,
	[UnitPrice] [decimal](10, 2) NULL,
	[UnitsInStock] [int] NOT NULL,
	[Discontinued] [bit] NOT NULL,
	[ImgUrl] [varchar](250) NULL,
	[DateAdded] [datetime] NOT NULL,
	[DateModified] [datetime] NOT NULL,
	[Weight] [decimal](10, 2) NULL,
	[SKU] [varchar](50) NULL,
	[Barcode] [varchar](50) NULL,
	[Brand] [varchar](50) NULL,
	[Color] [varchar](50) NULL,
	[Size] [varchar](50) NULL,
	[CompanyId] [int] NOT NULL,
	[BuyPrice] [decimal](10, 2) NULL,
	[UnitPrice1] [decimal](10, 2) NULL,
	[UnitPrice2] [decimal](10, 2) NULL,
	[UnitPrice3] [decimal](10, 2) NULL,
	[InVentario] [int] NULL,
	[AlertaStockNumProducts] [int] NULL,
	[IsInVentarioUpdated] [bit] NULL,
 CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED 
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Rentas]    Script Date: 2/21/2025 7:20:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Rentas](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[ReferenceDate] [datetime] NOT NULL,
	[Amount] [decimal](10, 2) NOT NULL,
	[AddedByUserId] [int] NOT NULL,
	[StatusId] [int] NOT NULL,
	[TipoRentaDesc] [varchar](15) NULL,
	[ExpirationDate] [datetime] NULL,
	[Notas] [varchar](500) NULL,
 CONSTRAINT [PK_Rentas] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SaleDetails]    Script Date: 2/21/2025 7:20:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaleDetails](
	[SaleId] [int] NOT NULL,
	[ProductId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[UnitPrice] [decimal](10, 2) NOT NULL,
	[TotalPrice] [decimal](10, 2) NOT NULL,
	[BuyPrice] [decimal](10, 2) NOT NULL,
	[PriceLevel] [int] NULL,
 CONSTRAINT [PK_SaleDetails] PRIMARY KEY CLUSTERED 
(
	[SaleId] ASC,
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Sales]    Script Date: 2/21/2025 7:20:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sales](
	[SaleId] [int] IDENTITY(1,1) NOT NULL,
	[CustomerId] [int] NULL,
	[SaleDate] [datetime] NOT NULL,
	[PaymentMethod] [varchar](50) NOT NULL,
	[Tax] [decimal](10, 2) NOT NULL,
	[Status] [varchar](50) NOT NULL,
	[TotalSale] [decimal](10, 2) NOT NULL,
	[Notes] [varchar](500) NULL,
	[UserId] [int] NOT NULL,
	[CompanyId] [int] NOT NULL,
 CONSTRAINT [PK_Sales] PRIMARY KEY CLUSTERED 
(
	[SaleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Users]    Script Date: 2/21/2025 7:20:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [varchar](50) NOT NULL,
	[Token] [varchar](100) NOT NULL,
	[FirstName] [varchar](50) NOT NULL,
	[LastName] [varchar](50) NOT NULL,
	[CompanyId] [int] NOT NULL,
	[GroupId] [int] NOT NULL,
	[LastAccess] [datetime] NULL,
	[IsEnabled] [bit] NOT NULL,
	[StatusId] [int] NOT NULL,
	[CreatedBy] [int] NULL,
	[LastUpdateBy] [int] NULL,
	[CreationDate] [datetime] NULL,
	[RoleId] [int] NULL,
	[SessionToken] [uniqueidentifier] NULL,
	[DeviceId] [nvarchar](255) NULL,
 CONSTRAINT [PK_User_Id] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Index [IX_Company_CreatedBy]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Company_CreatedBy] ON [dbo].[Company]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Company_LastUpdateBy]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Company_LastUpdateBy] ON [dbo].[Company]
(
	[LastUpdateBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Customer_CompanyId]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Customer_CompanyId] ON [dbo].[Customers]
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Customer_Email]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Customer_Email] ON [dbo].[Customers]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Customer_Name]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Customer_Name] ON [dbo].[Customers]
(
	[FirstName] ASC,
	[LastName1] ASC,
	[LastName2] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Customer_Phone]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Customer_Phone] ON [dbo].[Customers]
(
	[CellPhoneNumber] ASC,
	[WorkPhoneNumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Groups_CompanyId]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Groups_CompanyId] ON [dbo].[Groups]
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Products]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Products] ON [dbo].[Products]
(
	[Name] ASC,
	[Description] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Products_Barcode_SKU]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Products_Barcode_SKU] ON [dbo].[Products]
(
	[CompanyId] ASC,
	[Barcode] ASC,
	[SKU] ASC
)
WHERE ([Barcode] IS NOT NULL AND [SKU] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Products_CompanyId]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Products_CompanyId] ON [dbo].[Products]
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_SaleDetails_ProductId]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_SaleDetails_ProductId] ON [dbo].[SaleDetails]
(
	[ProductId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Sales_CompanyId]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Sales_CompanyId] ON [dbo].[Sales]
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Sales_CustomerId]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Sales_CustomerId] ON [dbo].[Sales]
(
	[CustomerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Sales_UserId]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Sales_UserId] ON [dbo].[Sales]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_CompanyId]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Users_CompanyId] ON [dbo].[Users]
(
	[CompanyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_CreatedBy]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Users_CreatedBy] ON [dbo].[Users]
(
	[CreatedBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_GroupId]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Users_GroupId] ON [dbo].[Users]
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
/****** Object:  Index [IX_Users_LastUpdateBy]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE NONCLUSTERED INDEX [IX_Users_LastUpdateBy] ON [dbo].[Users]
(
	[LastUpdateBy] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Object:  Index [IX_Users_UserName]    Script Date: 2/21/2025 7:20:12 AM ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Users_UserName] ON [dbo].[Users]
(
	[UserName] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Company] ADD  DEFAULT ((0)) FOR [StatusId]
GO
ALTER TABLE [dbo].[PlanRentas] ADD  CONSTRAINT [DF_PlanRentas_Price]  DEFAULT ((1)) FOR [Price]
GO
ALTER TABLE [dbo].[PlanRentas] ADD  CONSTRAINT [DF_PlanRentas_StatusId]  DEFAULT ((0)) FOR [StatusId]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (getdate()) FOR [DateAdded]
GO
ALTER TABLE [dbo].[Products] ADD  DEFAULT (getdate()) FOR [DateModified]
GO
ALTER TABLE [dbo].[SaleDetails] ADD  DEFAULT ((0.0)) FOR [BuyPrice]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT (getdate()) FOR [SaleDate]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT ('Completada') FOR [Status]
GO
ALTER TABLE [dbo].[Sales] ADD  DEFAULT ((0)) FOR [CompanyId]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF__Users__IsEnabled__4316F928]  DEFAULT (CONVERT([bit],(0))) FOR [IsEnabled]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF__Users__StatusId__5AEE82B9]  DEFAULT ((0)) FOR [StatusId]
GO
ALTER TABLE [dbo].[Company]  WITH CHECK ADD  CONSTRAINT [FK_Company_PlanRentas] FOREIGN KEY([PlanId])
REFERENCES [dbo].[PlanRentas] ([Id])
GO
ALTER TABLE [dbo].[Company] CHECK CONSTRAINT [FK_Company_PlanRentas]
GO
ALTER TABLE [dbo].[Company]  WITH CHECK ADD  CONSTRAINT [FK_Company_Users_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Company] CHECK CONSTRAINT [FK_Company_Users_CreatedBy]
GO
ALTER TABLE [dbo].[Company]  WITH CHECK ADD  CONSTRAINT [FK_Company_Users_LastUpdateBy] FOREIGN KEY([LastUpdateBy])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Company] CHECK CONSTRAINT [FK_Company_Users_LastUpdateBy]
GO
ALTER TABLE [dbo].[Customers]  WITH CHECK ADD  CONSTRAINT [FK_Customer_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[Customers] CHECK CONSTRAINT [FK_Customer_Company]
GO
ALTER TABLE [dbo].[Groups]  WITH CHECK ADD  CONSTRAINT [FK_Groups_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[Groups] CHECK CONSTRAINT [FK_Groups_Company]
GO
ALTER TABLE [dbo].[ProductCategory]  WITH CHECK ADD  CONSTRAINT [FK_ProductCategory_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[ProductCategory] CHECK CONSTRAINT [FK_ProductCategory_Company]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_Company]
GO
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_ProductCategory] FOREIGN KEY([CategoryId])
REFERENCES [dbo].[ProductCategory] ([CategoryId])
GO
ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK_Products_ProductCategory]
GO
ALTER TABLE [dbo].[Rentas]  WITH CHECK ADD  CONSTRAINT [FK_Rentas_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[Rentas] CHECK CONSTRAINT [FK_Rentas_Company]
GO
ALTER TABLE [dbo].[Rentas]  WITH CHECK ADD  CONSTRAINT [FK_Rentas_Users] FOREIGN KEY([AddedByUserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Rentas] CHECK CONSTRAINT [FK_Rentas_Users]
GO
ALTER TABLE [dbo].[SaleDetails]  WITH CHECK ADD  CONSTRAINT [FK_Sale_SaleDetails] FOREIGN KEY([SaleId])
REFERENCES [dbo].[Sales] ([SaleId])
GO
ALTER TABLE [dbo].[SaleDetails] CHECK CONSTRAINT [FK_Sale_SaleDetails]
GO
ALTER TABLE [dbo].[SaleDetails]  WITH CHECK ADD  CONSTRAINT [FK_SaleDetails_Products] FOREIGN KEY([ProductId])
REFERENCES [dbo].[Products] ([ProductId])
GO
ALTER TABLE [dbo].[SaleDetails] CHECK CONSTRAINT [FK_SaleDetails_Products]
GO
ALTER TABLE [dbo].[Sales]  WITH CHECK ADD  CONSTRAINT [FK_Sale_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[Sales] CHECK CONSTRAINT [FK_Sale_Company]
GO
ALTER TABLE [dbo].[Sales]  WITH CHECK ADD  CONSTRAINT [FK_Sales_Customers] FOREIGN KEY([CustomerId])
REFERENCES [dbo].[Customers] ([CustomerId])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Sales] CHECK CONSTRAINT [FK_Sales_Customers]
GO
ALTER TABLE [dbo].[Sales]  WITH CHECK ADD  CONSTRAINT [FK_Sales_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Sales] CHECK CONSTRAINT [FK_Sales_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Company] FOREIGN KEY([CompanyId])
REFERENCES [dbo].[Company] ([CompanyId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Company]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Groups] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Groups] ([GroupId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Groups]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Users_CreatedBy] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Users_CreatedBy]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Users_LastUpdateBy] FOREIGN KEY([LastUpdateBy])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_Users_LastUpdateBy]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Valor utilizado para guardar informacion temporal del inventario del producto' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Products', @level2type=N'COLUMN',@level2name=N'InVentario'
GO
