Type.registerNamespace("AE");

AE.PopupForm = function(element) {
    AE.PopupForm.initializeBase(this, [element]);

    this._InputIds = [];
    this._ValidatorIds = [];

    this._inputs = {};
    this._validators = [];
    this._buttons = [];

    this._moveToCenter = false;
    this._moveToClick = false;
    this._animate = false;
    this._showVeil = false;
    this._veilCloses = false;

    this._veil = null;

    this._openTrigger = null;
    this._closeTrigger = null;
    this._errorPlaceholder = null;
}

AE.PopupForm.prototype = {

	initialize: function() {
		AE.PopupForm.callBaseMethod(this, 'initialize');

		this._handleClickDel = Function.createDelegate(this, this._handleClick);
		this._openDel = Function.createDelegate(this, this.Open);
		this._closeDel = Function.createDelegate(this, this.Close);
		this._callbackResultDel = Function.createDelegate(this, this._callbackResult);
		//this.GetElementByServerId = Function.createDelegate(this, this._getElementByServerId);

		this._window = $get(this.get_element().id + '_window');

		for (var i = 0; i < this._InputIds.length; i++) {
			this._inputs[this._InputIds[i].serverId] = $get(this._InputIds[i].clientId);
		}
		for (var i = 0; i < this._ValidatorIds.length; i++) {
			this._validators.push($get(this._ValidatorIds[i]));
		}
		for (var i = 0; i < this._buttons.length; i++) {
			var btn = $get(this._buttons[i].id);
			btn.button = this._buttons[i];
			$addHandler(btn, 'click', this._handleClickDel);
		}
		if (this._openTrigger)
			$addHandler(this._openTrigger, 'click', this._openDel);
		if (this._closeTrigger)
			$addHandler(this._closeTrigger, 'click', this._closeDel);

		//if (this._showVeil) {
		//    this._initVeil();
		//}
	},
	get_inputs: function() {
		return this._InputIds;
	},
	set_inputs: function(value) {
		this._InputIds = value;
	},
	get_validators: function() {
		return this._ValidatorIds;
	},
	set_validators: function(value) {
		this._ValidatorIds = value;
	},
	get_input: function(id) {
		return this._inputs[id];
	},
	get_buttons: function() {
		return this._buttons;
	},
	set_buttons: function(value) {
		this._buttons = value;
	},
	get_moveToCenter: function() {
		return this._moveToCenter;
	},
	set_moveToCenter: function(value) {
		this._moveToCenter = value;
	},
	get_moveToClick: function() {
		return this._moveToClick;
	},
	set_moveToClick: function(value) {
		this._moveToClick = value;
	},
	get_animate: function() {
		return this._animate;
	},
	set_animate: function(value) {
		this._animate = value;
	},
	get_openTrigger: function() {
		return this._openTrigger;
	},
	set_openTrigger: function(value) {
		this._openTrigger = value;
	},
	get_closeTrigger: function() {
		return this._closeTrigger;
	},
	set_closeTrigger: function(value) {
		this._closeTrigger = value;
	},
	get_showVeil: function() {
		return this._showVeil;
	},
	set_showVeil: function(value) {
		this._showVeil = value;
	},
	get_veilCloses: function() {
		return this._veilCloses;
	},
	set_veilCloses: function(value) {
		this._veilCloses = value;
	},
	get_errorPlaceholder: function() {
		return this._errorPlaceholder;
	},
	set_errorPlaceholder: function(value) {
		this._errorPlaceholder = value;
	},

	IsValid: function() {
		var isValid = true;
		for (v in this._validators) {
			isValid &= v.validate();
		}
		return isValid;
	},

	GetData: function() {
		var data = {};
		for (i in this._inputs) {
			if (this._inputs[i] != null) {
				data[i] = getValue(this._inputs[i]);
			}
		}
		this.get_element().value = Sys.Serialization.JavaScriptSerializer.serialize(data);
		return this.get_element().value;
	},

	GetElementByServerId: function(id) {
		return $get(this.get_element().id + '_' + id);
	},

	_callbackResult: function(res, ctxt) {
		var cb;
		try {
			cb = Sys.Serialization.JavaScriptSerializer.deserialize(res);
		} catch (e) {
			cb = res;
		}
		if (typeof cb === 'object' && cb.errors) {
			this._errorPlaceholder.innerHTML = cb.errorMsg;
		}
		if (ctxt.button.button.clientCallback) {
			ctxt.button.button.clientCallback(cb, ctxt);
		}
	},

	Open: function(e) {
		if (e) e.preventDefault();
		this._onOpen();
		if (this._showVeil) {
			this._initVeil();
			this._veil.style.display = '';
		}
		if (this._moveToCenter) {
			this._doMoveToCenter();
		} else if (this._moveToClick && e) {
			this._doMoveToClick(e);
		}
		if (this._animate) {
			$(this._window).fadeIn('fast', this._reDoMove);
		} else {
			$(this._window).show();
			this._reDoMove();
		}
	},

	_reDoMove: function(e) {
		if (this._moveToCenter) {
			this._doMoveToCenter();
		} else if (this._moveToClick && e) {
			this._doMoveToClick(e);
		}
	},

	_onOpen: function() {
		var h = this.get_events().getHandler('open');
		if (h) h(this, Sys.EventArgs.Empty);
	},

	add_open: function(h) {
		this.get_events().addHandler(h);
	},

	Close: function() {
		this._onClose();
		if (this._veil) {
			this._veil.style.display = 'none';
		}
		if (this._animate) {
			$(this._window).fadeOut('fast', null);
		} else {
			$(this._window).hide();
		}
	},

	_onClose: function() {
		var h = this.get_events().getHandler('close');
		if (h) h(this, Sys.EventArgs.Empty);
	},

	add_close: function(h) {
		this.get_events().addHandler(h);
	},

	_handleClick: function(e) {
		e.preventDefault();
		if (e.target.button.onclick) {
			if (typeof e.target.button.onclick === 'function') {
				e.target.button.onclick(e);
			} else {
				eval(e.target.button.onclick);
			}
		}
		if (e.target.button.callback) {
			var ctxt = { 'control': this, 'button': e.target };
			var callbackArg = this._escape(e.target.button.serverId) + "|" + this._escape(this.GetData());
			var callbackResultDel = this._callbackResultDel;
			eval(e.target.button.callback);
		} else if (e.target.button.postback) {
			eval(e.target.button.postback);
		}
		return false;
	},

	_escape: function(str) {
		return str.replace("\\", "\\\\").replace("|", "\\|");
	},

	_initVeil: function() {
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
				$addHandler(this._veil, 'click', this._closeDel);
			}
		} else
			v = this._veil;
		var w, h;
		w = document.body.offsetWidth;
		if (window.innerHeight && window.scrollMaxY !== undefined) {
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

	_doMoveToCenter: function() {
		this._centered = true;
		var el = this._window;
		el.style.top = '';
		el.style.left = '';
		var w = this._getWinWidth();
		var h = this._getWinHeight();
		var loc = Sys.UI.DomElement.getLocation(el);
		var offX = loc.x;
		var offY = loc.y;
		offX = document.documentElement.scrollLeft > 0 ? document.documentElement.scrollLeft : document.body.scrollLeft;
		offY = document.documentElement.scrollTop > 0 ? document.documentElement.scrollTop : document.body.scrollTop;
		if (el.offsetParent) {
			var loc = Sys.UI.DomElement.getLocation(el.offsetParent);
			if (loc.x >= 0 && loc.y >= 0) {
				offX += offX - loc.x;
				offY += offY - loc.y;
			}
		}
		//var offX = 0;
		//var offY = 0;
		var elWidth = el.style.width ? parseInt(el.style.width) : null;
		var elHeight = el.style.height ? parseInt(el.style.height) : null;
		if (!elWidth || !elHeight) {
			var vis = el.style.visibility;
			var pos = el.style.position;
			var disp = el.style.display;
			el.style.visibility = 'hidden';
			el.style.position = 'absolute';
			el.style.display = 'inline';
			if (!elWidth) {
				elWidth = el.clientWidth;
			}
			if (!elHeight) {
				elHeight = el.clientHeight;
			}
			el.style.top = '0px';
			el.style.display = disp;
			el.style.position = pos;
			el.style.visibility = vis;
		}
		var lft = Math.round((w / 2) - (elWidth / 2) + offX);
		var top = Math.round((h / 2) - (elHeight / 2) + offY);
		if (lft < 0 || top < 0) {
			lft = 50;
			top = 50;
		}
		el.style.left = lft + 'px';
		el.style.top = top + 'px';
	},

	_doMoveToClick: function(e) {
		var el = this._window;
		//var offset = this.getDocumentOffset();
		var offset = el.offsetParent ? Sys.UI.DomElement.getLocation(el.offsetParent) : Sys.UI.DomElement.getLocation(el);
		var scrollX = document.documentElement != 'undefined' ? document.documentElement.scrollLeft : document.body.scrollLeft;
		var scrollY = document.documentElement != 'undefined' ? document.documentElement.scrollTop : document.body.scrollTop;
		var elWidth = el.style.width ? parseInt(el.style.width) : null;
		var elHeight = el.style.height ? parseInt(el.style.height) : null;
		if (!elWidth || !elHeight) {
			var vis = el.style.visibility;
			var pos = el.style.position;
			var disp = el.style.display;
			el.style.visibility = 'hidden';
			el.style.position = 'absolute';
			el.style.display = '';
			if (!elWidth) {
				elWidth = el.clientWidth;
			}
			if (!elHeight) {
				elHeight = el.clientHeight;
			}
			el.style.display = disp;
			el.style.position = pos;
			el.style.visibility = vis;
		}
		var cx = e.clientX;
		var cy = e.clientY;
		var winWidth = this._getWinWidth();
		var winHeight = this._getWinHeight();
		if (cx - elWidth / 2 - 25 < 0) {
			cx += Math.abs(cx - elWidth / 2 - 25);
		}
		if (cx + elWidth / 2 > winWidth - 25) {
			cx -= cx + elWidth / 2 - winWidth + 25;
		}
		if (cy - elHeight / 2 - 25 < 0) {
			cy += Math.abs(cy - elHeight / 2 - 25);
		}
		if (cy + elHeight / 2 > winHeight - 25) {
			cy -= cy + elHeight / 2 - winHeight + 25;
		}
		el.style.left = Math.round(cx - offset.x + (2 * scrollX) - elWidth / 2) + 'px';
		el.style.top = (cy - offset.y + (2 * scrollY) - elHeight / 2) + 'px';
	},
	_getWinWidth: function() {
		return window.innerWidth ? window.innerWidth : (document.documentElement.clientWidth ? document.documentElement.clientWidth : document.body.clientWidth);
	},

	_getWinHeight: function() {
		return window.innerHeight ? window.innerHeight : (document.documentElement.clientHeight ? document.documentElement.clientHeight : document.body.clientHeight);
	}
}

AE.PopupForm.registerClass('AE.PopupForm', Sys.UI.Control);

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();



