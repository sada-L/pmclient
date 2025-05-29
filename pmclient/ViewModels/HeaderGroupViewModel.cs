using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using pmclient.Extensions;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Group = pmclient.Models.Group;

namespace pmclient.ViewModels;

public class HeaderGroupViewModel : GroupViewModel
{
    public ReactiveCollection<GroupViewModel> SubGroups { get; } = new();

    [Reactive] public bool IsVisible { get; set; }

    public ICommand AddSubGroupCommand { get; set; }

    public HeaderGroupViewModel()
    {
    }

    public HeaderGroupViewModel(Group group)
    {
        SetData(group);

        this.WhenAnyValue(x => x.Id)
            .Subscribe(x => IsVisible = x > 0 && GroupId == 0);
    }
}

public static class HeaderGroupViewModelExtensions
{
    public static HeaderGroupViewModel SetSubGroups(this HeaderGroupViewModel vm, List<GroupViewModel> groups)
    {
        vm.SubGroups.Replace(groups.Where(x => x.GroupId == vm.Id));
        return vm;
    }
}