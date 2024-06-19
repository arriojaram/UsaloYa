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
GO