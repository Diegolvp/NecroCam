using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OBSWebsocketDotNet;
using System.IO;
using System.Windows;
using OBSWebsocketDotNet.Types;
using System.Drawing;

namespace NecroCam.Models
{
    class OBSController
    {
        private OBSWebsocket _obs = new OBSWebsocket();
        public bool IsConnect => _obs.IsConnected;

        private string obsPath = @"C:\Program Files\obs-studio\bin\64bit\obs64.exe";
        private string obsUrl = "ws://127.0.0.1:4455";
        private string obsPassword = "";

        public async Task StartOBSAsync()
        {
            if(Process.GetProcessesByName("obs64").Length == 0)
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = obsPath,
                    WorkingDirectory = Path.GetDirectoryName(obsPath),
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Minimized
                };

                Process.Start(startInfo);
                await Task.Delay(5000);
            }
        }

        public void Connect()
        {
            if (!_obs.IsConnected) _obs.ConnectAsync(obsUrl, obsPassword);
        }
        
        public void StartVirutalCam()
        {
            if (_obs.IsConnected)
            {
                try
                {
                    _obs.StartVirtualCam();
                    _obs.SendRequest("StartVirtualCam");
                    Console.WriteLine("Câmera Virtual iniciada.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Erro ao iniciar a câmera virtual: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Nao conectado");
            }
        }
        public async Task StopVirtualCam()
        {
            if (_obs.IsConnected)
            {
                var status = _obs.GetVirtualCamStatus();

                if (status.IsActive)
                {
                    _obs.StopVirtualCam();

                    while (_obs.GetVirtualCamStatus().IsActive)
                    {
                        await Task.Delay(500);
                    }
                }
            }
        }

        public void Disconnect()
        {
            if (_obs.IsConnected) _obs.Disconnect(); 
        }

        public void CloseOBS()
        {
            Process[] process = Process.GetProcessesByName("obs64");

            if (process.Length == 0)
            {
                MessageBox.Show("OBS Studio não está em execução.", "Aviso", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show("OBS Studio Nao encontrado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
