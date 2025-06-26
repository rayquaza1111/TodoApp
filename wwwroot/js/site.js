// Write your JavaScript code. 

// SignalR client setup for Todo notifications
// Make sure to include the SignalR client library in your HTML (e.g., via CDN or LibMan)

// Create a connection to the SignalR hub
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/todoHub")
    .configureLogging(signalR.LogLevel.Information)
    .build();

// Receive notification when a todo is created
connection.on("TodoCreated", function (todo) {
    console.log("SignalR: TodoCreated notification received", todo);
    alert("A new todo was created (RAYMOND TESTING): " + todo.title);
    // Optionally, reload the todo list or add the new todo to the UI
    loadTodos();
});

// Receive notification when a todo is updated
connection.on("TodoUpdated", function (todo) {
    console.log("SignalR: TodoUpdated notification received", todo);
    alert("A todo was updated: " + todo.title);
    // Update the todo in the UI in real time
    // Option 1: Reload the todo list (simple, always up-to-date)
    loadTodos();
    // Option 2: Update only the changed todo (advanced, not implemented here)
});

// Receive notification when a todo is deleted
connection.on("TodoDeleted", function (id) {
    console.log("SignalR: TodoDeleted notification received for id", id);
    alert("A todo was deleted. ID: " + id);
    // Remove the todo from the UI in real time
    const todoItem = document.getElementById('todo-' + id);
    if (todoItem) {
        todoItem.remove();
    }
    // Optionally, reload the todo list to ensure consistency
    // loadTodos();
});

// Start the SignalR connection
connection.start()
    .then(function () {
        console.log("SignalR connection established and running.");
    })
    .catch(function (err) {
        return console.error(err.toString());
    }); 