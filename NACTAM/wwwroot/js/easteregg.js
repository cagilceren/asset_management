// author: Tuan Bui
let activatedEasterEgg = false;
let geckoMousePos = {x: 0, y: 0};
let geckoPos = {x: 10, y: 10};

let timer;

document.querySelectorAll("img.logo-small").forEach(el => {
	el.addEventListener("mouseenter", function(ev) {
		timer = setTimeout(() => {
			if (!activatedEasterEgg) {
				el.style.position = "absolute";
				el.src = "/images/logoWithoutGecko.svg";
				window.addEventListener("mousemove", function(ev2) {
					el.style.top = "" + ev2.y + "px";
					el.style.left = "" + ev2.x + "px";
					geckoMousePos.x = ev2.x;
					geckoMousePos.y = ev2.y;
				});
				el.style.transform = "translate(-50%, -50%)";
				el.style.cursor = "none";
				el.style.zIndex = "11";
				el.style.display = "fixed";
				el.style.pointerEvents = "none";
				init();
				anim_container.style.display = "block";
				anim_container.style.pointerEvents = "none";
				anim_container.style.cursor = "none";
				canvas.style.pointerEvents = "none";
				canvas.style.cursor = "none";
				document.body.style.cursor = "none";
				setTimeout(displayEasterEgg, 50)
				moveTowards();
			}
			activatedEasterEgg = true;
		}, 5000);
	});
	el.addEventListener("mouseleave", () => clearTimeout(timer));
})

let walkingSpeed = 15;
let stoppedGecko = false;

function moveTowards(){
	anim_container.style.top = "" + geckoPos.y + "px";
	anim_container.style.left = "" + geckoPos.x + "px";
	let dx = geckoMousePos.x - geckoPos.x;
	let dy = geckoMousePos.y - geckoPos.y;
	let len = Math.sqrt((dx * dx) + (dy * dy));
	if (len > 30.0) {
		geckoPos.x += dx / len * walkingSpeed;
		geckoPos.y += dy / len * walkingSpeed;
		stopRecursive();
	} else if (stoppedGecko)
		resumeRecursive();
	let angle = Math.atan2(dy, dx);
	anim_container.style.transform = `translate(-50%, -50%) scale(0.3) rotate(${angle}rad)`;
	requestAnimationFrame(moveTowards);
}

function displayRecursive(){
	stage.seek(11);
	stage.play();
	if (!stoppedGecko)
		setTimeout(displayRecursive, 420);
}

function stopRecursive(){
	stoppedGecko = true;
}

function resumeRecursive(){
	stoppedGecko = false;
	setTimeout(displayRecursive, 420);
}


function displayEasterEgg(){
	stage.seek(0);
	stage.play();
	setTimeout(displayRecursive, 700)
}



