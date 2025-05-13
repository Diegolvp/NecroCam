using NecroCam.Models;
using OBSWebsocketDotNet.Types;
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
                    MessageBox.Show("Nao conectado", "Erro", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                if (_obsController.IsConnect)
                    break;

                await Task.Delay(1000);
            }
            _obsController.StartVirutalCam();

            if (!_obsController.IsConnect)
                throw new System.Exception("Não foi possível conectar ao OBS.");
            
        }

        public async Task StopStreamingAsync()
        { 
            await _obsController.StopVirtualCam();
            _obsController.Disconnect();
            _obsController.CloseOBS();
        }


        public bool IsConnected => _obsController.IsConnect;

        public async Task<bool> IsConnectedAsync()
        {
            try
            {
                return await Task.FromResult(_obsController.IsConnect);
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> VCamIsActiveAsync()
        {
            try
            {
                var status = _obsController.GetVirtualCamStatus();
                return await Task.FromResult(status?.IsActive ?? false);
            }
            catch
            {
                return false;
            }
        }
    }
}
