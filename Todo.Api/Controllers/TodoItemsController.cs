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
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoRepositoryContext _context;

        public TodoItemsController(ITodoRepositoryContext context)
        {
            _context = context;
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TodoListItem>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<TodoListItem>>> GetAllItemsAsync()
        {
            return await _context.TodoItems.GetAsync();
        }

        [HttpGet("{id}", Name = "GetItem")]
        [ProducesResponseType(typeof(TodoListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoListItem>> GetItemAsync(int id)
        {
            var item = await _context.TodoItems.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TodoListItem), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<TodoListItem>> CreateItemAsync([FromBody] TodoListItem item)
        {
            // sort items based on requested position
            var items = await _context.TodoItems.GetAsync(t => t.TodoListId == item.TodoListId);
            PositionAdjustor.AdjustForCreate(item, items.ToList<ISortable>());

            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetItem", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoListItem>> UpdateItemAsync(int id, [FromBody] TodoListItem item)
        {
            var current = await _context.TodoItems.GetAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            // update item positions for todo list
            var items = await _context.TodoItems.GetAsync(t => t.TodoListId == item.TodoListId);
            PositionAdjustor.AdjustForUpdate(item, items.ToList<ISortable>(), current);
            current.UpdateFrom(item);

            _context.TodoItems.Update(current);
            await _context.SaveChangesAsync();

            return current;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<int>> DeleteItemAsync(int id)
        {
            var item = await _context.TodoItems.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                await DeleteItem(item);
                await _context.SaveChangesAsync();

                return id;
            }
        }

        private async Task DeleteItem(TodoListItem item)
        {
            var items = await _context.TodoItems.GetAsync(t => t.TodoListId == item.TodoListId);
            PositionAdjustor.AdjustForDelete(item, items.ToList<ISortable>());

            var references = await _context.TodoReferences.GetAsync(r => r.Item.Id == item.Id);
            foreach (var reference in references)
            {
                var group = await _context.TodoReferences.GetAsync(r => r.TodoQueryId == reference.TodoQueryId);
                PositionAdjustor.AdjustForDelete(reference, references.ToList<ISortable>());

                _context.TodoReferences.Delete(reference);
            }

            _context.TodoItems.Delete(item);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(List<int>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<int>>> DeleteItemsAsync([FromQuery(Name = "id")] List<int> ids)
        {
            foreach (var id in ids)
            {
                var item = await _context.TodoItems.GetAsync(id);
                if (item == null)
                {
                    return NotFound();
                }
                else
                {
                    await DeleteItem(item);
                }
            }

            await _context.SaveChangesAsync();
            return ids;
        }
    }
}