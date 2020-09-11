<%@ Page Language="VB" AutoEventWireup="false" CodeFile="edit.aspx.vb" Inherits="takeoffs_edit" EnableEventValidation="false" %>

<%@ Register TagName="ProductsToCompareForm" TagPrefix="CC" Src="~/controls/ProductsToCompareForm.ascx" %>
<%@ Register Assembly="NineRays.WebControls.FlyTreeView" Namespace="NineRays.WebControls" TagPrefix="NineRays" %>
<%@ Register TagName="CurrentTakeOffs" TagPrefix="CC" Src="~/modules/twoprice/currenttakeoffs.ascx" %>
<%@ Register TagName="Search" TagPrefix="CC" Src="~/controls/SearchSql.ascx" %>

<CT:MasterPage ID="CTMain" runat="server">

<asp:PlaceHolder runat="server">
<script src="/includes/formdnd.js" type="text/javascript" ></script>
<script src="/includes/jquery.tablesorter.js" type="text/javascript" ></script>

<script type="text/javascript">
    function UpdateDrops(show) {
        if (show) {
            $get('trDrops').style.display = '';
            $get('trDelivery').style.display = 'none';
        } else {
            $get('trDrops').style.display = 'none';
            $get('trDelivery').style.display = '';
        }
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
    (function (b) { var a = b.tools.scrollable; a.plugins = a.plugins || {}; a.plugins.navigator = { version: "1.0.2", conf: { navi: ".navi", naviItem: null, activeClass: "active", indexed: false, api: false, idPrefix: null } }; b.fn.navigator = function (d) { var e = b.extend({}, a.plugins.navigator.conf), c; if (typeof d == "string") { d = { navi: d } } d = b.extend(e, d); this.each(function () { var i = b(this).scrollable(), f = i.getRoot(), l = f.data("finder").call(null, d.navi), g = null, k = i.getNaviButtons(); if (i) { c = i } i.getNaviButtons = function () { return k.add(l) }; function j() { if (!l.children().length || l.data("navi") == i) { l.empty(); l.data("navi", i); for (var m = 0; m < i.getPageAmount(); m++) { l.append(b("<" + (d.naviItem || "a") + "/>")) } g = l.children().each(function (n) { var o = b(this); o.click(function (p) { i.setPage(n); return p.preventDefault() }); if (d.indexed) { o.text(n) } if (d.idPrefix) { o.attr("id", d.idPrefix + n) } }) } else { g = d.naviItem ? l.find(d.naviItem) : l.children(); g.each(function (n) { var o = b(this); o.click(function (p) { i.setPage(n); return p.preventDefault() }) }) } g.eq(0).addClass(d.activeClass) } i.onStart(function (o, n) { var m = d.activeClass; g.removeClass(m).eq(i.getPageIndex()).addClass(m) }); i.onReload(function () { j() }); j(); var h = g.filter("[href=" + location.hash + "]"); if (h.length) { i.move(g.index(h)) } }); return d.api ? c : this } })(jQuery);
    (function (b) { b.fn.wheel = function (e) { return this[e ? "bind" : "trigger"]("wheel", e) }; b.event.special.wheel = { setup: function () { b.event.add(this, d, c, {}) }, teardown: function () { b.event.remove(this, d, c) } }; var d = !b.browser.mozilla ? "mousewheel" : "DOMMouseScroll" + (b.browser.version < "1.9" ? " mousemove" : ""); function c(e) { switch (e.type) { case "mousemove": return b.extend(e.data, { clientX: e.clientX, clientY: e.clientY, pageX: e.pageX, pageY: e.pageY }); case "DOMMouseScroll": b.extend(e, e.data); e.delta = -e.detail / 3; break; case "mousewheel": e.delta = e.wheelDelta / 120; break } e.type = "wheel"; return b.event.handle.call(this, e, e.delta) } var a = b.tools.scrollable; a.plugins = a.plugins || {}; a.plugins.mousewheel = { version: "1.0.1", conf: { api: false, speed: 50 } }; b.fn.mousewheel = function (f) { var g = b.extend({}, a.plugins.mousewheel.conf), e; if (typeof f == "number") { f = { speed: f } } f = b.extend(g, f); this.each(function () { var h = b(this).scrollable(); if (h) { e = h } h.getRoot().wheel(function (i, j) { h.move(j < 0 ? 1 : -1, f.speed || 50); return false }) }); return f.api ? e : this } })(jQuery);
    (function (c) { c.tools = c.tools || {}; c.tools.overlay = { version: "1.1.2", addEffect: function (e, f, g) { b[e] = [f, g] }, conf: { top: "10%", left: "center", absolute: false, speed: "normal", closeSpeed: "fast", effect: "default", close: null, oneInstance: true, closeOnClick: true, closeOnEsc: true, api: false, expose: null, target: null } }; var b = {}; c.tools.overlay.addEffect("default", function (e) { this.getOverlay().fadeIn(this.getConf().speed, e) }, function (e) { this.getOverlay().fadeOut(this.getConf().closeSpeed, e) }); var d = []; function a(g, k) { var o = this, m = c(this), n = c(window), j, i, h, e = k.expose && c.tools.expose.version; var f = k.target || g.attr("rel"); i = f ? c(f) : null || g; if (!i.length) { throw "Could not find Overlay: " + f } if (g && g.index(i) == -1) { g.click(function (p) { o.load(p); return p.preventDefault() }) } c.each(k, function (p, q) { if (c.isFunction(q)) { m.bind(p, q) } }); c.extend(o, { load: function (u) { if (o.isOpened()) { return o } var r = b[k.effect]; if (!r) { throw 'Overlay: cannot find effect : "' + k.effect + '"' } if (k.oneInstance) { c.each(d, function () { this.close(u) }) } u = u || c.Event(); u.type = "onBeforeLoad"; m.trigger(u); if (u.isDefaultPrevented()) { return o } h = true; if (e) { i.expose().load(u) } var t = k.top; var s = k.left; var p = i.outerWidth({ margin: true }); var q = i.outerHeight({ margin: true }); if (typeof t == "string") { t = t == "center" ? Math.max((n.height() - q) / 2, 0) : parseInt(t, 10) / 100 * n.height() } if (s == "center") { s = Math.max((n.width() - p) / 2, 0) } if (!k.absolute) { t += n.scrollTop(); s += n.scrollLeft() } i.css({ top: t, left: s, position: "absolute" }); u.type = "onStart"; m.trigger(u); r[0].call(o, function () { if (h) { u.type = "onLoad"; m.trigger(u) } }); if (k.closeOnClick) { c(document).bind("click.overlay", function (w) { if (!o.isOpened()) { return } var v = c(w.target); if (v.parents(i).length > 1) { return } c.each(d, function () { this.close(w) }) }) } if (k.closeOnEsc) { c(document).unbind("keydown.overlay").bind("keydown.overlay", function (v) { if (v.keyCode == 27) { c.each(d, function () { this.close(v) }) } }) } return o }, close: function (q) { if (!o.isOpened()) { return o } q = q || c.Event(); q.type = "onBeforeClose"; m.trigger(q); if (q.isDefaultPrevented()) { return } h = false; b[k.effect][1].call(o, function () { q.type = "onClose"; m.trigger(q) }); var p = true; c.each(d, function () { if (this.isOpened()) { p = false } }); if (p) { c(document).unbind("click.overlay").unbind("keydown.overlay") } return o }, getContent: function () { return i }, getOverlay: function () { return i }, getTrigger: function () { return g }, getClosers: function () { return j }, isOpened: function () { return h }, getConf: function () { return k }, bind: function (p, q) { m.bind(p, q); return o }, unbind: function (p) { m.unbind(p); return o } }); c.each("onBeforeLoad,onStart,onLoad,onBeforeClose,onClose".split(","), function (p, q) { o[q] = function (r) { return o.bind(q, r) } }); if (e) { if (typeof k.expose == "string") { k.expose = { color: k.expose } } c.extend(k.expose, { api: true, closeOnClick: k.closeOnClick, closeOnEsc: false }); var l = i.expose(k.expose); l.onBeforeClose(function (p) { o.close(p) }); o.onClose(function (p) { l.close(p) }) } j = i.find(k.close || ".close"); if (!j.length && !k.close) { j = c('<div class="close"></div>'); i.prepend(j) } j.click(function (p) { o.close(p) }) } c.fn.overlay = function (e) { var f = this.eq(typeof e == "number" ? e : 0).data("overlay"); if (f) { return f } if (c.isFunction(e)) { e = { onBeforeLoad: e } } var g = c.extend({}, c.tools.overlay.conf); e = c.extend(true, g, e); this.each(function () { f = new a(c(this), e); d.push(f); c(this).data("overlay", f) }); return e.api ? f : this } })(jQuery);
    (function (d) { var b = d.tools.overlay; b.effects = b.effects || {}; b.effects.apple = { version: "1.0.1" }; d.extend(b.conf, { start: { absolute: true, top: null, left: null }, fadeInSpeed: "fast", zIndex: 99999999 }); function c(f) { var g = f.offset(); return [g.top + f.height() / 2, g.left + f.width() / 2] } var e = function (n) { var k = this.getOverlay(), f = this.getConf(), i = this.getTrigger(), q = this, r = k.outerWidth({ margin: true }), m = k.data("img"); if (!m) { var l = k.css("backgroundImage"); if (!l) { throw "background-image CSS property not set for overlay" } l = l.substring(l.indexOf("(") + 1, l.indexOf(")")).replace(/\"/g, ""); k.css("backgroundImage", "none"); m = d('<img src="' + l + '"/>'); m.css({ border: 0, position: "absolute", display: "none" }).width(r); d("body").append(m); k.data("img", m) } var o = d(window), j = f.start.top || Math.round(o.height() / 2), h = f.start.left || Math.round(o.width() / 2); if (i) { var g = c(i); j = g[0]; h = g[1] } if (!f.start.absolute) { j += o.scrollTop(); h += o.scrollLeft() } m.css({ top: j, left: h, width: 0, zIndex: f.zIndex }).show(); m.animate({ top: k.css("top"), left: k.css("left"), width: r }, f.speed, function () { k.css("zIndex", f.zIndex + 1).fadeIn(f.fadeInSpeed, function () { if (q.isOpened() && !d(this).index(k)) { n.call() } else { k.hide() } }) }) }; var a = function (f) { var h = this.getOverlay(), i = this.getConf(), g = this.getTrigger(), l = i.start.top, k = i.start.left; h.hide(); if (g) { var j = c(g); l = j[0]; k = j[1] } h.data("img").animate({ top: l, left: k, width: 0 }, i.closeSpeed, f) }; b.addEffect("apple", e, a) })(jQuery);
    (function (b) { b.tools = b.tools || {}; b.tools.expose = { version: "1.0.5", conf: { maskId: null, loadSpeed: "slow", closeSpeed: "fast", closeOnClick: true, closeOnEsc: true, zIndex: 9998, opacity: 0.8, color: "#456", api: false } }; function a() { if (b.browser.msie) { var f = b(document).height(), e = b(window).height(); return [window.innerWidth || document.documentElement.clientWidth || document.body.clientWidth, f - e < 20 ? e : f] } return [b(window).width(), b(document).height()] } function c(h, g) { var e = this, j = b(this), d = null, f = false, i = 0; b.each(g, function (k, l) { if (b.isFunction(l)) { j.bind(k, l) } }); b(window).resize(function () { e.fit() }); b.extend(this, { getMask: function () { return d }, getExposed: function () { return h }, getConf: function () { return g }, isLoaded: function () { return f }, load: function (n) { if (f) { return e } i = h.eq(0).css("zIndex"); if (g.maskId) { d = b("#" + g.maskId) } if (!d || !d.length) { var l = a(); d = b("<div/>").css({ position: "absolute", top: 0, left: 0, width: l[0], height: l[1], display: "none", opacity: 0, zIndex: g.zIndex }); if (g.maskId) { d.attr("id", g.maskId) } b("body").append(d); var k = d.css("backgroundColor"); if (!k || k == "transparent" || k == "rgba(0, 0, 0, 0)") { d.css("backgroundColor", g.color) } if (g.closeOnEsc) { b(document).bind("keydown.unexpose", function (o) { if (o.keyCode == 27) { e.close() } }) } if (g.closeOnClick) { d.bind("click.unexpose", function (o) { e.close(o) }) } } n = n || b.Event(); n.type = "onBeforeLoad"; j.trigger(n); if (n.isDefaultPrevented()) { return e } b.each(h, function () { var o = b(this); if (!/relative|absolute|fixed/i.test(o.css("position"))) { o.css("position", "relative") } }); h.css({ zIndex: Math.max(g.zIndex + 1, i == "auto" ? 0 : i) }); var m = d.height(); if (!this.isLoaded()) { d.css({ opacity: 0, display: "block" }).fadeTo(g.loadSpeed, g.opacity, function () { if (d.height() != m) { d.css("height", m) } n.type = "onLoad"; j.trigger(n) }) } f = true; return e }, close: function (k) { if (!f) { return e } k = k || b.Event(); k.type = "onBeforeClose"; j.trigger(k); if (k.isDefaultPrevented()) { return e } d.fadeOut(g.closeSpeed, function () { k.type = "onClose"; j.trigger(k); h.css({ zIndex: b.browser.msie ? i : null }) }); f = false; return e }, fit: function () { if (d) { var k = a(); d.css({ width: k[0], height: k[1] }) } }, bind: function (k, l) { j.bind(k, l); return e }, unbind: function (k) { j.unbind(k); return e } }); b.each("onBeforeLoad,onLoad,onBeforeClose,onClose".split(","), function (k, l) { e[l] = function (m) { return e.bind(l, m) } }) } b.fn.expose = function (d) { var e = this.eq(typeof d == "number" ? d : 0).data("expose"); if (e) { return e } if (typeof d == "string") { d = { color: d } } var f = b.extend({}, b.tools.expose.conf); d = b.extend(f, d); this.each(function () { e = new c(b(this), d); b(this).data("expose", e) }); return d.api ? e : this } })(jQuery);



    function OpenNonSpecial() {
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=frmNonSpecial.BehaviorId%>'), '<%=frmNonSpecial.BehaviorName%>');
        b.fadeIn();
        return false;
    }

    function OpenPrevTakeOffs() {
        var b = Sys.UI.Behavior.getBehaviorByName($get('<%=frmPrevTakeoffs.BehaviorId%>'), '<%=frmPrevTakeoffs.BehaviorName%>');
        b.moveToCenter()
        b.fadeIn();
        return false;
    }

    function OpenSubForm(takeoffProductId, vendorId, name, sku, price, qty) {
        var c = $get('<%=frmSubstitute.ClientID %>').control;
        c.GetElementByServerId('spanProduct').innerHTML = name;
        c.GetElementByServerId('spanSubSku').innerHTML = sku ? sku : 'N/A';
        c.GetElementByServerId('spanSubPrice').innerHTML = price;
        c.get_input('txtSubQuantity').value = qty;
        c.get_input('hdnSubTakeoffProductID').value = takeoffProductId;
        c.get_input('hdnSubVendorID').value = vendorId;
        c._doMoveToCenter();
        c.Open();
        return false;
    }

    function OpenSpecialForm(ProductType, takeoffProductId, vendorId, name) {
        var c = $get('<%=frmAverage.ClientID %>').control;
        c.GetElementByServerId('spanAverageProduct').innerHTML = name;
        c.get_input('hdnAvgTakeoffProductID').value = takeoffProductId;
        c.get_input('hdnAvgVendorID').value = vendorId;
        c.get_input('hdnProductType').value = ProductType;
        c._doMoveToCenter();
        c.Open();
    }

    function OpenPriceReqFormBeforeReq(vendorId) {                /*ProductType, takeoffProductId, vendorId, name*/
        var c = $get('<%=PopupFormOrderSubmit.ClientID %>').control;
        //c.GetElementByServerId('span1Product').innerHTML = name;
        //c.get_input('hdnReq1TakeoffProductID').value = takeoffProductId;
        c.get_input('hdnReq1VendorID').value = vendorId;
        //c.get_input('hdnreq1ProductType').value = ProductType;
        c._doMoveToCenter();
        c.Open();
    }

    function OpenPriceReqFormAfterReq(vendorId) {                  /*ProductType, takeoffProductId, vendorId, name*/
        var c = $get('<%=PopUpFormOrderPriceReq.ClientID %>').control;
        //c.GetElementByServerId('span2Product').innerHTML = name;
        //c.get_input('hdnReq2TakeoffProductID').value = takeoffProductId;
        c.get_input('hdnReq2VendorID').value = vendorId;
        //c.get_input('hdnreq2ProductType').value = ProductType;
        c._doMoveToCenter();
        c.Open();
    }
    function OpenReqPopup() {
        //
        var HdnPriceReq = ('<%=PriceReqStatus.Value %>');
        var _VendorId = '<%= CurrentOrder.VendorId %>';
        //alert(_VendorId);
        if (HdnPriceReq == 1) {
            OpenPriceReqFormAfterReq(_VendorId);
            //alert('here');
            return false;
        }
        else if (HdnPriceReq == 2) {
            OpenPriceReqFormBeforeReq(_VendorId);
            //alert('here2');
            return false;
        }
        else {
            return true;
        }
    }
    $(document).ready(function () {
        $('.tooltip').tooltip({
            position: 'bottom right',
            relative: true,
            track: false
        });

        //  $("#gvList").tablesorter({ sortList: [[2, 0]] });
        //OpenPriceReqFormAfterReq();
        $("#gvList").tablesorter();

        $('#<%= BtnPriceReqCancel.ClientID %>').click(function () {
        //alert('ok');
        location.href = location.href;
    })
    $("#<%= BtnOrderReqCancel.ClientID %>").click(function () {
        //alert('ok');
        location.href = location.href;
        })
        
    });
    
    function FilterTakeOff(ctl) {
        if (ctl.PostbackTimer) {
            window.clearTimeout(ctl.PostbackTimer);
        }
        $get('<%=hdnPostback.ClientID %>').value = ctl.value;
        ctl.PostbackTimer = window.setTimeout('<%=Page.ClientScript.GetPostBackEventReference(hdnPostback, "").Replace("'", "\'") %>', 500);
    }

    function ClearTakeOffFilter() {
        $get('<%=txtTakeOffFilter.ClientID %>').value = '';

        FilterTakeOff('<%=txtTakeOffFilter %>');
    }

    function DisablePricingRequestButton() {
        var RequestPricingStatus = document.getElementById("hdnRequestPricingStatus").value;

        if (RequestPricingStatus == "True") {
            document.getElementById("btnRequestPricingAll").disabled = true;
            return false;
        }

        document.getElementById("hdnRequestPricingStatus").value = "True";
        document.getElementById("btnRequestPricingAll").style.color = "lightgrey";
        document.getElementById("btnRequestPricingAll").style.borderColor = "lightgrey";
        document.getElementById("btnRequestPricingAll").style.cursor = "none";

        alert("Missing Price request has been sent to the Vendor");

        return true;
    }
</script>
        <style type="text/css">
            /* Tooltip Classes */
            .adminToolTipWrpr {
                display: none;
                width: 340px;
                z-index: 99;
                line-height: 15px;
                text-align: left;
            }

            .adminToolTipShadow {
                background: url(/images/tooltipbacktop.png) no-repeat top left;
                padding: 10px 13px 1px 13px;
                color: #666666;
            }

                .adminToolTipShadow h3 {
                    line-height: 25px;
                }

            .adminToolTopShadowBottom {
                background: url(/images/tooltipbackbottom.png) no-repeat top left;
                width: 340px;
                height: 16px;
            }

            .adminToolTipDate {
                font-size: 11px;
                color: #999999;
                font-weight: bold;
            }

            .adminToolTipName {
                font-size: 14px;
                color: #666666;
                font-weight: bold;
                margin-bottom: 14px;
            }

            .window2 {
                top: 20px !important;
            }

            .header-center {
                text-align: center;
            }
            .desired-product-table table.tbltodata td {
                vertical-align: middle;
                padding: 6px 10px;
            }
            .desired-product-table table td {
                border-top: 1px solid #f7f7f7;
            }
            .desired-product-table h2 {
                font-size:12px;
            }
            .desired-product-table h2 + div table {
                width:100%;
            }
            .desired-product-table h2 + div {
                overflow-y:auto;
                max-height:100px;
            }
            	
            /* Tooltip Classes */
        </style>
    
</asp:PlaceHolder>
<span id="trace"></span>

    <div align='center'>
        <asp:literal id="ltlTwoPriceInfo" runat="server" />
        <br />
       <asp:Literal id="ltlTakeoffTitle" runat="server"></asp:Literal>
        <%--<div style="width:60%;margin:0px auto;position:relative;background-color:#fff;border:1px solid #666;">
                        Current Take Loaded Is:
                        <br />
                        <h1 style="font-size:16px;"></h1>
         </div>--%>
         <table class="slctcomr"  runat ="server" id ="tblLegend" style="margin-top:10px; margin-left:65px;" align="left" cellpadding="0" cellspacing="0" border="0">
            <tr>
                <td>
                    <table class="complgndtbl" cellSpacing="2" cellPadding="0" border="0" style="border:solid 1px;">
                        <tr>
                            <td class="desc" style="vertical-align:middle;">Action Button</td>
                            <td style="padding: 0px;"><div class="clgnd7" style="background:#ebebeb; padding: 5px;"><img src="/images/global/icon-message.gif" style="height:23px;width:23px;" border="0" alt="" /></div></td>

                            <td class="desc" style="vertical-align:middle;">Product is Vendor's Substitution</td>
                            <td style="padding: 0px;"><div class="clgnd5" style="background:#FFCCCC; padding: 10px; "><img border="0" alt="" src="/images/global/twoprice/icon-substitution.png"></div></td>
                        
                            <td class="desc" style="vertical-align:middle;">No Pricing from Vendor</td>
                            <td style="padding: 0px;"><div class="clgnd6" style="background:#BCC79A; padding: 10px;"><img src="/images/global/twoprice/icon-nopricing.png" border="0" alt="" /></div></td>

                            <td class="desc" style="vertical-align:middle;">No Pricing: Request Pending</td>
                            <td style="padding: 0px;"><div class="clgnd7" style="background:#26e7f1; padding: 11px;"><img src="/images/global/twoprice/icon-pending.png" border="0" alt="" /></div></td>
                            
                            <td class="desc" style="vertical-align:middle;">Catalog Pricing</td>
                            <td style="padding: 0px;"><div class="clgnd7" style="background:Yellow; padding: 16px;"></div></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>      
            </div>
<nStuff:UpdateHistory ID="ctlHistory" runat="server"></nStuff:UpdateHistory>
    <asp:Panel id="pnBuildOrder" runat="server">
</CT:MasterPage><CC:PopupForm ID="frmLoading" runat="server" Animate="false" CssClass="pform" ShowVeil="true" VeilCloses="false">
    <FormTemplate>
        <div style="background-color: #fff; width: 200px; height: 80px; text-align: center; padding: 30px 10px;">
            <img alt="Processing..." src="/images/loading.gif" /><br />
            <br />
            <h1 class="largest">Processing... Please Wait</h1></div></FormTemplate></CC:PopupForm><CC:PopupForm ID="frmAverage" runat="server" CloseTriggerId="btnCancelAverage" ShowVeil='true' ValidateCallback="true" ErrorPlaceholderId="spanAverageErrors" VeilCloses="false" CssClass="pform" Style="text-align: center;">
    <FormTemplate>
        <div class="pckggraywrpr" style="width: 450px; margin-bottom: 0px;">
            <div class="pckghdgred">Pricing Options</div><table cellpadding="4" cellspacing="0" border="0" style="background-color: Transparent; width: 100%">
                <tr>
                    <td style="padding: 0px; margin: 0px;" colspan="2"><span id="spanAverageErrors" runat="server"></span></td>
                </tr>
                <tr>
                    <td colspan="2" class="largest bold">Vendor has not priced this item.  What do you want to do? </td></tr><tr>
                    <td><b>Product:</b></td><td><span id="spanAverageProduct" runat="server"></span></td>
                </tr>

                <tr>
                    <td colspan="2" style="text-align: center;">
                        <asp:button id="btnAcceptAverage" runat="server" cssclass="btnred" commandname="Accept" text="Keep in order and accept unknown price" />
                        <br />
                        <br />
                        <asp:button id="btnRequestAverage" runat="server" cssclass="btnred" commandname="Request" text="Request pricing from this vendor" />
                        <br />
                        <br />
                        <asp:button id="btnOmitAverage" runat="server" cssclass="btnred" commandname="Omit" text="Remove from order" />
                        <br />
                        <br />
                        <asp:button id="btnCancelAverage" runat="server" cssclass="btnred" text="Cancel" />
                        <br />
                        <br />
                        <asp:hiddenfield id="hdnAvgTakeoffProductID" runat="server"></asp:hiddenfield>
                        <asp:hiddenfield id="hdnAvgVendorID" runat="server"></asp:hiddenfield>
                        <asp:hiddenfield id="hdnProductType" runat="server"></asp:hiddenfield>
                    </td>
                </tr>
            </table>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlId="btnAcceptAverage" ButtonType="Postback" />
        <CC:PopupFormButton ControlId="btnOmitAverage" ButtonType="Postback" />
        <CC:PopupFormButton ControlId="btnRequestAverage" ButtonType="Postback" />
        <CC:PopupFormButton ControlId="btnCancelAverage" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>


<CC:PopupForm ID="frmSubstitute" runat="server" ShowVeil="true" ValidateCallback="true" ErrorPlaceholderId="spanErrors" VeilCloses="true" CssClass="pform" Animate="true" OpenMode="MoveToCenter" CloseTriggerId="btnReject">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin-bottom: 0px;">
            <div class="pckghdgred" style="height: 15px;">Substitute Product Options</div><table cellpadding="2" cellspacing="0" border="0">
                <tr>
                    <td style="padding: 0px; margin: 0px;" colspan="2"><span id="spanErrors" runat="server">
                        <img src="/images/spacer.gif" alt="" style="height: 1px;" /></span></td>
                </tr>
                <tr>
                    <td>Product:</td><td><span id="spanProduct" runat="server"></span></td>
                </tr>
                <tr>
                    <td>Substitute SKU:</td><td><span id="spanSubSku" runat="server"></span></td>
                </tr>
                <tr>
                    <td>Unit Price:</td><td><span id="spanSubPrice" runat="server"></span></td>
                </tr>
                <tr>
                    <td>Quantity:</td><td>
                        <asp:textbox id="txtSubQuantity" runat="server"></asp:textbox></td></tr><tr>
                    <td colspan="2" style="padding-top: 0px;" class="smallest">Default is Vendor Recommendation</td></tr><tr>
                    <td colspan="2">
                        <asp:button id="btnAccept" runat="server" cssclass="btnred" text="Accept" commandname="Accept" />
                        <asp:button id="btnReject" runat="server" cssclass="btnred" text="Reject" commandname="Reject" />
                        <asp:hiddenfield id="hdnSubTakeoffProductID" runat="server"></asp:hiddenfield>
                        <asp:hiddenfield id="hdnSubVendorID" runat="server"></asp:hiddenfield>
                    </td>
                </tr>
            </table>
            <CC:IntegerValidatorFront ID="ivSubQuantity" runat="server" ControlToValidate="txtSubQuantity" ErrorMessage="Quantity is invalid"></CC:IntegerValidatorFront></div></FormTemplate><Buttons>
        <CC:PopupFormButton ControlId="btnAccept" ButtonType="Postback" />
        <CC:PopupFormButton ControlID="btnReject" ButtonType="Postback" />
    </Buttons>
</CC:PopupForm>

<CC:PopupForm ID="PopupFormOrderSubmit" runat="server" CloseTriggerId="" ShowVeil='true' ValidateCallback="true" ErrorPlaceholderId="spanReqErrors" VeilCloses="false" CssClass="pform" Style="text-align: center;">
    <FormTemplate>
        <div class="pckggraywrpr" style="width: 450px; margin-bottom: 0px;">
            <div class="pckghdgred">Pricing Options</div><table cellpadding="4" cellspacing="0" border="0" style="background-color: Transparent; width: 100%">
                <tr>
                    <td style="padding: 0px; margin: 0px;" colspan="2"><span id="spanReqErrors" runat="server"></span></td>
                </tr>
                <tr>
                    <td colspan="2" class="largest bold">Your request has unpriced items. What would you like to do? </td></tr><tr>
                    <%--<td><b>Product:</b></td>
                    <td><span id="span1Product" runat="server"></span></td>--%>
                </tr>

                <tr>
                    <td colspan="2" style="text-align: center;">
                        <asp:button id="BtnPriceReqAccept" runat="server" cssclass="btnred" commandname="Accept" text="Submit order with unpriced items" />
                        <br />
                        <br />
                        <asp:button id="BtnPriceReqRequest" runat="server" cssclass="btnred" commandname="Request" text="Request pricing from vendor" />
                        <br />
                        <br />
                        <asp:button id="BtnPriceReqOmit" runat="server" cssclass="btnred" commandname="Omit" text="Remove unpriced items and submit order" />
                        <br />
                        <br />
                        <asp:button id="BtnPriceReqCancel" runat="server" cssclass="btnred" text="Cancel and edit order" />
                        <br />
                        <br />
                        <asp:hiddenfield id="hdnReq1TakeoffProductID" runat="server"></asp:hiddenfield>
                        <asp:hiddenfield id="hdnReq1VendorID" runat="server"></asp:hiddenfield>
                        <asp:hiddenfield id="hdnreq1ProductType" runat="server"></asp:hiddenfield>
                    </td>
                </tr>
            </table>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlId="BtnPriceReqAccept" ButtonType="Postback" />
        <CC:PopupFormButton ControlId="BtnPriceReqOmit" ButtonType="Postback" />
        <CC:PopupFormButton ControlId="BtnPriceReqRequest" ButtonType="Postback" />
        <CC:PopupFormButton ControlId="BtnPriceReqCancel" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>

<CC:PopupForm ID="PopUpFormOrderPriceReq" runat="server" CloseTriggerId="" ShowVeil='true' ValidateCallback="true" ErrorPlaceholderId="spanPriceReqErrors" VeilCloses="false" CssClass="pform" Style="text-align: center;">
    <FormTemplate>
        <div class="pckggraywrpr" style="width: 450px; margin-bottom: 0px;">
            <div class="pckghdgred">Pricing Options</div><table cellpadding="4" cellspacing="0" border="0" style="background-color: Transparent; width: 100%">
                <tr>
                    <td style="padding: 0px; margin: 0px;" colspan="2"><span id="spanPriceReqErrors" runat="server"></span></td>
                </tr>
                <tr>
                    <td colspan="2" class="largest bold">You have requested pricing from the vendor. What would you like to do? </td></tr><tr>
                    <%--<td><b>Product:</b></td>
                    <td><span id="span2Product" runat="server"></span></td>--%>
                </tr>

                <tr>
                    <td colspan="2" style="text-align: center;">
                        <asp:button id="BtnOrderReqAccept" runat="server" cssclass="btnred" commandname="Accept" text="Submit order with unpriced items" />
                        <%--<br />
                        <br />
                        <asp:button id="Button2" runat="server" cssclass="btnred" commandname="Request" text="Request pricing from vendor" />--%>
                        <br />
                        <br />
                        <asp:button id="BtnOrderReqOmit" runat="server" cssclass="btnred" commandname="Omit" text="Remove unpriced items and submit order" />
                        <br />
                        <br />
                        <asp:button id="BtnOrderReqCancel" runat="server" cssclass="btnred" text="Cancel and edit order" />
                        <br />
                        <br />
                        <asp:hiddenfield id="hdnReq2TakeoffProductID" runat="server"></asp:hiddenfield>
                        <asp:hiddenfield id="hdnReq2VendorID" runat="server"></asp:hiddenfield>
                        <asp:hiddenfield id="hdnreq2ProductType" runat="server"></asp:hiddenfield>
                    </td>
                </tr>
            </table>
        </div>
    </FormTemplate>
    <Buttons>
        <CC:PopupFormButton ControlId="BtnOrderReqAccept" ButtonType="Postback" />
        <CC:PopupFormButton ControlId="BtnOrderReqOmit" ButtonType="Postback" />
        <%--<CC:PopupFormButton ControlId="btnRequestAverage" ButtonType="Postback" />--%>
        <CC:PopupFormButton ControlId="BtnOrderReqCancel" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>


<CC:DivWindow ID="frmPrevTakeoffs" runat="server" TargetControlID="divPrevTakeoffs" CloseTriggerId="btnCloseWin" ShowVeil="false" VeilCloses="true" MoveToTop="false" />
<div id="divPrevTakeoffs" runat="server" class="window window2" style="height: 575px;">
    <div class="pckggraywrpr automargin">
        <div class="pckghdgred">Add Products From Existing TakeOffs</div><div style="height: 18px; padding-top: 5px; padding-left: 14px; padding-bottom: 10px;">
            Search TakeOff:&nbsp;<asp:textbox id="txtTakeOffFilter" runat="server" onkeyup="FilterTakeOff(this);"></asp:textbox><a style="cursor: pointer;" onclick="ClearTakeOffFilter();">Clear</a> </div><div style="height: 460px; overflow-y: scroll;">
            <asp:updatepanel id="updatePnl" runat="server" updatemode="Conditional">
                <ContentTemplate>
                    <CC:GridView ID="gvList" CellSpacing="2" CellPadding="2" CssClass="tblcompr" runat="server" PageSize="3200" SortBy ="Saved" SortOrder="Desc" AllowPaging="True" AllowSorting="True" HeaderText="In order to change display order, please use header links" EmptyDataText="There are no records that match the search criteria" AutoGenerateColumns="False" BorderWidth="0" >
                        <HeaderStyle CssClass="sbttl" />
                        <AlternatingRowStyle CssClass="alternate" VerticalAlign="Top" Font-Size="Larger"></AlternatingRowStyle>
                        <RowStyle CssClass="row" VerticalAlign="Top" Font-Size="Larger" Font-Bold="true"></RowStyle>
                        <Columns>
                            <asp:templatefield>
		                            <ItemStyle width="20px" />
		                            <ItemTemplate>
		                                <asp:CheckBox id="cbInclude" runat="server"></asp:CheckBox>
                                        </ItemTemplate>
		                        </asp:templatefield>
                            <asp:boundfield datafield="Project" headertext="Project" />
                            <asp:boundfield datafield="Title" headertext="Title" />
                            <asp:boundfield datafield="Saved" headertext="Saved" />
                        </Columns>
                    </CC:GridView>
                </ContentTemplate>
                </asp:updatepanel>
        </div>
    </div>
    <asp:button id="btnAddTakeOffProducts" runat="server" text="Add Products from Existing Takeoff(s)" cssclass="btnred" />
    <asp:button id="btnCloseWin" runat="server" text="Close window" cssclass="btnred" />
</div>

<CC:DivWindow ID="frmNonSpecial" runat="server" TargetControlID="divSubstitute" CloseTriggerId="btnCancelSub" ShowVeil="false" VeilCloses="true" />
<div id="divSubstitute" runat="server" class="window" style="width: 800px; left: 10px; top: 152px; background: #e1e1e1">
    <div class="pckggraywrpr" style="margin-bottom: 0px;">
        <div class="pckghdgred" style="height: 15px;">Non-special pricing from <%= dbVendor.CompanyName %></div><%--<div style="background-color: #fff;">--%><table cellpadding="0" cellspacing="10" border="0" style="table-layout: fixed; width: 800px; height: 600px;">
            <tr valign="top">
                <td style="width: 35%;">
                    <div style="height: 600px;">
                        <CC:Search ID="ctlSearch" runat="server" KeywordsTextboxId="txtKeywords" PageSize="10000" />
                    </div>
                </td>
                <td>
                    <asp:updatepanel id="upResults" runat="server" updatemode="Conditional" childrenastriggers="false">
                            <ContentTemplate>
                            <input type="hidden" id="hdnRequestId" runat="server" />
                            <div class="pckghdgblue autoHeight">
                                <div style="float:left;margin-right:100px;">
                                    <span class="center" >Search for products:</span> </div><div style="float:left;color:#fff;">
                                    <asp:RadioButtonList ID="rblPricedOnly" runat="server" AutoPostBack="true" Visible="false">
                                        <asp:ListItem Value="true" Selected="True">Show Priced Products Only</asp:ListItem><asp:ListItem Value="false">Show All LLC Products</asp:ListItem></asp:RadioButtonList></div><div class="clear">&nbsp;</div><div style="color:#fff;">
                                    <asp:Literal ID="ltlBreadcrumbs" runat="server"></asp:Literal><br /><span><asp:TextBox ID="txtKeywords" runat="server" Columns="25"></asp:TextBox>&nbsp;<asp:Button ID="btnSearchNonSpecial" runat="server" Text="Search" CssClass="btnred" /></span>
                                </div>
                                <div class="clear">&nbsp;</div></div><div style="height:475px;overflow-y:scroll;overflow-x: hidden;">
                            <table class="tbltodata" cellpadding="2" cellspacing="0" border="0" style="width:100%;">
                                <tr >
                                    <th>My SKU</th><th style="text-align:center">Name</th><th>Price</th><th>Quantity</th><th>&nbsp;</th></tr><asp:Repeater ID="rptNonSpecialProducts" runat="server">
                                <ItemTemplate>
                                    <tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "", "alt") %>'>
                                    <asp:HiddenField id="hdnNonSpecialProductsVendorSku" runat="server"></asp:HiddenField>
                                        <td><span ID="spanVendorSku" runat="server"><%#DataBinder.Eval(Container.DataItem, "VendorSku") %></span></td>
                                        
                                        <td style="text-align:center"><span id="spanName" runat="server"><%# DataBinder.Eval(Container.DataItem, "ProductName")%></span></td>
                                        <td><span id="spanPrice" runat="server"><%# FormatCurrency(DataBinder.Eval(Container.DataItem, "VendorPrice"))%></span></td>
                                        <td><asp:textbox runat="server" id="txtQtyNonSpecial" width="35" /></td>
                                        <td><asp:Button ID="btnAdd" CausesValidation="false" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "VendorPrice")%>' CommandName='<%#DataBinder.Eval(Container.DataItem, "ProductID") %>' runat="server" CssClass="btnred" Text="Add" /></td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            </table>
                            <CC:Navigator ID="ctlNavigator" runat="server" MaxPerPage="10" PagerSize="5" AddHiddenField="true" />
                            </div>
                                </ContentTemplate>
                            </asp:updatepanel>
                    <p style="text-align: center; margin: 10px">
                        <asp:button id="btnCancelSub" runat="server" text="Close Window" cssclass="btnred" />
                    </p>
                </td>
            </tr>
        </table>
    </div>
</div>
<%--</div>--%>

<p></p>
<asp:updatepanel id="upFilterBar" runat="server" childrenastriggers="false" updatemode="Conditional"><ContentTemplate><asp:Panel runat="server" DefaultButton="btnSearch" visible="false"><table border="0" cellpadding="0" cellspacing="0" class="fltrbar"><tr><td class="hdg">Select Products From:</td><td><span class="smallest">Projects:</span><br /> <CC:SearchList ID="slProjects" runat="server" AllowNew="false" AutoPostback="true" CssClass="searchlist" Table="Project" TextField="ProjectName" ValueField="ProjectID" ViewAllLength="20" width="150px" /></td>
                <td><span class="smallest">Orders:</span><br /> <CC:SearchList ID="slOrders" runat="server" AllowNew="false" AutoPostback="false" CssClass="searchlist" Table="Order" TextField="Title" ValueField="OrderId" ViewAllLength="20" width="150px"></CC:SearchList>
                </td>
                <td><span class="smallest">Takeoffs:</span><br /> <CC:SearchList ID="slTakeoffs" runat="server" AllowNew="false" AutoPostBack="false" CssClass="searchlist" Table="Takeoff" TextField="Title" ValueField="TakeoffId" ViewAllLength="20" width="150px"></CC:SearchList>
                </td>
                <td><asp:ImageButton ID="btnAdd" runat="server" AlternateText="Add Related Products to Take-off" ImageUrl="/images/global/btn-fltrbar-add.gif" style="width:28px;border:none;height:26px;" /></td>
                <td><asp:ImageButton ID="btnUpload" runat="server" AlternateText="Upload Products to Take-off" ImageUrl="/images/global/btn-fltrbar-upload.gif" onclientclick="return OpenUploadProduct();" style="width:28px;border:none;height:26px;" /></td>
                <td><asp:ImageButton ID="btnSpecial" runat="server" alternatetext="Add Special Order Product to Take-off" ImageUrl="/images/global/btn-fltrbar-addspecial.gif" onclientclick="return OpenSpecialForm();" style="width:28px;border:none;height:26px;" /></td>
                <td><img alt="" src="/images/spacer.gif" style="width: 30px; height: 1px" /><br /></td>
                <td><asp:TextBox ID="txtKeyword" runat="server" onfocus="this.value='',this.style.color='#000'" style="width:154px;color:#666;" Text="Keyword Search"></asp:TextBox></td><td><asp:ImageButton ID="btnSearch" runat="server" AlternateText="Search" ImageUrl="/images/global/btn-fltrbar-search.gif" style="width:28px;border:none;height:26px;" /></td>    
            </tr>
        </table>
       </asp:Panel>
    </ContentTemplate>
    <Triggers><asp:PostBackTrigger ControlID="btnAdd" /><asp:AsyncPostbackTrigger ControlID="slProjects" /><asp:AsyncPostBackTrigger ControlID="btnUpload" /><asp:PostBackTrigger ControlID="btnSearch" /><asp:AsyncPostBackTrigger ControlID="hdnSearch" /></Triggers>
</asp:updatepanel>
<asp:hiddenfield id="hdnSearch" runat="server" value=" "></asp:hiddenfield>
<asp:literal id="ltrErrorMsg" runat="server"></asp:literal><table border="0" cellpadding="0" cellspacing="0" class="tblnotify" style="border:0 solid #585858; margin: 10px auto; width: 600px;">
    <asp:repeater id="rptProductsNotIncluded" runat="server">
  
            <HeaderTemplate>
           <tr class="row" valign="top">
	<th>SKU</th><th>Product Name</th></tr></HeaderTemplate><ItemTemplate>
            <tr valign="top" class='<%#IIf(Container.ItemIndex Mod 2 = 1, "row", "alt") %>'>
                <td><%#DataBinder.Eval(Container.DataItem, "Sku")%></td>                                
                <td><%#DataBinder.Eval(Container.DataItem, "Product")%></td>
            </tr>
            </ItemTemplate>
        </asp:repeater>
</table>







<table border="0" cellpadding="0" cellspacing="0" class="tblnwto">
    <tr valign="top">
        <td id="Td1" class="leftcol" runat="server" visible="false">
            <asp:updatepanel id="upFacets" runat="server" childrenastriggers="False" updatemode="Conditional"><ContentTemplate><div class="leftcolwrpr"><div class="pckghdgred">1. Select Category</div><div class="redbox"><ul><li><ul><li><asp:Literal ID="ltlAllProducts" runat="server"></asp:Literal></li><li><asp:Literal ID="ltlLLCOnly" runat="server"></asp:Literal></li></ul></li><asp:Repeater ID="rptProductType" runat="server"><HeaderTemplate><li>By Product Type <ul></ul></li></HeaderTemplate>
                            <ItemTemplate><li><asp:Literal ID="ltlLabel" runat="server"></asp:Literal></li></ItemTemplate><FooterTemplate></ul></li></FooterTemplate>
                        </asp:Repeater>
                        <asp:Repeater ID="rptManufacturer" runat="server"><HeaderTemplate><li>By Manufacturer <ul></ul></li></HeaderTemplate>
                            <ItemTemplate><li><asp:Literal ID="Literal1" runat="server"></asp:Literal></li></ItemTemplate><FooterTemplate></ul></li></FooterTemplate>
                        </asp:Repeater>
                        <asp:Repeater ID="rptUnitOfMeasure" runat="server"><HeaderTemplate><li>By Unit of Measure <ul></ul></li></HeaderTemplate>
                            <ItemTemplate><li><asp:Literal ID="Literal2" runat="server"></asp:Literal></li></ItemTemplate><FooterTemplate></ul></li></FooterTemplate>
                        </asp:Repeater>
                        </ul>
                    </div>
                    <NineRays:FlyTreeView ID="flyTreeView" runat="server" BackColor="White" BorderColor="Silver" BorderWidth="1px" FadeEffect="True" Height="320px" ImageSet="Classic" Padding="3px" PostBackOnClick="true" Style="display: block"><DefaultStyle Font-Names="Tahoma" Font-Size="11px" ForeColor="Black" ImageUrl="$classic_folder" Padding="1px;3px;3px;1px" RowHeight="16px" /><SelectedStyle BackColor="65, 86, 122" BorderColor="40, 40, 40" BorderStyle="Solid" BorderWidth="1px" ForeColor="White" ImageUrl="$classic_folder_open" Padding="0px;2px;2px;0px" /><HoverStyle Font-Underline="True" /></NineRays:FlyTreeView>
                                        
                    <div class="cblock10" style="display:none;"><asp:LinkButton ID="lnkExpand" runat="server">Expand All</asp:LinkButton><span class="pipe">|</span> <asp:LinkButton ID="lnkCollapse" runat="server">Collapse All</asp:LinkButton></div></div></ContentTemplate><Triggers><asp:PostBackTrigger ControlID="flyTreeView" /></Triggers>            
        </asp:updatepanel>

            <!-- 'RAY
                        <asp:PostBackTrigger ControlID="tvSupplyPhases" />
        //-->
        </td>
        <td class="spacercol">&#160;</td><td class="maincol">
            <div class="maincolwrpr">
                <div class="pckghdgred autoHeight">
                    1. Add Desired Products to order <asp:button id="btnAddNonSpecial" runat="server" causesvalidation="false" text="Add Std. Catalog Items" cssclass="btnred" style="width: 183px;" onclientclick="return OpenNonSpecial();"></asp:button>
                    <asp:button id="btnAddPrevTakeOffs" runat="server" causesvalidation="false" text="Add From Existing Takeoffs" cssclass="btnred" onclientclick="return OpenPrevTakeOffs();"></asp:button>
                </div>
                <asp:panel id="pnlPreferredVendor" runat="server" visible="false"><table border="0" cellpadding="0" cellspacing="0" class="fltrbar"><tr><td class="hdg">Select a Preferred Vendor:</td><td style="padding-top: 5px;"><div style="width: 205px; margin-left: 10px; margin-bottom: 0px;"><asp:UpdatePanel ID="upPreferred2" runat="server" ChildrenAsTriggers="true" UpdateMode="Conditional"><ContentTemplate><CC:SearchList ID="slPreferredVendor2" runat="server" AllowNew="false" AutoPostback="true" CssClass="searchlist" Table="Vendor" TextField="CompanyName" ValueField="VendorID" ViewAllLength="10" Width="200px" /></ContentTemplate>
                </asp:UpdatePanel>
                        </div>
                        </td>
                        </tr>
                        </table>
                    </asp:panel>

                <asp:updatepanel id="upProducts" runat="server" childrenastriggers="true" updatemode="Conditional" class="desired-product-table">
                    <ContentTemplate>
                    <table border="0" cellpadding="0" cellspacing="0" class="tbltodata">
                        <tr><th></th><th>Sku Number</th><th>Name</th><th></th><th id="tdPriceHeader" runat="server">Price</th><th>Qty</th><th>&#160;</th></tr><tr><td colspan="2">&#160;</td><td class="right" colspan="3"><asp:Button ID="btnExport" runat="server" text="Export Committed Buy Pricing" cssclass="btnred" /></td>
                                <td><asp:Button ID="btnAddAll" runat="server" cssclass="btnred" text="Add All" /></td>
                            </tr>
                            <asp:Repeater ID="rptProducts" runat="server"><ItemTemplate>
                            <asp:HiddenField id="hdnVendorSku" runat="server"></asp:HiddenField>
                                <tr class='<%#IIf(Container.ItemIndex Mod 2 = 1, "", "alt") %>'>
                                    
                                    <td><asp:Image ID="imgPlus" runat="server" alt="" style="cursor: pointer" />

                                    <asp:Panel ID="pnlOldPrice" runat="server" Style="display: none">
                                    <h2>Price History</h2><asp:GridView ID="gvOldPrice" runat="server" AutoGenerateColumns="false">
                                    <Columns>
                                    <asp:BoundField DataField="OldPrice" HeaderText="Old Price" ItemStyle-Width="150px" />
                                    <asp:BoundField DataField="NewPrice" HeaderText="New Price" ItemStyle-Width="150px" />
                                    <asp:BoundField DataField="UpdatedOn" HeaderText="Updated On" SortExpression="UpdatedOn"><HeaderStyle Wrap="False" /><ItemStyle Wrap="False" /></asp:BoundField>
                                    </Columns>
                                    </asp:GridView>
                                    </asp:Panel>


                                    </td>
                                    
                                    <td><%#DataBinder.Eval(Container.DataItem, "Sku")%></td>                                
                                        <td>
                                            
                                            <%#DataBinder.Eval(Container.DataItem, "ProductName")%>

			    <div class="adminToolTipWrpr">
                    <div class="adminToolTipShadow">
                        <%#DataBinder.Eval(Container.DataItem, "Comments")%>
                      </div>
                    <div class="adminToolTopShadowBottom">&nbsp;</div>
                </div>
                                        </td><td id="tdOldPrice" runat="server"></td>
                                        <td id="tdPrice" runat="server"></td>
                                        <td><asp:TextBox ID="txtQty" runat="server" class="inptqty" columns="6" maxlength="6"></asp:TextBox></td><td><asp:Button ID="btnAddProduct" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Price")%>' CommandName='<%#DataBinder.Eval(Container.DataItem, "ProductID") %>' cssclass="btnred" Text="Add"></asp:Button>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr class="bluerow"><td colspan="2"><CC:Navigator visible="false" ID="ctlNavigate" runat="server" AddHiddenField="true" MaxPerPage="32000" Pagersize="5" /></td>
                                <td class="right" colspan="3"><asp:Button ID="btnExport2" runat="server" text="Export Committed Buy Pricing" cssclass="btnred" /></td>
                                    <td colspan="2"><asp:Button ID="btnAddAll2" runat="server" cssclass="btnred" text="Add All" /></td>
                            </tr>
                        <tr>
                            <td>


                            </td>
                        </tr>
                        </table>
                    </ContentTemplate>
                    <Triggers></Triggers>
                </asp:updatepanel>
                <!-- 'RAY
                        <asp:AsyncPostbackTrigger ControlId="tvSupplyPhases" EventName="SelectedIndexChanged"></asp:AsyncPostbackTrigger>                    
                        //-->
            </div>
        </td>
        <td class="spacercol">&#160;</td><td class="rightcol" style="width: 450px">
            <div class="rightcolwrpr">
                <div class="pckghdgblue">
                    2. Review Material List &amp; Place Order<div class="right">
                        <asp:button id="btnDeleteAll" runat="server" text="Clear Material List" cssclass="btnred" />
                    </div>
                    &nbsp; </div><div class="bold right bgltblue" style="padding: 10px">
                    <br />
                    <span>
                        <asp:updatepanel id="upTakeoff" runat="server" updatemode="Conditional">
                        <ContentTemplate>                      
                     <asp:label ID= "ltlProductsNotIncluded"  cssclass="larger center"  style="display: block;" runat="server" Text="Products Currently Not Priced by Vendor" visible="false"></asp:label><CC:GridView ID="gvPendingPricesTakeOff" runat="server" AutoGenerateColumns="False" BorderWidth="0" CellPadding="0" CellSpacing="0" CssClass="tbltodata" GridLines="None"><AlternatingRowStyle CssClass="alt" />
                           <Columns>
                               <asp:TemplateField  HeaderText="Name" HeaderStyle-CssClass="header-center">
                                   <ItemTemplate>
                                       <asp:Image ID="ImgRequestStatus" runat="server"  style="float :left;padding :1px"></asp:Image><%#DataBinder.Eval(Container.DataItem, "Product")%>
							        </ItemTemplate>
						        </asp:TemplateField>
						        <asp:TemplateField HeaderText="Qty">
                                    <ItemTemplate>
                                        <asp:TextBox ID="txtNewQty" runat="server" class="inptqty" columns="6" maxlength="6" text='<%#DataBinder.Eval(Container.DataItem, "Quantity")%>'></asp:TextBox></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Price">
                                    <ItemTemplate>
                                        <asp:Literal ID="ltlPrice" runat="server"></asp:Literal></ItemTemplate></asp:TemplateField><asp:TemplateField></asp:TemplateField>
						        <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnRequestPricing" ImageUrl="/images/global/icon-message.gif" runat="server" style="height:23px;width:23px;"></asp:ImageButton>
						            </ItemTemplate>
						        </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:ImageButton ID="btnDelete" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "VendorPrice")%>' CommandName='<%#DataBinder.Eval(Container.DataItem, "TwoPriceBuilderTakeOffProductPendingID") %>'  ImageUrl="/images/global/icon-remove.gif" style="height:13px;width:13px;"></asp:ImageButton>
						            </ItemTemplate>
					            </asp:TemplateField>
						    </Columns>
						</CC:GridView>
                      <asp:button id="btnRequestPricingAll" runat="server" text="Request Pricing for Unpriced Items" cssclass="btnred" style="margin:3px" onClientClick="DisablePricingRequestButton();" />
                      <asp:hiddenfield id="hdnRequestPricingStatus" runat="server" value="False"></asp:hiddenfield>
                      <CC:GridView ID="gvTakeoff" runat="server" AutoGenerateColumns="False" BorderWidth="0" CellPadding="0" CellSpacing="0" CssClass="tbltodata" GridLines="None"><AlternatingRowStyle CssClass="alt" /><Columns><asp:TemplateField HeaderText="Name"><ItemTemplate><%#DataBinder.Eval(Container.DataItem, "Product")%>
							</ItemTemplate>
								</asp:TemplateField>
								<asp:TemplateField HeaderText="Qty"><ItemTemplate><asp:TextBox ID="txtNewQty" runat="server" class="inptqty" columns="6" maxlength="6" text='<%#DataBinder.Eval(Container.DataItem, "Quantity")%>'></asp:TextBox></ItemTemplate></asp:TemplateField><asp:TemplateField HeaderText="Price"><ItemTemplate><asp:Literal ID="ltlPrice" runat="server"></asp:Literal></ItemTemplate></asp:TemplateField><asp:TemplateField></asp:TemplateField>
								<asp:TemplateField><ItemTemplate><asp:ImageButton ID="btnDelete" runat="server" CommandArgument='<%#DataBinder.Eval(Container.DataItem, "Price")%>' CommandName='<%#DataBinder.Eval(Container.DataItem, "TwoPriceOrderProductId") %>' ImageUrl="/images/global/icon-remove.gif" style="height:13px;width:13px;"></asp:ImageButton>
									</ItemTemplate>
								</asp:TemplateField>
							</Columns>
						</CC:GridView>
                        <table border="0" cellpadding="0" cellspacing="0" class="tbltodata"><tr class="bluerow"><td class="center bold" style="padding:10px 0px 2px 0px;">Total Products</td><td class="center bold" style="padding:10px 0px 2px 0px;">Total Price</td></tr><tr class="bluerow"><td class="center" style="padding:2px 0px 10px 0px;"><asp:Literal ID="ltlTotalProducts" runat="server"></asp:Literal></td><td class="center" style="padding:2px 0px 10px 0px;"><asp:Literal ID="ltlTotalPrice" runat="server"></asp:Literal></td></tr></table></ContentTemplate><Triggers><asp:asyncpostbacktrigger ControlId="btnAddSpecial"></asp:asyncpostbacktrigger>
                    </Triggers>
                </asp:updatepanel>
                        <%--<asp:updatepanel runat="server" Id="PnlOrderStart" UpdateMode="Conditional" ChildrenAsTriggers="true">
                            <ContentTemplate>--%>
                        <asp:button id="btnExportcsv" runat="server" cssclass="btngold" text="Export Take-Off" />
                        <asp:button id="btnUpdate" runat="server" cssclass="btngold" text="Update Quantities" />
                        <asp:button id="btnStartSubmitOrder" runat="server" OnClientClick="return OpenReqPopup();" cssclass="btnred" text="Submit Order" />
                                <%--</ContentTemplate>
                            <Triggers>--%>
                                <%--<asp:AsyncPostBackTrigger ControlID="btnStartSubmitOrder" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnUpdate" EventName="Click" />
                                <asp:AsyncPostBackTrigger ControlID="btnExportcsv" EventName="Click" />--%>
 <%--</Triggers>
                            </asp:updatepanel>--%>
                    </span>
                </div>
            </div>
        </td>
    </tr>




</table>

<div id="divSpecialOrderForm" runat="server" class="window" style="visibility: hidden; border: 1px solid #000; background-color: #fff; width: 350px;">
    <div class="pckghdgred">Add Special Order Product</div><table border="0" cellpadding="2" cellspacing="0" style="background-color: #fff; margin: 5px;">
        <tr valign="top">
            <td>&#160;</td><td class="fieldreq">&#160;</td><td class="field smaller">indicates required field</td></tr><tr valign="top">
            <td class="fieldlbl bold" style="white-space: nowrap;"><span id="labeltxtProductName" runat="server">Product Name:</span></td><td id="bartxtProductName" runat="server" class="fieldreq">&#160;</td><td class="field">
                <asp:textbox id="txtProductName" runat="server" columns="50" maxlength="100" style="width: 200px;"></asp:textbox><asp:requiredfieldvalidator id="rfvtxtProductName" runat="server" controltovalidate="txtProductName" errormessage="Product Name is required" validationgroup="SpecialForm"></asp:requiredfieldvalidator></td></tr><tr valign="top">
            <td class="fieldlbl bold"><span id="labeltxtDescription" runat="server">Description:</span></td><td id="bartxtDescription" runat="server" class="fieldreq">&#160;</td><td class="field">
                <asp:textbox id="txtDescription" runat="server" columns="50" maxlength="2000" rows="3" style="width: 200px;" textmode="Multiline"></asp:textbox><asp:requiredfieldvalidator id="rfvtxtDescription" runat="server" controltovalidate="txtDescription" errormessage="Description is required" validationgroup="SpecialForm"></asp:requiredfieldvalidator></td></tr><tr valign="top">
            <td class="fieldlbl bold"><span id="labeltxtSpecialQuantity" runat="server">Quantity:</span></td><td id="bartxtSpecialQuantity" runat="server" class="fieldreq">&#160;</td><td class="field">
                <asp:textbox id="txtSpecialQuantity" runat="server" columns="10" maxlength="10" style="width: 50px;"></asp:textbox><asp:requiredfieldvalidator id="rfvtxtSpecialQuantity" runat="server" controltovalidate="txtSpecialQuantity" errormessage="Quantity is required" validationgroup="SpecialForm"></asp:requiredfieldvalidator><CC:IntegerValidator ID="ivtxtSpecialQuantity" runat="server" ControlToValidate="txtSpecialQuantity" ErrorMessage="Quantity is invalid" ValidationGroup="SpecialForm"></CC:IntegerValidator></td></tr><tr valign="top">
            <td class="fieldlbl bold" style="white-space: nowrap;"><span id="labelacUnit" runat="server">Unit of Measure:</span></td><td id="baracUnit" runat="server" class="fieldreq">&#160;</td><td class="field">
                <CC:SearchList ID="acUnit" runat="server" AllowNew="false" CssClass="searchlist" Table="UnitOfMeasure" TextField="UnitOfMeasure" ValueField="UnitOfMeasureId" ViewAllLength="15"></CC:SearchList>
                <asp:requiredfieldvalidator id="rfvacUnit" runat="server" controltovalidate="acUnit" errormessage="Unit of Measure is required" validationgroup="SpecialForm"></asp:requiredfieldvalidator></td></tr></table><p style="text-align: center; padding: 10px;">
        <asp:button id="btnAddSpecial" runat="server" cssclass="btnred" text="Add Product" validationgroup="SpecialForm" />
        &#160; <asp:button id="btnCancelSpecial" runat="server" causesvalidation="false" cssclass="btnred" text="Cancel" />
    </p>
</div>
<CC:DivWindow ID="ctrlSpecial" runat="server" CloseTriggerId="btnCancelSpecial" ShowVeil="true" TargetControlID="divSpecialOrderForm" VeilCloses="false" />
<div id="divSaveForm" runat="server" class="window" style="border: 1px solid #000; background-color: #fff; width: 300px;">
    <asp:updatepanel id="upSaveForm" runat="server" childrenastriggers="false" updatemode="Conditional"><ContentTemplate><div id="divHead" runat="server" class="pckghdgred">Save Takeoff</div><div style="margin:10px;"><table border="0" cellpadding="2" cellspacing="0"><tr><td colspan="3"><asp:Label ID="lblSavedMsg" runat="server"></asp:Label></td></tr><tr><td><b>Title: </b></td><td id="bartxtTakeoffTitle" runat="server" class="fieldreq">&#160;</td><td><asp:TextBox ID="txtTakeoffTitle" runat="server" columns="50" maxlength="100" style="width:200px;"></asp:TextBox><br /></td></tr><tr><td><b>Project: </b></td><td>&#160;</td><td><CC:SearchList ID="slTakeoffProject" runat="server" AllowNew="false" CssClass="searchlist" Table="Project" TextField="ProjectName" ValueField="ProjectID" ViewAllLength="10" /><br /><a class="smaller" href="/projects/edit.aspx" target="_blank">Click Here to Start a New Project</a> </td></tr><tr><td colspan="3"><p style="text-align:center;padding:10px;"><asp:Button ID="btnSaveSubmit" runat="server" cssclass="btnred" onclientclick="UpdateTitle()" text="Save" ValidationGroup="SaveTakeoff" /><asp:Button ID="btnCancel" runat="server" cssclass="btnred" onclientclick="return CloseSaveForm();" text="Close" /><asp:HiddenField ID="hdnIsCopy" runat="server"></asp:HiddenField>
                            </p>
                        </td>
                    </tr>
                </table>
            </div>
            <CC:RequiredFieldValidatorFront ID="rfvtxtTakeoffTitle" runat="server" ControlToValidate="txtTakeoffTitle" ErrorMessage="Field 'Takeoff Title' is empty" ValidationGroup="SaveTakeoff"></CC:RequiredFieldValidatorFront></ContentTemplate><Triggers><asp:AsyncPostbackTrigger ControlId="btnSaveSubmit" EventName="click"></asp:AsyncPostbackTrigger>
        </Triggers>
    </asp:updatepanel>
</div>
<CC:DivWindow ID="ctrlSaveForm" runat="server" CloseTriggerId="btnCancel" ShowVeil="true" TargetControlID="divSaveForm" VeilCloses="false" />
<div id="divProductQty" runat="server" class="window" style="border: 1px solid #000; background-color: #fff; width: 350px; visibility: hidden;">
    <div class="pckghdgred">Upload Products </div><div style="margin-left: 10px; margin-right: 10px">
        <br />
        <p>Use this form upload products and quantities from a csv file. The product should be identified with its CBUSA SKU followed by the required quantity.</p><p><a href="ProductTemplate.csv" target="_blank">Get Product File Template</a> <span class="smallest">(Uploaded file must match the template exactly including the header line)</span></p><center><CC:FileUpload ID="fulDocument" runat="server" /><br /><br /><CC:OneClickButton ID="btnImport" runat="server" cssclass="btnred" Text="Import CSV" /><asp:Button ID="btnCancelImport" runat="server" CausesValidation="false" cssclass="btnred" Text="Cancel" /><br /></center>
    </div>
</div>
<CC:DivWindow ID="ctrlProductUpload" runat="server" CloseTriggerId="btnCancelImport" ShowVeil="true" TargetControlID="divProductQty" VeilCloses="false" />

</asp:panel>
<asp:panel id="pnSubmitOrder" runat="server" visible="false">

    <script type="text/javascript">
        function ProjectFormResult(res, ctxt) {

            if (!res.errors) $get('<%=frmProject.ClientID %>').control.Close();
        }
        function UpdateTaxRate(zip) {
            Sys.Net.WebServiceProxy.invoke('/builder/twoprice/edit.aspx', 'GetTaxRate', false, { 'Zip': zip }, UpdateTaxRateCB, UpdateTaxRateCB);
        }
        function UpdateTaxRateCB(res, ctxt) {
            var span = $get('<%=spanTaxRate.ClientID %>');
            if (res.get_exceptionType) {
                span.innerHTML = 'Error -- please check Zip Code';
            } else {
                span.innerHTML = res + '%';
            }
        }

        function UpdateName() {
            $get('<%=txtOrderTitle.ClientID %>').value = $get('<%=acProject.ClientID %>').value;
            var ProjectID = $get('<%=acProject.ClientID %>').control.get_value();
            if (ProjectID !== "") {
                UpdateOrderTaxRate($get('<%=acProject.ClientID %>').control.get_value());
            }
        }

        function UpdateOrderTaxRate(ProjectID) {
            Sys.Net.WebServiceProxy.invoke('/builder/twoprice/edit.aspx', 'GetTaxRateFromProjectZip', false, { 'ProjectID': ProjectID }, UpdateOrderTaxRateCB, UpdateOrderTaxRateCB);
        }

        function UpdateOrderTaxRateCB(res, ctxt) {

            var span = $get('<%=txtTaxRate.ClientID %>');

            if (res.get_exceptionType) {
                $get('<%=txtTaxRate.ClientID %>').value = 'Error -- please check Zip Code';
            } else {
                $get('<%=txtTaxRate.ClientID %>').value = res;
            }
        }
    </script>
<div class="pckggraywrpr">
    <CC:PopupForm ID="frmProject" runat="server" cssclass="pform" style="width:425px;" OpenMode="MoveToCenter" ShowVeil="true" VeilCloses="false" Animate="true" OpenTriggerId="btnAddProject" CloseTriggerId="btnCancelAddProj">
    <FormTemplate>
        <div class="pckggraywrpr" style="margin-bottom:0px;">
            <div class="pckghdgred" style="height:15px;">Add Project</div><table cellpadding="5" cellspacing="0" border="0">
                <tr valign="top">
                    <td>&nbsp;</td><td class="fieldreq">&nbsp;</td><td class="field smaller"> indicates required field</td></tr><tr valign="top">
                    <td class="fieldlbl"><span id="labeltxtProjectName" runat="server">Project Name:</span></td><td class="fieldreq" id="bartxtProjectName" runat="server">&nbsp;</td><td class="field"><asp:TextBox id="txtProjectName" runat="server" columns="150" maxlength="150" style="width:319px;"></asp:TextBox></td></tr><tr valign="top">
                    <td class="fieldlbl"><span id="labeltxtProjectLotNo" runat="server">Lot #:</span></td><td>&nbsp;</td><td class="field"><asp:TextBox id="txtProjectLotNo" runat="server" columns="20" maxlength="20" style="width:319px;"></asp:TextBox></td></tr><tr valign="top">
                    <td class="fieldlbl"><span id="labeltxtProjectSubdivision" runat="server">Subdivision:</span></td><td>&nbsp;</td><td class="field"><asp:TextBox id="txtProjectSubdivision" runat="server" maxlength="50" columns="50" style="width:319px;"></asp:TextBox></td></tr><tr valign="top">
                    <td class="fieldlbl"><span id="labeltxtProjectAddress1" runat="server">Address:</span></td><td class="fieldreq" id="bartxtProjectAddress1" runat="server">&nbsp;</td><td class="field"><asp:TextBox id="txtProjectAddress1" runat="server" columns="50" maxlength="50" style="width:319px;"></asp:TextBox></td></tr><tr valign="top">
                    <td class="fieldlbl">Address 2:</td><td>&nbsp;</td><td class="field"><asp:TextBox id="txtProjectAddress2" runat="server" Columns="50" maxlength="50" style="width:319px;"></asp:TextBox></td></tr><tr valign="top">
                    <td class="fieldlbl"><span id="labeltxtProjectCity" runat="server">City:</span></td><td class="fieldreq" id="bartxtProjectCity" runat="server">&nbsp;</td><td class="field"><asp:TextBox id="txtProjectCity" runat="server" columns="50" maxlength="50" style="width:319px;"></asp:TextBox></td></tr><tr valign="top">
                    <td class="fieldlbl"><span id="labeldrpProjectState" runat="server">State:</span></td><td class="fieldreq" id="bardrpProjectState" runat="server">&nbsp;</td><td class="field"><asp:DropDownList id="drpProjectState" runat="server"></asp:DropDownList></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl"><span id="labeltxtProjectZip" runat="server">Zip:</span></td><td class="fieldreq" id="bartxtProjectZip" runat="server">&nbsp;</td><td class="field"><asp:TextBox id="txtProjectZip" runat="server" maxlength="15" columns="15" style="width:50px;" onchange="UpdateTaxRate(this.value);"></asp:TextBox></td></tr><tr valign="top">
                    <td class="fieldlbl">Tax Rate:</td><td>&nbsp;</td><td class="field"><span id="spanTaxRate" runat="server">Enter your Zip Code above to load its tax rate.</span></td></tr><tr valign="top">
                    <td class="fieldlbl">Portfolio:</td><td>&nbsp;</td><td class="field"><asp:DropDownList id="drpProjectPortfolio" runat="server"></asp:DropDownList></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl"><span id="labeldrpProjectStatus" runat="server">Status:</span></td><td class="fieldreq" id="bardrpProjectStatus" runat="server">&nbsp;</td><td class="field"><asp:DropDownList id="drpProjectStatus" runat="server"></asp:DropDownList></td>
                </tr>
                <tr valign="top">
                    <td class="fieldlbl">Start Date:</td><td>&nbsp;</td><td class="field"><CC:DatePicker ID="dpProjectStartDate" runat="server"></CC:DatePicker></td></tr><tr valign="top">
                    <td class="fieldlbl"><span id="labelrblProjectArchive" runat="server">Archive:</span></td><td class="fieldreq" id="barrblProjectArchive" runat="server">&nbsp;</td><td class="field">
                        <asp:RadioButtonList id="rblProjectArchive" runat="server">
                            <asp:ListItem value="true">Yes</asp:ListItem><asp:ListItem value="false" selected="true">No</asp:ListItem></asp:RadioButtonList></td></tr></table><asp:Button id="btnSaveProject" runat="server" cssclass="btnred" text="Add Project" ValidationGroup="ProjectForm" />
            <asp:Button id="btnCancelAddProj" runat="server" cssclass="btnred" text="Cancel"></asp:Button>
        </div>
        <CC:RequiredFieldValidatorFront ID="rfvProjectName" runat="server" ControlToValidate="txtProjectName" ErrorMessage="Field 'Project Name' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront><CC:RequiredFieldValidatorFront ID="rfvAddress1" runat="server" ControlToValidate="txtProjectAddress1" ErrorMessage="Field 'Address 1' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront><CC:RequiredFieldValidatorFront ID="rfvCity" runat="server" ControlToValidate="txtProjectCity" ErrorMessage="Field 'City' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront><CC:RequiredFieldValidatorFront ID="rfvState" runat="server" ControlToValidate="drpProjectState" ErrorMessage="Field 'State' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront><CC:RequiredFieldValidatorFront ID="rfvZip" runat="server" ControlToValidate="txtProjectZip" ErrorMessage="Field 'Zip' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront><CC:RequiredFieldValidatorFront ID="rfvStatus" runat="server" ControlToValidate="drpProjectStatus" ErrorMessage="Field 'Status' is blank" ValidationGroup="ProjectForm"></CC:RequiredFieldValidatorFront><CC:DateValidatorFront ID="dvStartDate" runat="server" ControlToValidate="dpProjectStartDate" ErrorMessage="Field 'Start Date' is invalid" ValidationGroup="ProjectForm"></CC:DateValidatorFront></FormTemplate><Buttons>
        <CC:PopupFormButton ControlId="btnSaveProject" ButtonType="Callback" ClientCallback="ProjectFormResult" />
        <CC:PopupFormButton ControlId="btnCancelAddProj" ButtonType="ScriptOnly" />
    </Buttons>
</CC:PopupForm>
    <div class="pckghdgred">Submit Order</div><table border="0" cellpadding="2" cellspacing="2">
<tr valign="top">
   <td>&#160;</td><td class="fieldreq">&#160;</td><td class="bold smaller">indicates required field</td></tr><tr style="position:relative;z-index:2;" valign="top">
   <td class="fieldlbl"><span id="labelacProject" runat="server" style="position:relative;z-index:9;">Project:</span></td><td id="baracProject" runat="server" class="fieldreq">&#160;</td><td class="field">
    <%--  <asp:dropdownlist ID="drpProjects" runat="server" />--%>
       <CC:SearchList id="acProject" runat="server" OnClientTextChanged="UpdateName();"  OnClientValueUpdated="UpdateName();" AllowNew="false" Table="Project" TextField="ProjectName" ValueField="ProjectId"  CssClass="searchlist" style="width:150px;" ViewAllLength="15"></CC:SearchList>
       <asp:Button id="btnAddProject" runat="server" cssclass="btnred" text="Add Project" /> 
   </td>
</tr>
<tr valign="top"><td class="fieldlbl"><span id="labeltxtOrderTitle" runat="server">Order Title:</span></td><td id="bartxtOrderTitle" runat="server" class="fieldreq">&#160;</td><td class="field"><asp:TextBox ID="txtOrderTitle" runat="server" columns="50" maxlength="100"></asp:TextBox></td></tr><tr valign="top"><td class="fieldlbl"><span id="labeltxtPONumber" runat="server">PO Number:</span></td><td id="bartxtPONumber" runat="server" class="fieldreq">&#160;</td><td class="field"><asp:TextBox ID="txtPONumber" runat="server" columns="20" maxlength="20"></asp:TextBox></td></tr><tr valign="top"><td class="fieldlbl"><span id="labeltxtOrdererFirstName" runat="server">Order-Placer First Name:</span></td><td id="bartxtOrdererFirstName" runat="server" class="fieldreq">&#160;</td><td class="field"><asp:TextBox ID="txtOrdererFirstName" runat="server" columns="50" maxlength="50"></asp:TextBox></td></tr><tr valign="top"><td class="fieldlbl"><span id="labeltxtOrdererLastName" runat="server">Order-Placer Last Name:</span></td><td id="bartxtOrdererLastName" runat="server" class="fieldreq">&#160;</td><td class="field"><asp:TextBox ID="txtOrdererLastName" runat="server" columns="50" maxlength="50"></asp:TextBox></td></tr><tr valign="top"><td class="fieldlbl"><span id="labeltxtOrdererEmail" runat="server">Order-Placer Email:</span></td><td id="bartxtOrdererEmail" runat="server" class="fieldreq">&#160;</td><td class="field"><asp:TextBox ID="txtOrdererEmail" runat="server" columns="50" maxlength="100"></asp:TextBox></td></tr><tr valign="top"><td class="fieldlbl"><span id="labeltxtOrdererPhone" runat="server">Order-Placer Phone:</span></td><td id="bartxtOrdererPhone" runat="server" class="fieldreq">&#160;</td><td class="field"><asp:TextBox ID="txtOrdererPhone" runat="server" columns="50" maxlength="50"></asp:TextBox></td></tr><tr valign="top">
   
        <td class="fieldlbl"><span id="labeltxtTaxRate" runat="server">Tax Rate (In %):</span></td><td class="fieldreq" id="bartxtTaxRate" runat="server">&nbsp;</td><td class="field"><asp:TextBox id="txtTaxRate" runat="server" columns="50" maxlength="50"></asp:TextBox></td></tr><tr valign="top"><td class="fieldlbl"><span id="labeltxtSuperFirstName" runat="server">Site Super. First Name:</span></td><td>&#160;</td><td class="field"><asp:TextBox ID="txtSuperFirstName" runat="server" columns="50" maxlength="50"></asp:TextBox></td></tr><tr valign="top"><td class="fieldlbl"><span id="labeltxtSuperLastName" runat="server">Site Super. Last Name:</span></td><td>&#160;</td><td class="field"><asp:TextBox ID="txtSuperLastName" runat="server" columns="50" maxlength="50"></asp:TextBox></td></tr><tr valign="top"><td class="fieldlbl"><span id="labeltxtSuperEmail" runat="server">Site Super. Email:</span></td><td>&#160;</td><td class="field"><asp:TextBox ID="txtSuperEmail" runat="server" columns="50" maxlength="100"></asp:TextBox></td></tr><tr valign="top"><td class="fieldlbl"><span id="labeltxtSuperPhone" runat="server">Site Super. Phone:</span></td><td>&#160;</td><td class="field"><asp:TextBox ID="txtSuperPhone" runat="server" columns="50" maxlength="50"></asp:TextBox></td></tr><tr id="trDelivery" valign="top"><td class="fieldlbl"><span id="labeldpRequestedDelivery" runat="server">Requested Delivery:</span></td><td>&#160;</td><td class="field"><CC:DatePicker ID="dpRequestedDelivery" runat="server"></CC:DatePicker></td></tr><tr valign="top"><td class="fieldlbl"><span id="labeltxtDeliveryInstructions" runat="server">Shipping/Delivery Instructions:</span></td><td>&#160;</td><td class="field"><asp:TextBox ID="txtDeliveryInstructions" runat="server" columns="50" rows="3" textmode="MultiLine"></asp:TextBox></td></tr><tr valign="top"><td class="fieldlbl"><span id="labeltxtNotes" runat="server">Notes/Comments:</span></td><td>&#160;</td><td class="field"><asp:TextBox ID="txtNotes" runat="server" columns="50" rows="3" textmode="MultiLine"></asp:TextBox></td></tr><tr valign="top" id="trRbDrops" runat="server">
        <td class="fieldlbl"><span id="labelrbDrops" runat="server">Set up Drops:</span></td><td class="fieldreq" id="barrbDrops" runat="server">&nbsp;</td><td class="field">
            <asp:RadioButton id="rbDropsYes" runat="server" value="true" text="Yes" GroupName="rbDrops" OnClick="UpdateDrops(true);"></asp:RadioButton>
            <asp:RadioButton id="rbDropNo" runat="server" checked="true" value="false" text="No" GroupName="rbDrops" OnClick="UpdateDrops(false);"></asp:RadioButton>
        </td>
    </tr>
    <tr valign="top" id="trDrops" style="display:none;">
        <td colspan="2">&nbsp;</td><td class="white">
            <CC:SubForm id="sfDrops" runat="server" InitialRows="1">
                <Fields>
                    <CC:SubFormField ID="SubFormField1" FieldName="Name" runat="server">
                        <HtmlTemplate>
                            <asp:TextBox id="txtDropName" runat="server" columns="50" maxlength="50" style="width:75px;"></asp:TextBox><div id="divDropNameError" runat="server" class="red smallest bold" style="display:none;">Name is required</div></HtmlTemplate><Inputs>
                            <CC:SubFormInput ServerId="txtDropName" ErrorSpanId="divDropNameError" ValidateFunction="function() {return !isEmptyField(this)}" />
                        </Inputs>
                    </CC:SubFormField>
                    <CC:SubFormField ID="SubFormField2" FieldName="Requested Delivery" runat="server">
                        <HtmlTemplate>
                            <CC:DatePicker ID="dpRequestedDelivery" runat="server"></CC:DatePicker><div id="divRequestedDeliveryError" runat="server" class="red smallest bold" style="display:none;">Requested Delivery is invalid</div></HtmlTemplate><Inputs>
                            <CC:SubFormInput ServerId="dpRequestedDelivery" ErrorSpanId="divRequestedDeliveryError"   ValidateFunction="function() {return isEmptyField(this)}" />
                        </Inputs>
                    </CC:SubFormField>
                </Fields>
            </CC:SubForm>
        </td>
    </tr>
    <tr>
        <td colspan="2"></td>
        <td><asp:Button ID="btnSubmitOrder" runat="server" cssclass="btnred" text="Submit" /></td>
    </tr>
</table>
    <CC:SubFormValidator id="sfvDrops" runat="server" ControlToValidate="sfDrops" ErrorMessage="One or more drops is invalid" ValidationGroup="AddProject"></CC:SubFormValidator></div><asp:hiddenfield id="PriceReqStatus" value="0" runat="server"></asp:hiddenfield>
</asp:panel>
<asp:hiddenfield id="hdnPostback" runat="server"></asp:hiddenfield>

</CT:MasterPage>


<script type="text/javascript">
    $("[src*=plus]").live("click", function () {
        $(this).closest("tr").after("<tr><td></td><td colspan = '999'>" + $(this).next().html() + "</td></tr>")
        $(this).attr("src", "../images/minus.png");
    });
    $("[src*=minus]").live("click", function () {
        $(this).attr("src", "../images/plus.png");
        $(this).closest("tr").next().remove();
    });
</script>
