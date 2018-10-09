using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Repository;

namespace TodoApi.Models
{
    internal static class EntityHelper
    {
        public static List<TodoListInfo> ToListInfos(IList<TodoList> lists)
        {
            return lists.Select(list => ToListInfo(list)).ToList();
        }

        public static TodoListInfo ToListInfo(TodoList list)
        {
            return new TodoListInfo(list.Items.Count)
            {
                Id = list.Id,
                Name = list.Name,
                Position = list.Position
            };
        }

        public static void AdjustListPosition(TodoList list, IList<IEntityBase> lists, bool add)
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

        private static void AdjustEntityPosition(IEntityBase entity, int count)
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

        public static void AdjustListItemPosition(TodoListItem item, IList<IEntityBase> items, bool add)
        {
            AdjustEntityPosition(item, items.Count);

            AdjustEntityPositions(items, item.Position, add);
        }

        public static void AdjustEntityPositions(IList<IEntityBase> list, int position, bool add)
        {
            for (int i = add ? position : position + 1; i < list.Count; i++)
            {
                list[i].Position = add ? list[i].Position + 1 : list[i].Position - 1;
            }
        }

        public static void AdjustListPositions(TodoList newList, IList<IEntityBase> lists, TodoList currentList)
        {
            AdjustListPositionInternal(newList, lists.Count);

            AdjustEntityPositions(lists, currentList.Position, newList.Position);
        }

        private static void AdjustEntityPositions(IList<IEntityBase> list, int oldPosition, int newPosition)
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

        public static void AdjustListItemPositions(TodoListItem newListItem, IList<IEntityBase> items, TodoListItem currentItem)
        {
            AdjustEntityPosition(newListItem, items.Count);

            AdjustEntityPositions(items, currentItem.Position, newListItem.Position);
        }

        public static void AdjustListInfoPositions(TodoListInfo newInfo, IList<IEntityBase> infos, TodoListInfo currentInfo)
        {
            AdjustEntityPosition(newInfo, infos.Count);

            AdjustEntityPositions(infos, currentInfo.Position, newInfo.Position);
        }

        public static void UpdateFrom(TodoListItem toItem, TodoListItem fromItem)
        {
            toItem.Id = fromItem.Id;
            toItem.Task = fromItem.Task;
            toItem.Done = fromItem.Done;
            toItem.Position = fromItem.Position;
            toItem.TodoListId = fromItem.TodoListId;
        }

        public static void UpdateFrom(TodoListInfo toInfo, TodoListInfo fromInfo)
        {
            toInfo.Id = fromInfo.Id;
            toInfo.Name = fromInfo.Name;
            toInfo.Position = fromInfo.Position;
        }

        public static void UpdateFrom(TodoList toList, TodoList fromList)
        {
            // update info
            UpdateFrom(toList as TodoListInfo, fromList as TodoListInfo);

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
