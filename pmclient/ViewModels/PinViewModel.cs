using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using pmclient.Helpers;
using pmclient.Settings;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Splat;

namespace pmclient.ViewModels;

public class PinViewModel : ViewModelBase, IRoutableViewModel
{
    private const int PinLength = 4;
    private string _correctPin;
    private bool _isNewPin;

    [Reactive] public string Status { get; set; }

    public ObservableCollection<string> EnteredDigits { get; set; }

    public IScreen HostScreen { get; }

    public string UrlPathSegment => "/pin";

    public ICommand EnterDigitCommand { get; }

    public ICommand BackspaceCommand { get; }

    public PinViewModel(IScreen? hostScreen = null)
    {
        var pin = UserSettings.Pin;
        if (pin == null) SetPin();
        _correctPin = pin!;
        HostScreen = hostScreen ?? Locator.Current.GetService<IScreen>()!;

        EnteredDigits = new ObservableCollection<string>(Enumerable.Repeat("", PinLength));

        EnterDigitCommand = ReactiveCommand.CreateFromTask<string>(EnterDigit);
        BackspaceCommand = ReactiveCommand.Create(Backspace);
    }

    private async Task EnterDigit(string digit)
    {
        for (var i = 0; i < PinLength; i++)
        {
            if (string.IsNullOrEmpty(EnteredDigits[i]))
            {
                EnteredDigits[i] = digit;
                break;
            }
        }

        if (GetEnteredDigitCount() == PinLength)
        {
            if (_isNewPin)
            {
                await SavePin();
                return;
            }

            OnPinCompleted();
        }
    }

    private void Backspace()
    {
        for (var i = PinLength - 1; i >= 0; i--)
        {
            if (!string.IsNullOrEmpty(EnteredDigits[i]))
            {
                EnteredDigits[i] = "";
                break;
            }
        }

        Status = "";
    }

    private void SetPin()
    {
        Status = "Введите новый пин-код";

        _isNewPin = true;
    }

    private async Task SavePin()
    {
        Status = "Пин-код установлен";
        _isNewPin = false;
        var newPin = string.Concat(EnteredDigits);
        await FileStorage.SaveFileAsync(newPin, "pin.dat");
        UserSettings.Pin = newPin;
        _correctPin = newPin;

        Observable
            .Timer(TimeSpan.FromMilliseconds(500))
            .ObserveOn(RxApp.MainThreadScheduler)
            .Subscribe(_ =>
            {
                Status = "Повторите пин-код";
                for (var i = 0; i < PinLength; i++)
                    EnteredDigits[i] = "";
            });
    }

    private void OnPinCompleted()
    {
        var pin = string.Concat(EnteredDigits);

        if (pin == _correctPin)
        {
            Status = "Пин-код верен";
            Observable
                .Timer(TimeSpan.FromMilliseconds(300))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ => HostScreen.Router.Navigate.Execute(new HomeViewModel(HostScreen)));
        }
        else
        {
            Status = "Неверный пин-код";

            Observable
                .Timer(TimeSpan.FromMilliseconds(250))
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(_ =>
                {
                    for (var i = 0; i < PinLength; i++)
                        EnteredDigits[i] = "";
                });
        }
    }

    private int GetEnteredDigitCount() =>
        EnteredDigits.Count(d => !string.IsNullOrWhiteSpace(d));
}