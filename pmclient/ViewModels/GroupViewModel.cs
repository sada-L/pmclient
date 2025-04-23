using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using pmclient.Models;
using ReactiveUI;

namespace pmclient.ViewModels;

public class GroupViewModel : ViewModelBase
{
    private Group _group;
    private string _title;
    private string _image;
    private ObservableCollection<GroupViewModel> _subGroups;
    private ObservableCollection<CardViewModel> _cards;
    private bool _isExpanded; 

    public int Id { get; init; }
    
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
        
    }
    
    public GroupViewModel(Group group, List<Card> cards, ICommand addGroupCommand, List<Group>? groups = null)
    {
        _group = group;
        Title = group.Title;
        Id = group.Id;
        Image = group.Image;
        AddGroupCommand = addGroupCommand;
        
        if (group.Id == 0)
            Cards = new ObservableCollection<CardViewModel>(cards
                .Select(x => new CardViewModel(x)));
        else
            Cards = new ObservableCollection<CardViewModel>(cards
                .Where(x => x.GroupId == group.Id)
                .Select(x => new CardViewModel(x)));

        if (groups != null)
            SubGroups = new ObservableCollection<GroupViewModel>(groups
                .Where(x => x.GroupId == group.Id)
                .Select(x => new GroupViewModel(x, cards, addGroupCommand, groups)));
    }
}