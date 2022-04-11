using Avalonia.ReactiveUI;
using Avalonia.Web.Blazor;

namespace TheDungeonGuide.UI.Web;

public partial class App
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        WebAppBuilder.Configure<TheDungeonGuide.UI.App>()
                     .UseReactiveUI()
                     .SetupWithSingleViewLifetime();
    }
}