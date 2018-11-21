using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Common.Repository;

namespace Todo.Api.Models
{
    public abstract class TodoElementBase : EntityBase
    {
        public string Name { get; set; }

        public void UpdateFrom(TodoElementBase fromBase)
        {
            base.UpdateFrom(fromBase);

            Name = fromBase.Name;
        }
    }
}
