namespace PoProj.CommandsSupport;

public static class QueryHelper
{
    public static List<Condition> ConditionParser(string conditionsString)
    {
        var conditionsList = new List<Condition>();
        var ConditionsAndOperators = conditionsString.Trim().Split(' ');
        for (int i = 0; i < ConditionsAndOperators.Length; i++)
        {
            ConditionsAndOperators[i] = ConditionsAndOperators[i].Trim();
        }
            
        // first condition
        int comparerId = Array.FindIndex(ComparerHelpers.ComparerTypes, comparerType =>
            ConditionsAndOperators[0].Contains(comparerType));
        string comparer = ComparerHelpers.ComparerTypes[comparerId];
        var stringValues = ConditionsAndOperators[0].Split(comparer, StringSplitOptions.RemoveEmptyEntries);
        conditionsList.Add(new Condition(stringValues[0], comparer, stringValues[1]));
            
        // the rest of the conditions
        for (int i = 2; i < ConditionsAndOperators.Length; i += 2)
        {
            string logicalOperator = ConditionsAndOperators[i - 1];
            comparerId = Array.FindIndex(ComparerHelpers.ComparerTypes, comparerType =>
                ConditionsAndOperators[i].Contains(comparerType));
            comparer = ComparerHelpers.ComparerTypes[comparerId];
            stringValues = ConditionsAndOperators[i].Split(comparer, StringSplitOptions.RemoveEmptyEntries);
            conditionsList.Add(new Condition(stringValues[0], comparer, stringValues[1], logicalOperator));
        }

        return conditionsList;
    }

    public static Dictionary<string, string> KeyValueParser(string keyValueString)
    {
        var output = new Dictionary<string, string>();
        
        var subStrings = keyValueString.Split(',');
        for (int i = 0; i < subStrings.Length; i++)
        {
            subStrings[i] = subStrings[i].Trim();
        }

        foreach (var subString in subStrings)
        {
            var keyVal = subString.Split('=');
            output.Add(keyVal[0].Trim(), keyVal[1].Trim());
        }
        return output;
    }
}

public class DisplayQuery
{
    public string ObjectClass { get; set; }
    public string[] ObjectFields { get; set; }
    public List<Condition> Conditions = null;
    public DisplayQuery(string command)
    {
        // basic edit of the command
        command = command.ToLower();
        command = command.Trim();
        
        // breaking command into little parts
        var stringSeparators = new string[] { "display", "from", "where" };
        var subCommands = command.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < subCommands.Length; i++)
        {
            subCommands[i] = subCommands[i].Trim();
        }
        
        // extracting data
        var fieldSeparators = new string[] { " ", "," };
        if (subCommands.Length >= 2)
        {
            ObjectFields = subCommands[0].Split(fieldSeparators, StringSplitOptions.RemoveEmptyEntries);
            ObjectClass = subCommands[1].Trim();
        }
        if (subCommands.Length == 3)
        {
            Conditions = QueryHelper.ConditionParser(subCommands[2]);
        }
    }
}

public class DeleteQuery
{
    public string ObjectClass { get; set; }
    public List<Condition> Conditions = new();
    public DeleteQuery(string command)
    {
        // basic edit of the command
        command = command.ToLower();
        command = command.Trim();
        
        // breaking command into little parts
        var stringSeparators = new string[] { "delete", "where" };
        var subCommands = command.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < subCommands.Length; i++)
        {
            subCommands[i] = subCommands[i].Trim();
        }
        
        // extracting data
        if (subCommands.Length >= 1)
        {
            ObjectClass = subCommands[0].Trim();
        }
        if (subCommands.Length == 2)
        {
            Conditions = QueryHelper.ConditionParser(subCommands[1]);
        }
    }
}

public class AddQuery
{
    public string ObjectClass { get; set; }
    public Dictionary<string, string> KeyValueDict = null;
    public static Dictionary<string, string> ClassToFactoryCode = new()
    {
        { "airport", "AI" },
        { "cargoplane", "CP" },
        { "passengerplane", "PP" },
        { "cargo", "CA" },
        { "crew", "C" },
        { "passenger", "P" },
        { "flight", "FL" }
    };

    public AddQuery(string command)
    {
        // basic edit of the command
        command = command.ToLower();
        command = command.Trim();
        
        // breaking command into little parts
        var stringSeparators = new string[] { "add", "new" };
        var subCommands = command.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < subCommands.Length; i++)
        {
            subCommands[i] = subCommands[i].Trim();
        }
        
        if (subCommands.Length == 2)
        {
            ObjectClass = subCommands[0];
            KeyValueDict = QueryHelper.KeyValueParser(subCommands[1].Substring(1, subCommands[1].Length - 2));
        }
    }
}

public class UpdateQuery
{
    public string ObjectClass { get; set; }
    public Dictionary<string, string> KeyValueDict = null;
    public List<Condition> Conditions = null;
    public UpdateQuery(string command)
    {
        // basic edit of the command
        command = command.ToLower();
        command = command.Trim();
        
        // breaking command into little parts
        var stringSeparators = new string[] { "update", "set", "where" };
        var subCommands = command.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < subCommands.Length; i++)
        {
            subCommands[i] = subCommands[i].Trim();
        }
        
        // extracting data
        if (subCommands.Length >= 2)
        {
            ObjectClass = subCommands[0].Trim();
            KeyValueDict = QueryHelper.KeyValueParser(subCommands[1].Substring(1, subCommands[1].Length - 2));
        }
        if (subCommands.Length == 3)
        {
            Conditions = QueryHelper.ConditionParser(subCommands[2]);
        }
    }
}
