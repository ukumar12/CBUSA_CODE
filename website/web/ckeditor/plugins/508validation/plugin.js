CKEDITOR.plugins.add('508validation',
{
    init: function(editor)
    {
        var pluginName = '508validation';
        CKEDITOR.dialog.add(pluginName, this.path + 'dialogs/508validation.js');
        
        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));
        
        editor.ui.addButton('508validation',
            {
                label: '508 Validation',
                command: pluginName,
                icon: CKEDITOR.plugins.getPath('508validation') + '/images/icon-508-6.gif'
            });
    }
});