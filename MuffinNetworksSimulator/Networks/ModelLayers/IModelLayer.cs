using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuffinNetworksSimulator.Networks.Protocols;

namespace MuffinNetworksSimulator.Networks.ModelLayer
{
    /// <summary>
    /// Описывает функционал уровней модели TCP/IP
    /// </summary>
    interface IModelLayer
    {
        void ExecuteProtocol(IProtocols Protocol, Device Device);
    }
}
