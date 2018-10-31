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
    public class TodoElementsController : ControllerBase
    {
        private readonly ITodoRepositoryContext _context;
        private readonly QueryHelper _queryHelper;

        public TodoElementsController(ITodoRepositoryContext context)
        {
            _context = context;
            _queryHelper = new QueryHelper(context);

            Task.Run(() => EntityHelper.CreateDefaultListAsync(context)).Wait();
        }

        [HttpGet("lists")]
        [ProducesResponseType(typeof(List<TodoElement>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<TodoElement>>> GetAllListElementsAsync()
        {
            var lists = await _context.TodoLists.GetAsync(l => l.Id != EntityHelper.defaultListId, l => l.Items);
            return lists.Select(list => EntityHelper.ToElement(list)).ToList();
        }

        [HttpGet("queries")]
        [ProducesResponseType(typeof(List<TodoQueryElement>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<List<TodoQueryElement>>> GetAllQueryElementsAsync()
        {
            var queries = await _context.TodoQueries.GetAsync(q => q.Predicates);

            var elements = new List<TodoQueryElement>();
            foreach (var query in queries)
            {
                // execute query to return up to date remaining counts
                var references = await _queryHelper.ExecuteQueryAsync(query);
                elements.Add(EntityHelper.ToElement(query, references.Count(r => !r.Item.Done)));
            }

            return elements;
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

            return EntityHelper.ToElement(list);
        }

        [HttpGet("queries/{id}", Name = "GetQueryElement")]
        [ProducesResponseType(typeof(TodoQueryElement), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoQueryElement>> GetQueryElementAsync(int id)
        {
            var query = await _context.TodoQueries.GetAsync(id, q => q.Predicates);
            if (query == null)
            {
                return NotFound();
            }

            // execute query to return up to date remaining counts
            var references = await _queryHelper.ExecuteQueryAsync(query);
            return EntityHelper.ToElement(query, references.Count(r => !r.Item.Done));
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

            if (id != EntityHelper.defaultListId)
            {
                // if default list is being updated, no need to adjust other lists positions
                var lists = await _context.TodoLists.GetAsync(l => l.Id != EntityHelper.defaultListId);
                EntityHelper.AdjustEntityPositions(element, lists.ToList<ISortable>(), current);
            }

            EntityHelper.UpdateFrom(current, element);
            _context.TodoLists.Update(current);
            await _context.SaveChangesAsync();

            return EntityHelper.ToElement(current);
        }

        [HttpPut("queries/{id}")]
        [ProducesResponseType(typeof(TodoQueryElement), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoQueryElement>> UpdateQueryElementAsync(int id, [FromBody] TodoQueryElement element)
        {
            var current = await _context.TodoQueries.GetAsync(id, q => q.Predicates);
            if (current == null)
            {
                return NotFound();
            }

            EntityHelper.UpdateFrom(current, element);

            _context.TodoQueries.Update(current);
            await _context.SaveChangesAsync();

            // we do not execute query, just return existing information
            var references = await _context.TodoReferences.GetAsync(r => r.TodoQueryId == id);
            return EntityHelper.ToElement(current, references.Count(r => !r.Item.Done));
        }
    }
}
