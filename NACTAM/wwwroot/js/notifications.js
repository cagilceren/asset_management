// author: Tuan Bui

var connection = new signalR.HubConnectionBuilder().withUrl("/NotificationHub").build();

connection.on("ReceiveMessage", function (user, message) {
    // We can assign user-supplied strings to an element's textContent because it
    // is not interpreted as markup. If you're assigning in any other way, you 
    // should be aware of possible script injection concerns.
    console.log(`${user} says ${message}`);
});
connection.on("ReceiveNotify", function (notifications, number) {
	document.getElementById("raw-notifications").innerHTML = notifications;
	document.getElementById("notification-count").innerText = "" + number;
	setTimeout(addNotificationEventListeners, 50);
});
connection.on("ReceiveNotifyCenter", function (notifications) {
	let el = document.getElementById("notification-center");
	if (el !== undefined){
		el.innerHTML = notifications;
		setTimeout(addNotificationEventListeners, 50);
	}
});
connection.on("UpdateDarkmode", function (to) {
	changeDarkmode(to);
});



connection.start().then(function () {
	// sth
	connection.invoke("SendMessage", currentUserId, "test message").catch(function (err) {
        return console.error(err.toString());
    })
}).catch(function (err) {
    return console.error(err.toString());
});

function changeDarkmode(to){
	let isDark = document.body.classList.contains("dark");
	if (to === undefined){
		document.body.classList.toggle("dark");
		to = !isDark;
		connection.invoke("SetDarkMode", to).catch(function(err) {
			return console.error(err.toString());
		});
	} else if (to) {
		document.body.classList.add("dark");
	} else {
		document.body.classList.remove("dark");
	}
}

function addNotificationEventListeners(){
	document.querySelectorAll(".notificationtype-Unread").forEach(x => {
		x.addEventListener("mouseover", function(ev){
			let value = ev.target.id.substring(15).split("-");
			connection.invoke("Read", parseInt(value[0]), value[1]); // "notificationid-" is 15 ASCII characters long
		})
	})
}

addNotificationEventListeners()
