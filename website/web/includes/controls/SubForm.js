Type.registerNamespace("AE");

AE.SubForm = function(element) {
    AE.SubForm.initializeBase(this, [element]);

    this._template = [];
    this._fields = null;
    this._templateKey = null;
    this._initRows = null;
    this._rowCount = 0;
    this._fieldCount = 0;
    this._blurInterval = 50;
    this._initData = null;
    this._showAddBtn = false;

    this._hidden = document.createElement('input');
    this._hidden.type = 'hidden';
    this._hidden.id = element.id + '_hdn';
    this._hidden.name = element.name;

    this._pbBtns = null;
}

function SubFormPostbackFocusEvent(e) {
    window.FocusIsPostback = true;
}
function SubFormPostbackBlurEvent(e) {
    window.FocusIsPostback = false;
}

AE.SubForm.prototype = {

    initialize: function() {
        AE.SubForm.callBaseMethod(this, 'initialize');

        this._blurDel = Function.createDelegate(this, this._blur);
        this._focusDel = Function.createDelegate(this, this._focus);
        this._blurredDel = Function.createDelegate(this, this._blurred);
        this._keyDownDel = Function.createDelegate(this, this._keyDown);
        this._deleteRowDel = Function.createDelegate(this, this._deleteRow);
        this._addRowDel = Function.createDelegate(this, this._addRow);

        //init fields
        var el = this.get_element();
        if (this._fields && this._fields.length > 0) {
            var tr = el.insertRow(0);
            for (var i = 0; i < this._fields.length; i++) {
                var c = $find(this._fields[i]);
                if (c) {
                    this._template.push(c);
                    var th = document.createElement('th')
                    th.innerHTML = c.get_fieldName();
                    tr.appendChild(th);
                }
            }
        }
        if (this._initData) {
            for (var i = 0; i < this._initData.length; i++) {
                this._addRowWithData(true, this._initData[i]);
            }
            this._addRow(true);
        } else {
            for (var i = 0; i < this._initRows; i++) {
                this._addRow(true);
            }
        }
        if (this._showAddBtn) {
            var btn = document.createElement('input');
            btn.className = 'btnred';
            btn.type = 'button';
            btn.value = 'Add Row';
            btn.onclick = 'return false';
            el.parentNode.insertAfter(el, btn);
            $addHandler(btn, 'click', this._addRowDel);
        }
        if (this._pbBtns) {
            var ids = this._pbBtns.split(",");
            for (var i = 0; i < ids.length; i++) {
                var ctl = $get(ids[i]);
                if (ctl) {
                    $addHandler(ctl, 'focus', SubFormPostbackFocusEvent);
                    $addHandler(ctl, 'blur', SubFormPostbackBlurEvent);
                }
            }
        }
        $addHandler(el, 'keydown', this._keyDownDel);
    },

    get_errorRowClass: function() {
        return this._errorRowClass;
    },
    set_errorRowClass: function(value) {
        this._errorRowClass = value;
    },
    get_field: function(i) {
        return this._template[i];
    },
    set_field: function(value) {
        this._template.push(value);
    },
    get_initRows: function() {
        return this._initRows;
    },
    set_initRows: function(value) {
        this._initRows = value;
    },
    get_fields: function() {
        return this._fields;
    },
    set_fields: function(value) {
        this._fields = value;
    },
    get_OnRowAdded: function() {
        return this.get_events().getHandler('RowAdded');
    },
    set_OnRowAdded: function(value) {
        this.add_RowAdded(value);
    },
    get_initData: function() {
        return this._initData;
    },
    set_initData: function(value) {
        this._initData = value;
    },
    get_pbBtns: function() {
        return this._pbBtns;
    },
    set_pbBtns: function(value) {
        this._pbBtns = value;
    },
    get_showAddBtn: function() {
        return this._showAddBtn;
    },
    set_showAddBtn: function(value) {
        this._showAddBtn = value;
    },

    add_RowAdded: function(handler) {
        this.get_events().addHandler('RowAdded', handler);
    },
    remove_RowAdded: function(handler) {
        this.get_events().removeHandler('RowAdded', handler);
    },

    get_value: function(Row, FieldName) {
        var el = this.get_element();
        var field;
        for (var i = 0; i < this._template.length; i++) {
            if (this._template[i].get_fieldName() == FieldName) {
                field = this._template[i];
                break;
            }
        }
        if (field) {
            var inputs = field.get_inputs();
            var out = '';
            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].isDataField) {
                    var id = inputs[i].id.replace(field._idRegex, el.rows[Row].cells[i].id);
                    var input = $get(id);
                    if (input.value) {
                        if ((input.type != 'radio' && input.type != 'checkbox') || input.checked) {
                            out += input.value;
                        }
                    } else if (input.selectedIndex && input.options) {
                        out += input.options[input.selectedIndex];
                    }
                }
            }
            return out;
        }
        return null;
    },

    _addRowWithData: function(init, data) {
        var el = this.get_element();
        var tr = el.insertRow(-1);
        tr.id = el.id + '_TR' + this._rowCount;
        tr.name = el.id + '$TR' + this._rowCount;
        this._rowCount++;
        var firstElement = document.forms[0].elements.length;
        for (var f = 0; f < this._template.length; f++) {
            var td = tr.insertCell(-1);
            td.id = tr.id + '_TD' + f;
            td.name = tr.name + '$TD' + f;
            this._fieldCount++;
            if (data) {
                this._template[f].instantiateWithData(td, this._blurDel, this._focusDel, data[this._template[f].get_fieldName()]);
            } else {
                this._template[f].instantiateIn(td, this._blurDel, this._focusDel);
            }
        }
        //delete button
        var td = document.createElement('td');
        var img = document.createElement('img');
        img.src = '/images/admin/delete.gif';
        img.alt = 'Delete Row';
        img.style.cursor = 'pointer';
        $addHandler(img, 'click', this._deleteRowDel);
        tr.appendChild(td);
        td.appendChild(img);

        if (!init && tr.cells[0].focus) tr.cells[0].focus();
        var handler = this.get_events().getHandler('RowAdded');
        if (handler) {
            handler(this, Sys.EventArgs.Empty);
        }
    },
    _addRow: function(init) {
        this._addRowWithData(init, null);
    },

    _deleteRow: function(e) {
        var row = e.target.parentNode.parentNode;
        var el = this.get_element();
        for (var i = 0; i < el.rows.length; i++) {
            if (el.rows[i] === row) {
                if (!this._isEmptyRow(el.rows[i])) {
                    el.deleteRow(i);
                }
                return;
            }
        }
    },

    _isEmptyRow: function(row) {
        for (var i = 0; i < row.cells.length; i++) {
            if (row.cells[i].isEmpty && !row.cells[i].isEmpty()) {
                return false;
            }
        }
        return true;
    },

    _blur: function(e) {
        this._curField = null;
        if (this._blurTimeout) {
            window.clearTimeout(this._blurTimeout);
        }
        if (e.target && e.target.field) {
            this._oldField = e.target.field;
        }
        this._blurTimeout = window.setTimeout(this._blurredDel, this._blurInterval);
        return true;
    },

    _focus: function(e) {
        if (e.target && e.target.field) {
            this._curField = e.target.field;
        }
        return true;
    },

    _blurred: function() {
        if (window.FocusIsPostback) return;
        var bError = false;
        var sError = '';
        if (this._oldField === this._curField) return;
        if (this._oldField && (!this._curField || this._curField !== this._oldField)) {
            var tr = this._oldField.parentNode;
            //go to length-1 to skip delete cell
            //first pass check to make sure row isn't blank/default
            var isDefault = true;
            for (var i = 0; i < tr.cells.length - 1; i++) {
                isDefault &= tr.cells[i].isEmpty();
            }
            if (!isDefault) {
                for (var i = 0; i < tr.cells.length - 1; i++) {
                    var td = tr.cells[i];
                    bError |= td.validate && !td.validate();
                    //if (bError && td.focus && parseInt(/tr([\d]+)/i.exec(tr.id)[1]) < this._rowCount - 1) td.focus();
                    if (this._curField && tr.cells[i] === this._oldField) break;
                }
                if (!this._curField) {
                    if (!bError) {
                        tr.className = this._validRowClass;
                        if (parseInt(/tr([\d]+)/i.exec(tr.id)[1]) == this._rowCount - 1 && !this._showAddBtn)
                            this._addRow(false);
                    } else {
                        tr.className = this._errorRowClass;
                    }
                }
            }
        }
    },

    _keyDown: function(e) {
        if (e.rawEvent) e = e.rawEvent;
        if (e.keyCode && (e.keyCode == 13 || e.keyCode == 11)) {
            if (this._curField) {
                var tr = this._curField.parentNode;
                var bEmpty = true;
                var i;
                for (i = 0; i < tr.cells.length - 1; i++) {
                    if (tr.cells[i].isEmpty && !tr.cells[i].isEmpty()) {
                        bEmpty = false;
                    }
                }
                var rowNumber = parseInt(/tr([\d]+)/i.exec(tr.id)[1]);
                var cellNumber = parseInt(/td([\d]+)/i.exec(this._curField.id)[1]);
                if (bEmpty && rowNumber == this._rowCount - 1) {
                    return true;
                } else {
                    if (cellNumber < tr.cells.length - 2) {
                        if (tr.cells[cellNumber + 1].focus) tr.cells[cellNumber + 1].focus;
                    } else {
                        if (rowNumber == this._rowCount - 1) {
                            this._addRow();
                        } else {
                            var td = this.get_element().rows[rowNumber + 1].cells[cellNumber];
                            td.focus();
                        }
                    }
                    return false;
                }
            }
        }
        return true;
    }
}

AE.SubForm.registerClass('AE.SubForm', Sys.UI.Control);

AE.SubFormTemplate = function() {
    this._html='';
    this._idPrefix='';
    this._namePrefix='';
    this._inputs=[];
    this._validators=[];
    this._defaults=[];
    this._validClass='';
    this._invalidClass='';
    this._fieldName='';
}

AE.SubFormTemplate.prototype = {
    initialize: function() {
        AE.SubFormTemplate.callBaseMethod(this, 'initialize');

        if (this._idPrefix != '') {
            this._idRegex = new RegExp(RegExp.escape(this._idPrefix), 'g');
        }
        if (this._namePrefix != '') {
            this._nameRegex = new RegExp(RegExp.escape(this._namePrefix), 'g');
        }
    },
    get_fieldName: function() {
        return this._fieldName;
    },
    set_fieldName: function(value) {
        this._fieldName = value;
    },
    get_html: function() {
        return this._html;
    },
    set_html: function(value) {
        this._html = value;
    },
    get_inputs: function() {
        return this._inputs;
    },
    set_inputs: function(value) {
        this._inputs = value;
    },
    get_validClass: function() {
        return this._validClass;
    },
    set_validClass: function(value) {
        this._valueClass = value;
    },
    get_invalidClass: function() {
        return this._invalidClass;
    },
    set_invalidClass: function(value) {
        this._invalidClass = value;
    },
    get_idPrefix: function() {
        return this._idPrefix;
    },
    set_idPrefix: function(value) {
        this._idPrefix = value;
    },
    get_namePrefix: function() {
        return this._namePrefix;
    },
    set_namePrefix: function(value) {
        this._namePrefix = value;
    },

    instantiateWithData: function(container, onBlur, onFocus, data) {
        var cellId = container.id;
        var cellName = container.name;
        var html = this._html.replace(this._idRegex, cellId);
        html = html.replace(this._nameRegex, cellName);
        container.innerHTML = html;
        container.invalidClass = this._invalidClass;
        container.validClass = this._validClass;
        var focusSet = false;
        for (var i = 0; i < this._inputs.length; i++) {
            var newId = this._inputs[i].id.replace(this._idRegex, cellId);
            var el = $get(newId);
            if (el.focus && !focusSet) {
                container.focus = el.focus;
                focusSet = true;
            }
            if (this._inputs[i].validate) {
                if (!container.validators) {
                    container.validators = [];
                }
                container.validators.push({ 'validate': Function.createDelegate(el, this._inputs[i].validate), 'errorSpan': this._inputs[i].errorSpan });
                if(this._inputs[i].errorSpan) $get(cellId + '_' + this._inputs[i].errorSpan).style.display = 'none';
            }
            if (this._inputs[i].isDefault) {
                if (!container.defaults) {
                    container.defaults = [];
                }
                container.defaults.push(Function.createDelegate(el, this._inputs[i].isDefault));
            } else {
                if (!container.defaults) {
                    container.defaults = [];
                }
                container.defaults.push(Function.createDelegate(el, this._defaultIsEmpty));
            }
            if (this._inputs[i].isDataField && this._inputs[i].setValue && data) {
                el.setValue = this._inputs[i].setValue;
                el.setValue(data);
            }
            if (this._inputs[i].create) {
                var create = this._inputs[i].create;
                create = create.replace(this._idRegex, cellId);
                create = create.replace(this._nameRegex, cellName);
                eval(create);
            }
            el.field = container;
            $addHandler(el, 'blur', onBlur);
            $addHandler(el, 'focus', onFocus);
        }
        container.validate = Function.createDelegate(container, this.validate);
        container.isEmpty = Function.createDelegate(container, this.isEmpty);
    },

    instantiateIn: function(container, onBlur, onFocus) {
        this.instantiateWithData(container, onBlur, onFocus, null);
    },

    validate: function() {

        var bValid = true;
        for (v in this.validators) {
            if (!this.validators[v].validate()) {
                bValid = false;
                this.className = this.invalidClass;
                if (this.validators[v].errorSpan) {
                    $get(this.id + '_' + this.validators[v].errorSpan).style.display = '';
                }
            } else {
                this.className = this.validClass;
                if (this.validators[v].errorSpan) {
                    $get(this.id + '_' + this.validators[v].errorSpan).style.display = 'none';
                }
            }
        }
        return bValid;
    },

    isEmpty: function() {
        var bEmpty = true;
        for (i in this.defaults) {
            if (!this.defaults[i]()) {
                bEmpty = false;
            }
        }
        return bEmpty;
    },

    _defaultIsEmpty: function() {
        return isEmptyField(this);
    }
}

AE.SubFormTemplate.registerClass('AE.SubFormTemplate', Sys.Component);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();


