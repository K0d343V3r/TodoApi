﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public class TodoElement : TodoElementBase
    {
        public int ChildCount { get; internal set; }
    }
}