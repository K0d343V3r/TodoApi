using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
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

            if (_context.TodoLists.Count() == 0)
            {
                var todoList = new TodoList
                {
                    Name = "List 1"
                };
                todoList.Items.Add(new TodoListItem() { Task = "Item 1", Position = 0 });
                todoList.Items.Add(new TodoListItem() { Task = "Item 2", Position = 1 });
                _context.TodoLists.Add(todoList);
                _context.SaveChanges();
            }
        }
        
        [HttpGet]
        public async Task<ActionResult<List<TodoList>>> GetAllListsAsync()
        { 
            List<TodoList> lists = await _context.TodoLists
                .Include(s => s.Items)
                .OrderBy(s => s.Position)
                .ToListAsync();
            lists.ForEach(m => m.Items = m.Items.OrderBy(o => o.Position).ToList());
            return lists;
        }

        [HttpGet("{id}", Name = "GetList")]
        public async Task<ActionResult<TodoList>> GetListAsync(long id)
        {
            var list = await FetchTodoListAsync(id);
            if (list == null)
            {
                return NotFound();
            }

            return list;
        }

        private async Task<TodoList> FetchTodoListAsync(long id)
        {
            TodoList list = await _context.TodoLists
                .Include(s => s.Items)
                .FirstOrDefaultAsync(m => m.Id == id);
            list.Items = list.Items.OrderBy(o => o.Position).ToList();
            return list;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TodoList), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateListAsync([FromBody] TodoList list)
        {
            List<TodoList> lists = await _context.TodoLists.ToListAsync();
            if (list.Position > lists.Count)
            {
                return BadRequest();
            }
            else
            {
                AdjustPositions(lists, list.Position);
                await _context.TodoLists.AddAsync(list);

                await _context.SaveChangesAsync();
                return CreatedAtRoute("GetList", new { id = list.Id }, list);
            }
        }

        private void AdjustPositions(IList<TodoList> lists, int position)
        {
            if (position < lists.Count)
            {
                foreach (var list in lists)
                {
                    if (list.Position >= position)
                    {
                        list.Position = list.Position + 1;
                    }
                }
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoList), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateListAsync(long id, [FromBody] TodoList list)
        {
            List<TodoList> lists = await _context.TodoLists.ToListAsync();
            if (list.Position > lists.Count)
            {
                return BadRequest();
            }
            else
            {
                var current = await FetchTodoListAsync(id);
                if (current == null)
                {
                    return NotFound();
                }

                // update list entity
                AdjustPositions(lists, list.Position);
                current.UpdateFrom(list);

                _context.Update(current);
                await _context.SaveChangesAsync();
                return Ok(current);
            }
        }

        //[HttpPatch("{id}")]
        //[ProducesResponseType(typeof(TodoList), (int)HttpStatusCode.OK)]
        //[ProducesResponseType((int)HttpStatusCode.BadRequest)]
        //public async Task<IActionResult> PatchListAsync(long id, [FromBody] JsonPatchDocument<TodoList> patch)
        //{
        //    var current = await _context.TodoLists
        //        .Include(s => s.Items)
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (current == null)
        //    {
        //        return NotFound();
        //    }

        //    patch.ApplyTo(current);

        //    _context.Update(current);
        //    await _context.SaveChangesAsync();
        //    return Ok(current);
        //}

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
