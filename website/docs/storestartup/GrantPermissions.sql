DECLARE @IIS_USER VARCHAR(50)

--User to grant permissions to
SET @IIS_USER = 'studiofortyone-iis-653' 

EXEC('GRANT EXECUTE ON [dbo].[sp_GetAttributeTree] TO [' + @IIS_USER + ']')
EXEC('GRANT EXECUTE ON [dbo].[sp_GetAttributeTreeTableLayout] TO [' + @IIS_USER + ']')
EXEC('GRANT EXECUTE ON [dbo].[sp_GetRecentlyViewedItems] TO [' + @IIS_USER + ']')
EXEC('GRANT EXECUTE ON [dbo].[sp_GetTemplateAttributeTree] TO [' + @IIS_USER + ']')
EXEC('GRANT EXECUTE ON [dbo].[sp_GetTemplateAttributeTreeByTemplate] TO [' + @IIS_USER + ']')
EXEC('GRANT EXECUTE ON [dbo].[sp_MailingRemoveDuplicates] TO [' + @IIS_USER + ']')
EXEC('GRANT EXECUTE ON [dbo].[sp_StoreDepartmentDelete] TO [' + @IIS_USER + ']')
EXEC('GRANT EXECUTE ON [dbo].[sp_StoreDepartmentInsert] TO [' + @IIS_USER + ']')
EXEC('GRANT EXECUTE ON [dbo].[sp_StoreDepartmentMove] TO [' + @IIS_USER + ']')
EXEC('GRANT EXECUTE ON [dbo].[sp_StoreDepartmentRename] TO [' + @IIS_USER + ']')
EXEC('GRANT EXECUTE ON [dbo].[sp_ValidateSKU] TO [' + @IIS_USER + ']')
EXEC('GRANT SELECT ON [dbo].[fn_SearchBuildSKU] TO [' + @IIS_USER + ']')
