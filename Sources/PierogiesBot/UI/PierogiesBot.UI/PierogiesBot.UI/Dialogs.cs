using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.Media;
using Avalonia.VisualTree;
using FluentAvalonia.UI.Controls;
using Modalonia;
using PierogiesBot.UI.Views;
using Shared.UI.IoC;

namespace PierogiesBot.UI;

public static class Dialogs
{
    public static async Task ShowMessageBox(this IVisual @this, string message, string title)
    {
        
        var content = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Text         = message
        };

        var result = await Modal.Show(title, content, ModalButtons.Ok);
        if (result == ModalResult.Yes)
        {
            // do something..
        }
    }
    public static async Task ShowDialog(this IVisual @this, string message, Action okAction, Action cancelAction)
    {
        var content = new TextBlock
        {
            TextWrapping = TextWrapping.Wrap,
            Text         = message
        };

        var result = await Modal.Show("Confirm", content, ModalButtons.YesNo);
        if (result == ModalResult.Yes)
        {
            okAction();
            return;
        }
        cancelAction();
    }
    
} 