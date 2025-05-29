using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class HeaderGroupView : ReactiveUserControl<HeaderGroupViewModel>
{
    public HeaderGroupView()
    {
        InitializeComponent();
    }
}