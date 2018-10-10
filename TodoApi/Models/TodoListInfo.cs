using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public class TodoListInfo : TodoListBase
    {
        public int ItemCount { get; internal set; }
    }
}
