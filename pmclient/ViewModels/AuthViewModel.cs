using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using pmclient.Services;
using ReactiveUI;
using Splat;
using static pmclient.Helpers.QrCodeGeneratorHelper;

namespace pmclient.ViewModels;

public class AuthViewModel : ViewModelBase, IRoutableViewModel
{
    private TwoFaService? _twoFaService;

    private Bitmap _qrCode;
    private string _errorMessage;
    private bool _isError;
    private bool _isActive;

    public Bitmap QrCode
    {
        get => _qrCode;
        set => this.RaiseAndSetIfChanged(ref _qrCode, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => this.RaiseAndSetIfChanged(ref _isActive, value);
    }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => "/auth";

    public ICommand ChangeCommand { get; }

    public ICommand BackCommand { get; }

    public AuthViewModel(IScreen? screen = null, TwoFaService? twoFaService = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        _twoFaService = twoFaService ?? Locator.Current.GetService<TwoFaService>()!;
        IsActive = !string.IsNullOrEmpty(UserSettings.User!.Secret);
        QrCode = new Bitmap("./Assets/picture.png");

        ChangeCommand = ReactiveCommand.CreateFromTask(Change);
        BackCommand = ReactiveCommand.CreateFromObservable(HostScreen.Router.NavigateBack.Execute);
    }

    private async Task Change(CancellationToken cancellationToken)
    {
        if (!IsActive) await Enable(cancellationToken);
        else await Disable(cancellationToken);

        IsActive = !IsActive;
    }


    private async Task Enable(CancellationToken cancellationToken)
    {
        await _twoFaService!.EnableTwoFaAsync(cancellationToken);

        var user = UserSettings.User;
        var uri = GenerateQrCodeUri(user!.Secret!, user.Email);
        var qrCodeBitmap = GenerateQrCodeImage(uri);
        QrCode = ConvertToBitmap(qrCodeBitmap);
    }

    private async Task Disable(CancellationToken cancellationToken)
    {
        await _twoFaService!.DisableTwoFaAsync(cancellationToken);
        QrCode = new Bitmap("./Assets/picture.png");
    }
}