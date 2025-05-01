using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
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
    private bool _isEnable;
    private bool _isVisible;

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

    public bool IsEnable
    {
        get => _isEnable;
        set => this.RaiseAndSetIfChanged(ref _isEnable, value);
    }

    public bool IsVisible
    {
        get => _isVisible;
        set => this.RaiseAndSetIfChanged(ref _isVisible, value);
    }

    public List<string> Images { get; } =
    [
        "\uf1c5",
        "\uf2ba",
        "\uf2bc",
        "\uf097",
        "\uf274",
        "\uf2c3",
        "\uf015",
        "\uf114",
        "\uf03e"
    ];

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

    public ICommand ConfirmCommand { get; set; }

    public ICommand AddSubGroupCommand { get; set; }

    public ICommand DeleteCommand { get; set; }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

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
        Cards = new ObservableCollection<CardViewModel>();
        SubGroups = new ObservableCollection<GroupViewModel>();
        IsVisible = Id > 0 && GroupId == 0;

        SaveCommand = ReactiveCommand.Create(Save);
        CancelCommand = ReactiveCommand.Create(Cancel);
    }

    public Group GetGroup()
    {
        return _group;
    }

    private void SetData(Group group)
    {
        _group = group;
        Id = group.Id;
        Title = group.Title;
        GroupId = group.GroupId;
        Image = Regex.Unescape(group.Image);
    }

    private void Save()
    {
        ConfirmCommand.Execute(true);
        var group = new Group
        {
            Id = Id,
            Title = Title,
            GroupId = GroupId,
            Image = Image,
        };
        SetData(group);
        IsEnable = !IsEnable;
    }

    private void Cancel()
    {
        SetData(_group);
        IsEnable = !IsEnable;
        ConfirmCommand.Execute(false);
    }
}