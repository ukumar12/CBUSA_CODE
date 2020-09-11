SELECT     'insert into ContentToolPage (PageId,TemplateId,SectionId, PageURL, [Name], Title, IsIndexed, IsFollowed, MetaDescription, MetaKeywords, ModifyDate, NavigationId, SubNavigationId) values ('
                       + STR(PageId) + ',' + STR(TemplateId) + ',' + COALESCE(STR(SectionId), 'NULL') + ',''' + COALESCE (PageURL, 'NULL') + ''',''' + COALESCE (Name, 
                      'NULL') + ''',''' + COALESCE (Title, 'NULL') + ''',' + STR(IsIndexed) + ',' + STR(IsFollowed) + ',''' + COALESCE (MetaDescription, 'NULL') 
                      + ''',''' + COALESCE (MetaKeywords, 'NULL') + ''',''' + CAST(GETDATE() AS varchar(50)) + ''',' + COALESCE (STR(NavigationId), 'NULL') 
                      + ',' + COALESCE (STR(SubNavigationId), 'NULL') + ');' AS record
FROM         ContentToolPage


SELECT    'insert into ContentToolPageRegion (RegionType, ContentRegion, PageId, PageRegionId) values (''' 
+ RegionType + ''',''' + cast(ContentRegion as varchar(50)) + ''',' + str(PageId)+ ',' + str(PageRegionId) + ');'
FROM         ContentToolPageRegion

SELECT    'insert into ContentToolRegionModule (RegionModuleId, PageRegionId, ModuleId, SortOrder, Args) values ('
+ str(RegionModuleId) + ',' +  str(PageRegionId) + ',' +  str(ModuleId) + ',' + str(SortOrder) + ',''' +  coalesce(Args,'NULL') + ''');'
FROM         ContentToolRegionModule