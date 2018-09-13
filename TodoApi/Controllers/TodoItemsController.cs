using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public ActionResult<List<TodoListItem>> GetAll()
        {
            return _context.TodoItems.ToList();
        }

        [HttpGet("{id}", Name = "GetItem")]
        public ActionResult<TodoListItem> GetById(long id)
        {
            var item = _context.TodoItems.FirstOrDefault(m => m.Id == id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        [HttpPost]
        [ProducesResponseType(201)]     // Created
        [ProducesResponseType(400)]     // BadRequest
        public IActionResult Create([FromBody] TodoListItem item)
        {
            var list = _context.TodoLists.Find(item.TodoListId);
            if (list == null)
            {
                return NotFound();
            }
            else
            { 
                _context.TodoItems.Add(item);
                _context.SaveChanges();
                return CreatedAtRoute("GetItem", new { id = item.Id }, item);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoListItem item)
        {
            var current = _context.TodoItems.Find(id);
            if (current == null)
            {
                return NotFound();
            }

            current.Name = item.Name;
            current.IsComplete = item.IsComplete;

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var item = _context.TodoItems.Find(id);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                _context.TodoItems.Remove(item);
                _context.SaveChanges();
                return NoContent();
            }
        }
    }
}