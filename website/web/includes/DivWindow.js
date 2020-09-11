Type.registerNamespace("AE");

AE.DivWindow = function(element) {
    AE.DivWindow.initializeBase(this, [element]);

    this._header = null;
    this._trigger = null;
    this._closeTrigger = null;
    
    this._stateField = null;

    this._moveable = false;
    this._showVeil = false;
    this._veil = null;
    this._veilCloses = false;
    this._moveTop = false;

    this._timers = [];
    this._interval = 30;
    this._step = .25;
    this._fade = 0;
    this._x = null;
    this._y = null;
    this._w = null;
    this._h = null;

    this._clickX = null;
    this._clickY = null;
    this._xOff = 0;
    this._yOff = 0;
    this._hoverCnt = 0;
}

AE.DivWindow.prototype = {

    initialize: function() {
        AE.DivWindow.callBaseMethod(this, 'initialize');

        this._onDragStart = Function.createDelegate(this, this._dragStart);
        this._onDrag = Function.createDelegate(this, this._drag);
        this._onDragEnd = Function.createDelegate(this, this._dragEnd);
        this._onFadeOut = Function.createDelegate(this, this.fadeOut);
        this._onFadeIn = Function.createDelegate(this, this.fadeIn);

        if (this._header && this._moveable) {
            this._header.style.cursor = 'move';
        }
        if (this._closeTrigger) {
            this._closeTrigger.style.cursor = 'pointer';
        }

        var el = this.get_element();
        var p = null;
        if (!this._x) {
            if (el.style.left) {
                this._x = parseInt(el.style.left);
            } else {
                if (!p) {
                    p = Sys.UI.DomElement.getLocation(el);
                }
                this._x = p.x;
            }
        }
        if (!this._y) {
            if (el.style.top) {
                this._y = parseInt(el.style.top);
            } else {
                if (!p) {
                    p = Sys.UI.DomElement.getLocation(el);
                }
                this._y = p.y;
            }
        }

        if (this._moveTop) {
            var root = $get('divWindowRoot');
            if (!root) root = document.forms[0];
            el.parentNode.removeChild(el);
            root.appendChild(el);
        }

        if (this._showVeil) {
            this.initVeil();
        }

        if (this._trigger) {
            $addHandler(this._trigger, "click", Function.createDelegate(this, this._clickFadeIn));
        }
        $addHandler(window, "resize", Function.createDelegate(this, this.initVeil));
        $addHandler(el, "mouseover", Function.createDelegate(this, this._onMouseOver));
        //$addHandler(el,"focus",Function.createDelegate(this,this._onMouseOver));
        $addHandler(el, "mouseout", Function.createDelegate(this, this._onMouseOut));
        //$addHandler(el,"blur",Function.createDelegate(this,this._onMouseOut));
        $addHandler(el, "click", Function.createDelegate(this, function(e) { this.IsClick = true }));
        var children = el.getElementsByTagName('div');
        for (c in children) {
            if (children[c].className && children[c].className.substr(0, 4) == 'shad') {
                $addHandler(children[c], "mouseover", this.stopProp);
                $addHandler(children[c], "mouseout", this.stopProp);
            }
        }
    },
    stopProp: function(e) {
        if (e.stopPropagation) e.stopPropagation();
    },

    get_hoverCnt: function() {
        return this._hoverCnt;
    },
    get_moveTop: function() {
        return this._moveTop;
    },
    set_moveTop: function(value) {
        this._moveTop = value;
    },
    get_veilCloses: function() {
        return this._veilCloses;
    },
    set_veilCloses: function(value) {
        this._veilCloses = value;
    },
    get_showVeil: function() {
        return this._showVeil;
    },
    set_showVeil: function(value) {
        this._showVeil = value;
    },
    get_isOpen: function() {
        return this._isOpen;
    },

    get_moveable: function() {
        return this._moveable;
    },
    set_moveable: function(value) {
        this._moveable = value;
    },
    get_header: function() {
        return this._header;
    },
    set_header: function(value) {
        this._header = value;
    },

    get_trigger: function() {
        return this._trigger;
    },
    set_trigger: function(value) {
        this._trigger = value;
    },

    get_closeTrigger: function() {
        return this._closeTrigger;
    },
    set_closeTrigger: function(value) {
        this._closeTrigger = value;
    },

    get_x: function() {
        return this._x;
    },
    set_x: function(value) {
        this._x = value;
    },

    get_y: function() {
        return this._y;
    },
    set_y: function(value) {
        this._y = value;
    },

    get_w: function() {
        return this._w;
    },
    set_w: function(value) {
        this._w = value;
    },

    get_h: function() {
        return this._h;
    },
    set_h: function(value) {
        this._h = value;
    },
    /*
    initVeil: function() {
    var el = this.get_element();
    if (!el) return;
    var v = null;
    if (!this._veil) {
    v = document.createElement('div');
    v.style.backgroundColor = '#aaa';
    v.style.opacity = '.50';
    v.style.visibility = 'visible';
    v.style.filter = 'alpha(opacity=50)';
    v.style.position = 'absolute';
    v.style.display = 'none';
    v.style.zIndex = '4';
    this._veil = v;
    el.parentNode.appendChild(v);
    } else
    v = this._veil;
    var w, h;
    w = document.body.offsetWidth;
    if (window.innerHeight && window.scrollMaxY !== 'undefined') {
    h = window.innerHeight + window.scrollMaxY;
    }
    else if (document.body.scrollHeight > document.body.offsetHeight) // all but Explorer Mac
    {
    h = document.body.scrollHeight;
    }
    else // works in Explorer 6 Strict, Mozilla (not FF) and Safari
    {
    h = document.body.offsetHeight + document.body.offsetTop;
    }
    v.style.width = w + 'px';
    v.style.height = h + 'px';
    var offset = el.offsetParent;
    if (!offset) {
    var temp = el;
    while (temp.parentNode && !temp.parentNode.offsetParent) {
    temp = temp.parentNode;
    }
    if (temp.parentNode) offset = temp.parentNode.offsetParent;
    }
    if (offset) {
    var offX = offset.offsetLeft,
    offY = offset.offsetTop;
    var tmp = offset;
    while (tmp = tmp.offsetParent) {
    offX += tmp.offsetLeft;
    offY += tmp.offsetTop;
    }
    }
    v.style.top = -offY + 'px';
    v.style.left = -offX + 'px';
    },
    */

    initVeil: function() {
        var el = this.get_element();
        if (!el) return;
        var v = null;
        if (!this._veil) {
            v = document.createElement('div');
            v.style.backgroundColor = '#aaa';
            v.style.opacity = '.50';
            v.style.visibility = 'visible';
            v.style.filter = 'alpha(opacity=50)';
            v.style.position = 'absolute';
            v.style.display = 'none';
            v.style.zIndex = '4';
            this._veil = v;
            el.parentNode.appendChild(v);
            if (this._veilCloses) {
                $addHandler(this._veil, 'click', this._onFadeOut);
            }
        } else
            v = this._veil;
        var w, h;
        w = document.body.offsetWidth;
        if (window.innerHeight && window.scrollMaxY !== 'undefined') {
            h = window.innerHeight + window.scrollMaxY;
        }
        else if ((document.documentElement ? document.documentElement.scrollHeight : document.body.scrollHeight) > document.body.offsetHeight) // all but Explorer Mac
        {
            h = (document.documentElement ? document.documentElement.scrollHeight : document.body.scrollHeight);
        }
        else // works in Explorer 6 Strict, Mozilla (not FF) and Safari
        {
            h = document.body.offsetHeight + document.body.offsetTop;
        }
        h += 50;
        if (h < screen.availHeight) {
            h = screen.availHeight;
        }
        v.style.width = w + 'px';
        v.style.height = h + 'px';
        //var loc = Sys.UI.DomElement.getLocation(v);
        //var offX = loc.x;
        //var offY = loc.y;
        if (v.offsetParent && v.offsetParent.style.position == 'absolute') {
            var loc = Sys.UI.DomElement.getLocation(v.offsetParent);
            v.style.top = loc.y + 'px';
            v.style.left = loc.x + 'px';
        } else {
            v.style.top = '0px';
            v.style.left = '0px';
        }
    },

    moveToCenter: function() {
        this._centered = true;
        var el = this.get_element();
        var w = this._getWinWidth();
        var h = this._getWinHeight();

        var tmp = el.offsetParent;
        var offX = tmp.offsetLeft,
            offY = tmp.offsetTop;
        while (tmp = tmp.offsetParent) {
            offX += tmp.offsetLeft;
            offY += tmp.offsetTop;
        }
        offX -= document.body.scrollLeft ? document.body.scrollLeft : document.documentElement.scrollLeft;
        offY -= document.body.scrollTop ? document.body.scrollTop : document.documentElement.scrollTop;
        el.style.left = Math.round((w / 2) - (parseFloat(el.clientWidth ? el.clientWidth : el.style.width) / 2) - offX) + 'px';
        el.style.top = Math.round((h / 2) - (parseFloat(el.clientHeight ? el.clientHeight : el.style.height) / 2) - offY) + 'px';
    },

    moveToClick: function(e) {
        var el = this.get_element();
        var offset = this.getDocumentOffset();
        var scrollX = document.documentElement != 'undefined' ? document.documentElement.scrollLeft : document.body.scrollLeft;
        var scrollY = document.documentElement != 'undefined' ? document.documentElement.scrollTop : document.body.scrollTop;
        var cx = e.clientX;
        var cy = e.clientY;
        var winWidth = this._getWinWidth();
        var winHeight = this._getWinHeight();
        if (cx - el.clientWidth / 2 - 25 < 0) {
            cx += Math.abs(cx - el.clientWidth / 2 - 25);
        }
        if (cx + el.clientWidth / 2 > winWidth - 25) {
            cx -= (cx + el.clientWidth / 2 - winWidth + 25);
        }
        if (cy - el.clientHeight / 2 - 25 < 0) {
            cy += Math.abs(cy - el.clientHeight / 2 - 25);
        }
        if (cy + el.clientHeight / 2 > winHeight - 25) {
            cy -= (cy + el.clientHeight / 2 - winHeight + 25);
        }
        el.style.left = Math.round(cx - offset.x + scrollX - el.clientWidth / 2) + 'px';
        el.style.top = (cy - offset.y + scrollY - el.clientHeight / 2) + 'px';
    },

    getDocumentOffset: function() {
        var el = this.get_element();
        var x = 0,
            y = 0;
        while (el = el.offsetParent) {
            x += el.offsetLeft;
            y += el.offsetTop;
        }
        return { 'x': x, 'y': y };
    },

    _getWinWidth: function() {
        return window.innerWidth ? window.innerWidth : (document.documentElement.clientWidth ? document.documentElement.clientWidth : document.body.clientWidth);
    },

    _getWinHeight: function() {
        return window.innerHeight ? window.innerHeight : (document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight);
    },

    addHoverOver: function(handler) {
        if (!this._hovers) {
            this._hovers = new Array();
        }
        this._hovers.push(handler);
        this.get_events().addHandler('hoverover', handler);
    },

    removeHoverOver: function(handler) {
        for (h in this._hovers) {
            if (this._hovers[h] === handler) {
                this._hovers.splice(h, 1);
                break;
            }
        }
        this.get_events().removeHandler('hoverover', handler);
    },

    addHoverOut: function(handler) {
        this.get_events().addHandler('hoverout', handler);
    },

    removeHoverOut: function(handler) {
        this.get_events().removeHandler('hoverout', handler);
    },

    _onMouseOver: function(e) {
        if (!(e.rawEvent.fromElement || e.rawEvent.relatedTarget)) return;
        this._hoverCnt++;
        if (!this._hoverCnt || this._hoverFired) return;
        var h = this.get_events().getHandler('hoverover');
        if (h) h(this, Sys.EventArgs.Empty);
        this._hoverFired = true;
    },

    _onMouseOut: function(e) {
        this._hoverCnt--;
        if (this._hoverCnt <= 0) this._hoverCnt = 0;
        if (this._hoverFired && !this._hoverCnt) {
            this._hoverFired = false;
            var h = this.get_events().getHandler('hoverout');
            if (h) h(this, Sys.EventArgs.Empty);
        }
    },

    veilOn: function() {
        if (this._veil) {
            this._veil.style.display = 'block';
        }
    },
    veilOff: function() {
        if (this._veil) {
            this._veil.style.display = 'none';
        }
    },
    _clearTimers: function() {
        if (this._timers && this._timers.length > 0) {
            for (var t in this._timers) {
                window.clearTimeout(this._timers[t]);
            }
        }
        this._timers = [];
    },

    _clickFadeIn: function(e) {
        e.preventDefault();
        this.moveToClick(e);
        this.startFadeIn();
    },

    fadeInFrom: function(start) {
        this._clearTimers();
        var el = this.get_element();
        if (start >= 1) {
            this._fadeInComplete();
        } else {
            this._fade = start;
            el.style.visibility = 'visible';
            var del = Function.createDelegate(this, this._fadeInTick)
            for (var i = 1; (start + i * this._step) <= 1; i++) {
                this._timers[i - 1] = window.setTimeout(del, i * this._interval);
            }
        }
    },

    fadeOutFrom: function(start) {
        this._clearTimers();
        var el = this.get_element();
        if (start <= 0) {
            this._fadeOutComplete();
        } else {
            this._fade = start;
            var del = Function.createDelegate(this, this._fadeOutTick)
            for (var i = 1; (start - i * this._step) >= 0; i++) {
                this._timers[i - 1] = window.setTimeout(del, i * this._interval);
            }
        }
    },

    fadeIn: function() {
        this._clearTimers();
        var el = this.get_element();
        this._fade = 0;
        el.style.opacity = 0;
        el.style.filter = 'alpha(opacity=0)';
        el.style.visibility = 'visible';

        var del = Function.createDelegate(this, this._fadeInTick)
        for (var i = 1; (i * this._step) <= 1; i++) {
            this._timers[i - 1] = window.setTimeout(del, i * this._interval);
        }

        if (this._showVeil) {
            this.initVeil();
            this.veilOn();
            /*
            if (this._veilCloses) {
            $addHandler(this._veil, "click", this._onFadeOut);
            }
            */
        }
    },

    _fadeInComplete: function() {
        this._clearTimers();
        if (this._closeTrigger) {
            $addHandler(this._closeTrigger, "click", this._onFadeOut);
            $addHandler(this._closeTrigger, "mouseup", function(e) { e.stopPropagation() });
        }
        var el = this.get_element();
        if (el) {
            if (el.style.filter) {
                el.style.filter = '';
            }
            if (el.style.opacity) {
                el.style.opacity = '';
            }
        }
        this._fade = 1;
        if (this._moveable && this._header) $addHandler(this._header, "mousedown", this._onDragStart);
        this._isOpen = true;
    },

    startFadeIn: function() {
        var el = this.get_element();
        if ((typeof (el.style.opacity) == 'undefined' || el.style.opacity == '') && (typeof (el.style.filter) == 'undefined' || el.style.filter == '') && this._fade == 1 && el.style.visibility == 'visible') {
            this._clearTimers();
            return;
        }
        if ((this._timers && this._timers.length > 0) || this._fade > 0) {
            if (this._timers && this._timers.length > 0 && this._step > 0) return;
            if (this._timers) this._clearTimers();
            this.fadeInFrom(this._fade);
        } else {
            this.fadeIn();
        }
    },

    startFadeOut: function() {
        var el = this.get_element();
        if ((typeof (el.style.opacity) == 'undefined' || el.style.opacity == '') && (typeof (el.style.filter) == 'undefined' || el.style.filter == '') && this._fade == 0 && el.style.visibility == 'hidden') {
            this._clearTimers();
            return;
        }
        if ((this._timers && this._timers.length > 0) || this._fade < 1) {
            if (this._timers && this._timers.length > 0 && this._step < 0) return;
            if (this._timers) this._clearTimers();
            this.fadeOutFrom(this._fade);
        } else {
            this.fadeOut(false);
        }
    },

    fadeOut: function(e) {
        if (e) {
            if (e.preventDefault) e.preventDefault();
            if (e.stopPropagation) e.stopPropagation();
        }
        this._clearTimers();
        if (this._veil && this._veil.style.display != 'none') {
            this._veil.style.display = 'none';
        }
        this._fade = 1;
        var del = Function.createDelegate(this, this._fadeOutTick)
        for (var i = 1; (1 - i * this._step) >= 0; i++) {
            this._timers[i - 1] = window.setTimeout(del, i * this._interval);
        }
    },

    _fadeOutComplete: function() {
        this._clearTimers();
        var el = this.get_element();
        if (el) {
            el.style.visibility = 'hidden';
            if (el.style.filter) {
                el.style.filter = '';
            }
            if (el.style.opacity) {
                el.style.opacity = '';
            }
        }
        this._fade = 0;
        if (this._closeTrigger) {
            $clearHandlers(this._closeTrigger);
        }
        if (this._header) $clearHandlers(this._header);
        this._isOpen = false;
    },

    _fadeInTick: function() {
        this._fade += this._step;
        this._fade = this._fade > 1 ? 1 : this._fade;
        var el = this.get_element();
        if (el) {
            el.style.opacity = this._fade;
            el.style.filter = 'alpha(opacity=' + (this._fade * 100) + ')';
        }
        if (this._fade >= 1) {
            this._fadeInComplete();
        }
    },

    _fadeOutTick: function() {
        this._fade -= this._step;
        this._fade = this._fade < 0 ? 0 : this._fade;
        var el = this.get_element();
        if (el) {
            el.style.opacity = this._fade;
            el.style.filter = 'alpha(opacity=' + (this._fade * 100) + ')';
        }
        if (this._fade <= 0) {
            this._fadeOutComplete();
        }
    },

    _dragStart: function(e) {
        e.preventDefault();
        this._clickX = e.clientX;
        this._clickY = e.clientY;
        $addHandler(document, "mousemove", this._onDrag);
        $addHandler(document, "mouseup", this._onDragEnd);
    },

    _drag: function(e) {
        e.preventDefault();
        var el = this.get_element();
        el.style.left = (this._x + e.clientX - this._clickX) + 'px';
        el.style.top = (this._y + e.clientY - this._clickY) + 'px';
    },

    _dragEnd: function(e) {
        e.preventDefault();
        $removeHandler(document, "mousemove", this._onDrag);
        $removeHandler(document, "mouseup", this._onDragEnd);
        var el = this.get_element();
        this._x = parseInt(el.style.left);
        this._y = parseInt(el.style.top);
        this._clickX = null;
        this._clickY = null;
    },

    dispose: function() {
        if (this._lastanim) {
            delete this._lastanim;
        }
        AE.DivWindow.callBaseMethod(this, 'dispose');
        if (this._header) $clearHandlers(this._header);
        if (this._closeTrigger) $clearHandlers(this._closeTrigger);
        if (this._trigger) $clearHandlers(this._trigger);
    }
}

AE.DivWindow.registerClass('AE.DivWindow', Sys.UI.Behavior);

if (typeof(Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();