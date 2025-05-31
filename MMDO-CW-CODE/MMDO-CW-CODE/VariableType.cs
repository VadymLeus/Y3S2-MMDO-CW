using System;

public enum VariableType
{
    Original,
    Slack,
    Surplus,
    Artificial
}

public class VariableInfo
{
    public string Name { get; }
    public VariableType Type { get; }
    public int Index { get; }
    public double OriginalCost { get; }

    public VariableInfo(string name, VariableType type, int index, double originalCost = 0)
    {
        Name = name;
        Type = type;
        Index = index;
        OriginalCost = originalCost;
    }

    public double GetEffectiveCost(double mValue, bool isMin)
    {
        switch (Type)
        {
            case VariableType.Original:
                return OriginalCost;
            case VariableType.Slack:
            case VariableType.Surplus:
                return 0;
            case VariableType.Artificial:
                return (isMin ? 1.0 : -1.0) * mValue;
            default:
                throw new InvalidOperationException("Невідомий тип змінної");
        }
    }

    public string GetSymbolicCost(bool isMin)
    {
        switch (Type)
        {
            case VariableType.Original:
                return OriginalCost.ToString("G");
            case VariableType.Slack:
            case VariableType.Surplus:
                return "0";
            case VariableType.Artificial:
                return isMin ? "M" : "-M";
            default:
                return "?";
        }
    }
}