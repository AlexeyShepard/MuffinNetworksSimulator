using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuffinNetworksSimulator.Networks.Frames;

namespace MuffinNetworksSimulator.Networks.Protocols
{
    /// <summary>
    /// Описывает функционал протокола SF
    /// </summary>
    class SF : IProtocols
    {
        /// <summary>
        /// Принятие фрейма
        /// </summary>
        /// <param name="Device">Устройство получатель</param>
        /// <param name="Frame">Отправляемый фрейм</param>
        public void Execute(Device Device, Frame Frame)
        {
            //Время отправки фрейма
            Frame.Time = DateTime.Now.TimeOfDay;
            Device.Cash.Add(Frame);
            //Если запущен режим Sniffering
            if (Device.IsSniffering) Device.Sniffer.Add(Frame);
        }

        /// <summary>
        /// Чтобы было
        /// </summary>
        public void Execute(Device Device)
        {
            throw new Exception("Данный метод не реализуется протоколом SF");
        }

        /// <summary>
        /// Чтобы было
        /// </summary>
        public void Processing(List<Frame> Cash, Device Device)
        {
            throw new Exception("Данный метод не реализуется протоколом SF");
        }
    }
}
