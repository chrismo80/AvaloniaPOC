namespace CompanyName.Core.Data;

public interface IParameterManager
{
    bool Has(string operation, string product, string category, string name);

    Parameter? Read(string operation, string product, string category, string name);

    IEnumerable<Parameter> Read(string operation, string product, string category);
}