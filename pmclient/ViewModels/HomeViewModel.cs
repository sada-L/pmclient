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
using pmclient.Services;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class HomeViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly CardService? _cardService;
    private readonly GroupService? _groupService;
    private List<CardViewModel> _userCards;
    private List<GroupViewModel> _userGroups;

    private CardViewModel _selectedCard;
    private GroupViewModel _selectedGroup;
    private GroupViewModel _favorites;
    private GroupViewModel _allItems;
    private GroupViewModel _deleted;
    private List<GroupViewModel> _headerGroups;
    private ObservableCollection<CardViewModel> _currentCards;
    private ObservableCollection<GroupViewModel> _currentGroups;
    private string _errorMessage;
    private string _search;
    private int _selectedCardIndex;
    private bool _isEnabled;
    private bool _isAddEnabled;
    private bool _isGroupAdd;

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

    public bool IsGroupAdd
    {
        get => _isGroupAdd;
        set => this.RaiseAndSetIfChanged(ref _isGroupAdd, value);
    }

    public ICommand LoadDataCommand { get; }

    public ICommand LogoutCommand { get; }

    public ICommand SortCommand { get; }

    public ICommand AddCardCommand { get; }

    public ICommand AddGroupCommand { get; }

    public ICommand EditGroupCommand { get; }

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
            new Group() { Id = 1, Title = "Group1", Image = "\uf097", GroupId = 0 },
            new Group() { Id = 2, Title = "group2", Image = "\uf097", GroupId = 1 },
            new Group() { Id = 3, Title = "group3", Image = "\uf097", GroupId = 1 },
            new Group() { Id = 4, Title = "Group4", Image = "\uf097", GroupId = 0 },
            new Group() { Id = 5, Title = "group5", Image = "\uf097", GroupId = 4 },
        };

        InitList();
        SetData(cards, groups);
    }

    public HomeViewModel(IScreen? hostScreen = null, CardService? cardService = null, GroupService? groupService = null)
    {
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
        _cardService = cardService ?? Locator.Current.GetService<CardService>()!;
        _groupService = groupService ?? Locator.Current.GetService<GroupService>()!;

        InitList();

        SortCommand = ReactiveCommand.Create<int>(SortCards);
        AddCardCommand = ReactiveCommand.Create(AddCard, CanAddCard());
        AddGroupCommand = ReactiveCommand.Create(AddHeaderGroup);
        EditGroupCommand = ReactiveCommand.Create(EditGroup, CanEditGroup());
        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
        LogoutCommand = ReactiveCommand.CreateFromObservable(LogOut);
        LoadDataCommand.Execute(null);
    }

    private void AddCard()
    {
        IsEnabled = false;

        var card = new Card
        {
            Id = 0,
            Title = "",
            Website = "",
            Image = '\uf2bc',
            Username = "",
            Notes = "",
            Password = "",
            GroupId = SelectedGroup.Id,
            IsFavorite = false,
        };

        var cardViewModel = new CardViewModel(card) { IsEnable = true };
        SetCardProperties(cardViewModel);

        SelectedGroup.Cards.Add(cardViewModel);
        SelectedCard = cardViewModel;
        SelectedCard = cardViewModel;
    }

    private IObservable<bool> CanAddCard() =>
        this.WhenAnyValue(x => x.SelectedGroup)
            .WhereNotNull()
            .Select(x => x.Id > 0);

    private void AddHeaderGroup()
    {
        IsEnabled = false;
        IsGroupAdd = true;

        var group = new Group
        {
            Id = -1,
            Title = "",
            Image = '\uf097',
            GroupId = 0,
        };

        var groupViewModel = new GroupViewModel(group)
        {
            ConfirmCommand = ReactiveCommand.CreateFromTask<bool>(ConfirmGroupChanges),
            AddSubGroupCommand = ReactiveCommand.Create<GroupViewModel>(AddSubGroup),
            IsEnable = true
        };

        SelectedGroup = groupViewModel;
    }

    private void AddSubGroup(GroupViewModel header)
    {
        IsEnabled = false;
        IsGroupAdd = true;

        var group = new Group
        {
            Id = -1,
            Title = "",
            Image = '\uf097',
            GroupId = header.Id,
        };

        var groupViewModel = new GroupViewModel(group)
        {
            ConfirmCommand = ReactiveCommand.CreateFromTask<bool>(ConfirmGroupChanges),
            IsEnable = true
        };

        SelectedGroup = groupViewModel;
    }

    private void EditGroup()
    {
        SelectedGroup.IsEnable = true;
        IsGroupAdd = true;
        IsEnabled = false;
    }

    private IObservable<bool> CanEditGroup() =>
        this.WhenAnyValue(x => x.SelectedGroup)
            .WhereNotNull()
            .Select(x => x.Id > 0);

    private void DeleteCard()
    {
        var selectedGroup = SelectedGroup;
        var selectedCard = SelectedCard;

        _userCards.Remove(selectedCard);
        _deleted.Cards.Add(selectedCard);
        selectedGroup.Cards.Remove(selectedCard);
        SetHandlers();
        SelectedGroup = selectedGroup;
    }

    private async Task ConfirmCardChanges(bool confirmed)
    {
        var card = SelectedCard;

        if (confirmed)
        {
            if (card.Id == 0)
            {
                card.GroupId = card.CurrentGroup.Id == 0 ? card.HeaderGroup.Id : card.CurrentGroup.Id;
                card.Id = await _cardService!.CreateCard(card.GetCard());
                _userCards.Add(card);
            }
            else
            {
                card.GroupId = card.CurrentGroup.Id == 0 ? card.HeaderGroup.Id : card.CurrentGroup.Id;
                await _cardService!.UpdateCard(card.GetCard());
            }

            SetCurrentCards(card);
        }
        else
        {
            SetCurrentCards(SelectedCard.Id == 0 ? null : SelectedCard);
        }
    }

    private async Task ConfirmGroupChanges(bool confirmed)
    {
        var group = SelectedGroup;

        if (confirmed)
        {
            if (group.Id == -1)
            {
                group.Id = await _groupService!.CreateGroup(group.GetGroup());
                _userGroups.Add(group);
            }
            else
            {
                await _groupService!.UpdateGroup(group.GetGroup());
            }

            SetCurrentGroup(group);
        }
        else
        {
            SetCurrentGroup(group.Id == 0 ? null : group);
        }
    }

    private void SetCurrentCards(CardViewModel? card = null)
    {
        card ??= SelectedGroup.Cards.First();

        IsEnabled = true;
        SetHandlers();
        SelectedGroup = _userGroups.FirstOrDefault(g => g.Id == card.GroupId) ?? new GroupViewModel();
        SelectedCard = card;
    }

    private void SetCurrentGroup(GroupViewModel? group = null)
    {
        group ??= _userGroups.FirstOrDefault() ?? new GroupViewModel();

        IsEnabled = true;
        if (IsGroupAdd) IsGroupAdd = false;
        SetHandlers();
        SelectedGroup = group;
    }

    private void SearchCards()
    {
        CurrentCards.Clear();
        CurrentCards.AddRange(!string.IsNullOrEmpty(Search)
            ? SelectedGroup.Cards.Where(x => x.Title.Contains(Search, StringComparison.CurrentCultureIgnoreCase))
                .ToList()
            : SelectedGroup.Cards);
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
        IsEnabled = true;

        _allItems.Cards.Clear();
        _allItems.Cards.AddRange(_userCards);

        _favorites.Cards.Clear();
        _favorites.Cards.AddRange(_userCards.Where(x => x.IsFavorite));

        CurrentGroups.RemoveMany(_headerGroups);

        _headerGroups.Clear();
        _headerGroups.AddRange(_userGroups.Where(x => x.GroupId == 0).ToList());

        CurrentGroups.AddOrInsertRange(_headerGroups, 2);
    }

    private void SetProperties()
    {
        var headerGroup = _userGroups.Where(g => g.GroupId == 0).ToList();

        foreach (var group in headerGroup)
        {
            group.SubGroups.Clear();
            group.SubGroups.AddRange(_userGroups.Where(x => x.GroupId == group.Id));
            group.AddSubGroupCommand = ReactiveCommand.Create<GroupViewModel>(AddSubGroup);
        }

        foreach (var card in _userCards)
        {
            SetCardProperties(card);
            card.WhenAnyValue(x => x.HeaderGroup)
                .WhereNotNull()
                .Subscribe(x =>
                {
                    card.CurrentGroups.Clear();
                    card.CurrentGroups.Add(new GroupViewModel());
                    card.CurrentGroups.AddRange(x.SubGroups);
                    card.CurrentGroup = card.CurrentGroups.FirstOrDefault(model => model.Id == card.GroupId) ??
                                        card.CurrentGroups.First();
                });
        }

        foreach (var group in _userGroups)
        {
            group.Cards.Clear();
            group.Cards.AddRange(_userCards.Where(x => x.GroupId == group.Id));
            group.ConfirmCommand = ReactiveCommand.CreateFromTask<bool>(ConfirmGroupChanges);
        }

        SetList();
    }

    private void SetCardProperties(CardViewModel card)
    {
        card.HeaderGroups.Clear();
        card.HeaderGroups.AddRange(_userGroups.Where(g => g.GroupId == 0).ToList());
        card.HeaderGroup = card.HeaderGroups.First(h =>
            h.SubGroups.Any(s => s.Id == card.GroupId) ||
            h.Id == card.GroupId);

        card.CurrentGroups.Clear();
        card.CurrentGroups.Add(new GroupViewModel());
        card.CurrentGroups.AddRange(card.HeaderGroup.SubGroups);
        card.CurrentGroup = card.CurrentGroups.FirstOrDefault(model => model.Id == card.GroupId) ??
                            card.CurrentGroups.First();

        card.DeleteCommand = ReactiveCommand.Create(DeleteCard);
        card.ConfirmCommand = ReactiveCommand.CreateFromTask<bool>(ConfirmCardChanges);
    }

    private void SetHandlers()
    {
        this.WhenAnyValue(x => x.SelectedGroup)
            .WhereNotNull()
            .Subscribe(x =>
            {
                CurrentCards.Clear();
                CurrentCards.AddRange(x.Cards);
            });

        this.WhenAnyValue(x => x.Search)
            .DistinctUntilChanged()
            .Subscribe(_ => SearchCards());

        SetProperties();
    }

    private void SetData(List<Card> cards, List<Group> groups)
    {
        _userCards = cards.Select(x => new CardViewModel(x)).ToList();
        _userGroups = groups.Select(x => new GroupViewModel(x)).ToList();

        SetHandlers();
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        try
        {
            var cardResponse = await _cardService!.GetCardsByUser(cancellationToken);
            var cards = cardResponse ?? [];

            var groupResponse = await _groupService!.GetGroupsByUser(cancellationToken);
            var groups = groupResponse ?? [];

            SetData(cards, groups);
        }
        catch (Exception e)
        {
            ErrorMessage = "error";
        }
    }

    private IObservable<IRoutableViewModel> LogOut() => Observable.FromAsync(async token =>
    {
        await AuthService.LogoutAsync();
        return HostScreen.Router.Navigate.Execute(new LoginViewModel(HostScreen));
    }).ObserveOn(RxApp.MainThreadScheduler).SelectMany(x => x);

    private void InitList()
    {
        SelectedGroup = new GroupViewModel();

        CurrentCards = [];
        _headerGroups = [];

        _allItems = new GroupViewModel(new Group
        {
            Id = -1,
            Title = "All Items",
            Image = '\uf2ba',
            GroupId = 0
        });

        _favorites = new GroupViewModel(new Group
        {
            Id = -1,
            Title = "Favorites",
            Image = '\uf006',
            GroupId = 0
        });

        _deleted = new GroupViewModel(new Group
        {
            Id = -1,
            Title = "Recently Deleted",
            Image = '\uf014',
            GroupId = 0
        });

        CurrentGroups = [_allItems, _favorites, _deleted];
    }
}