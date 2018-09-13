using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        [HttpGet]
        public async Task<ActionResult<List<TodoListItem>>> GetAllAsync()
        {
            return await _context.TodoItems.ToListAsync();
        }

        [HttpGet("{id}", Name = "GetItem")]
        public async Task<ActionResult<TodoListItem>> GetByIdAsync(long id)
        {
            var item = await _context.TodoItems.FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        [HttpPost]
        [ProducesResponseType(201)]     // Created
        [ProducesResponseType(400)]     // BadRequest
        public async Task<IActionResult> CreateAsync([FromBody] TodoListItem item)
        {
            var list = await _context.TodoLists.FindAsync(item.TodoListId);
            if (list == null)
            {
                return NotFound();
            }
            else
            { 
                await _context.TodoItems.AddAsync(item);
                await _context.SaveChangesAsync();
                return CreatedAtRoute("GetItem", new { id = item.Id }, item);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAsync(long id, [FromBody] TodoListItem item)
        {
            var current = await _context.TodoItems.FindAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            current.Name = item.Name;
            current.IsComplete = item.IsComplete;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(long id)
        {
            var item = await _context.TodoItems.FindAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                _context.TodoItems.Remove(item);
                await _context.SaveChangesAsync();
                return NoContent();
            }
        }
    }
}