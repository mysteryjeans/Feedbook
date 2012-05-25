
(function($) {
	$.extend({
		jGFeed : function(url, fnk, num, key) {
			// Make sure url to get is defined
			if(url == null)
				return false;
			// Build Google Feed API URL
			var gurl = "http://ajax.googleapis.com/ajax/services/feed/load?v=1.0&callback=?&q="+url;
			if(num != null)
				gurl += "&num="+num;
			if(key != null)
				gurl += "&key="+key;
			// AJAX request the API
			$.getJSON(gurl, function(data) {
				if(typeof fnk == 'function') {
					if(data.responseStatus == 200)
						fnk.call(this, data.responseData.feed);
					else throw data.responseDetails + '';
				} else
					return false;
			});
		}
	});
})(jQuery);


(function($) {
	$.extend({
		jGFindFeed : function(query, fnk, num, key) {
			// Make sure url to get is defined
			if(query == null)
				return false;
			// Build Google Feed API URL
			var gurl = "http://ajax.googleapis.com/ajax/services/feed/find?v=1.0&callback=?&q="+query;
			if(num != null)
				gurl += "&num="+num;
			if(key != null)
				gurl += "&key="+key;
			// AJAX request the API
			$.getJSON(gurl, function(data) {
				if(typeof fnk == 'function') {
					if(data.responseStatus == 200)
						fnk.call(this, data.responseData);
					else throw data.responseDetails + '';
				} else
					return false;
			});
		}
	});
})(jQuery);