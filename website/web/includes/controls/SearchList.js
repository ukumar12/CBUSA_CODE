Type.registerNamespace("AE");

AE.SearchList = function(element) {
    AE.SearchList.initializeBase(this, [element]);

    this._div = null;
    this._list = null;
    this._wrapper = null;
    this._hdn = null;

    this._waitTimer = null;
    this._waitInterval = 0;
    this._request = null;
    this._selectedIndex = null;
    this._oldSelectedIndex = null;
    this._minLength = 0;
    this._allowNew = null;
    this._table = null;
    this._textField = null;
    this._valueField = null;
    this._searchFunction = null;
    this._autopostback = null;
    this._whereClause = null;
    this._onClientUpdate = null;
    this._onTextChanged = null;
    this._viewAllLength = 10;

    this._isViewAll = false;
    this._lastRequestText = null;
    this._lastViewAll = false;

    this._defaultHeight = 15;
}

AE.SearchList.prototype = {

    initialize: function() {
        AE.SearchList.callBaseMethod(this, 'initialize');

        var el = this.get_element();

        this._updateListDelegate = Function.createDelegate(this, this._updateList);
        this._updateListCbDelegate = Function.createDelegate(this, this._updateListCb);
        this._callServiceDelegate = Function.createDelegate(this, this._callService);
        this._listMouseOverDelegate = Function.createDelegate(this, this._listMouseOver);
        this._selectListItemDelegate = Function.createDelegate(this, this._selectListItem);
        this._onKeyPressDelegate = Function.createDelegate(this, this._onKeyPress);
        this._onKeyDownDelegate = Function.createDelegate(this, this._onKeyDown);
        this._openListDelegate = Function.createDelegate(this, this._openList);
        this._closeListDelegate = Function.createDelegate(this, this._closeList);
        this._viewAllDelegate = Function.createDelegate(this, this._viewAll);
        this._onBlurDelegate = Function.createDelegate(this, this._onBlur);
        this._clearBlurDelegate = Function.createDelegate(this, this._clearBlur);
        this._onClickDelegate = Function.createDelegate(this, this._onClick);
        this._clearDelegate = Function.createDelegate(this, this._clear);
        this._listOpenedDelegate = Function.createDelegate(this, this._listOpened);

        $addHandler(el, 'mousedown', this._openListDelegate);
        $addHandler(el, 'blur', this._onBlurDelegate);
        $addHandler(el, 'focus', this._clearBlurDelegate);
        $addHandler(el, 'keyup', this._onKeyPressDelegate);
        $addHandler(el, 'keydown', this._onKeyDownDelegate);
        $addHandler(this._div, 'scroll', this._clearBlurDelegate);

        this._div.style.display = 'none';
        this._div.tabIndex = -1;
        $addHandler(this._div, 'focus', this._clearBlurDelegate);
        $addHandler(this._div, 'blur', this._onBlurDelegate);
    },
    get_minLength: function() {
        return this._minLength;
    },
    set_minLength: function(value) {
        this._minLength = value;
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
    get_list: function() {
        return this._list;
    },
    set_list: function(value) {
        this._list = value;
    },
    get_wrapper: function() {
        return this._wrapper;
    },
    set_wrapper: function(value) {
        this._wrapper = value;
    },
    get_div: function() {
        return this._div;
    },
    set_div: function(value) {
        this._div = value;
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
    get_viewAllLength: function() {
        return this._viewAllLength;
    },
    set_viewAllLength: function(value) {
        this._viewAllLength = value;
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

    _onClick: function(e) {
        if (this._blurTimer) {
            window.clearTimeout(this._blurTimer);
        }
        this._blurTimer = null;
        if (e.target.NoSelect) {
            this._viewAll();
        } else {
            this._selectListItem(e);
        }
        return false;
    },

    _handleFocus: function(e) {
        if (this._blurTimer) {
            window.clearTimeout(this._blurTimer);
        }
        this._blurTimer = null;
    },

    _onBlur: function(e) {
        e.preventDefault();
        if (this._blurTimer) {
            window.clearTimeout(this._blurTimer);
        }
        this._blurTimer = window.setTimeout(this._selectListItemDelegate, 100);

        //return true;

    },

    _clearBlur: function() {
        if (this._blurTimer) {
            window.clearTimeout(this._blurTimer);
        }
        this._blurTimer = null;
        return true;
    },

    _openList: function() {
        this._updateList();
        return false;
    },

    _closeList: function() {
        $(this._div).slideUp('fast', null);
    },

    _showList: function() {
        if (this._div.style.display == 'none') {
            $(this._div).slideDown('fast', this._listOpenedDelegate);
        }
    },

    _hideList: function() {
        if (this._div.style.display != 'none') {
            $(this._div).slideUp('fast', null);
        }
    },

    _listOpened: function() {
        this._div.style.overflowY = this._isViewAll ? 'auto' : '';
    },

    _updateList: function() {
        if (this._lastViewAll == this._isViewAll && this.get_element().value == this._lastRequestText) {
            this._showList();
            return;
        }
        if (this._request) {
            var ex = this._request.get_executor();
            ex.abort();
        } else if (this._waitTimer) {
            window.clearTimeout(this._waitTimer);
            this._waitTimer = null;
        }
        this._waitTimer = window.setTimeout(this._callServiceDelegate, this._waitInterval);
    },

    _callService: function() {
        if (this._request) {
            var ex = this._request.get_executor();
            ex.abort();
        } else if (this._waitTimer) {
            window.clearTimeout(this._waitTimer);
        }
        this._waitTimer = null;
        var maxResults = this._isViewAll ? 0 : this._viewAllLength;
        var params = { 'Table': this._table, 'TextField': this._textField, 'ValueField': this._valueField, 'Text': this.get_element().value, 'MaxResults': maxResults };
        var fname = this._searchFunction ? this._searchFunction : 'GetSearchList';
        if (this._whereClause) {
            if (!this._searchFunction) fname = 'GetFilteredSearchList';
            params.WhereClause = this._whereClause;
        }
        this._request = Sys.Net.WebServiceProxy.invoke('/includes/controls/SearchList.asmx', fname, false, params, this._updateListCbDelegate, this._updateListCbDelegate);
        this._lastRequestText = params.Text;
        this._lastViewAll = this._isViewAll;
    },

    _updateListCb: function(res, ctxt) {
        if (res.get_exceptionType) return;
        res = Sys.Serialization.JavaScriptSerializer.deserialize(res);
        this._request = null;
        while (this._list.hasChildNodes()) {
            $clearHandlers(this._list.lastChild);
            this._list.removeChild(this._list.lastChild);
        }
        if (res.count == 0) {
            var li = document.createElement('li');
            li.style.textAlign = 'center';
            li.innerHTML = '<em class="smallest">No results found</em>';
            li.NoSelect = true;
            this._list.appendChild(li);
        } else {
            var pageHeight = 0;
            if (!this._viewAllLength || res.count <= this._viewAllLength) {
                this._isViewAll = true;
            }
            for (var i = 0; i < res.items.length; i++) {
                var li = document.createElement('li');
                li.idx = i;
                li.id = this._list.id + i;
                li.innerHTML = res.items[i].text;
                li.dataValue = res.items[i].value;
                $addHandler(li, 'click', this._onClickDelegate);
                //$addHandler(li, 'click', this._selectListItemDelegate);
                //$addHandler(li, 'mousedown', this._selectListItemDelegate);
                $addHandler(li, 'mouseover', this._listMouseOverDelegate);
                $addHandler(li, 'mouseout', this._listMouseOut);
                this._list.appendChild(li);
                if (!this._viewAllLength || i < this._viewAllLength) {
                    pageHeight += li.clientHeight ? li.clientHeight + 10 : li.offsetHeight ? li.offsetHeight : 30;
                }
            }
            this._div.style.overflowX = 'hidden';
            if (this._isViewAll) {
                this._div.style.overflowY = 'auto';
                this._div.style.height = pageHeight + 'px';
            } else {
                this._div.style.overflowY = '';
                this._div.style.height = '';
            }
            if (res.count > this._viewAllLength && !this._isViewAll) {
                var li = document.createElement('li');
                li.style.textAlign = 'center';
                li.className = 'bold smallest';
                li.innerHTML = 'Showing top ' + res.items.length + ' of ' + res.count + ' results.<br/>Keep typing to narrow your search, or Click Here to View All';
                li.NoSelect = true;
                $addHandler(li, 'click', this._onClickDelegate);
                //$addHandler(li, 'click', this._viewAllDelegate);
                //$addHandler(li, 'mousedown', this._viewAllDelegate);
                $addHandler(li, 'mouseover', this._listMouseOverDelegate);
                $addHandler(li, 'mouseout', this._listMouseOut);
                this._list.appendChild(li);
            }
        }
        this._selectedIndex = this._oldSelectedIndex = null;

        var txt = this.get_element();
        if (this._onTextChanged) {
            eval(this._onTextChanged);
        }

        if (txt.value.length > 0) {
            var li = document.createElement('li');
            var h = this._div.style.height;
            if (h) {
                h = 25 + parseInt(h);
                this._div.style.height = h + 'px';
            }
            li.style.textAlign = 'center';
            li.className = 'bold smallest';
            li.style.fontStyle = 'italic';
            li.innerHTML = 'Clear Selected';
            li.NoSelect = true;
            $addHandler(li, 'click', this._clearDelegate);
            $addHandler(li, 'mouseover', this._listMouseOverDelegate);
            $addHandler(li, 'mouseout', this._listMouseOut);
            this._list.appendChild(li);
        }

        if (txt.value.length >= this._minLength && this._list.hasChildNodes()) {
            this._showList();
        } else {
            this._hideList();
        }
    },

    _onKeyDown: function(e) {
        var list = this._list;
        var txt = this.get_element();
        var bSelected = false;
        switch (e.keyCode) {
            case 9:
            case 13:
                if (this._selectedIndex !== null) {
                    txt.value = list.childNodes[this._selectedIndex].innerHTML;
                    this._selectListItem(true);
                    bSelected = true;
                    if(e.keyCode == 13) e.preventDefault();
                } else if (!this._allowNew) {
                    if (list.childNodes.length > 0) {
                        txt.value = list.childNodes[0].innerHTML;
                        this._selectListItem(true);
                        bSelected = true;
                    } else {
                        if(e.keyCode == 13) e.preventDefault();
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
                break;

        }
        if (e.keyCode == 38 || e.keyCode == 40) {
            if (this._oldSelectedIndex != null && this._selectedIndex == this.oldSelectedIndex) return;
            if (this._oldSelectedIndex != null && this._oldSelectedIndex < list.childNodes.length) list.childNodes[this._oldSelectedIndex].style.backgroundColor = '';
            list.childNodes[this._selectedIndex].style.backgroundColor = '#ccc';
            this._oldSelectedIndex = this._selectedIndex;
            /*
            if (this._hdn && list.childNodes[this._selectedIndex].dataValue) {
            this._hdn.value = list.childNodes[this._selectedIndex].dataValue;
            }
            */
        }
        if (list.childNodes[this._selectedIndex] && list.childNodes[this._selectedIndex].offsetHeight > 0) {
            var listOffset = list.childNodes[this._selectedIndex].offsetTop;
            while (listOffset < this._div.scrollTop) {
                this._div.scrollTop -= list.childNodes[this._selectedIndex].offsetHeight;
            }
            while (listOffset >= (this._div.clientHeight + this._div.scrollTop)) {
                this._div.scrollTop += list.childNodes[this._selectedIndex].offsetHeight;
            }
        }
    },

    _viewAll: function() {
        this._isViewAll = true;
        this._updateList();
    },

    _onKeyPress: function(e) {
        if (e.keyCode != 9 && e.keyCode != 13 && e.keyCode != 38 && e.keyCode != 40) {
            this._updateList();
        }
    },

    _selectListItem: function(e) {
        var txt = this.get_element();
        var sel = null;
        var list = this._list;
		if (txt == null) return;
        if (txt.value == null && this._selectedIndex === null) return;
        if (e && e.target && e.target.tagName.toLowerCase() == 'li' && e.target.id.indexOf(list.id) >= 0) {
            sel = e.target;
        } else {
            if (txt.value != '') {
                if (!this._allowNew) {
                    if (this._selectedIndex === null && this._list.childNodes.length > 0) {
                        this._selectedIndex = 0;
                    }
                    if (this._selectedIndex !== null) {
                        sel = list.childNodes[this._selectedIndex];
                    }
                }
            }
        }
        if (sel && sel.NoSelect) {
            return;
        }
        this._hideList();
        if (txt.value == '' && !sel) {
            return;
        }
        if (txt) {
            if (sel) {
                txt.value = unescapeHTML(sel.innerHTML);
                if (sel.dataValue) {
                    this._hdn.value = sel.dataValue;
                } else {
                    this._hdn.value = null;
                }
            }
            if (this._onSelect) eval(this._onSelect);
        }
        if (!this._allowNew && !this._hdn.value) {
            txt.value = '';
        }
        if (this._onClientUpdate) {
            eval(this._onClientUpdate);
        }
        if (this._autopostback) {
            this._autopostback();
        }
    },

    _listMouseOver: function(e) {
        var list = this._list;
        var txt = this.get_element();
        if (this._oldSelectedIndex) list.childNodes[this._oldSelectedIndex].style.backgroundColor = '';
        e.target.style.backgroundColor = '#ccc';
        this._selectedIndex = e.target.idx;
    },
    _listMouseOut: function(e) {
        e.target.style.backgroundColor = '';
    },

    _clear: function() {
        this._clearBlur();
        this._selectedIndex = null;
        this.get_element().value = '';
        this._hdn.value = '';
        this._closeList();
        if (this._onClientUpdate) {
            eval(this._onClientUpdate);
        }
        if (this._autopostback) {
            this._autopostback();
        }
    },

    dispose: function() {
        var el = this.get_element();
        $clearHandlers(el);
        for (var i = 0; i < this._list.childNodes.length; i++) {
            $clearHandlers(this._list.childNodes[i]);
        }
        if (this._waitTimer) {
            window.clearTimeout(this._waitTimer);
        }
        if (this._blurTimer) {
            window.clearTimeout(this._blurTimer);
        }
        if (this._request) {
            var ex = this._request.get_executor();
            ex.abort();
        }
        AE.SearchList.callBaseMethod(this, 'dispose');
    }
}

AE.SearchList.registerClass('AE.SearchList', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();