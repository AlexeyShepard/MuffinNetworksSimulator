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
    /// Описывает сетевой уровень
    /// </summary>
    class NetworkLevel : IModelLayer
    {
        public void ExecuteProtocol(IProtocols Protocol, Device Device, Frame Frame)
        {
            throw new Exception("Запрещено использование данного метода, на сетевом уровне!");
        }

        public void ExecuteProtocol(IProtocols Protocol, Device Device)
        {
            throw new Exception("Запрещено использование данного метода, на сетевом уровне!");
        }

        public void ProccessingCash(IProtocols Protocol, List<Frame> Cash, Device Device)
        {
            throw new Exception("Запрещено использование данного метода, на сетевом уровне!");
        }
    }
}
