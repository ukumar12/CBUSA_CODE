CKEDITOR.plugins.add('w3cvalidation',
{
    init: function(editor)
    {
        var pluginName = 'w3cvalidation';
        CKEDITOR.dialog.add(pluginName, this.path + 'dialogs/w3cvalidation.js');
        
        editor.addCommand(pluginName, new CKEDITOR.dialogCommand(pluginName));
        
        editor.ui.addButton('w3cvalidation',
            {
                label: 'HTML Validation',
                command: pluginName,
                icon: CKEDITOR.plugins.getPath('w3cvalidation') + '/images/icon-w3c.gif'
            });
    }
});