﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    public class TodoElement : TodoBrowsingElement, ISortable
    {
        public int Position { get; set; }
    }
}
