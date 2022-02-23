using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ReactiveUI;
using Shared.UI.IoC;

#nullable enable
namespace TheDungeonGuide.UI;

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
        var name = param.GetType().Name!.Replace("ViewModel", "View");
        var type = Type.GetType($"TheDungeonGuide.UI.Views.{name}");

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