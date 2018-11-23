using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Todo.Api.Helpers;
using Todo.Api.Models;
using Api.Common.Repository;
using Api.Common;

namespace Todo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListsController : ControllerBase
    {
        private readonly ITodoRepositoryContext _context;

        public TodoListsController(ITodoRepositoryContext context)
        {
            _context = context;

            Task.Run(() => DefaultList.CreateAsync(context)).Wait();
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TodoList>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<TodoList>>> GetAllListsAsync()
        {
            List<TodoList> lists = await _context.TodoLists.GetAsync(l => l.Id != DefaultList.id, l => l.Items);
            lists.ForEach(l => l.Items = l.Items.OrderBy(i => i.Position).ToList());

            return lists;
        }

        [HttpGet("{id}", Name = "GetList")]
        [ProducesResponseType(typeof(TodoList), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoList>> GetListAsync(int id)
        {
            var list = await FetchTodoListAsync(id);
            if (list == null)
            {
                return NotFound();
            }

            return list;
        }

        private async Task<TodoList> FetchTodoListAsync(int id)
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
        public async Task<ActionResult<TodoList>> CreateListAsync([FromBody] TodoList list)
        {
            var lists = await _context.TodoLists.GetAsync(l => l.Id != DefaultList.id);
            PositionAdjuster.AdjustForCreate(list, lists.ToList<ISortable>(), list.Items.ToList<ISortable>());

            await _context.TodoLists.AddAsync(list);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetList", new { id = list.Id }, list);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoList), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoList>> UpdateListAsync(int id, [FromBody] TodoList list)
        {
            var current = await FetchTodoListAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            if (id != DefaultList.id)
            {
                // if default list being updated, no need to mess with list ordering
                var lists = await _context.TodoLists.GetAsync(l => l.Id != DefaultList.id);
                PositionAdjuster.AdjustForUpdate(list, lists.ToList<ISortable>(), current, list.Items.ToList<ISortable>());
            }

            current.UpdateFrom(list);
            _context.TodoLists.Update(current);
            await _context.SaveChangesAsync();

            return current;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<int>> DeleteListAsync(int id)
        {
            if (id == DefaultList.id)
            {
                // we do not delete the default list
                return BadRequest("Cannot delete default list");
            }
            else
            {
                var list = await _context.TodoLists.GetAsync(id);
                if (list == null)
                {
                    return NotFound();
                }
                else
                {
                    var lists = await _context.TodoLists.GetAsync(l => l.Id != DefaultList.id);
                    PositionAdjuster.AdjustForDelete(list, lists.ToList<ISortable>());

                    _context.TodoLists.Delete(list);
                    await _context.SaveChangesAsync();

                    return id;
                }
            }
        }

        [HttpDelete]
        [ProducesResponseType(typeof(List<int>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<List<int>>> DeleteListsAsync([FromQuery(Name ="id")] List<int> ids)
        {
            var lists = await _context.TodoLists.GetAsync(l => l.Id != DefaultList.id);
            foreach (var id in ids)
            {
                if (id == DefaultList.id)
                {
                    return BadRequest("Cannot delete default list.");
                }
                else
                {
                    var list = lists.FirstOrDefault(t => t.Id == id);
                    if (list == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        PositionAdjuster.AdjustForDelete(list, lists.ToList<ISortable>());
                        _context.TodoLists.Delete(list);
                    }
                }
            }

            await _context.SaveChangesAsync();

            return ids;
        }
    }
}
