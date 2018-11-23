using Microsoft.AspNetCore.Mvc;
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
    public class TodoReferencesController : ControllerBase
    {
        private readonly ITodoRepositoryContext _context;

        public TodoReferencesController(ITodoRepositoryContext context)
        {
            _context = context;
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoItemReference), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoItemReference>> MoveReferenceAsync(int id, [FromBody] TodoItemReference reference)
        {
            var current = await _context.TodoReferences.GetAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            // re-arrange other references affected by move
            var references = await _context.TodoReferences.GetAsync(r => r.TodoQueryId == reference.TodoQueryId);
            PositionAdjustor.AdjustForUpdate(reference, references.ToList<ISortable>(), current);

            // move reference to desired location
            current.UpdateFrom(reference);

            _context.TodoReferences.Update(current);
            await _context.SaveChangesAsync();

            return current;
        }
    }
}
