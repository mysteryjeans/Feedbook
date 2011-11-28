/*
 * Author: Faraz Masood Khan
 *
 * Perform basic functions on page like loading special tags. f:html
 *
 */

$(document).ready( function() {
	loadWebPart();
});

function loadWebPart() {
	$('div').each( function() {
		var source = $(this).attr('src');
		if(source != undefined && source != '')
			loadSource(this, source);
	});
}

function loadSource(target, source) {
	$.get(source, function(data) {
		$(target).html(data);
	});
}
