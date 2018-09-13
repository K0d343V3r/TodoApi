using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoListsController(TodoContext context)
        {
            _context = context;

            //if (_context.TodoLists.Count() == 0)
            //{
            //    // Create a new TodoItem if collection is empty,
            //    // which means you can't delete all TodoItems.
            //    var todoList = new TodoList();
            //    todoList.Name = "List 1";
            //    todoList.Items.Add(new TodoListItem() { Name = "Item 1" });
            //    todoList.Items.Add(new TodoListItem() { Name = "Item 2" });
            //    _context.TodoLists.Add(todoList);
            //    _context.SaveChanges();
            //}
        }
        
        [HttpGet]
        public async Task<ActionResult<List<TodoList>>> GetAllAsync()
        {
            return await _context.TodoLists.Include(s => s.Items).ToListAsync();
        }

        [HttpGet("{id}", Name = "GetList")]
        public async Task<ActionResult<TodoList>> GetByIdAsync(long id)
        {
            var list = await _context.TodoLists
                .Include(s => s.Items)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (list == null)
            {
                return NotFound();
            }

            return list;
        }

        [HttpPost]
        [ProducesResponseType(201)]     // Created
        [ProducesResponseType(400)]     // BadRequest
        public async Task<IActionResult> CreateAsync([FromBody] TodoList list)
        {
            await _context.TodoLists.AddAsync(list);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetList", new { id = list.Id }, list);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] TodoList list)
        {
            var current = await _context.TodoLists.FindAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            var items = _context.TodoItems.Where(i => i.TodoListId == current.Id);
            foreach(var item in items)
            {
                current.Items.Remove(item);
            }

            current.Name = list.Name;
            current.Items.AddRange(list.Items);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var list = await _context.TodoLists.FindAsync(id);
            if (list == null)
            {
                return NotFound();
            }
            else
            {
                _context.TodoLists.Remove(list);
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}
