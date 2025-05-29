using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class GroupView : ReactiveUserControl<GroupViewModel>
{
    public GroupView()
    {
        InitializeComponent();
    }
}