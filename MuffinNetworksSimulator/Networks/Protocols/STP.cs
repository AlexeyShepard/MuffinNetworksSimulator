using System;
using System.Windows;
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
                BPDU BPDU = new BPDU();
                BPDU = new BPDU(Device.Id, ((Switch)Device).DeviceIDToRetranslate, Port.ID, ((Switch)Device).PathCostToRetranslate, Device.MACAdress);
                
                //Если в порт подключено устройство и порт не имеет root роли
                if (Port.Device != null && !Port.PortStpRole.Equals(PortSTPRole.RootPort)) PhysicalLayer.ExecuteProtocol(new SF(), Port.Device.DeviceObject, BPDU);
            }       
        }

        /// <summary>
        /// Обработка пакетов по правилам STP
        /// </summary>
        /// <param name="Cash">Кэш устройства</param>
        public void Processing(List<Frame> Cash, Device Device)
        {
            foreach(var Frame in Cash.ToArray())
            {
                if(((BPDU)Frame).BridgeID < ((Switch)Device).DeviceIDToRetranslate)
                {
                    ((Switch)Device).RootSwitch = false;
                    ((Switch)Device).DeviceIDToRetranslate = ((BPDU)Frame).RootBridgeId; 
                    ((Switch)Device).PathCostToRetranslate = ((BPDU)Frame).RootPathCost + 1;
                }
            }
            Cash.Clear();
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
