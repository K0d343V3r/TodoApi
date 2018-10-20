using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
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

        public static TodoElement ToElement(TodoElementBase element, int childCount)
        {
            return new TodoElement()
            {
                Id = element.Id,
                Name = element.Name,
                Position = element.Position,
                ChildCount = childCount
            };
        }

        public static void AdjustListPosition(TodoList list, IList<EntityBase> lists, bool add)
        {
            AdjustListPositionInternal(list, lists.Count);

            AdjustEntityPositions(lists, list.Position, add);
        }

        private static void AdjustListPositionInternal(TodoList list, int count)
        {
            AdjustEntityPosition(list, count);

            for (int i = 0; i < list.Items.Count; i++)
            {
                list.Items[i].Position = i;
            }
        }

        private static void AdjustEntityPosition(EntityBase entity, int count)
        {
            if (entity.Position < 0)
            {
                entity.Position = 0;
            }
            if (entity.Position > count)
            {
                entity.Position = count;
            }
        }

        public static void AdjustEntityPosition(EntityBase entity, IList<EntityBase> entities, bool add)
        {
            AdjustEntityPosition(entity, entities.Count);

            AdjustEntityPositions(entities, entity.Position, add);
        }

        public static void AdjustEntityPositions(IList<EntityBase> entities, int position, bool add)
        {
            for (int i = add ? position : position + 1; i < entities.Count; i++)
            {
                entities[i].Position = add ? entities[i].Position + 1 : entities[i].Position - 1;
            }
        }

        public static void AdjustListPositions(TodoList newList, IList<EntityBase> lists, TodoList currentList)
        {
            AdjustListPositionInternal(newList, lists.Count);

            AdjustEntityPositions(lists, currentList.Position, newList.Position);
        }

        private static void AdjustEntityPositions(IList<EntityBase> list, int oldPosition, int newPosition)
        {
            if (newPosition != oldPosition)
            {
                int start, end, step;
                if (newPosition < oldPosition)
                {
                    start = newPosition;
                    end = oldPosition - 1;
                    step = 1;
                }
                else
                {
                    start = oldPosition + 1;
                    end = newPosition;
                    step = -1;
                }
                for (int i = start; i <= end; i++)
                {
                    list[i].Position += step;
                }
            }
        }

        public static void AdjustEntityPositions(
            EntityBase newEntity, IList<EntityBase> entities, EntityBase currentEntity)
        {
            AdjustEntityPosition(newEntity, entities.Count);

            AdjustEntityPositions(entities, currentEntity.Position, newEntity.Position);
        }

        public static void UpdateFrom(EntityBase toBase, EntityBase fromBase)
        {
            toBase.Id = fromBase.Id;
            toBase.Position = fromBase.Position;
        }

        public static void UpdateFrom(TodoListItem toItem, TodoListItem fromItem)
        {
            UpdateFrom(toItem as EntityBase, fromItem as EntityBase);

            toItem.Task = fromItem.Task;
            toItem.Done = fromItem.Done;
            toItem.DueDate = fromItem.DueDate;
            toItem.TodoListId = fromItem.TodoListId;
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
