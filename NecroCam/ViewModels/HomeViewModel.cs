using System.Windows.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NecroCam.Helpers;
using NecroCam.Services;
using NecroCam.Models;
using System.Windows;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly OBSService _obsService;
    private readonly WebcamController _webcam;

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
    public ICommand ToggleStartCommand { get; }

    public HomeViewModel()
    {
        _obsService = new OBSService();
        _webcam = new WebcamController();

        ToggleStartCommand = new RelayCommand(async () => await ToggleOBSAsync());

        Status = "Aguardando...";
    }

    private async Task ToggleOBSAsync()
    {
        IsButtonEnable = false;

        if (!IsOBSRunning)
        {
            Status = "Iniciando OBS e Webcam...";
            try
            {
                await StartWebcamAsync();
                Status = "Webcam iniciado com sucesso.";
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

    private async Task StartWebcamAsync()
    {
        try
        {
            await _webcam.StartWebcam();
            MessageBox.Show("Video Power Webcam Iniciada", "Success!", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Erro ao iniciar Webcam: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string nome = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
}
