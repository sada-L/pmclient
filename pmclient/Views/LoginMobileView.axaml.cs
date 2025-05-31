using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class LoginMobileView : ReactiveUserControl<LoginViewModel>
{
    public LoginMobileView()
    {
        InitializeComponent();
    }
}