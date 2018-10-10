﻿using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TodoApi.Models;
using TodoApi.Repository;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly ITodoRepositoryContext _context;

        public TodoListsController(ITodoRepositoryContext context)
        {
            _context = context;

            //if (_context.TodoLists.Count() == 0)
            //{
            //    var todoList = new TodoList
            //    {
            //        Name = "List 1"
            //    };
            //    todoList.Items.Add(new TodoListItem() { Task = "Item 1", Position = 0 });
            //    todoList.Items.Add(new TodoListItem() { Task = "Item 2", Position = 1 });
            //    _context.TodoLists.Add(todoList);
            //    _context.SaveChanges();
            //}
        }
        
        [HttpGet]
        public async Task<ActionResult<List<TodoList>>> GetAllListsAsync()
        {
            List<TodoList> lists = await _context.TodoLists.GetAsync(s => s.Items);
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
            TodoList list = await _context.TodoLists.GetAsync(id, s => s.Items);
            if (list != null)
            {
                list.Items = list.Items.OrderBy(o => o.Position).ToList();
            }
            return list;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TodoList), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateListAsync([FromBody] TodoList list)
        {
            var lists = await _context.TodoLists.GetAsync();
            EntityHelper.AdjustListPosition(list, lists.ToList<EntityBase>(), true);

            await _context.TodoLists.AddAsync(list);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetList", new { id = list.Id }, list);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoList), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateListAsync(long id, [FromBody] TodoList list)
        {
            var current = await FetchTodoListAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            var lists = await _context.TodoLists.GetAsync();
            EntityHelper.AdjustListPositions(list, lists.ToList<EntityBase>(), current);
            EntityHelper.UpdateFrom(current, list);

            _context.TodoLists.Update(current);
            await _context.SaveChangesAsync();
            return Ok(current);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteListAsync(long id)
        {
            var list = await _context.TodoLists.GetAsync(id);
            if (list == null)
            {
                return NotFound();
            }
            else
            {
                var lists = await _context.TodoLists.GetAsync();
                EntityHelper.AdjustEntityPositions(lists.ToList<EntityBase>(), list.Position, false);

                _context.TodoLists.Delete(list);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteListsAsync([FromQuery(Name ="id")] List<long> ids)
        {
            var lists = await _context.TodoLists.GetAsync();
            foreach (var id in ids)
            {
                var list = lists.FirstOrDefault(t => t.Id == id);
                if (list == null)
                {
                    return NotFound();
                }
                else
                {
                    EntityHelper.AdjustEntityPositions(lists.ToList<EntityBase>(), list.Position, false);
                    _context.TodoLists.Delete(list);
                }
            }

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
