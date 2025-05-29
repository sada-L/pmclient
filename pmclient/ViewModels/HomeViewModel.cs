using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Threading;
using DynamicData;
using pmclient.Extensions;
using pmclient.Models;
using pmclient.Services;
using pmclient.Settings;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class HomeViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly UserService? _userService;
    private readonly CardService? _cardService;
    private readonly GroupService? _groupService;
    private readonly SettingsService? _settingsService;
    private List<CardViewModel> _userCards;
    private List<GroupViewModel> _userGroups;

    private readonly List<Card> _cards = [];
    private readonly List<Group> _groups = [];
    private readonly List<CardViewModel> _currentCards = [];
    private readonly List<GroupViewModel> _subGroups = [];

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
    private bool _isDefaultTheme;

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

    public ReactiveCollection<CardViewModel> CurrentCards { get; } = new();

    [Reactive] public GroupViewModel SelectedGroup { get; set; }

    [Reactive] public CardViewModel? SelectedCard { get; set; }

    [Reactive] public ViewModelBase? CurrentItem { get; set; }

    [Reactive] public string ErrorMessage { get; set; }

    [Reactive] public string Search { get; set; }

    [Reactive] public bool IsEnabled { get; set; }

    [Reactive] public bool IsDefaultTheme { get; set; }

    private IDisposable? SelectedGroupChanged { get; set; }

    private IDisposable? SelectedCardChanged { get; set; }

    private IDisposable? SearchChanged { get; set; }

    public ICommand LoadDataCommand { get; }

    public ICommand LogoutCommand { get; }

    public ICommand SortCommand { get; }

    public ICommand AddCardCommand { get; }

    public ICommand AddGroupCommand { get; }

    public ICommand EditGroupCommand { get; }

    public ICommand ChangeThemeCommand { get; }

    public ICommand ChangeLanguageCommand { get; }

    public ICommand AuthCommand { get; }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => "/home";

    public HomeViewModel()
    {
    }

    public HomeViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        _userService = Locator.Current.GetService<UserService>()!;
        _cardService = Locator.Current.GetService<CardService>()!;
        _groupService = Locator.Current.GetService<GroupService>()!;
        _settingsService = Locator.Current.GetService<SettingsService>()!;

        var canEditGroup = this
            .WhenAnyValue(x => x.SelectedGroup)
            .WhereNotNull()
            .Select(x => x.Id > 0);

        SortCommand = ReactiveCommand.Create<int>(SortCards);
        AddCardCommand = ReactiveCommand.Create(AddCard, canEditGroup);
        AddGroupCommand = ReactiveCommand.Create(AddHeaderGroup);
        EditGroupCommand = ReactiveCommand.Create(EditGroup, canEditGroup);
        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
        ChangeThemeCommand = ReactiveCommand.Create(ChangeTheme);
        ChangeLanguageCommand = ReactiveCommand.Create(ChangeLanguage);
        LogoutCommand = ReactiveCommand.CreateFromObservable(LogOut);
        AuthCommand = ReactiveCommand.CreateFromObservable(OpenAuth);
        LoadDataCommand.Execute(null);
    }

    private IObservable<IRoutableViewModel> OpenAuth()
    {
        return HostScreen.Router.Navigate.Execute(new AuthViewModel(HostScreen));
    }

    private void ChangeTheme()
    {
        if (_settingsService!.CurrentSettings.Theme == "Dark")
        {
            _settingsService!.SetTheme("Light");
            IsDefaultTheme = true;
        }
        else
        {
            _settingsService!.SetTheme("Dark");
            IsDefaultTheme = false;
        }
    }

    private void SetLanguage(bool isDefault)
    {
        _allItems.Title = isDefault ? "Все" : "All Items";
        _favorites.Title = isDefault ? "Избранное" : "Favorites";
        _deleted.Title = isDefault ? "Удаленное" : "Deleted";
    }

    private void ChangeLanguage()
    {
        var isDefault = _settingsService!.CurrentSettings.Language == "Ru";
        _settingsService!.SetLanguage(isDefault ? "En" : "Ru");
        SetLanguage(!isDefault);
    }

    private void AddCard()
    {
        var group = SelectedGroup;

        var card = new Card
        {
            Id = 0,
            GroupId = group.Id,
            Title = "",
            Username = "",
            Password = "",
            Website = "",
            Notes = "",
            Image = '\uf2bc',
            IsFavorite = false
        };

        var cardViewModel = new CardViewModel(card) { IsEnable = true };
        SetCardProperties(cardViewModel);

        SelectedGroup.Cards.Add(cardViewModel);
        SelectedCard = null!;
        SelectedCard = cardViewModel;
    }

    private void AddHeaderGroup()
    {
        IsEnabled = false;
        IsGroupAdd = true;

        var group = new Group
        {
            Id = -1,
            Title = "Title",
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

    private IObservable<bool> CanEditGroup => this
        .WhenAnyValue(x => x.SelectedGroup)
        .WhereNotNull()
        .Select(x => x.Id > 0);

    private async Task DeleteCard()
    {
        var card = SelectedGroup;
        var group = SelectedCard;

        _userCards.Remove(group);
        _deleted.Cards.Add(group);
        card.Cards.Remove(group);
        SetHandlers();
        SelectedGroup = card;
        await _cardService!.DeleteCard(group.Id);
    }

    private async Task DeleteGroup()
    {
        var group = SelectedGroup;
        IsGroupAdd = false;

        _userCards.Remove(_userCards.Where(x => x.GroupId == group.Id));
        _userGroups.Remove(group);
        SetHandlers();
        await _groupService!.DeleteGroup(group.Id);
        SelectedGroup = new GroupViewModel();
        SelectedGroup = null!;
    }

    private IObservable<bool> CanDeleteGroup => this
        .WhenAnyValue(x => x.SelectedGroup)
        .WhereNotNull()
        .Select(x => x.Cards.Count == 0 && x.SubGroups.Count == 0);

    private async Task SaveCardChanges()
    {
        var card = SelectedCard;

        if (card.Id == 0)
        {
            card.GroupId = card.CurrentGroup.Id == 0 ? card.HeaderGroup.Id : card.CurrentGroup.Id;
            card.Id = await _cardService!.CreateCard(card.GetCard());
            card.Save();
            _userCards.Add(card);
        }
        else
        {
            card.GroupId = card.CurrentGroup.Id == 0 ? card.HeaderGroup.Id : card.CurrentGroup.Id;
            card.Save();
            await _cardService!.UpdateCard(card.GetCard());
        }

        SetCurrentCards(card);
    }

    private void CancelCardChanges()
    {
        var card = SelectedCard;
        card.Cancel();
        SetCurrentCards(SelectedCard.Id == 0 ? null : SelectedCard);
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
        var group = SelectedGroup;
        IsEnabled = true;
        SetHandlers();
        SelectedGroup = _userGroups.FirstOrDefault(x => x.Id == card?.GroupId) ?? group;
        SelectedCard = null!;
        SelectedCard = card ?? null!;
    }

    private void SetCurrentGroup(GroupViewModel? group = null)
    {
        group ??= SelectedGroup;

        IsEnabled = true;
        if (IsGroupAdd) IsGroupAdd = false;
        SetHandlers();
        SelectedGroup = null!;
        SelectedGroup = group;
    }

    private async Task SetCardFavorite()
    {
        var card = SelectedCard;
        card.Favorite();
        await SaveCardChanges();
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
            group.DeleteCommand = ReactiveCommand.CreateFromTask(DeleteGroup, CanDeleteGroup);
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

        card.SaveCommand = ReactiveCommand.CreateFromTask(SaveCardChanges);
        card.DeleteCommand = ReactiveCommand.CreateFromTask(DeleteCard);
        card.CancelCommand = ReactiveCommand.Create(CancelCardChanges);
        card.FavoriteCommand = ReactiveCommand.CreateFromTask(SetCardFavorite);
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

            if (UserSettings.User == null)
                await _userService!.GetUserAsync(cancellationToken);

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
    }).SelectMany(x => x);
}