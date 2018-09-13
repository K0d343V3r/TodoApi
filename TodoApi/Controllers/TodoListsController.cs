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
        public ActionResult<List<TodoList>> GetAll()
        {
            return _context.TodoLists.Include(s => s.Items).ToList();
        }

        [HttpGet("{id}", Name = "GetList")]
        public ActionResult<TodoList> GetById(long id)
        {
            var list = _context.TodoLists
                .Include(s => s.Items)
                .FirstOrDefault(m => m.Id == id);

            if (list == null)
            {
                return NotFound();
            }

            return list;
        }

        [HttpPost]
        [ProducesResponseType(201)]     // Created
        [ProducesResponseType(400)]     // BadRequest
        public IActionResult Create([FromBody] TodoList list)
        {
            _context.TodoLists.Add(list);
            _context.SaveChanges();
            return CreatedAtRoute("GetList", new { id = list.Id }, list);
        }

        [HttpPut("{id}")]
        public IActionResult Update(long id, [FromBody] TodoList list)
        {
            var current = _context.TodoLists.Find(id);
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

            _context.SaveChanges();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var list = _context.TodoLists.Find(id);
            if (list == null)
            {
                return NotFound();
            }
            else
            {
                _context.TodoLists.Remove(list);
                _context.SaveChanges();
                return NoContent();
            }
        }
    }
}
