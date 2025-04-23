using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class GroupListView : ReactiveUserControl<GroupViewModel>
{
    public GroupListView()
    {
        InitializeComponent();
    }
}