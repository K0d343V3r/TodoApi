using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoApi.Models
{
    public static class EntityHelper
    {
        public static List<TodoListInfo> ToListInfos(IList<TodoList> lists)
        {
            return lists.Select(list => new TodoListInfo() {
                Id = list.Id,
                Name = list.Name,
                Position = list.Position
            }).ToList();
        }

        public static void AdjustPositions(IEntityBase item, IList<IEntityBase> items, IEntityBase current = null)
        {
            if (items.Count == 0)
            {
                // list is empty, put item in first position
                item.Position = 0;
            }
            else if (item.Position >= items.Count)
            {
                // requested position is beyond the last item
                item.Position = items[items.Count - 1].Position + 1;
            }
            else if (current == null || current.Id != items[item.Position].Id)
            {
                int startIndex = item.Position;
                int endPosition = current == null ? items[items.Count - 1].Position : current.Position;
                int step = endPosition >= items[startIndex].Position ? 1 : -1;

                int position = items[item.Position].Position;
 
                for (int i = startIndex; ; i += step)
                {
                    int currentPosition = items[i].Position;
                    items[i].Position += step;
                    if (currentPosition == endPosition)
                    {
                        break;
                    }
                }

                item.Position = position;
            }
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
