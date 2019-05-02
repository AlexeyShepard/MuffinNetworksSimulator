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
    /// Описывает функционал уровней модели TCP/IP
    /// </summary>
    interface IModelLayer
    {            
        /// <summary>
        /// Для реализации протоколов канального уровня
        /// </summary>
        /// <param name="Protocols">Исполняемый протокол</param>
        /// <param name="Device">Устройство для иполнения</param>
        void ExecuteProtocol(IProtocols Protocols, Device Device);

        /// <summary>
        /// Для реализации протоколов физического уровня
        /// </summary>
        /// <param name="Protocols">Исполняемый протокол</param>
        /// <param name="Device">Устройство для исполнения</param>
        /// <param name="Frame">Генерируемый фрейм</param>
        void ExecuteProtocol(IProtocols Protocols, Device Device, Frame Frame);

        /// <summary>
        /// Обработка кэша устройства в зависимости от протокола
        /// </summary>
        /// <param name="Protocols">Протокол обработки</param>
        /// <param name="Cash">Кэш устройства</param>
        void ProccessingCash(IProtocols Protocols, List<Frame> Cash, Device Device);

    }
}
