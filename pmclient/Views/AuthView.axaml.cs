using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class AuthView : ReactiveUserControl<AuthViewModel>
{
    public AuthView()
    {
        InitializeComponent();
    }
}