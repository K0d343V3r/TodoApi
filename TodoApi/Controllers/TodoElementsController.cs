﻿using Microsoft.AspNetCore.Mvc;
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
            var queries = await _context.TodoQueries.GetAsync();

            var elements = new List<TodoQueryElement>();
            foreach (var query in queries)
            {
                var references = await _context.TodoReferences.GetAsync(r => r.TodoQueryId == query.Id);
                elements.Add(EntityHelper.ToElement(query, references.Count));
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
        public async Task<ActionResult<TodoElement>> UpdateListElementAsync(int id, [FromBody] TodoElementBase element)
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
                EntityHelper.AdjustEntityPositions(element, lists.ToList<EntityBase>(), current);
            }

            EntityHelper.UpdateFrom(current, element);
            _context.TodoLists.Update(current);
            await _context.SaveChangesAsync();

            return EntityHelper.ToElement(current);
        }

        [HttpPut("queries/{id}")]
        [ProducesResponseType(typeof(TodoQueryElement), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<TodoQueryElement>> UpdateQueryElementAsync(int id, [FromBody] TodoElementBase element)
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

            var references = await _context.TodoReferences.GetAsync(r => r.TodoQueryId == id);
            return EntityHelper.ToElement(current, references.Count);
        }
    }
}
