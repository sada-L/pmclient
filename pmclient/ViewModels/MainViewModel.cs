using ReactiveUI;

namespace pmclient.ViewModels;

public class MainViewModel : ViewModelBase, IScreen
{
    private RoutingState _router = new RoutingState();

    public RoutingState Router
    {
        get => _router;
        set => this.RaiseAndSetIfChanged(ref _router, value);
    }
}