GO

INSERT INTO [Company]
           ([Name]
           ,[Address],
		   StatusId,
		   ExpirationDate,
		   )
     VALUES(
           'JMC',
           'Online',
		   3,
		   '2030-12-31 00:00:00.000'
		   )
GO



INSERT INTO [Groups]
           ([Name]
           ,[Description]
           ,[Permissions]
           ,[CompanyId])
     VALUES
           ('Admin'
           ,'Auto-generated'
           ,'<permissions>*</permissions>'
           ,1)

INSERT INTO [Groups]
           ([Name]
           ,[Description]
           ,[Permissions]
           ,[CompanyId])
     VALUES
           ('Managers'
           ,'Auto-generated'
           ,'<permissions>*</permissions>'
           ,1)

INSERT INTO [Groups]
           ([Name]
           ,[Description]
           ,[Permissions]
           ,[CompanyId])
     VALUES
           ('Premium'
           ,'Auto-generated'
           ,'<permissions>*</permissions>'
           ,1)

INSERT INTO [Groups]
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

IF NOT EXISTS(SELECT [UserId] FROM [Users] WHERE [UserName] = 'johnwick') BEGIN
	
	INSERT [dbo].[Users] (
		[UserName], [Token], [FirstName], [LastName], [CompanyId], [GroupId], [LastAccess], [IsEnabled], [StatusId], [CreatedBy], [LastUpdateBy], [CreationDate], [RoleId]) 
	VALUES (N'johnwick', N'03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', N'John', N'Wick', 1, 1, NULL, 1, 0, NULL, NULL, GETDATE(), 14)

END

IF NOT EXISTS(SELECT * FROM [PlanRentas]) BEGIN
	INSERT INTO [PlanRentas]
			   ([Name]
			   ,[Notes]
			   ,[Price]
			   ,[StatusId])
		 VALUES
			   ('BETA-24-1'
			   ,'Plan piloto'
			   ,'250'
			   ,1)

END
