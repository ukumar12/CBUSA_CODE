Type.registerNamespace("AE");

AE.DropManagerDrop = function (id, element) {
    this.id = id;
    this.element = element;
}

AE.DropDetails = function () {
    this.name = '';
    this.id = '';
    this.city = '';
    this.state = '';
    this.address1 = '';
    this.address2 = '';
    this.zip = '';
    this.notes = '';
    this.instructions = '';
    this.delivery = '';
}

AE.DropManager = function (element) {
    AE.DropManager.initializeBase(this, [element]);

    this._initDrops = [];
    this._initItems = [];

    this._drops = [];
    this._newDrop = null;
    this._items = [];
    this._tbl = null;
    this._itemCell = null;
    this._dropCell = null;

    this._isDragging = false;
    this._dragElement = null;
    this._dragStart = { x: null, y: null };
    this._currentDrop = null;
    this._dragSource = null;

    this._itemNormalClass = null;
    this._itemDragClass = null;
    this._dropNormalClass = null;
    this._dropHoverClass = null;

    this._openDropFunction = null;
    this._callback = null;
}

AE.DropManager.prototype = {

    initialize: function () {
        AE.DropManager.callBaseMethod(this, 'initialize');

        var el = this.get_element();

        var tbl = document.createElement('table');
        tbl.cellSpacing = '10px';
        tbl.style.width = '100%';
        tbl.style.marginLeft = 'auto';
        tbl.style.marginRight = 'auto';
        el.parentNode.appendChild(tbl);
        tbl = tbl.appendChild(document.createElement('tbody'));
        tbl.appendChild(document.createElement('tr'));
        this._itemCell = tbl.rows[0].appendChild(document.createElement('td'));
        this._itemCell.className = 'pckggraywrpr'; /*pckgltgraywrpr*/
        this._itemCell.style.width = '50%';
        //this._itemCell.style.position = 'relative';
        var itemHdg = this._itemCell.appendChild(document.createElement('div'));
        itemHdg.className = 'pckghdgred nobdr';
        itemHdg.appendChild(document.createTextNode('Items'));
        this._itemCell = this._itemCell.appendChild(document.createElement('div'));
        this._itemCell.style.position = 'relative';

        this._dropCell = tbl.rows[0].appendChild(document.createElement('td'));
        this._dropCell.className = 'pckggraywrpr'; /*pckgltgraywrpr*/
        this._dropCell.style.position = 'relative';
        //this._dropCell.style.backgroundColor = '#fff';
        this._dropCell.style.width = '50%';
        var dropHdg = this._dropCell.appendChild(document.createElement('div'));
        dropHdg.className = 'pckghdgred nobdr';
        dropHdg.appendChild(document.createTextNode('Drops'));

        if (this._initDrops.length == 0) {
            this._dropCell.style.display = 'none';
            tbl.parentNode.style.width = '50%';
        }

        this._dragStartDel = Function.createDelegate(this, this._startDrag);
        this._dragDel = Function.createDelegate(this, this._drag);
        this._dragStopDel = Function.createDelegate(this, this._stopDrag);
        this._openDropFormDel = Function.createDelegate(this, this._openDropForm);
        this._deleteDropDel = Function.createDelegate(this, this._deleteDrop);
        this._doCallbackDel = Function.createDelegate(this, this._doCallback);

        var temp = {};
        for (var i = 0; i < this._initItems.length; i++) {
            var div = document.createElement('div');
            div.id = el.id + '_ITEM' + this._initItems[i].id;
            div.className = this._itemNormalClass;
            if (this._initItems[i].dropId) {
                if (!temp[this._initItems[i].dropId]) {
                    temp[this._initItems[i].dropId] = [];
                }
                temp[this._initItems[i].dropId].push(div);
            } else {
                this._itemCell.appendChild(div);
            }
            div.innerHTML = this._initItems[i].name;
            this._items[i] = new AE.DropManagerDrop(this._initItems[i].id, div);
            div.itemObj = this._items[i];
            div.itemObj.sortorder = this._initItems[i].sortorder;
            this._addItemHandlers(div);
        }

        for (var i = 0; i < this._initDrops.length; i++) {
            var div = document.createElement('div');
            div.id = el.id + '_DROP' + this._initDrops[i].id;
            div.className = this._dropNormalClass;

            var hdg = div.appendChild(document.createElement('div'));
            hdg.className = 'bold larger'; /*bgpurple */
            //hdg.style.color = '#fff';
            hdg.style.color = '#0f2e51';
            hdg.style.padding = '5px';

            var btnDelete = document.createElement('img');
            btnDelete.div = div;
            btnDelete.src = '/images/admin/delete.gif';
            btnDelete.className = 'dropbutton';
            hdg.appendChild(btnDelete);
            $addHandler(btnDelete, 'click', this._deleteDropDel);

            var btnEdit = document.createElement('img');
            btnEdit.div = div;
            btnEdit.src = '/images/admin/edit.gif';
            btnEdit.className = 'dropbutton';
            hdg.appendChild(btnEdit);
            $addHandler(btnEdit, 'click', this._openDropFormDel);

            var span = document.createElement('span');
            span.innerHTML = this._initDrops[i].name + '&nbsp;&nbsp;-&nbsp;&nbsp;' + this._initDrops[i].delivery;
            hdg.appendChild(span);

            var pNone = div.appendChild(document.createElement('p'));
            pNone.className = 'bold smaller center';
            pNone.appendChild(document.createTextNode('This drop has no items'));

            if (temp[this._initDrops[i].id] && temp[this._initDrops[i].id].length > 0) {
                pNone.style.display = 'none';
                for (var j = 0; j < temp[this._initDrops[i].id].length; j++) {
                    div.appendChild(temp[this._initDrops[i].id][j]);
                }
            } else {
                pNone.style.display = '';
            }
            this._drops[i] = new AE.DropManagerDrop(this._initDrops[i].id, div);
            div.dropObj = this._drops[i];
            div.dropDetails = this._initDrops[i];
            this._dropCell.appendChild(div);
        }
        this._initNewDrop(this._dropCell);

        //start scroll polling
        this._scrollInterval = window.setInterval(Function.createDelegate(this, this._scroll), 200);
    },
    get_initDrops: function () {
        return this._initDrops;
    },
    set_initDrops: function (value) {
        this._initDrops = value;
    },
    get_initItems: function () {
        return this._initItems;
    },
    set_initItems: function (value) {
        this._initItems = value;
    },
    get_itemNormalClass: function () {
        return this._itemNormalClass;
    },
    set_itemNormalClass: function (value) {
        this._itemNormalClass = value;
    },
    get_itemDragClass: function () {
        return this._itemDragClass;
    },
    set_itemDragClass: function (value) {
        this._itemDragClass = value;
    },
    get_dropNormalClass: function () {
        return this._dropNormalClass;
    },
    set_dropNormalClass: function (value) {
        this._dropNormalClass = value;
    },
    get_dropHoverClass: function () {
        return this._dropHoverClass;
    },
    set_dropHoverClass: function (value) {
        this._dropHoverClass = value;
    },
    get_openDropFunction: function () {
        return this._openDropFunction;
    },
    set_openDropFunction: function (value) {
        this._openDropFunction = value;
    },
    get_callback: function () {
        return this._callback;
    },
    set_callback: function (value) {
        this._callback = value;
    },

    AddDrop: function (drop, itemIdx) {
        if (itemIdx < 0) return 0;
        var div = document.createElement('div');
        div.id = this.get_element().id + '_DROP' + drop.id;
        div.className = this._dropNormalClass;

        var hdg = div.appendChild(document.createElement('div'));
        hdg.className = 'bold larger'; /*bgpurple */
        //hdg.style.color = '#fff'; 
        hdg.style.color = '#0f2e51';
        hdg.style.padding = '5px';

        var btnDelete = document.createElement('img');
        btnDelete.div = div;
        btnDelete.src = '/images/admin/delete.gif';
        btnDelete.className = 'dropbutton';
        hdg.appendChild(btnDelete);
        $addHandler(btnDelete, 'click', this._deleteDropDel);

        var btnEdit = document.createElement('img');
        btnEdit.div = div;
        btnEdit.src = '/images/admin/edit.gif';
        btnEdit.className = 'dropbutton';
        hdg.appendChild(btnEdit);
        $addHandler(btnEdit, 'click', this._openDropFormDel);

        hdg.appendChild(document.createTextNode(drop.name));

        this._items[itemIdx].element.parentNode.removeChild(this._items[itemIdx].element);
        div.appendChild(this._items[itemIdx].element);
        this._items[itemIdx].element.style.position = 'relative';
        this._items[itemIdx].element.style.top = '';
        this._items[itemIdx].element.style.left = '';

        this._drops.push(new AE.DropManagerDrop(drop.id, div));
        div.dropObj = this._drops[this._drops.length - 1];
        div.dropDetails = drop;
        this._dropCell.insertBefore(div, this._dropCell.lastChild);
        window.setTimeout(this._doCallbackDel, 500);
    },

    _addItemHandlers: function (element) {
        $addHandler(element, 'mousedown', this._dragStartDel);
    },

    _initNewDrop: function (container) {
        var div = document.createElement('div');
        div.id = this.get_element().id + '_NEWDROP';
        div.className = this._dropNormalClass;

        var hdg = div.appendChild(document.createElement('div'));
        hdg.className = ' bold larger'; /*bgpurple*/
        //hdg.style.color = '#fff';
        hdg.style.color = '#0f2e51';
        hdg.style.padding = '5px';
        /*
        var hdg = div.appendChild(document.createElement('div'));
        hdg.className = 'bggray bold larger';
        hdg.style.color = '#fff';
        hdg.style.padding = '5px';
        */

        hdg.appendChild(document.createTextNode('Create New Drop'));

        var p = div.appendChild(document.createElement('p'));
        p.appendChild(document.createTextNode('Drop items here to create new drops.'));

        this._newDrop = new AE.DropManagerDrop(-1, div);
        container.appendChild(div);
    },

    _startDrag: function (e) {
        e.stopPropagation();
        e.preventDefault();
        this._dragElement = e.target;
        while (!this._dragElement.itemObj) {
            if (this._dragElement.parentNode) {
                this._dragElement = this._dragElement.parentNode;
            } else {
                this._dragElement = null;
                return;
            }
        }
        this._dragSource = this._dragElement.parentNode;
        this._dragSource.oldZIndex = this._dragSource.style.zIndex;
        this._dragSource.style.zIndex = 20;

        this._dragStart.x = e.clientX;
        this._dragStart.y = e.clientY;
        this._isDragging = true;
        this._dragElement.className = this._itemDragClass;
        this._dragElement.originalLeft = e.target.style.left;
        this._dragElement.originalTop = e.target.style.top;
        this._dragElement.originalPos = e.target.style.position;
        this._dragElement.style.position = 'absolute';
        //this._dragElement.style.top = '';
        //this._dragElement.style.left = '';
        //var loc = Sys.UI.DomElement.getLocation(this._dragElement);
        //this._dragStart.offLeft = loc.x - e.clientX;
        //this._dragStart.offTop = loc.y - e.clientY;
        //this._dragStart.offLeft = this._dragElement.offsetLeft;
        this._dragStart.offTop = this._dragElement.offsetTop;
        this._dragStart.offLeft = 0;
        //this._dragStart.offTop = 0;
        $addHandler(document, 'mousemove', this._dragDel);
        $addHandler(document, 'selectstart', this._cancelEvent);
        $addHandler(document, 'mouseup', this._dragStopDel);

        //3 elements = header + is empty <p> + item being removed
        if (this._dragSource.childNodes.length == 3) {
            var p = this._dragSource.getElementsByTagName('p');
            if (p.length > 0) {
                p[0].style.display = 'block';
                this._dragElement.style.top = (this._dragElement.style.top ? parseInt(this._dragElement.style.top) + p[0].clientHeight : p[0].clientHeight) + 'px';
            }
        }
    },

    _drag: function (e) {
        this._dragElement.style.left = (this._dragStart.offLeft + e.clientX - this._dragStart.x) + 'px';
        this._dragElement.style.top = (this._dragStart.offTop + e.clientY - this._dragStart.y) + 'px';
        /*
        var height = document.documentElement && document.documentElement.clientHeight < document.body.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight;
        if (document.body.scrollHeight > height && e.clientY + 50 >= document.body.scrollTop + height) {
        window.scrollBy(5, 0);
        }
        */
        var h = document.documentElement ? document.documentElement.offsetHeight : document.body.offsetHeight;
        if (e.clientY > h - 15) {
            this._scrollAmt = 15;
        } else if (e.clientY < 15) {
            this._scrollAmt = -15
        } else {
            this._scrollAmt = null;
        }
    },

    _scroll: function () {
        if (this._scrollAmt) {
            window.scrollBy(0, this._scrollAmt);
            this._dragElement.style.top = (parseInt(this._dragElement.style.top) + this._scrollAmt) + 'px';
            this._dragStart.offTop += this._scrollAmt;
        }
    },

    _stopDrag: function (e) {
        e.stopPropagation();
        this._isDragging = false;
        this._scrollAmt = null;
        this._dragElement.className = this._itemNormalClass;
        this._dragSource.style.zIndex = this._dragSource.oldZIndex;
        $removeHandler(document, 'mousemove', this._dragDel);
        $removeHandler(document, 'selectstart', this._cancelEvent);
        $removeHandler(document, 'mouseup', this._dragStopDel);

        var dragDest = null;
        var b = Sys.UI.DomElement.getBounds(this._newDrop.element);
        var scrollX = document.documentElement ? document.documentElement.scrollLeft : document.body.scrollLeft;
        var scrollY = document.documentElement ? document.documentElement.scrollTop : document.body.scrollTop;
        if (b.x - scrollX <= e.clientX && (b.x + b.width - scrollX) >= e.clientX && b.y - scrollY <= e.clientY && (b.y + b.height - scrollY) >= e.clientY) {
            for (var i = 0; i < this._items.length; i++) {
                if (this._items[i] === this._dragElement.itemObj) break;
            }
            this._openDropFunction(new AE.DropDetails(), i);
            return;
        }
        for (var i = 0; i < this._drops.length; i++) {
            b = Sys.UI.DomElement.getBounds(this._drops[i].element);
            //scrollX = this._drops[i].element.scrollLeft;
            //scrollY = this._drops[i].element.scrollTop;
            if (b.x - scrollX <= e.clientX && (b.x + b.width - scrollX) >= e.clientX && b.y - scrollY <= e.clientY && (b.y + b.height - scrollY) >= e.clientY) {
                dragDest = this._drops[i].element;
                var p = this._drops[i].element.getElementsByTagName('p');
                if (p.length > 0) {
                    //this._drops[i].element.removeChild(p[0]);
                    p[0].style.display = 'none';
                }
                this._dragSource.removeChild(this._dragElement);
                if (this._drops[i].element.childNodes.length > 2) {
                    var child = this._drops[i].element.childNodes[2];
                    while (child && Sys.UI.DomElement.getLocation(child).y + child.offsetHeight / 2 < e.clientY) {
                        child = child.nextSibling;
                    }
                    if (child && child.itemObj & child.itemObj.sortorder < this._dragElement.itemObj.sortorder) {
                        var temp = child.itemObj.sortorder;
                        child.itemObj.sortorder = this._dragElement.itemObj.sortOrder;
                        this._dragElement.itemObj.sortorder = temp;
                    }
                    if (child) {
                        this._drops[i].element.insertBefore(this._dragElement, child);
                    } else {
                        this._drops[i].element.appendChild(this._dragElement);
                    }
                } else
                    this._drops[i].element.appendChild(this._dragElement);
                this._dragElement.style.position = 'relative';
                this._dragElement.style.top = '';
                this._dragElement.style.left = '';
                for (var u = 2; u < this._drops[i].element.childNodes.length; u++) {
                    for (var v = u + 1; v < this._drops[i].element.childNodes.length; v++) {
                        if (this._drops[i].element.childNodes[u].itemObj.sortorder > this._drops[i].element.childNodes[v].itemObj.sortorder) {
                            this._swap(this._drops[i].element.childNodes[u].itemObj, this._drops[i].element.childNodes[v].itemObj);
                        }
                    }
                }
            }
        }
        /*
        if (this._dragSource.childNodes.length == 2) {
        //show 'no items' <p> if no items
        this._dragSource.childNodes[0].style.display = '';
        }
        */
        if (!dragDest) {
            this._dragSource.removeChild(this._dragElement);
            if (this._itemCell.childNodes.length > 1) {
                var child = this._itemCell.childNodes[1];
                while (child && Sys.UI.DomElement.getLocation(child).y + child.offsetHeight / 2 < e.clientY) {
                    child = child.nextSibling;
                }
                if (child) {
                    this._itemCell.insertBefore(this._dragElement, child);
                } else {
                    this._itemCell.appendChild(this._dragElement);
                }
            } else
                this._itemCell.appendChild(this._dragElement);

            for (var u = 1; u < this._itemCell.childNodes.length; u++) {
                for (var v = u + 1; v < this._itemCell.childNodes.length; v++) {
                    if (this._itemCell.childNodes[u].itemObj.sortorder > this._itemCell.childNodes[v].itemObj.sortorder) {
                        this._swap(this._itemCell.childNodes[u].itemObj, this._itemCell.childNodes[v].itemObj);
                    }
                }
            }

            this._dragElement.style.position = 'relative';
            this._dragElement.style.top = '';
            this._dragElement.style.left = '';
        }
        this._doCallback();
    },

    _swap: function (a, b) {
        var temp = a.sortorder;
        a.sortorder = b.sortorder;
        b.sortorder = temp;
    },

    _doCallback: function () {
        var out = {};
        for (var i = 0; i < this._drops.length; i++) {
            out[this._drops[i].id] = '';
            var conn = '';
            for (var cid in this._drops[i].element.childNodes) {
                var c = this._drops[i].element.childNodes[cid];
                if (c.id) {
                    var m = /_ITEM([\d]+)/.exec(c.id);
                    if (m.length > 1) {
                        out[this._drops[i].id] += conn + m[1] + '|' + c.itemObj.sortorder;
                        conn = ',';
                    }
                }
            }
        }
        out[-1] = '';
        conn = '';
        for (var cid in this._itemCell.childNodes) {
            var c = this._itemCell.childNodes[cid];
            if (c.itemObj) {
                var m = /_ITEM([\d]+)/.exec(c.id);
                if (m.length > 1) {
                    out[-1] += conn + m[1] + '|' + c.itemObj.sortorder;
                    conn = ',';
                }
            }
        }
        var arg = Sys.Serialization.JavaScriptSerializer.serialize(out);
        eval(this._callback);
    },

    _openDropForm: function (e) {
        this._openDropFunction(e.target.div.dropDetails, -1);
    },

    _deleteDrop: function (e) {

    },

    _cancelEvent: function () {
        return false;
    },

    dispose: function () {
        AE.DropManager.callBaseMethod(this, 'dispose');

        window.clearInterval(this._scrollInterval);
    }
}

AE.DropManager.registerClass('AE.DropManager', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();


