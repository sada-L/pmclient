using System.Reactive;
using Avalonia.Media.Imaging;
using pmclient.Models;
using pmclient.Views;
using ReactiveUI;

namespace pmclient.ViewModels;

public class CardViewModel : ViewModelBase
{
    private Card _card;
    private string _title;
    private string _username;
    private string _password;
    private string _url;
    private string _notes;  
    private string _image;
    private int? _groupId; 
    private bool _isFavorite;
    private bool _isEnabled;
    
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

    public string Url
    {
        get => _url;
        set => this.RaiseAndSetIfChanged(ref _url, value);
    }

    public string Notes
    {
        get => _notes;
        set => this.RaiseAndSetIfChanged(ref _notes, value);
    }

    public int? GroupId
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
    public ReactiveCommand<Unit, Unit> EditCommand { get; }
    
    public CardViewModel()
    {
        _card = new Card
        {
            Id = 1,
            Title = "Title",
            Url = "www.site.com",
            Image = "\uf2bc",
            Username = "Username",
            Notes = "Notes",
            Password = "Password",
            GroupId = 1,
            IsFavorite = true,
        };
        Title = _card.Title;
        Url = _card.Url;
        Username = _card.Username;
        Notes = _card.Notes;
        Password = _card.Password;
        GroupId = _card.GroupId;
        IsFavorite = _card.IsFavorite;
        Image = _card.Image;
        IsEnabled = false;

        EditCommand = ReactiveCommand.Create(() => { IsEnabled = !IsEnabled; });
    }
    
    public CardViewModel(Card card)
    {
        _card = card;
        Title = _card.Title;
        Url = _card.Url;
        Username = _card.Username;
        Notes = _card.Notes;
        Password = _card.Password;
        GroupId = _card.GroupId;
        IsFavorite = _card.IsFavorite;
        Image = _card.Image;
        
        EditCommand = ReactiveCommand.Create(() => { IsEnabled = !IsEnabled; });
    }
}