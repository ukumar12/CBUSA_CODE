
//global event function
function SubFormFieldBlurred(e) {
    e=e?e:window.event;
    var rowid=/(row_[\d]+)([\D]|$)/.exec(e.id);
    var elementId=e.id.substr(0,e.id.indexOf(rowid)-1);
    var behaviors = Sys.UI.Behavior.getBehaviorsByType($get(elementId),FourSale.EditTable);
    if(!behaviors || behaviors.length==0) return;
    var b=behaviors[0];
    b.curRow=null
    b.oldRow=rowid;
    if(b.blurTimeout) {
        window.clearTimeout(b.blurTimeout);
    }
    b.blurTimeout=window.setTimeout(b.DelayedBlurDelegate,b.DelayedBlurInterval);
}
function SubFormFieldFocus(e) {
    var t=e.target?e.target:e.srcElement;
    var rowid=/(row_[\d]+)([\D]|$)/.exec(t.id);
    var elementId=t.id.substr(0,t.id.indexOf(rowid)-1);
    var behaviors = Sys.UI.Behavior.getBehaviorsByType($get(elementId),FourSale.EditTable);
    if(!behaviors || behaviors.length==0) return;
    var b=behaviors[0];
    b.curRow=rowid;
}


Type.registerNamespace("FourSale");

FourSale.EditTable = function(element) {
    FourSale.EditTable.initializeBase(this, [element]);

    this._template='';
    this._templateKey=null;
    this._initRows=null;
    this.curRow=null;
    this.oldRow=null;
}

FourSale.EditTable.prototype = {

    initialize: function() {
        FourSale.EditTable.callBaseMethod(this, 'initialize');
        
        for(var i=0;i<this._initRows;i++) {
            this.addRow();
        }
    },
    get_template: function() {
        return this._template;
    },
    set_template: function(value) {
        this._template=value;
    },
    get_templateKey: function() {
        return this._templateKey;
    },
    set_templateKey: function(value) {
        this._templateKey=value;
    },
    get_initRows: function() {  
        return this._initRows;
    },
    set_initRows: function(value) {
        this._initRows = value;
    },
    
    addRow: function() {
        var hdn=this.get_element();
        var i=0;
        while(document.forms[0].elements[i] !== hdn && i < document.forms[0].elements.length) {
            i++;
        }
        while(document.forms[0].elements[i] && document.forms[0].elements[i].id.indexOf(hdn.id) >= 0) {
            i++;
        }
        var tbl=$get(hdn.id+'_table');
        var id='row'+ tbl.rows.length;
        var tr=tbl.insertRow(-1);
        tr.firstIndex=i;
        var r=new RegExp(/<td[\s]*((?:[^\s=>]*)=(?:[^=>]*))*>/);
        //var r=new RegExp(/<td[\s]*((?:[^\s=>]*)=(?:[^=>]*))*>((?<!<\/td>)\w*)<\/td>/);
        var m=r.exec(this._template);
        for(var i=1;i<m.length;i++) {
            alert('val='+m.value);
        }
        var txt=this._template.replace(/[\s]{2,}/g,'');
        txt=txt.replace(/\'/g,'"');
        txt=txt.replace(r,id);
        //tr.innerHTML=txt;
        tr.innerHTML='<td>Test</td>';
        tr.id=hdn.id+id;
        this.focusElement(tr);
    },
    
    focusElement: function(el) {
        if(el.focus) {
            el.focus();
            return true;
        } else {
            for(var i=0;i<el.childNodes.length;i++) {
                if(focusElement(el.childNodes[i])) {
                    return true;
                }
            }
            return false;
        }
    },
                 
    _onRowBlurred: function() {
        if(this.oldRow && (!this.curRow || this.oldRow != this.curRow)) {
            var tr=$get(this.oldRow);
            var isValid=true;
            var i=0;
            var el=document.forms[0].elements[tr.firstIndex];
            while(el && el.id.indexOf(tr.id) >= 0) {
                var r=new RegExp(tr.id+'_([^_]*)');
                var name=r.exec(el.id)[1];
            }
            /*
            for(var i=0;i<this._validators.length;i++) {
                var el=document.forms[0].elements[tr.firstIndex+i];
                if(this._validators
            }
            if(!tr.validate()) {
                tr.className=this._errorClass;
            } else {
                tr.className=this._normalClass;
                this.addRow();
            }
            */
        }       
    }
}

FourSale.EditTable.registerClass('FourSale.EditTable', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();


