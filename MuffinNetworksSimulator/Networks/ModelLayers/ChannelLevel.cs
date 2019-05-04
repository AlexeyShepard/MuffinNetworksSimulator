using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuffinNetworksSimulator.Networks.Protocols;
using MuffinNetworksSimulator.Networks.Frames;

namespace MuffinNetworksSimulator.Networks.ModelLayer
{
    /// <summary>
    /// Описывает канальный уровень
    /// </summary>
    class ChannelLevel : IModelLayer
    {
        /// <summary>
        /// Исполнение протокола канального уровня
        /// </summary>
        /// <param name="Protocol">Исполняемый протокол</param>
        /// <param name="Device">Устройство исполнитель</param>
        public void ExecuteProtocol(IProtocols Protocol, Device Device)
        {
            Protocol.Execute(Device);
        }

        /// <summary>
        /// Исполнения протокола канального уровня, в случае если надо передать готовый пакет
        /// </summary>
        public void ExecuteProtocol(IProtocols Protocol, Device Device, Frame Frame)
        {
            Protocol.Execute(Device, Frame);
        }

        /// <summary>
        /// Обработка кэша устройства протколом
        /// </summary>
        /// <param name="Protocol">Протокол обработки</param>
        /// <param name="Cash">Кэш устройства</param>
        public void ProccessingCash(IProtocols Protocol, List<Frame> Cash, Device Device)
        {
            Protocol.Processing(Cash, Device);
        }

    }
}
