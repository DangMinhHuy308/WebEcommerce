"use strict";

var currentUserId = '@Model.FromUserId'; // Lấy ID người dùng hiện tại từ ViewModel
var selectedUserId = '@Model.ToUserId'; // Lấy ID người dùng được chọn từ ViewModel

var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
document.getElementById("sendButton").disabled = true;

// Khởi động kết nối SignalR
connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
    if (!selectedUserId) {
        // Nếu `selectedUserId` chưa có, chọn người dùng đầu tiên từ danh sách
        var firstUser = document.querySelector('.people-list li');
        if (firstUser) {
            selectedUserId = firstUser.getAttribute('data-user-id');
            loadChatHistory(selectedUserId);
        }
    }
}).catch(function (err) {
    return console.error(err.toString());
});

// Nhận tin nhắn từ server và hiển thị tin nhắn liên quan đến người dùng được chọn
connection.on("ReceiveMessage", function (chatMessage) {
    if (chatMessage.fromUserId === selectedUserId || chatMessage.toUserId === selectedUserId) {
        var li = document.createElement("li");
        li.className = "clearfix " + (chatMessage.fromUserId === currentUserId ? "text-right" : "");
        li.innerHTML = `
            <div class="message-data ${chatMessage.fromUserId === currentUserId ? 'text-right' : ''}">
                <span class="message-data-time">${new Date(chatMessage.when).toLocaleTimeString()}</span>
                <img src="${chatMessage.fromUserImage}" alt="avatar" class="avatar">
            </div>
            <div class="message ${chatMessage.fromUserId === currentUserId ? 'my-message' : 'other-message'}">
                ${chatMessage.messageContent}
            </div>`;
        document.getElementById("messagesList").appendChild(li);
    }
});

// Gửi tin nhắn khi nhấn nút "Send"
document.getElementById("sendButton").addEventListener("click", function (event) {
    var messageContent = document.getElementById("messageInput").value.trim();
    if (messageContent) {
        connection.invoke("SendMessage", {
            fromUserId: currentUserId,
            toUserId: selectedUserId,
            messageContent: messageContent,
            when: new Date() // Thời gian gửi tin nhắn
        }).catch(function (err) {
            return console.error(err.toString());
        });
        document.getElementById("messageInput").value = ""; // Xóa ô nhập sau khi gửi
    }
    event.preventDefault();
});

// Gửi tin nhắn khi nhấn phím Enter
document.getElementById("messageInput").addEventListener("keypress", function (event) {
    if (event.key === "Enter") {
        document.getElementById("sendButton").click(); // Gọi sự kiện click của nút Send
    }
});

// Cập nhật `selectedUserId` khi chọn người dùng từ danh sách và tải lịch sử tin nhắn của người dùng đó
document.querySelectorAll('.people-list li').forEach(function (element) {
    element.addEventListener('click', function () {
        selectedUserId = element.getAttribute('data-user-id');
        document.getElementById("messagesList").innerHTML = ""; // Xóa tin nhắn cũ
        loadChatHistory(selectedUserId); // Tải lịch sử chat của người dùng mới
    });
});

// Hàm tải lịch sử tin nhắn khi chọn người dùng mới
function loadChatHistory(userId) {
    $.ajax({
        url: '/Admin/Chat/GetMessages', // API lấy tin nhắn trong ChatController
        method: 'GET',
        data: { userId: userId },
        success: function (data) {
            document.getElementById("messagesList").innerHTML = data.messagesHtml; // Chèn nội dung HTML trả về từ API vào khung tin nhắn
        },
        error: function (err) {
            console.error("Error loading chat history:", err);
        }
    });
}
