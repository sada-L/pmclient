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
    
    public Bitmap Image => new ($"./Assets/{_card.Image}");
    
    public CardViewModel()
    {
        _card = new Card
        {
            Id = 1,
            Title = "Title",
            Url = "www.site.com",
            Image = "picture.png",
            Username = "Username",
            Notes = "Notes",
            Password = "Password",
        };
        Title = _card.Title;
        Url = _card.Url;
        Username = _card.Username;
        Notes = _card.Notes;
        Password = _card.Password;
    }
    
    public CardViewModel(Card card)
    {
        _card = card;
        Title = card.Title;
        Url = card.Url; 
    }
}