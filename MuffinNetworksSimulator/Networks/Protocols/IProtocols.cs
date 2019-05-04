using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuffinNetworksSimulator.Networks.Frames;

namespace MuffinNetworksSimulator.Networks.Protocols
{
    /// <summary>
    /// Интерфейс протокола
    /// </summary>
    interface IProtocols
    {
        /// <summary>
        /// Для исполнения протокола на канальном уровне
        /// </summary>
        /// <param name="Device">Устройство на котором происходит исполнение</param>
        void Execute(Device Device);

        /// <summary>
        /// Для исполнения протокола на физическом/канальном уровне
        /// </summary>
        /// <param name="Device">Устройство на котором происходит исполнение</param>
        /// <param name="Frame">Кадр для обработки</param>
        void Execute(Device Device, Frame Frame);

        /// <summary>
        /// Для обработки кадров/пакетов в зависимости от протокола
        /// </summary>
        void Processing(List<Frame> Cash, Device Device);
    }
}
