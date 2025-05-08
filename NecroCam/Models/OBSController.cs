using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using OBSWebsocketDotNet;
using System.IO;

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
                    UseShellExecute = true
                };

                Process.Start(startInfo);
                await Task.Delay(5000);
            }
        }

        public void Connect()
        {
            if (!_obs.IsConnected) _obs.Connect(obsUrl, obsPassword);
        }
        
        public void StartVirutalCam()
        {
            _obs.StartVirtualCam();
        }
        public void StopVirtualCam()
        {
            _obs.StopVirtualCam();
        }

        public void Disconnect()
        {
            if (_obs.IsConnected) _obs.Disconnect(); 
        }
    }
}
