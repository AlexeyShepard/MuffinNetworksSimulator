using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinNetworksSimulator.Networks.Frames
{
    /// <summary>
    /// Описывает фрейм ethernet протокола
    /// </summary>
    class Ethernet : Frame
    {
        /// <summary>
        /// Конструктор ethernet фрейма для проверки подключения
        /// </summary>
        /// <param name="SrcAddress">Адрес отправителя</param>
        /// <param name="DstAddress">Адрес получателя</param>
        /// <param name="Type">Тип фрейма</param>
        /// <param name="timeSpan">Время прибытия</param>
        public Ethernet(string SrcAddress, string DstAddress, FrameType Type, TimeSpan timeSpan)
        {
            this.SourceAddress = SrcAddress;
            this.DestinationAdress = DstAddress;
            this.FrameType = Type;
            this.Time = timeSpan;
        }         
    }
}
