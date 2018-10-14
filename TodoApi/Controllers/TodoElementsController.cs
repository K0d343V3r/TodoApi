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
    public class TodoElementsController : ControllerBase
    {
        private readonly ITodoRepositoryContext _context;

        public TodoElementsController(ITodoRepositoryContext context)
        {
            _context = context;
        }

        [HttpGet("lists")]
        public async Task<ActionResult<List<TodoElement>>> GetAllListElementsAsync()
        {
            return EntityHelper.ToListElements(await _context.TodoLists.GetAsync(s => s.Items));
        }

        [HttpGet("lists/{id}", Name = "GetListElement")]
        public async Task<ActionResult<TodoElement>> GetListElementAsync(int id)
        {
            var list = await _context.TodoLists.GetAsync(id, s => s.Items);
            if (list == null)
            {
                return NotFound();
            }

            return EntityHelper.ToListElement(list);
        }

        [HttpPut("lists/{id}")]
        [ProducesResponseType(typeof(TodoElement), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateListElementAsync(int id, [FromBody] TodoElement element)
        {
            var current = await _context.TodoLists.GetAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            // adjust list positions based on update request
            var lists = await _context.TodoLists.GetAsync();
            EntityHelper.AdjustListElementPositions(element, lists.ToList<EntityBase>(), current);
            EntityHelper.UpdateFrom(current, element);

            _context.TodoLists.Update(current);
            await _context.SaveChangesAsync();

            return Ok(EntityHelper.ToListElement(current));
        }
    }
}
