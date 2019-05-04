using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuffinNetworksSimulator.Networks.Frames;
using MuffinNetworksSimulator.Networks.ModelLayer;
using System.Windows;

namespace MuffinNetworksSimulator.Networks.Protocols
{
    class Ethernet : IProtocols
    {
        /// <summary>
        /// Отправка Ethernet кадра
        /// </summary>
        /// <param name="Device">Устройство отправитель</param>
        /// <param name="Frame">Кадр для отправления</param>
        public void Execute(Device Device, Frame Frame)
        {
            bool RecordExist = false; //Cуществование записи

            PhysicalLayer physicalLayer = new PhysicalLayer();

            foreach (var RouteRecord in ((Switch)Device).RoutingTable)
            {
                if (RouteRecord.DestinationAddress == Frame.DestinationAdress)
                {
                    //Установка id порта прибытия для кадра
                    foreach (var Port in ((Switch)Device).DataPorts[RouteRecord.PortId].Device.DeviceObject.DataPorts)
                    {
                        if(Port.Device != null)
                        {
                            if (((Switch)Device).Id.Equals(Port.Device.DeviceObject.Id)) ((Networks.Frames.Ethernet)Frame).PortDstId = Port.ID;
                        }                      
                    }
                    physicalLayer.ExecuteProtocol(new SF(), ((Switch)Device).DataPorts[RouteRecord.PortId].Device.DeviceObject, Frame);
                    RecordExist = true;
                    break;
                }
            }

            //Если нету записи в таблице маршрутизации 
            if (!RecordExist)
            {
                foreach (var Port in ((Switch)Device).DataPorts)
                {
                    if (Port.Device != null && !Port.PortStpRole.Equals(PortSTPRole.DisabledPort))
                    {
                        //Установка id порта отправителя для кадра
                        foreach (var Portt in Port.Device.DeviceObject.DataPorts)
                        {
                            if(Portt.Device != null)
                            {
                                if (((Switch)Device).Id.Equals(Portt.Device.DeviceObject.Id)) ((Networks.Frames.Ethernet)Frame).PortDstId = Portt.ID;
                            }                         
                        }
                        physicalLayer.ExecuteProtocol(new SF(), Port.Device.DeviceObject, Frame);
                    }
                }
            }              
        }

        /// <summary>
        /// Чтобы было
        /// </summary>
        public void Execute(Device Device)
        {
            throw new Exception("Данный метод не реализуется протоколом Ethernet");
        }

        /// <summary>
        /// Обработка кадров протоколом ethernet
        /// </summary>
        /// <param name="Cash">Кэш устройства</param>
        /// <param name="Device">Устройство</param>
        public void Processing(List<Frame> Cash, Device Device)
        {
            bool RecordExist = false; //Cуществование записи

            PhysicalLayer physicalLayer = new PhysicalLayer();

            foreach (var Frame in Cash)
            {             
                if (Frame.FrameType.Equals(FrameType.Ethernet))
                {
                    //Если кадр пришёл в пункт назначения
                    if (Device.MACAdress.Equals(Frame.DestinationAdress) && !((Networks.Frames.Ethernet)Frame).ACK)
                    {
                        bool RouteExist = false;
                        EthernetRoutingTableRecord ethernetRoutingTableRecord = new EthernetRoutingTableRecord(Frame.SourceAddress, ((Networks.Frames.Ethernet)Frame).PortDstId);                  
                        /*foreach (var RouteRecord in ((Switch)Device).RoutingTable)
                        {
                            if (RouteRecord.Equals(ethernetRoutingTableRecord)) RouteExist = true;
                        } 
                        if (!RouteExist)*/ ((Switch)Device).RoutingTable.Add(ethernetRoutingTableRecord);
                        Networks.Frames.Ethernet ethernet = new Networks.Frames.Ethernet(Frame.DestinationAdress, Frame.SourceAddress, FrameType.Ethernet, DateTime.Now.TimeOfDay);
                        ethernet.ACK = true;
                        this.Execute(Device, ethernet);
                    }
                    //Иначе, если это не пункт назначения
                    else
                    {
                        foreach(var RouteRecord in ((Switch)Device).RoutingTable)
                        {
                            if (RouteRecord.DestinationAddress == Frame.DestinationAdress)
                            {
                                foreach(var Port in ((Switch)Device).DataPorts[RouteRecord.PortId].Device.DeviceObject.DataPorts)
                                {
                                    if(Port.Device != null)
                                    {
                                        if (((Switch)Device).Id.Equals(Port.Device.DeviceObject.Id)) ((Networks.Frames.Ethernet)Frame).PortDstId = Port.ID;
                                    }
                                }
                                physicalLayer.ExecuteProtocol(new SF(), ((Switch)Device).DataPorts[RouteRecord.PortId].Device.DeviceObject, Frame);
                                RecordExist = true;
                                break;
                            }
                        }

                        //Если нету записи в таблице маршрутизации 
                        if (!RecordExist)
                        {
                            bool RouteExist = false;
                            EthernetRoutingTableRecord ethernetRoutingTableRecord = new EthernetRoutingTableRecord(Frame.SourceAddress, ((Networks.Frames.Ethernet)Frame).PortDstId);
                            /*foreach (var RouteRecord in ((Switch)Device).RoutingTable)
                            {
                                if (RouteRecord.Equals(ethernetRoutingTableRecord)) RouteExist = true;
                            }
                            if (!RouteExist)*/ ((Switch)Device).RoutingTable.Add(ethernetRoutingTableRecord);
                            foreach (var Port in ((Switch)Device).DataPorts)
                            {
                                if (Port.Device != null && !Port.PortStpRole.Equals(PortSTPRole.DisabledPort) && Port.ID != ((Networks.Frames.Ethernet)Frame).PortDstId)
                                {
                                    foreach (var Portt in Port.Device.DeviceObject.DataPorts)
                                    {
                                        if(Portt.Device != null)
                                        {
                                            if (((Switch)Device).Id.Equals(Portt.Device.DeviceObject.Id)) ((Networks.Frames.Ethernet)Frame).PortDstId = Portt.ID;
                                        }                                      
                                    }
                                    physicalLayer.ExecuteProtocol(new SF(), Port.Device.DeviceObject, Frame);
                                }
                            }
                        }
                    } 
                }
            }
        }
    }   
}
