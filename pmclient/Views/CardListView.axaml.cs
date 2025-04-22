using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class CardListView : ReactiveUserControl<CardViewModel>
{
    public CardListView()
    {
        InitializeComponent();
    }
}