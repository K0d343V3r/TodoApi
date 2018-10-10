using Microsoft.AspNetCore.Mvc;
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
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoRepositoryContext _context;

        public TodoItemsController(ITodoRepositoryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoListItem>>> GetAllItemsAsync()
        {
            return await _context.TodoItems.GetAsync();
        }

        [HttpGet("{id}", Name = "GetItem")]
        public async Task<ActionResult<TodoListItem>> GetItemAsync(long id)
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
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateItemAsync([FromBody] TodoListItem item)
        {
            // sort items based on requested position
            var items = await _context.TodoItems.GetAsync(t => t.TodoListId == item.TodoListId);
            EntityHelper.AdjustListItemPosition(item, items.ToList<EntityBase>(), true);

            await _context.TodoItems.AddAsync(item);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetItem", new { id = item.Id }, item);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateItemAsync(long id, [FromBody] TodoListItem item)
        {
            var current = await _context.TodoItems.GetAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            // update item positions for todo list
            var items = await _context.TodoItems.GetAsync(t => t.TodoListId == item.TodoListId);
            EntityHelper.AdjustListItemPositions(item, items.ToList<EntityBase>(), current);
            EntityHelper.UpdateFrom(current, item);

            _context.TodoItems.Update(current);
            await _context.SaveChangesAsync();

            return Ok(current);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteItemAsync(long id)
        {
            var item = await _context.TodoItems.GetAsync(id);
            if (item == null)
            {
                return NotFound();
            }
            else
            {
                var items = await _context.TodoItems.GetAsync(t => t.TodoListId == item.TodoListId);
                EntityHelper.AdjustEntityPositions(items.ToList<EntityBase>(), item.Position, false);

                _context.TodoItems.Delete(item);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteItemsAsync([FromQuery(Name = "id")] List<long> ids)
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
                    var items = await _context.TodoItems.GetAsync(t => t.TodoListId == item.TodoListId);
                    EntityHelper.AdjustEntityPositions(items.ToList<EntityBase>(), item.Position, false);
                    _context.TodoItems.Delete(item);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}