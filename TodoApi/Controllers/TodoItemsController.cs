using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public async Task<ActionResult<List<TodoListItem>>> GetAllItemsAsync()
        {
            return await _context.TodoItems.ToListAsync();
        }

        [HttpGet("{id}", Name = "GetItem")]
        public async Task<ActionResult<TodoListItem>> GetItemAsync(long id)
        {
            var item = await _context.TodoItems.FirstOrDefaultAsync(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TodoListItem), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateItemAsync([FromBody] TodoListItem item)
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
        [ProducesResponseType(typeof(TodoListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateItemAsync(long id, [FromBody] TodoListItem item)
        {
            var current = await _context.TodoItems.FindAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            // update item entity
            current.UpdateFrom(item);

            _context.Update(current);
            await _context.SaveChangesAsync();
            return Ok(current);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemAsync(long id)
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

        [HttpDelete]
        public async Task<IActionResult> DeleteItemsAsync([FromQuery(Name = "id")] List<long> ids)
        {
            var items = new List<TodoListItem>();
            foreach (var id in ids)
            {
                var list = await _context.TodoItems.FindAsync(id);
                if (list == null)
                {
                    return NotFound();
                }
                else
                {
                    items.Add(list);
                }
            }

            _context.TodoItems.RemoveRange(items);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}