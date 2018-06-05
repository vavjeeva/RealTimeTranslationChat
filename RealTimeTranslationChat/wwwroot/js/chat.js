const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chatHub")
    .build();

$(function () {
    var isLoggedin = false;
    if ($('#hiddenId').val())
        isLoggedin = true;
    setScreen(isLoggedin);
});

function setScreen(isLogin) {

    if (!isLogin) {

        $("#divChat").hide();
        $("#divLogin").show();
    }
    else {

        $("#divChat").show();
        $("#divLogin").hide();
    }

}

connection.on("onNewUserConnected", (user) => {
    const msg = user + " joined chat."
    $("#messagesList").append($("<li class='list-group-item'>").html(msg));
    $("#usersList").append($("<li class='list-group-item list-group-item-info'>").html("<span class='glyphicon glyphicon-user'></span>&nbsp;" + user));
});

connection.on("onConnected", (users, name, id) => {

    for (i = 0; i < users.length; ++i) {
        $("#usersList").append($("<li class='list-group-item list-group-item-info'>").html("<span class='glyphicon glyphicon-user'></span>&nbsp;" + users[i].name));
    }

    $('#hiddenId').val(id);
    $('#hiddenUserName').val(name);
    $('#spanUserName').html(name);
});

connection.on("onDisconnected", (name) => {    
    $("#usersList li:contains('" + name + "')").remove();
});

$("#btnLogin").click(function () {

    connection.start().then(function () {
        connection.invoke("Connect", $("#name").val(), "en").catch(err => console.error(err.toString()));
        setScreen(true);
    });
});

$("#btnExitChat").click(function () {

    connection.invoke("Disconnect").catch(err => console.error(err.toString()));
    connection.stop()
    $('#hiddenId').val('');
    $('#hiddenUserName').val('');
    $('#spanUserName').html('');
    $("#usersList").empty();
    $("#messagesList").empty();
    setScreen(false);
});

document.getElementById("btnChat").addEventListener("click", event => {
    const user = document.getElementById("name").innerText;
    const message = document.getElementById("inputMsg").value;
    connection.invoke("SendMessage", user, message).catch(err => console.error(err.toString()));
    event.preventDefault();
});
