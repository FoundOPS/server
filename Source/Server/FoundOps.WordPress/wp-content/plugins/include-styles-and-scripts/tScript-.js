$(document).ready(function(){
	
	var member  = getUrlVars()["id"];
	//Check if the url is a name.
	//If so, scroll to the user's section.
	if (member == 'apohl') {
		window.scrollTo(0, findPos(document.getElementById('andrewBg')));
	} else if (member == 'jperl') {
		window.scrollTo(0, findPos(document.getElementById('jonBg')));
	} else if (member == 'zbright') {
		window.scrollTo(0, findPos(document.getElementById('zachBg')));
	} else if (member == 'oshatken') {
		window.scrollTo(0, findPos(document.getElementById('orenBg')));
	}else if (member == 'jmahoney') {
		window.scrollTo(0, findPos(document.getElementById('johnBg')));
	}else if (member == 'cmcpherson') {
		window.scrollTo(0, findPos(document.getElementById('caitlinBg')));
	}
});

function getUrlVars()
{
	var vars = [], hash;
	var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
	for(var i = 0; i < hashes.length; i++)
	{
		hash = hashes[i].split('=');
		vars.push(hash[0]);
		vars[hash[0]] = hash[1];
	}
	return vars;
}

function findPos(obj) {
	var curtop = 0;
	if (obj.offsetParent) {
		do {
			curtop += obj.offsetTop;
		} while (obj = obj.offsetParent);
		return [curtop];
	}
}