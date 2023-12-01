// author: Tuan Bui
//

let deltaX = 0; // ev.movementX is not reliable
let deltaY = 0;

window.addEventListener("pointermove", ev => { [deltaX, deltaY] = [ev.movementX, ev.movementY] })

document.querySelectorAll('.logo').forEach(logo => {
	logo.addEventListener("mouseenter", ev => {
		let len = Math.sqrt((deltaX * deltaX) + (deltaY * deltaY));
		let normX = deltaX / len;
		let normY = deltaY / len;
		let f = "";
		let accX = 0;
		let accY = 0;
		for (let i = 1; i <= 7; i ++){
			accX += normX;
			accY += normY;

			f = f + ` drop-shadow(${~~accX}px ${~~accY}px 0 #888)`;
		}
		logo.style.filter = f + "invert(1) sepia(29%) saturate(7091%)  brightness(0.64) invert(1)  hue-rotate(-10deg)";
	})

	logo.addEventListener("mouseleave", ev => {
		logo.style.filter = "drop-shadow(0px 0px 0 #888) drop-shadow(0px 0px 0 #888) drop-shadow(0px 0px 0 #888) drop-shadow(0px 0px 0 #888) drop-shadow(0px 0px 0 #888) drop-shadow(0px 0px 0 #888) drop-shadow(0px 0px 0 #888) invert(1) sepia(29%) saturate(7091%)  brightness(0.7) invert(1)  hue-rotate(-10deg)";
	})
})

function readImage(input, updating) {
	if (input.files && input.files[0]) {
		var reader = new FileReader();

		reader.onload = function (e) {
			document.getElementById(updating).src = e.target.result;
		};

		reader.readAsDataURL(input.files[0]);
	}
}



