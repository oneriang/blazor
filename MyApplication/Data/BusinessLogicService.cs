// BusinessLogicService.cs

namespace MyApplication.Data
{
    public class Query
    {
        public string Operation { get; set; }
        public List<string> Columns { get; set; }
        public List<Condition> WhereConditions { get; set; }
    }

    public class Condition
    {
        public string ColumnName { get; set; }
        public ComparisonOperator Operator { get; set; }
        public object Value { get; set; }
        public LogicalOperator LogicalOperator { get; set; }
    }

    public enum ComparisonOperator
    {
        Equal,
        NotEqual,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        In
    }

    public enum LogicalOperator
    {
        And,
        Or
    }
}