using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using pmclient.Helpers;
using pmclient.Models;
using ReactiveUI;

namespace pmclient.ViewModels;

public class CardViewModel : ViewModelBase
{
    private Card _card;
    private int _id;
    private string _title;
    private string _username;
    private string _password;
    private string _website;
    private string _notes;
    private char _image;
    private int _groupId;
    private bool _isFavorite;
    private bool _isEnable;

    private GroupViewModel _headerGroup = new GroupViewModel();
    private GroupViewModel _currentGroup = new GroupViewModel();
    private ObservableCollection<GroupViewModel> _headerGroups = new ObservableCollection<GroupViewModel>();
    private ObservableCollection<GroupViewModel> _currentGroups = new ObservableCollection<GroupViewModel>();

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

    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public string Website
    {
        get => _website;
        set => this.RaiseAndSetIfChanged(ref _website, value);
    }

    public string Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    public int GroupId
    {
        get => _groupId;
        set => this.RaiseAndSetIfChanged(ref _groupId, value);
    }

    public bool IsFavorite
    {
        get => _isFavorite;
        set => this.RaiseAndSetIfChanged(ref _isFavorite, value);
    }

    public char Image
    {
        get => _image;
        set => this.RaiseAndSetIfChanged(ref _image, value);
    }

    public bool IsEnable
    {
        get => _isEnable;
        set => this.RaiseAndSetIfChanged(ref _isEnable, value);
    }

    public GroupViewModel HeaderGroup
    {
        get => _headerGroup;
        set => this.RaiseAndSetIfChanged(ref _headerGroup, value);
    }

    public GroupViewModel CurrentGroup
    {
        get => _currentGroup;
        set => this.RaiseAndSetIfChanged(ref _currentGroup, value);
    }

    public ObservableCollection<GroupViewModel> HeaderGroups
    {
        get => _headerGroups;
        set => this.RaiseAndSetIfChanged(ref _headerGroups, value);
    }

    public ObservableCollection<GroupViewModel> CurrentGroups
    {
        get => _currentGroups;
        set => this.RaiseAndSetIfChanged(ref _currentGroups, value);
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

    public ICommand DeleteCommand { get; set; }

    public ICommand SaveCommand { get; set; }

    public ICommand FavoriteCommand { get; set; }

    public ICommand CancelCommand { get; set; }

    public ICommand EditCommand { get; }
    
    public ICommand GeneratePasswordCommand { get; }

    public CardViewModel()
    {
        var card = new Card
        {
            Id = 0,
            Title = "Title",
            Website = "www.site.com",
            Image = '\uf2bc',
            Username = "Username",
            Notes = "Notes",
            Password = "Password",
            GroupId = 1,
            IsFavorite = false,
        };

        SetData(card);
        EditCommand = ReactiveCommand.Create(Edit);
        FavoriteCommand = ReactiveCommand.Create(Favorite);
    }

    public CardViewModel(Card card)
    {
        SetData(card);
        EditCommand = ReactiveCommand.Create(Edit);
        FavoriteCommand = ReactiveCommand.Create(Favorite);
        GeneratePasswordCommand = ReactiveCommand.Create(GeneratePassword);
    }

    public Card GetCard()
    {
        return _card;
    }

    private void GeneratePassword()
    {
        Password = PasswordGenerator.GenerateSecurePassword();
    }

    private void SetData(Card card)
    {
        _card = card;
        Id = card.Id;
        Title = card.Title;
        Website = card.Website;
        Username = card.Username;
        Notes = card.Notes;
        Password = card.Password;
        GroupId = card.GroupId;
        IsFavorite = card.IsFavorite;
        Image = card.Image;
    }

    private void Edit()
    {
        IsEnable = true;
    }

    public void Save()
    {
        var card = new Card
        {
            Id = Id,
            Title = Title,
            Username = Username,
            Website = Website,
            Password = Password,
            GroupId = GroupId,
            IsFavorite = IsFavorite,
            Image = Image,
            Notes = Notes,
        };
        SetData(card);
        IsEnable = false;
    }

    public void Cancel()
    {
        SetData(_card);
        IsEnable = false;
    }

    public void Favorite()
    {
        IsFavorite = !IsFavorite;
    }
}