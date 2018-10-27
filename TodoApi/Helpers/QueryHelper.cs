﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;
using TodoApi.Repository;

namespace TodoApi.Helpers
{
    internal class QueryHelper
    {
        private ITodoRepositoryContext _context;

        public QueryHelper(ITodoRepositoryContext context)
        {
            _context = context;
        }

        public async Task<List<TodoItemReference>> ExecuteQueryAsync(TodoQuery query)
        {
            // get last set of results
            var lastResults = await _context.TodoReferences.GetAsync(r => r.TodoQueryId == query.Id, r => r.Item);

            // re-execute query (get new list items)
            var matchedItems = await InternalExecuteQueryAsync(query);

            // re-arrange new results based on existing result positions
            var mergedResults = await MergeResultsAsync(query.Id, lastResults, matchedItems);

            await _context.SaveChangesAsync();

            return mergedResults;
        }

        private async Task<List<TodoItemReference>> 
            MergeResultsAsync(int queryId, IList<TodoItemReference> lastResults, IList<TodoListItem> matchedItems)
        {
            // remove stale result items
            var mergedResults = new List<TodoItemReference>();
            foreach (TodoItemReference result in lastResults)
            {
                var item = matchedItems.FirstOrDefault(i => i.Id == result.Item.Id);
                if (item != null)
                {
                    // result still relevant, remove it from matching list
                    matchedItems.Remove(item);
                    mergedResults.Add(result);
                }
                else
                {
                    // result is stale, remove it from result set
                    EntityHelper.AdjustEntityPositions(lastResults.ToList<EntityBase>(), result.Position, false);
                    _context.TodoReferences.Delete(result);
                }
            }

            // append new ones
            foreach (TodoListItem item in matchedItems)
            {
                var result = new TodoItemReference()
                {
                    Item = item,
                    Position = mergedResults.Count,
                    TodoQueryId = queryId
                };

                mergedResults.Add(result);
                await _context.TodoReferences.AddAsync(result);
            }

            return mergedResults;
        }

        private async Task<IList<TodoListItem>> InternalExecuteQueryAsync(TodoQuery query)
        {
            var options = new RetrievalOptions<TodoListItem>();
            if (query.Operand == QueryOperand.DueDate)
            {
                DateTime targetDay = ResolveTargetDate(query);
                options.OrderBy.Predicate = i => i.DueDate.Date;
                if (query.Operator == QueryOperator.Equals)
                {
                    options.OrderBy.Predicate = i => i.Important;
                    options.Where = i => i.DueDate.Date == targetDay;
                }
                else if (query.Operator == QueryOperator.NotEquals)
                {
                    options.Where = i => i.DueDate.Date != targetDay;
                }
                else if (query.Operator == QueryOperator.GreaterThan)
                {
                    options.Where = i => i.DueDate.Date > targetDay;
                }
                else if (query.Operator == QueryOperator.GreaterThanOrEquals)
                {
                    options.Where = i => i.DueDate.Date >= targetDay;
                }
                else if (query.Operator == QueryOperator.LessThan)
                {
                    options.OrderBy.Ascending = false;
                    options.Where = i => i.DueDate != default(DateTime) && i.DueDate.Date < targetDay;
                }
                else if (query.Operator == QueryOperator.LessThanOrEquals)
                {
                    options.OrderBy.Ascending = false;
                    options.Where = i => i.DueDate != default(DateTime) && i.DueDate.Date <= targetDay;
                }
                return await _context.TodoItems.GetAsync(options);
            }
            else if (query.Operand == QueryOperand.Important)
            {
                if (query.Operator == QueryOperator.Equals)
                {
                    options.Where = i => i.Important == query.BoolValue;
                }
                else if (query.Operator == QueryOperator.NotEquals)
                {
                    options.OrderBy.Predicate = i => i.Important;
                    options.Where = i => i.Important != query.BoolValue;
                }
            }

            return options.Where != null ? 
                await _context.TodoItems.GetAsync(options) : new List<TodoListItem>();
        }

        private DateTime ResolveTargetDate(TodoQuery query)
        {
            if (query.AbsoluteDateValue != default(DateTime))
            {
                return query.AbsoluteDateValue.Date;
            }
            else
            {
                DateTime date = DateTime.UtcNow;
                // offset day by requested value
                date = new DateTime(date.Year, date.Month, date.Day + query.RelativeDateValue);
                DateTime.SpecifyKind(date, DateTimeKind.Utc);
                return date.Date;
            }
        }
    }
}