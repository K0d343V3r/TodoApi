using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Models;
using Microsoft.EntityFrameworkCore;

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
    }
}
