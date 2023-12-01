// author: Tuan Bui
let loginScreen = document.querySelectorAll(".login-screen");
let forgotPasswordScreen = document.querySelectorAll(".forgot-password-screen");
let showLoginButton = document.querySelectorAll(".show-login-button");
let showForgotPassword = document.querySelectorAll(".show-forgot-password");

loginScreen.forEach(x => {
	x.addEventListener("click", function(ev) {
		if (ev.target === x) {
			x.style.top = "-100vh";
			x.style.backdropFilter = "blur(0px)";
		}
	})
});

forgotPasswordScreen.forEach(x => {
	x.addEventListener("click", function(ev) {
		if (ev.target === x) {
			x.style.top = "-100vh";
			x.style.backdropFilter = "blur(0px)";
		}
	})
});

showLoginButton.forEach(b => b.addEventListener("click", function(ev) {
	loginScreen.forEach(x => {
		x.style.top = "0";
		x.style.backdropFilter = "blur(5px)";
	});
}))

showForgotPassword.forEach(b => b.addEventListener("click", function(ev) {
	forgotPasswordScreen.forEach(x => {
		x.style.top = "0";
		x.style.backdropFilter = "blur(5px)";
	});
}))

window.addEventListener("keydown", function(ev) {
	if (ev.key == "Escape") {
		loginScreen.forEach(x => {
			x.style.top = "-100vh"
			x.style.backdropFilter = "blur(0px)";
		});
	}
})

let titleLetters = document.querySelectorAll(".title1 > span");
let subtitleLetters = document.querySelectorAll(".subtitle1> span");

titleLetters.forEach((v, k) => {
	v.addEventListener("mouseenter", ev => {
		v.style.color = "#007bff";
		subtitleLetters[k].style.color = "#007bff";
	});
	v.addEventListener("mouseleave", ev => {
		v.style.color = "#000";
		subtitleLetters[k].style.color = "#000";
	});
})
subtitleLetters.forEach((v, k) => {
	v.addEventListener("mouseenter", ev => {
		v.style.color = "#007bff";
		titleLetters[k].style.color = "#007bff";
	});
	v.addEventListener("mouseleave", ev => {
		v.style.color = "#000";
		titleLetters[k].style.color = "#000";
	});
})


const url = new URL(window.location.href); 
const loginParam = url.searchParams.get('login');

if (loginParam == "true") { // not === on purpose
	showLoginButton.forEach(x => x.click());
}
