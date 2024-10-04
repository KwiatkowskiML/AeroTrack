using DynamicData;
using PoProj.classes;

namespace PoProj.CommandsSupport;

public class Condition(string fieldName, string comparer, string value, string @operator = null)
{
    public string FieldName { get; init; } = fieldName;
    public string Comparer { get; init; } = comparer;
    public string Value { get; init; } = value;
    public string LogicalOperator { get; init; } = @operator;

    public static  Dictionary<string, Func<string, string, object, bool>> ConditionVerifier = new()
    {
        { "id", (string comVal, string comparer, object dbVal) => ComparerHelpers.UlongComparisons[comparer]((ulong)dbVal, ulong.Parse(comVal))},
        { "name", (string comVal, string comparer, object dbVal) => ComparerHelpers.StringComparisons[comparer]((string)dbVal, comVal)},
        { "code", (string comVal, string comparer, object dbVal) => ComparerHelpers.StringComparisons[comparer]((string)dbVal, comVal)},
        { "worldposition.lat", (string comVal, string comparer, object dbVal) => ComparerHelpers.FloatComparisons[comparer]((float)dbVal, float.Parse(comVal))},
        { "worldposition.long", (string comVal, string comparer, object dbVal) => ComparerHelpers.FloatComparisons[comparer]((float)dbVal, float.Parse(comVal))},
        { "amsl", (string comVal, string comparer, object dbVal) => ComparerHelpers.FloatComparisons[comparer]((float)dbVal, float.Parse(comVal))},
        { "serial", (string comVal, string comparer, object dbVal) => ComparerHelpers.StringComparisons[comparer]((string)dbVal, comVal)},
        { "countrycode", (string comVal, string comparer, object dbVal) => ComparerHelpers.StringComparisons[comparer]((string)dbVal, comVal)},
        { "model", (string comVal, string comparer, object dbVal) => ComparerHelpers.StringComparisons[comparer]((string)dbVal, comVal)},
        { "maxload", (string comVal, string comparer, object dbVal) => ComparerHelpers.FloatComparisons[comparer]((float)dbVal, float.Parse(comVal))},
        { "weight", (string comVal, string comparer, object dbVal) => ComparerHelpers.FloatComparisons[comparer]((float)dbVal, float.Parse(comVal))},
        { "description", (string comVal, string comparer, object dbVal) => ComparerHelpers.StringComparisons[comparer]((string)dbVal, comVal)},
        { "age", (string comVal, string comparer, object dbVal) => ComparerHelpers.UlongComparisons[comparer]((ulong)dbVal, ulong.Parse(comVal))},
        { "phone", (string comVal, string comparer, object dbVal) => ComparerHelpers.StringComparisons[comparer]((string)dbVal, comVal)},
        { "email", (string comVal, string comparer, object dbVal) => ComparerHelpers.StringComparisons[comparer]((string)dbVal, comVal)},
        { "practice", (string comVal, string comparer, object dbVal) => ComparerHelpers.UshortComparisons[comparer]((ushort)dbVal, ushort.Parse(comVal))},
        { "role", (string comVal, string comparer, object dbVal) => ComparerHelpers.StringComparisons[comparer]((string)dbVal, comVal)},
        { "class", (string comVal, string comparer, object dbVal) => ComparerHelpers.StringComparisons[comparer]((string)dbVal, comVal)},
        { "miles", (string comVal, string comparer, object dbVal) => ComparerHelpers.UlongComparisons[comparer]((ulong)dbVal, ulong.Parse(comVal))},
        { "firstclasssize", (string comVal, string comparer, object dbVal) => ComparerHelpers.UshortComparisons[comparer]((ushort)dbVal, ushort.Parse(comVal))},
        { "businessclasssize",(string comVal, string comparer, object dbVal) => ComparerHelpers.UshortComparisons[comparer]((ushort)dbVal, ushort.Parse(comVal))},
        { "economyclasssize",(string comVal, string comparer, object dbVal) => ComparerHelpers.UshortComparisons[comparer]((ushort)dbVal, ushort.Parse(comVal))},
        { "plane", (string comVal, string comparer, object dbVal) => ComparerHelpers.UlongComparisons[comparer]((ulong)dbVal, ulong.Parse(comVal))},
        { "takeofftime", (string comVal, string comparer, object dbVal) => ComparerHelpers.DateTimeComparisons[comparer](DateTime.Parse((string)dbVal), DateTime.Parse(comVal))},
        { "landingtime", (string comVal, string comparer, object dbVal) => ComparerHelpers.DateTimeComparisons[comparer](DateTime.Parse((string)dbVal), DateTime.Parse(comVal))}
    };
}


public static class ComparerHelpers
{
    public static readonly Dictionary<string, Func<ulong, ulong, bool>> UlongComparisons = new()
    {
        { "=", (x, y) => x == y },
        { "<=", (x, y) => x <= y },
        { ">=", (x, y) => x >= y },
        { "!=", (x, y) => x != y }
    };

    public static readonly Dictionary<string, Func<float, float, bool>> FloatComparisons = new()
    {
        { "=", (x, y) => x == y },
        { "<=", (x, y) => x <= y },
        { ">=", (x, y) => x >= y },
        { "!=", (x, y) => x != y }
    };
    
    public static readonly Dictionary<string, Func<string, string, bool>> StringComparisons = new()
    {
        { "=", (x, y) => x == y },
        { "!=", (x, y) => x != y }
    };
    
    public static readonly Dictionary<string, Func<DateTime, DateTime, bool>> DateTimeComparisons = new()
    {
        { "=", (x, y) => x == y },
        { "<=", (x, y) => x <= y },
        { ">=", (x, y) => x >= y },
        { "!=", (x, y) => x != y }
    };
    
    public static readonly Dictionary<string, Func<uint, uint, bool>> UintComparisons = new()
    {
        { "=", (x, y) => x == y },
        { "<=", (x, y) => x <= y },
        { ">=", (x, y) => x >= y },
        { "!=", (x, y) => x != y }
    };
    
    public static readonly Dictionary<string, Func<ushort, ushort, bool>> UshortComparisons = new()
    {
        { "=", (x, y) => x == y },
        { "<=", (x, y) => x <= y },
        { ">=", (x, y) => x >= y },
        { "!=", (x, y) => x != y }
    };

    public static readonly string[] ComparerTypes = new[]
    {
        "<=",
        ">=",
        "!=",
        "="
    };

    public static List<AirTraffic> ConditionExecution(List<AirTraffic> classList, List<Condition> conditions)
    {
        if (conditions == null || conditions.Count == 0)
            return classList;
        
        // initializing helper lists
        var acceptedSet = new HashSet<AirTraffic>();
        var dropoutList = new List<AirTraffic>();
        
        // executing first condition
        var firstCondition = conditions[0];
        foreach (var airTraffic in classList)
        {
            if (Condition.ConditionVerifier[firstCondition.FieldName]
                (firstCondition.Value, firstCondition.Comparer, airTraffic.Properties[firstCondition.FieldName].Value))
            {
                acceptedSet.Add(airTraffic);
            }
            else
            {
                dropoutList.Add(airTraffic);
            }
        }

        // executing next conditions
        for (int i = 1; i < conditions.Count; i++)
        {
            // and logical operator
            if (conditions[i].LogicalOperator == "and")
            {
                foreach (var airTraffic in acceptedSet)
                {
                    if (!Condition.ConditionVerifier[conditions[i].FieldName]
                        (conditions[i].Value, conditions[i].Comparer,
                            airTraffic.Properties[conditions[i].FieldName].Value))
                        acceptedSet.Remove(airTraffic);
                }
            }
            // or logical operator
            else if (conditions[i].LogicalOperator == "or")
            {
                for (int j = dropoutList.Count - 1; j >= 0; j--)
                {
                    if (!Condition.ConditionVerifier[conditions[i].FieldName]
                        (conditions[i].Value, conditions[i].Comparer,
                            dropoutList[j].Properties[conditions[i].FieldName].Value))
                        continue;

                    acceptedSet.Add(dropoutList[j]);
                    dropoutList.RemoveAt(j);
                }
            }
        }

        return acceptedSet.ToList();
    }
}