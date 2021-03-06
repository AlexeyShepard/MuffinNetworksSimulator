﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuffinNetworksSimulator.Networks.Packets;

namespace MuffinNetworksSimulator.Networks.Frames
{
    enum FrameType{
        Ethernet = 0,
        BPDU,
        TCA,
        TCN
    }
    
    /// <summary>
    /// Абстрактный класс фрейма для канального уровня
    /// </summary>
    abstract class Frame
    {
        public string SourceAddress { get; set; }               //Адрес источника
        public string DestinationAdress { get; set; }           //Адрес назначения
        public FrameType FrameType { get; set; }                //Тип фрейма
        public TimeSpan Time { get; set; }                      //Время прибытия кадра

        public Packet Packet { get; set; }                      //Пакет фрейма
    }
}
