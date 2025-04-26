using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using pmclient.Helpers;
using pmclient.Models;
using pmclient.RefitClients;
using pmclient.Services;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class HomeViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly ICardWebApi? _cardWebApi;
    private readonly IGroupWebApi? _groupWebApi;

    private CardViewModel _selectedCard;
    private GroupViewModel _selectedGroup;
    private GroupViewModel _favoriteCards;
    private GroupViewModel _allCards;
    private GroupViewModel _deletedCards;
    private int _selectedCardIndex;
    private ObservableCollection<CardViewModel> _cardViewModels;
    private ObservableCollection<GroupViewModel> _groupViewModels;
    private string _errorMessage;

    public CardViewModel SelectedCard
    {
        get => _selectedCard;
        set => this.RaiseAndSetIfChanged(ref _selectedCard, value);
    }

    public GroupViewModel SelectedGroup
    {
        get => _selectedGroup;
        set => this.RaiseAndSetIfChanged(ref _selectedGroup, value);
    }

    public ObservableCollection<CardViewModel> Cards
    {
        get => _cardViewModels;
        set => this.RaiseAndSetIfChanged(ref _cardViewModels, value);
    }

    public ObservableCollection<GroupViewModel> Groups
    {
        get => _groupViewModels;
        set => this.RaiseAndSetIfChanged(ref _groupViewModels, value);
    }

    public int SelectedCardIndex
    {
        get => _selectedCardIndex;
        set => this.RaiseAndSetIfChanged(ref _selectedCardIndex, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public ReactiveCommand<Unit, Unit> LoadDataCommand { get; }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => "/home";

    public HomeViewModel()
    {
        var cards = new List<Card>()
        {
            new Card()
            {
                Id = 1, Title = "card1", Username = "username", Image = "\uf2bc", Website = "www.card.com",
                Password = "123456", Notes = "notes", GroupId = 1, IsFavorite = true,
            },
            new Card()
            {
                Id = 2, Title = "card2", Username = "username", Image = "\uf2bc", Website = "www.card.com",
                Password = "123456", Notes = "notes", GroupId = 1, IsFavorite = false,
            },
            new Card()
            {
                Id = 3, Title = "card3", Username = "username", Image = "\uf2bc", Website = "www.card.com",
                Password = "123456", Notes = "notes", GroupId = 2, IsFavorite = false,
            },
        };

        var groups = new List<Group>()
        {
            new Group() { Id = 1, Title = "Group1", Image = "\uf02e", GroupId = 0 },
            new Group() { Id = 2, Title = "group2", Image = "\uf02e", GroupId = 1 },
            new Group() { Id = 3, Title = "group3", Image = "\uf02e", GroupId = 1 },
            new Group() { Id = 4, Title = "Group4", Image = "\uf02e", GroupId = 0 },
            new Group() { Id = 5, Title = "group5", Image = "\uf02e", GroupId = 4 },
        };

        SetData(cards, groups);
    }

    public HomeViewModel(IScreen? hostScreen = null, ICardWebApi? cardWebApi = null, IGroupWebApi? groupWebApi = null)
    {
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
        _cardWebApi = cardWebApi ?? Locator.Current.GetService<ICardWebApi>()!;
        _groupWebApi = groupWebApi ?? Locator.Current.GetService<IGroupWebApi>()!;

        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
        LoadDataCommand.Execute().Subscribe();
    }

    private void SetData(List<Card> cards, List<Group> groups)
    {
        _allCards = new GroupViewModel(new Group()
        {
            Id = -1,
            Title = "All Items",
            Image = "\uf2ba",
            GroupId = 0
        }, cards, null);

        _favoriteCards = new GroupViewModel(new Group()
        {
            Id = -1,
            Title = "Favorites",
            Image = "\uf006",
            GroupId = 0
        }, cards.Where(x => x.IsFavorite).ToList(), null);

        _deletedCards = new GroupViewModel(new Group()
        {
            Id = -1,
            Title = "Recently Deleted",
            Image = "\uf014",
            GroupId = 0
        }, new List<Card>(), null);

        Groups = [_allCards, _favoriteCards];
        
        Groups.AddRange(groups
            .Where(x => x.GroupId == 0)
            .Select(x => new GroupViewModel(x, cards, null, groups)).ToList());
        
        Groups.Add(_deletedCards);

        SelectedGroup = Groups.FirstOrDefault() ?? _allCards;
        
        Cards = new ObservableCollection<CardViewModel>(SelectedGroup.Cards);
        
        this.WhenAnyValue(x => x.SelectedGroup)
            .Subscribe(x => SelectedCardIndex = 0);

        this.WhenAnyValue(x => x.SelectedGroup.Cards)
            .BindTo(this, x => x.Cards);
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var cardResponse = await _cardWebApi!.GetCardByUser(cancellationToken);
            if (!cardResponse.IsSuccessStatusCode)
            {
                ErrorMessage = "error";
                return;
            }

            var cards = cardResponse.Content ?? new List<Card>();

            var groupResponse = await _groupWebApi!.GetGroupsByUser(cancellationToken);
            if (!groupResponse.IsSuccessStatusCode)
            {
                ErrorMessage = "error";
                return;
            }

            var groups = groupResponse.Content ?? new List<Group>();

            SetData(cards, groups);
        }
        catch (Exception e)
        {
            ErrorMessage = "error";
            return;
        }
    }
}