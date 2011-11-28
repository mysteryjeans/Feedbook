// Returns user friendly readlable date
function friendlyDate(date) {
	var timespan = (new Date()) - date;

	if(timespan > (365 * 24 * 60 * 60 * 1000))
		return Math.floor(timespan / (365 * 24 * 60 * 60 * 1000)) + ((timespan / (365 * 24 * 60 * 60 * 1000)) > 1 ? ' years ago' : ' year ago');

	if(timespan > (30 * 24 * 60 * 60 * 1000))
		return Math.floor(timespan / (30 * 24 * 60 * 60 * 1000))+ ((timespan / (30 * 24 * 60 * 60 * 1000)) > 1 ? ' months ago' : ' month ago');

	if(timespan > (24 * 60 * 60 * 1000))
		return Math.floor(timespan / (24 * 60 * 60 * 1000)) + ((timespan / (24 * 60 * 60 * 1000)) > 1 ? ' days ago' : ' day ago');

	if(timespan > (60 * 60 * 1000))
		return Math.floor(timespan / (60 * 60 * 1000)) + ((timespan / (60 * 60 * 1000)) > 1 ? ' hours ago' : ' hour ago');

	if(timespan > (60 * 1000))
		return Math.floor(timespan / (60 * 1000)) + ((timespan / (60 * 1000)) > 1 ? ' minutes ago' : ' minute ago');

	return 'A moment ago';
}

String.prototype.hashCode = function() {
	var hash = 0;
	if (this.length == 0)
		return hash;
	for (i = 0; i < this.length; i++) {
		char = this.charCodeAt(i);
		hash = ((hash<<5)-hash)+char;
		hash = hash & hash; // Convert to 32bit integer
	}
	return hash;
}
function showMessage(message) {
	alert(message);
}

function getQueryParameters() {
	// get the current URL
	var url = window.location.toString();
	//get the parameters
	url.match(/\?(.+)$/);
	var params = RegExp.$1;
	// split up the query string and store in an
	// associative array
	var params = params.split("&");
	var queryStringList = {};

	for(var i=0;i<params.length;i++) {
		var tmp = params[i].split("=");
		queryStringList[tmp[0]] = decodeURIComponent(tmp[1]);
	}

	return queryStringList;
}
