using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TodoApi.Models;
using TodoApi.Repository;

namespace TodoApi.Helpers
{
    internal class QueryHelper
    {
        private ITodoRepositoryContext _context;
        private readonly ParameterExpression _itemParameter = Expression.Parameter(typeof(TodoListItem), "i");

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
                    EntityHelper.AdjustEntityPositions(lastResults.ToList<ISortable>(), result.Position, false);
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
            return await _context.TodoItems.GetAsync(ToRetrievalOptions(query));
        }

        private RetrievalOptions<TodoListItem> ToRetrievalOptions(TodoQuery query)
        {
            var options = new RetrievalOptions<TodoListItem>();
            if (query.OrderBy.HasValue)
            {
                options.OrderBy.Predicate = GetSortByExpression(query);
                if (!query.OrderByDirection.HasValue)
                {
                    options.OrderBy.Ascending = true;
                }
                else
                {
                    options.OrderBy.Ascending = query.OrderByDirection.Value == QueryDirection.Ascending;
                }
            }

            options.Where = GetWhereExpression(query);
            return options;
        }

        private Expression<Func<TodoListItem, bool>> GetWhereExpression(TodoQuery query)
        {
            var expression = new StringBuilder();
            List<object> values = new List<object>(10);
            foreach (var predicate in query.Predicates)
            {
                if (predicate.Group.HasValue && predicate.Group.Value == QueryPredicateGroup.Begin)
                {
                    expression.Append("(");
                }
                object value = GetPredicateValue(predicate);
                expression.Append($"{GetOperandComparisonString(predicate.Operand, value)}");
                expression.Append($" {GetOperatorString(predicate.Operator)}");
                expression.Append($" @{values.Count}");
                values.Add(value);
                if (predicate.Group.HasValue && predicate.Group.Value == QueryPredicateGroup.End)
                {
                    expression.Append(")");
                }
                if (predicate.Keyword.HasValue)
                {
                    expression.Append($" {GetKeywordString(predicate.Keyword.Value)} ");
                }
            }

            return (Expression<Func<TodoListItem, bool>>)DynamicExpressionParser.ParseLambda(
                 new[] { _itemParameter }, 
                 typeof(bool), 
                 expression.ToString(), 
                 values.ToArray());
        }
        
        private string GetKeywordString(QueryKeyword keyword)
        {
            switch (keyword)
            {
                case QueryKeyword.And:
                    return "&&";

                case QueryKeyword.Or:
                    return "||";

                default:
                    throw new InvalidOperationException("Invalid keyword.");
            }
        }

        private object GetPredicateValue(TodoQueryPredicate predicate)
        {
            if (predicate.Operand == QueryOperand.Important)
            {
                return predicate.BoolValue ?? false;
            }
            else if (predicate.Operand == QueryOperand.DueDate)
            {
                return ResolveTargetDate(predicate);
            }
            else if  (predicate.Operand == QueryOperand.Done)
            {
                return predicate.BoolValue ?? false;
            }
            else
            {
                throw new InvalidOperationException("Invalid operand.");
            }
        }

        private DateTime? ResolveTargetDate(TodoQueryPredicate predicate)
        {
            if (!predicate.RelativeDateValue.HasValue)
            {
                if (!predicate.AbsoluteDateValue.HasValue)
                {
                    return null;
                }
                else
                {
                    return predicate.AbsoluteDateValue.Value.Date;
                }
            }
            else
            {
                DateTime date = DateTime.UtcNow;
                date = new DateTime(date.Year, date.Month, date.Day + predicate.RelativeDateValue.Value);
                DateTime.SpecifyKind(date, DateTimeKind.Utc);
                return date.Date;
            }
        }

        private Expression<Func<TodoListItem, object>> GetSortByExpression(TodoQuery query)
        {
            var operandString = GetOperandString(query.OrderBy.Value);
            if (query.OrderBy.Value == QueryOperand.Important || query.OrderBy.Value == QueryOperand.Done)
            {
                return (Expression<Func<TodoListItem, object>>)DynamicExpressionParser.ParseLambda(
                    new[] { _itemParameter },
                    typeof(object),
                    $"{operandString}");
            }
            else if (query.OrderBy.Value == QueryOperand.DueDate)
            {
                // bounds tells sorting what to do with null dates
                var direction = query.OrderByDirection ?? QueryDirection.Ascending;
                var bounds = direction == QueryDirection.Ascending ? DateTime.MaxValue : DateTime.MinValue;
                return (Expression<Func<TodoListItem, object>>)DynamicExpressionParser.ParseLambda(
                    new[] { _itemParameter }, 
                    typeof(object), 
                    $"{operandString} ?? @0", bounds);
            }
            else
            {
                throw new InvalidOperationException("Invalid operand.");
            }
        }

        private string GetOperandString(QueryOperand operand)
        {
            switch (operand)
            {
                case QueryOperand.DueDate:
                    return $"{_itemParameter}.DueDate";

                case QueryOperand.Important:
                    return $"{_itemParameter}.Important";

                case QueryOperand.Done:
                    return $"{_itemParameter}.Done";

                default:
                    throw new InvalidOperationException("Invalid operand.");
            }
        }

        private string GetOperandComparisonString(QueryOperand operand, object value)
        {
            if (operand == QueryOperand.DueDate && value != null)
            {
                return $"{_itemParameter}.DueDate.HasValue && {_itemParameter}.DueDate.Value.Date";
            }
            else
            {
                return GetOperandString(operand);
            }
        }

        private string GetOperatorString(QueryOperator _operator)
        {
            switch (_operator)
            {
                case QueryOperator.Equals:
                    return "==";

                case QueryOperator.GreaterThan:
                    return ">";

                case QueryOperator.GreaterThanOrEquals:
                    return ">=";

                case QueryOperator.LessThan:
                    return "<";

                case QueryOperator.LessThanOrEquals:
                    return "<=";

                case QueryOperator.NotEquals:
                    return "!=";

                default:
                    throw new InvalidOperationException("Invalid operator.");
            }
        }
    }
}
