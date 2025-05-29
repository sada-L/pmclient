using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class PinView : ReactiveUserControl<PinViewModel>
{
    public PinView()
    {
        InitializeComponent();
    }
}