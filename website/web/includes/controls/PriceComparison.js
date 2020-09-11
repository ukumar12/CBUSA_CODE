Type.registerNamespace("AE");

AE.VendorProduct = function() {
    this.ProductId = 0;
    this.Price = 0;
    this.IsAvg = false;
    this.IsSub = false;
    this.IsSpecial = false;
    this.Sku = '';
    this.Multiply = 1;
    this.Qty = 0;
    this.State = 0;
    this.Name = '';
}

AE.ProductState = {
    Init: 0,
    Pending: 1,
    Accepted: 2,
    Omit: 3
}

AE.PriceComparison = function(element) {
    AE.PriceComparison.initializeBase(this, [element]);

    this._priceComparisonId = null;
    this._takeoffId = null;

    this._vendors = [];
    this._products = [];
    this._divCorral = null;

    this._tblMain = null;
    this._tblSub = null;
    this._tblAvg = null;
    this._tblSum = null;
    this._tblSpec = null;

    this._openSubForm = null;
    this._opsnSpecialForm = null;
    
    this._pbTemplate = null;

    this._pendingVendors = 0;
    this._classes = null;

    this._initVendors = null;
}

AE.PriceComparison.prototype = {

    initialize: function() {
        AE.PriceComparison.callBaseMethod(this, 'initialize');

        var el = this.get_element();

        this._LoadVendorCbDel = Function.createDelegate(this, this._LoadVendorCb);
        /*
        this._SubFormOpenDel = Function.createDelegate(this, this._SubFormOpen);
        this._OnSubFormOpenDel = Function.createDelegate(this, this._OnSubFormOpen);
        this._SubFormCloseDel = Function.createDelegate(this, this._SubFormClose);
        this._OnSubFormCloseDel = Function.createDelegate(this, this._OnSubFormClose);
        */
        this._UpdateSubDel = Function.createDelegate(this, this._UpdateSub);
        this._DoPostBackDel = Function.createDelegate(this, this._DoPostBack);
        this._RedrawAllDel = Function.createDelegate(this, this._redrawAll);
        this._UpdateSubDel = Function.createDelegate(this, this._UpdateSub);
        this._UpdateSubCbDel = Function.createDelegate(this, this._UpdateSubCb);
        this._RequestPricingDel = Function.createDelegate(this, this._RequestPricing);
        this._RequestPricingCbDel = Function.createDelegate(this, this._RequestPricingCb);
        this._OpenSubFormDel = Function.createDelegate(this, this._doOpenSubForm);
        this._OpenSpecialFormDel = Function.createDelegate(this, this._doOpenSpecialForm);

        //this._divForm = document.createElement('div');
        //el.parentNode.appendChild(this._divForm);
        //this._buildSubForm(this._divForm);

        this._tblMain = document.createElement('table');
        this._tblMain.className = 'tblcompr';
        this._tblMain.appendChild(document.createElement('tbody'));
        var head = document.createElement('th');
        head.colSpan = 8;
        head.innerHTML = 'Products All Vendors Priced';
        this._tblMain.childNodes[0].appendChild(document.createElement('tr')).appendChild(head);
        this._tblMain.childNodes[0].appendChild(this._getHeaderRow());
        this._addShimRow(this._tblMain.childNodes[0]);

        this._tblSub = document.createElement('table');
        this._tblSub.className = 'tblcompr comprsub';
        this._tblSub.appendChild(document.createElement('tbody'));

        head = document.createElement('th');
        head.colSpan = 8;
        head.innerHTML = 'Products Priced with Some Vendors Providing Substitutions';
        this._tblSub.childNodes[0].appendChild(document.createElement('tr')).appendChild(head);
        this._tblSub.childNodes[0].appendChild(this._getHeaderRow());
        this._addShimRow(this._tblSub.childNodes[0]);

        this._tblAvg = document.createElement('table');
        this._tblAvg.className = 'tblcompr compravg';
        this._tblAvg.appendChild(document.createElement('tbody'));

        head = document.createElement('th');
        head.colSpan = 8;
        head.innerHTML = 'Products Some Vendors did not Price';
        this._tblAvg.childNodes[0].appendChild(document.createElement('tr')).appendChild(head);
        this._tblAvg.childNodes[0].appendChild(this._getHeaderRow());
        this._addShimRow(this._tblAvg.childNodes[0]);

        head = document.createElement('th');
        head.colSpan = 8;
        head.innerHTML = 'Special Order Product Requests';
        this._tblSpec = document.createElement('table');
        this._tblSpec.className = 'tblcompr comprsum';
        this._tblSpec.appendChild(document.createElement('tbody'));
        this._tblSpec.childNodes[0].appendChild(document.createElement('tr')).appendChild(head);

        this._addShimRow(this._tblSpec.childNodes[0]);

        this._tblSum = document.createElement('table');
        this._tblSum.className = 'tblcompr comprsum';
        this._tblSum.appendChild(document.createElement('tbody'));

        var trSumHead = this._tblSum.childNodes[0].appendChild(document.createElement('tr'));
        var td = trSumHead.appendChild(document.createElement('th'));
        td.innerHTML = 'Summary';
        td.colSpan = 3;

        var trSumCnt = document.createElement('tr');
        trSumCnt.className = 'ttl1';
        this._tblSum.childNodes[0].appendChild(trSumCnt);
        td = trSumCnt.insertCell(trSumCnt.cells.length);
        td.innerHTML = 'Number of Prices Compared';
        td.colSpan = 3;

        var trSumMain = document.createElement('tr');
        trSumMain.className = 'ttl2';
        this._tblSum.childNodes[0].appendChild(trSumMain);
        td = trSumMain.insertCell(trSumMain.cells.length);
        td.innerHTML = 'Subtotal: Products All Vendors Priced';
        td.colSpan = 3;

        var trSumSub = document.createElement('tr');
        trSumSub.className = 'ttl3';
        this._tblSum.childNodes[0].appendChild(trSumSub);
        td = trSumSub.insertCell(trSumSub.cells.length);
        td.innerHTML = 'Subtotal: Priced Substitutions Included';
        td.colSpan = 3;

        var trSumAvg = document.createElement('tr');
        trSumAvg.className = 'ttl2';
        this._tblSum.childNodes[0].appendChild(trSumAvg);
        td = trSumAvg.insertCell(trSumAvg.cells.length);
        td.innerHTML = 'Subtotal: Averages Used if Pricing Missing';
        td.colSpan = 3;

        var trSumSpec = document.createElement('tr');
        trSumSpec.className = 'ttl3';
        this._tblSum.childNodes[0].appendChild(trSumSpec);
        td = trSumSpec.insertCell(trSumSpec.cells.length);
        td.innerHTML = 'Subtotal: Special Order Products';
        td.colSpan = 3;

        var trSumTotal = document.createElement('tr');
        trSumTotal.className = 'ttl1';
        this._tblSum.childNodes[0].appendChild(trSumTotal);
        td = trSumTotal.insertCell(trSumTotal.cells.length);
        td.innerHTML = 'Total Price';
        td.colSpan = 3;

        var trSumButtons = document.createElement('tr');
        trSumButtons.className = 'ttl4';
        this._tblSum.childNodes[0].appendChild(trSumButtons);
        td = trSumButtons.insertCell(trSumButtons.cells.length);
        td.innerHTML = '&nbsp;';
        td.colSpan = 3;

        this._addShimRow(this._tblSum.childNodes[0]);

        el.parentNode.appendChild(this._tblMain);
        el.parentNode.appendChild(this._tblSub);
        el.parentNode.appendChild(this._tblAvg);
        el.parentNode.appendChild(this._tblSpec);
        el.parentNode.appendChild(this._tblSum);

        //set the tbl members to the tbody objects for convenience
        this._tblMain = this._tblMain.childNodes[0];
        this._tblSub = this._tblSub.childNodes[0];
        this._tblAvg = this._tblAvg.childNodes[0];
        this._tblSpec = this._tblSpec.childNodes[0];

        if (this._initVendors) {
            var vendors = this._initVendors.split(',');
            for (var i = 0; i < vendors.length; i++) {
                this._addVendor(i, vendors[i]);
            }
        }
        //this._redrawAll();
    },

    //properties
    get_priceComparisonId: function() {
        return this._priceComparisonId;
    },
    set_priceComparisonId: function(value) {
        this._priceComparisonId = value;
    },
    get_takeoffId: function() {
        return this._takeoffId;
    },
    set_takeoffId: function(value) {
        this._takeoffId = value;
    },
    get_vendors: function() {
        return this._vendors;
    },
    set_vendors: function(value) {
        this._vendors = value;
    },
    get_products: function() {
        return this._products;
    },
    set_products: function(value) {
        this._products = value;
    },
    get_classes: function() {
        return this._classes;
    },
    set_classes: function(value) {
        this._classes = value;
    },
    get_initVendors: function() {
        return this._initVendors;
    },
    set_initVendors: function(value) {
        this._initVendors = value;
    },
    get_openSubForm: function() {
        return this._openSubForm;
    },
    set_openSubForm: function(value) {
        this._openSubForm = value;
    },
    get_openSpecialForm: function() {
        return this._openSpecialForm;
    },
    set_openSpecialForm: function(value) {
        this._openSpecialForm = value;
    },
    get_pbTemplate: function() {
        return this._pbTemplate;
    },
    set_pbTemplate: function(value) {
        this._pbTemplate = value;
    },

    //methods
    UpdateVendors: function(idString) {
        ShowSearching();
        var serviceCalled = false;
        if (idString.length > 0) {
            var ids = idString.split(',');
            var saved = {};
            for (var i = 0; i < this._vendors.length; i++) {
                saved[this._vendors[i].id] = this._vendors[i];
            }
            this._vendors = [];
            for (var i = 0; i < ids.length; i++) {
                if (saved[ids[i]]) {
                    this._vendors[i] = saved[ids[i]];
                    saved[ids[i]] = undefined;
                } else {
                    serviceCalled = true;
                    this._addVendor(i, ids[i]);
                }
            }
            for (i = 0; i < saved.length; i++) {
                if (saved[i]) {
                    this._removeVendor(saved[i].id);
                    delete saved[i];
                }
            }
        } else {
            this._vendors = [];
        }
        if (!serviceCalled) {
            this._redrawAll();
            HideSearching();
        }
    },
    _addVendor: function(idx, id) {
        this._pendingVendors++;
        Sys.Net.WebServiceProxy.invoke('/includes/controls/PriceComparison.asmx', 'LoadVendor', false, { 'PriceComparisonId': this._priceComparisonId, 'VendorID': id, 'TakeoffID': this._takeoffId }, this._LoadVendorCbDel, this._LoadVendorCbDel, { 'idx': idx });
    },
    _removeVendor: function(id) {
        Sys.Net.WebServiceProxy.invoke('/includes/controls/PriceComparison.asmx', 'RemoveVendor', false, { 'PriceComparisonId': this._priceComparisonId, 'VendorID': id, 'TakeoffID': this._takeoffId }, this._RedrawAllDel, this._RedrawAllDel);
        delete this._vendors[id];
    },
    _LoadVendorCb: function(res, ctxt) {
        if (res.get_exceptionType) return;
        var vObj = Sys.Serialization.JavaScriptSerializer.deserialize(res);
        this._vendors[ctxt.idx] = vObj;
        this._pendingVendors--;
        if (this._pendingVendors <= 0) {
            this._redrawAll();
            HideSearching();
        }
    },

    _redrawAll: function() {
        if (this._tblMain.rows.length) {
            /*
            while (this._tblMain.rows.length > 1) {
            this._tblMain.deleteRow(0);
            }
            */
            while (this._tblMain.rows.length > 2) {
                var tr = this._tblMain.rows[this._tblMain.rows.length - 2];
                for (var i = 0; i < tr.cells.length; i++) {
                    $clearHandlers(tr.cells[i]);
                }
                this._tblMain.deleteRow(this._tblMain.rows.length - 2);
            }
        }
        if (this._tblSub.rows.length) {
            while (this._tblSub.rows.length > 2) {
                var tr = this._tblSub.rows[this._tblSub.rows.length - 2];
                for (var i = 0; i < tr.cells.length; i++) {
                    $clearHandlers(tr.cells[i]);
                }
                this._tblSub.deleteRow(this._tblSub.rows.length - 2);
            }
        }
        if (this._tblAvg.rows.length) {
            while (this._tblAvg.rows.length > 2) {
                var tr = this._tblAvg.rows[this._tblAvg.rows.length - 2];
                for (var i = 0; i < tr.cells.length; i++) {
                    $clearHandlers(tr.cells[i]);
                }
                this._tblAvg.deleteRow(this._tblAvg.rows.length - 2);
            }
        }
        if (this._tblSpec.rows.length) {
            while (this._tblSpec.rows.length > 2) {
                var tr = this._tblSpec.rows[this._tblSpec.rows.length - 2];
                for (var i = 0; i < tr.cells.length; i++) {
                    $clearHandlers(tr.cells[i]);
                }
                this._tblSpec.deleteRow(this._tblSpec.rows.length - 2);
            }
        }

        while (this._tblSum.rows[7].cells.length > 1) {
            var c = this._tblSum.rows[7].lastChild;
            if (c) {
                $clearHandlers(c);
            }
            this._tblSum.rows[7].deleteCell(this._tblSum.rows[7].cells.length - 1);
        }
        this._tblMain.insertBefore(this._getHeaderRow(true), this._tblMain.childNodes[1]);
        this._tblSub.insertBefore(this._getHeaderRow(false), this._tblSub.childNodes[1]);
        this._tblAvg.insertBefore(this._getHeaderRow(false), this._tblAvg.childNodes[1]);
        this._tblSpec.insertBefore(this._getHeaderRow(false), this._tblSpec.childNodes[1]);

        /*
        for (var i in this._products) {
        this._addRow(this._products[i]);
        }
        */
        for (var i = 0; i < this._products.length; i++) {
            this._addRow(this._products[i]);
        }

        var trMain = document.createElement('tr');
        trMain.className = 'sbttl';
        this._tblMain.insertBefore(trMain, this._tblMain.lastChild);
        //this._addShimRow(this._tblMain);

        var td = trMain.insertCell(trMain.cells.length);
        td.className = 'subt';
        td.colSpan = 3;
        td.innerHTML = 'Subtotal: Products All Vendors Priced';

        var trSub = document.createElement('tr');
        trSub.className = 'sbttl';
        this._tblSub.insertBefore(trSub, this._tblSub.lastChild);
        //this._addShimRow(this._tblSub);

        td = trSub.insertCell(trSub.cells.length);
        td.className = 'subt';
        td.colSpan = 3;
        td.innerHTML = 'Subtotal: Priced Substitutions Included';

        var trAvg = document.createElement('tr');
        trAvg.className = 'sbttl';
        this._tblAvg.insertBefore(trAvg, this._tblAvg.lastChild);
        //this._addShimRow(this._tblAvg);

        td = trAvg.insertCell(trAvg.cells.length);
        td.className = 'subt';
        td.colSpan = 3;
        td.innerHTML = 'Subtotal: Averages Used if Prices Missing';

        var tmpClass = function() {
            this.price = -1;
            this.cells = [];
        }
        var highSub = new tmpClass(),
            highAvg = new tmpClass(),
            highMain = new tmpClass(),
            highTotal = new tmpClass();
        var lowSub = new tmpClass(),
            lowAvg = new tmpClass(),
            lowMain = new tmpClass(),
            lowTotal = new tmpClass();
        for (var i = 0; i < this._vendors.length; i++) {
            var td = trMain.insertCell(trMain.cells.length);
            td.innerHTML = '$' + FormatCurrency(this._vendors[i].mainTotal);
            td.className = 'norm';
            td = trSub.insertCell(trSub.cells.length);
            td.innerHTML = '$' + FormatCurrency(this._vendors[i].subTotal);
            td.className = 'norm';
            td = trAvg.insertCell(trAvg.cells.length);
            if (isNaN(this._vendors[i].avgTotal)) {
                td.innerHTML = 'N/A';
            } else {
                td.innerHTML = '$' + FormatCurrency(this._vendors[i].avgTotal);
            }
            td.className = 'norm';

            td = this._tblSum.rows[1].insertCell(this._tblSum.rows[1].cells.length).innerHTML = this._vendors[i].mainCnt + '/' + this._products.length;
            td.className = 'norm';

            this._tblSum.rows[2].insertCell(this._tblSum.rows[2].cells.length).innerHTML = '$' + FormatCurrency(this._vendors[i].mainTotal);
            if (highMain.price < 0 || highMain.price < this._vendors[i].mainTotal) {
                highMain.cells = [this._tblSum.rows[2].cells[this._tblSum.rows[2].cells.length - 1]];
                highMain.price = this._vendors[i].mainTotal;
            } else if (highMain.price == this._vendors[i].mainTotal) {
                highMain.cells.push([this._tblSum.rows[2].cells[this._tblSum.rows[2].cells.length - 1]]);
            }
            if (lowMain.price < 0 || lowMain.price > this._vendors[i].mainTotal) {
                lowMain.cells = [this._tblSum.rows[2].cells[this._tblSum.rows[2].cells.length - 1]];
                lowMain.price = this._vendors[i].mainTotal;
            } else if (lowMain.price == this._vendors[i].mainTotal) {
                lowMain.cells.push(this._tblSum.rows[2].cells[this._tblSum.rows[2].cells.length - 1]);
            }

            this._tblSum.rows[3].insertCell(this._tblSum.rows[3].cells.length).innerHTML = '$' + FormatCurrency(this._vendors[i].subTotal);
            if (highSub.price < 0 || highSub.price < this._vendors[i].subTotal) {
                highSub.cells = [this._tblSum.rows[3].cells[this._tblSum.rows[3].cells.length - 1]];
                highSub.price = this._vendors[i].subTotal;
            } else if (highSub.price == this._vendors[i].subTotal) {
                highSub.cells.push(this._tblSum.rows[3].cells[this._tblSum.rows[3].cells.length - 1]);
            }
            if (lowSub.price < 0 || lowSub.price > this._vendors[i].subTotal) {
                lowSub.cells = [this._tblSum.rows[3].cells[this._tblSum.rows[3].cells.length - 1]];
                lowSub.price = this._vendors[i].subTotal;
            } else if (lowSub.price == this._vendors[i].subTotal) {
                lowSub.cells.push(this._tblSum.rows[3].cells[this._tblSum.rows[3].cells.length - 1]);
            }

            if (isNaN(this._vendors[i].avgTotal)) {
                this._tblSum.rows[4].insertCell(this._tblSum.rows[4].cells.length).innerHTML = 'N/A';
            } else {
                this._tblSum.rows[4].insertCell(this._tblSum.rows[4].cells.length).innerHTML = '$' + FormatCurrency(this._vendors[i].avgTotal);
                if (highAvg.price < 0 || highAvg.price < this._vendors[i].avgTotal) {
                    highAvg.cells = [this._tblSum.rows[4].cells[this._tblSum.rows[4].cells.length - 1]];
                    highAvg.price = this._vendors[i].avgTotal;
                } else if (highAvg.price == this._vendors[i].avgTotal) {
                    highAvg.cells.push(this._tblSum.rows[4].cells[this._tblSum.rows[4].cells.length - 1]);
                }
                if (lowAvg.price < 0 || lowAvg.price > this._vendors[i].avgTotal) {
                    lowAvg.cells = [this._tblSum.rows[4].cells[this._tblSum.rows[4].cells.length - 1]];
                    lowAvg.price = this._vendors[i].avgTotal;
                } else if (lowAvg.price == this._vendors[i].avgTotal) {
                    lowAvg.cells.push(this._tblSum.rows[4].cells[this._tblSum.rows[4].cells.length - 1]);
                }
            }

            if (isNaN(this._vendors[i].specTotal)) {
                this._tblSum.rows[5].insertCell(this._tblSum.rows[5].cells.length).innerHTML = 'N/A';
            } else {
                this._tblSum.rows[5].insertCell(this._tblSum.rows[5].cells.length).innerHTML = '$' + FormatCurrency(this._vendors[i].specTotal);
            }

            this._vendors[i].totalTotal = 0;
            if (!isNaN(this._vendors[i].mainTotal)) this._vendors[i].totalTotal += this._vendors[i].mainTotal;
            if (!isNaN(this._vendors[i].subTotal)) this._vendors[i].totalTotal += this._vendors[i].subTotal;
            if (!isNaN(this._vendors[i].avgTotal)) this._vendors[i].totalTotal += this._vendors[i].avgTotal;
            if (!isNaN(this._vendors[i].specTotal)) this._vendors[i].totalTotal += this._vendors[i].specTotal;

            this._tblSum.rows[6].insertCell(this._tblSum.rows[6].cells.length).innerHTML = '$' + FormatCurrency(this._vendors[i].totalTotal);
            if (highTotal.price < 0 || highTotal.price < this._vendors[i].totalTotal) {
                highTotal.cells = [this._tblSum.rows[6].cells[this._tblSum.rows[6].cells.length - 1]];
                highTotal.price = this._vendors[i].totalTotal;
            } else if (highTotal.price == this._vendors[i].totalTotal) {
                highTotal.cells.push(this._tblSum.rows[6].cells[this._tblSum.rows[6].cells.length - 1]);
            }
            if (lowTotal.price < 0 || lowTotal.price > this._vendors[i].totalTotal) {
                lowTotal.cells = [this._tblSum.rows[6].cells[this._tblSum.rows[6].cells.length - 1]];
                lowTotal.price = this._vendors[i].totalTotal;
            } else if (lowTotal.price == this._vendors[i].totalTotal) {
                lowTotal.cells.push(this._tblSum.rows[6].cells[this._tblSum.rows[6].cells.length - 1]);
            }

            if (this._vendors[i].isIncomplete) {
                var a = document.createElement('a');
                a.style.cursor = 'pointer';
                a.innerHTML = '<div style="text-align:center;height:24px;line-height:12px;">Request<br/>Prices</div>';
                a.vendorid = this._vendors[i].id;
                a.className = 'smaller btnblue center';
                a.style.whiteSpace = 'nowrap';
                $addHandler(a, 'click', this._RequestPricingDel);
                var tdBtn = this._tblSum.rows[7].insertCell(this._tblSum.rows[7].cells.length);
                tdBtn.appendChild(a);
            } else if (this._vendors[i].isPending) {
                var txt = document.createTextNode('Prices pending');
                var tdBtn = this._tblSum.rows[7].insertCell(this._tblSum.rows[7].cells.length);
                tdBtn.appendChild(txt);
            } else {
                var a = document.createElement('a');
                a.style.cursor = 'pointer';
                a.innerHTML = 'Place Order';
                a.vendorid = this._vendors[i].id;
                $addHandler(a, 'click', this._DoPostBackDel);
                var tdBtn = this._tblSum.rows[7].insertCell(this._tblSum.rows[7].cells.length);
                tdBtn.className = 'plcordr';
                tdBtn.appendChild(a);
            }
            this._DoPostBack(null);
        }
        for (var c = 0; c < highMain.cells.length; c++) {
            highMain.cells[c].className = (highMain.cells[c].className && highMain.cells[c].className != this._classes['normal']) ? highMain.cells[c].className + 'high' : 'high';
        }
        for (var c = 0; c < highSub.cells.length; c++) {
            highSub.cells[c].className = (highSub.cells[c].className && highSub.cells[c].className != this._classes['normal']) ? highSub.cells[c].className + 'high' : 'high';
        }
        for (var c = 0; c < highTotal.cells.length; c++) {
            highTotal.cells[c].className = (highTotal.cells[c].className && highTotal.cells[c].className != this._classes['normal']) ? highTotal.cells[c].className + 'high' : 'high';
        }
        for (var c = 0; c < highAvg.cells.length; c++) {
            highAvg.cells[c].className = (highAvg.cells[c].className && highAvg.cells[c].className != this._classes['normal']) ? highAvg.cells[c].className + 'high' : 'high';
        }
        for (var c = 0; c < lowMain.cells.length; c++) {
            lowMain.cells[c].className = (lowMain.cells[c].className && lowMain.cells[c].className != this._classes['normal']) ? lowMain.cells[c].className + 'low' : 'low';
        }
        for (var c = 0; c < lowSub.cells.length; c++) {
            lowSub.cells[c].className = (lowSub.cells[c].className && lowSub.cells[c].className != this._classes['normal']) ? lowSub.cells[c].className + 'low' : 'low';
        }
        for (var c = 0; c < lowAvg.cells.length; c++) {
            lowAvg.cells[c].className = (lowAvg.cells[c].className && lowAvg.cells[c].className != this._classes['normal']) ? lowAvg.cells[c].className + 'low' : 'low';
        }
        for (var c = 0; c < lowTotal.cells.length; c++) {
            lowTotal.cells[c].className = (lowTotal.cells[c].className && lowTotal.cells[c].className != this._classes['normal']) ? lowTotal.cells[c].className + 'low' : 'low';
        }

        for (var i = 0; i < this._vendors.length; i++) {
            if (this._vendors[i].mainTotal) {
                var pct = Math.round(100 * (this._vendors[i].mainTotal - lowMain.price) / lowMain.price);
                this._tblSum.rows[2].cells[i + 1].appendChild(document.createElement('br'));
                if (pct > 0) {
                    var span = this._tblSum.rows[2].cells[i + 1].appendChild(document.createElement('span'));
                    span.className = 'smaller';
                    span.innerHTML = pct + '% from low';
                }
            }

            if (this._vendors[i].subTotal) {
                pct = Math.round(100 * (this._vendors[i].subTotal - lowSub.price) / lowSub.price);
                this._tblSum.rows[3].cells[i + 1].appendChild(document.createElement('br'));
                var span = this._tblSum.rows[3].cells[i + 1].appendChild(document.createElement('span'));
                span.className = 'smaller';
                span.innerHTML = pct + '% from low';
            }

            if (this._vendors[i].avgTotal) {
                pct = Math.round(100 * (this._vendors[i].avgTotal - lowAvg.price) / lowAvg.price);
                this._tblSum.rows[4].cells[i + 1].appendChild(document.createElement('br'));
                if (pct > 0) {
                    span = this._tblSum.rows[4].cells[i + 1].appendChild(document.createElement('span'));
                    span.className = 'smaller';
                    span.innerHTML = pct + '% from low';
                }
            }

            if (this._vendors[i].totalTotal) {
                pct = Math.round(100 * (this._vendors[i].totalTotal - lowTotal.price) / lowTotal.price);
                this._tblSum.rows[6].cells[i + 1].appendChild(document.createElement('br'));
                if (pct > 0) {
                    span = this._tblSum.rows[6].cells[i + 1].appendChild(document.createElement('span'));
                    span.className = 'smaller';
                    span.innerHTML = pct + '% from low';
                }
            }
        }

        if (this._tblMain.rows.length < 5) {
            this._tblMain.parentNode.style.display = 'none';
        } else {
            this._tblMain.parentNode.style.display = '';
        }
        if (this._tblSub.rows.length < 5) {
            this._tblSub.parentNode.style.display = 'none';
        } else {
            this._tblSub.parentNode.style.display = '';
        }
        if (this._tblAvg.rows.length < 5) {
            this._tblAvg.parentNode.style.display = 'none';
        } else {
            this._tblAvg.parentNode.style.display = '';
        }
        if (this._tblSpec.rows.length < 4) {
            this._tblSpec.parentNode.style.display = 'none';
        } else {
            this._tblSpec.parentNode.style.display = '';
        }
    },

    _DoPostBack: function(e) {
        this.get_element().value = 'Updated';
        var pb;
        if (e) {
            var target = e.target ? e.target : e.srcElement;
            pb = this.get_pbTemplate().replace('##VID##', target.vendorid);
        } else {
            pb = this.get_pbTemplate().replace('##VID##', 'update');
        }
        for (var tr = 0; tr < this._tblMain.rows.length; tr++) {
            for (var td = 0; td < this._tblMain.rows[tr].cells.length; td++) {
                var cell = this._tblMain.rows[tr].cells[td];
                if (cell.hdn)
                    cell.hdn.value = Sys.Serialization.JavaScriptSerializer.serialize(cell.vproduct);
            }
        }
        for (var tr = 0; tr < this._tblSub.rows.length; tr++) {
            for (var td = 0; td < this._tblSub.rows[tr].cells.length; td++) {
                var cell = this._tblSub.rows[tr].cells[td];
                if (cell.hdn)
                    cell.hdn.value = Sys.Serialization.JavaScriptSerializer.serialize(cell.vproduct);
            }
        }
        for (var tr = 0; tr < this._tblAvg.rows.length; tr++) {
            for (var td = 0; td < this._tblAvg.rows[tr].cells.length; td++) {
                var cell = this._tblAvg.rows[tr].cells[td];
                if (cell.hdn)
                    cell.hdn.value = Sys.Serialization.JavaScriptSerializer.serialize(cell.vproduct);
            }
        }
        for (var tr = 0; tr < this._tblSpec.rows.length; tr++) {
            for (var td = 0; td < this._tblSpec.rows[tr].cells.length; td++) {
                var cell = this._tblSpec.rows[tr].cells[td];
                if (cell.hdn)
                    cell.hdn.value = Sys.Serialization.JavaScriptSerializer.serialize(cell.vproduct);
            }
        }
        eval(pb);
    },

    _addRow: function(product) {
        var tr = document.createElement('tr');
        var td = document.createElement('td');
        tr.appendChild(td);
        td.innerHTML = product.qty;

        td = document.createElement('td');
        tr.appendChild(td);
        td.innerHTML = product.sku ? product.sku : 'N/A';

        td = document.createElement('td');
        tr.appendChild(td);
        td.innerHTML = product.name;
        /*
        var a = document.createElement('a');
        a.style.cursor = 'pointer';
        a.productId = product.productid;
        a.priceComparisonId = this._priceComparisonId;
        a.innerHTML = product.name;
        td.appendChild(a);
        $addHandler(a, 'click', function() { eval('OpenIndicator(' + product.productid + ')') });
        */

        var lowPrice = null;
        for (var i = 0; i < this._vendors.length; i++) {
            if (this._vendors[i].products[product.id].Price > 0 && (lowPrice == null || this._vendors[i].products[product.id].Price < lowPrice)) {
                lowPrice = this._vendors[i].products[product.id].Price;
            }
        }

        var low = [];
        var high = [];
        var tblNo = 0;
        var vTotals = {};
        var el = this.get_element();
        for (var i = 0; i < this._vendors.length; i++) {
            var vProd = this._vendors[i].products[product.id];

            if (vProd.Price >= 0 && vProd.State != AE.ProductState.Omit) {
                this._vendors[i].total += vProd.Price;
                vTotals[i] = vProd.Price;
            } else {
                vTotals[i] = 0;
            }
            td = document.createElement('td');
            tr.appendChild(td);

            var hdn = document.createElement('input');
            hdn.type = 'hidden';
            hdn.id = el.id + '_VID' + this._vendors[i].id + '_PID' + product.id;
            hdn.name = el.name + '|VID' + this._vendors[i].id + '|PID' + product.id;
            td.appendChild(hdn);

            td.hdn = hdn;

            td.product = product;
            td.vendor = this._vendors[i];
            td.vproduct = vProd;
            if (!vProd.Qty) vProd.Qty = product.qty;
            td.style.position = 'relative';
            var tdClass = this._classes.normal;
            if (vProd.IsSpecial) {
                tblNo = 3;
                this._vendors[i].specCnt++;
                if (vProd.Price > 0) {
                    td.appendChild(document.createTextNode('$' + FormatCurrency(vProd.Price)));
                    if (vProd.IsSub) {
                        $addHandler(td, 'click', this._OpenSubFormDel);
                        switch (vProd.State) {
                            case AE.ProductState.Accepted:
                                tdClass = this._classes.subAccepted;
                                break;
                            case AE.ProductState.Omit:
                                tdClass = this._classes.subRejected;
                                break;
                            case AE.ProductState.Pending:
                                tdClass = this._classes.subInit;
                                this._vendors[i].isPending = true;
                                break;
                            case AE.ProductState.Init:
                                tdClass = this._classes.subInit;
                                this._vendors[i].isIncomplete = true;
                                break;
                        }
                    }
                } else {
                    td.appendChild(document.createTextNode('N/A'));
                    switch (vProd.State) {
                        case AE.ProductState.Accepted:
                            tdClass = this._classes.noPriceAccepted;
                            break;
                        case AE.ProductState.Omit:
                            tdClass = this._classes.noPriceOmit;
                            break;
                        case AE.ProductState.Pending:
                            tdClass = this._classes.noPricePending;
                            this._vendors[i].isPending = true;
                            break;
                        case AE.ProductState.Init:
                            tdClass = this._classes.noPrice;
                            this._vendors[i].isIncomplete = true;
                            break;
                    }
                    $addHandler(td, 'click', this._OpenSpecialFormDel);
                }

            }
            else if (vProd.IsAvg) {
                if (vProd.Price > 0) {
                    td.appendChild(document.createTextNode('$' + FormatCurrency(vProd.Price)));
                } else {
                    td.appendChild(document.createTextNode('N/A'));
                }
                switch (vProd.State) {
                    case AE.ProductState.Accepted:
                        tdClass = this._classes.noPriceAccepted;
                        break;
                    case AE.ProductState.Omit:
                        tdClass = this._classes.noPriceOmit;
                        break;
                    case AE.ProductState.Pending:
                        tdClass = this._classes.noPricePending;
                        this._vendors[i].isPending = true;
                        break;
                    case AE.ProductState.Init:
                        tdClass = this._classes.noPrice;
                        this._vendors[i].isIncomplete = true;
                        break;
                }

                if (tblNo < 2) tblNo = 2;
                this._vendors[i].avgCnt++;
                //$addHandler(td, 'click', this._SubFormOpenDel);
                $addHandler(td, 'click', this._OpenSpecialFormDel);
            }
            /*
            else if (vProd.IsAvg) {
            if (tblNo < 2) tblNo = 2;
            switch(vProd.State) {
            case AE.ProductState.Accepted:
            tdClass = this._classes.noPriceAccepted;
            break;
            $addHandler(td,'click', this._SubFormOpenDel);
            this._vendors[i].avgCnt++;
            td.appendChild(document.createTextNode('$' + vProd.Price));
            } 
            */
            else if (vProd.IsSub) {
                if (tblNo < 1) tblNo = 1;
                switch (vProd.State) {
                    case AE.ProductState.Accepted:
                        tdClass = this._classes.subAccepted;
                        break;
                    case AE.ProductState.Omit:
                        tdClass = this._classes.subRejected;
                        break;
                    case AE.ProductState.Pending:
                        tdClass = this._classes.subInit;
                        this._vendors[i].isPending = true;
                        break;
                    case AE.ProductState.Init:
                        tdClass = this._classes.subInit;
                        this._vendors[i].isIncomplete = true;
                        break;
                }
                //$addHandler(td, 'click', this._SubFormOpenDel);
                $addHandler(td, 'click', this._OpenSubFormDel);
                this._vendors[i].subCnt++;
                td.appendChild(document.createTextNode('$' + FormatCurrency(vProd.Price)));
            } else {
                this._vendors[i].mainCnt++;
                td.appendChild(document.createTextNode('$' + FormatCurrency(vProd.Price)));
                vProd.State = AE.ProductState.Accepted;
            }
            if (vProd.Price >= 0) {
                if (low.length == 0 || vProd.Price == low[0].vproduct.Price) {
                    low.push(td);
                } else if (vProd.Price < low[0].vproduct.Price) {
                    low = [td];
                }
            }
            if (vProd.Price >= 0) {
                if (high.length == 0 || vProd.Price == high[0].vproduct.Price) {
                    high.push(td);
                } else if (vProd.Price > high[0].vproduct.Price) {
                    high = [td];
                }
            }

            if (vProd.Price > 0 && lowPrice) {
                var pct = Math.round(100 * (vProd.Price - lowPrice) / lowPrice);
                td.appendChild(document.createElement('br'));
                if (pct > 0) {
                    var span = td.appendChild(document.createElement('span'));
                    span.className = 'smaller';
                    span.innerHTML = pct + '% from low';
                }
            }

            td.className = tdClass;
        }
        if (this._vendors.length > 0) {
            for (var i = 0; i < low.length; i++) {
                if (low[i].className != this._classes['noPriceOmit']) {
                    low[i].className = low[i].className && low[i].className != 'norm' ? low[i].className + 'low' : 'low';
                }
            }
            for (var i = 0; i < high.length; i++) {
                if (high[i].className != this._classes['noPriceOmit']) {
                    if (high[i].className) {
                        if (high[i].className.indexOf('low') < 0) {
                            high[i].className = high[i].className == 'norm' ? 'high' : high[i].className + 'high';
                        }
                    } else {
                        high[i].className = 'high';
                    }
                }
            }
            /*
            if (low) low.className = low.className ? low.className + 'low' : 'low';
            if (high) high.className = high.className ? high.className + 'high' : 'high';
            */
        }
        switch (tblNo) {
            case 0:
                this._tblMain.insertBefore(tr, this._tblMain.rows[this._tblMain.rows.length - 1]);
                for (var i = 0; i < this._vendors.length; i++) {
                    this._vendors[i].mainTotal += vTotals[i];
                }
                break;
            case 1:
                this._tblSub.insertBefore(tr, this._tblSub.rows[this._tblSub.rows.length - 1]);
                for (var i = 0; i < this._vendors.length; i++) {
                    this._vendors[i].subTotal += vTotals[i];
                }
                break;
            case 2:
                this._tblAvg.insertBefore(tr, this._tblAvg.rows[this._tblAvg.rows.length - 1]);
                for (var i = 0; i < this._vendors.length; i++) {
                    this._vendors[i].avgTotal += vTotals[i];
                }
                break;
            case 3:
                this._tblSpec.insertBefore(tr, this._tblSpec.rows[this._tblSpec.rows.length - 1]);
                for (var i = 0; i < this._vendors.length; i++) {
                    this._vendors[i].specTotal += vTotals[i];
                }
                break;
        }
    },

    _getHeaderRow: function(refresh) {
        if (refresh || !this._headerRow) {
            this._headerRow = document.createElement('tr');
            var th = document.createElement('th');
            th.innerHTML = 'Qty';
            this._headerRow.appendChild(th);

            th = document.createElement('th');
            th.innerHTML = 'Sku';
            this._headerRow.appendChild(th);

            th = document.createElement('th');
            th.innerHTML = 'Product Name';
            this._headerRow.appendChild(th);

            if (this._tblSum) {
                for (var i = this._tblSum.rows[0].cells.length - 1; i >= 1; i--) {
                    for (j = 0; j < 7; j++) {
                        $clearHandlers(this._tblSum.rows[j].cells[i]);
                        this._tblSum.rows[j].deleteCell(i);
                    }
                }
            }

            for (var i = 0; i < this._vendors.length; i++) {
                th = document.createElement('th');
                if (this._vendors[i].name) th.innerHTML = this._vendors[i].name;
                this._headerRow.appendChild(th);
                this._tblSum.rows[0].appendChild(th.cloneNode(true));
                this._vendors[i].mainCnt = 0;
                this._vendors[i].mainTotal = 0;
                this._vendors[i].subCnt = 0;
                this._vendors[i].subTotal = 0;
                this._vendors[i].avgCnt = 0;
                this._vendors[i].avgTotal = 0;
                this._vendors[i].specCnt = 0;
                this._vendors[i].specTotal = 0;
                this._vendors[i].isPending = false;
                this._vendors[i].isIncomplete = false;
            }
        }
        return this._headerRow.cloneNode(true);
    },

    _addShimRow: function(tbl) {
        if (!this._shimRow) {
            //var tr = tbl.appendChild(document.createElement('tr'));
            var tr = document.createElement('tr');
            var td = tr.appendChild(document.createElement('td'));
            td.className = 'shim';
            td.style.width = '44px';
            var spacer = td.appendChild(document.createElement('img'));
            spacer.style.width = '44px';
            spacer.style.height = '1px';
            spacer.src = '/images/spacer.gif';
            td.appendChild(document.createElement('br'));

            td = tr.appendChild(document.createElement('td'));
            td.className = 'shim';
            td.style.width = '89px';
            spacer = td.appendChild(document.createElement('img'));
            spacer.style.width = '89px';
            spacer.style.height = '1px';
            spacer.src = '/images/spacer.gif';
            td.appendChild(document.createElement('br'));

            td = tr.appendChild(document.createElement('td'));
            td.className = 'shim';
            td.style.width = '250px';
            spacer = td.appendChild(document.createElement('img'));
            spacer.style.width = '250px';
            spacer.style.height = '1px';
            spacer.src = '/images/spacer.gif';
            td.appendChild(document.createElement('br'));

            for (var i = 0; i < 5; i++) {
                td = tr.appendChild(document.createElement('td'));
                td.className = 'shim';
                td.style.width = '112px';
                spacer = td.appendChild(document.createElement('img'));
                spacer.style.width = '112px';
                spacer.style.height = '1px';
                spacer.src = '/images/spacer.gif';
                td.appendChild(document.createElement('br'));
            }
            this._shimRow = tr;
        }
        tbl.appendChild(this._shimRow.cloneNode(true));
    },

    _addMarketIndicator: function(productId) {
        Sys.Net.WebServiceProxy.invoke('/includes/controls/PriceComparison.asmx', 'AddMarketIndicator', false, { 'ProductID': productId }, null, null);
    },

    _doOpenSubForm: function(e) {
        var t = e.target;
        while (!t.vendor && t.parentNode) {
            t = t.parentNode;
        }
        if (t.vendor) this._openSubForm(t.vendor.id, t.product, t.vproduct);
    },

    _doOpenSpecialForm: function(e) {
        var t = e.target;
        while (!t.vendor && t.parentNode) {
            t = t.parentNode;
        }
        if (t.vendor) this._openSpecialForm(t.vendor.id, t.product, t.vproduct);
    },

    _updateVendorCB: function(res, ctxt) {
        if (res.get_excptionType) return;
        ctxt.Price = res.Price;
    },

    AcceptSubstitution: function(vendorId, productId, vproduct) {
        vproduct.State = AE.ProductState.Accepted;
        //this._redrawAll();
        Sys.Net.WebServiceProxy.invoke('/includes/controls/PriceComparison.asmx', 'UpdateVendorProduct', false, { 'PriceComparisonId': this._priceComparisonId, 'VendorId': vendorId, 'TakeoffProductId': productId, 'vprod': vproduct }, this._UpdateSubCbDel, this._UpdateSubCbDel, vproduct);
    },

    AcceptProduct: function(vendorId, productId, vproduct) {
        vproduct.State = AE.ProductState.Accepted;
        //this._redrawAll();
        Sys.Net.WebServiceProxy.invoke('/includes/controls/PriceComparison.asmx', 'UpdateVendorProduct', false, { 'PriceComparisonId': this._priceComparisonId, 'VendorId': vendorId, 'TakeoffProductId': productId, 'vprod': vproduct }, this._UpdateSubCbDel, this._UpdateSubCbDel, vproduct);
    },

    OmitProduct: function(vendorId, productId, vproduct) {
        vproduct.State = AE.ProductState.Omit;
        //this._redrawAll();
        Sys.Net.WebServiceProxy.invoke('/includes/controls/PriceComparison.asmx', 'UpdateVendorProduct', false, { 'PriceComparisonId': this._priceComparisonId, 'VendorId': vendorId, 'TakeoffProductId': productId, 'vprod': vproduct }, this._UpdateSubCbDel, this._UpdateSubCbDel, vproduct);
    },

    _UpdateSub: function(e) {
        var target = e.target ? e.target : e.srcElement;
        var div = target.parentNode.parentNode.parentNode.parentNode.parentNode;
        if (div.curCell.vproduct.IsSub) {
            div.curCell.vproduct.qty = div.QtyInput.value;
        }
        if (target.accept) {
            div.curCell.vproduct.State = AE.ProductState.Accepted;
        } else {
            div.curCell.vproduct.State = AE.ProductState.Omit;
        }
        Sys.Net.WebServiceProxy.invoke('/includes/controls/PriceComparison.asmx', 'UpdateVendorProduct', false, { 'PriceComparisonId': this._priceComparisonId, 'VendorId': div.curCell.vendor.id, 'TakeoffProductId': div.curCell.product.id, 'vprod': Sys.Serialization.JavaScriptSerializer.serialize(div.curCell.vproduct) }, this._UpdateSubCbDel, this._UpdateSubCbDel, { cell: div.curCell });
    },

    _UpdateSubCb: function(res, ctxt) {
        if (res.get_exceptionType) return;
        /*
        if (ctxt.State == AE.ProductState.Accepted) {
        ctxt.curCell.className = ctxt.curCell.vproduct.IsSub ? this._classes.subAccepted : this._classes.noPriceAccepted;
        } else {
        ctxt.curCell.className = ctxt.curCell.vproduct.IsSub ? this._classes.subRejected : this._classes.noPriceRejected;
        }
        */
        ctxt.Price = res.Price;
        this._redrawAll();
    },

    _RequestPricing: function(e) {
        var target = e.target ? e.target : e.srcElement;
        while (!target.vendorid && target.parentNode) {
            target = target.parentNode;
        }
        if (!target.vendorid) return;
        var params = {
            PriceComparisonId: this._priceComparisonId,
            VendorId: target.vendorid
        }
        var ctxt = { 'vendorid': target.vendorid };
        ShowSearching();
        Sys.Net.WebServiceProxy.invoke('/includes/controls/PriceComparison.asmx', 'RequestPricing', false, params, this._RequestPricingCbDel, this._RequestPricignCbDel, ctxt);
    },

    _RequestPricingCb: function(res, ctxt) {
        if (res && res.get_exceptionType) {
            HideSearching();
            return;
        }
        var idx = 0;
        while (idx < this._vendors.length && this._vendors[idx].id != ctxt.vendorid) {
            idx++;
        }
        if (idx < this._vendors.length) {
            for (var i in this._vendors[idx].products) {
                var p = this._vendors[idx].products[i];
                if (!p.State && (p.IsSub || p.IsAvg || p.IsSpecial)) {
                    p.State = AE.ProductState.Pending;
                }
            }
        }
        this._redrawAll();
        HideSearching();
    },

    _RaiseServerCallback: function(arg, cb, ctxt) {
        var ref = this._cbTemplate;
        var argument = arg;
        var callback = cb;
        var context = ctxt;
        eval(ref);
    }
}
AE.PriceComparison.registerClass('AE.PriceComparison', Sys.UI.Control);

AE.SubstituteForm = function(element) {
    AE.SubstituteForm.initializeBase(this, [element]);

}

AE.SubstituteForm.prototype = {

    initialize: function() {
        AE.SubstituteForm.callBaseMethod(this, 'initialize');

    }
}
AE.SubstituteForm.registerClass('AE.SubstituteForm', Sys.UI.Control);        

if (typeof (Sys) !== 'undefined') Sys.Application.notifyScriptLoaded();


