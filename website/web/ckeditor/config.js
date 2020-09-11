/*
Copyright (c) 2003-2010, CKSource - Frederico Knabben. All rights reserved.
For licensing, see LICENSE.html or http://ckeditor.com/license
*/

CKEDITOR.editorConfig = function( config )
{
	// Define changes to default configuration here. For example:
	// config.language = 'fr';
	// config.uiColor = '#AADC6E';
	
	config.extraPlugins = 'ultimatespell,w3cvalidation,508validation,youtube';
	config.removePlugins = 'scayt,autogrow';
	config.toolbar = 'admin';
	config.resize_enabled = true;
	config.width = 600;
	config.height = 200;
	config.toolbar_admin = [
		['Source','DocProps','-','Templates','flash','508validation', 'w3cvalidation'],
		['Cut','Copy','Paste','PasteText','PasteWord','-','Print','UltimateSpell'],
		['Undo','Redo'],['Find','Replace'],['SelectAll','RemoveFormat'],
		['Bold','Italic','Underline','Strike','-','Subscript','Superscript'],
		['NumberedList','BulletedList','-','Outdent','Indent'],
		['JustifyLeft','JustifyCenter','JustifyRight','JustifyBlock'],
		['youtube','Flash','Iframe'],
		['Link','Unlink','Anchor','Image','Table','Rule','TextColor','BGColor'],
		['Styles','Format','Font','About']
	];

	config.contentsCss = ['/cms/includes/style.css','/cms/includes/store.css'];

	// Asset Manager
	
	config.skin = 'kama',
	
	//config.filebrowserBrowseUrl = '../../../includes/CKAssetTreeBrowser.aspx?AssetType=Files';
	config.filebrowserBrowseUrl = '/fckeditor/editor/filemanager/browser/default/browser.html?Type=Flash&Connector=' + encodeURIComponent('/fckeditor/editor/filemanager/browser/default/connectors/aspx/connector.aspx');
	config.filebrowserWindowWidth = 858;
	config.filebrowserWindowHeight = 740;
	
	//config.filebrowserImageBrowseUrl = '../../../includes/CKAssetTreeBrowser.aspx?AssetType=Images';
	config.filebrowserImageBrowseUrl = '/fckeditor/editor/filemanager/browser/default/browser.html?Type=Image&Connector=' + encodeURIComponent('/fckeditor/editor/filemanager/browser/default/connectors/aspx/connector.aspx');
	config.filebrowserImageWindowWidth = 858;
	config.filebrowserImageWindowHeight = 740;
	
	//config.filebrowserImageBrowseLinkUrl = '../../../includes/CKAssetTreeBrowser.aspx?AssetType=Files';
	config.filebrowserImageBrowseLinkUrl = '/fckeditor/editor/filemanager/browser/default/browser.html?Connector=' + encodeURIComponent('/fckeditor/editor/filemanager/browser/default/connectors/aspx/connector.aspx');
	
	config.fileBrowserWindowFeatures = 'resizable=no,modal=yes,menubar=no';
};
