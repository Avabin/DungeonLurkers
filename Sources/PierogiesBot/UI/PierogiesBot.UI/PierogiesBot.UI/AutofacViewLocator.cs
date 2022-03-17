using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;
using Shared.UI.IoC;

#nullable enable
namespace PierogiesBot.UI;

public class AutofacViewLocator : IDataTemplate, IViewLocator
{
    public IViewFor? ResolveView<T>(T? viewModel, string? contract = null)
    {
        var iViewForType = typeof(IViewFor<>).MakeGenericType(viewModel!.GetType());
        var view         = (IViewFor?) ServiceLocator.GetService(iViewForType);
        return view;
    }

    public IControl Build(object param)
    {
        // throw new NotImplementedException();
        var viewType = param.GetType();
        // if (viewType.IsGenericType)
        // {
        //     if (viewType.IsAssignableFrom(typeof(IViewFor<>)))
        //     {
        //         var genericType = viewType.GetGenericTypeDefinition();
        //         var viewModel   = ServiceLocator.GetService(genericType);
        //         return (Control) ServiceLocator.GetService(type)!;
        //     }
        // }
        var name = viewType.Name!.Replace("ViewModel", "View");
        var type = Type.GetType($"PierogiesBot.UI.Views.{name}");

        if (type != null)
        {
            return (Control) ServiceLocator.GetService(type)!;
        }
        else
        {
            return new TextBlock { Text = "Not Found: " + name };
        }
    }

    public bool Match(object data)
    {
        return data is ReactiveObject;
    }
}