namespace CompanyName.Core.Data;

public struct ParameterMatch(string name, double min, double max, double value, string unit)
{
    public string Name { get; private set; } = name;

    public double Min { get; private set; } = min;

    public double Max { get; private set; } = max;

    public double Value { get; private set; } = value;

    public string Unit { get; private set; } = unit;

    public readonly bool Success => Value >= Min && Value <= Max;
}