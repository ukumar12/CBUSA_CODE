Type.registerNamespace("AE");

AE.AutoComplete = function(element) {
    AE.AutoComplete.initializeBase(this, [element]);

    this._window = null;
    this._className = 'aclist';
    this._b = null;
    this._waitTimer = null;
    this._waitInterval = null;
    this._request = null;
    this._selectedIndex = null;
    this._oldSelectedIndex = null;
    this._minLength = 0;
    this._allowNew = null;
    this._table = null;
    this._textField = null;
    this._valueField = null;
    this._hdn = null;
    this._searchFunction = null;
    this._autopostback = null;
    this._whereClause = null;
    this._onClientUpdate = null;
    this._onTextChanged = null;
    this._maxResults = 0;
    this._isComplete = false;
    this._hideList = false;
}

AE.AutoComplete.prototype = {

    initialize: function() {
        AE.AutoComplete.callBaseMethod(this, 'initialize');

        var el = this.get_element();

        var wrap = document.createElement('div');
        wrap.style.width = (el.offsetWidth ? el.offsetWidth : el.clientWidth) + 'px';
        wrap.style.height = (el.offsetHeight ? el.offsetHeight : el.clientHeight) + 'px';
        wrap.style.padding = '0px';
        wrap.style.margin = '0px';
        wrap.style.backgroundColor = 'transparent';
        wrap.style.position = 'relative';
        el.parentNode.replaceChild(wrap, el);
        wrap.appendChild(el);

        this._window = document.createElement('ul');
        this._window.className = this._className;
        this._window.style.visibility = 'hidden';
        wrap.appendChild(this._window);
        this._window.style.top = wrap.clientHeight + 'px';
        this._window.style.left = '0px';
        this._window.style.right = '0px';
        this._b = $create(AE.DivWindow, { 'moveTop': false, 'showVeil': false }, null, null, this._window);

        this._updateListDelegate = Function.createDelegate(this, this._updateList);
        this._updateListCbDelegate = Function.createDelegate(this, this._updateListCb);
        this._callServiceDelegate = Function.createDelegate(this, this._callService);
        this._listMouseOverDelegate = Function.createDelegate(this, this._listMouseOver);
        this._selectListItemDelegate = Function.createDelegate(this, this._selectListItem);
        this._onKeyPressDelegate = Function.createDelegate(this, this._onKeyPress);
        this._onKeyDownDelegate = Function.createDelegate(this, this._onKeyDown);
        this._openListDelegate = Function.createDelegate(this, this._openList);
        this._closeListDelegate = Function.createDelegate(this, this._closeList);

        $addHandler(el, 'click', this._openListDelegate);
        $addHandler(el, 'blur', this._selectListItemDelegate);
        $addHandler(el, 'keyup', this._onKeyPressDelegate);
        $addHandler(el, 'keydown', this._onKeyDownDelegate);

        if (!this._allowNew) {
            this._isInitCall = true;
            this._callService();
        }
    },
    get_minLength: function() {
        return this._minLength;
    },
    set_minLength: function(value) {
        this._minLength = value;
    },
    get_waitInterval: function() {
        return this._waitInterval;
    },
    set_waitInterval: function(value) {
        this._waitInterval = value;
    },
    get_behavior: function() {
        return this._b;
    },
    get_table: function() {
        return this._table;
    },
    set_table: function(value) {
        this._table = value;
    },
    get_textField: function() {
        return this._textField;
    },
    set_textField: function(value) {
        this._textField = value;
    },
    get_valueField: function() {
        return this._valueField;
    },
    set_valueField: function(value) {
        this._valueField = value;
    },
    get_allowNew: function() {
        return this._allowNew;
    },
    set_allowNew: function(value) {
        this._allowNew = value;
    },
    get_hdn: function() {
        return this._hdn;
    },
    set_hdn: function(value) {
        this._hdn = value;
    },
    get_className: function() {
        return this._className;
    },
    set_className: function(value) {
        this._className = value;
    },
    get_searchFunction: function() {
        return this._searchFunction;
    },
    set_searchFunction: function(value) {
        this._searchFunction = value;
    },
    get_autopostback: function() {
        return this._autopostback;
    },
    set_autopostback: function(value) {
        this._autopostback = value;
    },
    get_whereClause: function() {
        return this._whereClause;
    },
    set_whereClause: function(value) {
        this._whereClause = value;
    },
    get_onClientUpdate: function() {
        return this._onClientUpdate;
    },
    set_onClientUpdate: function(value) {
        this._onClientUpdate = value;
    },
    get_onTextChanged: function() {
        return this._onTextChanged;
    },
    set_onTextChanged: function(value) {
        this._onTextChanged = value;
    },
    get_maxResults: function() {
        return this._maxResults;
    },
    set_maxResults: function(value) {
        this._maxResults = value;
    },
    get_hideList: function() {
        return this._hideList;
    },
    set_hideList: function(value) {
        this._hideList = value;
    },
    get_value: function() {
        return this._hdn.value;
    },

    /* 'Public' functions */
    GetCurrentList: function() {
        var el = this.get_behavior().get_element();
        var out = [];
        for (var i = 0; i < el.childNodes.length; i++) {
            out.push({ 'text': el.childNodes[i].innerHTML, 'value': el.childNodes[i].dataValue });
        }
        return out;
    },

    /* 'Private' functions */
    _openList: function() {
        this._updateList(true);
    },

    _closeList: function() {
        var b = this.get_behavior();
        b.startFadeOut();
    },

    _updateList: function(forceRefresh) {
        if (this._request) {
            var ex = this._request.get_executor();
            ex.abort();
        } else if (this._waitTimer) {
            window.clearTimeout(this._waitTimer);
            this._waitTimer = null;
        }
        if (!forceRefresh && this._isComplete) {
            var el = this.get_element()
            var isNew = !this._filterResults(el.value);
            if (!this._allowNew && isNew) {
                el.value = el.value.substring(1, el.value.length - 1);
            }
            if (this._onTextChanged) {
                eval(this._onTextChanged);
            }
            var b = this.get_behavior();
            if (this.get_element().value.length >= this._minLength && b.get_element().hasChildNodes() && !this._hideList) {
                b.startFadeIn();
            } else {
                b.startFadeOut();
            }
        } else {
            this._waitTimer = window.setTimeout(this._callServiceDelegate, this._waitInterval);
        }
    },

    _callService: function() {
        if (this._request) {
            var ex = this._request.get_executor();
            ex.abort();
        } else if (this._waitTimer) {
            window.clearTimeout(this._waitTimer);
        }
        this._waitTimer = null;
        var params = { 'Table': this._table, 'TextField': this._textField, 'ValueField': this._valueField, 'Text': this.get_element().value, 'MaxResults': this._maxResults };
        var fname = this._searchFunction ? this._searchFunction : 'GetAutoCompleteList';
        if (this._whereClause) {
            if (!this._searchFunction) fname = 'GetFilteredACList';
            params.WhereClause = this._whereClause;
        }
        this._request = Sys.Net.WebServiceProxy.invoke('/includes/controls/AutoComplete.asmx', fname, false, params, this._updateListCbDelegate, this._updateListCbDelegate);
    },

    _updateListCb: function(res, ctxt) {
        if (res.get_exceptionType) return;
        res = Sys.Serialization.JavaScriptSerializer.deserialize(res);
        this._request = null;
        this._isComplete = res.isComplete;
        var b = this.get_behavior();
        var el = b.get_element();
        while (el.hasChildNodes()) {
            $clearHandlers(el.lastChild);
            el.removeChild(el.lastChild);
        }
        for (var i = 0; i < res.items.length; i++) {
            var li = document.createElement('li');
            li.idx = i;
            li.id = el.id + i;
            li.className = 'acitem';
            li.innerHTML = res.items[i].text;
            li.dataValue = res.items[i].value;
            $addHandler(li, 'click', this._selectListItemDelegate);
            $addHandler(li, 'mouseover', this._listMouseOverDelegate);
            $addHandler(li, 'mouseout', this._listMouseOut);
            el.appendChild(li);
        }
        this._selectedIndex = this._oldSelectedIndex = null;
        if (this._isInitCall) {
            this._isInitCall = null;
            return;
        }
        var txt = this.get_element();
        if (txt.value.length > 0 && !this._allowNew && !this._filterResults(txt.value)) {
            while (txt.value.length > 0 && !this._filterResults(txt.value)) {
                txt.value = txt.value.length == 1 ? '' : txt.value.substring(1, txt.value.length - 1);
            }
            if (txt.value.length == 0) this._isComplete = false;
        }
        if (this._onTextChanged) {
            eval(this._onTextChanged);
        }
        if (txt.value.length >= this._minLength && el.hasChildNodes() && !this._hideList) {
            b.startFadeIn();
        } else {
            b.startFadeOut();
        }
    },

    _filterResults: function(txt) {
        var el = this.get_behavior().get_element();
        var temp = [];
        for (var i = 0; i < el.childNodes.length; i++) {
            if (el.childNodes[i].innerHTML.toLowerCase().indexOf(txt.toLowerCase()) == 0) {
                temp.push(el.childNodes[i]);
            }
        }
        if (temp.length > 0) {
            var idx = 0;
            while (temp.length > 0) {
                while (el.childNodes[idx] !== temp[0]) {
                    el.removeChild(el.childNodes[idx]);
                }
                temp.shift();
                idx++;
            }
            while (el.childNodes.length > idx) {
                el.removeChild(el.lastChild);
            }
            return true;
        } else {
            return false;
        }
    },

    _onKeyDown: function(e) {
        var list = this.get_behavior().get_element();
        var txt = this.get_element();
        var bSelected = false;
        switch (e.keyCode) {
            case 9:
            case 13:
                if (this._selectedIndex !== null) {
                    txt.value = list.childNodes[this._selectedIndex].innerHTML;
                    this._selectListItemDelegate();
                    bSelected = true;
                    e.preventDefault();
                } else if (!this._allowNew) {
                    if (list.childNodes.length > 0) {
                        txt.value = list.childNodes[0].innerHTML;
                        this._selectListItemDelegate();
                        bSelected = true;
                    } else {
                        e.preventDefault();
                    }
                }
                break;
            case 38:
                if (typeof (this._selectedIndex) != 'undefined' && this._selectedIndex > 0) this._selectedIndex--; else this._selectedIndex = 0;
                break;
            case 40:
                if (this._selectedIndex === null)
                    this._selectedIndex = 0;
                else if (this._selectedIndex < list.childNodes.length - 1)
                    this._selectedIndex++;
                else
                    this._selectedIndex = list.childNodes.length - 1;
                break;
            default:
                //always allow backspace
                if (e.keyCode != 8 && !this._allowNew) {
                    if (e.keyCode >= 32 && e.keyCode < 127) {
                        var newText = txt.value + String.fromCharCode(e.keyCode);
                        var bAllow = false;
                        if (!this._isComplete) {
                            bAllow = true;
                        } else {
                            for (var i = 0; i < list.childNodes.length; i++) {
                                if (list.childNodes[i].innerHTML.toLowerCase().indexOf(newText.toLowerCase()) == 0) {
                                    bAllow = true;
                                    break;
                                }
                            }
                        }
                        if (!bAllow) {
                            return false;
                        }
                    } else {
                        e.preventDefault();
                    }
                }
                break;

        }
        if (e.keyCode == 38 || e.keyCode == 40) {
            if (this._oldSelectedIndex != null && this._selectedIndex == this.oldSelectedIndex) return;
            if (this._oldSelectedIndex != null && this._oldSelectedIndex < list.childNodes.length) list.childNodes[this._oldSelectedIndex].style.backgroundColor = '';
            list.childNodes[this._selectedIndex].style.backgroundColor = '#ccc';
            this._oldSelectedIndex = this._selectedIndex;
            if (this._hdn && list.childNodes[this._selectedIndex].dataValue) {
                this._hdn.value = list.childNodes[this._selectedIndex].dataValue;
            }
        }
    },

    _onKeyPress: function(e) {
        if (e.keyCode != 9 && e.keyCode != 13 && e.keyCode != 38 && e.keyCode != 40) {
            this._updateList(e.keyCode == 8);
        }
    },

    _selectListItem: function(e) {
        var txt = this.get_element();
        var b = this.get_behavior();
        var sel = null;
        var list = b.get_element();
        if (txt.value == '' && this._selectedIndex === null) return;
        if (e && e.target.tagName.toLowerCase() == 'li') {
            sel = e.target;
        } else {
            if (!this._allowNew) {
                if (this._selectedIndex === null && txt.value.length > 0) {
                    for (var i = 0; i < list.childNodes.length; i++) {
                        if (list.childNodes[i].innerHTML.toLowerCase().indexOf(txt.value.toLowerCase()) >= 0) {
                            this._selectedIndex = i;
                            break;
                        }
                    }
                }
                if (this._selectedIndex !== null) {
                    sel = list.childNodes[this._selectedIndex];
                }
            }
        }
        if (txt) {
            if (sel) {
                txt.value = unescapeHTML(sel.innerHTML);
                if (sel.dataValue) {
                    this._hdn.value = sel.dataValue;
                }
            }
            if (this._onSelect) eval(this._onSelect);
            b.startFadeOut();
        }
        if (this._onClientUpdate) {
            eval(this._onClientUpdate);
        }
        if (this._autopostback) {
            this._autopostback();
        }
    },

    _listMouseOver: function(e) {
        var list = this.get_behavior().get_element();
        var txt = this.get_element();
        if (this._oldSelectedIndex) list.childNodes[this._oldSelectedIndex].style.backgroundColor = '';
        e.target.style.backgroundColor = '#ccc';
        this._selectedIndex = e.target.idx;
    },
    _listMouseOut: function(e) {
        e.target.style.backgroundColor = '';
    }
}

AE.AutoComplete.registerClass('AE.AutoComplete', Sys.UI.Control);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();


