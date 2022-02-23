using Microsoft.Extensions.DependencyInjection;
using ReactiveUI;

namespace Shared.UI.Navigation.RoutableViewModel;

public class RoutableViewModelFactory : IRoutableViewModelFactory
{
    private readonly IServiceProvider _provider;

    public RoutableViewModelFactory(IServiceProvider provider)
    {
        _provider = provider;
    }
    public T      GetViewModel<T>() where T : IRoutableViewModel => _provider.GetRequiredService<T>();
    public object GetViewModel(Type viewModelType)
    {
        var viewModel = _provider.GetRequiredService(viewModelType);
        return viewModel;
    }
}