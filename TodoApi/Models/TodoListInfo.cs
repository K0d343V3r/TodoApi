using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public class TodoListInfo : IEntityBase
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Position { get; set; }

        [NotMapped]
        public virtual int ItemCount { get; private set; }

        public TodoListInfo()
        {
        }

        public TodoListInfo(int itemCount)
        {
            ItemCount = itemCount;
        }
    }
}
