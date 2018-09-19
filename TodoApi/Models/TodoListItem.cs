using System;

namespace TodoApi.Models
{
    public class TodoListItem : ISortable
    {
        public long Id { get; set; }
        public string Task { get; set; }
        public bool Done { get; set; }
        public int Position { get; set; }
        public long TodoListId { get; set; }
    }
}