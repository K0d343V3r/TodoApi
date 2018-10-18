using Microsoft.AspNetCore.Mvc;
using System;
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
        [ProducesResponseType(typeof(List<TodoQueryResult>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<List<TodoQueryResult>>> ExecuteQueryAsync(int id)
        {
            var query = await _context.TodoQueries.GetAsync(id);
            if (query == null)
            {
                return NotFound();
            }

            // get last set of results
            var lastResults = await _context.TodoResults.GetAsync(r => r.TodoQueryId == id);

            // re-execute query (get new list items)
            var matchedItems = await InternalExecuteQueryAsync(query);

            // re-arrange new results based on existing result positions
            var mergedResults = await MergeResultsAsync(id, lastResults, matchedItems);

            await _context.SaveChangesAsync();
            return mergedResults;
        }

        private async Task<List<TodoQueryResult>> MergeResultsAsync(
            int queryId, IList<TodoQueryResult> lastResults, IList<TodoListItem> matchedItems)
        {
            // remove stale result items
            var mergedResults = new List<TodoQueryResult>();
            foreach (TodoQueryResult result in lastResults)
            {
                var item = matchedItems.FirstOrDefault(i => i.Id == result.Item.Id);
                if (item != null)
                {
                    // result still relevant, remove it from matching list
                    matchedItems.Remove(item);
                    mergedResults.Add(result);
                }
                else
                {
                    // result is stale, remove it from result set
                    EntityHelper.AdjustEntityPositions(lastResults.ToList<EntityBase>(), result.Position, false);
                    _context.TodoResults.Delete(result);
                }
            }

            // append new ones
            foreach (TodoListItem item in matchedItems)
            {
                var result = new TodoQueryResult()
                {
                    Item = item,
                    Position = mergedResults.Count,
                    TodoQueryId = queryId
                };

                mergedResults.Add(result);
                await _context.TodoResults.AddAsync(result);
            }

            return mergedResults;
        }

        private async Task<IList<TodoListItem>> InternalExecuteQueryAsync(TodoQuery query)
        {
            if (query.Operand == QueryOperand.DueDate)
            {
                var options = new RetrievalOptions<TodoListItem>()
                {
                    OrderByPredicate = i => i.DueDate.Date
                };
                if (query.Operator == QueryOperator.Equals)
                {
                    options.WherePredicate = i => i.DueDate.Date == query.DateValue.Date;
                }
                else if (query.Operator == QueryOperator.NotEquals)
                {
                    options.WherePredicate = i => i.DueDate.Date != query.DateValue.Date;
                }
                return await _context.TodoItems.GetAsync(options);
            }
            else if (query.Operand == QueryOperand.Important)
            {
                throw new NotImplementedException();
            }

            return new List<TodoListItem>();
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

            // delete query
            _context.TodoQueries.Delete(query);

            // and its last results
            var results = await _context.TodoResults.GetAsync(r => r.TodoQueryId == query.Id);
            results.ForEach(r => _context.TodoResults.Delete(r));
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