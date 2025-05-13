using System.Windows.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NecroCam.Helpers;
using NecroCam.Services;
using NecroCam.Models;
using System.Windows;
using System.Windows.Threading;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly OBSService _obsService;
    private readonly WebcamController _webcam;
    private readonly DispatcherTimer _statusCheckTimer;

    private string _status;
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    private string _allServicesStatusColor;
    public string AllServiceStatusColor
    {
        get => _allServicesStatusColor;
        set
        {
            _allServicesStatusColor = value;
            OnPropertyChanged();
        }
    }

    private string _allServicesStatus;
    public string AllServiceStatus
    {
        get => _allServicesStatus;
        set
        {
            _allServicesStatus = value;
            OnPropertyChanged();
        }
    }

    private string _obsStatus;
    public string ObsStatus
    {
        get => _obsStatus;
        set
        {
            _obsStatus = value;
            OnPropertyChanged();
        }
    }

    private string _vCamStatus;
    public string VCamStatus
    {
        get => _vCamStatus;
        set
        {
            _vCamStatus = value;
            OnPropertyChanged();
        }
    }

    private string _webcamStatus;
    public string WebcamStatus
    {
        get => _webcamStatus;
        set
        {
            _webcamStatus = value;
            OnPropertyChanged();
        }
    }

    //public StatusUpdaterService updateAllServiceStatus;
    //public StatusUpdaterService updateOBSStatus;
    //public StatusUpdaterService updateWebcamStatus;
    //public StatusUpdaterService updateVCamStatus;

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

        _statusCheckTimer = new DispatcherTimer();
        _statusCheckTimer.Interval = TimeSpan.FromSeconds(2);
        _statusCheckTimer.Tick += async (s, e) => await CheckAllStatusesAsync();
        _statusCheckTimer.Start(); 

        //updateAllServiceStatus = new StatusUpdaterService(() => AllServiceIsActiveAsync());
        //updateAllServiceStatus.StatusUpdated += (color) => AllServiceStatus = color;

        //updateOBSStatus = new StatusUpdaterService(() => _obsService.IsConnectedAsync());
        //updateOBSStatus.StatusUpdated += (color) => ObsStatus = color;

        //updateWebcamStatus = new StatusUpdaterService(() => _webcam.WebcamIsRunningAsync());
        //updateWebcamStatus.StatusUpdated += (color) => WebcamStatus = color;

        //updateVCamStatus = new StatusUpdaterService(() => _obsService.VCamIsActiveAsync());
        //updateVCamStatus.StatusUpdated += (color) => VCamStatus = color;
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
        }
        catch(Exception ex)
        {
            MessageBox.Show($"Erro ao iniciar Webcam: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async Task<bool> AllServiceIsActiveAsync()
    {
        try
        {
            bool status = ObsStatus == "Green" && WebcamStatus == "Green" && VCamStatus == "Green";
            return await Task.FromResult(status);
        }
        catch
        {
            return false;
        }
    }

    private async Task CheckAllStatusesAsync()
    {
        bool obsStatus = await _obsService.IsConnectedAsync();
        bool webcamStatus = await _webcam.WebcamIsRunningAsync();
        bool vcamStatus = await _obsService.VCamIsActiveAsync();

        ObsStatus = obsStatus ? "Green" : "Red";
        WebcamStatus = webcamStatus ? "Green" : "Red";
        VCamStatus = vcamStatus ? "Green" : "Red";

        bool allStatusOK = await AllServiceIsActiveAsync();

        AllServiceStatusColor = allStatusOK ? "Green" : "Red";
        AllServiceStatus = allStatusOK ? "Connected" : "Disconnect";

    }


    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string nome = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
}
