namespace TodoApi.Models
{
    public class TodoListItem
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public bool IsComplete { get; set; }

        public long TodoListId { get; set; }
    }
}
