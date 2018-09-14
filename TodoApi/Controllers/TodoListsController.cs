using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
        public async Task<ActionResult<List<TodoList>>> GetAllListsAsync()
        {
            return await _context.TodoLists.Include(s => s.Items).ToListAsync();
        }

        [HttpGet("{id}", Name = "GetList")]
        public async Task<ActionResult<TodoList>> GetListAsync(long id)
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
        [ProducesResponseType(typeof(TodoList), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateListAsync([FromBody] TodoList list)
        {
            await _context.TodoLists.AddAsync(list);
            await _context.SaveChangesAsync();
            return CreatedAtRoute("GetList", new { id = list.Id }, list);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoList), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateListAsync(long id, [FromBody] TodoList list)
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
            return Ok(current);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListAsync(long id)
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

        [HttpDelete]
        public async Task<IActionResult> DeleteListsAsync([FromQuery(Name ="id")] List<long> ids)
        {
            var lists = new List<TodoList>();
            foreach (var id in ids)
            {
                var list = await _context.TodoLists.FindAsync(id);
                if (list == null)
                {
                    return NotFound();
                }
                else
                {
                    lists.Add(list);
                }
            }

            _context.TodoLists.RemoveRange(lists);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
