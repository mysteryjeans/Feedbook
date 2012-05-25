$(document).ready( function() {
	loadTrends();
})

function loadTrends() {

	$.jGFeed('http://www.google.com/trends/hottrends/atom/hourly', function (mytrends) {
		if(mytrends.entries.length > 0) {
			$('#googleTrends').html(mytrends.entries[0].content);
			var index = 0;
			$('#googleTrends a').each( function () {
				if(index < 5)
					addTrend(this.innerHTML, this.href);
				index++;
			});
		}
		$('#googleTrends').html(null);
	}, 10);
	$.jGFeed('http://buzzlog.yahoo.com/feeds/buzzoverl.xml', function (mytrends) {
		for(var i=0; i < mytrends.entries.length && i < 5; i++) {
			var title = mytrends.entries[i].title;
			title = title.indexOf('. ') != -1 ? title.substring(title.indexOf('. ') + 2) : title;
			addTrend(title.toLowerCase(),  mytrends.entries[i].link);
		}
	});
	$.getJSON('http://api.twitter.com/1/trends.json?count=5&callback=?', function (feed) {
		for(var i=0; feed != null && i < feed.trends.length && i < 5; i++)
			addTrend(feed.trends[i].name, feed.trends[i].url);
	});
}

function addTrend(name, url) {
	var trend = '<li><a href="' + url + '" target="_blank">' + name + '</a></li>';
	$('#trends-list').append(trend);
}