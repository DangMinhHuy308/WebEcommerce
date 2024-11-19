"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

//Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

// Giả sử bạn đã có currentUserId (ID người dùng hiện tại)
/*const currentUserId = document.getElementById("currentUserId").value;*/
/*fromUserId, fromUserName, messageContent*/
connection.on("ReceiveMessage", function (user,message) {
    var messagesList = document.getElementById("messagesList");
    var li = document.createElement("li");

    if (user === "You") {
        // Tin nhắn từ người dùng hiện tại (bên phải)
        li.className = "chat-right";
        li.innerHTML = `
            <div class="chat-hour">Just now <span class="fa fa-check-circle"></span></div>
            <div class="chat-text">${message}</div>
            <div class="chat-avatar">
                <img src="~/thumbnails/" alt="User Avatar">
                <div class="chat-name">You</div>
            </div>
        `;
    } else {
        // Tin nhắn từ người khác (bên trái)
        li.className = "chat-left";
        li.innerHTML = `
            <div class="chat-avatar">
                <img src="~/thumbnails/" alt="User Avatar">
                <div class="chat-name">${user}</div>
            </div>
            <div class="chat-text">${message}</div>
            <div class="chat-hour">Just now <span class="fa fa-check-circle"></span></div>
        `;
    }

    messagesList.appendChild(li);
    messagesList.scrollTop = messagesList.scrollHeight;
});


connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    var user = document.getElementById("userInput").value;
    var message = document.getElementById("messageInput").value;
    connection.invoke("SendMessage", user, message).catch(function (err) {
        return console.error(err.toString());
    });
    event.preventDefault();
});