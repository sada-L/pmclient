using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class RegisterMobileView : ReactiveUserControl<RegisterViewModel>
{
    public RegisterMobileView()
    {
        InitializeComponent();
    }
}