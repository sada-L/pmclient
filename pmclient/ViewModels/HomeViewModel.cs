using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class HomeViewModel : ViewModelBase, IRoutableViewModel
{
    public string UrlPathSegment => "/home";
    public IScreen HostScreen { get; }

    public HomeViewModel(IScreen? hostScreen = null)
    {
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
    }
}