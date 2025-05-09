using System.Windows.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NecroCam.Helpers;
using NecroCam.Services;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly OBSService _obsService;

    private string _status;
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    private bool _isOBSRunning;
    public bool IsOBSRunning
    {
        get => _isOBSRunning;
        set
        {
            _isOBSRunning = value;
            OnPropertyChanged(); 
            OnPropertyChanged(nameof(StartStopButtonText));
        }
    }

    private bool _isButtonEnable = true;
    public bool IsButtonEnable
    {
        get => _isButtonEnable;
        set
        {
            _isButtonEnable = value;
            OnPropertyChanged();
        }
    }

    public string StartStopButtonText => IsOBSRunning ? "Stop" : "Start";
    public ICommand ToggleOBSCommand { get; }

    public HomeViewModel()
    {
        _obsService = new OBSService();

        ToggleOBSCommand = new RelayCommand(async () => await ToggleOBSAsync());

        Status = "Aguardando...";
    }

    private async Task ToggleOBSAsync()
    {
        IsButtonEnable = false;

        if (!IsOBSRunning)
        {
            Status = "Iniciando OBS...";
            try
            {
                await _obsService.StartStreamingSetupAsync();
                IsOBSRunning = true;
                Status = "OBS iniciado com sucesso.";
            }
            catch (Exception ex)
            {
                Status = $"Erro: {ex.Message}";
            }
        }
        else
        {
            Status = "Parando OBS...";
            await _obsService.StopStreamingAsync();
            IsOBSRunning = false;
            Status = "OBS parado.";
        }

        IsButtonEnable = true;
    }


    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string nome = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
}
