using System.Windows.Input;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NecroCam.Helpers;
using NecroCam.Services;

public class HomeViewModel : INotifyPropertyChanged
{
    private readonly OBSService _obsService;

    public ICommand StartCommand { get; }
    public ICommand StopCommand { get; }

    private string _status;
    public string Status
    {
        get => _status;
        set { _status = value; OnPropertyChanged(); }
    }

    public HomeViewModel()
    {
        _obsService = new OBSService();

        StartCommand = new RelayCommand(async () => await StartOBS());
        
        Status = "Aguardando...";
    }

    private async Task StartOBS()
    {
        try
        {
            Status = "Iniciando OBS...";
            await _obsService.StartStreamingSetupAsync();
            Status = "OBS iniciado com sucesso.";
        }
        catch (Exception ex)
        {
            Status = $"Erro: {ex.Message}";
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string nome = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nome));
}
