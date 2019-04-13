using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MuffinNetworksSimulator.Networks.Protocols;

namespace MuffinNetworksSimulator.Networks.ModelLayer
{
    /// <summary>
    /// Описывает канальный уровень
    /// </summary>
    class ChannelLevel : IModelLayer
    {
        public void ExecuteProtocol(IProtocols Protocol, Device Device)
        {
            Protocol.Execute(Device);                                 
        }                 
    }
}
