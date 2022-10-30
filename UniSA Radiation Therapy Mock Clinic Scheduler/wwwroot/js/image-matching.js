function startDrag(e) {
	// determine event object
	if (!e) {
		var e = window.event;
	}
	console.log(e)
	// IE uses srcElement, others use target
	var targ = e.target ? e.target : e.srcElement;

	if (targ.className != 'draggable') { return };
	// calculate event X, Y coordinates
	offsetX = e.clientX;
	offsetY = e.clientY;

	// assign default values for top and left properties
	if (!targ.style.left) { targ.style.left = '0px' };
	if (!targ.style.top) { targ.style.top = '0px' };

	// calculate integer values for top and left 
	// properties
	coordX = parseInt(targ.style.left);
	coordY = parseInt(targ.style.top);
	drag = true;

	// move div element
	document.querySelector(".draggable").onmousemove = dragDiv;
	//document.querySelector("#draggable-image-2").onmousemove = dragDiv;

}

function dragDiv(e) {
	if (!drag) { return };
	if (!e) { var e = window.event };
	var targ = e.target ? e.target : e.srcElement;

	// move div element
	targ.style.left = coordX + e.clientX - offsetX + 'px';
	targ.style.top = coordY + e.clientY - offsetY + 'px';
	return false;
}

function stopDrag() {
	drag = false;
}

window.onload = function () {
	document.querySelector(".draggable").onmousedown = startDrag;
	document.querySelector(".draggable").onmouseup = stopDrag;

	//document.querySelector("#draggable-image-2").onmousedown = startDrag;
	//document.querySelector("#draggable-image-2").onmouseup = stopDrag;
}