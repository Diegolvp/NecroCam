using System.Windows.Threading;

public class StatusUpdaterService
{
    private readonly DispatcherTimer _statusCheckTimer;
    private readonly Func<Task<bool>> _checkServiceStatus;

    public string ColorStatus { get; private set; }

    public event Action<string> StatusUpdated;

    public StatusUpdaterService(Func<Task<bool>> checkServiceStatus)
    {
        _checkServiceStatus = checkServiceStatus;

        _statusCheckTimer = new DispatcherTimer();
        _statusCheckTimer.Interval = TimeSpan.FromSeconds(2);
        _statusCheckTimer.Tick += StatusCheckTimer_Tick;
        _statusCheckTimer.Start();
    }

    private async void StatusCheckTimer_Tick(object sender, EventArgs e)
    {
        try
        {
            bool status = await _checkServiceStatus();
            string color = status ? "Green" : "Red";

            if (ColorStatus != color)
            {
                ColorStatus = color;
                StatusUpdated?.Invoke(ColorStatus);
            }
        }
        catch
        {
            ColorStatus = "Red";
            StatusUpdated?.Invoke(ColorStatus);
        }
    }
}
