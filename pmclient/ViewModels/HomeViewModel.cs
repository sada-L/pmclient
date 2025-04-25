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
    private User _user;
    private List<Card> _cards = [];
    private List<Group> _groups = [];

    private CardViewModel _selectedCard;
    private GroupViewModel _selectedGroup;
    private GroupViewModel _favoriteCards;
    private GroupViewModel _allCards;
    private GroupViewModel _deletedCards;
    private int _selectedCardIndex;
    private ObservableCollection<GroupViewModel> _vmGroups;
    private string _errorMessage;

    public User User
    {
        get => _user;
        set => this.RaiseAndSetIfChanged(ref _user, value);
    }

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

    public ObservableCollection<GroupViewModel> Groups
    {
        get => _vmGroups;
        set => this.RaiseAndSetIfChanged(ref _vmGroups, value);
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
        User = new User()
        {
            Id = 1,
            Email = "user@gmail.com",
            Username = "user",
        };

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
            Image = "\uf005",
            GroupId = 0
        }, cards.Where(x => x.IsFavorite).ToList(), null);

        _deletedCards = new GroupViewModel(new Group()
        {
            Id = -1,
            Title = "Recently Deleted",
            Image = "\uf1f8",
            GroupId = 0
        }, new List<Card>(), null);


        var vmgroups = groups
            .Where(x => x.GroupId == 0)
            .Select(x => new GroupViewModel(x, cards, null, groups)).ToList();

        Groups = new ObservableCollection<GroupViewModel>();
        Groups.Add(_allCards);
        Groups.Add(_favoriteCards);
        Groups.AddRange(vmgroups);
        Groups.Add(_deletedCards);

        this.WhenAnyValue(x => x.SelectedGroup)
            .Subscribe(x => SelectedCardIndex = 0);

        SelectedGroup = Groups.FirstOrDefault();
    }

    public HomeViewModel(IScreen? hostScreen = null, ICardWebApi? cardWebApi = null, IGroupWebApi? groupWebApi = null)
    {
        User = StaticStorage.User!;
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
        _cardWebApi = cardWebApi ?? Locator.Current.GetService<ICardWebApi>()!;
        _groupWebApi = groupWebApi ?? Locator.Current.GetService<IGroupWebApi>()!;

        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
        LoadDataCommand.Execute().Subscribe();
    }

    private void SetList()
    {
        _allCards = new GroupViewModel(new Group()
        {
            Id = -1,
            Title = "All Items",
            Image = "\uf2ba",
            GroupId = 0
        }, _cards, null);

        _favoriteCards = new GroupViewModel(new Group()
        {
            Id = -1,
            Title = "Favorites",
            Image = "\uf005",
            GroupId = 0
        }, _cards.Where(x => x.IsFavorite).ToList(), null);

        _deletedCards = new GroupViewModel(new Group()
        {
            Id = -1,
            Title = "Recently Deleted",
            Image = "\uf1f8",
            GroupId = 0
        }, new List<Card>(), null);

        var vmgroups = _groups
            .Where(x => x.GroupId == 0)
            .Select(x => new GroupViewModel(x, _cards, null, _groups)).ToList();

        Groups = [_allCards, _favoriteCards];
        Groups.AddRange(vmgroups);
        Groups.Add(_deletedCards);

        this.WhenAnyValue(x => x.SelectedGroup)
            .Subscribe(x => SelectedCardIndex = 0);

        SelectedGroup = Groups.FirstOrDefault() ?? _allCards;
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

            _cards = cardResponse.Content ?? new List<Card>();

            var groupResponse = await _groupWebApi!.GetGroupsByUser(cancellationToken);
            if (!groupResponse.IsSuccessStatusCode)
            {
                ErrorMessage = "error";
                return;
            }

            _groups = groupResponse.Content ?? new List<Group>();

            SetList();
        }
        catch (Exception e)
        {
            ErrorMessage = "error";
            return;
        }
    }
}