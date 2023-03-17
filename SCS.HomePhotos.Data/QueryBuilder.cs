using Dapper;

using System;
using System.Globalization;
using System.Linq;

namespace SCS.HomePhotos.Data
{
    public static class QueryBuilder
    {
        public static class Operators
        {
            public const string Or = "OR";
            public const string And = "AND";
        }

        public static class Comparisons
        {
            public const string Equal = "=";

            public const string NotEqual = "<>";

            public const string LessThan = "<";

            public const string GreaterThan = ">";

            public const string GreaterThanOrEqual = ">=";

            public const string LessThanOrEqual = "<=";
        }

        public static string BuildClause(string propertyName, string operation, string givenValue)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException($"'{nameof(propertyName)}' cannot be null or empty.", nameof(propertyName));
            }

            if (string.IsNullOrEmpty(operation))
            {
                throw new ArgumentException($"'{nameof(operation)}' cannot be null or empty.", nameof(operation));
            }

            return $"{propertyName} {operation} {givenValue}";
        }

        public static string CombineFilters(string filterA, string operatorString, string filterB)
        {
            return string.Format(CultureInfo.InvariantCulture, "({0}) {1} ({2})", filterA, operatorString, filterB);
        }

        public static string AndFilters(params string[] clauses)
        {
            if (clauses == null || clauses.Length == 0)
            {
                return string.Empty;
            }
            return clauses.Aggregate((c1, c2) => $"({c1.Trim()}) {Operators.And} ({c2.Trim()})");
        }

        public static string OrFilters(params string[] clauses)
        {
            return clauses.Aggregate((c1, c2) => $"({c1.Trim()}) {Operators.Or} ({c2.Trim()})");
        }

        public static (string Sql, DynamicParameters Parameters) AndFilters(params (string Sql, DynamicParameters Parameters)[] clauses)
        {
            return ConcatinateFilters(Operators.And, clauses);
        }

        public static (string Sql, DynamicParameters Parameters) OrFilters(params (string Sql, DynamicParameters Parameters)[] clauses)
        {
            return ConcatinateFilters(Operators.Or, clauses);
        }

        private static (string Sql, DynamicParameters Parameters) ConcatinateFilters(string op, params (string Sql, DynamicParameters Parameters)[] clauses)
        {
            if (clauses == null || clauses.Length == 0)
            {
                return (string.Empty, new DynamicParameters());
            }

            var sql = string.Empty;

            foreach (var c in clauses)
            {
                if (c.Sql.Trim().Length > 0)
                {
                    sql += $"{op} {c.Sql} ";
                }
            }

            if (sql.Length > 0)
            {
                sql = sql.Substring(op.Length + 1);
            }

            var parameters = new DynamicParameters();

            foreach (var c in clauses)
            {
                parameters.AddDynamicParams(c.Parameters);
            }
            return (sql, parameters);
        }
    }
}
