<%@ Page Language="VB" AutoEventWireup="false" CodeFile="Edit.aspx.vb" MasterPageFile="~/controls/AdminMaster.master" Inherits="Edit"  Title="Committed Purchase Event"%>

<asp:content ContentPlaceHolderId="ph" ID="Content" runat="server">
<asp:Panel ID="pnMain" runat="server">
<link rel="stylesheet" href="/includes/style.css" type="text/css">
<asp:ScriptManager ID="sc" runat="server"></asp:ScriptManager>
<script type="text/javascript" src="/includes/formdnd.js"/></script>

<h4><% If TwoPriceCampaignId = 0 Then%>Add<% Else %>Edit<% End If %> Committed Purchase Event</h4>
    <a href="/admin/twoprice/campaigns/default.aspx" target="main" class="btn campaignBtn" style="text-decoration:none;color:black;">Return to event management</a>

    <asp:PlaceHolder ID="PlaceHolder1" runat="server">
<script type="text/javascript">
    function pageLoad() {
        //Sys.Application.add_init(InitPriceComparison);
    }
    function handleClick(sender,args) {
        ShowSearching();
    }
    function OpenExportForm() {
        Sys.Application.remove_load(OpenExportForm);
        var c = $get('<%=frmExport.ClientID %>').control;
        c.Open();
    }
    function ToggleDetails() {
        var div = $get('divTakeoff');
        $(div).slideToggle('fast', null);
        var lnk = $get('lnkDetails');
        if (lnk.innerHTML == 'Show Details') {
            lnk.innerHTML = 'Hide Details';
        } else {
            lnk.innerHTML = 'Show Details';
        }
    }
    function ShowSearching() {
        var frm = $get('<%=frmLoading.ClientID %>').control;
        frm._doMoveToCenter();
        frm.Open();
    }
    function HideSearching(sender, args) {
        var frm = $get('<%=frmLoading.ClientID %>').control;
        frm.Close();
    }

    /*
* jquery.tools 1.1.2 - The missing UI library for the Web
* 
* [tools.tooltip-1.1.2, tools.tooltip.slide-1.0.0, tools.tooltip.dynamic-1.0.1, tools.scrollable-1.1.2, tools.scrollable.circular-0.5.1, tools.scrollable.autoscroll-1.0.1, tools.scrollable.navigator-1.0.2, tools.scrollable.mousewheel-1.0.1]
* 
* Copyright (c) 2009 Tero Piirainen
* http://flowplayer.org/tools/
*
* Dual licensed under MIT and GPL 2+ licenses
* http://www.opensource.org/licenses
* 
* -----
* 
* jquery.event.wheel.js - rev 1 
* Copyright (c) 2008, Three Dub Media (http://threedubmedia.com)
* Liscensed under the MIT License (MIT-LICENSE.txt)
* http://www.opensource.org/licenses/mit-license.php
* Created: 2008-07-01 | Updated: 2008-07-14
* 
* -----
* 
* File generated: Thu Oct 08 09:36:42 GMT+00:00 2009
*/
    (function (c) { var d = []; c.tools = c.tools || {}; c.tools.tooltip = { version: "1.1.3", conf: { effect: "toggle", fadeOutSpeed: "fast", tip: null, predelay: 0, delay: 30, opacity: 1, lazy: false, position: ["top", "center"], offset: [0, 0], cancelDefault: true, relative: false, oneInstance: true, events: { def: "mouseover,mouseout", input: "focus,blur", widget: "focus mouseover,blur mouseout", tooltip: "mouseover,mouseout" }, api: false }, addEffect: function (e, g, f) { b[e] = [g, f] } }; var b = { toggle: [function (e) { var f = this.getConf(), g = this.getTip(), h = f.opacity; if (h < 1) { g.css({ opacity: h }) } g.show(); e.call() }, function (e) { this.getTip().hide(); e.call() }], fade: [function (e) { this.getTip().fadeIn(this.getConf().fadeInSpeed, e) }, function (e) { this.getTip().fadeOut(this.getConf().fadeOutSpeed, e) }] }; function a(f, g) { var p = this, k = c(this); f.data("tooltip", p); var l = f.next(); if (g.tip) { l = c(g.tip); if (l.length > 1) { l = f.nextAll(g.tip).eq(0); if (!l.length) { l = f.parent().nextAll(g.tip).eq(0) } } } function o(u) { var t = g.relative ? f.position().top : f.offset().top, s = g.relative ? f.position().left : f.offset().left, v = g.position[0]; t -= l.outerHeight() - g.offset[0]; s += f.outerWidth() + g.offset[1]; var q = l.outerHeight() + f.outerHeight(); if (v == "center") { t += q / 2 } if (v == "bottom") { t += q } v = g.position[1]; var r = l.outerWidth() + f.outerWidth(); if (v == "center") { s -= r / 2 } if (v == "left") { s -= r } return { top: t, left: s } } var i = f.is(":input"), e = i && f.is(":checkbox, :radio, select, :button"), h = f.attr("type"), n = g.events[h] || g.events[i ? (e ? "widget" : "input") : "def"]; n = n.split(/,\s*/); if (n.length != 2) { throw "Tooltip: bad events configuration for " + h } f.bind(n[0], function (r) { if (g.oneInstance) { c.each(d, function () { this.hide() }) } var q = l.data("trigger"); if (q && q[0] != this) { l.hide().stop(true, true) } r.target = this; p.show(r); n = g.events.tooltip.split(/,\s*/); l.bind(n[0], function () { p.show(r) }); if (n[1]) { l.bind(n[1], function () { p.hide(r) }) } }); f.bind(n[1], function (q) { p.hide(q) }); if (!c.browser.msie && !i && !g.predelay) { f.mousemove(function () { if (!p.isShown()) { f.triggerHandler("mouseover") } }) } if (g.opacity < 1) { l.css("opacity", g.opacity) } var m = 0, j = f.attr("title"); if (j && g.cancelDefault) { f.removeAttr("title"); f.data("title", j) } c.extend(p, { show: function (r) { if (r) { f = c(r.target) } clearTimeout(l.data("timer")); if (l.is(":animated") || l.is(":visible")) { return p } function q() { l.data("trigger", f); var t = o(r); if (g.tip && j) { l.html(f.data("title")) } r = c.Event(); r.type = "onBeforeShow"; k.trigger(r, [t]); if (r.isDefaultPrevented()) { return p } t = o(r); l.css({ position: "absolute", top: t.top, left: t.left }); var s = b[g.effect]; if (!s) { throw 'Nonexistent effect "' + g.effect + '"' } s[0].call(p, function () { r.type = "onShow"; k.trigger(r) }) } if (g.predelay) { clearTimeout(m); m = setTimeout(q, g.predelay) } else { q() } return p }, hide: function (r) { clearTimeout(l.data("timer")); clearTimeout(m); if (!l.is(":visible")) { return } function q() { r = c.Event(); r.type = "onBeforeHide"; k.trigger(r); if (r.isDefaultPrevented()) { return } b[g.effect][1].call(p, function () { r.type = "onHide"; k.trigger(r) }) } if (g.delay && r) { l.data("timer", setTimeout(q, g.delay)) } else { q() } return p }, isShown: function () { return l.is(":visible, :animated") }, getConf: function () { return g }, getTip: function () { return l }, getTrigger: function () { return f }, bind: function (q, r) { k.bind(q, r); return p }, onHide: function (q) { return this.bind("onHide", q) }, onBeforeShow: function (q) { return this.bind("onBeforeShow", q) }, onShow: function (q) { return this.bind("onShow", q) }, onBeforeHide: function (q) { return this.bind("onBeforeHide", q) }, unbind: function (q) { k.unbind(q); return p } }); c.each(g, function (q, r) { if (c.isFunction(r)) { p.bind(q, r) } }) } c.prototype.tooltip = function (e) { var f = this.eq(typeof e == "number" ? e : 0).data("tooltip"); if (f) { return f } var g = c.extend(true, {}, c.tools.tooltip.conf); if (c.isFunction(e)) { e = { onBeforeShow: e } } else { if (typeof e == "string") { e = { tip: e } } } e = c.extend(true, g, e); if (typeof e.position == "string") { e.position = e.position.split(/,?\s/) } if (e.lazy !== false && (e.lazy === true || this.length > 20)) { this.one("mouseover", function (h) { f = new a(c(this), e); f.show(h); d.push(f) }) } else { this.each(function () { f = new a(c(this), e); d.push(f) }) } return e.api ? f : this } })(jQuery);
    (function (b) { var a = b.tools.tooltip; a.effects = a.effects || {}; a.effects.slide = { version: "1.0.0" }; b.extend(a.conf, { direction: "up", bounce: false, slideOffset: 10, slideInSpeed: 200, slideOutSpeed: 200, slideFade: !b.browser.msie }); var c = { up: ["-", "top"], down: ["+", "top"], left: ["-", "left"], right: ["+", "left"] }; b.tools.tooltip.addEffect("slide", function (d) { var f = this.getConf(), g = this.getTip(), h = f.slideFade ? { opacity: f.opacity } : {}, e = c[f.direction] || c.up; h[e[1]] = e[0] + "=" + f.slideOffset; if (f.slideFade) { g.css({ opacity: 0 }) } g.show().animate(h, f.slideInSpeed, d) }, function (e) { var g = this.getConf(), i = g.slideOffset, h = g.slideFade ? { opacity: 0 } : {}, f = c[g.direction] || c.up; var d = "" + f[0]; if (g.bounce) { d = d == "+" ? "-" : "+" } h[f[1]] = d + "=" + i; this.getTip().animate(h, g.slideOutSpeed, function () { b(this).hide(); e.call() }) }) })(jQuery);
    (function (d) { var c = d.tools.tooltip; c.plugins = c.plugins || {}; c.plugins.dynamic = { version: "1.0.1", conf: { api: false, classNames: "top right bottom left" } }; function b(h) { var e = d(window); var g = e.width() + e.scrollLeft(); var f = e.height() + e.scrollTop(); return [h.offset().top <= e.scrollTop(), g <= h.offset().left + h.width(), f <= h.offset().top + h.height(), e.scrollLeft() >= h.offset().left] } function a(f) { var e = f.length; while (e--) { if (f[e]) { return false } } return true } d.fn.dynamic = function (g) { var h = d.extend({}, c.plugins.dynamic.conf), f; if (typeof g == "number") { g = { speed: g } } g = d.extend(h, g); var e = g.classNames.split(/\s/), i; this.each(function () { if (d(this).tooltip().jquery) { throw "Lazy feature not supported by dynamic plugin. set lazy: false for tooltip" } var j = d(this).tooltip().onBeforeShow(function (n, o) { var m = this.getTip(), l = this.getConf(); if (!i) { i = [l.position[0], l.position[1], l.offset[0], l.offset[1], d.extend({}, l)] } d.extend(l, i[4]); l.position = [i[0], i[1]]; l.offset = [i[2], i[3]]; m.css({ visibility: "hidden", position: "absolute", top: o.top, left: o.left }).show(); var k = b(m); if (!a(k)) { if (k[2]) { d.extend(l, g.top); l.position[0] = "top"; m.addClass(e[0]) } if (k[3]) { d.extend(l, g.right); l.position[1] = "right"; m.addClass(e[1]) } if (k[0]) { d.extend(l, g.bottom); l.position[0] = "bottom"; m.addClass(e[2]) } if (k[1]) { d.extend(l, g.left); l.position[1] = "left"; m.addClass(e[3]) } if (k[0] || k[2]) { l.offset[0] *= -1 } if (k[1] || k[3]) { l.offset[1] *= -1 } } m.css({ visibility: "visible" }).hide() }); j.onShow(function () { var l = this.getConf(), k = this.getTip(); l.position = [i[0], i[1]]; l.offset = [i[2], i[3]] }); j.onHide(function () { var k = this.getTip(); k.removeClass(g.classNames) }); f = j }); return g.api ? f : this } })(jQuery);
    (function (b) { b.tools = b.tools || {}; b.tools.scrollable = { version: "1.1.2", conf: { size: 5, vertical: false, speed: 400, keyboard: true, keyboardSteps: null, disabledClass: "disabled", hoverClass: null, clickable: true, activeClass: "active", easing: "swing", loop: false, items: ".items", item: null, prev: ".prev", next: ".next", prevPage: ".prevPage", nextPage: ".nextPage", api: false } }; var c; function a(o, m) { var r = this, p = b(this), d = !m.vertical, e = o.children(), k = 0, i; if (!c) { c = r } b.each(m, function (s, t) { if (b.isFunction(t)) { p.bind(s, t) } }); if (e.length > 1) { e = b(m.items, o) } function l(t) { var s = b(t); return m.globalNav ? s : o.parent().find(t) } o.data("finder", l); var f = l(m.prev), h = l(m.next), g = l(m.prevPage), n = l(m.nextPage); b.extend(r, { getIndex: function () { return k }, getClickIndex: function () { var s = r.getItems(); return s.index(s.filter("." + m.activeClass)) }, getConf: function () { return m }, getSize: function () { return r.getItems().size() }, getPageAmount: function () { return Math.ceil(this.getSize() / m.size) }, getPageIndex: function () { return Math.ceil(k / m.size) }, getNaviButtons: function () { return f.add(h).add(g).add(n) }, getRoot: function () { return o }, getItemWrap: function () { return e }, getItems: function () { return e.children(m.item) }, getVisibleItems: function () { return r.getItems().slice(k, k + m.size) }, seekTo: function (s, w, t) { if (s < 0) { s = 0 } if (k === s) { return r } if (b.isFunction(w)) { t = w } if (s > r.getSize() - m.size) { return m.loop ? r.begin() : this.end() } var u = r.getItems().eq(s); if (!u.length) { return r } var v = b.Event("onBeforeSeek"); p.trigger(v, [s]); if (v.isDefaultPrevented()) { return r } if (w === undefined || b.isFunction(w)) { w = m.speed } function x() { if (t) { t.call(r, s) } p.trigger("onSeek", [s]) } if (d) { e.animate({ left: -u.position().left }, w, m.easing, x) } else { e.animate({ top: -u.position().top }, w, m.easing, x) } c = r; k = s; v = b.Event("onStart"); p.trigger(v, [s]); if (v.isDefaultPrevented()) { return r } f.add(g).toggleClass(m.disabledClass, s === 0); h.add(n).toggleClass(m.disabledClass, s >= r.getSize() - m.size); return r }, move: function (u, t, s) { i = u > 0; return this.seekTo(k + u, t, s) }, next: function (t, s) { return this.move(1, t, s) }, prev: function (t, s) { return this.move(-1, t, s) }, movePage: function (w, v, u) { i = w > 0; var s = m.size * w; var t = k % m.size; if (t > 0) { s += (w > 0 ? -t : m.size - t) } return this.move(s, v, u) }, prevPage: function (t, s) { return this.movePage(-1, t, s) }, nextPage: function (t, s) { return this.movePage(1, t, s) }, setPage: function (t, u, s) { return this.seekTo(t * m.size, u, s) }, begin: function (t, s) { i = false; return this.seekTo(0, t, s) }, end: function (t, s) { i = true; var u = this.getSize() - m.size; return u > 0 ? this.seekTo(u, t, s) : r }, reload: function () { p.trigger("onReload"); return r }, focus: function () { c = r; return r }, click: function (u) { var v = r.getItems().eq(u), s = m.activeClass, t = m.size; if (u < 0 || u >= r.getSize()) { return r } if (t == 1) { if (m.loop) { return r.next() } if (u === 0 || u == r.getSize() - 1) { i = (i === undefined) ? true : !i } return i === false ? r.prev() : r.next() } if (t == 2) { if (u == k) { u-- } r.getItems().removeClass(s); v.addClass(s); return r.seekTo(u, time, fn) } if (!v.hasClass(s)) { r.getItems().removeClass(s); v.addClass(s); var x = Math.floor(t / 2); var w = u - x; if (w > r.getSize() - t) { w = r.getSize() - t } if (w !== u) { return r.seekTo(w) } } return r }, bind: function (s, t) { p.bind(s, t); return r }, unbind: function (s) { p.unbind(s); return r } }); b.each("onBeforeSeek,onStart,onSeek,onReload".split(","), function (s, t) { r[t] = function (u) { return r.bind(t, u) } }); f.addClass(m.disabledClass).click(function () { r.prev() }); h.click(function () { r.next() }); n.click(function () { r.nextPage() }); if (r.getSize() < m.size) { h.add(n).addClass(m.disabledClass) } g.addClass(m.disabledClass).click(function () { r.prevPage() }); var j = m.hoverClass, q = "keydown." + Math.random().toString().substring(10); r.onReload(function () { if (j) { r.getItems().hover(function () { b(this).addClass(j) }, function () { b(this).removeClass(j) }) } if (m.clickable) { r.getItems().each(function (s) { b(this).unbind("click.scrollable").bind("click.scrollable", function (t) { if (b(t.target).is("a")) { return } return r.click(s) }) }) } if (m.keyboard) { b(document).unbind(q).bind(q, function (t) { if (t.altKey || t.ctrlKey) { return } if (m.keyboard != "static" && c != r) { return } var u = m.keyboardSteps; if (d && (t.keyCode == 37 || t.keyCode == 39)) { r.move(t.keyCode == 37 ? -u : u); return t.preventDefault() } if (!d && (t.keyCode == 38 || t.keyCode == 40)) { r.move(t.keyCode == 38 ? -u : u); return t.preventDefault() } return true }) } else { b(document).unbind(q) } }); r.reload() } b.fn.scrollable = function (d) { var e = this.eq(typeof d == "number" ? d : 0).data("scrollable"); if (e) { return e } var f = b.extend({}, b.tools.scrollable.conf); d = b.extend(f, d); d.keyboardSteps = d.keyboardSteps || d.size; this.each(function () { e = new a(b(this), d); b(this).data("scrollable", e) }); return d.api ? e : this } })(jQuery);
    (function (b) { var a = b.tools.scrollable; a.plugins = a.plugins || {}; a.plugins.circular = { version: "0.5.1", conf: { api: false, clonedClass: "cloned" } }; b.fn.circular = function (e) { var d = b.extend({}, a.plugins.circular.conf), c; b.extend(d, e); this.each(function () { var i = b(this).scrollable(), n = i.getItems(), k = i.getConf(), f = i.getItemWrap(), j = 0; if (i) { c = i } if (n.length < k.size) { return false } n.slice(0, k.size).each(function (o) { b(this).clone().appendTo(f).click(function () { i.click(n.length + o) }).addClass(d.clonedClass) }); var l = b.makeArray(n.slice(-k.size)).reverse(); b(l).each(function (o) { b(this).clone().prependTo(f).click(function () { i.click(-o - 1) }).addClass(d.clonedClass) }); var m = f.children(k.item); var h = k.hoverClass; if (h) { m.hover(function () { b(this).addClass(h) }, function () { b(this).removeClass(h) }) } function g(o) { var p = m.eq(o); if (k.vertical) { f.css({ top: -p.position().top }) } else { f.css({ left: -p.position().left }) } } g(k.size); b.extend(i, { move: function (s, r, p, q) { var u = j + s + k.size; var t = u > i.getSize() - k.size; if (u <= 0 || t) { var o = j + k.size + (t ? -n.length : n.length); g(o); u = o + s } if (q) { m.removeClass(k.activeClass).eq(u + Math.floor(k.size / 2)).addClass(k.activeClass) } if (u === j + k.size) { return self } return i.seekTo(u, r, p) }, begin: function (p, o) { return this.seekTo(k.size, p, o) }, end: function (p, o) { return this.seekTo(n.length, p, o) }, click: function (p, r, q) { if (!k.clickable) { return self } if (k.size == 1) { return this.next() } var s = p - j, o = k.activeClass; s -= Math.floor(k.size / 2); return this.move(s, r, q, true) }, getIndex: function () { return j }, setPage: function (p, q, o) { return this.seekTo(p * k.size + k.size, q, o) }, getPageAmount: function () { return Math.ceil(n.length / k.size) }, getPageIndex: function () { if (j < 0) { return this.getPageAmount() - 1 } if (j >= n.length) { return 0 } var o = (j + k.size) / k.size - 1; return o }, getVisibleItems: function () { var o = j + k.size; return m.slice(o, o + k.size) } }); i.onStart(function (p, o) { j = o - k.size; return false }); i.getNaviButtons().removeClass(k.disabledClass) }); return d.api ? c : this } })(jQuery);
    (function (b) { var a = b.tools.scrollable; a.plugins = a.plugins || {}; a.plugins.autoscroll = { version: "1.0.1", conf: { autoplay: true, interval: 3000, autopause: true, steps: 1, api: false } }; b.fn.autoscroll = function (d) { if (typeof d == "number") { d = { interval: d } } var e = b.extend({}, a.plugins.autoscroll.conf), c; b.extend(e, d); this.each(function () { var g = b(this).scrollable(); if (g) { c = g } var i, f, h = true; g.play = function () { if (i) { return } h = false; i = setInterval(function () { g.move(e.steps) }, e.interval); g.move(e.steps) }; g.pause = function () { i = clearInterval(i) }; g.stop = function () { g.pause(); h = true }; if (e.autopause) { g.getRoot().add(g.getNaviButtons()).hover(function () { g.pause(); clearInterval(f) }, function () { if (!h) { f = setTimeout(g.play, e.interval) } }) } if (e.autoplay) { setTimeout(g.play, e.interval) } }); return e.api ? c : this } })(jQuery);
    (function (b) { var a = b.tools.scrollable; a.plugins = a.plugins || {}; a.plugins.navigator = { version: "1.0.2", conf: { navi: ".navi", naviItem: null, activeClass: "active", indexed: false, api: false, idPrefix: null } }; b.fn.navigator = function (d) { var e = b.extend({}, a.plugins.navigator.conf), c; if (typeof d == "string") { d = { navi: d } } d = b.extend(e, d); this.each(function () { var i = b(this).scrollable(), f = i.getRoot(), l = f.data("finder").call(null, d.navi), g = null, k = i.getNaviButtons(); if (i) { c = i } i.getNaviButtons = function () { return k.add(l) }; function j() { if (!l.children().length || l.data("navi") == i) { l.empty(); l.data("navi", i); for (var m = 0; m < i.getPageAmount() ; m++) { l.append(b("<" + (d.naviItem || "a") + "/>")) } g = l.children().each(function (n) { var o = b(this); o.click(function (p) { i.setPage(n); return p.preventDefault() }); if (d.indexed) { o.text(n) } if (d.idPrefix) { o.attr("id", d.idPrefix + n) } }) } else { g = d.naviItem ? l.find(d.naviItem) : l.children(); g.each(function (n) { var o = b(this); o.click(function (p) { i.setPage(n); return p.preventDefault() }) }) } g.eq(0).addClass(d.activeClass) } i.onStart(function (o, n) { var m = d.activeClass; g.removeClass(m).eq(i.getPageIndex()).addClass(m) }); i.onReload(function () { j() }); j(); var h = g.filter("[href=" + location.hash + "]"); if (h.length) { i.move(g.index(h)) } }); return d.api ? c : this } })(jQuery);
    (function (b) { b.fn.wheel = function (e) { return this[e ? "bind" : "trigger"]("wheel", e) }; b.event.special.wheel = { setup: function () { b.event.add(this, d, c, {}) }, teardown: function () { b.event.remove(this, d, c) } }; var d = !b.browser.mozilla ? "mousewheel" : "DOMMouseScroll" + (b.browser.version < "1.9" ? " mousemove" : ""); function c(e) { switch (e.type) { case "mousemove": return b.extend(e.data, { clientX: e.clientX, clientY: e.clientY, pageX: e.pageX, pageY: e.pageY }); case "DOMMouseScroll": b.extend(e, e.data); e.delta = -e.detail / 3; break; case "mousewheel": e.delta = e.wheelDelta / 120; break } e.type = "wheel"; return b.event.handle.call(this, e, e.delta) } var a = b.tools.scrollable; a.plugins = a.plugins || {}; a.plugins.mousewheel = { version: "1.0.1", conf: { api: false, speed: 50 } }; b.fn.mousewheel = function (f) { var g = b.extend({}, a.plugins.mousewheel.conf), e; if (typeof f == "number") { f = { speed: f } } f = b.extend(g, f); this.each(function () { var h = b(this).scrollable(); if (h) { e = h } h.getRoot().wheel(function (i, j) { h.move(j < 0 ? 1 : -1, f.speed || 50); return false }) }); return f.api ? e : this } })(jQuery);
    (function (c) { c.tools = c.tools || {}; c.tools.overlay = { version: "1.1.2", addEffect: function (e, f, g) { b[e] = [f, g] }, conf: { top: "10%", left: "center", absolute: false, speed: "normal", closeSpeed: "fast", effect: "default", close: null, oneInstance: true, closeOnClick: true, closeOnEsc: true, api: false, expose: null, target: null } }; var b = {}; c.tools.overlay.addEffect("default", function (e) { this.getOverlay().fadeIn(this.getConf().speed, e) }, function (e) { this.getOverlay().fadeOut(this.getConf().closeSpeed, e) }); var d = []; function a(g, k) { var o = this, m = c(this), n = c(window), j, i, h, e = k.expose && c.tools.expose.version; var f = k.target || g.attr("rel"); i = f ? c(f) : null || g; if (!i.length) { throw "Could not find Overlay: " + f } if (g && g.index(i) == -1) { g.click(function (p) { o.load(p); return p.preventDefault() }) } c.each(k, function (p, q) { if (c.isFunction(q)) { m.bind(p, q) } }); c.extend(o, { load: function (u) { if (o.isOpened()) { return o } var r = b[k.effect]; if (!r) { throw 'Overlay: cannot find effect : "' + k.effect + '"' } if (k.oneInstance) { c.each(d, function () { this.close(u) }) } u = u || c.Event(); u.type = "onBeforeLoad"; m.trigger(u); if (u.isDefaultPrevented()) { return o } h = true; if (e) { i.expose().load(u) } var t = k.top; var s = k.left; var p = i.outerWidth({ margin: true }); var q = i.outerHeight({ margin: true }); if (typeof t == "string") { t = t == "center" ? Math.max((n.height() - q) / 2, 0) : parseInt(t, 10) / 100 * n.height() } if (s == "center") { s = Math.max((n.width() - p) / 2, 0) } if (!k.absolute) { t += n.scrollTop(); s += n.scrollLeft() } i.css({ top: t, left: s, position: "absolute" }); u.type = "onStart"; m.trigger(u); r[0].call(o, function () { if (h) { u.type = "onLoad"; m.trigger(u) } }); if (k.closeOnClick) { c(document).bind("click.overlay", function (w) { if (!o.isOpened()) { return } var v = c(w.target); if (v.parents(i).length > 1) { return } c.each(d, function () { this.close(w) }) }) } if (k.closeOnEsc) { c(document).unbind("keydown.overlay").bind("keydown.overlay", function (v) { if (v.keyCode == 27) { c.each(d, function () { this.close(v) }) } }) } return o }, close: function (q) { if (!o.isOpened()) { return o } q = q || c.Event(); q.type = "onBeforeClose"; m.trigger(q); if (q.isDefaultPrevented()) { return } h = false; b[k.effect][1].call(o, function () { q.type = "onClose"; m.trigger(q) }); var p = true; c.each(d, function () { if (this.isOpened()) { p = false } }); if (p) { c(document).unbind("click.overlay").unbind("keydown.overlay") } return o }, getContent: function () { return i }, getOverlay: function () { return i }, getTrigger: function () { return g }, getClosers: function () { return j }, isOpened: function () { return h }, getConf: function () { return k }, bind: function (p, q) { m.bind(p, q); return o }, unbind: function (p) { m.unbind(p); return o } }); c.each("onBeforeLoad,onStart,onLoad,onBeforeClose,onClose".split(","), function (p, q) { o[q] = function (r) { return o.bind(q, r) } }); if (e) { if (typeof k.expose == "string") { k.expose = { color: k.expose } } c.extend(k.expose, { api: true, closeOnClick: k.closeOnClick, closeOnEsc: false }); var l = i.expose(k.expose); l.onBeforeClose(function (p) { o.close(p) }); o.onClose(function (p) { l.close(p) }) } j = i.find(k.close || ".close"); if (!j.length && !k.close) { j = c('<div class="close"></div>'); i.prepend(j) } j.click(function (p) { o.close(p) }) } c.fn.overlay = function (e) { var f = this.eq(typeof e == "number" ? e : 0).data("overlay"); if (f) { return f } if (c.isFunction(e)) { e = { onBeforeLoad: e } } var g = c.extend({}, c.tools.overlay.conf); e = c.extend(true, g, e); this.each(function () { f = new a(c(this), e); d.push(f); c(this).data("overlay", f) }); return e.api ? f : this } })(jQuery);
    (function (d) { var b = d.tools.overlay; b.effects = b.effects || {}; b.effects.apple = { version: "1.0.1" }; d.extend(b.conf, { start: { absolute: true, top: null, left: null }, fadeInSpeed: "fast", zIndex: 99999999 }); function c(f) { var g = f.offset(); return [g.top + f.height() / 2, g.left + f.width() / 2] } var e = function (n) { var k = this.getOverlay(), f = this.getConf(), i = this.getTrigger(), q = this, r = k.outerWidth({ margin: true }), m = k.data("img"); if (!m) { var l = k.css("backgroundImage"); if (!l) { throw "background-image CSS property not set for overlay" } l = l.substring(l.indexOf("(") + 1, l.indexOf(")")).replace(/\"/g, ""); k.css("backgroundImage", "none"); m = d('<img src="' + l + '"/>'); m.css({ border: 0, position: "absolute", display: "none" }).width(r); d("body").append(m); k.data("img", m) } var o = d(window), j = f.start.top || Math.round(o.height() / 2), h = f.start.left || Math.round(o.width() / 2); if (i) { var g = c(i); j = g[0]; h = g[1] } if (!f.start.absolute) { j += o.scrollTop(); h += o.scrollLeft() } m.css({ top: j, left: h, width: 0, zIndex: f.zIndex }).show(); m.animate({ top: k.css("top"), left: k.css("left"), width: r }, f.speed, function () { k.css("zIndex", f.zIndex + 1).fadeIn(f.fadeInSpeed, function () { if (q.isOpened() && !d(this).index(k)) { n.call() } else { k.hide() } }) }) }; var a = function (f) { var h = this.getOverlay(), i = this.getConf(), g = this.getTrigger(), l = i.start.top, k = i.start.left; h.hide(); if (g) { var j = c(g); l = j[0]; k = j[1] } h.data("img").animate({ top: l, left: k, width: 0 }, i.closeSpeed, f) }; b.addEffect("apple", e, a) })(jQuery);
    (function (b) { b.tools = b.tools || {}; b.tools.expose = { version: "1.0.5", conf: { maskId: null, loadSpeed: "slow", closeSpeed: "fast", closeOnClick: true, closeOnEsc: true, zIndex: 9998, opacity: 0.8, color: "#456", api: false } }; function a() { if (b.browser.msie) { var f = b(document).height(), e = b(window).height(); return [window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth, f - e < 20 ? e : f] } return [b(window).width(), b(document).height()] } function c(h, g) { var e = this, j = b(this), d = null, f = false, i = 0; b.each(g, function (k, l) { if (b.isFunction(l)) { j.bind(k, l) } }); b(window).resize(function () { e.fit() }); b.extend(this, { getMask: function () { return d }, getExposed: function () { return h }, getConf: function () { return g }, isLoaded: function () { return f }, load: function (n) { if (f) { return e } i = h.eq(0).css("zIndex"); if (g.maskId) { d = b("#" + g.maskId) } if (!d || !d.length) { var l = a(); d = b("<div/>").css({ position: "absolute", top: 0, left: 0, width: l[0], height: l[1], display: "none", opacity: 0, zIndex: g.zIndex }); if (g.maskId) { d.attr("id", g.maskId) } b("body").append(d); var k = d.css("backgroundColor"); if (!k || k == "transparent" || k == "rgba(0, 0, 0, 0)") { d.css("backgroundColor", g.color) } if (g.closeOnEsc) { b(document).bind("keydown.unexpose", function (o) { if (o.keyCode == 27) { e.close() } }) } if (g.closeOnClick) { d.bind("click.unexpose", function (o) { e.close(o) }) } } n = n || b.Event(); n.type = "onBeforeLoad"; j.trigger(n); if (n.isDefaultPrevented()) { return e } b.each(h, function () { var o = b(this); if (!/relative|absolute|fixed/i.test(o.css("position"))) { o.css("position", "relative") } }); h.css({ zIndex: Math.max(g.zIndex + 1, i == "auto" ? 0 : i) }); var m = d.height(); if (!this.isLoaded()) { d.css({ opacity: 0, display: "block" }).fadeTo(g.loadSpeed, g.opacity, function () { if (d.height() != m) { d.css("height", m) } n.type = "onLoad"; j.trigger(n) }) } f = true; return e }, close: function (k) { if (!f) { return e } k = k || b.Event(); k.type = "onBeforeClose"; j.trigger(k); if (k.isDefaultPrevented()) { return e } d.fadeOut(g.closeSpeed, function () { k.type = "onClose"; j.trigger(k); h.css({ zIndex: b.browser.msie ? i : null }) }); f = false; return e }, fit: function () { if (d) { var k = a(); d.css({ width: k[0], height: k[1] }) } }, bind: function (k, l) { j.bind(k, l); return e }, unbind: function (k) { j.unbind(k); return e } }); b.each("onBeforeLoad,onLoad,onBeforeClose,onClose".split(","), function (k, l) { e[l] = function (m) { return e.bind(l, m) } }) } b.fn.expose = function (d) { var e = this.eq(typeof d == "number" ? d : 0).data("expose"); if (e) { return e } if (typeof d == "string") { d = { color: d } } var f = b.extend({}, b.tools.expose.conf); d = b.extend(f, d); this.each(function () { e = new c(b(this), d); b(this).data("expose", e) }); return d.api ? e : this } })(jQuery);


    $(document).ready(function () {
        $('.tooltip').tooltip({
            position: 'bottom right',
            relative: true,
            track: true
        });
    });
</script>

<style type="text/css">
    /* Tooltip Classes */
    .adminToolTipWrpr {display:none;width:340px;z-index:99; line-height: 15px; text-align: left;}
    .adminToolTipShadow {background: url(/images/tooltipbacktop.png) no-repeat top left;padding: 10px 13px 1px 13px;color:#666666;}
    .adminToolTipShadow h3 { line-height: 25px; }
    .adminToolTopShadowBottom {background: url(/images/tooltipbackbottom.png) no-repeat top left;width:340px;height:16px;}
    .adminToolTipDate {font-size:11px;color:#999999;font-weight:bold;}
    .adminToolTipName {font-size:14px;color:#666666;font-weight:bold;margin-bottom:14px;}
    /* Tooltip Classes */
</style>
</asp:PlaceHolder>

<CC:PopupForm ID="frmLoading" runat="server" CssClass="pform" Animate="false" ShowVeil="true" VeilCloses="false">
    <FormTemplate>
        <div style="background-color:#fff;width:200px;height:80px;text-align:center;padding:30px 10px;">
            <img src="/images/loading.gif" alt="Processing..." /><br /><br />
            <h1 class="largest">Processing... Please Wait</h1>
        </div>
    </FormTemplate>
</CC:PopupForm>

<CC:PopupForm ID="frmExport" runat="server" ShowVeil="true" VeilCloses="true" CssClass="pform" OpenMode="MoveToCenter" width="300px" CloseTriggerId="btnCloseExport">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin:0px;">
            <div class="pckghdgred">Price Comparison Export</div>
            <p class="center bold" style="margin:10px;">
                <a id="lnkExportFile" runat="server">Click here to download export file.</a><br /><br />
                <asp:Button id="btnCloseExport" runat="server" cssclass="btnred" text="Close" />
            </p>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlID="btnCloseExport" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

    <div class="slctwrpr">
        <div class="pckghdgred nobdr">Campaign Price Comparison </div>
        <table class="slctcomr" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td class="bold">Selected Vendors</td>
                <td class="bold">Table Legend</td>
                <td class="bold">&nbsp;</td>
            </tr>
            <tr>
                <td width="40%">
                    <asp:Literal id="ltlVendors" runat="server"></asp:Literal>
                </td>
                <td>
                    <table class="complgndtbl" cellSpacing="1" cellPadding="0" border="0">
                        <tr>
                            <td class="desc">Lowest Price</td>
                            <td style="padding: 0px;"><div class="clgnd1" style="background:#ebebeb; padding: 2px;"><img src="/images/global/icon-clgnd1.gif" border="0" alt="" /></div></td>
                        </tr>
                        <tr>
                            <td class="desc">Highest Price</td>
                            <td style="padding: 0px;"><div class="clgnd2" style="background:#ebebeb; padding: 2px;"><img src="/images/global/icon-clgnd2.gif" border="0" alt="" /></div></td>
                        </tr>
                        <tr>
                            <td class="desc">Vendor Comment</td>
                            <td style="padding: 0px;"><div class="clgnd3" style="background:#ebebeb; padding: 2px;text-align:right;height:17px;"><span  class="fa fa-comment"></span></div></td>
                        </tr>
                    </table>
                </td>
                <td>
                </td>
            </tr>
        </table>
    </div>    
    <div class="pckggraywrpr">
        <div class="pckghdgred nobdr">Compare Prices </div>
        <table id="tblPriced" runat="server" cellspacing="1" cellpadding="0" border="0" class="tblcompr">        
            <tr><td class="shim" style="width:44px;"><img src="/images/spacer.gif" style="width:44px; height:1px;" alt="" /><br /></td><td class="shim" style="width:89px;"><img src="/images/spacer.gif" style="width:89px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:250px;"><img src="/images/spacer.gif" style="width:250px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td></tr>            
            <tr>
                <th colspan="8">Products All Vendors Priced</th>
            </tr>
            <tr id="trPricedHeader" runat="server">
				<th>Qty</th>
				<th>Sku</th>
				<th>Product Name</th>
			</tr>
        </table>

        <asp:Button ID="btnExport" runat="server" Text="Export Comparison" CssClass="btnred" />
        
        <table id="tblSub" runat="server" cellspacing="1" cellpadding="0" border="0" class="tblcompr comprsub">
		    <tr><td class="shim" style="width:44px;"><img src="/images/spacer.gif" style="width:44px; height:1px;" alt="" /><br /></td><td class="shim" style="width:89px;"><img src="/images/spacer.gif" style="width:89px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:250px;"><img src="/images/spacer.gif" style="width:250px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td></tr>            
		    <tr>
				<th colspan="8">Products Priced with Some Vendors Providing Substitutions</th>
			</tr>
            <tr id="trSubHeader" runat="server">
				<th>Qty</th>
				<th>Sku</th>
				<th>Product Name</th>
			</tr>
        </table>
        
        <table id="tblAvg" runat="server" cellspacing="1" cellpadding="0" border="0" class="tblcompr compravg">
            <tr><td class="shim" style="width:44px;"><img src="/images/spacer.gif" style="width:44px; height:1px;" alt="" /><br /></td><td class="shim" style="width:89px;"><img src="/images/spacer.gif" style="width:89px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:250px;"><img src="/images/spacer.gif" style="width:250px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td></tr>            
		    <tr>
				<th colspan="8">Products Some Vendors Did Not Price</th>
            </tr>
            <tr id="trAvgHeader" runat="server">
				<th>Qty</th>
				<th>Sku</th>
				<th>Product Name</th>
			</tr>
                    
        </table>
        
        <table id="tblSpec" runat="server" cellspacing="1" cellpadding="0" border="0" class="tblcompr">
            <tr><td class="shim" style="width:44px;"><img src="/images/spacer.gif" style="width:44px; height:1px;" alt="" /><br /></td><td class="shim" style="width:89px;"><img src="/images/spacer.gif" style="width:89px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:250px;"><img src="/images/spacer.gif" style="width:250px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td> <td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td></tr>            
		    <tr>
				<th colspan="8">Special Order Products</th>
            </tr>
            <tr id="trSpecHeader" runat="server">
				<th>Qty</th>
				<th>Sku</th>
				<th>Product Name</th>
			</tr>
        </table>

        <table id="tblSum" runat="server" cellspacing="1" cellpadding="0" border="0" class="tblcompr comprsum">        
			<tr>
				<td class="shim" style="width:383px;"><img src="/images/spacer.gif" style="width:383px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td><td class="shim" style="width:112px;"><img src="/images/spacer.gif" style="width:112px; height:1px;" alt="" /><br /></td>
			</tr>
            <tr id="trSumHeader" runat="server">
				<th>Summary</th>
            </tr>
        </table>
    </div>
    
    <CC:UnvalidatedPostback ID="btnAward" runat="server"></CC:UnvalidatedPostback>
    <CC:UnvalidatedPostback ID="btnRequestPricing" runat="server"></CC:UnvalidatedPostback>
    </asp:panel>
    
        <asp:Panel ID="pnlSendMessage" runat="server" Visible="false">
        <h2>Send Automatic Messages</h2>
            <fieldset style="width:45%;display: inline-block; vertical-align: middle;">
                <legend>Vendor Message Format:</legend>
                <div class="field">
                    <asp:Literal ID="ltlVendorAutoMessage" runat="server"></asp:Literal>
                </div>
            </fieldset>
            <fieldset style="width:45%;display: inline-block; vertical-align: middle;">
                <legend>Builder Message Format:</legend>
                <div class="field">
                    <asp:Literal ID="ltlBuilderAutoMessage" runat="server"></asp:Literal>
                </div>
            </fieldset>
            <fieldset style="width:45%;display: inline-block; vertical-align: middle;">
                <legend>Vendor Additional Message Text:</legend>
                <div class="field">
                    <asp:Textbox ID="txtMessage" runat="server" Columns="60" Rows="10" MaxLength="255" TextMode="MultiLine"></asp:Textbox>
                </div>
                
            </fieldset>
            <fieldset style="width:45%;display: inline-block; vertical-align: middle;">
                <legend>Builder Additional Message Text:</legend>
                <div class="field">
                    <asp:Textbox ID="txtMessageBuilder" runat="server" Columns="60" Rows="10" MaxLength="255" TextMode="MultiLine"></asp:Textbox>
                </div>
            </fieldset>
            <CC:ConfirmButton ID="btnSendMessage" runat="server" CssClass="btn" Text="Send Message" Message="Are you sure you would like to continue, this Campaign's status will become 'Awarded'?"/>    
    </asp:Panel>
        <asp:Panel ID="pnComplete" runat="server" Visible="false">
        <div>
            Thank you, your campaign emails have been sent to the appropriate Vendor.
        </div>
    </asp:Panel>
</asp:content>
