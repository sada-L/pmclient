using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Windows.Input;
using ReactiveUI;
using Group = pmclient.Models.Group;

namespace pmclient.ViewModels;

public class GroupViewModel : ViewModelBase
{
    private Group _group;
    private int _id;
    private string _title;
    private string _image;
    private int _groupId;
    private ObservableCollection<GroupViewModel> _subGroups;
    private ObservableCollection<CardViewModel> _cards;
    private bool _isExpanded;

    public int Id
    {
        get => _id;
        set => this.RaiseAndSetIfChanged(ref _id, value);
    }

    public string Title
    {
        get => _title;
        set => this.RaiseAndSetIfChanged(ref _title, value);
    }

    public string Image
    {
        get => _image;
        set => this.RaiseAndSetIfChanged(ref _image, value);
    }

    public int GroupId
    {
        get => _groupId;
        set => this.RaiseAndSetIfChanged(ref _groupId, value);
    }

    public bool IsExpanded
    {
        get => _isExpanded;
        set => this.RaiseAndSetIfChanged(ref _isExpanded, value);
    }

    public ObservableCollection<CardViewModel> Cards
    {
        get => _cards;
        set => this.RaiseAndSetIfChanged(ref _cards, value);
    }

    public ObservableCollection<GroupViewModel> SubGroups
    {
        get => _subGroups;
        set => this.RaiseAndSetIfChanged(ref _subGroups, value);
    }

    public ICommand AddGroupCommand { get; }

    public GroupViewModel()
    {
        Cards = new ObservableCollection<CardViewModel>();
        SubGroups = new ObservableCollection<GroupViewModel>();
    }

    public GroupViewModel(Group group)
    {
        _group = group;
        Id = group.Id;
        Title = group.Title;
        GroupId = group.GroupId;
        Image = Regex.Unescape(group.Image);
    }
}