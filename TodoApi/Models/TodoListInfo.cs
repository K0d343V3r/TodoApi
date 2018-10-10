using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public class TodoListInfo : ITodoListBase
    {
        public long Id { get; set; }
        public int Position { get; set; }
        public string Name { get; set; }
        public int ItemCount { get; internal set; }
    }
}
