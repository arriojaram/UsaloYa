GO

INSERT INTO [dbo].[Company]
           ([Name]
           ,[Address])
     VALUES(
           'JMC',
           'Online')
GO



INSERT INTO [dbo].[Groups]
           ([Name]
           ,[Description]
           ,[Permissions]
           ,[CompanyId])
     VALUES
           ('Admin'
           ,'Auto-generated'
           ,'<permissions>*</permissions>'
           ,1)

INSERT INTO [dbo].[Groups]
           ([Name]
           ,[Description]
           ,[Permissions]
           ,[CompanyId])
     VALUES
           ('Managers'
           ,'Auto-generated'
           ,'<permissions>*</permissions>'
           ,1)

INSERT INTO [dbo].[Groups]
           ([Name]
           ,[Description]
           ,[Permissions]
           ,[CompanyId])
     VALUES
           ('Premium'
           ,'Auto-generated'
           ,'<permissions>*</permissions>'
           ,1)

INSERT INTO [dbo].[Groups]
           ([Name]
           ,[Description]
           ,[Permissions]
           ,[CompanyId])
     VALUES
           ('Regular User'
           ,'Auto-generated'
           ,'<permissions>*</permissions>'
           ,1)
GO