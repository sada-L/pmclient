using pmclient.Models;
using ReactiveUI;

namespace pmclient.ViewModels;

public class GroupViewModel : ViewModelBase
{
    private Group _group;
    private string _name;

    public int Id { get; init; }
    public string Name
    {
       get => _name;
       set => this.RaiseAndSetIfChanged(ref _name, value);
    }

    public GroupViewModel()
    {
        _group = new Group{Name = "Group", Id = 1};
        Name = _group.Name;
        Id = _group.Id;
    }
    
    public GroupViewModel(Group group)
    {
        _group = group;
        Name = group.Name;
        Id = group.Id;
    }
}