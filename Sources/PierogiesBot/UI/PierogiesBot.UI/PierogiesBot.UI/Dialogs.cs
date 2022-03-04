using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.LogicalTree;

namespace PierogiesBot.UI;

public static class Dialogs
{
    public static void ShowMessageBox(this ILogical @this, string message, string title, Action okAction)
    {
        // To be implemented
    }
    public static void ShowDialog(this ILogical @this, string message, Action okAction, Action cancelAction)
    {
        // To be implemented
    }
    
    public static IObservable<Unit> ShowMessageBoxAndWait(this ILogical @this, string message, string title, Action onClose)
    {
        // To be implemented
        return Observable.Return(Unit.Default);
    }
    
    
    public static IObservable<Unit> ShowDialogAndWait(this ILogical @this, string message, Action okAction, Action cancelAction, string okButtonContent = "Ok", string cancelButtonContent = "Cancel")
    {
        // To be implemented
        return Observable.Return(Unit.Default);
    }
    
} 