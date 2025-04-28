using System.Collections.Generic;
using System.Reactive;
using System.Text.RegularExpressions;
using System.Windows.Input;
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
    private string _image;
    private int _groupId;
    private bool _isFavorite;
    private bool _isEnabled;

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

    public string Image
    {
        get => _image;
        set => this.RaiseAndSetIfChanged(ref _image, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
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

    public ReactiveCommand<Unit, Unit> DeleteCommand { get; set; }

    public ICommand ConfirmCommand { get; set; }

    public ReactiveCommand<Unit, Unit> FavoriteCommand { get; }

    public ReactiveCommand<Unit, Unit> EditCommand { get; }

    public ReactiveCommand<Unit, Unit> SaveCommand { get; }

    public ReactiveCommand<Unit, Unit> CancelCommand { get; }

    public CardViewModel()
    {
        var card = new Card
        {
            Id = 0,
            Title = "Title",
            Website = "www.site.com",
            Image = "\uf2bc",
            Username = "Username",
            Notes = "Notes",
            Password = "Password",
            GroupId = 1,
            IsFavorite = false,
        };

        SetData(card);
        EditCommand = ReactiveCommand.Create(Edit);
        SaveCommand = ReactiveCommand.Create(Save);
        CancelCommand = ReactiveCommand.Create(Cancel);
    }

    public CardViewModel(Card card)
    {
        SetData(card);
        EditCommand = ReactiveCommand.Create(Edit);
        SaveCommand = ReactiveCommand.Create(Save);
        CancelCommand = ReactiveCommand.Create(Cancel);
        FavoriteCommand = ReactiveCommand.Create(Favorite);
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
        Image = Regex.Unescape(card.Image);
    }

    private void Edit()
    {
        IsEnabled = !IsEnabled;
    }

    private void Save()
    {
        var card = new Card
        {
            Id = _card.Id,
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
        IsEnabled = !IsEnabled;
        ConfirmCommand.Execute(true);
    }

    private void Cancel()
    {
        SetData(_card);
        IsEnabled = !IsEnabled;
        ConfirmCommand.Execute(false);
    }

    private void Favorite()
    {
        IsFavorite = !IsFavorite;
        ConfirmCommand.Execute(true);
    }
}