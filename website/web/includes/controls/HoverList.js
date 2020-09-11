Type.registerNamespace("AE");

AE.HoverList = function(element) {
    AE.HoverList.initializeBase(this, [element]);

    this._items = [];
    this._postback = null;
    this._div = null;
    this._label = null;
    this._image = null;
    this._list = null;
    this._wrapper = null;

    this._hoverCount = 0;
    this._hoverFired = false;

    this._maxHeight = 0;
}

AE.HoverList.prototype = {

    initialize: function() {
        AE.HoverList.callBaseMethod(this, 'initialize');

        this._onMouseOverDel = Function.createDelegate(this, this._onMouseOver);
        this._onMouseOutDel = Function.createDelegate(this, this._onMouseOut);
        this._closeListDel = Function.createDelegate(this, this._closeList);
        this._openListDel = Function.createDelegate(this, this._openList);
        this._listItemClickDel = Function.createDelegate(this, this._listItemClick);

        var el = this.get_element();
        this._div = $get(el.id + '_div');
        this._label = $get(el.id + '_lbl');
        this._image = $get(el.id + '_img');
        this._list = $get(el.id + '_list');
        this._wrapper = $get(el.id + '_wrapper');

        if (this._items) {
            for (var i = 0; i < this._items.length; i++) {
                var li = document.createElement('li');
                var a = document.createElement('a');
                a.innerHTML = this._items[i].Name;
                a.value = this._items[i].Value;
                li.appendChild(a);
                this._list.appendChild(li);
                $addHandler(a, 'click', this._listItemClickDel);
            }
        }

        if (this._maxHeight) {
            var listHeight = this._div.clientHeight ? this._div.clientHeight : this._div.offsetHeight;
            if (listHeight > this._maxHeight) {
                this._list.style.height = this._maxHeight + 'px';
                this._list.style.overflowY = 'auto';
            }
        }
        this._div.style.display = 'none';
        $addHandler(this._wrapper, 'mouseover', this._onMouseOverDel);
        $addHandler(this._wrapper, 'mouseout', this._onMouseOutDel);
    },

    //properties
    get_items: function() {
        return this._items;
    },
    set_items: function(value) {
        this._items = value;
    },
    get_postback: function() {
        return this._postback;
    },
    set_postback: function(value) {
        this._postback = value;
    },
    get_maxHeight: function() {
        return this._maxHeight;
    },
    set_maxHeight: function(value) {
        this._maxHeight = value;
    },
    //events
    add_select: function(handler) {
        this.get_events().addHandler('select', handler);
    },

    remove_select: function(handler) {
        this.get_events().removeHandler('select', handler);
    },

    //methods
    _onMouseOver: function(e) {
        this._hoverCount++;
        if (!this._hoverCount || this._hoverFired) return;
        this._hoverFired = true;
        if (this._hoverTimer) {
            window.clearTimeout(this._hoverTimer);
        }
        this._openList();
    },

    _onMouseOut: function(e) {
        this._hoverCount--;
        if (this._hoverCount < 0) this._hoverCount = 0;
        if (!this._hoverCount && this._hoverFired) {
            this._hoverFired = false;
            if (this._hoverTimer) {
                window.clearTimeout(this._hoverTimer);
            }
            this._hoverTimer = window.setTimeout(this._closeListDel, 150);
        }
    },

    _openList: function() {
        this._image.src = '/images/expanded.gif';
        $(this._div).slideDown('fast');
    },

    _closeList: function() {
        this._image.src = '/images/collapsed.gif';
        $(this._div).slideUp('fast');
    },

    _listItemClick: function(e) {
        this.get_element().value = e.target.value;
        this._closeList();
        if (this._postback) {
            eval(this._postback);
        }
        var h = this.get_events().getHandler('select');
        if (h) {
            h(this, Sys.EventArgs.Empty);
        }
    }
}

AE.HoverList.registerClass('AE.HoverList', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();


