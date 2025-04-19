using System;
using System.Reactive;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class RegisterViewModel : ViewModelBase, IRoutableViewModel
{
    private string _email = string.Empty;
    private string _username = string.Empty;
    private string _password = string.Empty;
    private string _confirmPassword = string.Empty;

    public string Email
    {
        get => _email;
        set => this.RaiseAndSetIfChanged(ref _email, value);
    }
    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }
    public string ConfirmPassword
    {
        get => _confirmPassword;
        set => this.RaiseAndSetIfChanged(ref _confirmPassword, value);
    }
    public IScreen HostScreen { get; }
    public string UrlPathSegment { get; } = "register";
    public ReactiveCommand<Unit, Unit> RegisterCommand { get; }
    public ReactiveCommand<Unit, IRoutableViewModel> BackCommand { get; }

    public RegisterViewModel()
    {
        Email = "email@gmail.com";
        Username = "username";
        Password = "password";
        ConfirmPassword = "password";
        RegisterCommand = ReactiveCommand.Create(() => {}, CanExecRegister());
        BackCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.NavigateBack.Execute());
    }
    
    public RegisterViewModel(IScreen? screen = null)
    {
       HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
       RegisterCommand = ReactiveCommand.Create(() => {}, CanExecRegister());
       BackCommand = ReactiveCommand.CreateFromObservable(() => HostScreen.Router.NavigateBack.Execute());
    }
    
     private IObservable<bool> CanExecRegister() =>
            this.WhenAnyValue(
                x => x.Email,
                x => x.Username,
                x => x.Password,
                x => x.ConfirmPassword,
                (email, username, password, confirmPassword) =>
                    !string.IsNullOrWhiteSpace(email) &&
                    !string.IsNullOrWhiteSpace(username) &&
                    !string.IsNullOrWhiteSpace(password) &&
                    !string.IsNullOrWhiteSpace(confirmPassword) && 
                    password == confirmPassword);
}