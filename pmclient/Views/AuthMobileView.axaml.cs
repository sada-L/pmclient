using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class AuthMobileView : ReactiveUserControl<AuthViewModel>
{
    public AuthMobileView()
    {
        InitializeComponent();
    }
}