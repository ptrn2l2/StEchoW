// ref: https://docs.microsoft.com/en-us/aspnet/core/tutorials/signalr?view=aspnetcore-5.0&tabs=visual-studio

(function () {
    "use strict";

    if(!"WebSocket" in window) {
        alert("Browser not supported");
        return;
    }

    var connection = new signalR.HubConnectionBuilder()
            .withUrl("/chatep", {
                // I only want WebSockets here
                transport: signalR.HttpTransportType.WebSockets
            }).build();

    //Disable send button until connection is established
    document.getElementById("sendButton").disabled = true;

    connection.on("ReceiveMessage", function (user, message) {
        var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
        var encodedMsg = user + " says " + msg;
        var li = document.createElement("li");
        li.textContent = encodedMsg;
        document.getElementById("messagesList").appendChild(li);
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
})();