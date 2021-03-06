CREATE TABLE [dbo].[Admin] (
	[AdminId] [int] IDENTITY (1, 1) NOT NULL ,
	[Username] [varchar] (50) NOT NULL ,
	[Password] [varchar] (100) NULL ,
	[PasswordEx] [varchar] (100) NULL ,
	[Email] [varchar] (100) NULL ,
	[FirstName] [varchar] (50) NULL ,
	[LastName] [varchar] (50) NULL ,
	[IsInternal] [bit] NULL ,
	[IsLocked] [bit] NULL ,
	[PasswordDate] [datetime] NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AdminAccess] (
	[Id] [int] IDENTITY (1, 1) NOT NULL ,
	[SectionId] [int] NOT NULL ,
	[GroupId] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AdminAdminGroup] (
	[Id] [int] IDENTITY (1, 1) NOT NULL ,
	[AdminId] [int] NOT NULL ,
	[GroupId] [int] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AdminGroup] (
	[GroupId] [int] IDENTITY (1, 1) NOT NULL ,
	[Description] [varchar] (50) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AdminLog] (
	[LogId] [int] IDENTITY (1, 1) NOT NULL ,
	[AdminId] [int] NULL ,
	[Username] [varchar] (50) NOT NULL ,
	[RemoteIP] [varchar] (50) NOT NULL ,
	[LoginDate] [datetime] NOT NULL ,
	[Succeeded] [bit] NOT NULL ,
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AdminSection] (
	[SectionId] [int] IDENTITY (1, 1) NOT NULL ,
	[Code] [varchar] (50) NOT NULL ,
	[Description] [varchar] (50) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[AdminPassword] (
	[PasswordId] [int] IDENTITY (1, 1) NOT NULL ,
	[AdminId] [int] NOT NULL ,
	[Password] [varchar] (100) NOT NULL ,
	[PasswordDate] [datetime] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContentToolContent] (
	[ContentId] [int] IDENTITY (1, 1) NOT NULL ,
	[Content] [varchar](max) NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContentToolModule] (
	[ModuleId] [int] IDENTITY (1, 1) NOT NULL ,
	[Name] [varchar] (100) NOT NULL ,
	[Args] [varchar] (255) NULL ,
	[ControlURL] [varchar] (255) NULL ,
	[MinWidth] [int] NULL ,
	[MaxWidth] [int] NULL ,
	[SkipIndexing] [bit] NOT NULL CONSTRAINT [DF_ContentToolModule_SkipIndexing]  DEFAULT ((0)),
	[HTML] [varchar](max) NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContentToolNavigation] (
	[NavigationId] [int] IDENTITY (1, 1) NOT NULL ,
	[Title] [varchar] (50) NOT NULL ,
	[ParentId] [int] NULL ,
	[IsInternalLink] [bit] NOT NULL ,
	[PageId] [int] NULL ,
	[URL] [varchar] (255) NULL ,
	[Target] [varchar] (50) NULL ,
	[Parameters] [varchar] (50) NULL ,
	[SortOrder] [int] NOT NULL ,
	[SkipSiteMap] [bit] NOT NULL ,
	[SkipBreadCrumb] [bit] NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContentToolPage] (
	[PageId] [int] IDENTITY (1, 1) NOT NULL ,
	[TemplateId] [int] NULL ,
	[SectionId] [int] NULL ,
	[PageURL] [varchar] (255) NULL ,
	[Name] [varchar] (50) NULL ,
	[Title] [varchar] (255) NULL ,
	[CustomURL] [varchar] (100) NULL ,
	[IsContentBefore] [bit] NOT NULL ,
	[IsIndexed] [bit] NOT NULL ,
	[IsFollowed] [bit] NOT NULL ,
	[MetaDescription] [varchar] (MAX) NULL ,
	[MetaKeywords] [varchar] (MAX) NULL ,
	[ModifyDate] [datetime] NULL ,
	[NavigationId] [int] NULL ,
	[SubNavigationId] [int] NULL ,
	[SkipIndexing] [bit] NOT NULL CONSTRAINT [DF_ContentToolPage_SkipIndexing]  DEFAULT ((0)),
	[IsPermanent] [bit] NOT NULL CONSTRAINT [DF_ContentToolPage_IsPermanent]  DEFAULT ((0))
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContentToolPageRegion] (
	[PageRegionId] [int] IDENTITY (1, 1) NOT NULL ,
	[PageId] [int] NULL ,
	[ContentRegion] [varchar] (50) NULL ,
	[RegionType] [varchar] (50) NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContentToolRegionModule] (
	[RegionModuleId] [int] IDENTITY (1, 1) NOT NULL ,
	[PageRegionId] [int] NOT NULL ,
	[ModuleId] [int] NOT NULL ,
	[SortOrder] [int] NOT NULL ,
	[Args] [varchar] (255) NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContentToolSection] (
	[SectionId] [int] IDENTITY (1, 1) NOT NULL ,
	[SectionName] [varchar] (50) NULL ,
	[Folder] [varchar] (255) NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContentToolTemplate] (
	[TemplateId] [int] IDENTITY (1, 1) NOT NULL ,
	[TemplateName] [varchar] (50) NULL ,
	[DefaultContentId] [int] NULL ,
	[IsDefault] [bit] NOT NULL ,
	[TemplateHTML] [varchar](max) NULL ,
	[PrintHTML] [varchar](max) NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[ContentToolTemplateRegion] (
	[TemplateRegionId] [int] IDENTITY (1, 1) NOT NULL ,
	[TemplateId] [int] NULL ,
	[ContentRegion] [varchar] (50) NULL ,
	[RegionName] [varchar] (50) NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Log4Net](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[DateTime] [datetime] NULL,
	[Thread] [varchar](255)  NULL,
	[ErrorLevel] [varchar](50)  NULL,
	[Logger] [varchar](255)  NULL,
	[Host] [varchar](50)  NULL,
	[ServerName] [varchar](50)  NULL,
	[UserAgent] [varchar](255)  NULL,
	[RemoteAddr] [varchar](50)  NULL,
	[Url] [varchar](max)  NULL,
	[Message] [varchar](max)  NULL,
 CONSTRAINT [PK_Log4Net] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Sysparam] (
	[ParamId] [int] NOT NULL ,
	[GroupName] [varchar] (100) NOT NULL ,
	[Name] [varchar] (50) NOT NULL ,
	[Value] [varchar] (1000) NULL ,
	[Type] [varchar] (50) NULL ,
	[SortOrder] [int] NOT NULL ,
	[IsInternal] [bit] NOT NULL ,
	[IsEncrypted] [bit] NOT NULL ,
	[Comments] [varchar](max) NOT NULL 
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[Country](
	[CountryId] [int] IDENTITY(1,1) NOT NULL,
	[CountryCode] [varchar](2) NOT NULL,
	[CountryName] [varchar](50) NOT NULL,
	[Shipping] [money] NOT NULL CONSTRAINT [DF_Country_Shipping] DEFAULT ((0)),
 CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED 
(
	[CountryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

CREATE TABLE [dbo].[State] (
	[StateId] [int] NOT NULL ,
	[StateCode] [varchar] (5) NOT NULL ,
	[StateName] [varchar] (50) NOT NULL ,
	[TaxRate] [float] NULL,
	[TaxShipping] [bit] NOT NULL CONSTRAINT [DF_State_TaxShipping] DEFAULT ((0)),
	[TaxGiftWrap] [bit] NOT NULL CONSTRAINT [DF_State_TaxGiftWrap] DEFAULT ((0))
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[Admin] WITH NOCHECK ADD 
	CONSTRAINT [PK_Admin] PRIMARY KEY CLUSTERED 
	(
		[AdminId]
	) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AdminAccess] WITH NOCHECK ADD 
	CONSTRAINT [PK_AdminAccess] PRIMARY KEY CLUSTERED 
	(
		[SectionId],
		[GroupId]
	) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AdminAdminGroup] WITH NOCHECK ADD 
	CONSTRAINT [PK_AdminAdminGroup] PRIMARY KEY CLUSTERED 
	(
		[AdminId],
		[GroupId]
	) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AdminGroup] WITH NOCHECK ADD 
	CONSTRAINT [PK_AdminGroup] PRIMARY KEY CLUSTERED 
	(
		[GroupId]
	) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AdminLog] WITH NOCHECK ADD 
	CONSTRAINT [PK_AdminLog] PRIMARY KEY CLUSTERED 
	(
		[LogId]
	) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AdminSection] WITH NOCHECK ADD 
	CONSTRAINT [PK_AdminSection] PRIMARY KEY CLUSTERED 
	(
		[SectionId]
	) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AdminPassword] WITH NOCHECK ADD 
	CONSTRAINT [PK_AdminPassword] PRIMARY KEY CLUSTERED 
	(
		[PasswordId]
	) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[AdminPassword] ADD 
	CONSTRAINT [FK_AdminPassword_Admin] FOREIGN KEY 
	(
		[AdminId]
	) REFERENCES [dbo].[Admin] (
		[AdminId]
	)
GO

ALTER TABLE [dbo].[ContentToolContent] WITH NOCHECK ADD 
	CONSTRAINT [PK_Content] PRIMARY KEY CLUSTERED 
	(
		[ContentId]
	) WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ContentToolModule] WITH NOCHECK ADD 
	CONSTRAINT [PK_Module] PRIMARY KEY CLUSTERED 
	(
		[ModuleId]
	) WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ContentToolNavigation] WITH NOCHECK ADD 
	CONSTRAINT [PK_SiteSection] PRIMARY KEY CLUSTERED 
	(
		[NavigationId]
	) ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ContentToolPage] WITH NOCHECK ADD 
	CONSTRAINT [IX_ContentToolPage] UNIQUE CLUSTERED 
	(
		[TemplateId],
		[SectionId],
		[PageId]
	) WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ContentToolPageRegion] WITH NOCHECK ADD 
	CONSTRAINT [PK_PageModule] PRIMARY KEY CLUSTERED 
	(
		[PageRegionId]
	) WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ContentToolSection] WITH NOCHECK ADD 
	CONSTRAINT [PK_ContentToolSection] PRIMARY KEY CLUSTERED 
	(
		[SectionId]
	) WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ContentToolTemplate] WITH NOCHECK ADD 
	CONSTRAINT [PK_Template] PRIMARY KEY CLUSTERED 
	(
		[TemplateId]
	) WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Sysparam] WITH NOCHECK ADD 
	CONSTRAINT [Sysparam_PK] PRIMARY KEY CLUSTERED 
	(
		[ParamId]
	) WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

 CREATE CLUSTERED INDEX [IX_ContentToolRegionModule] ON [dbo].[ContentToolRegionModule]([PageRegionId], [ModuleId]) WITH FILLFACTOR = 90 ON [PRIMARY]
GO

 CREATE CLUSTERED INDEX [IX_ContentToolTemplateRegion] ON [dbo].[ContentToolTemplateRegion]([TemplateId], [ContentRegion]) WITH FILLFACTOR = 90 ON [PRIMARY]
GO

ALTER TABLE [dbo].[Admin] ADD 
	CONSTRAINT [DF_Admin_IsInternal] DEFAULT (0) FOR [IsInternal]
GO

ALTER TABLE [dbo].[AdminLog] ADD 
	CONSTRAINT [DF_AdminLog_Succeeded] DEFAULT (0) FOR [Succeeded]
GO

ALTER TABLE [dbo].[Admin] ADD 
	CONSTRAINT [DF_Admin_IsLocked] DEFAULT (0) FOR [IsLocked]
GO

ALTER TABLE [dbo].[ContentToolNavigation] ADD 
	CONSTRAINT [DF_ContentToolMenu_IsInternalLink] DEFAULT (0) FOR [IsInternalLink],
	CONSTRAINT [DF_ContentToolNavigation_SkipSiteMap] DEFAULT (0) FOR [SkipSiteMap],
	CONSTRAINT [DF_ContentToolNavigation_SkipBreadCrumb] DEFAULT (0) FOR [SkipBreadCrumb]
GO

ALTER TABLE [dbo].[ContentToolPage] ADD 
	CONSTRAINT [DF_ContentToolPage_IsContentBefore] DEFAULT (0) FOR [IsContentBefore],
	CONSTRAINT [DF_ContentToolPage_IsIndexed] DEFAULT (1) FOR [IsIndexed],
	CONSTRAINT [DF_ContentToolPage_IsFollowed] DEFAULT (1) FOR [IsFollowed],
	CONSTRAINT [PK_Page] PRIMARY KEY NONCLUSTERED 
	(
		[PageId]
	) WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

 CREATE INDEX [IX_ContentToolPage_PageURL] ON [dbo].[ContentToolPage]([PageURL]) WITH FILLFACTOR = 90 ON [PRIMARY]
GO

ALTER TABLE [dbo].[ContentToolPageRegion] ADD 
	CONSTRAINT [CK_ContentToolPageRegion] CHECK ([RegionType] = 'Custom' or [RegionType] = 'Default')
GO

 CREATE INDEX [IX_ContentToolPageRegion] ON [dbo].[ContentToolPageRegion]([PageId], [RegionType]) WITH FILLFACTOR = 90 ON [PRIMARY]
GO

ALTER TABLE [dbo].[ContentToolRegionModule] ADD 
	CONSTRAINT [PK_ContentToolRegionModule] PRIMARY KEY NONCLUSTERED 
	(
		[RegionModuleId]
	) WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ContentToolSection] ADD 
	CONSTRAINT [IX_ContentToolSection] UNIQUE NONCLUSTERED 
	(
		[Folder]
	) WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

ALTER TABLE [dbo].[ContentToolTemplate] ADD 
	CONSTRAINT [DF_Template_IsDefault] DEFAULT (0) FOR [IsDefault]
GO

ALTER TABLE [dbo].[ContentToolTemplateRegion] ADD 
	CONSTRAINT [PK_TemplateRegion] PRIMARY KEY NONCLUSTERED 
	(
		[TemplateRegionId]
	) WITH FILLFACTOR = 90 ON [PRIMARY] 
GO

ALTER TABLE [dbo].[Sysparam] ADD 
	CONSTRAINT [DF_Sysparam_IsInternal] DEFAULT (0) FOR [IsInternal]
GO

ALTER TABLE [dbo].[Sysparam] ADD 
	CONSTRAINT [DF_Sysparam_IsEncrypted] DEFAULT (0) FOR [IsEncrypted]
GO

ALTER TABLE [dbo].[AdminAccess] ADD 
	CONSTRAINT [AdminGroup_AdminAccess_FK1] FOREIGN KEY 
	(
		[GroupId]
	) REFERENCES [dbo].[AdminGroup] (
		[GroupId]
	),
	CONSTRAINT [AdminSection_AdminAccess_FK1] FOREIGN KEY 
	(
		[SectionId]
	) REFERENCES [dbo].[AdminSection] (
		[SectionId]
	),
	CONSTRAINT [FK_AdminAccess_AdminSection] FOREIGN KEY 
	(
		[SectionId]
	) REFERENCES [dbo].[AdminSection] (
		[SectionId]
	)
GO

ALTER TABLE [dbo].[AdminAdminGroup] ADD 
	CONSTRAINT [Admin_AdminAdminGroup_FK1] FOREIGN KEY 
	(
		[AdminId]
	) REFERENCES [dbo].[Admin] (
		[AdminId]
	),
	CONSTRAINT [AdminGroup_AdminAdminGroup_FK1] FOREIGN KEY 
	(
		[GroupId]
	) REFERENCES [dbo].[AdminGroup] (
		[GroupId]
	),
	CONSTRAINT [FK_AdminAdminGroup_Admin] FOREIGN KEY 
	(
		[AdminId]
	) REFERENCES [dbo].[Admin] (
		[AdminId]
	),
	CONSTRAINT [FK_AdminAdminGroup_AdminGroup] FOREIGN KEY 
	(
		[GroupId]
	) REFERENCES [dbo].[AdminGroup] (
		[GroupId]
	)
GO

ALTER TABLE [dbo].[ContentToolNavigation] ADD 
	CONSTRAINT [FK_ContentToolNavigation_ContentToolNavigation] FOREIGN KEY 
	(
		[ParentId]
	) REFERENCES [dbo].[ContentToolNavigation] (
		[NavigationId]
	)
GO

ALTER TABLE [dbo].[ContentToolPage] ADD 
	CONSTRAINT [FK_ContentToolPage_ContentToolNavigation] FOREIGN KEY 
	(
		[NavigationId]
	) REFERENCES [dbo].[ContentToolNavigation] (
		[NavigationId]
	),
	CONSTRAINT [FK_ContentToolPage_ContentToolNavigationSub] FOREIGN KEY 
	(
		[SubNavigationId]
	) REFERENCES [dbo].[ContentToolNavigation] (
		[NavigationId]
	),
	CONSTRAINT [FK_ContentToolPage_ContentToolSection] FOREIGN KEY 
	(
		[SectionId]
	) REFERENCES [dbo].[ContentToolSection] (
		[SectionId]
	),
	CONSTRAINT [FK_ContentToolPage_ContentToolTemplate] FOREIGN KEY 
	(
		[TemplateId]
	) REFERENCES [dbo].[ContentToolTemplate] (
		[TemplateId]
	)
GO

ALTER TABLE [dbo].[ContentToolRegionModule] ADD 
	CONSTRAINT [FK_ContentToolRegionModule_ContentToolModule] FOREIGN KEY 
	(
		[ModuleId]
	) REFERENCES [dbo].[ContentToolModule] (
		[ModuleId]
	),
	CONSTRAINT [FK_ContentToolRegionModule_ContentToolPageRegion1] FOREIGN KEY 
	(
		[PageRegionId]
	) REFERENCES [dbo].[ContentToolPageRegion] (
		[PageRegionId]
	)
GO

ALTER TABLE [dbo].[ContentToolTemplateRegion] ADD 
	CONSTRAINT [FK_ContentToolTemplateRegion_ContentToolTemplate] FOREIGN KEY 
	(
		[TemplateId]
	) REFERENCES [dbo].[ContentToolTemplate] (
		[TemplateId]
	)
GO

ALTER TABLE [dbo].[State] WITH NOCHECK ADD 
	CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED 
	(
		[StateId]
	) ON [PRIMARY] 
GO



IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_StoreDepartmentDelete]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
-- CREATE PROCEDURE
CREATE PROCEDURE [dbo].[sp_StoreDepartmentDelete]
	@DepartmentId int
AS
DECLARE @lft int,
		@rgt int,
		@rc int
		
-- get @lft and @rgt for department to delete
SELECT @lft = LFT, @rgt = RGT FROM StoreDepartment WHERE DepartmentId = @DepartmentId
DELETE FROM StoreDepartment 
WHERE rgt <= @rgt and lft >= @lft
-- GET NUMBER OF AFFECTED ROWS ( MULTIPLIED BY 2)
set @rc= 2 * @@rowcount
UPDATE StoreDepartment
	SET	lft = CASE WHEN lft > @rgt
 THEN lft - @rc
 ELSE lft END,
 	rgt = CASE WHEN rgt >= @rgt
 THEN rgt - @rc
 ELSE rgt END
WHERE rgt >= @rgt

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreFeature]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreFeature](
	[FeatureId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[IsUnique] [bit] NOT NULL CONSTRAINT [DF_StoreFeature_IsUnique] DEFAULT ((0)),
 CONSTRAINT [PK_StoreFeature] PRIMARY KEY CLUSTERED 
(
	[FeatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreItemTemplate]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreItemTemplate](
	[TemplateId] [int] IDENTITY(1,1) NOT NULL,
	[TemplateName] [varchar](50) NOT NULL,
	[IsAttributes] [bit] NOT NULL CONSTRAINT [DF_StoreTemplate_IsAttributes] DEFAULT ((0)),
	[DisplayMode] [varchar](20)  NOT NULL CONSTRAINT [DF_StoreItemTemplate_DisplayMode]  DEFAULT ('AdminDriven'),
	[IsToAndFrom] [bit] NOT NULL CONSTRAINT [DF_StoreTemplate_IsToAndFrom] DEFAULT ((0)),
	[IsGiftMessage] [bit] NOT NULL CONSTRAINT [DF_StoreTemplate_IsGiftMessage] DEFAULT ((0)),
 CONSTRAINT [PK_StoreItemTemplate] PRIMARY KEY CLUSTERED 
(
	[TemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
ALTER TABLE dbo.StoreItemTemplate ADD CONSTRAINT
	CK_StoreItemTemplate CHECK (([DisplayMode] = 'AdminDriven' OR [DisplayMode] = 'TableLayout'))
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_StoreDepartmentInsert]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'

-- CREATE PROCEDURE
CREATE PROCEDURE [dbo].[sp_StoreDepartmentInsert]
	@ParentId int, 
	@Name varchar(50)
AS
DECLARE @right_most_sibling int
DECLARE @first_greater_sibling int
SELECT @right_most_sibling
 = (SELECT rgt 
 FROM StoreDepartment
 WHERE DepartmentId = @ParentId);
SELECT @first_greater_sibling
 = (SELECT TOP 1 lft
 FROM StoreDepartment
 WHERE ParentId = @ParentId AND Name > @Name order by Name asc);
IF @first_greater_sibling IS NULL
	BEGIN
		UPDATE StoreDepartment
			SET	lft = CASE WHEN lft > @right_most_sibling
		 THEN lft + 2
		 ELSE lft END,
		 	rgt = CASE WHEN rgt >= @right_most_sibling
		 THEN rgt + 2
		 ELSE rgt END
		WHERE rgt >= @right_most_sibling
	
		INSERT INTO StoreDepartment (lft, rgt, ParentId, Name)
		VALUES (@right_most_sibling, (@right_most_sibling + 1), @ParentId, @Name)
	END
ELSE
	BEGIN
		UPDATE StoreDepartment
			SET	lft = CASE WHEN lft >= @first_greater_sibling
		 THEN lft + 2
		 ELSE lft END,
		 	rgt = CASE WHEN rgt >= @first_greater_sibling
		 THEN rgt + 2
		 ELSE rgt END
		WHERE rgt >= @first_greater_sibling
	
		INSERT INTO StoreDepartment (lft, rgt, ParentId, Name)
		VALUES (@first_greater_sibling, (@first_greater_sibling + 1), @ParentId, @Name)
	END



' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_StoreDepartmentMove]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
-- CREATE PROCEDURE
CREATE PROCEDURE [dbo].[sp_StoreDepartmentMove]
	@DepartmentId int, 
	@TargetId int
AS
DECLARE @ParentId int,
		@first_greater_sibling int, 
		@right_most_sibling int,
		@lft int, 
		@rgt int,
		@increment int,
		@rc int,
		@name varchar(50)
		
	-- set increment value for temporary moved departments
	select @increment = 1000000
	
	-- get @lft, @rgt, @name and @ParentId for department to move
	SELECT @lft = LFT, @rgt = RGT, @name = NAME, @ParentId = ParentId FROM StoreDepartment WHERE DepartmentId = @DepartmentId
	
	-- if @lft = 1, which means that this is a root node, then display comment and return
	IF @lft = 1
	BEGIN
		PRINT ''CUSTOM: Main department cannot be moved to any location''
		RETURN
	END
	
	-- if @ParentId is equal @TargetId then display message and return
	IF @ParentId = @TargetId
	BEGIN
		PRINT ''CUSTOM: department cannot be moved to the same location''
		RETURN
	END
	-- if @TargetId is among departments that are children of @DepartmentId then
	-- operation cannot be performed as well
	
	IF EXISTS( 
		SELECT f1.DepartmentId FROM StoreDepartment AS f1, StoreDepartment AS f2 
		WHERE f1.LFT BETWEEN f2.lft AND f2.rgt 
		 AND f2.DepartmentId = @DepartmentId 
		 AND f1.DepartmentId = @TargetId )
	BEGIN
		PRINT ''CUSTOM: department cannot be moved to it''''s own subdepartments''
		RETURN
	END
	-- Move departments to temporary location by adding @increment to @lft and @rgt
	-- (virtually removing)
	UPDATE StoreDepartment SET
	lft = lft + @increment, rgt = rgt + @increment
	where lft >= @lft and rgt <= @rgt
	-- GET NUMBER OF AFFECTED ROWS ( MULTIPLIED BY 2)
	set @rc= 2 * @@rowcount
	UPDATE StoreDepartment
	SET	lft = CASE WHEN lft > @rgt
 THEN lft - @rc
 ELSE lft END,
 	rgt = CASE WHEN rgt >= @rgt
 THEN rgt - @rc
 ELSE rgt END
	WHERE rgt >= @rgt
	
	-- Re-Insert moved departments to proper location
	SELECT @right_most_sibling = rgt FROM StoreDepartment
	WHERE DepartmentId = @TargetId
	
	SELECT TOP 1 @first_greater_sibling = lft FROM StoreDepartment
 WHERE ParentId = @TargetId AND NAME > @name order by name asc
	
	IF @first_greater_sibling IS NULL
		BEGIN
			UPDATE StoreDepartment
				SET	lft = CASE WHEN lft > @right_most_sibling
			 THEN lft + @rc
			 ELSE lft END,
			 	rgt = CASE WHEN rgt >= @right_most_sibling
			 THEN rgt + @rc
			 ELSE rgt END
			WHERE rgt >= @right_most_sibling
		
			-- MOVE StoreDepartment BACK TO ACCURATE LOCATION
			UPDATE StoreDepartment
			SET	lft 	 = lft - @increment - @lft + @right_most_sibling,
			 	rgt 	 = rgt - @increment - @lft + @right_most_sibling,
		 	ParentId = 
				 	CASE WHEN ParentId = @ParentId 
				 	THEN @TargetId
				 	ELSE ParentId END
			WHERE lft >= (@lft + @increment) and rgt <= (@rgt + @increment)
		END
	ELSE
		BEGIN
			UPDATE StoreDepartment
				SET	lft = CASE WHEN lft >= @first_greater_sibling
			 THEN lft + @rc
			 ELSE lft END,
			 	rgt = CASE WHEN rgt >= @first_greater_sibling
			 THEN rgt + @rc
			 ELSE rgt END
			WHERE rgt >= @first_greater_sibling
			-- MOVE StoreDepartment BACK TO ACCURATE LOCATION
			UPDATE StoreDepartment
			SET	lft 	 = lft - @increment - @lft + @first_greater_sibling,
			 	rgt 	 = rgt - @increment - @lft + @first_greater_sibling,
		 	ParentId = 
				 	CASE WHEN ParentId = @ParentId 
				 	THEN @TargetId
				 	ELSE ParentId END
			WHERE lft >= (@lft + @increment) and rgt <= (@rgt + @increment)
		END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreRecentlyViewed]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreRecentlyViewed](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NOT NULL,
	[DepartmentId] [int] NULL,
	[BrandId] [int] NULL,
	[SessionNo] [varchar](50) NOT NULL,
	[MemberId] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_StoreRecentlyViewed] PRIMARY KEY NONCLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SearchTerm]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[SearchTerm](
	[TermId] [int] IDENTITY(1,1) NOT NULL,
	[Term] [varchar](255) NOT NULL,
	[RemoteIP] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[OrderId] [int] NULL,
	[MemberId] [int] NULL,
	[NumberResults] [int] NOT NULL CONSTRAINT [DF_SearchTerm_NumberResults] DEFAULT ((0)),
 CONSTRAINT [PK_SearchTerm] PRIMARY KEY CLUSTERED 
(
	[TermId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetRecentlyViewedItems]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'CREATE Procedure [dbo].[sp_GetRecentlyViewedItems]
@Howmany int,
@CurrentItemId int,
@MemberId varchar(10),
@SessionNo varchar(50)
as
set nocount on
CREATE TABLE [#RecentViewedItem] (
 [ItemId] [int] NOT NULL ,
 [DepartmentId] [int] NULL ,
 [BrandId] [int] NULL ,
 [SessionNo] [varchar] (50) NOT NULL ,
 [MemberId] [int] ,
 [CreateDate] [datetime] NOT NULL
)
While (@Howmany > 0)
BEGIN
 insert into [#RecentViewedItem] select top 1 vi.ItemId, vi.DepartmentId, vi.BrandId, vi.SessionNo, vi.MemberId, vi.CreateDate from StoreRecentlyViewed vi, StoreItem si where si.IsActive = 1 and si.ItemId = vi.ItemId and NOT si.ItemId = @CurrentItemId AND si.ItemId NOT IN (SELECT ItemId FROM [#RecentViewedItem]) AND (SessionNo = @SessionNo OR (@MemberId IS NOT NULL AND Coalesce(MemberId,0) = @MemberId)) ORDER BY vi.CreateDate DESC
 Set @Howmany = @Howmany - 1
END
set nocount off
select ri.DepartmentId, ri.BrandId, si.* from [#RecentViewedItem] ri, StoreItem si WHERE si.ItemId = ri.ItemId order by ri.CreateDate DESC
DROP TABLE [#RecentViewedItem]


' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReferralClick]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ReferralClick](
	[ClickId] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NULL,
	[RemoteIP] [varchar](50) NULL,
	[ClickDate] [datetime] NULL,
 CONSTRAINT [PK_ReferralClick] PRIMARY KEY CLUSTERED 
(
	[ClickId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreItemTemplateAttribute]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreItemTemplateAttribute](
	[TemplateAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NOT NULL,
	[ParentId] [int] NULL,
	[AttributeName] [varchar](50) NOT NULL,
	[FunctionType] [varchar](20) NOT NULL,
	[AttributeType] [varchar](20) NOT NULL,
	[SpecifyValue] [varchar](max) NULL,
	[LookupTable] [varchar](50) NULL,
	[LookupColumn] [varchar](50) NULL,
	[SKUColumn] [varchar](50) NULL,
	[IncludeSKU] [bit] NOT NULL CONSTRAINT [DF_StoreItemTemplateAttribute_IncludeSKU]  DEFAULT ((0)),
	[PriceColumn] [varchar](50) NULL,
	[WeightColumn] [varchar](50) NULL,
	[SwatchColumn] [varchar](50) NULL,
	[SwatchAltColumn] [varchar](50) NULL,
	[IsInventoryManagement] [bit] NOT NULL CONSTRAINT [DF_StoreItemTemplateAttribute_IsInventoryManagement]  DEFAULT ((0)),
	[SortOrder] [int] NOT NULL CONSTRAINT [DF_StoreTemplateAttribute_SortOrder] DEFAULT ((0)),
 CONSTRAINT [PK_StoreItemTemplateAttribute] PRIMARY KEY NONCLUSTERED 
(
	[TemplateAttributeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_MailingRemoveDuplicates]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE procedure [dbo].[sp_MailingRemoveDuplicates]
	@MessageId int, 
	@MimeType varchar(10)
AS
-- create temporary table
DECLARE @tmp_duplicates TABLE (MemberId int not null, Email varchar(255) primary key ([MemberId]))

-- insert duplicates
insert into @tmp_duplicates (MemberId, Email)
SELECT MIN(MemberId) AS MemberId, Email FROM MailingRecipient mr
WHERE 	mr.MessageId = @MessageId
AND 	mr.MimeType = @MimeType
group by mr.Email, mr.MimeType having count(*) > 1

-- delete duplicates
DELETE FROM MailingRecipient WHERE MessageId = @MessageId AND MimeType = @MimeType 
AND Email IN (SELECT Email FROM @tmp_duplicates td WHERE MailingRecipient.Email = td.Email) 
AND MemberId NOT IN (SELECT MemberId FROM @tmp_duplicates td where MailingRecipient.MemberId = td.MemberId)' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreOrderNote]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreOrderNote](
	[NoteId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[NoteDate] [datetime] NOT NULL,
	[AdminId] [int] NOT NULL,
	[Note] [varchar](max) NOT NULL,
 CONSTRAINT [PK_StoreOrderNote] PRIMARY KEY NONCLUSTERED 
(
	[NoteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreItemFeature]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreItemFeature](
	[ItemId] [int] NOT NULL,
	[FeatureId] [int] NOT NULL,
 CONSTRAINT [PK_StoreItemFeature] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC,
	[FeatureId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_StoreDepartmentRename]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
-- CREATE PROCEDURE
CREATE PROCEDURE [dbo].[sp_StoreDepartmentRename]
	@DepartmentId int, 
	@name varchar(50)
AS
DECLARE @ParentId int,
		@first_greater_sibling int, 
		@right_most_sibling int,
		@lft int, 
		@rgt int,
		@increment int,
		@rc int
		
-- set increment value for temporary moved departments
select @increment = 1000000
-- get @lft and @rgt for department to rename
SELECT @lft = LFT, @rgt = RGT FROM StoreDepartment WHERE DepartmentId = @DepartmentId
-- get @ParentId for department to rename
SELECT @ParentId = ParentId FROM StoreDepartment WHERE DepartmentId = @DepartmentId
	
-- if ParentId is null then just rename department and no additional action is required
IF @ParentId IS NULL
	BEGIN
		UPDATE StoreDepartment SET 
		NAME = @name 
		WHERE DepartmentId = @DepartmentId
	END	
ELSE
	BEGIN
		SELECT TOP 1 @first_greater_sibling = lft
		FROM StoreDepartment
	 WHERE ParentId = @ParentId AND NAME > @name order by name asc
		SELECT @right_most_sibling = rgt 
 FROM StoreDepartment
 WHERE DepartmentId = @ParentId
		-- There are 4 possiblity here
		-- (A) 	@first_greater_sibling is equal to @lft
		-- (B) 	@first_greater_sibling may be lower than @lft
		-- (C) 	@first_greater_sibling may be greater than @lft
		-- (D) 	@first_greater_sibling may be not found
		-- (A) 	@first_greater_sibling is equal to @lft
		-- 		department after renaming doesn''t change position
		-- 		In this case department is only renamed and no additional action is taken
		IF @first_greater_sibling = @lft
		BEGIN
			UPDATE StoreDepartment SET NAME = @name
			WHERE DepartmentId = @DepartmentId
		END
		
		-- (B) 	@first_greater_sibling is lower than @lft
		IF @first_greater_sibling < @lft
		BEGIN
			-- Move departments to temporary location by adding @increment to @lft and @rgt
			UPDATE StoreDepartment SET
			lft = lft + @increment, rgt = rgt + @increment
			where lft >= @lft and rgt <= @rgt
			UPDATE StoreDepartment
			SET	lft = lft + (@rgt-@lft) + 1,
		 	rgt = rgt + (@rgt-@lft) + 1
			WHERE lft >= @first_greater_sibling and rgt < @rgt
			-- GET NUMBER OF AFFECTED ROWS ( MULTIPLIED BY 2)
			set @rc= 2 * @@rowcount
			UPDATE StoreDepartment
			SET	lft = lft - @increment - @rc,
		 	rgt = rgt - @increment - @rc
			WHERE lft >= (@lft + @increment) and rgt <= (@rgt + @increment)
			UPDATE StoreDepartment SET NAME = @name
			WHERE DepartmentId = @DepartmentId
		END
		-- (C) 	@first_greater_sibling is greater than @lft
		IF @first_greater_sibling > @lft
		BEGIN
			-- Move departments to temporary location by adding @increment to @lft and @rgt
			UPDATE StoreDepartment SET
			lft = lft + @increment, rgt = rgt + @increment
			where lft >= @lft and rgt <= @rgt
			UPDATE StoreDepartment
			SET	lft = lft - (@rgt-@lft) - 1,
		 	rgt = rgt - (@rgt-@lft) - 1
			WHERE rgt < @first_greater_sibling and lft > @lft
			-- GET NUMBER OF AFFECTED ROWS ( MULTIPLIED BY 2)
			set @rc= 2 * @@rowcount
			UPDATE StoreDepartment
			SET	lft = lft - @increment + @rc,
		 	rgt = rgt - @increment + @rc
			WHERE lft >= (@lft + @increment) and rgt <= (@rgt + @increment)
			UPDATE StoreDepartment SET NAME = @name
			WHERE DepartmentId = @DepartmentId
		END
		-- (D) 	@first_greater_sibling has not been found
		IF @first_greater_sibling is null
		BEGIN
			-- Move departments to temporary location by adding @increment to @lft and @rgt
			UPDATE StoreDepartment SET
			lft = lft + @increment, rgt = rgt + @increment
			where lft >= @lft and rgt <= @rgt
			UPDATE StoreDepartment
			SET	lft = lft - (@rgt-@lft) - 1,
		 	rgt = rgt - (@rgt-@lft) - 1
			WHERE rgt < @right_most_sibling and lft > @lft
			-- GET NUMBER OF AFFECTED ROWS ( MULTIPLIED BY 2)
			set @rc= 2 * @@rowcount
			UPDATE StoreDepartment
			SET	lft = lft - @increment + @rc,
		 	rgt = rgt - @increment + @rc
			WHERE lft >= (@lft + @increment) and rgt <= (@rgt + @increment)
			UPDATE StoreDepartment SET NAME = @name
			WHERE DepartmentId = @DepartmentId
		END
	END

' 
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[State]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[State](
	[StateId] [int] NOT NULL,
	[StateCode] [varchar](5) NOT NULL,
	[StateName] [varchar](50) NOT NULL,
	[TaxRate] [float] NOT NULL,
	[TaxShipping] [bit] NOT NULL CONSTRAINT [DF_State_TaxShipping] DEFAULT ((0)),
	[TaxGiftWrap] [bit] NOT NULL CONSTRAINT [DF_State_TaxGiftWrap] DEFAULT ((0)),
 CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED 
(
	[StateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mailing_Lyris_Outmail]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Mailing_Lyris_Outmail](
	[Created_] [datetime] NULL,
	[E_Success_] [text] NULL,
	[E_SuccessCount_] [int] NULL,
	[E_ToSend_] [text] NULL,
	[E_ToSendCount_] [int] NULL,
	[E_Trans_] [text] NULL,
	[E_TransCount_] [int] NULL,
	[E_Unreach_] [text] NULL,
	[E_UnreachCount_] [int] NULL,
	[Finished_] [smalldatetime] NULL,
	[ID_Success_] [text] NULL,
	[ID_SuccessCount_] [int] NULL,
	[ID_ToSend_] [text] NULL,
	[ID_ToSendCount_] [int] NULL,
	[ID_Trans_] [text] NULL,
	[ID_TransCount_] [int] NULL,
	[ID_Unreach_] [text] NULL,
	[ID_UnreachCount_] [int] NULL,
	[ListID_] [int] NOT NULL,
	[Priority_] [tinyint] NULL,
	[RetryTime_] [smalldatetime] NULL,
	[SendDate_] [smalldatetime] NULL,
	[SendTry_] [tinyint] NULL,
	[Status_] [varchar](20) NULL,
	[Transact_] [text] NULL,
 CONSTRAINT [PK_Mailing_Lyris_Outmail] PRIMARY KEY NONCLUSTERED 
(
	[ListID_] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_MailingLyrisOutmail] UNIQUE CLUSTERED 
(
	[ListID_] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreOrderStatus]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreOrderStatus](
	[StatusId] [int] NOT NULL,
	[Code] [varchar](10) NULL,
	[Name] [varchar](50) NULL,
	[ProcessSortOrder] [int] NOT NULL,
	[IsFinalAction] [bit] NOT NULL default 0,
 CONSTRAINT [PK_StoreOrderStatus] PRIMARY KEY CLUSTERED 
(
	[StatusId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BannerOrderTracking]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BannerOrderTracking](
	[TrackingId] [int] IDENTITY(1,1) NOT NULL,
	[BannerId] [int] NOT NULL,
	[OrderId] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_BannerOrderTracking] PRIMARY KEY NONCLUSTERED 
(
	[TrackingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreShippingRange]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreShippingRange](
	[RangeId] [int] IDENTITY(1,1) NOT NULL,
	[ShippingFrom] [money] NULL,
	[ShippingTo] [money] NULL,
	[ShippingValue] [money] NULL,
 CONSTRAINT [PK_StoreShippingRange] PRIMARY KEY CLUSTERED 
(
	[RangeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MemberWishlistItemAttribute]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MemberWishlistItemAttribute](
	[UniqueId] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[WishlistItemId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[ItemAttributeId] [int] NOT NULL,
	[ParentAttributeId] [int] NULL,
	[TemplateAttributeId] [int] NOT NULL,
	[AttributeValue] [varchar](255) NOT NULL,
	[SKU] [varchar](25) NULL,
	[Price] [money] NULL,
	[Weight] [float] NOT NULL CONSTRAINT [DF_MemberWishlistItemAttribute_Weight]  DEFAULT ((0)),
	[ImageName] [varchar](100) NULL,
	[ImageAlt] [varchar](255) NULL,
	[ProductImage] [varchar](100) NULL,
	[ProductAlt] [varchar](255) NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_MemberWishlistItemAttribute] PRIMARY KEY NONCLUSTERED 
(
	[UniqueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[HowHeard]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[HowHeard](
	[HowHeardId] [int] IDENTITY(1,1) NOT NULL,
	[HowHeard] [varchar](50) NOT NULL,
	[IsUserInput] [bit] NOT NULL CONSTRAINT [DF_HowHeard_IsUserInput]  DEFAULT ((0)),
	[UserInputLabel] [varchar](50) NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_HowHeard] PRIMARY KEY CLUSTERED 
(
	[HowHeardId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CreditCardType]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CreditCardType](
	[CardTypeId] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](10) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[ImageName] [varchar](50) NULL,
	[IsActive] [bit] NULL,
	[SortOrder] [int] NULL,
 CONSTRAINT [PK_CreditCardType] PRIMARY KEY CLUSTERED 
(
	[CardTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Banner]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Banner](
	[BannerId] [int] IDENTITY(1,1) NOT NULL,
	[BannerType] [varchar](20) NOT NULL,
	[Width] [int] NOT NULL,
	[Height] [int] NOT NULL,
	[Name] [varchar](255) NOT NULL,
	[Link] [varchar](255) NOT NULL,
	[Filename] [varchar](255) NOT NULL,
	[AltText] [varchar](255) NULL,
	[Target] [varchar](50) NULL,
	[IsActive] [bit] NOT NULL,
	[IsOrderTracking] [bit] NOT NULL CONSTRAINT [DF_Banner_IsOrderTracking] DEFAULT ((0)),
	[HTML] [varchar](max) NULL,
 CONSTRAINT [PK_Banner] PRIMARY KEY CLUSTERED 
(
	[BannerId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Member]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Member](
	[MemberId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [varchar](50) NOT NULL,
	[Password] [varchar](75) NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsSameDefaultAddress] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_Member_CreateDate] DEFAULT (getdate()),
	[ModifyDate] [datetime] NULL,
	[Guid] [varchar](32) NULL,
 CONSTRAINT [PK_Member] PRIMARY KEY CLUSTERED 
(
	[MemberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StorePromotion]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StorePromotion](
	[PromotionId] [int] IDENTITY(1,1) NOT NULL,
	[PromotionName] [varchar](255) NULL,
	[PromotionCode] [varchar](50) NULL,
	[PromotionType] [varchar](50) NULL,
	[Message] [varchar](255) NULL,
	[Discount] [money] NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NOT NULL,
	[MinimumPurchase] [money] NULL,
	[MaximumPurchase] [money] NULL,
	[IsItemSpecific] [bit] NOT NULL,
	[IsFreeShipping] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[NumberSent] [int] NULL,
	[DeliveryMethod] [varchar](50) NULL,
	TimesApplicable int NOT NULL CONSTRAINT DF_StorePromotion_TimesApplicable DEFAULT 0,
	TimesUsed int NOT NULL CONSTRAINT DF_StorePromotion_TimesUsed DEFAULT 0
 CONSTRAINT [PK_StorePromotion] PRIMARY KEY CLUSTERED 
(
	[PromotionId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingMessage]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingMessage](
	[MessageId] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL,
	[ListPrefix] [varchar](50) NULL,
	[ListHTMLId] [int] NULL,
	[ListTextId] [int] NULL,
	[NewsletterDate] [datetime] NULL,
	[Step1] [bit] NULL,
	[Step2] [bit] NULL,
	[Step3] [bit] NULL,
	[GroupId] [int] NULL,
	[Name] [varchar](100) NOT NULL,
	[TemplateId] [int] NOT NULL,
	[MimeType] [varchar](50) NOT NULL,
	[FromEmail] [varchar](255) NULL,
	[FromName] [varchar](255) NULL,
	[ReplyEmail] [varchar](255) NULL,
	[SentDate] [datetime] NULL,
	[Subject] [varchar](255) NULL,
	[ScheduledDate] [datetime] NULL,
	[Status] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateAdminId] [int] NOT NULL,
	[ModifyDate] [datetime] NOT NULL,
	[ModifyAdminId] [int] NOT NULL,
	[HTMLQuery] [varchar](max) NULL,
	[HTMLLyrisQuery] [varchar](max) NULL,
	[TextQuery] [varchar](max) NULL,
	[TextLyrisQuery] [varchar](max) NULL,
	[TargetType] [varchar](20) NULL,
	[MessageHTML] [varchar](max) NULL,
	[MessageText] [varchar](max) NULL,
	[SavedText] [varchar](max) NULL,
 CONSTRAINT [PK_MailingMessage] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreShippingMethod]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreShippingMethod](
	[MethodId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[UPSCode] [varchar](50) NULL,
	[FedExCode] [varchar](50) NULL,
	[SortOrder] [int] NOT NULL,
	[IsInternational] bit not null default(0), 
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_StoreShippingMethod_IsActive] DEFAULT ((0)),
 CONSTRAINT [PK_StoreShippingMethod] PRIMARY KEY CLUSTERED 
(
	[MethodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingTemplate]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingTemplate](
	[TemplateId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[ImageName] [varchar](255) NOT NULL,
	[HTMLMember] [varchar](max) NOT NULL,
	[TextMember] [varchar](max) NULL,
	[HTMLDynamic] [varchar](max) NULL,
	[TextDynamic] [varchar](max) NULL,
 CONSTRAINT [PK_MailingTemplate] PRIMARY KEY CLUSTERED 
(
	[TemplateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreItemAttributeTemp]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreItemAttributeTemp](
	[TempAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[Guid] [varchar](50) NOT NULL,
	[ItemAttributeId] [int] NULL,
	[TemplateAttributeId] [int] NOT NULL,
	[ParentAttributeId] [int] NULL,
	[ItemId] [int] NULL,
	[AttributeValue] [varchar](255) NOT NULL,
	[SKU] [varchar](25) NULL,
	[Price] [money] NOT NULL,
	[Weight] [float] NOT NULL CONSTRAINT [DF_StoreItemAttributeTemp_Weight]  DEFAULT ((0)),
	[InventoryQty] [int] NOT NULL CONSTRAINT [DF_StoreItemAttributeTemp_InventoryQty]  DEFAULT ((0)),
	[InventoryAction] [varchar](10) NULL,
	[InventoryActionThreshold] [int] NULL,
	[InventoryWarningThreshold] [int] NULL,
	[BackorderDate] [datetime] NULL,
	[ImageName] [varchar](100) NULL,
	[ImageAlt] [varchar](255) NULL,
	[ProductImage] [varchar](100) NULL,
	[ProductAlt] [varchar](255) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_StoreItemAttributeTemp_IsActive]  DEFAULT ((0)),
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_StoreItemAttributeTemp] PRIMARY KEY NONCLUSTERED 
(
	[TempAttributeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreItemAttributeTempGuid]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreItemAttributeTempGuid](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Guid] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_StoreItemAttributeTempGuid_CreateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_StoreItemAttributeTempGuid] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[StoreItemNotify](
	[ItemNotifyId] [int] IDENTITY(1,1) NOT NULL,
	[Email] [varchar](50) NOT NULL,
	[ItemId] [int] NULL,
	[ItemAttributeId] [int] NULL,
	[CreateDate] [datetime] NOT NULL,
	[SendDate] [datetime] NULL,
	[ViewDate] [datetime] NULL,
 CONSTRAINT [PK_StoreItemNotify] PRIMARY KEY CLUSTERED 
(
	[ItemNotifyId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[StoreItemNotify]  WITH CHECK ADD  CONSTRAINT [CK_StoreItemNotify] CHECK  (([ItemId] IS NOT NULL OR [ItemAttributeId] IS NOT NULL))
GO
ALTER TABLE [dbo].[StoreItemNotify] CHECK CONSTRAINT [CK_StoreItemNotify]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[LookupSwatch]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[LookupSwatch](
	[SwatchId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[SKU] [varchar](50) NULL,
	[Price] [float] NULL,
	[Weight] [float] NULL,
	[Image] [varchar](50) NULL,
 CONSTRAINT [PK_Swatch] PRIMARY KEY CLUSTERED 
(
	[SwatchId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreBrand]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreBrand](
	[BrandId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[Image] [varchar](100) NULL,
	[Logo] [varchar](100) NULL,
	[ThumbnailWidth] [int] NULL,
	[ThumbnailHeight] [int] NULL,
	[PageTitle] [varchar](255) NULL,
	[MetaDescription] [varchar](255) NULL,
	[MetaKeywords] [varchar](255) NULL,
	[CustomURL] [varchar](100) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_StoreBrand_IsActive] DEFAULT ((0)),
	[SortOrder] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[DateModified] [datetime] NULL,
	[Description] [varchar](max) NULL,
 CONSTRAINT [PK_StoreBrand] PRIMARY KEY CLUSTERED 
(
	[BrandId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreSequence]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreSequence](
	[Id] [int] IDENTITY(10000000,1) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_StoreSequence] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomText]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomText](
	[TextId] [int] IDENTITY(1,1) NOT NULL,
	[Code] [varchar](50) NOT NULL,
	[Title] [varchar](50) NOT NULL,
	[Value] [varchar](max) NULL,
	[IsHelpTag] [bit] NOT NULL CONSTRAINT [DF_CustomText_IsHelpTag]  DEFAULT ((0)),
 CONSTRAINT [PK_CustomText] PRIMARY KEY CLUSTERED 
(
	[TextId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

INSERT INTO CustomText (Code, Title, Value, IsHelpTag) VALUES ('PackingListFooter','Packing List Footer','',0)
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES
 (86, 'Shipping', 'ShippingCompany', 'IDEV Store Startup','STRING','10',0,0,'Name of the company which is to appear on the Packing List.')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES
 (87, 'Shipping', 'ShippingPhone', '847.699.0300','STRING','10',0,0,'Phone number that is to appear on the shipping packing list.')

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreDepartment]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreDepartment](
	[DepartmentId] [int] IDENTITY(1,1) NOT NULL,
	[Lft] [int] NULL,
	[Rgt] [int] NULL,
	[ParentId] [int] NULL,
	[Name] [varchar](100) NOT NULL,
	[PageTitle] [varchar](255) NULL,
	[MetaDescription] [varchar](255) NULL,
	[MetaKeywords] [varchar](255) NULL,
	[CustomURL] [varchar](100) NULL,
	[IsInactive] [bit] NOT NULL CONSTRAINT [DF_StoreDepartment_IsInactive] DEFAULT ((0)),
	[IsSpecial] [bit] NOT NULL CONSTRAINT [DF_StoreDepartment_IsSpecial] DEFAULT ((0)),
	[ViewImage] [varchar](100) NULL,
	[ViewImageAlt] [varchar](100) NULL,
	[ThumbnailWidth] [int] NULL,
	[ThumbnailHeight] [int] NULL,
	[Description] [varchar](max) NULL,
 CONSTRAINT [PK_StoreDepartment] PRIMARY KEY NONCLUSTERED 
(
	[DepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingList]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingList](
	[ListId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Status] [varchar](50) NOT NULL,
	[Filename] [varchar](255) NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateAdminId] [int] NOT NULL,
	[ModifyDate] [datetime] NOT NULL,
	[ModifyAdminId] [int] NOT NULL,
	[IsPermanent] [bit] NOT NULL,
 CONSTRAINT [PK_MailingList] PRIMARY KEY CLUSTERED 
(
	[ListId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingMember]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingMember](
	[MemberId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[Email] [varchar](255) NOT NULL,
	[MimeType] [varchar](50) NOT NULL,
	[Status] [varchar](50) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[ModifyDate] [datetime] NOT NULL,
	[Unsubscribe] [datetime] NULL,
 CONSTRAINT [PK_MailingMember] PRIMARY KEY NONCLUSTERED 
(
	[MemberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreRelatedItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreRelatedItem](
	[UniqueId] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_StoreRelatedItem] PRIMARY KEY CLUSTERED 
(
	[ParentId] ASC,
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PaymentLog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[PaymentLog](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[OrderNo] [varchar](50) NULL,
	[TransactionNo] [varchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[Result] [int] NOT NULL,
	[Amount] [float] NOT NULL,
	[Description] [varchar](max) NULL,
	[Response] [varchar](max) NULL,
	[VerificationResponse] [varchar](max) NULL,
	[IsHighRisk] [bit] NOT NULL CONSTRAINT [DF_PaymentLog_IsHighRisk]  DEFAULT ((0)),
 CONSTRAINT [PK_PaymentLog] PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BannerGroup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BannerGroup](
	[BannerGroupId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[MinWidth] [int] NOT NULL,
	[MaxWidth] [int] NOT NULL,
 CONSTRAINT [PK_BannerGroup] PRIMARY KEY CLUSTERED 
(
	[BannerGroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingGroup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingGroup](
	[GroupId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](100) NOT NULL,
	[Description] [varchar](500) NULL,
	[StartDate] [datetime] NULL,
	[EndDate] [datetime] NULL,
	[IsPermanent] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[CreateAdminId] [int] NOT NULL,
	[ModifyDate] [datetime] NOT NULL,
	[ModifyAdminId] [int] NOT NULL,
 CONSTRAINT [PK_MailingGroup] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Mailing_Lyris_Member]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Mailing_Lyris_Member](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[List_Id] [int] NOT NULL,
	[Domain] [varchar](250) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[Full_Name] [varchar](100) NULL,
	[Member_Id] [int] NULL,
	[Lyris_Member_Id] [int] NOT NULL,
	[Num_Bounces] [int] NOT NULL,
	[Password] [varchar](50) NULL,
 CONSTRAINT [PK_MAILING_LYRIS_MEMBER] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreItemAttribute]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreItemAttribute](
	[ItemAttributeId] [int] IDENTITY(1,1) NOT NULL,
	[TemplateAttributeId] [int] NOT NULL,
	[ParentAttributeId] [int] NULL,
	[TempAttributeId] [int] NULL,
	[ItemId] [int] NOT NULL,
	[AttributeValue] [varchar](255) NOT NULL,
	[SKU] [varchar](25) NULL,
	[FinalSKU] [varchar](50) NULL,
	[Price] [money] NOT NULL CONSTRAINT [DF_StoreItemAttribute_Price] DEFAULT ((0)),
	[Weight] [float] NOT NULL CONSTRAINT [DF_StoreItemAttribute_Weight]  DEFAULT ((0)),
	[InventoryQty] [int] NOT NULL CONSTRAINT [DF_StoreItemAttribute_InventoryQty]  DEFAULT ((0)),
	[InventoryAction] [varchar](10) NULL,
	[InventoryActionThreshold] [int] NULL,
	[InventoryWarningThreshold] [int] NULL,
	[BackorderDate] [datetime] NULL,
	[ImageName] [varchar](100) NULL,
	[ImageAlt] [varchar](255) NULL,
	[ProductImage] [varchar](100) NULL,
	[ProductAlt] [varchar](255) NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_StoreItemAttribute_IsActive]  DEFAULT ((0)),
	[IsValidated] [bit] NOT NULL CONSTRAINT [DF_StoreItemAttribute_IsValidated]  DEFAULT ((0)),
	[SendNotification] [bit] NOT NULL CONSTRAINT [DF_StoreItemAttribute_SendNotification]  DEFAULT ((0)),
	[SortOrder] [int] NOT NULL CONSTRAINT [DF_StoreItemAttribute_SortOrder] DEFAULT ((0)),
	[ControlCount] int NOT NULL CONSTRAINT [DF_StoreItemAttribute_ControlCount] DEFAULT ((0)),
 CONSTRAINT [PK_StoreItemAttribute] PRIMARY KEY NONCLUSTERED 
(
	[ItemAttributeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

CREATE CLUSTERED INDEX IX_StoreItemAttribute ON dbo.[StoreItemAttribute]
	(
	ItemId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_StoreItemAttribute_1 ON dbo.[StoreItemAttribute]
	(
	FinalSKU
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_StoreItemAttribute_2 ON dbo.[StoreItemAttribute]
	(
	ParentAttributeId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_StoreItemAttribute_3 ON dbo.[StoreItemAttribute]
	(
	TemplateAttributeId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

CREATE NONCLUSTERED INDEX IX_StoreItemAttribute_4 ON dbo.[StoreItemAttribute]
	(
	IsValidated
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreOrderRecipient]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreOrderRecipient](
	[RecipientId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[AddressId] [int] NULL,
	[Label] [varchar](50) NOT NULL,
	[IsCustomer] [bit] NOT NULL CONSTRAINT [DF_StoreOrderRecipient_IsCustomer] DEFAULT ((0)),
	[IsSameAsBilling] [bit] NOT NULL CONSTRAINT [DF_StoreOrderRecipient_IsSameAsBilling] DEFAULT ((0)),
	[FirstName] [varchar](50) NULL,
	[MiddleInitial] [varchar](1) NULL,
	[LastName] [varchar](50) NULL,
	[Company] [varchar](50) NULL,
	[Address1] [varchar](50) NULL,
	[Address2] [varchar](50) NULL,
	[City] [varchar](50) NULL,
	[State] [varchar](2) NULL,
	[Region] [varchar](50) NULL,
	[Country] [varchar](2) NULL,
	[Zip] [varchar](15) NULL,
	[Phone] [varchar](50) NULL,
	[GiftMessage] [varchar](100) NULL,
	[GiftMessageLabel] [varchar](50) NULL,
	[BaseSubtotal] [money] NOT NULL CONSTRAINT [DF_StoreOrderRecipient_BaseSubtotal] DEFAULT ((0)),
	[Discount] [money] NOT NULL CONSTRAINT [DF_StoreOrderRecipient_Discount] DEFAULT ((0)),
	[Subtotal] [money] NOT NULL CONSTRAINT [DF_StoreOrderRecipient_Subtotal] DEFAULT ((0)),
	[GiftWrapping] [money] NOT NULL CONSTRAINT [DF_StoreOrderRecipient_GiftWrapping] DEFAULT ((0)),
	[Shipping] [money] NOT NULL CONSTRAINT [DF_StoreOrderRecipient_Shipping] DEFAULT ((0)),
	[ShippingMethodId] [int] NULL,
	[Tax] [money] NOT NULL CONSTRAINT [DF_StoreOrderRecipient_Tax] DEFAULT ((0)),
	[Total] [money] NOT NULL CONSTRAINT [DF_StoreOrderRecipient_Total] DEFAULT ((0)),
	[Status] [varchar](1) NOT NULL CONSTRAINT [DF_StoreOrderRecipient_Status] DEFAULT ('N'),
	[ShippedDate] [datetime] NULL,
	[TrackingNr] [varchar](25) NULL,
	[IsShippingIndividually] bit NOT NULL DEFAULT 0,
 CONSTRAINT [PK_StoreOrderRecipient] PRIMARY KEY NONCLUSTERED 
(
	[RecipientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
CREATE TABLE [dbo].[GiftMessage](
	[GiftMessageId] [int] IDENTITY(1,1) NOT NULL,
	[GiftMessage] [varchar](100) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[GiftMessageLabel] [varchar](25) NULL,
 CONSTRAINT [PK_GiftMessage] PRIMARY KEY CLUSTERED 
(
	[GiftMessageId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StorePromotionItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StorePromotionItem](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PromotionId] [int] NOT NULL,
	[ItemId] [int] NULL,
	[DepartmentId] [int] NULL,
 CONSTRAINT [PK_StorePromotionItem] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY],
 CONSTRAINT [IX_StorePromotionItem] UNIQUE NONCLUSTERED 
(
	[PromotionId] ASC,
	[ItemId] ASC,
	[DepartmentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Referral]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[Referral](
	[ReferralId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[PromotionId] [int] NULL,
	[Code] [varchar](50) NOT NULL,
 CONSTRAINT [PK_Referral] PRIMARY KEY CLUSTERED 
(
	[ReferralId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreOrder]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreOrder](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[OrderNo] [varchar](50) NULL,
	[OrderCode] [varchar](20) NOT NULL,
	[HowHeardId] [int] NULL,
	[HowHeardName] [varchar](100) NULL,
	[MemberId] [int] NULL,
	[BillingFirstName] [varchar](50) NULL,
	[BillingMiddleInitial] [varchar](1) NULL,
	[BillingLastName] [varchar](50) NULL,
	[BillingCompany] [varchar](50) NULL,
	[BillingAddress1] [varchar](50) NULL,
	[BillingAddress2] [varchar](50) NULL,
	[BillingCity] [varchar](50) NULL,
	[BillingState] [varchar](2) NULL,
	[BillingRegion] [varchar](50) NULL,
	[BillingCountry] [varchar](2) NULL,
	[BillingZip] [varchar](15) NULL,
	[BillingPhone] [varchar](50) NULL,
	[CardNumber] [varchar](100) NULL,
	[CardTypeId] [int] NULL,
	[ExpirationDate] [varchar](100) NULL,
	[CardholderName] [varchar](255) NULL,
	[CIDNumber] [varchar](100) NULL,
	[Email] [varchar](100) NULL,
	[BaseSubtotal] [money] NOT NULL CONSTRAINT [DF_StoreOrder_BaseSubtotal] DEFAULT ((0)),
	[Discount] [money] NOT NULL CONSTRAINT [DF_StoreOrder_Discount] DEFAULT ((0)),
	[Subtotal] [money] NOT NULL CONSTRAINT [DF_StoreOrder_Subtotal] DEFAULT ((0)),
	[Shipping] [money] NOT NULL CONSTRAINT [DF_StoreOrder_Shipping] DEFAULT ((0)),
	[GiftWrapping] [money] NOT NULL CONSTRAINT [DF_StoreOrder_GiftWrapping] DEFAULT ((0)),
	[Tax] [money] NOT NULL CONSTRAINT [DF_StoreOrder_Tax] DEFAULT ((0)),
	[Total] [money] NOT NULL CONSTRAINT [DF_StoreOrder_Total] DEFAULT ((0)),
	[IsFreeShipping] [bit] NOT NULL CONSTRAINT [DF_StoreOrder_IsFreeShipping] DEFAULT ((0)),
	[PaymentReference] [varchar](50) NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_StoreOrder_CreateDate] DEFAULT (getdate()),
	[ProcessDate] [datetime] NULL,
	[ExportDate] [datetime] NULL,
	[ReferralCode] [varchar](50) NULL,
	[RemoteIP] [varchar](20) NOT NULL,
	[PromotionCode] [varchar](50) NULL,
	[Status] [varchar](1) NOT NULL DEFAULT 'N',
	[Guid] [varchar](32) NULL,
	[Comments] [varchar](max) NULL,
 CONSTRAINT [PK_StoreOrder] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO

CREATE NONCLUSTERED INDEX IX_StoreOrder_1 ON dbo.StoreOrder
	(
	ProcessDate
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_StoreOrder_2 ON dbo.StoreOrder
	(
	PromotionCode
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_StoreOrder_3 ON dbo.StoreOrder
	(
	BillingLastName
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_StoreOrder_4 ON dbo.StoreOrder
	(
	BillingState
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_StoreOrder_5 ON dbo.StoreOrder
	(
	Status
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX IX_StoreOrder_6 ON dbo.StoreOrder
	(
	OrderCode
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreCode]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreCode](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_StoreCode] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BannerTracking]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BannerTracking](
	[TrackingId] [int] IDENTITY(1,1) NOT NULL,
	[BannerId] [int] NOT NULL,
	[ImpressionCount] [bigint] NULL,
	[ClickCount] [bigint] NULL,
	[CreateDate] [datetime] NULL,
 CONSTRAINT [PK_BannerTracking] PRIMARY KEY NONCLUSTERED 
(
	[TrackingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BannerBannerGroup]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[BannerBannerGroup](
	[UniqueId] [int] IDENTITY(1,1) NOT NULL,
	[BannerId] [int] NOT NULL,
	[BannerGroupId] [int] NOT NULL,
	[DateFrom] [datetime] NULL,
	[DateTo] [datetime] NULL,
	[Weight] [int] NOT NULL,
 CONSTRAINT [PK_BannerBannerGroup] PRIMARY KEY CLUSTERED 
(
	[UniqueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MemberAddress]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MemberAddress](
	[AddressId] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[AddressType] [varchar](50) NOT NULL,
	[Label] [varchar](50) NOT NULL,
	[Company] [varchar](50) NULL,
	[FirstName] [varchar](50) NOT NULL,
	[MiddleInitial] [varchar](1) NULL,
	[LastName] [varchar](50) NOT NULL,
	[Address1] [varchar](50) NOT NULL,
	[Address2] [varchar](50) NULL,
	[City] [varchar](50) NOT NULL,
	[State] [varchar](2) NULL,
	[Region] [varchar](50) NULL,
	[Zip] [varchar](15) NULL,
	[Country] [varchar](50) NULL,
	[Email] [varchar](100) NULL,
	[Phone] [varchar](50) NULL,
	[IsDefault] [bit] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[ModifyDate] [datetime] NOT NULL,
 CONSTRAINT [PK_MemberAddress] PRIMARY KEY CLUSTERED 
(
	[AddressId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO

CREATE NONCLUSTERED INDEX IX_MemberAddress ON dbo.MemberAddress
	(
	MemberId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_MemberAddress_1 ON dbo.MemberAddress
	(
	AddressType
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingMessageOpen]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingMessageOpen](
	[UniqueId] [int] IDENTITY(1,1) NOT NULL,
	[ListId] [int] NOT NULL,
	[MessageId] [int] NOT NULL,
	[MemberId] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_MailingMessageOpen] PRIMARY KEY CLUSTERED 
(
	[UniqueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MemberReminder]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MemberReminder](
	[ReminderId] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[Name] [varchar](50) NOT NULL,
	[IsRecurrent] [bit] NOT NULL,
	[EventDate] [datetime] NOT NULL,
	[DaysBefore1] [int] NULL,
	[DaysBefore2] [int] NULL,
	[Email] [varchar](100) NOT NULL,
	[Body] [varchar](max) NULL,
 CONSTRAINT [PK_MemberReminder] PRIMARY KEY NONCLUSTERED 
(
	[ReminderId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MemberWishlistItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MemberWishlistItem](
	[WishlistItemId] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[Quantity] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
	[ModifyDate] [datetime] NULL,
	[AttributeString] [varchar](1000) NULL,
 CONSTRAINT [PK_MemberWishlistItem] PRIMARY KEY NONCLUSTERED 
(
	[WishlistItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingMessageSlot]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingMessageSlot](
	[SlotId] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [int] NOT NULL,
	[Headline] [varchar](100) NULL,
	[Slot] [varchar](max) NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_MailingMessageSlot] PRIMARY KEY CLUSTERED 
(
	[SlotId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingLinkHit]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingLinkHit](
	[HitId] [int] IDENTITY(1,1) NOT NULL,
	[LinkId] [int] NOT NULL,
	[MessageId] [int] NOT NULL,
	[MemberId] [int] NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_MailingLinkHit] PRIMARY KEY NONCLUSTERED 
(
	[HitId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingLink]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingLink](
	[LinkId] [int] NOT NULL,
	[MessageId] [int] NOT NULL,
	[Link] [varchar](1000)  NULL,
	[MimeType] [varchar](50) NULL,
 CONSTRAINT [PK_MailingLink] PRIMARY KEY CLUSTERED 
(
	[MessageId] ASC,
	[LinkId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingRecipient]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingRecipient](
	[RecipientId] [int] IDENTITY(1,1) NOT NULL,
	[MessageId] [int] NOT NULL,
	[MemberId] [int] NOT NULL,
	[Email] [varchar](255) NOT NULL,
	[FullName] [varchar](255) NULL,
	[MimeType] [varchar](50) NULL,
 CONSTRAINT [PK_MailingRecipient] PRIMARY KEY NONCLUSTERED 
(
	[RecipientId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingTemplateSlot]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingTemplateSlot](
	[SlotId] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NOT NULL,
	[SlotName] [varchar](50) NOT NULL,
	[ImageName] [varchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_MailingTemplateSlot] PRIMARY KEY CLUSTERED 
(
	[SlotId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MemberReminderLog]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MemberReminderLog](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[ReminderId] [int] NULL,
	[CreateDate] [datetime] NULL,
 CONSTRAINT [PK_MemberReminderLog] PRIMARY KEY NONCLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreItem](
	[ItemId] [int] IDENTITY(1,1) NOT NULL,
	[TemplateId] [int] NOT NULL,
	[DisplayMode] [varchar](20) NULL,
	[BrandId] [int] NULL,
	[SKU] [varchar](50) NULL,
	[ItemName] [varchar](255) NOT NULL,
	[PageTitle] [varchar](500) NULL,
	[MetaDescription] [varchar](500) NULL,
	[MetaKeywords] [varchar](500) NULL,
	[Image] [varchar](100) NULL,
	[ThumbnailWidth] [int] NULL,
	[ThumbnailHeight] [int] NULL,
	[ItemUnit] [varchar](20) NULL,
	[CustomURL] [varchar](100) NULL,
	[Width] [float] NULL,
	[Height] [float] NULL,
	[Thickness] [float] NULL,
	[Weight] [float] NOT NULL,
	[InventoryQty] [int] NOT NULL CONSTRAINT [DF_StoreItem_InventoryQty]  DEFAULT ((0)),
	[InventoryAction] [varchar](10) NOT NULL CONSTRAINT [DF_StoreItem_InventoryAction]  DEFAULT ('OutOfStock'),
	[InventoryActionThreshold] [int] NULL CONSTRAINT [DF_StoreItem_InventoryActionThreshold]  DEFAULT ((0)),
	[InventoryWarningThreshold] [int] NULL,
	[BackorderDate] [datetime] NULL,
	[Price] [money] NOT NULL,
	[SalePrice] [money] NULL,
	[Shipping1] [money] NULL,
	[Shipping2] [money] NULL,
	[CountryUnit] [int] NULL,
	[IsOnSale] [bit] NOT NULL,
	[IsTaxFree] [bit] NOT NULL,
	[IsActive] [bit] NOT NULL,
	[IsValidAttributes] [bit] NOT NULL CONSTRAINT [DF_StoreItem_IsValidAttributes]  DEFAULT ((0)),
	[IsFeatured] [bit] NOT NULL,
	[IsGiftWrap] [bit] NOT NULL,
	[DepartmentCode] [int] NOT NULL CONSTRAINT [DF_StoreItem_DepartmentCode] DEFAULT ((0)),
	[CategoryCode] [int] NOT NULL CONSTRAINT [DF_StoreItem_CategoryCode] DEFAULT ((0)),
	[SubcategoryCode] [int] NOT NULL CONSTRAINT [DF_StoreItem_SubcategoryCode] DEFAULT ((0)),
	[SKUSequence] [float] NOT NULL CONSTRAINT [DF_StoreItem_SKUSequence] DEFAULT ((0)),
	[QuantityIndicator] [varchar](1) NULL,
	[IsNewSKU] [bit] NOT NULL CONSTRAINT [DF_StoreItem_IsNewSKU] DEFAULT ((0)),
	[IsPromote] [bit] NOT NULL CONSTRAINT [DF_StoreItem_IsPromote] DEFAULT ((0)),
	[VendorSKU] [varchar](20) NULL,
	[CreateDate] [datetime] NOT NULL,
	[ModifyDate] [datetime] NULL,
	[ShortDescription] [varchar](max) NULL,
	[LongDescription] [varchar](max) NULL,
	[SendNotification] [bit] NOT NULL CONSTRAINT [DF_StoreItem_SendNotification]  DEFAULT ((0)),
 CONSTRAINT [PK_StoreItem] PRIMARY KEY CLUSTERED 
(
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
ALTER TABLE dbo.StoreItem ADD CONSTRAINT
	CK_StoreItem CHECK (([DisplayMode]='AdminDriven' OR [DisplayMode]='TableLayout' OR [DisplayMode] IS NULL))
GO
ALTER TABLE dbo.StoreItem ADD CONSTRAINT
	CK_StoreItem_1 CHECK (([InventoryAction]='OutOfStock' OR [InventoryAction]='Disable' OR [InventoryAction] = 'Backorder'))
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CustomURLHistory]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[CustomURLHistory](
	[CustomURLHistoryId] [int] IDENTITY(1,1) NOT NULL,
	[OldCustomURL] [varchar](200) NOT NULL,
	[RedirectURL] [varchar](200) NOT NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_CustomURLHistory_CreateDate]  DEFAULT (getdate()),
 CONSTRAINT [PK_CustomURLHistory] PRIMARY KEY CLUSTERED 
(
	[CustomURLHistoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreDepartmentItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreDepartmentItem](
	[DepartmentId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
 CONSTRAINT [PK_StoreDepartmentItem] PRIMARY KEY CLUSTERED 
(
	[DepartmentId] ASC,
	[ItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingGroupList]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingGroupList](
	[ListId] [int] NOT NULL,
	[GroupId] [int] NOT NULL
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MailingListMember]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[MailingListMember](
	[MemberId] [int] NOT NULL,
	[ListId] [int] NOT NULL,
 CONSTRAINT [PK_MailingListMember] PRIMARY KEY CLUSTERED 
(
	[MemberId] ASC,
	[ListId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreItemImage]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreItemImage](
	[ImageId] [int] IDENTITY(1,1) NOT NULL,
	[ItemId] [int] NULL,
	[ImageType] [varchar](50) NULL,
	[Image] [varchar](50) NULL,
	[ImageAltTag] [varchar](100) NULL,
	[SortOrder] [int] NULL,
 CONSTRAINT [PK_StoreItemImage] PRIMARY KEY NONCLUSTERED 
(
	[ImageId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreOrderItemAttribute]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreOrderItemAttribute](
	[UniqueId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[ItemAttributeId] [int] NOT NULL,
	[OrderItemId] [int] NOT NULL,
	[TemplateAttributeId] [int] NOT NULL,
	[ParentAttributeId] [int] NULL,
	[ItemId] [int] NOT NULL,
	[AttributeValue] [varchar](255) NOT NULL,
	[SKU] [varchar](25) NULL,
	[Price] [money] NULL,
	[Weight] [float] NOT NULL CONSTRAINT [DF_StoreOrderItemAttribute_Weight]  DEFAULT ((0)),
	[ImageName] [varchar](100) NULL,
	[ImageAlt] [varchar](255) NULL,
	[ProductImage] [varchar](100) NULL,
	[ProductAlt] [varchar](255) NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_StoreOrderItemAttribute] PRIMARY KEY NONCLUSTERED 
(
	[UniqueId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreOrderItem]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[StoreOrderItem](
	[OrderItemId] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [int] NOT NULL,
	[RecipientId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[DepartmentId] [int] NULL,
	[BrandId] [int] NULL,
	[TemplateId] [int] NULL,
	[SKU] [varchar](50) NOT NULL,
	[ItemName] [varchar](255) NOT NULL,
	[ItemUnit] [varchar](20) NULL,
	[Image] [varchar](50) NULL,
	[Width] [float] NULL,
	[Height] [float] NULL,
	[Thickness] [float] NULL,
	[Weight] [float] NULL,
	[Quantity] [int] NOT NULL CONSTRAINT [DF_StoreCartItem_Quantity] DEFAULT ((0)),
	[GiftQuantity] [int] NOT NULL,
	[ItemPrice] [money] NOT NULL CONSTRAINT [DF_StoreCartItem_Price] DEFAULT ((0)),
	[SalePrice] [money] NULL,
	[Price] [money] NOT NULL,
	[Discount] [money] NOT NULL,
	[Shipping1] [money] NULL,
	[Shipping2] [money] NULL,
	[CountryUnit] [int] NULL,
	[IsOnSale] [bit] NOT NULL CONSTRAINT [DF_StoreCartItem_IsOnSale] DEFAULT ((0)),
	[IsTaxFree] [bit] NOT NULL CONSTRAINT [DF_StoreCartItem_IsTaxFree] DEFAULT ((0)),
	[IsFeatured] [bit] NOT NULL CONSTRAINT [DF_StoreOrderItem_IsFeatured] DEFAULT ((0)),
	[IsGiftWrap] [bit] NOT NULL CONSTRAINT [DF_StoreOrderItem_IsHomePage] DEFAULT ((0)),
	[CustomURL] [varchar](100) NULL,
	[CreateDate] [datetime] NOT NULL CONSTRAINT [DF_StoreCartItem_CreateDate] DEFAULT (getdate()),
	[ModifyDate] [datetime] NULL,
	[AttributeString] [varchar](1000) NULL,
	[Status] varchar(1) NOT NULL DEFAULT 'N',
	TrackingNumber varchar(50)  NULL,
	ShippedDate datetime NULL,	
 CONSTRAINT [PK_StoreOrderItem] PRIMARY KEY NONCLUSTERED 
(
	[OrderItemId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO
SET ANSI_NULLS ON
GO

CREATE NONCLUSTERED INDEX IX_StoreOrderItem_1 ON dbo.StoreOrderItem
	(
	ItemId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_StoreOrderItem_2 ON dbo.StoreOrderItem
	(
	Status
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
CREATE NONCLUSTERED INDEX IX_StoreOrderItem_3 ON dbo.StoreOrderItem
	(
	ModifyDate
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO




/****** Object:  Table [dbo].[ArchiveStoreOrder]    Script Date: 04/17/2008 14:37:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ArchiveStoreOrder](
	[OrderId] [int] NOT NULL,
	[OrderNo] [varchar](50) NULL,
	[HowHeardId] [int] NULL,
	[HowHeardName] [varchar](100) NULL,
	[MemberId] [int] NULL,
	[BillingFirstName] [varchar](50) NULL,
	[BillingMiddleInitial] [varchar](1) NULL,
	[BillingLastName] [varchar](50) NULL,
	[BillingCompany] [varchar](50) NULL,
	[BillingAddress1] [varchar](50) NULL,
	[BillingAddress2] [varchar](50) NULL,
	[BillingCity] [varchar](50) NULL,
	[BillingState] [varchar](2) NULL,
	[BillingRegion] [varchar](50) NULL,
	[BillingCountry] [varchar](2) NULL,
	[BillingZip] [varchar](15) NULL,
	[BillingPhone] [varchar](50) NULL,
	[CardNumber] [varchar](100) NULL,
	[CardTypeId] [int] NULL,
	[ExpirationDate] [varchar](100) NULL,
	[CardholderName] [varchar](255) NULL,
	[CIDNumber] [varchar](100) NULL,
	[Email] [varchar](100) NULL,
	[BaseSubtotal] [money] NOT NULL,
	[Discount] [money] NOT NULL,
	[Subtotal] [money] NOT NULL,
	[Shipping] [money] NOT NULL,
	[GiftWrapping] [money] NOT NULL,
	[Tax] [money] NOT NULL,
	[Total] [money] NOT NULL,
	[IsFreeShipping] [bit] NOT NULL,
	[PaymentReference] [varchar](50) NULL,
	[CreateDate] [datetime] NOT NULL,
	[ProcessDate] [datetime] NULL,
	[ExportDate] [datetime] NULL,
	[ReferralCode] [varchar](50) NULL,
	[RemoteIP] [varchar](20) NOT NULL,
	[PromotionCode] [varchar](50) NULL,
	[Status] [varchar](50) NULL,
	[Guid] [varchar](32) NULL,
	[Comments] [varchar](max) NULL,
 CONSTRAINT [PK_ArchiveStoreOrder] PRIMARY KEY NONCLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ArchiveStoreOrderItem]    Script Date: 04/17/2008 14:37:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ArchiveStoreOrderItem](
	[OrderItemId] [int] NOT NULL,
	[OrderId] [int] NOT NULL,
	[RecipientId] [int] NOT NULL,
	[ItemId] [int] NOT NULL,
	[DepartmentId] [int] NULL,
	[BrandId] [int] NULL,
	[TemplateId] [int] NULL,
	[SKU] [varchar](50) NOT NULL,
	[ItemName] [varchar](255) NOT NULL,
	[ItemUnit] [varchar](20) NULL,
	[Image] [varchar](50) NULL,
	[Width] [float] NULL,
	[Height] [float] NULL,
	[Thickness] [float] NULL,
	[Weight] [float] NULL,
	[Quantity] [int] NOT NULL,
	[GiftQuantity] [int] NOT NULL,
	[ItemPrice] [money] NOT NULL,
	[SalePrice] [money] NULL,
	[Price] [money] NOT NULL,
	[Discount] [money] NOT NULL,
	[Shipping1] [money] NULL,
	[Shipping2] [money] NULL,
	[CountryUnit] [int] NULL,
	[IsOnSale] [bit] NOT NULL,
	[IsTaxFree] [bit] NOT NULL,
	[IsFeatured] [bit] NOT NULL,
	[IsGiftWrap] [bit] NOT NULL,
	[CustomURL] [varchar](100) NULL,
	[CreateDate] [datetime] NOT NULL,
	[ModifyDate] [datetime] NULL,
	[AttributeString] [varchar](1000) NULL,
	[Status] [varchar](1) NOT NULL,
	[TrackingNumber] [varchar](500) NULL,
	[ShippedDate] [datetime] NULL,
 CONSTRAINT [PK_ArchiveStoreOrderItem] PRIMARY KEY NONCLUSTERED 
(
	[OrderItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ArchiveStoreOrderItemAttribute]    Script Date: 04/17/2008 14:37:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ArchiveStoreOrderItemAttribute](
	[UniqueId] [int] NOT NULL,
	[OrderId] [int] NOT NULL,
	[ItemAttributeId] [int] NOT NULL,
	[OrderItemId] [int] NOT NULL,
	[TemplateAttributeId] [int] NOT NULL,
	[ParentAttributeId] [int] NULL,
	[ItemId] [int] NOT NULL,
	[AttributeValue] [varchar](255) NOT NULL,
	[SKU] [varchar](25) NULL,
	[Price] [money] NULL,
	[Weight] [float] NOT NULL,
	[ImageName] [varchar](100) NULL,
	[ImageAlt] [varchar](255) NULL,
	[ProductImage] [varchar](100) NULL,
	[ProductAlt] [varchar](255) NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_ArchiveStoreOrderItemAttribute] PRIMARY KEY NONCLUSTERED 
(
	[UniqueId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ArchiveStoreOrderRecipient]    Script Date: 04/17/2008 14:37:13 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ArchiveStoreOrderRecipient](
	[RecipientId] [int] NOT NULL,
	[OrderId] [int] NOT NULL,
	[AddressId] [int] NULL,
	[Label] [varchar](50) NOT NULL,
	[IsCustomer] [bit] NOT NULL,
	[IsSameAsBilling] [bit] NOT NULL,
	[FirstName] [varchar](50) NULL,
	[MiddleInitial] [varchar](1) NULL,
	[LastName] [varchar](50) NULL,
	[Company] [varchar](50) NULL,
	[Address1] [varchar](50) NULL,
	[Address2] [varchar](50) NULL,
	[City] [varchar](50) NULL,
	[State] [varchar](2) NULL,
	[Region] [varchar](50) NULL,
	[Country] [varchar](2) NULL,
	[Zip] [varchar](15) NULL,
	[Phone] [varchar](50) NULL,
	[GiftMessage] [varchar](100) NULL,
	[GiftMessageLabel] [varchar](50) NULL,
	[BaseSubtotal] [money] NOT NULL,
	[Discount] [money] NOT NULL,
	[Subtotal] [money] NOT NULL,
	[GiftWrapping] [money] NOT NULL,
	[Shipping] [money] NOT NULL,
	[ShippingMethodId] [int] NULL,
	[Tax] [money] NOT NULL,
	[Total] [money] NOT NULL,
	[Status] [varchar](1) NOT NULL,
	[ShippedDate] [datetime] NULL,
	[TrackingNr] [varchar](500) NULL,
	[IsShippingIndividually] [bit] NOT NULL,
 CONSTRAINT [PK_ArchiveStoreOrderRecipient] PRIMARY KEY CLUSTERED 
(
	[RecipientId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[ArchiveStoreOrderItem]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveStoreOrderItem_ArchiveStoreOrder] FOREIGN KEY([OrderId])
REFERENCES [dbo].[ArchiveStoreOrder] ([OrderId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ArchiveStoreOrderItem] CHECK CONSTRAINT [FK_ArchiveStoreOrderItem_ArchiveStoreOrder]
GO
ALTER TABLE [dbo].[ArchiveStoreOrderItemAttribute]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveStoreOrderItemAttribute_ArchiveStoreOrderItem] FOREIGN KEY([OrderItemId])
REFERENCES [dbo].[ArchiveStoreOrderItem] ([OrderItemId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ArchiveStoreOrderItemAttribute] CHECK CONSTRAINT [FK_ArchiveStoreOrderItemAttribute_ArchiveStoreOrderItem]
GO
ALTER TABLE [dbo].[ArchiveStoreOrderRecipient]  WITH CHECK ADD  CONSTRAINT [FK_ArchiveStoreOrderRecipient_ArchiveStoreOrder] FOREIGN KEY([OrderId])
REFERENCES [dbo].[ArchiveStoreOrder] ([OrderId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ArchiveStoreOrderRecipient] CHECK CONSTRAINT [FK_ArchiveStoreOrderRecipient_ArchiveStoreOrder]





SET QUOTED_IDENTIFIER ON
GO
IF NOT EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[StoreSalesView]'))
EXEC dbo.sp_executesql @statement = N'CREATE VIEW [dbo].[StoreSalesView]
AS
SELECT o.OrderId, o.PromotionCode, o.ReferralCode, o.ProcessDate, sor.ShippedDate, ci.OrderItemId, o.OrderNo, ci.ItemId, ci.DepartmentId, 
 CASE WHEN ci.IsOnSale = 1 THEN ci.SalePrice ELSE ci.ItemPrice END AS Price, ci.SKU, ci.Quantity, ci.ItemName, ci.Width, ci.Thickness, ci.Height, 
 ci.Weight, sor.Status, o.HowHeardId
FROM dbo.StoreOrder AS o INNER JOIN
 dbo.StoreOrderItem AS ci ON o.OrderId = ci.OrderId INNER JOIN
 dbo.StoreOrderRecipient AS sor ON o.OrderId = sor.OrderId AND ci.RecipientId = sor.RecipientId
WHERE (o.ProcessDate IS NOT NULL)
' 
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreDepartment_StoreDepartment]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreDepartment]'))
ALTER TABLE [dbo].[StoreDepartment] WITH CHECK ADD CONSTRAINT [FK_StoreDepartment_StoreDepartment] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[StoreDepartment] ([DepartmentId])
GO
ALTER TABLE [dbo].[StoreDepartment] CHECK CONSTRAINT [FK_StoreDepartment_StoreDepartment]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreItemAttribute_StoreItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreItemAttribute]'))
ALTER TABLE [dbo].[StoreItemAttribute]  WITH CHECK ADD  CONSTRAINT [CK_StoreItemAttribute] CHECK  (([InventoryAction]='OutOfStock' OR [InventoryAction]='Disable' OR [InventoryAction]='Backorder' OR [InventoryAction] IS NULL))
GO
ALTER TABLE [dbo].[StoreItemAttribute] WITH CHECK ADD CONSTRAINT [FK_StoreItemAttribute_StoreItem] FOREIGN KEY([ItemId])
REFERENCES [dbo].[StoreItem] ([ItemId])
GO
ALTER TABLE [dbo].[StoreItemAttribute] CHECK CONSTRAINT [FK_StoreItemAttribute_StoreItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreItemAttribute_StoreTemplateAttribute]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreItemAttribute]'))
ALTER TABLE [dbo].[StoreItemAttribute] WITH CHECK ADD CONSTRAINT [FK_StoreItemAttribute_StoreTemplateAttribute] FOREIGN KEY([TemplateAttributeId])
REFERENCES [dbo].[StoreItemTemplateAttribute] ([TemplateAttributeId])
GO
ALTER TABLE [dbo].[StoreItemAttribute] CHECK CONSTRAINT [FK_StoreItemAttribute_StoreTemplateAttribute]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreOrderRecipient_MemberAddress]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreOrderRecipient]'))
ALTER TABLE [dbo].[StoreOrderRecipient] WITH CHECK ADD CONSTRAINT [FK_StoreOrderRecipient_MemberAddress] FOREIGN KEY([AddressId])
REFERENCES [dbo].[MemberAddress] ([AddressId])
GO
ALTER TABLE [dbo].[StoreOrderRecipient] CHECK CONSTRAINT [FK_StoreOrderRecipient_MemberAddress]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreOrderRecipient_StoreOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreOrderRecipient]'))
ALTER TABLE [dbo].[StoreOrderRecipient] WITH CHECK ADD CONSTRAINT [FK_StoreOrderRecipient_StoreOrder] FOREIGN KEY([OrderId])
REFERENCES [dbo].[StoreOrder] ([OrderId])
GO
ALTER TABLE [dbo].[StoreOrderRecipient] CHECK CONSTRAINT [FK_StoreOrderRecipient_StoreOrder]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreOrderRecipient_StoreShippingMethod]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreOrderRecipient]'))
ALTER TABLE [dbo].[StoreOrderRecipient] WITH CHECK ADD CONSTRAINT [FK_StoreOrderRecipient_StoreShippingMethod] FOREIGN KEY([ShippingMethodId])
REFERENCES [dbo].[StoreShippingMethod] ([MethodId])
GO
ALTER TABLE [dbo].[StoreOrderRecipient] CHECK CONSTRAINT [FK_StoreOrderRecipient_StoreShippingMethod]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StorePromotionItem_StoreDepartment]') AND parent_object_id = OBJECT_ID(N'[dbo].[StorePromotionItem]'))
ALTER TABLE [dbo].[StorePromotionItem] WITH CHECK ADD CONSTRAINT [FK_StorePromotionItem_StoreDepartment] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[StoreDepartment] ([DepartmentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StorePromotionItem] CHECK CONSTRAINT [FK_StorePromotionItem_StoreDepartment]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StorePromotionItem_StoreItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[StorePromotionItem]'))
ALTER TABLE [dbo].[StorePromotionItem] WITH CHECK ADD CONSTRAINT [FK_StorePromotionItem_StoreItem] FOREIGN KEY([ItemId])
REFERENCES [dbo].[StoreItem] ([ItemId])
GO
ALTER TABLE [dbo].[StorePromotionItem] CHECK CONSTRAINT [FK_StorePromotionItem_StoreItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StorePromotionItem_StorePromotion]') AND parent_object_id = OBJECT_ID(N'[dbo].[StorePromotionItem]'))
ALTER TABLE [dbo].[StorePromotionItem] WITH CHECK ADD CONSTRAINT [FK_StorePromotionItem_StorePromotion] FOREIGN KEY([PromotionId])
REFERENCES [dbo].[StorePromotion] ([PromotionId])
GO
ALTER TABLE [dbo].[StorePromotionItem] CHECK CONSTRAINT [FK_StorePromotionItem_StorePromotion]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Referral_StorePromotion]') AND parent_object_id = OBJECT_ID(N'[dbo].[Referral]'))
ALTER TABLE [dbo].[Referral] WITH CHECK ADD CONSTRAINT [FK_Referral_StorePromotion] FOREIGN KEY([PromotionId])
REFERENCES [dbo].[StorePromotion] ([PromotionId])
GO
ALTER TABLE [dbo].[Referral] CHECK CONSTRAINT [FK_Referral_StorePromotion]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreOrder_CreditCardType]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreOrder]'))
ALTER TABLE [dbo].[StoreOrder] WITH CHECK ADD CONSTRAINT [FK_StoreOrder_CreditCardType] FOREIGN KEY([CardTypeId])
REFERENCES [dbo].[CreditCardType] ([CardTypeId])
GO
ALTER TABLE [dbo].[StoreOrder] CHECK CONSTRAINT [FK_StoreOrder_CreditCardType]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreOrder_HowHeard]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreOrder]'))
ALTER TABLE [dbo].[StoreOrder] WITH CHECK ADD CONSTRAINT [FK_StoreOrder_HowHeard] FOREIGN KEY([HowHeardId])
REFERENCES [dbo].[HowHeard] ([HowHeardId])
GO
ALTER TABLE [dbo].[StoreOrder] CHECK CONSTRAINT [FK_StoreOrder_HowHeard]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BannerTracking_Banner]') AND parent_object_id = OBJECT_ID(N'[dbo].[BannerTracking]'))
ALTER TABLE [dbo].[BannerTracking] WITH CHECK ADD CONSTRAINT [FK_BannerTracking_Banner] FOREIGN KEY([BannerId])
REFERENCES [dbo].[Banner] ([BannerId])
GO
ALTER TABLE [dbo].[BannerTracking] CHECK CONSTRAINT [FK_BannerTracking_Banner]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BannerBannerGroup_Banner]') AND parent_object_id = OBJECT_ID(N'[dbo].[BannerBannerGroup]'))
ALTER TABLE [dbo].[BannerBannerGroup] WITH CHECK ADD CONSTRAINT [FK_BannerBannerGroup_Banner] FOREIGN KEY([BannerId])
REFERENCES [dbo].[Banner] ([BannerId])
GO
ALTER TABLE [dbo].[BannerBannerGroup] CHECK CONSTRAINT [FK_BannerBannerGroup_Banner]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BannerBannerGroup_BannerGroup]') AND parent_object_id = OBJECT_ID(N'[dbo].[BannerBannerGroup]'))
ALTER TABLE [dbo].[BannerBannerGroup] WITH CHECK ADD CONSTRAINT [FK_BannerBannerGroup_BannerGroup] FOREIGN KEY([BannerGroupId])
REFERENCES [dbo].[BannerGroup] ([BannerGroupId])
GO
ALTER TABLE [dbo].[BannerBannerGroup] CHECK CONSTRAINT [FK_BannerBannerGroup_BannerGroup]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MemberAddress_Member]') AND parent_object_id = OBJECT_ID(N'[dbo].[MemberAddress]'))
ALTER TABLE [dbo].[MemberAddress] WITH CHECK ADD CONSTRAINT [FK_MemberAddress_Member] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Member] ([MemberId])
GO
ALTER TABLE [dbo].[MemberAddress] CHECK CONSTRAINT [FK_MemberAddress_Member]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MemberAddress_MemberAddress]') AND parent_object_id = OBJECT_ID(N'[dbo].[MemberAddress]'))
ALTER TABLE [dbo].[MemberAddress] WITH CHECK ADD CONSTRAINT [FK_MemberAddress_MemberAddress] FOREIGN KEY([AddressId])
REFERENCES [dbo].[MemberAddress] ([AddressId])
GO
ALTER TABLE [dbo].[MemberAddress] CHECK CONSTRAINT [FK_MemberAddress_MemberAddress]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingMessageOpen_MailingList]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingMessageOpen]'))
ALTER TABLE [dbo].[MailingMessageOpen] WITH CHECK ADD CONSTRAINT [FK_MailingMessageOpen_MailingList] FOREIGN KEY([ListId])
REFERENCES [dbo].[MailingList] ([ListId])
GO
ALTER TABLE [dbo].[MailingMessageOpen] CHECK CONSTRAINT [FK_MailingMessageOpen_MailingList]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingMessageOpen_MailingMessage]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingMessageOpen]'))
ALTER TABLE [dbo].[MailingMessageOpen] WITH CHECK ADD CONSTRAINT [FK_MailingMessageOpen_MailingMessage] FOREIGN KEY([MessageId])
REFERENCES [dbo].[MailingMessage] ([MessageId])
GO
ALTER TABLE [dbo].[MailingMessageOpen] CHECK CONSTRAINT [FK_MailingMessageOpen_MailingMessage]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingMessageOpen_Member]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingMessageOpen]'))
ALTER TABLE [dbo].[MailingMessageOpen] WITH CHECK ADD CONSTRAINT [FK_MailingMessageOpen_Member] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Member] ([MemberId])
GO
ALTER TABLE [dbo].[MailingMessageOpen] CHECK CONSTRAINT [FK_MailingMessageOpen_Member]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MemberReminder_Member]') AND parent_object_id = OBJECT_ID(N'[dbo].[MemberReminder]'))
ALTER TABLE [dbo].[MemberReminder] WITH CHECK ADD CONSTRAINT [FK_MemberReminder_Member] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Member] ([MemberId])
GO
ALTER TABLE [dbo].[MemberReminder] CHECK CONSTRAINT [FK_MemberReminder_Member]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MemberReminder_MemberReminder]') AND parent_object_id = OBJECT_ID(N'[dbo].[MemberReminder]'))
ALTER TABLE [dbo].[MemberReminder] WITH CHECK ADD CONSTRAINT [FK_MemberReminder_MemberReminder] FOREIGN KEY([ReminderId])
REFERENCES [dbo].[MemberReminder] ([ReminderId])
GO
ALTER TABLE [dbo].[MemberReminder] CHECK CONSTRAINT [FK_MemberReminder_MemberReminder]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MemberWishlistItem_Member]') AND parent_object_id = OBJECT_ID(N'[dbo].[MemberWishlistItem]'))
ALTER TABLE [dbo].[MemberWishlistItem] WITH CHECK ADD CONSTRAINT [FK_MemberWishlistItem_Member] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Member] ([MemberId])
GO
ALTER TABLE [dbo].[MemberWishlistItem] CHECK CONSTRAINT [FK_MemberWishlistItem_Member]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MemberWishlistItem_MemberWishlistItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[MemberWishlistItem]'))
ALTER TABLE [dbo].[MemberWishlistItem] WITH CHECK ADD CONSTRAINT [FK_MemberWishlistItem_MemberWishlistItem] FOREIGN KEY([WishlistItemId])
REFERENCES [dbo].[MemberWishlistItem] ([WishlistItemId])
GO
ALTER TABLE [dbo].[MemberWishlistItem] CHECK CONSTRAINT [FK_MemberWishlistItem_MemberWishlistItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MemberWishlistItem_StoreItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[MemberWishlistItem]'))
ALTER TABLE [dbo].[MemberWishlistItem] WITH CHECK ADD CONSTRAINT [FK_MemberWishlistItem_StoreItem] FOREIGN KEY([ItemId])
REFERENCES [dbo].[StoreItem] ([ItemId])
GO
ALTER TABLE [dbo].[MemberWishlistItem] CHECK CONSTRAINT [FK_MemberWishlistItem_StoreItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingMessageSlot_MailingMessage]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingMessageSlot]'))
ALTER TABLE [dbo].[MailingMessageSlot] WITH CHECK ADD CONSTRAINT [FK_MailingMessageSlot_MailingMessage] FOREIGN KEY([MessageId])
REFERENCES [dbo].[MailingMessage] ([MessageId])
GO
ALTER TABLE [dbo].[MailingMessageSlot] CHECK CONSTRAINT [FK_MailingMessageSlot_MailingMessage]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingLinkHit_MailingLink]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingLinkHit]'))
ALTER TABLE [dbo].[MailingLinkHit]  WITH CHECK ADD  CONSTRAINT [FK_MailingLinkHit_MailingLink] FOREIGN KEY([MessageId], [LinkId])
REFERENCES [dbo].[MailingLink] ([MessageId], [LinkId])
GO
ALTER TABLE [dbo].[MailingLinkHit] CHECK CONSTRAINT [FK_MailingLinkHit_MailingLink]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingLinkHit_MailingMember]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingLinkHit]'))
ALTER TABLE [dbo].[MailingLinkHit] WITH CHECK ADD CONSTRAINT [FK_MailingLinkHit_MailingMember] FOREIGN KEY([MemberId])
REFERENCES [dbo].[MailingMember] ([MemberId])
GO
ALTER TABLE [dbo].[MailingLinkHit] CHECK CONSTRAINT [FK_MailingLinkHit_MailingMember]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingLinkHit_MailingMessage]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingLinkHit]'))
ALTER TABLE [dbo].[MailingLinkHit] WITH CHECK ADD CONSTRAINT [FK_MailingLinkHit_MailingMessage] FOREIGN KEY([MessageId])
REFERENCES [dbo].[MailingMessage] ([MessageId])
GO
ALTER TABLE [dbo].[MailingLinkHit] CHECK CONSTRAINT [FK_MailingLinkHit_MailingMessage]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingLink_MailingMessage]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingLink]'))
ALTER TABLE [dbo].[MailingLink] WITH CHECK ADD CONSTRAINT [FK_MailingLink_MailingMessage] FOREIGN KEY([MessageId])
REFERENCES [dbo].[MailingMessage] ([MessageId])
GO
ALTER TABLE [dbo].[MailingLink] CHECK CONSTRAINT [FK_MailingLink_MailingMessage]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingRecipient_MailingMember]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingRecipient]'))
ALTER TABLE [dbo].[MailingRecipient] WITH CHECK ADD CONSTRAINT [FK_MailingRecipient_MailingMember] FOREIGN KEY([MemberId])
REFERENCES [dbo].[MailingMember] ([MemberId])
GO
ALTER TABLE [dbo].[MailingRecipient] CHECK CONSTRAINT [FK_MailingRecipient_MailingMember]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingRecipient_MailingMessage]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingRecipient]'))
ALTER TABLE [dbo].[MailingRecipient] WITH CHECK ADD CONSTRAINT [FK_MailingRecipient_MailingMessage] FOREIGN KEY([MessageId])
REFERENCES [dbo].[MailingMessage] ([MessageId])
GO
ALTER TABLE [dbo].[MailingRecipient] CHECK CONSTRAINT [FK_MailingRecipient_MailingMessage]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingTemplateSlot_MailingTemplate]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingTemplateSlot]'))
ALTER TABLE [dbo].[MailingTemplateSlot] WITH CHECK ADD CONSTRAINT [FK_MailingTemplateSlot_MailingTemplate] FOREIGN KEY([TemplateId])
REFERENCES [dbo].[MailingTemplate] ([TemplateId])
GO
ALTER TABLE [dbo].[MailingTemplateSlot] CHECK CONSTRAINT [FK_MailingTemplateSlot_MailingTemplate]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MemberReminderLog_MemberReminder]') AND parent_object_id = OBJECT_ID(N'[dbo].[MemberReminderLog]'))
ALTER TABLE [dbo].[MemberReminderLog] WITH CHECK ADD CONSTRAINT [FK_MemberReminderLog_MemberReminder] FOREIGN KEY([ReminderId])
REFERENCES [dbo].[MemberReminder] ([ReminderId])
GO
ALTER TABLE [dbo].[MemberReminderLog] CHECK CONSTRAINT [FK_MemberReminderLog_MemberReminder]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreItem_StoreBrand]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreItem]'))
ALTER TABLE [dbo].[StoreItem] WITH CHECK ADD CONSTRAINT [FK_StoreItem_StoreBrand] FOREIGN KEY([BrandId])
REFERENCES [dbo].[StoreBrand] ([BrandId])
GO
ALTER TABLE [dbo].[StoreItem] CHECK CONSTRAINT [FK_StoreItem_StoreBrand]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreItem_StoreItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreItem]'))
ALTER TABLE [dbo].[StoreItem] WITH CHECK ADD CONSTRAINT [FK_StoreItem_StoreItem] FOREIGN KEY([ItemId])
REFERENCES [dbo].[StoreItem] ([ItemId])
GO
ALTER TABLE [dbo].[StoreItem] CHECK CONSTRAINT [FK_StoreItem_StoreItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreDepartmentItem_StoreDepartment]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreDepartmentItem]'))
ALTER TABLE [dbo].[StoreDepartmentItem] WITH CHECK ADD CONSTRAINT [FK_StoreDepartmentItem_StoreDepartment] FOREIGN KEY([DepartmentId])
REFERENCES [dbo].[StoreDepartment] ([DepartmentId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[StoreDepartmentItem] CHECK CONSTRAINT [FK_StoreDepartmentItem_StoreDepartment]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreDepartmentItem_StoreItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreDepartmentItem]'))
ALTER TABLE [dbo].[StoreDepartmentItem] WITH CHECK ADD CONSTRAINT [FK_StoreDepartmentItem_StoreItem] FOREIGN KEY([ItemId])
REFERENCES [dbo].[StoreItem] ([ItemId])
GO
ALTER TABLE [dbo].[StoreDepartmentItem] CHECK CONSTRAINT [FK_StoreDepartmentItem_StoreItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingGroupList_MailingGroup]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingGroupList]'))
ALTER TABLE [dbo].[MailingGroupList] WITH CHECK ADD CONSTRAINT [FK_MailingGroupList_MailingGroup] FOREIGN KEY([GroupId])
REFERENCES [dbo].[MailingGroup] ([GroupId])
GO
ALTER TABLE [dbo].[MailingGroupList] CHECK CONSTRAINT [FK_MailingGroupList_MailingGroup]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingGroupList_MailingList]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingGroupList]'))
ALTER TABLE [dbo].[MailingGroupList] WITH CHECK ADD CONSTRAINT [FK_MailingGroupList_MailingList] FOREIGN KEY([ListId])
REFERENCES [dbo].[MailingList] ([ListId])
GO
ALTER TABLE [dbo].[MailingGroupList] CHECK CONSTRAINT [FK_MailingGroupList_MailingList]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingListMember_MailingList]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingListMember]'))
ALTER TABLE [dbo].[MailingListMember] WITH CHECK ADD CONSTRAINT [FK_MailingListMember_MailingList] FOREIGN KEY([ListId])
REFERENCES [dbo].[MailingList] ([ListId])
GO
ALTER TABLE [dbo].[MailingListMember] CHECK CONSTRAINT [FK_MailingListMember_MailingList]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_MailingListMember_MailingMember]') AND parent_object_id = OBJECT_ID(N'[dbo].[MailingListMember]'))
ALTER TABLE [dbo].[MailingListMember] WITH CHECK ADD CONSTRAINT [FK_MailingListMember_MailingMember] FOREIGN KEY([MemberId])
REFERENCES [dbo].[MailingMember] ([MemberId])
GO
ALTER TABLE [dbo].[MailingListMember] CHECK CONSTRAINT [FK_MailingListMember_MailingMember]
GO
CREATE NONCLUSTERED INDEX [IX_MailingListMember] ON [dbo].[MailingListMember]
(
	[ListId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreItemImage_StoreItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreItemImage]'))
ALTER TABLE [dbo].[StoreItemImage] WITH CHECK ADD CONSTRAINT [FK_StoreItemImage_StoreItem] FOREIGN KEY([ItemId])
REFERENCES [dbo].[StoreItem] ([ItemId])
GO
ALTER TABLE [dbo].[StoreItemImage] CHECK CONSTRAINT [FK_StoreItemImage_StoreItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreOrderItemAttribute_StoreOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreOrderItemAttribute]'))
ALTER TABLE [dbo].[StoreOrderItemAttribute] WITH CHECK ADD CONSTRAINT [FK_StoreOrderItemAttribute_StoreOrder] FOREIGN KEY([OrderId])
REFERENCES [dbo].[StoreOrder] ([OrderId])
GO
ALTER TABLE [dbo].[StoreOrderItemAttribute] CHECK CONSTRAINT [FK_StoreOrderItemAttribute_StoreOrder]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreOrderItemAttribute_StoreOrderItem]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreOrderItemAttribute]'))
ALTER TABLE [dbo].[StoreOrderItemAttribute] WITH CHECK ADD CONSTRAINT [FK_StoreOrderItemAttribute_StoreOrderItem] FOREIGN KEY([OrderItemId])
REFERENCES [dbo].[StoreOrderItem] ([OrderItemId])
GO
ALTER TABLE [dbo].[StoreOrderItemAttribute] CHECK CONSTRAINT [FK_StoreOrderItemAttribute_StoreOrderItem]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreCartItem_StoreOrder]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreOrderItem]'))
ALTER TABLE [dbo].[StoreOrderItem] WITH CHECK ADD CONSTRAINT [FK_StoreCartItem_StoreOrder] FOREIGN KEY([OrderId])
REFERENCES [dbo].[StoreOrder] ([OrderId])
GO
ALTER TABLE [dbo].[StoreOrderItem] CHECK CONSTRAINT [FK_StoreCartItem_StoreOrder]
GO
IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_StoreCartItem_StoreOrderRecipient]') AND parent_object_id = OBJECT_ID(N'[dbo].[StoreOrderItem]'))
ALTER TABLE [dbo].[StoreOrderItem] WITH CHECK ADD CONSTRAINT [FK_StoreCartItem_StoreOrderRecipient] FOREIGN KEY([RecipientId])
REFERENCES [dbo].[StoreOrderRecipient] ([RecipientId])
GO
ALTER TABLE [dbo].[StoreOrderItem] CHECK CONSTRAINT [FK_StoreCartItem_StoreOrderRecipient]



/****** Object:  Table [dbo].[Faq]    Script Date: 04/16/2008 20:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Faq](
	[FaqId] [int] IDENTITY(1,1) NOT NULL,
	[FaqCategoryId] [int] NOT NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_Faq_IsActive]  DEFAULT ((1)),
	[SortOrder] [int] NOT NULL,
	[Question] [varchar](MAX) NOT NULL,
	[Answer] [varchar](MAX) NULL,
	[Email] [varchar](100) NULL,
 CONSTRAINT [PK_Faq] PRIMARY KEY CLUSTERED 
(
	[FaqId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[FaqCategory]    Script Date: 04/16/2008 20:00:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[FaqCategory](
	[FaqCategoryId] [int] IDENTITY(1,1) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[IsActive] [bit] NOT NULL CONSTRAINT [DF_FaqCategory_IsActive]  DEFAULT ((1)),
	[CategoryName] [varchar](400) NOT NULL,
	[AdminId] int NULL,
 CONSTRAINT [PK_FaqCategory] PRIMARY KEY CLUSTERED 
(
	[FaqCategoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[Faq]  WITH CHECK ADD  CONSTRAINT [FK_Faq_FaqCategory] FOREIGN KEY([FaqCategoryId])
REFERENCES [dbo].[FaqCategory] ([FaqCategoryId])
GO
ALTER TABLE [dbo].[Faq] CHECK CONSTRAINT [FK_Faq_FaqCategory]

ALTER TABLE dbo.FaqCategory ADD CONSTRAINT
	FK_FaqCategory_Admin FOREIGN KEY
	(
	AdminId
	) REFERENCES dbo.Admin
	(
	AdminId
	) ON UPDATE  SET NULL 
	 ON DELETE  CASCADE 
	
GO
ALTER TABLE dbo.FaqCategory
	NOCHECK CONSTRAINT FK_FaqCategory_Admin
GO


/****** Object:  Table [dbo].[ContactUs]    Script Date: 04/17/2008 08:24:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ContactUs](
	[ContactUsId] [int] IDENTITY(1,1) NOT NULL,
	[FullName] [varchar](50) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[OrderNumber] [varchar](50) NULL,
	[Phone] [varchar](50) NULL,
	[HowHeardId] int NULL,
	[HowHeardName] varchar(50) NULL,
	[QuestionId] [int] NOT NULL,
	[YourMessage] [varchar](max) NOT NULL,
	[CreateDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ContactUs] PRIMARY KEY CLUSTERED 
(
	[ContactUsId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ContactUsQuestion]    Script Date: 04/17/2008 08:24:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ContactUsQuestion](
	[QuestionId] [int] IDENTITY(1,1) NOT NULL,
	[Question] [varchar](50) NOT NULL,
	[EmailAddress] [varchar](100) NULL,
	[SortOrder] [int] NOT NULL,
 CONSTRAINT [PK_ContactUsQuestion] PRIMARY KEY CLUSTERED 
(
	[QuestionId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ContactUsReply]    Script Date: 04/17/2008 08:24:00 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ContactUsReply](
	[ReplyId] [int] IDENTITY(1,1) NOT NULL,
	[ContactUsId] [int] NOT NULL,
	[FullName] [varchar](50) NOT NULL,
	[Email] [varchar](100) NOT NULL,
	[Subject] [varchar](255) NOT NULL,
	[Message] [varchar](max) NOT NULL,
	[CreateDate] datetime NOT NULL,
	[AdminId] int NOT NULL,
 CONSTRAINT [PK_ContactUsReply] PRIMARY KEY NONCLUSTERED 
(
	[ReplyId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[ContactUsReply]  WITH CHECK ADD  CONSTRAINT [FK_ContactUsReply_ContactUs1] FOREIGN KEY([ContactUsId])
REFERENCES [dbo].[ContactUs] ([ContactUsId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ContactUsReply] CHECK CONSTRAINT [FK_ContactUsReply_ContactUs1]




CREATE TABLE dbo.FaqReply
	(
	ReplyId int NOT NULL IDENTITY (1, 1),
	FaqId int NOT NULL,
	FullName varchar(50) NOT NULL,
	Email varchar(100) NOT NULL,
	Subject varchar(255) NOT NULL,
	Message varchar(MAX) NOT NULL,
	CreateDate datetime NOT NULL,
	AdminId int NOT NULL
	)  ON [PRIMARY]
	 TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE dbo.FaqReply ADD CONSTRAINT
	PK_FaqReply PRIMARY KEY NONCLUSTERED 
	(
	ReplyId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

GO
CREATE CLUSTERED INDEX IX_FaqReply ON dbo.FaqReply
	(
	FaqId
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE dbo.FaqReply ADD CONSTRAINT
	FK_FaqReply_Faq FOREIGN KEY
	(
	FaqId
	) REFERENCES dbo.Faq
	(
	FaqId
	) ON UPDATE  NO ACTION 
	 ON DELETE  CASCADE 
	
GO





INSERT INTO LookupSwatch (Name,SKU,Price,Weight,Image) VALUES ('Red','RD',0,0,'red.jpg')
INSERT INTO LookupSwatch (Name,SKU,Price,Weight,Image) VALUES ('Black','BK',0,0,'black.jpg')
INSERT INTO LookupSwatch (Name,SKU,Price,Weight,Image) VALUES ('Blue','BL',0,0,'blue.jpg')
INSERT INTO LookupSwatch (Name,SKU,Price,Weight,Image) VALUES ('Green','GR',0,0,NULL)

INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES ( 1,'AL','Alabama', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES ( 2,'AK','Alaska', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES ( 3,'AZ','Arizona', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES ( 4,'AR','Arkansas', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES ( 5,'CA','California', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES ( 6,'CO','Colorado', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES ( 7,'CT','Connecticut', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES ( 8,'DE','Delaware', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES ( 9,'DC','District of Columbia', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (10,'FL','Florida', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (11,'GA','Georgia', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (12,'HI','Hawaii', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (13,'ID','Idaho', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (14,'IL','Illinois', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (15,'IN','Indiana', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (16,'IA','Iowa', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (17,'KS','Kansas', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (18,'KY','Kentucky', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (19,'LA','Louisiana', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (20,'ME','Maine', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (21,'MD','Maryland', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (22,'MA','Massachusetts', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (23,'MI','Michigan', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (24,'MN','Minnesota', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (25,'MS','Mississippi', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (26,'MO','Missouri', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (27,'MT','Montana', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (28,'NE','Nebraska', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (29,'NV','Nevada', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (30,'NH','New Hampshire', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (31,'NJ','New Jersey', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (32,'NM','New Mexico', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (33,'NY','New York', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (34,'NC','North Carolina', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (35,'ND','North Dakota', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (36,'OH','Ohio', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (37,'OK','Oklahoma', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (38,'OR','Oregon', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (39,'PA','Pennsylvania', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (40,'RI','Rhode Island', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (41,'SC','South Carolina', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (42,'SD','South Dakota', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (43,'TN','Tennessee', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (44,'TX','Texas', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (45,'UT','Utah', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (46,'VT','Vermont', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (47,'VA','Virginia', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (48,'WA','Washington', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (49,'WV','West Virginia', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (50,'WI','Wisconsin', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (51,'WY','Wyoming', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (52,'AA','Armed Forces-AA', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (53,'AE','Armed Forces-AE', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (54,'AP','Armed Forces-AP', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (55,'GU','Guam', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (56,'PR','Puerto Rico', 0);
INSERT INTO State (StateId, StateCode, StateName, TaxRate) VALUES (57,'VI','Virgin Islands', 0);

INSERT INTO Country (CountryCode, CountryName) VALUES ('AL', 'Albania');
INSERT INTO Country (CountryCode, CountryName) VALUES ('DZ', 'Algeria');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AS', 'American Samoa');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AD', 'Andorra');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AO', 'Angola');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AI', 'Anguilla');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AG', 'Antigua');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AR', 'Argentina');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AM', 'Armenia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AW', 'Aruba');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AU', 'Australia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AT', 'Austria');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AZ', 'Azerbaijan');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BS', 'Bahamas');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BH', 'Bahrain');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BD', 'Bangladesh');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BB', 'Barbados');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BY', 'Belarus');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BE', 'Belgium');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BZ', 'Belize');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BJ', 'Benin');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BM', 'Bermuda');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BT', 'Bhutan');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BO', 'Bolivia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BW', 'Botswana');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BR', 'Brazil');
INSERT INTO Country (CountryCode, CountryName) VALUES ('VG', 'British Virgin Is.');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BN', 'Brunei');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BG', 'Bulgaria');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BF', 'Burkino Faso');
INSERT INTO Country (CountryCode, CountryName) VALUES ('BI', 'Burundi');
INSERT INTO Country (CountryCode, CountryName) VALUES ('KH', 'Cambodia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CM', 'Cameroon');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CA', 'Canada');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CV', 'Cape Verde');
INSERT INTO Country (CountryCode, CountryName) VALUES ('KY', 'Cayman Islands');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CF', 'Central African');
INSERT INTO Country (CountryCode, CountryName) VALUES ('TD', 'Chad');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CL', 'Chile');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CN', 'China');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CO', 'Colombia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CG', 'Congo');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CD', 'Congo, The Republic of');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CK', 'Cook Islands');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CR', 'Costa Rica');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CI', 'Cote D''Ivoire');
INSERT INTO Country (CountryCode, CountryName) VALUES ('HR', 'Croatia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CY', 'Cyprus');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CZ', 'Czech Republic');
INSERT INTO Country (CountryCode, CountryName) VALUES ('DK', 'Denmark');
INSERT INTO Country (CountryCode, CountryName) VALUES ('DJ', 'Djibouti');
INSERT INTO Country (CountryCode, CountryName) VALUES ('DM', 'Dominica');
INSERT INTO Country (CountryCode, CountryName) VALUES ('DO', 'Dominican Republic');
INSERT INTO Country (CountryCode, CountryName) VALUES ('EC', 'Ecuador');
INSERT INTO Country (CountryCode, CountryName) VALUES ('EG', 'Egypt');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SV', 'El Salvador');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GQ', 'Equatorial Guinea');
INSERT INTO Country (CountryCode, CountryName) VALUES ('ER', 'Eritrea');
INSERT INTO Country (CountryCode, CountryName) VALUES ('EE', 'Estonia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('ET', 'Ethiopia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('FO', 'Faeroe Islands');
INSERT INTO Country (CountryCode, CountryName) VALUES ('FJ', 'Fiji');
INSERT INTO Country (CountryCode, CountryName) VALUES ('FI', 'Finland');
INSERT INTO Country (CountryCode, CountryName) VALUES ('FR', 'France');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GF', 'French Guiana');
INSERT INTO Country (CountryCode, CountryName) VALUES ('PF', 'French Polynesia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GA', 'Gabon');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GM', 'Gambia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GE', 'Georgia, Republic of');
INSERT INTO Country (CountryCode, CountryName) VALUES ('DE', 'Germany');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GH', 'Ghana');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GI', 'Gibraltar');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GR', 'Greece');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GL', 'Greenland');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GD', 'Grenada');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GP', 'Guadeloupe');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GU', 'Guam');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GT', 'Guatemala');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GN', 'Guinea');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GW', 'Guinea-Bissau');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GY', 'Guyana');
INSERT INTO Country (CountryCode, CountryName) VALUES ('HT', 'Haiti');
INSERT INTO Country (CountryCode, CountryName) VALUES ('HN', 'Honduras');
INSERT INTO Country (CountryCode, CountryName) VALUES ('HK', 'Hong Kong');
INSERT INTO Country (CountryCode, CountryName) VALUES ('HU', 'Hungary');
INSERT INTO Country (CountryCode, CountryName) VALUES ('IS', 'Iceland');
INSERT INTO Country (CountryCode, CountryName) VALUES ('IN', 'India');
INSERT INTO Country (CountryCode, CountryName) VALUES ('ID', 'Indonesia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('IE', 'Ireland');
INSERT INTO Country (CountryCode, CountryName) VALUES ('IL', 'Israel');
INSERT INTO Country (CountryCode, CountryName) VALUES ('IT', 'Italy');
INSERT INTO Country (CountryCode, CountryName) VALUES ('JM', 'Jamaica');
INSERT INTO Country (CountryCode, CountryName) VALUES ('JP', 'Japan');
INSERT INTO Country (CountryCode, CountryName) VALUES ('JO', 'Jordan');
INSERT INTO Country (CountryCode, CountryName) VALUES ('KZ', 'Kazakhstan');
INSERT INTO Country (CountryCode, CountryName) VALUES ('KE', 'Kenya');
INSERT INTO Country (CountryCode, CountryName) VALUES ('KW', 'Kuwait');
INSERT INTO Country (CountryCode, CountryName) VALUES ('KG', 'Kyrgyzstan');
INSERT INTO Country (CountryCode, CountryName) VALUES ('LV', 'Latvia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('LB', 'Lebanon');
INSERT INTO Country (CountryCode, CountryName) VALUES ('LS', 'Lesotho');
INSERT INTO Country (CountryCode, CountryName) VALUES ('LI', 'Liechtenstein');
INSERT INTO Country (CountryCode, CountryName) VALUES ('LT', 'Lithuania');
INSERT INTO Country (CountryCode, CountryName) VALUES ('LU', 'Luxembourg');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MO', 'Macau');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MK', 'Macedonia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MG', 'Madagascar');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MW', 'Malawi');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MY', 'Malaysia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MV', 'Maldives');
INSERT INTO Country (CountryCode, CountryName) VALUES ('ML', 'Mali');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MT', 'Malta');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MH', 'Marshall Islands');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MQ', 'Martinique');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MR', 'Mauritania');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MU', 'Mauritius');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MX', 'Mexico');
INSERT INTO Country (CountryCode, CountryName) VALUES ('FM', 'Micronesia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MD', 'Moldova');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MC', 'Monaco');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MN', 'Mongolia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MS', 'Montserrat');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MA', 'Morocco');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MZ', 'Mozambique');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MM', 'Myanmar');
INSERT INTO Country (CountryCode, CountryName) VALUES ('NA', 'Namibia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('NP', 'Nepal');
INSERT INTO Country (CountryCode, CountryName) VALUES ('NL', 'Netherlands');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AN', 'Netherlands Antilles');
INSERT INTO Country (CountryCode, CountryName) VALUES ('NC', 'New Caledonia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('NZ', 'New Zealand');
INSERT INTO Country (CountryCode, CountryName) VALUES ('NI', 'Nicaragua');
INSERT INTO Country (CountryCode, CountryName) VALUES ('NE', 'Niger');
INSERT INTO Country (CountryCode, CountryName) VALUES ('NG', 'Nigeria');
INSERT INTO Country (CountryCode, CountryName) VALUES ('NO', 'Norway');
INSERT INTO Country (CountryCode, CountryName) VALUES ('OM', 'Oman');
INSERT INTO Country (CountryCode, CountryName) VALUES ('PK', 'Pakistan');
INSERT INTO Country (CountryCode, CountryName) VALUES ('PW', 'Palau');
INSERT INTO Country (CountryCode, CountryName) VALUES ('PA', 'Panama');
INSERT INTO Country (CountryCode, CountryName) VALUES ('PG', 'Papua New Guinea');
INSERT INTO Country (CountryCode, CountryName) VALUES ('PY', 'Paraguay');
INSERT INTO Country (CountryCode, CountryName) VALUES ('PE', 'Peru');
INSERT INTO Country (CountryCode, CountryName) VALUES ('PH', 'Philippines');
INSERT INTO Country (CountryCode, CountryName) VALUES ('PL', 'Poland');
INSERT INTO Country (CountryCode, CountryName) VALUES ('PT', 'Portugal');
INSERT INTO Country (CountryCode, CountryName) VALUES ('PR', 'Puerto Rico');
INSERT INTO Country (CountryCode, CountryName) VALUES ('QA', 'Qatar');
INSERT INTO Country (CountryCode, CountryName) VALUES ('RE', 'Reunion Island');
INSERT INTO Country (CountryCode, CountryName) VALUES ('RO', 'Romania');
INSERT INTO Country (CountryCode, CountryName) VALUES ('RU', 'Russia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('RW', 'Rwanda');
INSERT INTO Country (CountryCode, CountryName) VALUES ('MP', 'Saipan');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SM', 'San Marino');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SA', 'Saudi Arabia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SN', 'Senegal');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SC', 'Seychelles');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SL', 'Sierra Leone');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SG', 'Singapore');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SK', 'Slovak Republic');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SI', 'Slovenia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('ZA', 'South Africa');
INSERT INTO Country (CountryCode, CountryName) VALUES ('KR', 'South Korea');
INSERT INTO Country (CountryCode, CountryName) VALUES ('ES', 'Spain');
INSERT INTO Country (CountryCode, CountryName) VALUES ('LK', 'Sri Lanka');
INSERT INTO Country (CountryCode, CountryName) VALUES ('KN', 'St. Kitts &amp; Nevis');
INSERT INTO Country (CountryCode, CountryName) VALUES ('LC', 'St. Lucia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('VC', 'St. Vincent');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SR', 'Suriname');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SZ', 'Swaziland');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SE', 'Sweden');
INSERT INTO Country (CountryCode, CountryName) VALUES ('CH', 'Switzerland');
INSERT INTO Country (CountryCode, CountryName) VALUES ('SY', 'Syria');
INSERT INTO Country (CountryCode, CountryName) VALUES ('TW', 'Taiwan');
INSERT INTO Country (CountryCode, CountryName) VALUES ('TZ', 'Tanzania');
INSERT INTO Country (CountryCode, CountryName) VALUES ('TH', 'Thailand');
INSERT INTO Country (CountryCode, CountryName) VALUES ('TG', 'Togo');
INSERT INTO Country (CountryCode, CountryName) VALUES ('TT', 'Trinidad & Tobago');
INSERT INTO Country (CountryCode, CountryName) VALUES ('TN', 'Tunisia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('TR', 'Turkey');
INSERT INTO Country (CountryCode, CountryName) VALUES ('TM', 'Turkmenistan, Republic of');
INSERT INTO Country (CountryCode, CountryName) VALUES ('TC', 'Turks & Caicos Is.');
INSERT INTO Country (CountryCode, CountryName) VALUES ('AE', 'U.A.E.');
INSERT INTO Country (CountryCode, CountryName) VALUES ('VI', 'U.S. Virgin Islands');
INSERT INTO Country (CountryCode, CountryName) VALUES ('US', 'United States');
INSERT INTO Country (CountryCode, CountryName) VALUES ('UG', 'Uganda');
INSERT INTO Country (CountryCode, CountryName) VALUES ('UA', 'Ukraine');
INSERT INTO Country (CountryCode, CountryName) VALUES ('GB', 'United Kingdom');
INSERT INTO Country (CountryCode, CountryName) VALUES ('UY', 'Uruguay');
INSERT INTO Country (CountryCode, CountryName) VALUES ('UZ', 'Uzbekistan');
INSERT INTO Country (CountryCode, CountryName) VALUES ('VU', 'Vanuatu');
INSERT INTO Country (CountryCode, CountryName) VALUES ('VA', 'Vatican City');
INSERT INTO Country (CountryCode, CountryName) VALUES ('VE', 'Venezuela');
INSERT INTO Country (CountryCode, CountryName) VALUES ('VN', 'Vietnam');
INSERT INTO Country (CountryCode, CountryName) VALUES ('WF', 'Wallis & Futuna Islands');
INSERT INTO Country (CountryCode, CountryName) VALUES ('YE', 'Yemen');
INSERT INTO Country (CountryCode, CountryName) VALUES ('ZM', 'Zambia');
INSERT INTO Country (CountryCode, CountryName) VALUES ('ZW', 'Zimbabwe');

-- CREATE DEFAULT ADMIN GROUP
INSERT INTO AdminGroup (Description) VALUES ('Administrator')

-- CREATE DEFAULT ADMIN SECTION (ONLY ONE FOR STARTUP)
INSERT INTO AdminSection (Code, Description) VALUES ('USERS', 'User Management')
INSERT INTO AdminSection (Code, Description) VALUES ('CONTENT_TOOL', 'Content Tool')
INSERT INTO AdminSection (Code, Description) VALUES ('STORE', 'Store Administration')
INSERT INTO AdminSection (Code, Description) VALUES ('MEMBERS', 'Member Administration')
INSERT INTO AdminSection (Code, Description) VALUES ('BROADCAST', 'Broadcast Email')
INSERT INTO AdminSection (Code, Description) VALUES ('ORDERS', 'Orders Section')
INSERT INTO AdminSection (Code, Description) VALUES ('BANNERS', 'Banners')
INSERT INTO AdminSection (Code, Description) VALUES ('REPORTS', 'Reports')
INSERT INTO AdminSection (Code, Description) VALUES ('MARKETING_TOOLS', 'Marketing Tools')
INSERT INTO AdminSection (Code, Description) VALUES ('SHIPPING_TAX', 'Shipping & Tax')
INSERT INTO AdminSection (Code, Description) VALUES ('FAQ', 'FAQ''s')
INSERT INTO AdminSection (Code, Description) VALUES ('CONTACT_US', 'Contact Us')

-- SET UP ACCESS RIGTHS FOR GROUP 'Administrators'
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (1,1)
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (2,1)
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (3,1)
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (4,1)
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (5,1)
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (6,1)
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (7,1)
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (8,1)
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (9,1)
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (10,1)
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (11,1)
INSERT INTO AdminAccess (SectionId, GroupId) VALUES (12,1)

-- INSERT FIRST ADMIN USER
INSERT INTO Admin (Username, Password, PasswordEx, Email, FirstName, LastName, IsInternal) VALUES ('admin', null, null, 'admin@americaneagle.com', 'Americaneagle', 'Admin User', 1)


-- ASSIGN DEFAULT ADMIN USER TO THE ADMIN GROUP
INSERT INTO AdminAdminGroup (AdminId, GroupId) VALUES (1,1)

SET IDENTITY_INSERT ContentToolSection ON;

insert into ContentToolSection (SectionId, SectionName, Folder) values (1, 'Main', '/')
insert into ContentToolSection (SectionId, SectionName, Folder) values (2, 'Contact Us', '/contactus/')
insert into ContentToolSection (SectionId, SectionName, Folder) values (3, 'News', '/news/')
insert into ContentToolSection (SectionId, SectionName, Folder) values (4, 'Store', '/store/')
insert into ContentToolSection (SectionId, SectionName, Folder) values (5, 'Members', '/members/')
insert into ContentToolSection (SectionId, SectionName, Folder) values (11,	'Customer Service',	'/service/')

SET IDENTITY_INSERT ContentToolSection OFF;

SET IDENTITY_INSERT ContentToolTemplate ON;
 
insert into ContentToolTemplate (TemplateId, TemplateName, IsDefault, DefaultContentId, TemplateHTML, PrintHTML) values (1, '1 Column', 0, null, 
'<%@ Register TagPrefix="CT" Namespace="MasterPages" Assembly="Common"%>
<%@ Register TagName="SmartBug" TagPrefix="CC" Src="~/controls/SmartBug.ascx" %>
<%@ Register TagName="GoogleTracking" TagPrefix="CC" Src="~/controls/GoogleTracking.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
<title id="PageTitle" EnableViewState="False" runat="server"></title>
<asp:literal id="MetaKeywords" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaDescription" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaRobots" EnableViewState="False" runat=server></asp:literal>
<link rel="stylesheet" href="/includes/style.css" type="text/css" />
<script src="/includes/functions.js.aspx" type="text/javascript"></script>
<script type="text/javascript" src="/includes/qm.js"></script>
<script type="text/javascript"  src="/includes/XmlHttpLookup.js"></script>
</head>

<body class="mainbody" style="min-width:1000px;">
<CC:SmartBug id="bug" runat="Server"></CC:SmartBug>

<CC:form id="main" method=post runat="server">

<div class="bdywrpr">

<div id="divSiteWrapper">
<div id="divScreen"></div>

<CT:ContentRegion runat="server" id="CT_Header" width="980" />

<table cellspacing="0" cellpadding="0" border="0" class="corwrpr">
<tr>
<td class="cormain">
<CT:ErrorMessage id="ErrorPlaceHolder" runat="server"/>
<CT:ContentRegion runat="server" id="CT_Main" width="980"/>
</td>
</tr>
</table>

<CT:ContentRegion runat="server" id="CT_Footer" width="980" />

<CT:NavigationRegion runat="server" id="NavigationRegion"/>

</div>

</div>
</CC:form>
<CC:GoogleTracking runat="server" />
</body>
</html>'
,
'<%@ Register TagPrefix="CT" Namespace="MasterPages" Assembly="Common"%>
<%@ Register TagName="GoogleTracking" TagPrefix="CC" Src="~/controls/GoogleTracking.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
<title id="PageTitle" EnableViewState="False" runat="server"></title>
<asp:literal id="MetaKeywords" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaDescription" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaRobots" EnableViewState="False" runat=server></asp:literal>
<link rel="stylesheet" href="/includes/style.css" type="text/css" />
<script src="/includes/functions.js.aspx" type="text/javascript"></script>
<script type="text/javascript" src="/includes/qm.js"></script>
<script type="text/javascript"  src="/includes/XmlHttpLookup.js"></script>
</head>

<body class="mainbody">

<CC:form id="main" method=post runat="server">

<div class="bdywrpr">

<table cellspacing="0" cellpadding="0" border="0">
<tr>
<td class="cormain" style="width:660px;">

<CT:ErrorMessage id="ErrorPlaceHolder" runat="server" Width="660"/>
<CT:ContentRegion runat="server" id="CT_Main" width="660"/>

</td>
</tr>
</table>

</div>
</CC:form>
<CC:GoogleTracking runat="server" />
</body>
</html>'
)

SET IDENTITY_INSERT ContentToolTemplate OFF;

SET IDENTITY_INSERT ContentToolTemplateRegion ON;

insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (1,1,'CT_Footer','Footer')
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (2,1,'CT_Header','Header')
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (3,1,'CT_Main','Main Area')

SET IDENTITY_INSERT ContentToolTemplateRegion OFF;

Update ContentToolTemplate set DefaultContentId = 3 where TemplateId = 1

SET IDENTITY_INSERT ContentToolModule ON;

insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (1, 'HTML', '/modules/Content.ascx', 0, 1000, null)
insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (2, 'Spacer - 10px', '/modules/Text.ascx', 0, 1000, '<br /><img src="/images/spacer.gif" width="1" height="10"><br />')
insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (3, 'Spacer - 20px', '/modules/Text.ascx', 0, 1000, '<br /><img src="/images/spacer.gif" width="1" height="20"><br />')
insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (4, 'Header', '/modules/header.ascx', 0, 1000, null)
insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (5, 'Footer', '/modules/footer.ascx', 0, 1000, null)
insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (13, 'Banner', '/modules/banner.ascx', 0, 1000, null)
insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (14, 'Store Left Navigation', '/modules/storeleftnavigation.ascx', 150, 200, null)
insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (15, 'My Account Left Navigation', '/modules/MyAccountLeftNavigation.ascx', 150, 200, null)
insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (16, 'Recently Viewed Items', '/modules/StoreRecentlyViewed.ascx', 150, 200, null)
insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (17, 'Store Home', '/modules/StoreHome.ascx', 600, 1000, null)
insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (19, 'Customer Service Left Navigation', '/modules/ServiceLeftNavigation.ascx', 150, 200, NULL)
insert into ContentToolModule (ModuleId, Name, ControlURL, MinWidth, MaxWidth, HTML) values (20, 'Contact Us','/modules/contact.ascx',350,1000,NULL);

SET IDENTITY_INSERT ContentToolModule OFF;

insert into ContentToolNavigation (Title,ParentId,IsInternalLink,PageId,SortOrder) values ('Section', null, 1, null, 1)
insert into ContentToolNavigation (Title,ParentId,IsInternalLink,PageId,SortOrder) values ('Sub-Section', 1, 1, null, 1)

-- Interior w/Left Rail TEMPLATE

SET IDENTITY_INSERT ContentToolTemplate ON;

insert into ContentToolTemplate (TemplateId, TemplateName, IsDefault, DefaultContentId, TemplateHTML, PrintHTML) values (2, '2 Column', 1, null, 
'<%@ Register TagPrefix="CT" Namespace="MasterPages" Assembly="Common"%>
<%@ Register TagName="SmartBug" TagPrefix="CC" Src="~/controls/SmartBug.ascx" %>
<%@ Register TagName="GoogleTracking" TagPrefix="CC" Src="~/controls/GoogleTracking.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
<title id="PageTitle" EnableViewState="False" runat="server"></title>
<asp:literal id="MetaKeywords" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaDescription" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaRobots" EnableViewState="False" runat=server></asp:literal>
<link rel="stylesheet" href="/includes/style.css" type="text/css" />
<script src="/includes/functions.js.aspx" type="text/javascript"></script>
<script type="text/javascript" src="/includes/qm.js"></script>
<script type="text/javascript"  src="/includes/XmlHttpLookup.js"></script>
</head>

<body class="mainbody" style="min-width:1000px;">
<CC:SmartBug id="bug" runat="Server"></CC:SmartBug>

<CC:form id="main" method=post runat="server">

<div class="bdywrpr">

<div id="divSiteWrapper">
<div id="divScreen"></div>

<CT:ContentRegion runat="server" id="CT_Header" width="980" />

<table cellspacing="0" cellpadding="0" border="0" class="corwrpr">
<tr>
<td class="corlnav">
<CT:ContentRegion runat="server" id="CT_Left" width="180" />

</td>
<td style="width:10px;"><div class="colsp">&nbsp;</div></td>
<td class="cormain" style="width:800px;">

<CT:ErrorMessage id="ErrorPlaceHolder" runat="server" Width="800"/>
<CT:ContentRegion runat="server" ID="CT_Top" Width="800" />
<CT:ContentRegion runat="server" id="CT_Main" width="800"/>

</td>
</tr>
</table>

<CT:ContentRegion runat="server" id="CT_Footer" width="980" />

<CT:NavigationRegion runat="server" id="NavigationRegion"/>

</div>

</div>
</CC:form>
<CC:GoogleTracking runat="server" />
</body>
</html>'
,
'<%@ Register TagPrefix="CT" Namespace="MasterPages" Assembly="Common"%>
<%@ Register TagName="GoogleTracking" TagPrefix="CC" Src="~/controls/GoogleTracking.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
<title id="PageTitle" EnableViewState="False" runat="server"></title>
<asp:literal id="MetaKeywords" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaDescription" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaRobots" EnableViewState="False" runat=server></asp:literal>
<link rel="stylesheet" href="/includes/style.css" type="text/css" />
<script src="/includes/functions.js.aspx" type="text/javascript"></script>
<script type="text/javascript" src="/includes/qm.js"></script>
<script type="text/javascript"  src="/includes/XmlHttpLookup.js"></script>
</head>

<body class="mainbody">

<CC:form id="main" method=post runat="server">

<div class="bdywrpr">

<table cellspacing="0" cellpadding="0" border="0">
<tr>
<td class="cormain" style="width:660px;">

<CT:ErrorMessage id="ErrorPlaceHolder" runat="server" Width="660"/>
<CT:ContentRegion runat="server" id="CT_Main" width="660"/>

</td>
</tr>
</table>

</div>
</CC:form>
<CC:GoogleTracking runat="server" />
</body>
</html>'
)
SET IDENTITY_INSERT ContentToolTemplate OFF;
  
SET IDENTITY_INSERT ContentToolTemplateRegion ON;
  
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (4,2,'CT_Footer','Footer')
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (5,2,'CT_Header','Header')
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (9,2,'CT_Left','Left Rail')
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (7,2,'CT_Top','Status Messages')
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (6,2,'CT_Main','Main Area')

SET IDENTITY_INSERT ContentToolTemplateRegion OFF;

Update ContentToolTemplate set DefaultContentId = 6 where TemplateId = 2

SET IDENTITY_INSERT ContentToolTemplate ON;

-- Interior w/Left Rail TEMPLATE
insert into ContentToolTemplate (TemplateId, TemplateName, IsDefault, DefaultContentId, TemplateHTML, PrintHTML) values (4, '3 Column', 0, null, 
'<%@ Register TagPrefix="CT" Namespace="MasterPages" Assembly="Common"%>
<%@ Register TagName="SmartBug" TagPrefix="CC" Src="~/controls/SmartBug.ascx" %>
<%@ Register TagName="GoogleTracking" TagPrefix="CC" Src="~/controls/GoogleTracking.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
<title id="PageTitle" EnableViewState="False" runat="server"></title>
<asp:literal id="MetaKeywords" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaDescription" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaRobots" EnableViewState="False" runat=server></asp:literal>
<link rel="stylesheet" href="/includes/style.css" type="text/css" />
<script src="/includes/functions.js.aspx" type="text/javascript"></script>
<script type="text/javascript" src="/includes/qm.js"></script>
<script type="text/javascript"  src="/includes/XmlHttpLookup.js"></script>
</head>

<body class="mainbody" style="min-width:1000px;">
<CC:SmartBug id="bug" runat="Server"></CC:SmartBug>

<CC:form id="main" method=post runat="server">

<div class="bdywrpr">

<div id="divSiteWrapper">
<div id="divScreen"></div>

<CT:ContentRegion runat="server" id="CT_Header" width="980" />

<table cellspacing="0" cellpadding="0" border="0" class="corwrpr">
<tr>
<td class="corlnav">
<CT:ContentRegion runat="server" id="CT_Left" width="180" />

</td>
<td style="width:10px;"><div class="colsp">&nbsp;</div></td>
<td class="cormain" style="width:600px;">

<CT:ErrorMessage id="ErrorPlaceHolder" runat="server" Width="600"/>
<CT:ContentRegion runat="server" ID="CT_Top" Width="600" />
<CT:ContentRegion runat="server" id="CT_Main" width="600"/>

</td>
<td style="width:10px;"><div class="colsp">&nbsp;</div></td>
<td class="corrrail">
<CT:ContentRegion runat="server" id="CT_Right" width="180" />
</td>
</tr>
</table>

<CT:ContentRegion runat="server" id="CT_Footer" width="980" />

<CT:NavigationRegion runat="server" id="NavigationRegion"/>

</div>

</div>
</CC:form>
<CC:GoogleTracking runat="server" />
</body>
</html>'
,
'<%@ Register TagPrefix="CT" Namespace="MasterPages" Assembly="Common"%>
<%@ Register TagName="GoogleTracking" TagPrefix="CC" Src="~/controls/GoogleTracking.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml" lang="en">
<head>
<title id="PageTitle" EnableViewState="False" runat="server"></title>
<asp:literal id="MetaKeywords" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaDescription" EnableViewState="False" runat=server></asp:literal>
<asp:literal id="MetaRobots" EnableViewState="False" runat=server></asp:literal>
<link rel="stylesheet" href="/includes/style.css" type="text/css" />
<script src="/includes/functions.js.aspx" type="text/javascript"></script>
<script type="text/javascript" src="/includes/qm.js"></script>
<script type="text/javascript"  src="/includes/XmlHttpLookup.js"></script>
</head>

<body class="mainbody">

<CC:form id="main" method=post runat="server">

<div class="bdywrpr">

<table cellspacing="0" cellpadding="0" border="0">
<tr>
<td class="cormain" style="width:660px;">

<CT:ErrorMessage id="ErrorPlaceHolder" runat="server" Width="660"/>
<CT:ContentRegion runat="server" id="CT_Main" width="660"/>

</td>
</tr>
</table>

</div>
</CC:form>
<CC:GoogleTracking runat="server" />
</body>
</html>'
)
SET IDENTITY_INSERT ContentToolTemplate OFF;
  
SET IDENTITY_INSERT ContentToolTemplateRegion ON;
  
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (10,4,'CT_Footer','Footer')
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (11,4,'CT_Header','Header')
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (14,4,'CT_Top','Status Messages')
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (15,4,'CT_Left','Left Rail')
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (16,4,'CT_Right','Right Rail')
insert into ContentToolTemplateRegion(TemplateRegionId, TemplateId, ContentRegion, RegionName) values (13,4,'CT_Main','Main Area')

SET IDENTITY_INSERT ContentToolTemplateRegion OFF;

Update ContentToolTemplate set DefaultContentId = 13 where TemplateId = 4

-- credit card types
insert into CreditCardType (Code, [Name], ImageName, IsActive, SortOrder) values ('V','Visa','visa.gif', 1,1);
insert into CreditCardType (Code, [Name], ImageName, IsActive, SortOrder) values ('M','Mastercard/Eurocard',	'mc.gif',	1,	2);
insert into CreditCardType (Code, [Name], ImageName, IsActive, SortOrder) values ('A','American Express',	'amx.gif',	1,	3);
insert into CreditCardType (Code, [Name], ImageName, IsActive, SortOrder) values ('D','Discover Network',	'disc.gif',	1,	4);

insert into HowHeard (HowHeard, SortOrder) values ('Search Engine',1);
insert into HowHeard (HowHeard, SortOrder) values ('Word of Mouth',2);
insert into HowHeard (HowHeard, SortOrder) values ('Other Web Site',3);
insert into HowHeard (HowHeard, SortOrder) values ('From Product',4);
insert into HowHeard (HowHeard, SortOrder) values ('Current Customer',5);
insert into HowHeard (HowHeard, SortOrder) values ('E-mail a Friend',6);
insert into HowHeard (HowHeard, SortOrder) values ('Catalog',7);
insert into HowHeard (HowHeard, SortOrder) values ('Referral Code',8);
insert into HowHeard (HowHeard, SortOrder, IsUserInput, UserInputLabel) values ('Other',9,1,'Please enter value below:');


SET IDENTITY_INSERT StoreDepartment ON;

INSERT INTO StoreDepartment (DepartmentId, LFT, RGT, ParentId, NAME, IsInactive) VALUES (23,	1,	22,	NULL,	'Store', 0);
INSERT INTO StoreDepartment (DepartmentId, LFT, RGT, ParentId, NAME, IsInactive) VALUES (24,	2,	3,	23,	'Recycle Bin', 1);
INSERT INTO StoreDepartment (DepartmentId, LFT, RGT, ParentId, NAME, IsInactive) VALUES (25, 4,	5,	23,	'Department #1', 0);
INSERT INTO StoreDepartment (DepartmentId, LFT, RGT, ParentId, NAME, IsInactive) VALUES (26,	6,	21,	23,	'Department #2', 0);
INSERT INTO StoreDepartment (DepartmentId, LFT, RGT, ParentId, NAME, IsInactive) VALUES (27,	7,	8,	26,	'AAA', 0);
INSERT INTO StoreDepartment (DepartmentId, LFT, RGT, ParentId, NAME, IsInactive) VALUES (28,	9,	14,	26,	'Another one', 0);
INSERT INTO StoreDepartment (DepartmentId, LFT, RGT, ParentId, NAME, IsInactive) VALUES (30,	10,	11,	28,	'Child 1', 0);
INSERT INTO StoreDepartment (DepartmentId, LFT, RGT, ParentId, NAME, IsInactive) VALUES (31,	12,	13,	28,	'Child 2', 0);
INSERT INTO StoreDepartment (DepartmentId, LFT, RGT, ParentId, NAME, IsInactive) VALUES (29,	15,	20,	26,	'One more', 0);
INSERT INTO StoreDepartment (DepartmentId, LFT, RGT, ParentId, NAME, IsInactive) VALUES (33,	16,	17,	29,	'AAA', 0);
INSERT INTO StoreDepartment (DepartmentId, LFT, RGT, ParentId, NAME, IsInactive) VALUES (32,	18,	19,	29,	'Child 1', 0);

SET IDENTITY_INSERT StoreDepartment OFF;

-- INSERT StoreOrderStatus
INSERT INTO StoreOrderStatus (StatusId, CODE, NAME , ProcessSortOrder, IsFinalAction) VALUES (1, 'N', 'New',1,0);
INSERT INTO StoreOrderStatus (StatusId, CODE, NAME , ProcessSortOrder, IsFinalAction) VALUES (2, 'O', 'Open',2,0);
INSERT INTO StoreOrderStatus (StatusId, CODE, NAME , ProcessSortOrder, IsFinalAction) VALUES (3, 'B', 'Back Ordered',3,0);
INSERT INTO StoreOrderStatus (StatusId, CODE, NAME , ProcessSortOrder, IsFinalAction) VALUES (4, 'P', 'Pending',4,0);
INSERT INTO StoreOrderStatus (StatusId, CODE, NAME , ProcessSortOrder, IsFinalAction) VALUES (5, 'S', 'Shipped',5,1);
INSERT INTO StoreOrderStatus (StatusId, CODE, NAME , ProcessSortOrder, IsFinalAction) VALUES (6, 'C', 'Cancelled',6,1);
INSERT INTO StoreOrderStatus (StatusId, CODE, NAME , ProcessSortOrder, IsFinalAction) VALUES (7, 'R', 'Returned',7,1);
INSERT INTO StoreOrderStatus (StatusId, CODE, NAME , ProcessSortOrder, IsFinalAction) VALUES (8, 'X', 'Completed',8,1);

-- INSERT CustomText
INSERT INTO CustomText (CODE, TITLE) VALUES ('OrderConfirmation', 'Order Confirmation Text')
INSERT INTO CustomText (CODE, TITLE, [VALUE], IsHelpTag) VALUES ('TitleTag', 'Title Tag', '<strong>Title Tags</strong>
<p><strong>Description:</strong><br />
This tag appears at the top of the browser when visitor view your site. Search engines use this tag when crawling sites to index content on a web page. The title tag will also be used to describe the web page whenever someone bookmarks the page or adds the page to favorites, and will be what is displayed as the first line of a search result listing. </p>
<p><strong>How to use:</strong><br />
Each title tag should be about 60 characters long and include two or three ideal search phrases. We encourage you to use unique title tags on each page of the site that provide a brief description of the content. The site name should be included at the beginning or at the end of the tag. This positioning should remain consistent through out the site. Also, avoid listing duplicate words next to each other - i.e. premium coffee, coffee flavors. This type of repetition can hurt your placement. </p>
<p>When writing title tags, capitalize significant words, but avoid using all capital letters. Also, avoid the use of symbols in place of words whenever possible (&amp;, #, @, etc.). Remember that this tag is displayed when viewing search engine results, so make sure that it is easy for people to read and understand.</p>', 1)
INSERT INTO CustomText (CODE, TITLE, [VALUE], IsHelpTag) VALUES ('MetaKeyWords', 'Meta Keywords', '<strong>META Keywords:</strong>
<p><strong>Description:</strong><br />
Meta key words are descriptive words inserted into the code on your web site. These words are not visible by the regular site visitor. Instead they are used to provide information about the content of your site to search engines. Not all search engines support this any longer, but for those that do, you can provide this text in addition to the page text to help get your site indexed. </p>
<p><strong>How to use:</strong><br />
Please add a list of keywords that you think other people would use to search for your site. This list does NOT have to be unique for each page. It can just be a short list of search terms separated by commas.</p>',1)
INSERT INTO CustomText (CODE, TITLE, [VALUE], IsHelpTag) VALUES ('MetaDescription', 'Meta Description', '<strong>META Description:</strong> <br />
<p><strong>Description:</strong><br />
The text that you enter into your meta description field is sometimes used by search engines to describe your site on the search results page. It is a place where you should use your key words to create a brief description of the content on your page. </p>
<p><strong>How to use:</strong><br />
Please limit this description to two or three sentences. These descriptions should be unique for each page on the site. Search engines place a lot less emphasis on this field, but these descriptions can show up on Google''s search result listing, so make sure that the content is easy to read and understand. </p>',1)

-- INSERT StoreShippingMethod
INSERT INTO StoreShippingMethod (NAME,UPSCODE,FEDEXCODE,SORTORDER,ISACTIVE) VALUES ('Ground', '03', 'FEDEXGROUND', 1, 1)
INSERT INTO StoreShippingMethod (NAME,UPSCODE,FEDEXCODE,SORTORDER,ISACTIVE) VALUES ('2nd Day Air', '02', 'FEDEX2DAY', 2, 1)




IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAttributeTree]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE  PROCEDURE [dbo].[sp_GetAttributeTree]
	@ItemId int,
	@IsValidated bit = 1
AS
BEGIN

DECLARE @RootSKU VARCHAR(50)
SET @RootSKU = (SELECT TOP 1 SKU FROM StoreItem WHERE ItemId = @ItemId);

	WITH AttributeTree (
			AttributeName, 
			AttributeType, 
			ItemAttributeId, 
			ItemId, 
			TemplateAttributeId, 
			ParentAttributeId, 
			AttributeValue, 
			SKU, 
			Price, 
			Weight, 
			IsInventoryManagement, 
			InventoryQty, 
			InventoryAction, 
			InventoryActionThreshold, 
			InventoryWarningThreshold, 
			BackorderDate, 
			ImageName, 
			ImageAlt, 
			ProductImage, 
			ProductAlt, 
			IsValidated, 
			IsActive, 
			SortOrder, 
			MasterSortOrder, 
			Level, 
			BuildSKU,
			ControlCount
	) AS (
		SELECT 
			t.AttributeName, 
			t.AttributeType, 
			a.ItemAttributeId, 
			a.ItemId, 
			a.TemplateAttributeId, 
			a.ParentAttributeId, 
			a.AttributeValue, 
			a.SKU, 
			a.Price, 
			a.Weight, 
			t.IsInventoryManagement, 
			a.InventoryQty, 
			a.InventoryAction, 
			a.InventoryActionThreshold, 
			a.InventoryWarningThreshold, 
			a.BackorderDate, 
			a.ImageName, 
			a.ImageAlt, 
			a.ProductImage, 
			a.ProductAlt, 
			a.IsValidated, 
			a.IsActive, 
			a.SortOrder, 
			CONVERT(VARBINARY(MAX), a.SortOrder) + CONVERT(VARBINARY(MAX), a.ItemAttributeId) AS MasterSortOrder, 
			0 AS Level, 
			CONVERT(VARCHAR(255), @RootSKU) + a.SKU AS BuildSKU, 
			a.ControlCount
		FROM 
			StoreItemAttribute a
			INNER JOIN StoreItemTemplateAttribute t ON a.TemplateAttributeId = t.TemplateAttributeId
		WHERE 
			a.ItemId = @ItemId
			AND a.ParentAttributeId IS NULL
			AND IsValidated >= @IsValidated 

		UNION ALL 

		SELECT 
			t.AttributeName, 
			t.AttributeType, 
			a.ItemAttributeId, 
			a.ItemId, 
			a.TemplateAttributeId, 
			a.ParentAttributeId, 
			a.AttributeValue, 
			a.SKU, 
			a.Price, 
			a.Weight, 
			AttributeTree.IsInventoryManagement, 
			a.InventoryQty, 
			a.InventoryAction, 
			a.InventoryActionThreshold, 
			a.InventoryWarningThreshold, 
			a.BackorderDate, 
			a.ImageName, 
			a.ImageAlt, 
			a.ProductImage, 
			a.ProductAlt, 
			a.IsValidated, 
			a.IsActive, 
			a.SortOrder, 
			AttributeTree.MasterSortOrder + CONVERT(VARBINARY(MAX), a.SortOrder) + CONVERT(VARBINARY(MAX), a.ItemAttributeId) AS MasterSortOrder, 
			AttributeTree.Level + 1 AS Level, 
			CONVERT(VARCHAR(255), AttributeTree.BuildSKU) + a.SKU AS BuildSKU,
			a.ControlCount
		FROM 
			StoreItemAttribute a 
			INNER JOIN StoreItemTemplateAttribute t ON a.TemplateAttributeId = t.TemplateAttributeId
			INNER JOIN AttributeTree ON a.ParentAttributeId = AttributeTree.ItemAttributeId AND a.ItemId = AttributeTree.ItemId
		WHERE
			a.IsValidated >= @IsValidated 
			AND a.ItemId = @ItemId
	)

	SELECT 
		AttributeName, 
		AttributeType, 
		ItemAttributeId, 
		TemplateAttributeId, 
		ParentAttributeId, 
		AttributeValue, 
		SKU, 
		Price, 
		Weight, 
		IsInventoryManagement, 
		InventoryQty, 
		InventoryAction, 
		InventoryActionThreshold, 
		InventoryWarningThreshold, 
		BackorderDate, 
		ImageName, 
		ImageAlt, 
		ProductImage, 
		ProductAlt, 
		SortOrder, 
		Level,
		ControlCount,
		IsValidated,
		IsActive,
		BuildSKU,
		MasterSortOrder
	FROM 
		AttributeTree

END'
END






IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetAttributeTreeTableLayout]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[sp_GetAttributeTreeTableLayout]
	@ItemId int,
	@TemplateId int
AS
BEGIN

DECLARE @RootSKU VARCHAR(50)
SET @RootSKU = (SELECT TOP 1 SKU FROM StoreItem WHERE ItemId = @ItemId);

	WITH AttributeTree (
		AttributeName, 
		AttributeType, 
		ItemAttributeId, 
		TemplateAttributeId, 
		ParentAttributeId, 
		AttributeValue, 
		SKU, 
		Price, 
		Weight, 
		IsInventoryManagement, 
		InventoryQty, 
		InventoryAction, 
		InventoryActionThreshold, 
		InventoryWarningThreshold, 
		BackorderDate, 
		ImageName, 
		ImageAlt, 
		ProductImage, 
		ProductAlt, 
		IsValidated, 
		SortOrder, 
		MasterSortOrder, 
		Level, 
		BuildSKU, 
		Description, 
		ItemAttributeIds
	) AS (

		SELECT 
			t.AttributeName, 
			t.AttributeType, 
			a.ItemAttributeId, 
			a.TemplateAttributeId, 
			a.ParentAttributeId, 
			a.AttributeValue, 
			a.SKU, 
			a.Price, 
			a.Weight, 
			t.IsInventoryManagement, 
			a.InventoryQty, 
			a.InventoryAction, 
			a.InventoryActionThreshold, 
			a.InventoryWarningThreshold, 
			a.BackorderDate, 
			a.ImageName, 
			a.ImageAlt, 
			a.ProductImage, 
			a.ProductAlt, 
			a.IsValidated, 
			a.SortOrder, 
			CONVERT(VARBINARY(MAX), a.SortOrder) + CONVERT(VARBINARY(MAX), a.ItemAttributeId) AS MasterSortOrder, 
			0 AS Level, 
			CONVERT(VARCHAR(255), @RootSKU) + a.SKU AS BuildSKU, 
			''<b>'' + CONVERT(VARCHAR(MAX), t.AttributeName + '':</b> '' + a.AttributeValue) AS Description, 
			CONVERT(VARCHAR(MAX), a.ItemAttributeId) AS ItemAttributeIds
		FROM 
			StoreItemAttribute a
			INNER JOIN StoreItemTemplateAttribute t ON a.TemplateAttributeId = t.TemplateAttributeId
		WHERE 
			a.ItemAttributeId IN (SELECT ItemAttributeId FROM StoreItemAttribute WHERE ItemId = @ItemId AND ParentAttributeId IS NULL) 
			AND IsValidated = 1

		UNION ALL 

		SELECT 
			t.AttributeName, 
			t.AttributeType, 
			a.ItemAttributeId, 
			a.TemplateAttributeId, 
			a.ParentAttributeId, 
			a.AttributeValue, 
			a.SKU, 
			a.Price, 
			a.Weight, 
			AttributeTree.IsInventoryManagement, 
			a.InventoryQty, 
			a.InventoryAction, 
			a.InventoryActionThreshold, 
			a.InventoryWarningThreshold, 
			a.BackorderDate, 
			a.ImageName, 
			a.ImageAlt, 
			COALESCE(a.ProductImage, AttributeTree.ProductImage) AS ProductImage, 
			a.ProductAlt, 
			a.IsValidated, 
			a.SortOrder, 
			AttributeTree.MasterSortOrder + CONVERT(VARBINARY(MAX), a.SortOrder) + CONVERT(VARBINARY(MAX), a.ItemAttributeId) AS MasterSortOrder, 
			AttributeTree.Level + 1 AS Level, 
			CONVERT(VARCHAR(255), AttributeTree.BuildSKU) + a.SKU AS BuildSKU, 
			CONVERT(VARCHAR(MAX), AttributeTree.Description) + ''<br /><b>'' + t.AttributeName + '':</b> '' + a.AttributeValue AS Description,
			CONVERT(VARCHAR(MAX), AttributeTree.ItemAttributeIds) + '','' + CONVERT(VARCHAR(MAX), a.ItemAttributeId) AS ItemAttributeIds
		FROM 
			StoreItemAttribute a 
			INNER JOIN StoreItemTemplateAttribute t ON a.TemplateAttributeId = t.TemplateAttributeId
			INNER JOIN AttributeTree ON a.ParentAttributeId = AttributeTree.ItemAttributeId
		WHERE
			a.IsValidated = 1

	)

	SELECT 
		AttributeName, 
		AttributeType, 
		ItemAttributeId, 
		TemplateAttributeId, 
		ParentAttributeId, 
		AttributeValue, 
		SKU, 
		Price, 
		Weight, 
		IsInventoryManagement, 
		InventoryQty, 
		InventoryAction, 
		InventoryActionThreshold, 
		InventoryWarningThreshold, 
		BackorderDate, 
		ImageName, 
		ImageAlt, 
		ProductImage, 
		ProductAlt, 
		SortOrder, 
		Level,
		BuildSKU, 
		Description, 
		ItemAttributeIds
	FROM 
		AttributeTree
	WHERE 
		TemplateAttributeId IN (SELECT TemplateAttributeId FROM StoreItemTemplateAttribute WHERE TemplateId = @TemplateId AND TemplateAttributeId NOT IN (SELECT ParentId FROM StoreItemTemplateAttribute WHERE ParentId IS NOT NULL))
	ORDER BY 
		MasterSortOrder

END'
END


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetTemplateAttributeTree]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[sp_GetTemplateAttributeTree]
	@TemplateAttributeId INT
AS
BEGIN

	WITH TemplateAttributeTree (
		TemplateAttributeId, ParentId, AttributeName, IsInventoryManagement, FunctionType, AttributeType, SortOrder, Level
	) AS (
		SELECT 
			TemplateAttributeId, ParentId, AttributeName, IsInventoryManagement, FunctionType, AttributeType, SortOrder, 0 AS Level 
		FROM 
			StoreItemTemplateAttribute 
		WHERE 
			TemplateAttributeId = @TemplateAttributeId

		UNION ALL 

		SELECT 
			t.TemplateAttributeId, t.ParentId, t.AttributeName, TemplateAttributeTree.IsInventoryManagement, t.FunctionType, t.AttributeType, t.SortOrder, TemplateAttributeTree.Level + 1 AS Level 
		FROM 
			StoreItemTemplateAttribute t 
			INNER JOIN TemplateAttributeTree ON t.ParentId = TemplateAttributeTree.TemplateAttributeId 
	)

	SELECT * FROM TemplateAttributeTree

END'
END


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_GetTemplateAttributeTreeByTemplate]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[sp_GetTemplateAttributeTreeByTemplate]
	@TemplateId INT
AS
BEGIN

	WITH TemplateAttributeTree (
		TemplateAttributeId, TemplateId, ParentId, AttributeName, IsInventoryManagement, Parent, FunctionType, AttributeType, Level, SortOrder, MasterSortOrder
	) AS (
		SELECT 
			a.TemplateAttributeId, a.TemplateId, a.ParentId, a.AttributeName, a.IsInventoryManagement, CONVERT(VARCHAR(50),'''') AS Parent, a.FunctionType, a.AttributeType, 0 AS Level, 
			a.SortOrder, CONVERT(VARBINARY(MAX), a.TemplateAttributeId) AS MasterSortOrder
		FROM 
			StoreItemTemplateAttribute a
		WHERE 
			a.TemplateAttributeId IN (SELECT TemplateAttributeId FROM StoreItemTemplateAttribute WHERE TemplateId = @TemplateId AND ParentId IS NULL)

		UNION ALL 

		SELECT 
			t.TemplateAttributeId, t.TemplateId, t.ParentId, t.AttributeName, TemplateAttributeTree.IsInventoryManagement, CONVERT(VARCHAR(50), TemplateAttributeTree.AttributeName) AS Parent, t.FunctionType, t.AttributeType, 
			TemplateAttributeTree.Level + 1 AS Level, t.SortOrder,
			TemplateAttributeTree.MasterSortOrder + CONVERT(VARBINARY(MAX), t.TemplateAttributeId) AS MasterSortOrder
		FROM 
			StoreItemTemplateAttribute t 
			INNER JOIN TemplateAttributeTree ON t.ParentId = TemplateAttributeTree.TemplateAttributeId 

	)
	SELECT TemplateAttributeId, TemplateId, ParentId, AttributeName, IsInventoryManagement, Parent, FunctionType, AttributeType, Level, SortOrder FROM TemplateAttributeTree ORDER BY MasterSortOrder

END'
END



IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_SaveFinalSKUS]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[sp_SaveFinalSKUS](@ItemId INT) AS
BEGIN

	WITH AttributeTree (
			ItemAttributeId, ParentAttributeId, BuildSKU, TemplateAttributeId
	) AS (

		SELECT 
			a.ItemAttributeId, 
			a.ParentAttributeId, 
			CONVERT(VARCHAR(255), COALESCE(si.SKU,'''') + COALESCE(a.SKU,'''')) AS BuildSKU,
			TemplateAttributeId
		FROM 
			StoreItemAttribute a
			INNER JOIN StoreItem si ON a.ItemId = si.ItemId
		WHERE 
			a.ItemId = @ItemId
			AND TemplateAttributeId = (select top 1 TemplateAttributeId FROM StoreItemTemplateAttribute WHERE TemplateId = si.TemplateId AND ParentId IS NULL)

		UNION ALL 

		SELECT 
			a.ItemAttributeId, 
			a.ParentAttributeId, 
			CONVERT(VARCHAR(255), AttributeTree.BuildSKU + COALESCE(a.SKU,'''')) AS BuildSKU, 
			a.TemplateAttributeId
		FROM 
			StoreItemAttribute a 
			INNER JOIN AttributeTree ON a.ParentAttributeId = AttributeTree.ItemAttributeId
	)


	UPDATE StoreItemAttribute SET FinalSKU = tmp.BuildSKU FROM (

	SELECT
		ItemAttributeId,
		BuildSKU
	FROM 
		AttributeTree at
	WHERE 
		TemplateAttributeId IN (SELECT TemplateAttributeId FROM StoreItemTemplateAttribute a1 WHERE NOT EXISTS (SELECT TemplateAttributeId FROM StoreItemTemplateAttribute a2 WHERE a1.TemplateAttributeId = a2.ParentId))
	) tmp WHERE tmp.ItemAttributeId = StoreItemAttribute.ItemAttributeId

END'
END



IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_ValidateSKU]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE PROCEDURE [dbo].[sp_ValidateSKU]
AS
BEGIN
	WITH AttributeTree (
			TemplateAttributeId, ItemAttributeId, ParentAttributeId, BuildSKU, Level
	) AS (

		SELECT 
			a.TemplateAttributeId, a.ItemAttributeId, a.ParentAttributeId, CONVERT(VARCHAR(255), COALESCE(i.SKU,'''')) + COALESCE(a.SKU,'''') AS BuildSKU, 0 AS Level
		FROM 
			StoreItemAttribute a
			INNER JOIN StoreItem i ON a.ItemId = i.ItemId
			INNER JOIN StoreItemTemplateAttribute t ON a.TemplateAttributeId = t.TemplateAttributeId
		WHERE 
			a.ItemAttributeId IN (SELECT ItemAttributeId FROM StoreItemAttribute WHERE ParentAttributeId IS NULL) 
			AND IsValidated = 1

		UNION ALL 

		SELECT 
			a.TemplateAttributeId, a.ItemAttributeId, a.ParentAttributeId, CONVERT(VARCHAR(255), COALESCE(AttributeTree.BuildSKU,'''')) + COALESCE(a.SKU,'''') AS BuildSKU, AttributeTree.Level + 1 AS Level
		FROM 
			StoreItemAttribute a 
			INNER JOIN StoreItem i ON a.ItemId = i.ItemId
			INNER JOIN StoreItemTemplateAttribute t ON a.TemplateAttributeId = t.TemplateAttributeId
			INNER JOIN AttributeTree ON a.ParentAttributeId = AttributeTree.ItemAttributeId
		WHERE
			IsValidated = 1

	)

	SELECT * FROM (
		SELECT 
			BuildSKU, COUNT(*) AS iCount
		FROM 
			AttributeTree 
		WHERE
			NOT EXISTS (SELECT TOP 1 TemplateAttributeId FROM StoreItemTemplateAttribute WHERE ParentId = AttributeTree.TemplateAttributeId)
		GROUP BY
			BuildSKU

		UNION

		SELECT SKU AS BuildSKU, 1 AS iCount FROM StoreItem
		WHERE
			NOT EXISTS (SELECT TOP 1 ItemAttributeId FROM StoreItemAttribute WHERE ItemId = StoreItem.ItemId)
		GROUP BY
			SKU
	) TMP
	WHERE
		iCount > 1
		OR BuildSKU = ''''

END'
END



IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[fn_SearchBuildSKU]') AND type in (N'P', N'PC'))
BEGIN
EXEC dbo.sp_executesql @statement = N'
CREATE FUNCTION [dbo].[fn_SearchBuildSKU]
(
	@SKU VARCHAR(50)
)
RETURNS @RESULT TABLE (ItemId INT PRIMARY KEY)
AS
BEGIN
	WITH AttributeTree (
			ItemId, ItemAttributeId, ParentAttributeId, BuildSKU
	) AS (

		SELECT 
			i.ItemId, 
			a.ItemAttributeId, 
			a.ParentAttributeId, 
			CONVERT(VARCHAR(255), COALESCE(i.SKU,'''')) + COALESCE(a.SKU,'''') AS BuildSKU
		FROM 
			StoreItemAttribute a
			INNER JOIN StoreItem i ON a.ItemId = i.ItemId
		WHERE 
			a.ItemAttributeId IN (SELECT ItemAttributeId FROM StoreItemAttribute WHERE ParentAttributeId IS NULL) 
			AND IsValidated = 1

		UNION ALL 

		SELECT 
			i.ItemId, 
			a.ItemAttributeId, 
			a.ParentAttributeId, 
			CONVERT(VARCHAR(255), 
			COALESCE(AttributeTree.BuildSKU,'''')) + COALESCE(a.SKU,'''') AS BuildSKU
		FROM 
			StoreItemAttribute a 
			INNER JOIN StoreItem i ON a.ItemId = i.ItemId
			INNER JOIN AttributeTree ON a.ParentAttributeId = AttributeTree.ItemAttributeId
		WHERE
			IsValidated = 1

	)

	INSERT INTO @RESULT
	SELECT ItemId FROM AttributeTree WHERE BuildSKU LIKE ''%'' + @SKU + ''%''
	UNION 
	SELECT ItemId FROM StoreItem WHERE SKU LIKE ''%'' + @SKU + ''%''

RETURN 
END'
END









INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (57,'Authorize.net settings','AuthorizeNetEnabled','0','YESNO', 1, 1, 0,'Is Authorize.net functionality used on this site?'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES ( 4,'General','ContactUsEmail','andreww@americanealge.com','STRING', 1, 0, 0,'Contact Us e-mail Address'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES ( 7,'General','ContactUsEmailSubject','Contact Us','STRING', 3, 0, 0,'Subject used when responding to contacts via email'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (22,'General','ContactUsInternalEmailSubject','New contact us message','STRING', 4, 0, 0,'Subject used when a user is trying to contact'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES ( 6,'General','ContactUsName','Andrew Wawrzynek','STRING', 2, 0, 0,'Name used when responding to contact email'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES ( 2,'Search Engine Optimization','DefaultMetaDescription','Store Startup Module Meta Description','TA', 2, 1, 0,'Meta Description (default value)'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES ( 1,'Search Engine Optimization','DefaultMetaKeywords','Store Startup Module Meta Keywords','TA', 1, 1, 0,'Meta Keywords (default value)'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (45,'Store','GiftWrappingEnabled','1','YESNO', 1, 1, 0,'Is Gift Wrap Functionality Enabled?'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (44,'Store','GiftWrapPrice','5','INTEGER', 1, 0, 0,'Gift Wrap Price'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (46,'Store','MultipleShipToEnabled','1','YESNO', 1, 1, 0,'Is Multiple Ship-To functionality enabled?'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (49,'Payflow Pro settings','PayflowEnabled','0','YESNO', 2, 1, 0,'Is VeriSign PayFlow functionality used on this site?'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (51,'Payflow Pro settings','PayflowPartner','verisign','STRING', 4, 1, 0,'PayFlow Partner'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (48,'Payflow Pro settings','PayflowPassword',NULL,'STRING', 3, 1, 1,'PayFlow Password'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (53,'Payflow Pro settings','PayflowTimeout','30','INTEGER', 6, 1, 0,'PayFlow Timeout'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (54,'Payflow Pro settings','PayflowTransactionType','A','TRANSACTION', 7, 1, 0,'Payflow Transaction Type'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (50,'Payflow Pro settings','PayflowUsername',NULL,'STRING', 2, 1, 1,'PayFlow Login'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (52,'Payflow Pro settings','PayflowVendor',NULL,'STRING', 5, 1, 0,'PayFlow Vendor'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (56,'PayPal settings','PayPalEnabled','0','YESNO', 2, 1, 0,'Is PayPal functionality used on this site?'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (47,'Store','SaveCreditCardInfo','1','YESNO', 3, 1, 0,'Save Credit Card info with the Order?'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (41,'Shipping','ShippingAddress1','1 S. Northwest Hwy','STRING', 8, 0, 0,'If you are using UPS or FedEx for shipping calculations, please provide the depot Address Line 1. This Address will be used for all automatic calculations.'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (42,'Shipping','ShippingAddress2',NULL,'STRING', 9, 0, 0,'If you are using UPS or FedEx for shipping calculations, please provide the depot Address Line 2. This Address will be used for all automatic calculations.'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (43,'Shipping','ShippingCity','Park Ridge','STRING', 6, 0, 0,'Shipping City, If you are using UPS or Fedex for Shipping Calculation, this field is requried'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (35,'Shipping','ShippingOption','ShippingUPS','SHIPPING', 1, 0, 0,'Please select the shipping option that you choose for your site'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (37,'Shipping','ShippingPerc',NULL,'FLOAT', 3, 0, 0,'Please input the value [%] that will be counted for each order if shipping option is choosen as "Straight Percentage of Amount"'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (38,'Shipping','ShippingSame','5','FLOAT', 4, 0, 0,'Please input the amount [$] that will be added to each order if shipping option is choosen as "Allways the same"'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (40,'Shipping','ShippingState','IL','STRING', 7, 0, 0,'Shipping State, If you are using UPS or Fedex for Shipping Calculation, this field is requried'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (39,'Shipping','ShippingZip','60068','STRING', 5, 0, 0,'If you are using UPS or FedEx for shipping calculations, please provide the depot zipcode. This zipcode will be used for all automatic calculations.'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (28,'Image Sizes - Department','StoreDepartmentLargeHeight','500','INTEGER', 16, 1, 0,'Department large height'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (27,'Image Sizes - Department','StoreDepartmentLargeWidth','500','INTEGER', 15, 1, 0,'Department large width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (24,'Image Sizes - Department','StoreDepartmentRegularHeight','300','INTEGER', 12, 1, 0,'Department regular height'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (23,'Image Sizes - Department','StoreDepartmentRegularWidth','300','INTEGER', 11, 1, 0,'Department regular width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (26,'Image Sizes - Department','StoreDepartmentThumbnailHeight','150','INTEGER', 14, 1, 0,'Department thumbnail height'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (25,'Image Sizes - Department','StoreDepartmentThumbnailWidth','150','INTEGER', 13, 1, 0,'Department thumbnail width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (34,'Image Sizes - Alternate Item Image','StoreItemImageAlternateLargeHeight','500','INTEGER', 22, 1, 0,'Item image alternate Large width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (33,'Image Sizes - Alternate Item Image','StoreItemImageAlternateLargeWidth','500','INTEGER', 21, 1, 0,'Item image alternate large width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (32,'Image Sizes - Alternate Item Image','StoreItemImageAlternateSmallHeight','75','INTEGER', 20, 1, 0,'Item image alternate small width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (31,'Image Sizes - Alternate Item Image','StoreItemImageAlternateSmallWidth','75','INTEGER', 19, 1, 0,'Item image alternate small width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES ( 9,'Image Sizes - Item','StoreItemImageCartHeight','75','INTEGER', 2, 1, 0,'Item image cart height'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES ( 8,'Image Sizes - Item','StoreItemImageCartWidth','75','INTEGER', 1, 1, 0,'Item image cart width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (13,'Image Sizes - Item','StoreItemImageFeaturedHeight','175','INTEGER', 6, 1, 0,'Item image related height'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (12,'Image Sizes - Item','StoreItemImageFeaturedWidth','175','INTEGER', 5, 1, 0,'Item image related width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (17,'Image Sizes - Item','StoreItemImageLargeHeight','500','INTEGER', 10, 1, 0,'Item image large height'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (16,'Image Sizes - Item','StoreItemImageLargeWidth','500','INTEGER', 9, 1, 0,'Item image large width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (15,'Image Sizes - Item','StoreItemImageRegularHeight','246','INTEGER', 8, 1, 0,'Item image product height'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (14,'Image Sizes - Item','StoreItemImageRegularWidth','246','INTEGER', 7, 1, 0,'Item image product width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (11,'Image Sizes - Item','StoreItemImageRelatedHeight','123','INTEGER', 4, 1, 0,'Item image related height'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (10,'Image Sizes - Item','StoreItemImageRelatedWidth','123','INTEGER', 3, 1, 0,'Item image related width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (30,'Image Sizes - Item','StoreItemImageThumbnailHeight','100','INTEGER', 18, 1, 0,'Item image thumbnail height'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (29,'Image Sizes - Item','StoreItemImageThumbnailWidth','100','INTEGER', 17, 1, 0,'Item image thumbnail width'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (58,'Order Confirmation Email','StoreOrderFromEmail','andreww@americaneagle.com','STRING', 1, 1, 0,'Store administrator email.'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (59,'Order Confirmation Email','StoreOrderFromName','Andrew Wawrzynek','STRING', 2, 1, 0,'Store administrator name.'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (55,'Administrator','TestMode','1','YESNO', 2, 1, 0,'Is this site in test mode?'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (60,'Store','UPSAddressVerification','1','YESNO', 4, 1, 0,'Use Address Verification from UPS?'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (61,'Administrator','PasswordEx','0','YESNO', 4, 1, 0,'Use Secondary Password feature?'); 
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (62,'Authorize.net settings','AuthorizeNetAPILogin',NULL,'STRING',2,1,1,'Authorize.Net API Login')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (63,'Authorize.net settings','AuthorizeNetTransactionKey',NULL,'STRING',3,1,1,'Authorize.Net Transaction Key')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (64,'Authorize.net settings','AuthorizeNetTransactionType','A','TRANSACTION',4,1,0,'Authorize.Net Transaction Type')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (65,'Image Sizes - Swatch','StoreItemSwatchWidth','56','INTEGER','1','True','False','Attribute Swatch Width')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (66,'Image Sizes - Swatch','StoreItemSwatchHeight','30','INTEGER','2','True','False','Attribute Swatch Height')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (67,'Image Sizes - Swatch','StoreItemSwatchSmallWidth','48','INTEGER','3','True','False','Attribute Swatch Small Width')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (68,'Image Sizes - Swatch','StoreItemSwatchSmallHeight','23','INTEGER','4','True','False','Attribute Swatch Small Height')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (71,'Store','StatusUpdateSubject','Your order has shipped!','STRING','5','False','False','Default email subject for sending order status emails.')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (72,'Store','StatusUpdateBody','Guess what? Your order has shipped. See details below.','STRING','6','False','False','Default email message for sending order status emails.')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (73,'Google Analytics','IsGoogleAnalyticsEcommerceTracking','0','YESNO','1','True','False','Is Google Ecommerce Tracking')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (76,'Content Tool','IdevSearchEnabled','0','YESNO','1','True','False','Is idev search enabled?')
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (77,'Inventory','EnableInventoryManagement','0','YESNO', 1, 1, 0,'Enable Inventory Management'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (78,'Inventory','EnableAttributeInventoryManagement','0','YESNO', 2, 1, 0,'Enable Attribute Inventory Management'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (79,'Inventory','InventoryActionThreshold','0','INTEGER', 3, 1, 0,'Inventory Action Threshold'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (80,'Inventory','InventoryWarningThreshold','10','INTEGER', 4, 1, 0,'Inventory Warning Threshold'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (81,'Inventory','InventoryWarningEmail','andreww@americaneagle.com','STRING', 5, 1, 0,'Inventory Warning Email'); 
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (82,'Inventory','EnableInventoryWarningEmail','0','YESNO', 6, 1, 0,'Enable Inventory Warning Email');
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (83,'Inventory','InventoryReplenishmentFromEmail','tom.osborn@americaneagle.com','STRING',7,1,0,'Inventory Replenishment From Email');
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (84,'Inventory','InventoryReplenishmentFromName','Store Startup','STRING',8,1,0,'Inventory Replenishment From Name');
INSERT INTO SYSPARAM (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (85,'Store','MinutesToCacheAttributes','0','INTEGER',6,1,0,'Minutes To Cache Attributes (0 = disable)');
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (88, 'Shipping', 'IsPackingListPDF', '1','YESNO','11',0,0,'Should packing list be created in PDF format?  If yes, please see how storestartup is setup on PDFSERVER and add this for your site as well.')
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (91, 'General', 'ContactUsReplyFromEmail', 'Contact Us Reply From Email', 'STRING', 7, 0, 0, 'Contact Us Reply From Email');
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (90, 'General', 'ContactUsReplyFromName', 'Contact Us Reply From Name', 'STRING', 6, 0, 0, 'Contact Us Reply From Name');
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (89, 'General', 'ContactUsReplySubject', 'Contact Us Reply Subject', 'STRING', 5, 0, 0, 'Contact Us Reply Subject');
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (92, 'General', 'FaqReplyFromEmail', 'reply@americaneagle.com', 'STRING', 8, 0, 0, 'FAQ Reply From Email');
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (93, 'General', 'FaqReplyFromName', 'FAQ Reply From Name', 'STRING', 9, 0, 0, 'FAQ Reply From Name');
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (94, 'General', 'FaqReplySubject', 'FAQ Reply Subject', 'STRING', 10, 0, 0, 'FAQ Reply Subject');
INSERT INTO Sysparam (ParamId, GroupName, Name, Value, Type, SortOrder, IsInternal, IsEncrypted, Comments) VALUES (95, 'Store', 'ArchiveIncompleteOrderDays', '91', 'INTEGER', 10, 1, 0, 'Archive Incomplete Order Days');
SET IDENTITY_INSERT ContentToolPage ON;

insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 1, 1,NULL,NULL,NULL,NULL, 0, 0,NULL,NULL,'11/15/2007',NULL,NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 2, 1, 1,NULL,NULL,NULL, 0, 0,NULL,NULL,'11/15/2007',NULL,NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 3, 1, 1,'/default.aspx','Home Page','Store Startup XXXXX', 1, 1,NULL,'shop, store','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 42, 1, 3,NULL,NULL,NULL, 0, 0,NULL,NULL,'11/15/2007',NULL,NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 5, 2,NULL,NULL,NULL,NULL, 0, 0,NULL,NULL,'11/15/2007',NULL,NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 8, 2, 1,'/signup.aspx','Newsletter Signup','Newsletter Signup', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 9, 2, 1,'/unsubscribe.aspx','Newsletter Unsubscribe','Newsletter Unsubscribe', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 21, 2, 3,NULL,NULL,NULL, 0, 0,NULL,NULL,'11/15/2007',NULL,NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 23, 2, 4,'/store/default.aspx','Online Store','Online Store', 0, 0,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 29, 2, 4,'/store/brand.aspx','Brand Page','Brand Page', 0, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 30, 2, 4,'/store/item.aspx','Store Item','Store Item', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 44, 2, 4,'/store/main.aspx','Store Department','Store Department', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 4, 2, 5,'/members/register.aspx','Members - Register','Members - Default', 1, 1,'startup, startup, startup','startup, startup, startup','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 6, 2, 5,'/members/login.aspx','Members - Login','Members - Login', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 7, 2, 5,'/members/default.aspx','Members - Default','Members - Default', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 11, 2, 5,'/members/reminders/default.aspx','Members - Reminders','Members - Reminders', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 12, 2, 5,'/members/reminders/edit.aspx','Members - Add/Edit Reminder','Members - Add/Edit Reminder', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 13, 2, 5,'/members/addressbook/default.aspx','Members - Address Book','Members - Address Book', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 14, 2, 5,'/members/addressbook/edit.aspx','Members - Add/Edit Address Book Entry','Members - Add/Edit Address Book Entry', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 15, 2, 5,'/members/email.aspx','Members - Email Preferences','Members - Email Preferences', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 16, 2, 5,'/members/account.aspx','Members - Change Password','Members - Change Password', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 24, 2, 5,'/members/forgot.aspx','Forgot Password?','Forgot Password?', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 25, 2, 5,'/members/confirmation.aspx','Confirmation Page','Confirmation Page', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 26, 2, 5,'/members/addresses.aspx','Edit Default Billing and Shipping Address','Edit Default Billing and Shipping Address', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 27, 2, 5,'/members/orders/default.aspx','Order History','Order History', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 41, 2, 5,'/members/wishlist/default.aspx','Wish List','Wish List', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 43, 2, 5,'/members/orders/view.aspx','Order Details','Order Details', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 45, 2, 5,'/members/wishlist/send.aspx','Send Wishlist','Send Wishlist', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 32, 4,NULL,NULL,NULL,NULL, 0, 0,NULL,NULL,'11/15/2007',NULL,NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 46, 4, 1,'/wishlist.aspx','Wishlist','Wishlist', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 33, 4, 4,NULL,NULL,NULL, 0, 0,NULL,NULL,'11/15/2007',NULL,NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 34, 4, 4,'/store/cart.aspx','Shopping Cart','Shopping Cart', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 35, 4, 4,'/store/checkout.aspx','Store Checkout','Store Checkout', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 36, 4, 4,'/store/billing.aspx','Checkout - Billing information','Checkout - Billing information', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 37, 4, 4,'/store/shipping.aspx','Checkout - Shipping information','Checkout - Shipping information', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 38, 4, 4,'/store/payment.aspx','Checkout - Payment information','Checkout - Payment information', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007',NULL,NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 39, 4, 4,'/store/confirm.aspx','Checkout - Order Confirmation','Checkout - Order Confirmation', 1, 1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','11/15/2007', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 66, 2, 1,'/404.aspx','Page Not Found','Page Not Found', 1, 1,'Site Startup Module Meta Description','Site Startup Module Meta Keywords','Jan 30 2008  3:15PM', 1,2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 64,	2, 4,'/store/search/default.aspx','StoreStartup Search','StoreStartup Search',1,1,'Site Startup Module Meta Description','Site Startup Module Meta Keywords','1/24/2008 3:04:46 PM',NULL,NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 76, 2, 11, NULL, NULL, NULL, 0, 0, NULL, NULL, '4/16/2008 2:00:50 PM', NULL, NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 77, 4, 11, NULL, NULL, NULL, 0, 0, NULL, NULL, '4/16/2008 2:12:56 PM', NULL, NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 78, 2, 11, '/service/default.aspx','Customer Service','Customer Service',1,1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','4/16/2008 2:18:03 PM',NULL,NULL);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 79, 2, 11, '/service/faq.aspx','FAQ''s','FAQ''s',1,1,'Store Startup Module Meta Description','Store Startup Module Meta Keywords','4/16/2008 7:53:08 PM',1,2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 80, 2, 11, '/service/contact.aspx', 'Contact Us', 'Contact Us', 1, 1, 'Store Startup Module Meta Description', 'Store Startup Module Meta Keywords', '4/17/2008 7:08:09 AM', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 81, 2, 11, '/service/order.aspx', 'Order Status', 'Order Status', 1, 1, 'Store Startup Module Meta Description', 'Store Startup Module Meta Keywords', '4/17/2008 7:14:39 AM', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 85, 2, 11, '/service/thankyou.aspx', 'Contact Us Confirmation - Thank You', 'Contact Us Confirmation - Thank You', 1, 1, 'Store Startup Module Meta Description', 'Store Startup Module Meta Keywords', '4/18/2008 4:58:34 PM', 1, 2);
insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ( 103, 2, 11, '/service/quickorder.aspx', 'Quick Order', 'Quick Order', 1, 1, 'Store Startup Module Meta Description', 'Store Startup Module Meta Keywords', '4/18/2008 4:58:34 PM', 1, 2);
SET IDENTITY_INSERT ContentToolPage OFF;

SET IDENTITY_INSERT ContentToolPageRegion ON;

insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 6, 24);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 6, 25);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 6, 26);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 6, 27);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 7, 28);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 7, 29);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 7, 30);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 7, 31);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 4, 32);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 4, 33);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 4, 34);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 4, 35);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 8, 40);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 8, 41);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 8, 42);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 8, 43);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 9, 44);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 9, 45);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 9, 46);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 9, 47);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 10, 48);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 10, 49);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 10, 50);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 10, 51);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 11, 52);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 11, 53);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 11, 54);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 11, 55);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 12, 56);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 12, 57);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 12, 58);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 12, 59);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 13, 60);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 13, 61);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 13, 62);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 13, 63);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 14, 64);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 14, 65);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 14, 66);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 14, 67);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 15, 68);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 15, 69);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 15, 70);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 15, 71);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 16, 72);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 16, 73);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 16, 74);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 16, 75);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 17, 88);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 17, 89);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Main', 17, 90);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 17, 91);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 18, 92);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 18, 93);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 18, 94);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 18, 95);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 19, 100);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 19, 101);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Main', 19, 102);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 19, 103);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 20, 104);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 20, 105);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 20, 106);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 20, 107);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_AdSection', 22, 158);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 22, 159);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 22, 160);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Main', 22, 161);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 22, 162);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 24, 181);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 24, 182);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 24, 183);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 24, 184);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 26, 189);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 26, 190);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 26, 191);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 26, 192);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 27, 201);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 27, 202);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 27, 203);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 27, 204);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Footer', 1, 208);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Header', 1, 209);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 1, 210);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 28, 251);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 28, 252);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 28, 253);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 28, 254);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 25, 271);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 25, 272);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 25, 273);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 25, 274);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Footer', 5, 281);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Header', 5, 282);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Left', 5, 283);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 5, 284);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 5, 285);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 30, 301);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 30, 302);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left', 30, 303);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 30, 304);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 30, 305);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 31, 306);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 31, 307);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left', 31, 308);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 31, 309);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 31, 310);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 34, 311);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 34, 312);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 34, 313);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 34, 314);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Footer', 32, 315);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Header', 32, 316);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 32, 317);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 32, 318);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 35, 319);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 35, 320);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 35, 321);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 35, 322);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 36, 323);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 36, 324);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 36, 325);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 36, 326);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 37, 327);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 37, 328);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 37, 329);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 37, 330);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 39, 331);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 39, 332);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 39, 333);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 39, 334);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 40, 338);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 40, 339);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 40, 340);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 40, 341);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 41, 342);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 41, 343);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left', 41, 344);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 41, 345);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 41, 346);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 43, 352);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 43, 353);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left', 43, 354);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 43, 355);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 43, 356);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 23, 357);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 23, 358);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left', 23, 359);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 23, 360);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 23, 361);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 44, 372);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 44, 373);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left', 44, 374);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 44, 375);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 44, 376);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 29, 377);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 29, 378);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left', 29, 379);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Main', 29, 380);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 29, 381);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 3, 382);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 3, 383);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Main', 3, 384);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 45, 385);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 45, 386);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left', 45, 387);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 45, 388);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 45, 389);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 46, 390);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 46, 391);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main', 46, 392);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 46, 393);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 66, 719);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header', 66, 720);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left', 66, 721);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Main', 66, 722);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top', 66, 723);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer',76,775);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header',76,776);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Left',76,777);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main',76,778);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top',76,779);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer',77,780);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header',77,781);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Left',77,782);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main',77,783);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Right',77,784);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top',77,785);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer',78,786);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header',78,787);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left',78,788);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Main',78,789);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top',78,790);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer',79,796);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header',79,797);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left',79,798);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main',79,799);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top',79,800);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer',80,811);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header',80,812);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left',80,813);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Custom','CT_Main',80,814);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top',80,815);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer', 81, 816);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default', 'CT_Header', 81, 817);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default', 'CT_Left', 81, 818);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default', 'CT_Main', 81, 819);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default', 'CT_Top', 81, 820);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default', 'CT_Footer', 85, 870);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default', 'CT_Header', 85, 871);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default', 'CT_Left', 85, 872);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default', 'CT_Main', 85, 873);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default', 'CT_Top', 85, 874);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Footer',103,946);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Header',103,947);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Left',103,948);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Main',103,949);
insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values ('Default','CT_Top',103,950);
SET IDENTITY_INSERT ContentToolPageRegion OFF;

SET IDENTITY_INSERT ContentToolRegionModule ON;

insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 15, 24, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 16, 25, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 17, 28, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 18, 29, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 19, 32, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 20, 33, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 23, 40, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 24, 41, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 25, 44, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 26, 45, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 27, 48, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 28, 49, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 29, 52, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 30, 53, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 31, 56, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 32, 57, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 33, 60, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 34, 61, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 35, 64, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 36, 65, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 37, 68, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 38, 69, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 39, 72, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 40, 73, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 48, 88, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 49, 89, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 50, 92, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 51, 93, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 54, 100, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 55, 101, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 57, 104, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 58, 105, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 91, 159, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 92, 160, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 102, 181, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 103, 182, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 106, 189, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 107, 190, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 112, 201, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 113, 202, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 116, 208, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 117, 209, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 138, 251, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 139, 252, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 148, 271, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 149, 272, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 155, 281, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 156, 282, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 157, 283, 14, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 167, 301, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 168, 302, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 169, 303, 14, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 170, 306, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 171, 307, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 172, 308, 14, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 173, 315, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 174, 316, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 175, 319, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 176, 320, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 177, 323, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 178, 324, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 179, 327, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 180, 328, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 181, 331, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 182, 332, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 185, 338, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 186, 339, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 187, 342, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 188, 343, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 189, 344, 14, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 193, 352, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 194, 353, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 195, 354, 14, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 196, 357, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 197, 358, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 198, 359, 14, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 206, 372, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 207, 373, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 208, 374, 14, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 209, 377, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 210, 378, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 211, 379, 14, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 213, 382, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 214, 383, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 216, 385, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 217, 386, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 218, 387, 14, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 219, 390, 5, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 220, 391, 4, 1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 453, 719, 5, 1, NULL );
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 454, 720, 4, 1, NULL );
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 455, 721, 14, 1, NULL );
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 456, 722, 1, 1, '6');
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 457, 722, 17,  2,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 504, 775, 5, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 505, 776, 4, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 507, 777, 14, 2, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 506, 777, 19, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 508, 780, 5, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 509, 781, 4, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 511, 782, 14, 2, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 510, 782, 19, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 512, 784, 16, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 513, 786, 5, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 514, 787, 4, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 516, 788, 14, 2, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 515, 788, 19, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 517, 789, 1, 1, 20);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 523, 796, 5, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 524, 797, 4, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 526, 798, 14, 2, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 525, 798, 19, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 538, 811, 5, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 539, 812, 4, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 541, 813, 14, 2, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 540, 813, 19, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 543, 814, 20, 2, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 544, 816, 5, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 545, 817, 4, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 547, 818, 14, 2, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 546, 818, 19, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 589, 870, 5, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 590, 871, 4, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 592, 872, 14, 2, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 591, 872, 19, 1, NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 593, 873, 1, 1, 26);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 645,946,5,1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 646,947,4,1,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 648,948,14,2,NULL);
insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ( 647,948,19,1,NULL);
SET IDENTITY_INSERT ContentToolRegionModule OFF;


SET IDENTITY_INSERT ContentToolContent ON;
insert into ContentToolContent (ContentId,[Content]) values (26,'Thank you!');
SET IDENTITY_INSERT ContentToolContent OFF;


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[AdminNavigation]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[AdminNavigation](
	[AdminNavigationId] [int] IDENTITY(1,1) NOT NULL,
	[ParentId] [int] NULL,
	[Image] [varchar](50) NULL,
	[AltTag] [varchar](50) NULL,
	[Name] [varchar](50) NOT NULL,
	[SortOrder] [int] NOT NULL,
	[IsSelectedPath] [varchar](max) NULL,
	[RedirectURL] [varchar](255) NULL,
	[AdminSection] [varchar](50) NULL,
	[IsInternal] [bit] NOT NULL CONSTRAINT [DF_AdminNavigation_IsInternal]  DEFAULT ((0)),
	[IsBranch] [bit] NOT NULL CONSTRAINT [DF_AdminNavigation_IsBranch]  DEFAULT ((0)),
	[IsBreak] [bit] NOT NULL CONSTRAINT [DF_AdminNavigation_IsBreak]  DEFAULT ((0)),
 CONSTRAINT [PK_AdminNavigation] PRIMARY KEY CLUSTERED 
(
	[AdminNavigationId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
END
GO


IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ExcludedSearchWords]') AND type in (N'U'))
BEGIN
CREATE TABLE [dbo].[ExcludedSearchWords](
 [ExcludeSearchWordId] [int] IDENTITY(1,1) NOT NULL,
 [ExcludeSearchWord] [varchar](50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
 CONSTRAINT [PK_ExcludedSearchWords] PRIMARY KEY CLUSTERED 
(
 [ExcludeSearchWordId] ASC
)WITH (PAD_INDEX  = OFF, IGNORE_DUP_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
END
GO


-- DEFAULT DATA FOR LEFT NAV AND BREADCRUMB --

ALTER TABLE [AdminNavigation] NOCHECK CONSTRAINT ALL
SET IDENTITY_INSERT [AdminNavigation] ON
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (1,NULL,'system_parameters.gif','System Parameters','System Parameters',9999,'/admin/settings/','/admin/settings/','USERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (3,NULL,NULL,NULL,'Break',9998,NULL,NULL,'USERS',0,0,1)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (4,NULL,'orders.gif','Orders','Orders',1,'/admin/store/orders/,/admin/store/payments/,/admin/store/text/edit.aspx?Code=PackingListFooter,/admin/store/text/edit.aspx?Code=OrderConfirmation',NULL,'ORDERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (5,4,NULL,NULL,'View Orders',1,'/admin/store/orders/','/admin/store/orders/','ORDERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (6,4,NULL,NULL,'Payment Log',2,'/admin/store/payments/','/admin/store/payments/','ORDERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (7,4,NULL,NULL,'Packing List Footer Text',3,'/admin/store/text/edit.aspx?Code=PackingListFooter','/admin/store/text/edit.aspx?Code=PackingListFooter','ORDERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (8,4,NULL,NULL,'Order Confirmation Text',4,'/admin/store/text/edit.aspx?Code=OrderConfirmation','/admin/store/text/edit.aspx?Code=OrderConfirmation','ORDERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (9,NULL,'reporting.gif','Reports','Reports',2,'/admin/store/sales/,/admin/store/promotions/promotionreports.aspx,/admin/store/search/',NULL,'REPORTS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (10,9,NULL,NULL,'Sales Report',1,'/admin/store/sales/','/admin/store/sales/','REPORTS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (11,9,NULL,NULL,'Promotional Code Report',2,'/admin/store/promotions/promotionreports.aspx','/admin/store/promotions/promotionreports.aspx','REPORTS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (12,9,NULL,NULL,'Search Term Report',3,'/admin/store/search/','/admin/store/search/','REPORTS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (13,NULL,'members.gif','Members','Members',3,'/admin/members/','/admin/members/','MEMBERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (14,NULL,'Banners.gif','Banners','Banners',4,'/admin/banners/',NULL,'BANNERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (15,14,NULL,NULL,'Banners',1,'/admin/banners/','/admin/banners/','BANNERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (16,14,NULL,NULL,'Add New Banner',2,'/admin/banners/edit.aspx','/admin/banners/edit.aspx','BANNERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (17,14,NULL,NULL,'Summary Report',3,'/admin/banners/report.aspx','/admin/banners/report.aspx','BANNERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (18,14,NULL,NULL,'Daily Report',4,'/admin/banners/daily.aspx','/admin/banners/daily.aspx','BANNERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (19,14,NULL,NULL,'Banner Groups',5,'/admin/banners/groups/','/admin/banners/groups/','BANNERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (20,14,NULL,NULL,'Add New Group',6,'/admin/banners/groups/edit.aspx','/admin/banners/groups/edit.aspx','BANNERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (21,NULL,'marketing_tools.gif','Marketing Tools','Marketing Tools',5,'/admin/store/promotions/,/admin/store/referrals/,/admin/store/giftmessage/,/admin/store/howheard/,/admin/store/features/,/admin/content/edit.aspx?PageURL=/404.aspx',NULL,'MARKETING_TOOLS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (22,21,NULL,NULL,'Promotions',1,'/admin/store/promotions/','/admin/store/promotions/','MARKETING_TOOLS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (23,21,NULL,NULL,'Referrals',2,'/admin/store/referrals/','/admin/store/referrals/','MARKETING_TOOLS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (24,21,NULL,NULL,'Gift Messages',3,'/admin/store/giftmessage/','/admin/store/giftmessage/','MARKETING_TOOLS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (25,21,NULL,NULL,'Discovery Sources',4,'/admin/store/howheard/','/admin/store/howheard/','MARKETING_TOOLS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (26,21,NULL,NULL,'Item Features',5,'/admin/store/features/','/admin/store/features/','MARKETING_TOOLS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (27,21,NULL,NULL,'Customize 404 Page',6,'/admin/content/pages/default.aspx?PageURL=/404.aspx','/admin/content/pages/default.aspx?PageURL=/404.aspx','MARKETING_TOOLS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (28,NULL,'broadcast.gif','Broadcast','Broadcast',6,'/admin/broadcast/',NULL,'BROADCAST',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (29,28,NULL,NULL,'Templates',1,'/admin/broadcast/templates/','/admin/broadcast/templates/','BROADCAST',1,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (30,28,NULL,NULL,'Lists',2,'/admin/broadcast/lists/','/admin/broadcast/lists/','BROADCAST',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (31,28,NULL,NULL,'Subscribers',3,'/admin/broadcast/subscribers/','/admin/broadcast/subscribers/','BROADCAST',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (32,28,NULL,NULL,'Recipient Groups',4,'/admin/broadcast/groups/','/admin/broadcast/groups/','BROADCAST',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (33,28,NULL,NULL,'Emails',5,'/admin/broadcast/','/admin/broadcast/','BROADCAST',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (34,NULL,'store.gif','Store','Store',7,'/admin/store/template,/admin/store/departments/,/admin/store/brands/,/admin/store/items/,/admin/store/search/exclude/,/admin/store/swatch/',NULL,'STORE',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (35,34,NULL,'Item Templates','Item Templates',1,'/admin/store/template/','/admin/store/template/','STORE',1,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (36,34,NULL,NULL,'Departments',2,'/admin/store/departments/','/admin/store/departments/','STORE',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (37,34,NULL,NULL,'Brands',3,'/admin/store/brands/','/admin/store/brands/','STORE',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (38,34,NULL,NULL,'Items',4,'/admin/store/items/','/admin/store/items/','STORE',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (39,34,NULL,NULL,'Excluded Search Terms',5,'/admin/store/search/exclude/','/admin/store/search/exclude/','STORE',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (40,34,NULL,NULL,'Swatches',6,'/admin/store/swatch/','/admin/store/swatch/','STORE',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (41,NULL,'Shipping_tax.gif','Shipping & Tax','Shipping & Tax',8,'/admin/store/shipping/,/admin/store/countries/,/admin/store/states/',NULL,'SHIPPING_TAX',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (42,41,NULL,NULL,'Shipping Calculation',1,'/admin/store/shipping/shippingparam.aspx','/admin/store/shipping/shippingparam.aspx','SHIPPING_TAX',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (43,41,NULL,NULL,'Shipping Methods',2,'/admin/store/shipping/','/admin/store/shipping/','SHIPPING_TAX',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (44,41,NULL,NULL,'Country Shipping',3,'/admin/store/countries/','/admin/store/countries/','SHIPPING_TAX',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (45,41,NULL,NULL,'State Tax',4,'/admin/store/states/','/admin/store/states/','SHIPPING_TAX',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (46,NULL,'content_tool.gif','Content Tool','Content Tool',9,'/admin/content/,/admin/help/','/admin/content/','CONTENT_TOOL',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (47,46,NULL,NULL,'Pages',1,'/admin/content/pages/','/admin/content/pages/','CONTENT_TOOL',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (48,46,NULL,NULL,'Sections',2,'/admin/content/sections/','/admin/content/sections/','CONTENT_TOOL',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (49,46,NULL,NULL,'Navigation',3,'/admin/content/navigation/','/admin/content/navigation/','CONTENT_TOOL',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (50,46,NULL,NULL,'Modules',4,'/admin/content/modules/','/admin/content/modules/','CONTENT_TOOL',1,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (51,46,NULL,NULL,'Help Messages',5,'/admin/help/','/admin/help/','CONTENT_TOOL',1,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (52,46,NULL,NULL,'Templates',6,'/admin/content/templates','/admin/content/templates','CONTENT_TOOL',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (53,NULL,'users.gif','Users','Users',12,'/admin/admins/',NULL,'USERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (54,53,NULL,NULL,'Admin Users',1,'/admin/admins/','/admin/admins/','USERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (55,53,NULL,NULL,'Add New Admin',2,'/admin/admins/edit.aspx','/admin/admins/edit.aspx','USERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (56,53,NULL,NULL,'Admin Groups',3,'/admin/admins/groups/','/admin/admins/groups/','USERS',0,0,0)
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (57,53,NULL,NULL,'Login Activity',4,'/admin/admins/activity.aspx','/admin/admins/activity.aspx','USERS',0,0,0)

INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (58,NULL,'faq.gif','FAQ''s','FAQ''s',10,'/admin/faq/,/admin/faq/category/',NULL,'FAQ',0,0,0);
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (59,58,NULL,NULL,'FAQ''s',1,'/admin/faq/','/admin/faq/','FAQ',0,0,0);
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (60,58,NULL,NULL,'FAQ Categories',2,'/admin/faq/category/','/admin/faq/category/','FAQ',0,0,0);
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (68, 61, NULL, NULL, 'Edit Thank You Page', 3, '/admin/content/edit.aspx?PageId=85', '/admin/content/edit.aspx?PageId=85', 'CONTACT_US', 0, 0, 0);
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (61,NULL,'contact.gif','Contact Us','Contact Us',11,'/admin/contactus/',NULL,'CONTACT_US',0,0,0);
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (62,61,NULL,NULL,'Contact Us Submissions',1,'/admin/contactus/','/admin/contactus/','CONTACT_US',0,0,0);
INSERT INTO [AdminNavigation] ([AdminNavigationId],[ParentId],[Image],[AltTag],[Name],[SortOrder],[IsSelectedPath],[RedirectURL],[AdminSection],[IsInternal],[IsBranch],[IsBreak]) VALUES (63,61,NULL,NULL,'Contact Us Questions',2,'/admin/contactus/question/','/admin/contactus/question/','CONTACT_US',0,0,0);

SET IDENTITY_INSERT [AdminNavigation] OFF
ALTER TABLE [AdminNavigation] CHECK CONSTRAINT ALL


-- enable full text search
exec sp_fulltext_database 'enable' 
exec sp_fulltext_catalog 'StoreSearchIndex', 'create' 
exec sp_fulltext_table @tabname='StoreItem', @action='create', @ftcat='StoreSearchIndex', @keyname='PK_StoreItem'

-- Add the fields you want to add to the full text search
exec sp_fulltext_column @tabname='StoreItem', @colname='SKU', @action='add'
exec sp_fulltext_column @tabname='StoreItem', @colname='LongDescription', @action='add'
exec sp_fulltext_column @tabname='StoreItem', @colname='ItemName', @action='add'

ALTER FULLTEXT INDEX ON [StoreItem] SET CHANGE_TRACKING AUTO




