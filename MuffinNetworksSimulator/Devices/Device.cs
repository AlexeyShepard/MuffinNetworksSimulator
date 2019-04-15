﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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

        public List<Frame> Cash;                        //Лист хранящий в себе фреймы

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
            this.MACAdress = Other.GenerateMacAdress();
        }
    }
}
