using Microsoft.Extensions.Configuration;

namespace CompanyName.Core.Data;

public class ParameterManager : Manager, IParameterManager
{
    readonly List<Parameter> _parameters = Enumerable.Range(1, 8)
        .Select(i => new Parameter("O", "P", "C", $"Name{i}", "", "", null, i, null)).ToList();

    public ParameterManager(IConfiguration configuration)
    {
        var connectionString = configuration.GetSection("ConnectionStrings")["DataContext"];
    }

    public bool Has(string operation, string product, string category, string name) =>
        Read(operation, product, category, name) != null;

    public Parameter? Read(string operation, string product, string category, string name) =>
        _parameters.FirstOrDefault(p => MatchesCategory(p, operation, product, category) && MatchesName(p, name));

    public IEnumerable<Parameter> Read(string operation, string product, string category) =>
        _parameters.Where(p => MatchesCategory(p, operation, product, category)).ToList();

    private static bool MatchesCategory(Parameter parameter, string operation, string product, string category) =>
        parameter.Operation == operation && parameter.Product == product && parameter.Category == category;

    private static bool MatchesName(Parameter parameter, string name) =>
        parameter.Name == name;
}