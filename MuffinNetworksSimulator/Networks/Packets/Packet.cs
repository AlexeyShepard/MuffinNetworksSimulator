using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuffinNetworksSimulator.Networks.Frames;

namespace MuffinNetworksSimulator.Networks.Packets
{
    enum PacketType
    {
        Ip = 0
    }

    /// <summary>
    /// Абстрактный класс для пакета сетевого уровня
    /// </summary>
    abstract class Packet
    {
        public string SourceAddress { get; set; }               //Адрес источника
        public string DestinationAdress { get; set; }           //Адрес назначения
        public PacketType PacketType { get; set; }              //Тип пакета
        public TimeSpan Time { get; set; }                      //Время прибытия кадра
    }
}
