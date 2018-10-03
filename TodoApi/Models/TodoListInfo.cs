using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public class TodoListInfo : ISortable
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }
    }
}
