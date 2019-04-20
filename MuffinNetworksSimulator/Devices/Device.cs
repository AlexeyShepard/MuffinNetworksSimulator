using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using MuffinNetworksSimulator.Networks.ModelLayer;
using MuffinNetworksSimulator.Networks.Frames;

/// <summary>
/// Класс описывающий общие свойства всех устройств
/// </summary>
namespace MuffinNetworksSimulator
{
    /// <summary>
    /// Перечисление типов устройств
    /// </summary>
    enum DeviceType
    {
        Computer = 0,
        Router,
        Switch
    }

    /// <summary>
    /// Абстрактный класс для всех устройств
    /// </summary>
    abstract class Device
    {
       
        public DeviceType Type;                         // Индекс типа устройства
        public int Id;                                  // Id устройства
        public Port[] DataPorts;                        // Массив портов
        public string MACAdress;                        // MAC адрес устройства для локальной маршрутизации 
        public bool IsSniffering;                       //Включем/Выключен режим снифферинга 

        public List<Frame> Cash;                        //Лист хранящий в себе фреймы
        public List<Frame> Sniffer;                     //Лист хранящий информацию обо всей истории хранимых фреймах на устройстве
        public List<BPDU> CashBPDU;

        /// <summary>
        /// Свойства таймера реального времени
        /// </summary>
        public static TimerCallback tm;
        public Timer timer;

        /// <summary>
        /// Конструктор устройства
        /// </summary>
        /// <param name="id">Уникальный идентификатор</param>
        /// <param name="type">Тип устройства</param>
        public Device(int id, DeviceType type)
        {
            this.Id = id;
            this.Type = type;
            this.Cash = new List<Frame>();
            this.Sniffer = new List<Frame>();
            this.CashBPDU = new List<BPDU>();
            this.MACAdress = Other.GenerateMacAdress();
            this.IsSniffering = false;
        }
    }
}
