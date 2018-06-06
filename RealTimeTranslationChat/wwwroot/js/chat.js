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

$("#inputMsg").keyup(function (event) {
    if (event.keyCode === 13) {
        $("#btnChat").click();
    }
});

connection.on("onNewUserConnected", (user) => {
    const msg = user + " joined chat."
    $("#messagesList").append($("<li class='list-group-item list-group-item-success'>").html(msg));
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
    const msg = name + " left chat."
    $("#messagesList").append($("<li class='list-group-item list-group-item-danger'>").html(msg));
});

connection.on("ReceiveMessage", (name, msg) => {
    $("#messagesList").append($("<li class='list-group-item list-group-item-info'>").html(name + ":" + msg));
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

$("#btnChat").click(function () {
    var msg = $("#inputMsg").val();
    if (msg.length > 0) {
        connection.invoke("SendMessage", $("#hiddenUserName").val(), msg).catch(err => console.error(err.toString()));
        $("#inputMsg").val('');
        $("#messagesList").append($("<li class='list-group-item list-group-item-warning'>").html($("#hiddenUserName").val() + ":" + msg));
    }
});

$("#btnLogin").click(function () {

    connection.start().then(function () {
        connection.invoke("Connect", $("#name").val(), $("#slLanguage").val()).catch(err => console.error(err.toString()));
        setScreen(true);
    });
});
