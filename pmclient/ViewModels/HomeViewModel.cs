using pmclient.Models;
using pmclient.Services;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class HomeViewModel : ViewModelBase, IRoutableViewModel
{
    private User _user;

    public User User
    {
        get => _user;
        set => this.RaiseAndSetIfChanged(ref _user, value);
    }
    
    public string UrlPathSegment => "/home";
    public IScreen HostScreen { get; }

    public HomeViewModel(IScreen? hostScreen = null)
    {
        User = StaticStorage.User!;
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
    }
}