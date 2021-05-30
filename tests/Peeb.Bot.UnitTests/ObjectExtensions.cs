using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Peeb.Bot.UnitTests
{
    public static class ObjectExtensions
    {
        public static TObject Set<TObject, TProperty>(this TObject obj, Expression<Func<TObject, TProperty>> property, TProperty value) where TObject : class
        {
            var memberExpression = (MemberExpression)property.Body;
            var propertyInfo = (PropertyInfo)memberExpression.Member;

            propertyInfo.SetValue(obj, value);

            return obj;
        }
    }
}
