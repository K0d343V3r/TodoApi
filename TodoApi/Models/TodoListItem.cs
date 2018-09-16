﻿using System;

namespace TodoApi.Models
{
    public class TodoListItem
    {
        public long Id { get; set; }
        public string Task { get; set; }
        public bool Done { get; set; }

        public long TodoListId { get; set; }

        public void UpdateFrom(TodoListItem item)
        {
            Id = item.Id;
            Task = item.Task;
            Done = item.Done;
            TodoListId = item.TodoListId;
        }
    }
}