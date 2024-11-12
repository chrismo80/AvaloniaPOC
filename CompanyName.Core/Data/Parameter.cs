using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyName.Core.Data;

public class Parameter
{
    [Key, Column(Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Operation { get; private set; }

    [Key, Column(Order = 1)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Product { get; private set; }

    [Key, Column(Order = 2)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Category { get; private set; }

    [Key, Column(Order = 3)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Name { get; private set; }

    public string Description { get; private set; }

    public double? Max { get; private set; }

    public double? Min { get; private set; }

    public double? Target { get; private set; }

    public string Unit { get; private set; }

    public Parameter(
        string operation, string product, string category, string name,
        string description, string unit,
        double? min, double? target, double? max)
    {
        (Operation, Product, Category, Name) = (operation, product, category, name);
        (Description, Unit) = (description, unit);
        (Min, Target, Max) = (min, target, max);

        if (new string[] { operation, product, category, name }.Any(string.IsNullOrEmpty))
            throw new ArgumentException($"Not all key defined ({operation}, {product}, {category}, {name})!");
    }
}