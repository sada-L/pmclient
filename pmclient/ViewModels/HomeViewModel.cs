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
using ReactiveUI.Fody.Helpers;
using Splat;

namespace pmclient.ViewModels;

public class HomeViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly UserService? _userService;
    private readonly CardService? _cardService;
    private readonly GroupService? _groupService;
    private readonly SettingsService? _settingsService;

    private readonly List<Card> _cards = [];
    private readonly List<Group> _groups = [];
    private readonly List<CardViewModel> _currentCards = [];
    private readonly List<GroupViewModel> _subGroups = [];
    private readonly List<HeaderGroupViewModel> _headerGroups = [];

    private readonly HeaderGroupViewModel _favorites = new(new Group
        { Id = -1, GroupId = 0, Title = "", Image = '\uf006' });

    private readonly HeaderGroupViewModel _allItems = new(new Group
        { Id = -1, GroupId = 0, Title = "", Image = '\uf2ba' });

    private readonly HeaderGroupViewModel _deleted = new(new Group
        { Id = -1, GroupId = 0, Title = "", Image = '\uf014' });

    public ReactiveCollection<HeaderGroupViewModel> CurrentGroups { get; } = new();

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

    private IObservable<bool> CanDeleteHeaderGroup => this
        .WhenAnyValue(x => x.SelectedGroup)
        .Select(x => x as HeaderGroupViewModel)
        .WhereNotNull()
        .Select(x => x.Cards.Items.Count == 0 && x.SubGroups.Items.Count == 0);

    private IObservable<bool> CanDeleteSubGroup => this
        .WhenAnyValue(x => x.SelectedGroup)
        .WhereNotNull()
        .Select(x => x.Cards.Items.Count == 0);

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

        var currentCard = new CardViewModel(card) { IsEnable = true }
            .SetHeaderGroups(_headerGroups)
            .SetCurrentGroups(group);

        SetCardCommand(currentCard);

        group.Cards.Add(currentCard);
        IsEnabled = false;
        SelectedCard = currentCard;
    }

    private void AddHeaderGroup()
    {
        var group = new Group { Id = -1, GroupId = 0, Title = "", Image = '\uf097' };
        var headerGroup = new HeaderGroupViewModel(group);

        SetGroupCommand(headerGroup, CanDeleteHeaderGroup);

        SelectedGroup = headerGroup;
        CurrentItem = SelectedGroup;
        IsEnabled = false;
    }

    private void AddSubGroup(HeaderGroupViewModel header)
    {
        var group = new Group { Id = -1, GroupId = header.Id, Title = "", Image = '\uf274' };
        var subGroup = new GroupViewModel(group);

        SetGroupCommand(subGroup, CanDeleteSubGroup);

        SelectedGroup = subGroup;
        CurrentItem = SelectedGroup;
        IsEnabled = false;
    }

    private void EditCard()
    {
        var card = SelectedCard;
        if (card is null) return;

        card.IsEnable = true;
        CurrentItem = card;
        IsEnabled = false;
    }

    private void EditGroup()
    {
        var group = SelectedGroup;

        group.IsEnable = true;
        CurrentItem = group;
        IsEnabled = false;
    }

    private async Task DeleteCard()
    {
        var group = SelectedGroup;
        var card = SelectedCard;
        if (card is null) return;

        _cards.Remove(card.GetCard());
        _deleted.Cards.Add(card);
        group.Cards.Remove(card);


        SetCurrentGroup();

        await _cardService!.DeleteCard(group.Id);
    }

    private async Task DeleteGroup()
    {
        var group = SelectedGroup?.GetGroup();
        if (group is null) return;

        _cards.Remove(_cards.Where(x => x.GroupId == group.Id));
        _groups.Remove(group);

        SetObservers();
        CurrentItem = null;

        await _groupService!.DeleteGroup(group.Id);
    }

    private async Task SaveCardChanges()
    {
        var card = SelectedCard;
        if (card is null) return;
        card.Save();

        if (card.Id == 0)
        {
            card.GroupId = card.CurrentGroup.Id == 0 ? card.HeaderGroup.Id : card.CurrentGroup.Id;
            card.Id = await _cardService!.CreateCard(card.GetCard());
            card.Save();
            _cards.Add(card.GetCard());
        }
        else
        {
            card.GroupId = card.CurrentGroup.Id == 0 ? card.HeaderGroup.Id : card.CurrentGroup.Id;
            card.Save();
            var index = _cards.FindIndex(x => x.Id == card.Id);
            _cards[index] = card.GetCard();
            await _cardService!.UpdateCard(card.GetCard());
        }

        SetCurrentCards(card);
    }

    private void CancelCardChanges()
    {
        SelectedCard?.Cancel();
        IsEnabled = true;
    }

    private async Task SaveGroupChanges()
    {
        var group = SelectedGroup;
        group.Save();

        if (group.Id == -1)
        {
            group.Id = await _groupService!.CreateGroup(group.GetGroup());
            group.Save();
            _groups.Add(group.GetGroup());
        }
        else
        {
            var index = _groups.FindIndex(x => x.Id == group.Id);
            _groups[index] = group.GetGroup();
            await _groupService!.UpdateGroup(group.GetGroup());
        }

        SetCurrentGroup(SelectedGroup);
    }

    private void CancelGroupChanges()
    {
        var group = SelectedGroup;

        group.Cancel();
        CurrentItem = null;
        SelectedCard = null;
        IsEnabled = true;
    }

    private void SetCurrentCards(CardViewModel? card = null)
    {
        var group = SelectedGroup;
        if (group is null) return;

        SetList();
        SelectedGroup = CurrentGroups.Items.FirstOrDefault(x => x.Id == group.Id) ?? group;
        CurrentCards.Replace(SelectedGroup.Cards.Items);
        var curCard = CurrentCards.Items.FirstOrDefault(x => x.Id == card?.Id);
        Dispatcher.UIThread.Post(() => SelectedCard = curCard, DispatcherPriority.Send);
        CurrentItem = SelectedCard;
        IsEnabled = true;
        SetObservers();
    }

    private void SetCurrentGroup(GroupViewModel? group = null)
    {
        group ??= SelectedGroup;

        SetList();
        SelectedGroup = CurrentGroups.Items.FirstOrDefault(x => x.Id == group.Id) ?? group;
        CurrentItem = null;
        IsEnabled = true;
        SetObservers();
    }

    private async Task SetCardFavorite()
    {
        var card = SelectedCard;
        if (card is null) return;

        card.IsFavorite = !card.IsFavorite;
        await SaveCardChanges();
    }

    private void SearchCards()
    {
        CurrentCards.Replace(!string.IsNullOrEmpty(Search)
            ? SelectedGroup.Cards.Items.Where(x => x.Title.Contains(Search, StringComparison.CurrentCultureIgnoreCase))
                .ToList()
            : SelectedGroup.Cards.Items.ToList());
    }

    private void SortCards(int index)
    {
        var sortList = index switch
        {
            0 => CurrentCards.Items.OrderBy(x => x.Title).ToList(),
            1 => CurrentCards.Items.OrderByDescending(x => x.Title).ToList(),
            3 => CurrentCards.Items.OrderBy(x => x.Website).ToList(),
            4 => CurrentCards.Items.OrderByDescending(x => x.Website).ToList(),
            5 => CurrentCards.Items.OrderBy(x => x.Username).ToList(),
            6 => CurrentCards.Items.OrderByDescending(x => x.Username).ToList(),
            _ => CurrentCards.Items.ToList()
        };
        CurrentCards.Replace(sortList);
    }

    private void SetList()
    {
        SelectedGroupChanged?.Dispose();
        SelectedCardChanged?.Dispose();
        SearchChanged?.Dispose();

        SetCurrentList();

        _allItems.Cards.Clear();
        _allItems.Cards.AddRange(_currentCards);

        _favorites.Cards.Clear();
        _favorites.Cards.AddRange(_currentCards.Where(x => x.IsFavorite));

        CurrentGroups.Replace(_headerGroups);
        CurrentGroups.AddRangeAt([_allItems, _favorites, _deleted], 0);

        SelectedGroup = _allItems;

        IsEnabled = true;
    }

    private void SetCurrentList()
    {
        _currentCards.Clear();
        _currentCards.AddRange(_cards.Select(x => new CardViewModel(x)));

        _subGroups.Clear();
        _subGroups.AddRange(_groups.Where(x => x.GroupId != 0).Select(x => new GroupViewModel(x)));

        _headerGroups.Clear();
        _headerGroups.AddRange(_groups.Where(x => x.GroupId == 0).Select(x => new HeaderGroupViewModel(x)));

        SetProperties();
    }

    private void SetObservers()
    {
        SelectedGroupChanged = this
            .WhenAnyValue(x => x.SelectedGroup)
            .Subscribe(x => CurrentCards.Replace(x.Cards.Items));

        SelectedCardChanged = this
            .WhenAnyValue(vm => vm.SelectedCard)
            .BindTo(this, vm => vm.CurrentItem);

        SearchChanged = this
            .WhenAnyValue(x => x.Search)
            .Subscribe(_ => SearchCards());
    }

    private void SetProperties()
    {
        foreach (var group in _headerGroups)
            group.SetSubGroups(_subGroups)
                .SetCommand(x => x.AddSubGroupCommand, ReactiveCommand.Create<HeaderGroupViewModel>(AddSubGroup));

        foreach (var card in _currentCards)
            SetCardCommand(card
                    .SetHeaderGroups(_headerGroups)
                    .SetCurrentGroups(card.HeaderGroup))
                .WhenAnyValue(x => x.HeaderGroup)
                .WhereNotNull()
                .Subscribe(x => card.SetCurrentGroups(x));

        foreach (var group in _subGroups)
            SetGroupCommand(group.SetCards(_currentCards), CanDeleteHeaderGroup);

        foreach (var group in _headerGroups)
            SetGroupCommand(group.SetCards(_currentCards), CanDeleteSubGroup);
    }

    private CardViewModel SetCardCommand(CardViewModel vm)
    {
        vm.SetCommand(x => x.DeleteCommand, ReactiveCommand.CreateFromTask(DeleteCard))
            .SetCommand(x => x.SaveCommand, ReactiveCommand.CreateFromTask(SaveCardChanges))
            .SetCommand(x => x.CancelCommand, ReactiveCommand.Create(CancelCardChanges))
            .SetCommand(x => x.FavoriteCommand, ReactiveCommand.CreateFromTask(SetCardFavorite))
            .SetCommand(x => x.EditCommand, ReactiveCommand.Create(EditCard));

        return vm;
    }

    private void SetGroupCommand(GroupViewModel vm, IObservable<bool> canDeleteGroup)
    {
        vm.SetCommand(x => x.DeleteCommand, ReactiveCommand.CreateFromTask(DeleteGroup, canDeleteGroup))
            .SetCommand(x => x.CancelCommand, ReactiveCommand.Create(CancelGroupChanges))
            .SetCommand(x => x.SaveCommand, ReactiveCommand.CreateFromTask(SaveGroupChanges));
    }

    private void SetData(List<Card> cards, List<Group> groups)
    {
        _cards.Clear();
        _cards.AddRange(cards);

        _groups.Clear();
        _groups.AddRange(groups);

        SetLanguage(_settingsService!.CurrentSettings.Language == "Ru");
        SetList();
        SetObservers();
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