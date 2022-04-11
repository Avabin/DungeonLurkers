using Avalonia.ReactiveUI;
using Avalonia.Web.Blazor;

namespace PierogiesBot.UI.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<PierogiesBot.UI.App>()
            .UseReactiveUI()
            .SetupWithSingleViewLifetime();
    }
}