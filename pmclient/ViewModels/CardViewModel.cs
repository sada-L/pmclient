using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using pmclient.Extensions;
using pmclient.Helpers;
using pmclient.Models;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace pmclient.ViewModels;

public class CardViewModel : ViewModelBase
{
    private Card _oldCard;

    public ReactiveCollection<HeaderGroupViewModel> HeaderGroups { get; } = new();

    public ReactiveCollection<GroupViewModel> CurrentGroups { get; } = new();

    [Reactive] public int Id { get; set; }

    [Reactive] public string Title { get; set; }

    [Reactive] public string Username { get; set; }

    [Reactive] public string Password { get; set; }

    [Reactive] public string Website { get; set; }

    [Reactive] public string Notes { get; set; }

    [Reactive] public int GroupId { get; set; }

    [Reactive] public bool IsFavorite { get; set; }

    [Reactive] public char Image { get; set; }

    [Reactive] public bool IsEnable { get; set; }

    [Reactive] public HeaderGroupViewModel HeaderGroup { get; set; }

    [Reactive] public GroupViewModel CurrentGroup { get; set; }

    public List<string> Images { get; } = ["\uf1c5", "\uf2ba", "\uf2bc", "\uf097", "\uf274", "\uf2c3"];

    public ICommand DeleteCommand { get; set; }

    public ICommand SaveCommand { get; set; }

    public ICommand FavoriteCommand { get; set; }

    public ICommand CancelCommand { get; set; }

    public ICommand EditCommand { get; set; }

    public ICommand GeneratePasswordCommand { get; }

    public CardViewModel()
    {
    }

    public CardViewModel(Card card)
    {
        SetData(card);
        GeneratePasswordCommand = ReactiveCommand.Create(GeneratePassword);
    }

    public Card GetCard() => _oldCard;

    private void GeneratePassword() => Password = PasswordGenerator.GenerateSecurePassword();

    private void SetData(Card newCard)
    {
        _oldCard = newCard;
        Id = newCard.Id;
        Title = newCard.Title;
        Website = newCard.Website;
        Username = newCard.Username;
        Notes = newCard.Notes;
        Password = newCard.Password;
        GroupId = newCard.GroupId;
        IsFavorite = newCard.IsFavorite;
        Image = newCard.Image;
    }

    public void Save()
    {
        var newCard = new Card
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
        SetData(newCard);
        IsEnable = false;
    }

    public void Cancel()
    {
        SetData(_oldCard);
        IsEnable = false;
    }
}

    public void Favorite()
    {
        IsFavorite = !IsFavorite;
    }
}