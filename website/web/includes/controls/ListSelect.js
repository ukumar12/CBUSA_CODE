Type.registerNamespace("AE");

AE.ListSelect = function(element) {
    AE.ListSelect.initializeBase(this, [element]);

    this._onList = null;
    this._offList = null;
    this._selectLimit = null;
    this._pbReference = null;
    this._addImageUrl = null;
    this._deleteImageUrl = null;
    this._onCnt = 0;
}

AE.ListSelect.prototype = {

    initialize: function() {
        AE.ListSelect.callBaseMethod(this, 'initialize');

        this._onListClickDel = Function.createDelegate(this, this._onListClick);
        this._offListClickDel = Function.createDelegate(this, this._offListClick);

        var el = this.get_element();
        this._selectedValues = el.value;
        this._onList = $get(el.id + '_onList');
        this._offList = $get(el.id + '_offList');
        for (var i = 0; i < this._onList.childNodes.length; i++) {
            if (this._onList.childNodes[i].nodeName.toLowerCase() == 'li') {
                $addHandler(this._onList.childNodes[i], 'click', this._onListClickDel);
                this._onCnt++;
            }
        }
        for (var i = 0; i < this._offList.childNodes.length; i++) {
            if (this._offList.childNodes[i].nodeName.toLowerCase() == 'li') {
                this._offList.childNodes[i].SortOrder = i;
                $addHandler(this._offList.childNodes[i], 'click', this._offListClickDel);
            }
        }
    },

    //properties
    get_onList: function() {
        return this._onList;
    },
    set_onList: function(value) {
        this._onList = value;
    },
    get_offList: function() {
        return this._offList;
    },
    set_offList: function(value) {
        this._offList = value;
    },
    get_selectedValues: function() {
        return this.get_element().value;
    },
    set_selectedValues: function(value) {
        this.get_element().value = value;
    },
    get_selectLimit: function() {
        return this._selectLimit;
    },
    set_selectLimit: function(value) {
        this._selectLimit = value;
    },
    get_pbReference: function() {
        return this._pbReference;
    },
    set_pbReference: function(value) {
        this._pbReference = value;
    },
    get_addImageUrl: function() {
        return this._addImageUrl;
    },
    set_addImageUrl: function(value) {
        this._addImageUrl = value;
    },
    get_deleteImageUrl: function() {
        return this._deleteImageUrl;
    },
    set_deleteImageUrl: function(value) {
        this._deleteImageUrl = value;
    },

    //events
    add_listclick: function(handler) {
        this.get_events().addHandler('listclick', handler);
    },
    remove_listclick: function(handler) {
        this.get_events().removeHandler('listclick', handler);
    },

    //methods
    _onListClick: function(e) {
        var li;
        if (e.target.nodeName == 'A')
            li = e.target.parentNode;
        else {
            li = e.target;
            while (li.nodeName != 'LI' && li.parentNode) {
                li = li.parentNode;
            }
        }
        if (!li || li.nodeName != 'LI') return;
        $removeHandler(li, 'click', this._onListClickDel);
        $addHandler(li, 'click', this._offListClickDel);
        this._onList.removeChild(li);
        var i = 0;
        while (i < this._offList.childNodes.length && this._offList.childNodes[i].SortOrder < li.SortOrder)
            i++;
        if (this._offList.childNodes.length == 0 || i == this._offList.childNodes.length) {
            this._offList.appendChild(li);
        } else {
            this._offList.insertBefore(li, this._offList.childNodes[i]);
        }
        //this._offList.appendChild(li);
        var el = this.get_element();
        if (el.value.indexOf(',') >= 0) {
            var re = new RegExp('(\\b,' + li.value + '\\b)|(\\b' + li.value + ',\\b)');
            el.value = el.value.replace(re, '');
        } else {
            el.value = '';
        }
        if (this._addImageUrl) {
            var img = li.getElementsByTagName('img');
            if (img.length > 0) {
                img[0].src = this._addImageUrl;
            }
        }
        this._onCnt--;
        var h = this.get_events().getHandler('listclick');
        if (h) h(this, Sys.EventArgs.Empty);
        if (this._pbReference) {
            eval(this._pbReference);
        }
    },
    _offListClick: function(e) {
        if (this._selectLimit && this._selectLimit <= this._onCnt) return;
        var li;
        if (e.target.nodeName == 'A')
            li = e.target.parentNode;
        else {
            li = e.target;
            while (li.nodeName != 'LI' && li.parentNode) {
                li = li.parentNode;
            }
        }
        if (!li || li.nodeName != 'LI') return;
        $removeHandler(li, 'click', this._offListClickDel);
        $addHandler(li, 'click', this._onListClickDel);
        this._offList.removeChild(li);
        this._onList.appendChild(li);
        var hdn = this.get_element();
        if (hdn.value == '') {
            hdn.value = li.value;
        } else {
            hdn.value += ',' + li.value;
        }
        if (this._deleteImageUrl) {
            var img = li.getElementsByTagName('img');
            if (img.length > 0) {
                img[0].src = this._deleteImageUrl;
            }
        }
        this._onCnt++;
        var h = this.get_events().getHandler('listclick');
        if (h) h(this, Sys.EventArgs.Empty);
        if (this._pbReference) {
            eval(this._pbReference);
        }
    }
}

AE.ListSelect.registerClass('AE.ListSelect', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();


