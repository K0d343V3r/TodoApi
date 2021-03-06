﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Common;
using Api.Common.Repository;

namespace Todo.Api.Models
{
    public abstract class TodoElementBase : EntityBase, IUpdatable<TodoElementBase>
    {
        public string Name { get; set; }

        public void UpdateFrom(TodoElementBase fromBase)
        {
            Name = fromBase.Name;
        }
    }
}
