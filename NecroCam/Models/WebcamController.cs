using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NecroCam.Models
{
    public class WebcamController
    {
        private string webcamPath = @"C:\Program Files (x86)\iCam\videopower.exe";

        public async Task<bool> StarWebcam()
        {
            if(Process.GetProcessesByName("videopower").Length == 0)
            {
                try
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo()
                    {
                        FileName = webcamPath,
                        WorkingDirectory = Path.GetDirectoryName(webcamPath),
                        UseShellExecute = true,
                    };
                    Process.Start(startInfo);
                    await Task.Delay(5000);
                    return true;

                }catch(Exception ex)
                {
                    MessageBox.Show($"Erro ao iniciar VideoPower: {ex.Message}", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                    return false;
                }

            }
            return true;

        }

        public void CloseWebcam()
        {
            Process[] process = Process.GetProcessesByName("videopower");

            if (process.Length == 0)
            {
                MessageBox.Show("VideoPower não está em execução.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            try
            {
                foreach (var proc in process)
                {
                    proc.CloseMainWindow();
                }
            }
            catch
            {
                MessageBox.Show("VideoPower Nao encontrado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}
