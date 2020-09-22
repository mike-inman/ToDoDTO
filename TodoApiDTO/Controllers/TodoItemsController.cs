using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoItemsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoItemsController(TodoContext context)
        {
            _context = context;
        }

        // GET: api/TodoItems
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTodoItems()
        {
            return await _context.TodoItems.ToListAsync();
        }

        // GET: api/TodoItems/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemInput>> GetTodoItem(long id)
        {
            var  td = await _context.TodoItems
                                  .Where(x => x.Id == id)
                                  .Select(x => new { x.Id, x.IsComplete, x.Name })
                                  .SingleAsync();

            if (td == null)
            {
                return NotFound();
            }

            return new TodoItemInput
            {
                Id = td.Id,
                Name = td.Name,
                IsComplete = td.IsComplete
            };            

        }

        // PUT: api/TodoItems/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTodoItem(long id, TodoItemInput todoItemInput)
        {
            if (id != todoItemInput.Id)
            {
                return BadRequest();
            }

            var db_todoItem = await _context.TodoItems.FindAsync(id);
            if (db_todoItem == null)
            {
                return NotFound();
            }

            var todoItem = InputModelToTodoItem(todoItemInput);
            todoItem.Secret = db_todoItem.Secret;

            _context.Entry(todoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoItemExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        [HttpPost]
        public async Task<ActionResult<TodoItem>> PostTodoItem( TodoItemInput todoItemInput)
        {
            var todoItem = InputModelToTodoItem(todoItemInput);

            _context.TodoItems.Add(todoItem);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetTodoItem), new { id = todoItem.Id }, todoItem);
        }

        private TodoItem InputModelToTodoItem(TodoItemInput todoItemInput)
        {
            return new TodoItem
            {
                IsComplete = todoItemInput.IsComplete,
                Name = todoItemInput.Name
            };
        }

        // DELETE: api/TodoItems/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoItem>> DeleteTodoItem(long id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null)
            {
                return NotFound();
            }

            _context.TodoItems.Remove(todoItem);
            await _context.SaveChangesAsync();

            return todoItem;
        }

        private bool TodoItemExists(long id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }

    }
}
