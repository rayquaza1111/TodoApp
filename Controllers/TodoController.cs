using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApp.Data;
using TodoApp.Models;
using Microsoft.AspNetCore.SignalR;
using TodoApp;

namespace TodoApp.Controllers
{
    /// <summary>
    /// Controller for managing Todo items
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TodoController> _logger;
        private readonly IHubContext<TodoHub> _hubContext;

        /// <summary>
        /// Initializes a new instance of the TodoController
        /// </summary>
        public TodoController(ApplicationDbContext context, ILogger<TodoController> logger, IHubContext<TodoHub> hubContext)
        {
            _context = context;
            _logger = logger;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Gets all todo items
        /// </summary>
        /// <returns>A list of todo items</returns>
        /// <response code="200">Returns the list of todo items</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Todo>>> GetTodos()
        {
            _logger.LogInformation("Getting all todos");
            return await _context.Todos.ToListAsync();
        }

        /// <summary>
        /// Gets a specific todo item by id
        /// </summary>
        /// <param name="id">The ID of the todo item</param>
        /// <returns>The todo item</returns>
        /// <response code="200">Returns the todo item</response>
        /// <response code="404">If the todo item is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Todo>> GetTodo(int id)
        {
            _logger.LogInformation("Getting todo with id: {Id}", id);
            var todo = await _context.Todos.FindAsync(id);

            if (todo == null)
            {
                _logger.LogWarning("Todo with id: {Id} not found", id);
                return NotFound();
            }

            return todo;
        }

        /// <summary>
        /// Creates a new todo item
        /// </summary>
        /// <param name="todo">The todo item to create</param>
        /// <returns>The created todo item</returns>
        /// <response code="201">Returns the newly created todo item</response>
        /// <response code="400">If the todo item is invalid</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Todo>> CreateTodo(Todo todo)
        {
            _logger.LogInformation("Creating new todo");
            todo.CreatedAt = DateTime.UtcNow;
            _context.Todos.Add(todo);
            await _context.SaveChangesAsync();

            // Notify clients via SignalR
            await _hubContext.Clients.All.SendAsync("TodoCreated", todo);
            _logger.LogInformation("SignalR: TodoCreated notification sent");

            return CreatedAtAction(nameof(GetTodo), new { id = todo.Id }, todo);
        }

        /// <summary>
        /// Updates a specific todo item
        /// </summary>
        /// <param name="id">The ID of the todo item to update</param>
        /// <param name="todo">The updated todo item</param>
        /// <returns>No content</returns>
        /// <response code="204">If the update was successful</response>
        /// <response code="400">If the ID in the URL doesn't match the ID in the todo item</response>
        /// <response code="404">If the todo item is not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTodo(int id, Todo todo)
        {
            if (id != todo.Id)
            {
                return BadRequest();
            }

            _logger.LogInformation("Updating todo with id: {Id}", id);
            _context.Entry(todo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(id))
                {
                    _logger.LogWarning("Todo with id: {Id} not found during update", id);
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // Notify clients via SignalR
            await _hubContext.Clients.All.SendAsync("TodoUpdated", todo);
            _logger.LogInformation("SignalR: TodoUpdated notification sent");

            return NoContent();
        }

        /// <summary>
        /// Deletes a specific todo item
        /// </summary>
        /// <param name="id">The ID of the todo item to delete</param>
        /// <returns>No content</returns>
        /// <response code="204">If the deletion was successful</response>
        /// <response code="404">If the todo item is not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTodo(int id)
        {
            _logger.LogInformation("Deleting todo with id: {Id}", id);
            var todo = await _context.Todos.FindAsync(id);
            if (todo == null)
            {
                _logger.LogWarning("Todo with id: {Id} not found during deletion", id);
                return NotFound();
            }

            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();

            // Notify clients via SignalR
            await _hubContext.Clients.All.SendAsync("TodoDeleted", id);
            _logger.LogInformation("SignalR: TodoDeleted notification sent");

            return NoContent();
        }

        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.Id == id);
        }
    }
} 