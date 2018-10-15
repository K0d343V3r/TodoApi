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
        [ProducesResponseType(typeof(List<TodoElement>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<TodoElement>>> GetAllListElementsAsync()
        {
            var lists = await _context.TodoLists.GetAsync(s => s.Items);
            return lists.Select(list => EntityHelper.ToElement(list, list.Items.Count)).ToList();
        }

        [HttpGet("queries")]
        [ProducesResponseType(typeof(List<TodoElement>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<TodoElement>>> GetAllQueryElementsAsync()
        {
            var queries = await _context.TodoQueries.GetAsync();
            // TODO: get childCount once query results are persisted
            var childCount = 0;
            return queries.Select(query => EntityHelper.ToElement(query, childCount)).ToList();
        }

        [HttpGet("lists/{id}", Name = "GetListElement")]
        [ProducesResponseType(typeof(TodoElement), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoElement>> GetListElementAsync(int id)
        {
            var list = await _context.TodoLists.GetAsync(id, s => s.Items);
            if (list == null)
            {
                return NotFound();
            }

            return EntityHelper.ToElement(list, list.Items.Count);
        }

        [HttpGet("queries/{id}", Name = "GetQueryElement")]
        [ProducesResponseType(typeof(TodoElement), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoElement>> GetQueryElementAsync(int id)
        {
            var query = await _context.TodoQueries.GetAsync(id);
            if (query == null)
            {
                return NotFound();
            }

            // TODO: get childCount once query results are persisted
            var childCount = 0;
            return EntityHelper.ToElement(query, childCount);
        }

        [HttpPut("lists/{id}")]
        [ProducesResponseType(typeof(TodoElement), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoElement>> UpdateListElementAsync(int id, [FromBody] TodoElement element)
        {
            var current = await _context.TodoLists.GetAsync(id, s => s.Items);
            if (current == null)
            {
                return NotFound();
            }

            // adjust list positions based on update request
            var lists = await _context.TodoLists.GetAsync();
            EntityHelper.AdjustEntityPositions(element, lists.ToList<EntityBase>(), current);
            EntityHelper.UpdateFrom(current, element);

            _context.TodoLists.Update(current);
            await _context.SaveChangesAsync();

            return EntityHelper.ToElement(current, current.Items.Count);
        }

        [HttpPut("queries/{id}")]
        [ProducesResponseType(typeof(TodoElement), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoElement>> UpdateQueryElementAsync(int id, [FromBody] TodoElement element)
        {
            var current = await _context.TodoQueries.GetAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            // adjust list positions based on update request
            var queries = await _context.TodoQueries.GetAsync();
            EntityHelper.AdjustEntityPositions(element, queries.ToList<EntityBase>(), current);
            EntityHelper.UpdateFrom(current, element);

            _context.TodoQueries.Update(current);
            await _context.SaveChangesAsync();

            // TODO: get childCount once query results are persisted
            var childCount = 0;
            return EntityHelper.ToElement(current, childCount);
        }
    }
}
