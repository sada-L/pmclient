using System.Reactive;
using Avalonia.Controls;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class MainWindowViewModel : ViewModelBase, IScreen
{
    private readonly Window _window;
    public RoutingState Router { get; } = new RoutingState();
    public ReactiveCommand<Unit,Unit> MinimizeCommand { get; }
    public ReactiveCommand<Unit,Unit> MaximizeCommand { get; }
    public ReactiveCommand<Unit,Unit> CloseCommand { get; }
    public MainWindowViewModel(Window window)
    {
        _window = window;

        MinimizeCommand = ReactiveCommand.Create(() => { _window.WindowState = WindowState.Minimized; });
        CloseCommand = ReactiveCommand.Create(() => { _window.Close(); });
        MaximizeCommand = ReactiveCommand.Create(() => {
            _window.WindowState = _window.WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;
        });
        
        Router.Navigate.Execute(new LoginViewModel(null, null,this));
    }
}