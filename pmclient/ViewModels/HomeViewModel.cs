using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DynamicData;
using pmclient.Models;
using pmclient.RefitClients;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class HomeViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly ICardWebApi? _cardWebApi;
    private readonly IGroupWebApi? _groupWebApi;
    private List<CardViewModel> _userCards;
    private List<GroupViewModel> _userGroups;
    private List<CardViewModel> _newCards;

    private CardViewModel _selectedCard;
    private GroupViewModel _selectedGroup;
    private GroupViewModel _favorites;
    private GroupViewModel _allItems;
    private GroupViewModel _deleted;
    private ObservableCollection<CardViewModel> _currentCards;
    private ObservableCollection<GroupViewModel> _currentGroups;
    private string _errorMessage;
    private string _search;
    private int _selectedCardIndex;
    private bool _isEnabled;
    private bool _isAddEnabled;

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

    public ObservableCollection<CardViewModel> CurrentCards
    {
        get => _currentCards;
        set => this.RaiseAndSetIfChanged(ref _currentCards, value);
    }

    public ObservableCollection<GroupViewModel> CurrentGroups
    {
        get => _currentGroups;
        set => this.RaiseAndSetIfChanged(ref _currentGroups, value);
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

    public string Search
    {
        get => _search;
        set => this.RaiseAndSetIfChanged(ref _search, value);
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => this.RaiseAndSetIfChanged(ref _isEnabled, value);
    }

    public bool IsAddEnabled
    {
        get => _isAddEnabled;
        set => this.RaiseAndSetIfChanged(ref _isAddEnabled, value);
    }

    public ICommand LoadDataCommand { get; }

    public ICommand SortCommand { get; }

    public ICommand AddCardCommand { get; }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => "/home";

    public HomeViewModel()
    {
        var cards = new List<Card>()
        {
            new Card()
            {
                Id = 1, Title = "card1", Username = "username", Image = "\uf2bc", Website = "www.card.com",
                Password = "123456", Notes = "notes", GroupId = 2, IsFavorite = true,
            },
            new Card()
            {
                Id = 2, Title = "card2", Username = "username", Image = "\uf2bc", Website = "www.card.com",
                Password = "123456", Notes = "notes", GroupId = 2, IsFavorite = false,
            },
            new Card()
            {
                Id = 3, Title = "card3", Username = "username", Image = "\uf2bc", Website = "www.card.com",
                Password = "123456", Notes = "notes", GroupId = 2, IsFavorite = false,
            },
            new Card()
            {
                Id = 4, Title = "card4", Username = "username", Image = "\uf2bc", Website = "www.card.com",
                Password = "123456", Notes = "notes", GroupId = 3, IsFavorite = false,
            },
            new Card()
            {
                Id = 5, Title = "card5", Username = "username", Image = "\uf2bc", Website = "www.card.com",
                Password = "123456", Notes = "notes", GroupId = 3, IsFavorite = false,
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

        SortCommand = ReactiveCommand.Create<int>(SortCards);
        AddCardCommand = ReactiveCommand.Create(AddCard);
        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
        LoadDataCommand.Execute(null);
    }

    private void AddCard()
    {
        IsEnabled = false;

        var card = new Card
        {
            Id = 0,
            Title = "Title",
            Website = "www.site.com",
            Image = "\uf2bc",
            Username = "Username",
            Notes = "Notes",
            Password = "Password",
            GroupId = SelectedGroup.Id switch
            {
                -1 => 0,
                -2 => 0,
                -3 => 0,
                _ => SelectedGroup.Id
            },
            IsFavorite = SelectedGroup.Id switch
            {
                -2 => true,
                _ => false,
            },
        };

        var cardViewModel = new CardViewModel(card)
        {
            ConfirmCommand = ReactiveCommand.Create<bool>(ConfirmChanges),
            IsEnabled = true
        };
        _userCards.Add(cardViewModel);
        _newCards.Add(cardViewModel);

        SelectedGroup.Cards.Add(cardViewModel);
        SelectedCard = cardViewModel;
    }

    private void DeleteCard()
    {
        if (SelectedGroup.Id < 0) return;
    }

    private void ConfirmChanges(bool confirmed)
    {
        if (!confirmed)
            _userCards.Remove(SelectedCard);
        IsEnabled = true;
        SetHandlers();
    }

    private void SearchCards()
    {
        CurrentCards = !string.IsNullOrEmpty(Search)
            ? new ObservableCollection<CardViewModel>(SelectedGroup.Cards
                .Where(x => x.Title.Contains(Search, StringComparison.CurrentCultureIgnoreCase)).ToList())
            : new ObservableCollection<CardViewModel>(SelectedGroup.Cards);
    }

    private void SortCards(int index)
    {
        var sortList = index switch
        {
            0 => CurrentCards.OrderBy(x => x.Title).ToList(),
            1 => CurrentCards.OrderByDescending(x => x.Title).ToList(),
            3 => CurrentCards.OrderBy(x => x.Website).ToList(),
            4 => CurrentCards.OrderByDescending(x => x.Website).ToList(),
            5 => CurrentCards.OrderBy(x => x.Username).ToList(),
            6 => CurrentCards.OrderByDescending(x => x.Username).ToList(),
            _ => CurrentCards.ToList()
        };
        CurrentCards = new ObservableCollection<CardViewModel>(sortList);
    }

    private void SetList()
    {
        _allItems = new GroupViewModel(new Group
        {
            Id = -1,
            Title = "All Items",
            Image = "\uf2ba",
            GroupId = 0
        })
        {
            Cards = new ObservableCollection<CardViewModel>(_userCards)
        };

        _favorites = new GroupViewModel(new Group
        {
            Id = -2,
            Title = "Favorites",
            Image = "\uf006",
            GroupId = 0
        })
        {
            Cards = new ObservableCollection<CardViewModel>(_userCards.Where(x => x.IsFavorite))
        };

        _deleted = new GroupViewModel(new Group
        {
            Id = -3,
            Title = "Recently Deleted",
            Image = "\uf014",
            GroupId = 0
        })
        {
            Cards = new ObservableCollection<CardViewModel>(new List<CardViewModel>())
        };

        CurrentGroups = new ObservableCollection<GroupViewModel>();
        CurrentGroups.Add(_allItems);
        CurrentGroups.Add(_favorites);
        CurrentGroups.AddRange(_userGroups
            .Where(x => x.GroupId == 0));
        CurrentGroups.Add(_deleted);
    }

    private void SetProperties()
    {
        foreach (var card in _userCards)
        {
            card.DeleteCommand = ReactiveCommand.Create(DeleteCard);
            card.ConfirmCommand = ReactiveCommand.Create<bool>(ConfirmChanges);
        }

        foreach (var group in _userGroups)
        {
            if (group.Id < 0)
                group.Cards = new ObservableCollection<CardViewModel>(_userCards);
            else
                group.Cards = new ObservableCollection<CardViewModel>(_userCards
                    .Where(x => x.GroupId == group.Id));

            if (group.GroupId == 0)
                group.SubGroups =
                    new ObservableCollection<GroupViewModel>(_userGroups.Where(x => x.GroupId == group.Id));
        }

        SetList();
    }

    private void SetHandlers()
    {
        this.WhenAnyValue(x => x.SelectedGroup)
            .Subscribe(x => CurrentCards = SelectedGroup.Cards);

        this.WhenAnyValue(x => x.Search)
            .DistinctUntilChanged()
            .Subscribe(_ => SearchCards());

        this.WhenAnyValue(x => x.SelectedGroup)
            .Subscribe(x => IsAddEnabled = x.Id > 0);

        SetProperties();
    }

    private void SetData(List<Card> cards, List<Group> groups)
    {
        _newCards = new List<CardViewModel>();
        _selectedGroup = new GroupViewModel();
        _userCards = cards.Select(x => new CardViewModel(x)).ToList();
        _userGroups = groups.Select(x => new GroupViewModel(x)).ToList();

        IsEnabled = true;
        SetHandlers();
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