var sessionTimer;
var loginTimer;
var totalTimeoutMinutes = 60;
var warningMinutes = 3;
var timeoutInterval = (totalTimeoutMinutes - warningMinutes) * 60000;

function extend(s) {
	if(s == '1') {
		clearTimeout(sessionTimer);
		clearTimeout(loginTimer);
		sessionTimer = setTimeout("extendSession()", timeoutInterval);
		document.getElementById('divAdminPopup').style.display = 'none';
	} else {
		gotoLogin();
	}
}

function gotoLogin() {window.location = '/admin/logout.aspx';}

function calculatePosition() {
	var clientBounds = getClientBounds();
	var width = clientBounds.split('|')[0];
	var height = clientBounds.split('|')[1];
	var inner = document.getElementById('divAdminPopupInner');
	var element = document.getElementById('divAdminPopupInfo');
	var popup = document.getElementById('divAdminPopup');
	if (!element || !inner || !popup || popup.style.display == 'none') return;
    
	var pageHeight = getPageHeight();
	var innerHeight = height;
	if(pageHeight > innerHeight) innerHeight = pageHeight; else innerHeight = innerHeight - 10;
	inner.style.height = innerHeight + 'px';

	var x = 0;
	var y = 0;

    if (document.documentElement && document.documentElement.scrollTop) {
        x = document.documentElement.scrollLeft;
        y = document.documentElement.scrollTop;
    } else {
        x = document.body.scrollLeft;
        y = document.body.scrollTop;
    }

	x = Math.max(0, Math.floor(x + width / 2.0 - element.offsetWidth / 2.0 - popup.offsetWidth / 2.0));
	y = Math.max(0, Math.floor(y + height / 2.0 - element.offsetHeight / 2.0));
	
	element.style.left = x + 'px';
	element.style.top = y + 'px';
}

function extendSession() {
	document.getElementById('warningMinutes').innerHTML = warningMinutes + ' ';
	document.getElementById('divAdminPopup').style.display = '';
	calculatePosition();
	loginTimer = setTimeout("gotoLogin()", warningMinutes * 60000);
}

function getPageHeight() {
	return document.getElementById('divAdminWrapper').offsetHeight;
}

function sessionBtnClick(btn) {
	if(btn.value=='Continue Logged-In') {
		enqueue("/admin/ajax.aspx?F=ExtendSession", extend);
	} else if(btn.value=='Logout Now') {
		gotoLogin();
	} else {
		loginTimer = setTimeout("gotoLogin()", warningMinutes * 60000);
	}
	document.getElementById('divAdminPopup').style.display = 'none';
}

sessionTimer = setTimeout("extendSession()", timeoutInterval);
