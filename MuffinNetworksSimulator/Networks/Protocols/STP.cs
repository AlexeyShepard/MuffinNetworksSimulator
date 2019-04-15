using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuffinNetworksSimulator.Networks.ModelLayer;
using MuffinNetworksSimulator.Networks.Frames;

namespace MuffinNetworksSimulator.Networks.Protocols
{
    /// <summary>
    /// Описывает функционал протокола STP
    /// </summary>
    class STP : IProtocols
    {
        /// <summary>
        /// Исполнение протокола STP
        /// </summary>
        /// <param name="Device">Исполняемое устройство</param>
        public void Execute(Device Device)
        {
            foreach(var Port in Device.DataPorts)
            {
                PhysicalLayer PhysicalLayer = new PhysicalLayer();
                if(Port.Device != null) PhysicalLayer.ExecuteProtocol(new SF(), Port.Device.DeviceObject, new BPDU(Device.MACAdress));
            }       
        }

        /// <summary>
        /// Обработка пакетов по правилам STP
        /// </summary>
        /// <param name="Cash">Кэш устройства</param>
        public void Processing(List<Frame> Cash)
        {

        }

        /// <summary>
        /// Чтобы было
        /// </summary>
        public void Execute(Device Device, Frame Frame)
        {
            throw new Exception("Данный метод не реализуется протоколом STP");
        }
    }
}
