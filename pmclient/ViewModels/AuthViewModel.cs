using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media.Imaging;
using pmclient.Services;
using pmclient.Settings;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;
using static pmclient.Helpers.QrCodeGeneratorHelper;

namespace pmclient.ViewModels;

public class AuthViewModel : ViewModelBase, IRoutableViewModel
{
    private readonly TwoFaService? _twoFaService;

    [Reactive] public Bitmap QrCode { get; set; }

    [Reactive] public string Secret { get; set; } = string.Empty;

    [Reactive] public bool IsActive { get; set; }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => "/auth";

    public ICommand ChangeCommand { get; }

    public ICommand BackCommand { get; }

    public AuthViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;
        _twoFaService = Locator.Current.GetService<TwoFaService>();
        IsActive = !string.IsNullOrEmpty(UserSettings.User!.Secret);

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
        Secret = user.Secret!;
        QrCode = ConvertToBitmap(qrCodeBitmap);
    }

    private async Task Disable(CancellationToken cancellationToken)
    {
        QrCode = null!;
        Secret = string.Empty;
        await _twoFaService!.DisableTwoFaAsync(cancellationToken);
    }
}