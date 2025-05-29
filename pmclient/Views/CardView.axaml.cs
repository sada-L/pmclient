using Avalonia.ReactiveUI;
using pmclient.ViewModels;

namespace pmclient.Views;

public partial class CardView : ReactiveUserControl<CardViewModel>
{
    public CardView()
    {
        InitializeComponent();
    }
}