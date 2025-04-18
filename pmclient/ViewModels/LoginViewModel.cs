using System;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using pmclient.Contracts.Requests.Auth;
using pmclient.RefitClients;
using ReactiveUI;
using Splat;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reactive.Linq;
using System.Security.Claims;
using pmclient.Services;

namespace pmclient.ViewModels;

public class LoginViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly IIdentityWebApi _identityWebApi;
    private readonly IUsersWebApi _usersWebApi;
    private string _errorMessage = string.Empty;
    private LoginRequest _loginRequest = new LoginRequest();

    public IScreen HostScreen { get; }

    public LoginRequest LoginRequest
    {
        get => _loginRequest;
        set => this.RaiseAndSetIfChanged(ref _loginRequest, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public string UrlPathSegment => "/login";

    ReactiveCommand<Unit, IRoutableViewModel> LoginCommand { get; }

    public LoginViewModel(IIdentityWebApi identityWebApi, IUsersWebApi usersWebApi)
    {
        HostScreen = Locator.Current.GetService<IScreen>()!;
        _identityWebApi = identityWebApi;
        _usersWebApi = usersWebApi;
        
        LoginCommand = ReactiveCommand.CreateFromObservable(Login, CanExecLogin());
    }

    private IObservable<IRoutableViewModel> Login() => LoginAsync()
        .Where(result => result)
        .ObserveOn(RxApp.TaskpoolScheduler)
        .SelectMany(_ => HostScreen.Router.Navigate.Execute(Locator.Current.GetService<HomeViewModel>()!));

    private IObservable<bool> LoginAsync() => Observable.FromAsync(async (cancellationToken) =>
    {
        var authResponse = await _identityWebApi.LoginAsync(LoginRequest, cancellationToken);
        if (!authResponse.IsSuccessStatusCode)
        {
            ErrorMessage = "Invalid username or password";
            return false;
        }

        string token = authResponse.Content!;
        var tokenHandler = new JwtSecurityTokenHandler().ReadJwtToken(token);

        string email = tokenHandler.Claims.First(claim => claim.Type is ClaimTypes.Email).Value;
        StaticStorage.JwtToken = authResponse.Content!;

        var userResponse = await _usersWebApi.GetUserByEmail(email, cancellationToken);
        if (!userResponse.IsSuccessStatusCode)
        {
            ErrorMessage = "Invalid username or password";
            return false;
        }

        StaticStorage.User = userResponse.Content;
        return true;
    });

    private IObservable<bool> CanExecLogin() =>
        this.WhenAnyValue(
            x => x.LoginRequest.Username,
            x => x.LoginRequest.Password,
            (username, password) =>
                !string.IsNullOrWhiteSpace(username) &&
                !string.IsNullOrWhiteSpace(password));
}