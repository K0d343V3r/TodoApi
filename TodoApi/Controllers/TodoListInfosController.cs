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
    public class TodoListInfosController : ControllerBase
    {
        private readonly ITodoRepositoryContext _context;

        public TodoListInfosController(ITodoRepositoryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoListInfo>>> GetAllListInfosAsync()
        {
            return EntityHelper.ToListInfos(await _context.TodoLists.GetAsync());
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoListInfo), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateListInfoAsync(long id, [FromBody] TodoListInfo info)
        {
            var current = await _context.TodoLists.GetAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            // adjust list positions based on update request
            var lists = await _context.TodoLists.GetAsync();
            EntityHelper.AdjustListInfoPositions(info, lists.ToList<IEntityBase>(), current);
            EntityHelper.UpdateFrom(current, info);

            _context.TodoLists.Update(current);
            await _context.SaveChangesAsync();

            return Ok(current);
        }
    }
}
