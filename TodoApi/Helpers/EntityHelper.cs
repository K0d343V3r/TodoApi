using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;
using TodoApi.Repository;

namespace TodoApi.Helpers
{
    internal static class EntityHelper
    {
        public static readonly int defaultListId = 1;

        public static async void CreateDefaultListAsync(ITodoRepositoryContext context)
        {
            if (context.TodoLists.Count() == 0)
            {
                await context.TodoLists.AddAsync(new TodoList() { Name = "Default" });
                await context.SaveChangesAsync();
            }
        }

        public static TodoElement ToElement(TodoList list)
        {
            var element = new TodoElement();
            InitializeElement(element, list);
            element.RemainingCount = list.Items.Count(i => !i.Done);
            return element;
        }

        private static void InitializeElement(TodoElementBase toElement, TodoElementBase fromElement)
        {
            toElement.Id = fromElement.Id;
            toElement.Name = fromElement.Name;
            ISortable fromSortable = fromElement as ISortable;
            ISortable toSortable = toElement as ISortable;
            if (fromSortable != null && toSortable != null)
            {
                toSortable.Position = fromSortable.Position;
            }
        }

        public static TodoQueryElement ToElement(TodoQuery query, int remainingCount)
        {
            var element = new TodoQueryElement();
            InitializeElement(element, query);
            element.Query = query;
            element.RemainingCount = remainingCount;
            return element;
        }

        public static void UpdateFrom(EntityBase toBase, EntityBase fromBase)
        {
            toBase.Id = fromBase.Id;
            var toSortable = toBase as ISortable;
            var fromSortable = fromBase as ISortable;
            if (toSortable != null && fromSortable != null)
            {
                toSortable.Position = fromSortable.Position;
            }
        }

        public static void UpdateFrom(TodoListItem toItem, TodoListItem fromItem)
        {
            UpdateFrom(toItem as EntityBase, fromItem as EntityBase);

            toItem.Task = fromItem.Task;
            toItem.Done = fromItem.Done;
            toItem.DueDate = fromItem.DueDate;
            toItem.Important = fromItem.Important;
            toItem.TodoListId = fromItem.TodoListId;
        }

        public static void UpdateFrom(TodoQuery toQuery, TodoQuery fromQuery)
        {
            UpdateFrom(toQuery as TodoElementBase, fromQuery as TodoElementBase);

            toQuery.OrderBy = fromQuery.OrderBy;
            toQuery.OrderByDirection = fromQuery.OrderByDirection;

            var predicates = new List<TodoQueryPredicate>(toQuery.Predicates);

            toQuery.Predicates.Clear();
            toQuery.Predicates.AddRange(fromQuery.Predicates);
        }

        public static void UpdateFrom(TodoElementBase toBase, TodoElementBase fromBase)
        {
            UpdateFrom(toBase as EntityBase, fromBase as EntityBase);

            toBase.Name = fromBase.Name;
        }

        public static void UpdateFrom(TodoList toList, TodoList fromList)
        {
            UpdateFrom(toList as TodoElementBase, fromList as TodoElementBase);

            // remove obsolete child items
            List<TodoListItem> items = new List<TodoListItem>(toList.Items);
            foreach (var item in items)
            {
                if (!fromList.Items.Any(i => i.Id == item.Id))
                {
                    toList.Items.Remove(item);
                }
            }

            // update existing or add new child items
            for (int i = 0; i < fromList.Items.Count; i++)
            {
                var current = fromList.Items[i].Id == 0 ? null : 
                    toList.Items.FirstOrDefault(t => t.Id == fromList.Items[i].Id);
                if (current == null)
                {
                    toList.Items.Add(fromList.Items[i]);
                }
                else
                {
                    UpdateFrom(current, fromList.Items[i]);
                }
            }
        }
    }
}
