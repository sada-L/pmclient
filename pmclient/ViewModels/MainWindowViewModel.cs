using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    public RoutingState Router { get; } = new RoutingState();

    public MainWindowViewModel()
    {
        Router.Navigate.Execute(new LoginViewModel(null, null,this));
    }
}