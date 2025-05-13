using NecroCam.Services;
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

        public async Task<bool> StartWebcam()
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
                    Process? proc = Process.Start(startInfo);
                    await Task.Delay(5000);

                    int retries = 10;

                    while(proc.MainWindowHandle == IntPtr.Zero && retries-- > 0)
                    {
                        await Task.Delay(500);
                        proc.Refresh();
                    }

                    if(proc.MainWindowHandle != IntPtr.Zero)
                    {
                        NativeMethodsService.SetWindowPos(proc.MainWindowHandle, NativeMethodsService.HWND_BOTTOM, 0, 0, 0, 0,
                        NativeMethodsService.SWP_NOMOVE | NativeMethodsService.SWP_NOSIZE | NativeMethodsService.SWP_NOACTIVATE);
                    }

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

        public bool GetWebcamIsRunning()
        {
            Process[] process = Process.GetProcessesByName("videopower");

            return process.Length != 0 ? true : false;
        }

        public async Task<bool> WebcamIsRunningAsync()
        {
            try
            {
                bool isOpen = GetWebcamIsRunning();
                return await Task.FromResult(isOpen);
            }
            catch
            {
                return false;
            }
        }
    }
}
