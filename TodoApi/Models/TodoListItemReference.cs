using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public class TodoListItemReference : EntityBase
    {
        public TodoElementBase Item { get; set; }
    }
}
