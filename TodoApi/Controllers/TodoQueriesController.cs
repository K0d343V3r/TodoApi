using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TodoApi.Helpers;
using TodoApi.Models;
using TodoApi.Repository;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoQueriesController : ControllerBase
    {
        private readonly ITodoRepositoryContext _context;
        private readonly QueryHelper _queryHelper;

        public TodoQueriesController(ITodoRepositoryContext context)
        {
            _context = context;
            _queryHelper = new QueryHelper(context);
        }

        [HttpGet]
        [ProducesResponseType(typeof(List<TodoQuery>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<TodoQuery>>> GetAllQueriesAsync()
        {
            return await _context.TodoQueries.GetAsync();
        }

        [HttpGet("{id}", Name = "GetQuery")]
        [ProducesResponseType(typeof(TodoQuery), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoQuery>> GetQueryAsync(int id)
        {
            var query = await _context.TodoQueries.GetAsync(id);
            if (query == null)
            {
                return NotFound();
            }

            return query;
        }

        [HttpGet("{id}/results")]
        [ProducesResponseType(typeof(TodoQueryResults), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoQueryResults>> ExecuteQueryAsync(int id)
        {
            var query = await _context.TodoQueries.GetAsync(id);
            if (query == null)
            {
                return NotFound();
            }

            var references = await _queryHelper.ExecuteQueryAsync(query);

            return new TodoQueryResults() { TodoQueryId = id, References = references };
        }

        [HttpPost]
        [ProducesResponseType(typeof(TodoQuery), (int)HttpStatusCode.Created)]
        public async Task<ActionResult<TodoQuery>> CreateQueryAsync([FromBody] TodoQuery query)
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
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoQuery>> UpdateQueryAsync(int id, [FromBody] TodoQuery query)
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

            return current;
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<int>> DeleteQueryAsync(int id)
        {
            var query = await _context.TodoQueries.GetAsync(id);
            if (query == null)
            {
                return NotFound();
            }
            else
            {
                var queries = await _context.TodoQueries.GetAsync();
                await DeleteQuery(query, queries);
                await _context.SaveChangesAsync();

                return id;
            }
        }

        private async Task DeleteQuery(TodoQuery query, IList<TodoQuery> queries)
        {
            // adjust query positions
            EntityHelper.AdjustEntityPositions(queries.ToList<EntityBase>(), query.Position, false);

            // delete last results
            var results = await _context.TodoReferences.GetAsync(r => r.TodoQueryId == query.Id);
            results.ForEach(r => _context.TodoReferences.Delete(r));

            // and query
            _context.TodoQueries.Delete(query);
        }

        [HttpDelete]
        [ProducesResponseType(typeof(List<int>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<int>>> DeleteQueriesAsync([FromQuery(Name = "id")] List<int> ids)
        {
            var queries = await _context.TodoQueries.GetAsync();
            foreach (var id in ids)
            {
                var query = queries.FirstOrDefault(t => t.Id == id);
                if (query == null)
                {
                    return NotFound();
                }
                else
                {
                    await DeleteQuery(query, queries);
                }
            }

            await _context.SaveChangesAsync();
            return ids;
        }
    }
}