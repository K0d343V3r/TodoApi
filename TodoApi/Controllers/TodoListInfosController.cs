using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoListInfosController : ControllerBase
    {
        private readonly TodoContext _context;

        public TodoListInfosController(TodoContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<List<TodoListInfo>>> GetAllListInfosAsync()
        {
            List<TodoList> lists = await _context.TodoLists
                .OrderBy(s => s.Position)
                .ToListAsync();
            return EntityHelper.ToListInfos(lists);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(TodoListInfo), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> UpdateListInfoAsync(long id, [FromBody] TodoListInfo info)
        {
            var current = await _context.TodoLists.FindAsync(id);
            if (current == null)
            {
                return NotFound();
            }

            if (current.Position != info.Position)
            {
                // adjust list positions based on update request
                var lists = await _context.TodoLists.OrderBy(t => t.Position).ToListAsync<ISortable>();
                EntityHelper.AdjustPositions(lists, info);
            }

            EntityHelper.UpdateFrom(current, info);
            _context.Update(current);

            await _context.SaveChangesAsync();
            return Ok(current);
        }
    }
}
