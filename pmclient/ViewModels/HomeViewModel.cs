using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using pmclient.Helpers;
using pmclient.Models;
using pmclient.RefitClients;
using pmclient.Services;
using pmclient.Views;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class HomeViewModel : ViewModelBase, IRoutableViewModel
{
    private CardViewModel _selectedCard;
    private readonly ICardWebApi? _cardWebApi;
    private User _user;
    private ObservableCollection<CardViewModel> _cards;

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

    public ObservableCollection<CardViewModel> Cards
    {
        get => _cards;
        set => this.RaiseAndSetIfChanged(ref _cards, value);
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
       
       Cards = new ObservableCollection<CardViewModel>(new List<CardViewModel>()
       {
           new CardViewModel(), 
           new CardViewModel()
           {
               Title = "card lulu"
           } 
       });
       
       SelectedCard = Cards.FirstOrDefault();
    }

    public HomeViewModel(ICardWebApi? cardWebApi = null, IScreen? hostScreen = null)
    {
        User = StaticStorage.User!;
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;
        _cardWebApi = cardWebApi ?? Locator.Current.GetService<ICardWebApi>()!;

        LoadDataCommand = ReactiveCommand.CreateFromTask(LoadDataAsync);
        LoadDataCommand.Execute().Subscribe();
    }

    private async Task LoadDataAsync(CancellationToken cancellationToken)
    {
        try
        {
           var cardResponse = await _cardWebApi!.GetCardByUser(cancellationToken);
           Cards = new ObservableCollection<CardViewModel>(cardResponse.Content!.Select(x => new CardViewModel(x)));
        }
        catch (Exception e)
        {
            return;
        }
    }
}