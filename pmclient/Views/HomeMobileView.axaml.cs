using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class HomeMobileView : ReactiveUserControl<HomeViewModel>
{
    public HomeMobileView()
    {
        InitializeComponent();
    }
}