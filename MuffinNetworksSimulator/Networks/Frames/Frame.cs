using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinNetworksSimulator.Networks.Frames
{
    /// <summary>
    /// Абстрактный класс фрейма для канального уровня
    /// </summary>
    abstract class Frame
    {
        public string SourceAddress { get; set; }           //Адрес источника
        public string DestinationAdress;        //Адрес назначения
    }
}
