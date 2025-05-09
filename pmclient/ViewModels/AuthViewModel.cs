using System.Windows.Input;
using Avalonia.Media.Imaging;
using ReactiveUI;
using Splat;

namespace pmclient.ViewModels;

public class AuthViewModel : ViewModelBase, IRoutableViewModel
{
    private Bitmap _qrCode;
    private string _errorMessage;
    private bool _isError;

    public Bitmap QrCode
    {
        get => _qrCode;
        set => this.RaiseAndSetIfChanged(ref _qrCode, value);
    }

    public string ErrorMessage
    {
        get => _errorMessage;
        set => this.RaiseAndSetIfChanged(ref _errorMessage, value);
    }

    public bool IsError
    {
        get => _isError;
        set => this.RaiseAndSetIfChanged(ref _isError, value);
    }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => "/auth";

    public ICommand GenerateQrCodeCommand { get; }
    
    public ICommand BackCommand { get; }

    public AuthViewModel(IScreen? screen = null)
    {
        HostScreen = screen ?? Locator.Current.GetService<IScreen>()!;

        GenerateQrCodeCommand = ReactiveCommand.Create(GenerateQrCode);
        BackCommand = ReactiveCommand.CreateFromObservable(HostScreen.Router.NavigateBack.Execute);
    }

    private void GenerateQrCode()
    {
    }
}