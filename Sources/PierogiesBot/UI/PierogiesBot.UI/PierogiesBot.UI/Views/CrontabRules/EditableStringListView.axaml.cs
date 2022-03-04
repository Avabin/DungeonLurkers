using System.Collections.Generic;
using System.Reactive;
using System.Windows.Input;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using ReactiveUI;

namespace PierogiesBot.UI.Views.CrontabRules;

public partial class EditableStringListView : UserControl
{
    public static readonly DirectProperty<EditableStringListView, IEnumerable<string>> ItemsProperty =
        AvaloniaProperty.RegisterDirect<EditableStringListView, IEnumerable<string>>(
         nameof(Items),
         o => o.Items,
         (o, v) => o.Items = v);

    private IEnumerable<string> _items = new AvaloniaList<string>();

    public IEnumerable<string> Items
    {
        get => _items;
        set => SetAndRaise(ItemsProperty, ref _items, value);
    }

    public static readonly DirectProperty<EditableStringListView, string> TitleProperty =
        AvaloniaProperty.RegisterDirect<EditableStringListView, string>(
                                                                        nameof(Title),
                                                                        o => o.Title,
                                                                        (o, v) => o.Title = v);

    private string _title = "";

    public string Title
    {
        get => _title;
        set => SetAndRaise(TitleProperty, ref _title, value);
    }
    
    public static readonly DirectProperty<EditableStringListView, ReactiveCommand<string, Unit>> AddElementCommandProperty =
        AvaloniaProperty.RegisterDirect<EditableStringListView, ReactiveCommand<string, Unit>>(
                                                                          nameof(AddElementCommand),
                                                                          o => o.AddElementCommand,
                                                                          (o, v) => o.AddElementCommand = v);
    
    private ReactiveCommand<string, Unit> _addElementCommand;

    public ReactiveCommand<string, Unit> AddElementCommand
    {
        get => _addElementCommand;
        set => SetAndRaise(AddElementCommandProperty, ref _addElementCommand, value);
    }
    
    public static readonly DirectProperty<EditableStringListView, ReactiveCommand<string, Unit>> RemoveElementCommandProperty =
        AvaloniaProperty.RegisterDirect<EditableStringListView, ReactiveCommand<string, Unit>>(
                                                                          nameof(RemoveElementCommand),
                                                                          o => o.RemoveElementCommand,
                                                                          (o, v) => o.RemoveElementCommand = v);
    
    private ReactiveCommand<string, Unit> _removeElementCommand;

    public ReactiveCommand<string, Unit> RemoveElementCommand
    {
        get => _removeElementCommand;
        set => SetAndRaise(RemoveElementCommandProperty, ref _removeElementCommand, value);
    }

    public EditableStringListView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}