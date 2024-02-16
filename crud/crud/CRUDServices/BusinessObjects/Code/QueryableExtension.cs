using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CRUDServices.BusinessObjects.Code
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> OrderByDynamic<T>(this IQueryable<T> q, string SortField, bool Ascending)
        {
            var param = Expression.Parameter(typeof(T), "p");
            var prop = Expression.Property(param, SortField);
            var exp = Expression.Lambda(prop, param);
            string method = Ascending ? "OrderBy" : "OrderByDescending";
            Type[] types = new Type[] { q.ElementType, exp.Body.Type };
            var mce = Expression.Call(typeof(Queryable), method, types, q.Expression, exp);
            return q.Provider.CreateQuery<T>(mce);
        }

        public static IQueryable<T> WhereByDynamic<T>(this IQueryable<T> source, string propertyName, object value, string @operator = "=")
        {
            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, propertyName);
            var propertyType = property.Type;
            var constant = Expression.Constant(value);

            Expression body = null;
            if (propertyType == typeof(string) && @operator == "%")
            {
                var method = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                body = Expression.Call(property, method, constant);
            }
            else if (propertyType == typeof(bool))
            {
                var convertedConstant = Expression.Constant(bool.Parse(value.ToString()), propertyType);
                body = Expression.Equal(property, convertedConstant);
            }
            else if (propertyType == typeof(bool?))
            {
                var convertedConstant = Expression.Constant(bool.Parse(value.ToString()), typeof(bool?));
                body = Expression.Equal(property, convertedConstant);
            }
            else if (propertyType == typeof(Guid))
            {
                var guidValue = Guid.Parse(value.ToString());
                var guidConstant = Expression.Constant(guidValue, typeof(Guid));
                body = Expression.Equal(property, guidConstant);
            }
            else if (propertyType == typeof(int))
            {
                var convertedConstant = Expression.Constant(int.Parse(value.ToString()), propertyType);

                switch (@operator)
                {
                    case "=":
                        body = Expression.Equal(property, convertedConstant);
                        break;
                    case ">":
                        body = Expression.GreaterThan(property, convertedConstant);
                        break;
                    case "<":
                        body = Expression.LessThan(property, convertedConstant);
                        break;
                    case ">=":
                        body = Expression.GreaterThanOrEqual(property, convertedConstant);
                        break;
                    case "<=":
                        body = Expression.LessThanOrEqual(property, convertedConstant);
                        break;
                    case "!=":
                        body = Expression.NotEqual(property, convertedConstant);
                        break;
                    default:
                        throw new NotSupportedException($"Operator '{@operator}' is not supported.");
                }
            }
            else if (propertyType == typeof(int?))
            {
                var convertedConstant = Expression.Constant(int.Parse(value.ToString()), typeof(int?));

                switch (@operator)
                {
                    case "=":
                        body = Expression.Equal(property, convertedConstant);
                        break;
                    case ">":
                        body = Expression.GreaterThan(property, convertedConstant);
                        break;
                    case "<":
                        body = Expression.LessThan(property, convertedConstant);
                        break;
                    case ">=":
                        body = Expression.GreaterThanOrEqual(property, convertedConstant);
                        break;
                    case "<=":
                        body = Expression.LessThanOrEqual(property, convertedConstant);
                        break;
                    case "!=":
                        body = Expression.NotEqual(property, convertedConstant);
                        break;
                    default:
                        throw new NotSupportedException($"Operator '{@operator}' is not supported.");
                }
            }
            else if (propertyType == typeof(long))
            {
                var convertedConstant = Expression.Constant(long.Parse(value.ToString()), propertyType);

                switch (@operator)
                {
                    case "=":
                        body = Expression.Equal(property, convertedConstant);
                        break;
                    case ">":
                        body = Expression.GreaterThan(property, convertedConstant);
                        break;
                    case "<":
                        body = Expression.LessThan(property, convertedConstant);
                        break;
                    case ">=":
                        body = Expression.GreaterThanOrEqual(property, convertedConstant);
                        break;
                    case "<=":
                        body = Expression.LessThanOrEqual(property, convertedConstant);
                        break;
                    case "!=":
                        body = Expression.NotEqual(property, convertedConstant);
                        break;
                    default:
                        throw new NotSupportedException($"Operator '{@operator}' is not supported.");
                }
            }
            else if (propertyType == typeof(long?))
            {
                var convertedConstant = Expression.Constant(long.Parse(value.ToString()), typeof(long?));

                switch (@operator)
                {
                    case "=":
                        body = Expression.Equal(property, convertedConstant);
                        break;
                    case ">":
                        body = Expression.GreaterThan(property, convertedConstant);
                        break;
                    case "<":
                        body = Expression.LessThan(property, convertedConstant);
                        break;
                    case ">=":
                        body = Expression.GreaterThanOrEqual(property, convertedConstant);
                        break;
                    case "<=":
                        body = Expression.LessThanOrEqual(property, convertedConstant);
                        break;
                    case "!=":
                        body = Expression.NotEqual(property, convertedConstant);
                        break;
                    default:
                        throw new NotSupportedException($"Operator '{@operator}' is not supported.");
                }
            }
            else if (propertyType == typeof(decimal))
            {
                var convertedConstant = Expression.Constant(decimal.Parse(value.ToString()), propertyType);

                switch (@operator)
                {
                    case "=":
                        body = Expression.Equal(property, convertedConstant);
                        break;
                    case ">":
                        body = Expression.GreaterThan(property, convertedConstant);
                        break;
                    case "<":
                        body = Expression.LessThan(property, convertedConstant);
                        break;
                    case ">=":
                        body = Expression.GreaterThanOrEqual(property, convertedConstant);
                        break;
                    case "<=":
                        body = Expression.LessThanOrEqual(property, convertedConstant);
                        break;
                    case "!=":
                        body = Expression.NotEqual(property, convertedConstant);
                        break;
                    default:
                        throw new NotSupportedException($"Operator '{@operator}' is not supported.");
                }
            }
            else if (propertyType == typeof(decimal?))
            {
                var convertedConstant = Expression.Constant(decimal.Parse(value.ToString()), typeof(decimal?));

                switch (@operator)
                {
                    case "=":
                        body = Expression.Equal(property, convertedConstant);
                        break;
                    case ">":
                        body = Expression.GreaterThan(property, convertedConstant);
                        break;
                    case "<":
                        body = Expression.LessThan(property, convertedConstant);
                        break;
                    case ">=":
                        body = Expression.GreaterThanOrEqual(property, convertedConstant);
                        break;
                    case "<=":
                        body = Expression.LessThanOrEqual(property, convertedConstant);
                        break;
                    case "!=":
                        body = Expression.NotEqual(property, convertedConstant);
                        break;
                    default:
                        throw new NotSupportedException($"Operator '{@operator}' is not supported.");
                }
            }
            else
            {
                var convertedConstant = Expression.Convert(constant, propertyType);

                switch (@operator)
                {
                    case "=":
                        body = Expression.Equal(property, convertedConstant);
                        break;
                    case ">":
                        body = Expression.GreaterThan(property, convertedConstant);
                        break;
                    case "<":
                        body = Expression.LessThan(property, convertedConstant);
                        break;
                    case ">=":
                        body = Expression.GreaterThanOrEqual(property, convertedConstant);
                        break;
                    case "<=":
                        body = Expression.LessThanOrEqual(property, convertedConstant);
                        break;
                    case "!=":
                        body = Expression.NotEqual(property, convertedConstant);
                        break;
                    default:
                        throw new NotSupportedException($"Operator '{@operator}' is not supported.");
                }
            }

            var lambda = Expression.Lambda<Func<T, bool>>(body, parameter);
            return source.Where(lambda);
        }
    }
}
