using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Threading;
using MuffinNetworksSimulator.Networks.ModelLayer;
using MuffinNetworksSimulator.Networks.Protocols;
using MuffinNetworksSimulator;

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

        public bool RSConnnection;          //Соединение с root мостом
        public int RSConnectionCheck;       //Отсчёт для проверки соединения с root мостом


        public Switch(int id, DeviceType type) : base(id, type)
        {
            this.DeviceIDToRetranslate = this.Id;
            this.PathCostToRetranslate = 0;
            this.RootSwitch = true;
            this.DataPorts = new Port[8];
            for (int i = 0; i < 8; i++) DataPorts[i] = new Port(i);

            this.RSConnnection = false;
            this.RSConnectionCheck = 0;

            timer = new Timer(RealTime, 0, 0, 1000);
        }

        /// <summary>
        /// Срабатывает, каждый интервал срабатывания таймера
        /// </summary>
        /// <param name="obj">Просто, какой объект</param>
        private void RealTime(object obj)
        {
            //MessageBox.Show("Работает!");
            ChannelLevel.ExecuteProtocol(new STP(), this);
            ChannelLevel.ProccessingCash(new STP(), this.Cash, this);
        }
    }
}
