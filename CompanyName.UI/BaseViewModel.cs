using CommunityToolkit.Mvvm.ComponentModel;

using System.Reflection;

using CompanyName.Core;
using CompanyName.Core.Logging;
using CompanyName.Core.Messages;

namespace CompanyName.UI;

public abstract class BaseViewModel : ObservableObject
{
    protected BaseViewModel()
    {
        this.Trace("Ctor");
    }

    /// <summary>
    /// Global error handler for view model methods<br/>
    /// Runs the method on a same thread (e.g. UI thread)<br/>
    /// If method is a task, it will be awaited<br/>
    /// Simple private void view model methods are enough<br/>
    /// Methods with default parameter values are supported
    /// </summary>
    public async Task Execute(string methodName)
    {
        try
        {
            // await if task method
            if (FindMethod(methodName).Invoke(this, []) is Task task)
                await task;
        }
        catch (Exception ex) { this.CreateMessage(ex.InnerException ?? ex); }
    }

    protected void Guard(bool condition, string message) => this.IsTrue(condition, message);

    private MethodInfo FindMethod(string name) => GetType()
        .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
        .Where(m => m.GetParameters().Length == 0 || m.GetParameters().All(p => p.HasDefaultValue))
        .First(m => m.Name == name);
}