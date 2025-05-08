using NecroCam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _obsController.Connect();

            if (!_obsController.IsConnect)
                throw new System.Exception("Não foi possível conectar ao OBS.");

            _obsController.StartVirutalCam();
        }

        public void StopStreaming()
        {
            _obsController.StopVirtualCam();
            _obsController.Disconnect();
        }

        public bool IsConnected => _obsController.IsConnect;
    }
}
