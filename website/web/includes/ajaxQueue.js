//The global Queue object
var g_q;

//Helper function to instantiate the object & add request to queue
function enqueue(sUrl, postFuncCall, preFuncCall) {
	if(!g_q) g_q = new ajaxQueue;

	if(!g_q.enqueue(sUrl, postFuncCall, preFuncCall))
		return false;

	if(!g_q.isProcessing)
		g_q.process();

	return true;
}

//Queue Class
function ajaxQueue() {
	this.queue = new Array();
	this.position = null;
	this.isProcessing;
	this.callPreFunctions = true;

	//Adds an AJAX request to the queue
	//sUrl = Full AJAX URL to call
	//funcCall = the function to call passing AJAX result
	this.enqueue = function(sUrl, postFuncCall, preFuncCall) {
		//Don't queue already queued request
		for(var i = 0; i < this.queue.length; i++)
			if(this.queue[i][0] == sUrl && this.queue[i][1] == postFuncCall && this.queue[i][2] == preFuncCall) return false;

		//initialize or increment position
		if(this.position == null) {
			this.isProcessing = false;
			this.callPreFunctions = true;
			this.position = 0;
		} else {
			++this.position;
		}
		
		//Add the request to the queue
		this.queue[this.position] = new Array();
		this.queue[this.position][0] = sUrl;
		this.queue[this.position][1] = postFuncCall;
		this.queue[this.position][2] = preFuncCall;

		//If processing and prefunction specified, execute it
		if(this.isProcessing && preFuncCall)
			eval(preFuncCall);
			
		return true;
	}

	//Dequeue all
	this.process = function() {
		if(this.position == null || this.queue.length == 0) return;

		//update processing state
		this.isProcessing = true;

		//Process pre functions ahead of time
		if(this.callPreFunctions) {
			var r = getXMLHTTP();
			var args;
			if(r) {
				for(var i = 0; i < this.queue.length; i++) {
					if(this.queue[i][2])
						eval(this.queue[i][2]);
				}
			}
			//clear the callPreFunctions flag to prevent multiple calls
			this.callPreFunctions = false;
		}

		//make the AJAX request
		this.getAjaxQueueResult(this.queue[0][0], this.queue[0][1]);
	}

	//Make the AJAX call and process results
	this.getAjaxQueueResult = function(sUrl, postFuncCall) {
		var r = getXMLHTTP();
		if(r)
		{
			r.open("GET", sUrl, true);
			r.onreadystatechange = function() {
				if(r.readyState == 4) {
					if(postFuncCall) postFuncCall(r.responseText);

					//Remove the element and decrement position
					g_q.queue.splice(0, 1);
					g_q.position > 0 ? --g_q.position : g_q.position = null;

					//If this was the last request clear variables 
					//and return, otherwise process next request
					if(g_q.position == null) {
						g_q.isProcessing = false;
						g_q.callPreFunctions = true;
						return;
					} else {
						g_q.process();
					}
				}
			}
			r.send(null);
		}
	}
}
