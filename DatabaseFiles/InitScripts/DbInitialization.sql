GO

INSERT INTO [Company]
           ([Name]
           ,[Address],
		   StatusId,
		   ExpirationDate
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
           ('General'
           ,'Auto-generated'
           ,'<permissions>*</permissions>'
           ,1)


GO

IF NOT EXISTS(SELECT [UserId] FROM [Users] WHERE [UserName] = 'johnwick') BEGIN
	
	INSERT [Users] (
		[UserName], [Token], [FirstName], [LastName], [CompanyId], [GroupId], [LastAccess], [IsEnabled], [StatusId], [CreatedBy], [LastUpdateBy], [CreationDate], [RoleId]) 
	VALUES (N'johnwick', N'03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', N'John', N'Wick', 1, 1, NULL, 1, 0, NULL, NULL, GETDATE(), 14)

END

IF NOT EXISTS(SELECT * FROM [PlanRentas]) BEGIN
	INSERT INTO [PlanRentas]
			   ([Name]
			   ,[Notes]
			   ,[Price]
			   ,[StatusId]
			   ,[NumUsers])
		 VALUES
			   ('BETA-24-1'
			   ,'Plan piloto'
			   ,'0'
			   ,1
			   ,1)

	INSERT INTO [PlanRentas]
			   ([Name]
			   ,[Notes]
			   ,[Price]
			   ,[StatusId]
			   ,[NumUsers])
		 VALUES
			   ('PR-25-1'
			   ,'Plan Premium'
			   ,'250'
			   ,1
			   ,2)
	UPDATE [PlanRentas] SET Price = 0, NumUsers = 2 WHERE Id = 1

END


/****** Object:  Table [dbo].[Questions]   ******/
CREATE TABLE [dbo].[Questions](
	[QuestionId] [int] IDENTITY(1,1) NOT NULL,
	[QuestionName] [nvarchar](300) NOT NULL,
	[Reply] [bit] NOT NULL,
	[IdUser] [int] NULL,
 CONSTRAINT [PK_Questions] PRIMARY KEY CLUSTERED 
(
	[QuestionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO


ALTER TABLE [dbo].[Users]
ADD [CodeVerification] NVARCHAR(10) NULL;

ALTER TABLE [dbo].[Users]
ADD [IsVerifiedCode] BIT NULL;

ALTER TABLE [dbo].[Users]
ADD [Email] NVARCHAR(100) NULL;


ALTER TABLE [dbo].[Users] ADD  DEFAULT (CONVERT([bit],(0))) FOR [IsVerifiedCode]
GO


ALTER TABLE [dbo].[Questions]  WITH CHECK ADD  CONSTRAINT [FK_Questions_Users_IdUser] FOREIGN KEY([IdUser])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Questions] CHECK CONSTRAINT [FK_Questions_Users_IdUser]
GO

UPDATE [Users]
   SET [IsVerifiedCode] = 1;



