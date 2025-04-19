using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class LoginView : ReactiveUserControl<LoginViewModel>
{
    public LoginView()
    {
        InitializeComponent();
    }
}