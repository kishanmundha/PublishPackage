using PublishPackage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PublishPackage
{
    public class Helper
    {
        //public static List<T> GetCommonData2<T>(List<T> list1, List<T> list2, string PropertyName)
        //{
        //    var type = typeof(T);
        //    var parameter = Expression.Parameter(typeof(T), "x");
        //    var property = type.GetProperty(PropertyName);
        //    var member = Expression.MakeMemberAccess(parameter, property);
        //    var exp = Expression.Equal(member, Expression.Constant(true));

        //    var predicate = Expression.Lambda<Func<T, bool>>(exp, parameter);

        //    //list1.Where(predicate.Compile());

        //    var parameter2 = Expression.Parameter(typeof(T), "y");
        //    var exp2 = Expression.Field(member, PropertyName);
        //    var predicate2 = Expression.Lambda<Func<T, string>>(exp2, parameter2);

        //    var call = Expression.Call(typeof(Enumerable), "Contains", )
        //    var commonData = list1
        //        .Where(x =>
        //            list2
        //            .Select(predicate2.Compile())
        //            .Contains(x.ToString()));
        //}

        public static List<Models.IUnique> GetCommonData(List<Models.IUnique> list1, List<Models.IUnique> list2)
        {
            var commonData = list1.Where(x => list2.Select(y => y.GetUniqueId()).Contains(x.GetUniqueId())).ToList();

            return commonData;
        }

        public static List<Models.IUnique> GetNewData(List<Models.IUnique> list1, List<Models.IUnique> list2)
        {
            var commonData = list1.Where(x => !list2.Select(y => y.GetUniqueId()).Contains(x.GetUniqueId())).ToList();

            return commonData;
        }

        public static List<Models.IUnique> GetDeletedData(List<Models.IUnique> list1, List<Models.IUnique> list2)
        {
            var commonData = list2.Where(x => !list1.Select(y => y.GetUniqueId()).Contains(x.GetUniqueId())).ToList();

            return commonData;
        }

        public static string GetMD5Hash(string str)
        {
            System.Security.Cryptography.MD5 md5Hash = System.Security.Cryptography.MD5.Create();
            byte[] data = md5Hash.ComputeHash(Encoding.ASCII.GetBytes(str));

            // Create a new Stringbuilder to collect the bytes 
            // and create a string.
            StringBuilder sBuilder = new StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string. 
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();
        }

        public static List<DataCompareResult<T>> GetCompareResult<T>(List<IDataCompare> list1, List<IDataCompare> list2) where T : IDataCompare
        {
            List<DataCompareResult<T>> result = new List<DataCompareResult<T>>();

            foreach(var item in list1)
            {
                DataCompareResult<T> dcr = new DataCompareResult<T>();
                dcr.OldData = (T)item;

                var newData = list2.Find(x => x.KeyName == item.KeyName);

                if (newData != null)
                    dcr.NewData = (T)newData;

                result.Add(dcr);
            }

            foreach(var item in list2)
            {
                if (result.Find(x => x.KeyName == item.KeyName) != null)
                    continue;

                DataCompareResult<T> dcr = new DataCompareResult<T>();
                dcr.NewData = (T)item;

                result.Add(dcr);
            }

            return result;
        }
    }



    /// <summary>  
    /// Enables the efficient, dynamic composition of query predicates.  
    /// </summary>  
    public static class PredicateBuilder
    {
        /// <summary>  
        /// Creates a predicate that evaluates to true.  
        /// </summary>  
        public static Expression<Func<T, bool>> True<T>() { return param => true; }

        /// <summary>  
        /// Creates a predicate that evaluates to false.  
        /// </summary>  
        public static Expression<Func<T, bool>> False<T>() { return param => false; }

        /// <summary>  
        /// Creates a predicate expression from the specified lambda expression.  
        /// </summary>  
        public static Expression<Func<T, bool>> Create<T>(Expression<Func<T, bool>> predicate) { return predicate; }

        /// <summary>  
        /// Combines the first predicate with the second using the logical "and".  
        /// </summary>  
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>  
        /// Combines the first predicate with the second using the logical "or".  
        /// </summary>  
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        /// <summary>  
        /// Negates the predicate.  
        /// </summary>  
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>  
        /// Combines the first expression with the second using the specified merge function.  
        /// </summary>  
        static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)  
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with the parameters in the first  
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression  
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        class ParameterRebinder : ExpressionVisitor
        {
            readonly Dictionary<ParameterExpression, ParameterExpression> map;

            ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }

            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }

            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression replacement;

                if (map.TryGetValue(p, out replacement))
                {
                    p = replacement;
                }

                return base.VisitParameter(p);
            }
        }
    }
}
