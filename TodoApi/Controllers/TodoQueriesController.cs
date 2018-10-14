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
    public class TodoQueriesController : ControllerBase
    {
        private readonly ITodoRepositoryContext _context;

        public TodoQueriesController(ITodoRepositoryContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoQuery>>> GetAllQueriesAsync()
        {
            return await _context.TodoQueries.GetAsync();
        }

        [HttpGet("{id}", Name = "GetQuery")]
        public async Task<ActionResult<TodoQuery>> GetQueryAsync(int id)
        {
            var query = await _context.TodoQueries.GetAsync(id);
            if (query == null)
            {
                return NotFound();
            }

            return query;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TodoQuery), (int)HttpStatusCode.Created)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> CreateQueryAsync([FromBody] TodoQuery query)
        {
            // sort items based on requested position
            var items = await _context.TodoQueries.GetAsync();
            EntityHelper.AdjustEntityPosition(query, items.ToList<EntityBase>(), true);

            await _context.TodoQueries.AddAsync(query);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetQuery", new { id = query.Id }, query);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoQuery), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateQueryAsync(int id, [FromBody] TodoQuery query)
        {
            var current = await _context.TodoQueries.GetAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            // update item positions for todo list
            var queries = await _context.TodoQueries.GetAsync();
            EntityHelper.AdjustEntityPositions(query, queries.ToList<EntityBase>(), current);
            EntityHelper.UpdateFrom(current, query);

            _context.TodoQueries.Update(current);
            await _context.SaveChangesAsync();

            return Ok(current);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQueryAsync(int id)
        {
            var query = await _context.TodoQueries.GetAsync(id);
            if (query == null)
            {
                return NotFound();
            }
            else
            {
                var queries = await _context.TodoQueries.GetAsync();
                EntityHelper.AdjustEntityPositions(queries.ToList<EntityBase>(), query.Position, false);

                _context.TodoQueries.Delete(query);
                await _context.SaveChangesAsync();

                return NoContent();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteQueriesAsync([FromQuery(Name = "id")] List<int> ids)
        {
            foreach (var id in ids)
            {
                var query = await _context.TodoQueries.GetAsync(id);
                if (query == null)
                {
                    return NotFound();
                }
                else
                {
                    var queries = await _context.TodoQueries.GetAsync();
                    EntityHelper.AdjustEntityPositions(queries.ToList<EntityBase>(), query.Position, false);
                    _context.TodoQueries.Delete(query);
                }
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}