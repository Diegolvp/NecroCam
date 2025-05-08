using NecroCam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NecroCam.Services
{
    public class OBSService
    {
        private readonly OBSController _obsController;

        public OBSService()
        {
            _obsController = new OBSController();
        }

        public async Task StartStreamingSetupAsync()
        {
            await _obsController.StartOBSAsync();

            while (!_obsController.IsConnect)
            {
                try
                {
                    _obsController.Connect();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Nao conectado");
                }

                if (_obsController.IsConnect)
                    break;

                await Task.Delay(1000);
            }

            _obsController.StartVirutalCam();

            if (!_obsController.IsConnect)
                throw new System.Exception("Não foi possível conectar ao OBS.");
            
        }

        public void StopStreaming()
        {
            _obsController.StopVirtualCam();
            _obsController.Disconnect();
        }

        public bool IsConnected => _obsController.IsConnect;
    }
}
