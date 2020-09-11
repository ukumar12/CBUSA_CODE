CKEDITOR.plugins.add('ultimatespell',
{
    init: function(editor)
    {
        var pluginName = 'ultimatespell';
        CKEDITOR.dialog.add(pluginName, this.path + 'ultimatespell/plugin.js');
        
        editor.addCommand(pluginName, {
        	exec: function(editor) {
        		var text = editor.getData();
        		if (text.indexOf('type="application/x-shockwave-flash"') > -1) {
        			alert('Unable to do spell-check if Flash elements are added to the editor.')
        		} else {
        			UltimateSpellClick(editor.element.getId().replace('ccEditor','UltimateSpell'));
        		}        		
        	}
        });
        
        editor.ui.addButton('UltimateSpell',
            {
                label: 'Spell Check',
                command: pluginName
            });
    }
});