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
            PhysicalLayer PhysicalLayer = new PhysicalLayer();
            BPDU BPDU = new BPDU();
            foreach (var Port in Device.DataPorts)
            {
                BPDU = new BPDU(Device.Id, ((Switch)Device).DeviceIDToRetranslate, Port.ID, ((Switch)Device).PathCostToRetranslate, Device.MACAdress);
                
                //Если в порт подключено устройство и порт не имеет root роли
                if (Port.Device != null && ((Switch)Device).RootSwitch) PhysicalLayer.ExecuteProtocol(new SF(), Port.Device.DeviceObject, BPDU);

                if (Port.Device != null && Port.PortStpRole.Equals(PortSTPRole.NondesignatedPort)) Port.PortStpRole = PortSTPRole.DisabledPort;
            }       
        }

        /// <summary>
        /// Обработка пакетов по правилам STP
        /// </summary>
        /// <param name="Cash">Кэш устройства</param>
        public void Processing(List<Frame> Cash, Device Device)
        {
            //Инициализация переменных для проверки соединения с root мостом
            if (!((Switch)Device).RootSwitch)
            {
                ((Switch)Device).RSConnnection = false;
                ((Switch)Device).RSConnectionCheck += 1;
            } 
            else ((Switch)Device).RSConnnection = true;      

            //Обнаружение root моста и изменение полей BPDU пакета
            foreach (var Frame in Cash)
            {
                if (Frame.FrameType.Equals(FrameType.BPDU))
                {
                    if (((BPDU)Frame).RootBridgeId < ((Switch)Device).DeviceIDToRetranslate)
                    {
                        ((Switch)Device).RootSwitch = false;
                        ((Switch)Device).DeviceIDToRetranslate = ((BPDU)Frame).RootBridgeId;
                        ((Switch)Device).PathCostToRetranslate = ((BPDU)Frame).RootPathCost + 1;

                        foreach (var Port in Device.DataPorts) if (Port.Device != null)
                        {
                            if (Port.Device.DeviceObject.MACAdress == ((BPDU)Frame).SourceAddress)
                            {
                                Port.PortStpRole = PortSTPRole.RootPort;
                                Port.Device.DeviceObject.DataPorts[((BPDU)Frame).PortID].PortStpRole = PortSTPRole.DesignatedPort;
                            }
                        }  

                        PhysicalLayer PhysicalLayer = new PhysicalLayer();
                        BPDU BPDU = new BPDU();

                        //Ретрансляция пакета
                        foreach (var Port in Device.DataPorts)
                        {
                            BPDU = new BPDU(Device.Id, ((Switch)Device).DeviceIDToRetranslate, Port.ID, ((Switch)Device).PathCostToRetranslate, Device.MACAdress);
                            if (Port.Device != null && !Port.PortStpRole.Equals(PortSTPRole.RootPort)) PhysicalLayer.ExecuteProtocol(new SF(), Port.Device.DeviceObject, BPDU);
                        }
                    }
                }               
            }

            //Ретрансляция дальше полученного пакета от root моста
            foreach (var Frame in Cash)
            {
                if (Frame.FrameType.Equals(FrameType.BPDU))
                {
                    bool exit = false;
                    if (((BPDU)Frame).RootBridgeId == ((Switch)Device).DeviceIDToRetranslate)
                    {
                        PhysicalLayer PhysicalLayer = new PhysicalLayer();
                        BPDU BPDU = new BPDU();

                        foreach (var Port in Device.DataPorts)
                        {
                            BPDU = new BPDU(Device.Id, ((Switch)Device).DeviceIDToRetranslate, Port.ID, ((Switch)Device).PathCostToRetranslate, Device.MACAdress);
                            if (Port.Device != null && !Port.PortStpRole.Equals(PortSTPRole.RootPort))
                            {
                                //MessageBox.Show("Попёр спам!!");
                                PhysicalLayer.ExecuteProtocol(new SF(), Port.Device.DeviceObject, BPDU);
                                exit = true;
                            }
                        }
                        if (exit) break;
                    }
                }                    
            }

            //Переключение root роли, если находится порт с меньшим путём до root моста
            foreach (var Frame in Cash)
            {
                if (Frame.FrameType.Equals(FrameType.BPDU))
                {
                    if (((BPDU)Frame).RootBridgeId == ((Switch)Device).DeviceIDToRetranslate && ((BPDU)Frame).RootPathCost < ((Switch)Device).PathCostToRetranslate - 1)
                    {
                        foreach (var Port in Device.DataPorts)
                        {
                            if (Port.Device != null)
                            {
                                if (Port.PortStpRole.Equals(PortSTPRole.RootPort)) Port.PortStpRole = PortSTPRole.NondesignatedPort;
                                if (Port.Device.DeviceObject.MACAdress == ((BPDU)Frame).SourceAddress)
                                {
                                    Port.PortStpRole = PortSTPRole.RootPort;
                                    ((Switch)Device).PathCostToRetranslate = ((BPDU)Frame).RootPathCost + 1;
                                    Port.Device.DeviceObject.DataPorts[((BPDU)Frame).PortID].PortStpRole = PortSTPRole.DesignatedPort;
                                }
                            }
                        }
                    }
                }                    
            }

            //Проверка соединения с root мостом
            foreach (var Frame in Cash)
            {
                if (Frame.FrameType.Equals(FrameType.BPDU))
                {
                    if (((BPDU)Frame).RootBridgeId == ((Switch)Device).DeviceIDToRetranslate)
                    {
                        foreach (var Port in Device.DataPorts)
                        {
                            if(Port.Device != null)
                            {
                                if (Port.Device.DeviceObject.MACAdress == ((BPDU)Frame).SourceAddress && Port.PortStpRole.Equals(PortSTPRole.RootPort) && !((Switch)Device).RootSwitch)
                                {
                                    ((Switch)Device).RSConnnection = true;
                                }
                            }                            
                        }
                    }
                }
            }

            //Контрольная проверка соедиения с root мостом
            if (!((Switch)Device).RSConnnection && ((Switch)Device).RSConnectionCheck == 10)
            {
                ((Switch)Device).RootSwitch = true;
                ((Switch)Device).DeviceIDToRetranslate = ((Switch)Device).Id;
                ((Switch)Device).PathCostToRetranslate = 0;

                foreach (var Port in Device.DataPorts) Port.PortStpRole = PortSTPRole.NondesignatedPort;

                ((Switch)Device).RSConnectionCheck = 0;
            }

            if (((Switch)Device).RSConnectionCheck == 10) ((Switch)Device).RSConnectionCheck = 0;

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
