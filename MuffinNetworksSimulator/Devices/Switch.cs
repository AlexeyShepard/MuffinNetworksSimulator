using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

/// <summary>
/// Класс описывающий коммутатор
/// </summary>
namespace MuffinNetworksSimulator
{
    class Switch : Device
    {
        /// <summary>
        /// Поля для хранения данных, которые будут использованы при ретрансляции пакетов
        /// </summary>
        public int DeviceIDToRetranslate;   //ID устройства
        public int PathCostToRetranslate;   //Цена пути до Root устройства

        public bool RootSwitch;             //Является ли этот switch root'ом

        public Switch(int id, DeviceType type) : base(id, type)
        {
            this.DeviceIDToRetranslate = this.Id;
            this.PathCostToRetranslate = 0;
            this.RootSwitch = true;
            this.DataPorts = new Port[8];
            for (int i = 0; i < 8; i++) DataPorts[i] = new Port(i);
        }
    }   
}
