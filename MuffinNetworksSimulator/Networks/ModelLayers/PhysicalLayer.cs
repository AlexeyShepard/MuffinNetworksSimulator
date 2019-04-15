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
    /// Описывает физический уровень
    /// </summary>
    class PhysicalLayer : IModelLayer
    {

        public void ExecuteProtocol(IProtocols Protocol, Device Device, Frame Frame)
        {
            Protocol.Execute(Device, Frame);
        }

        public void ExecuteProtocol(IProtocols Protocol, Device Device)
        {
            throw new Exception("Запрещено использование данного метода, на физическом уровне!");
        }

        public void ProccessingCash(IProtocols Protocol, List<Frame> Cash, Device Device)
        {
            throw new Exception("Запрещено использование данного метода, на физическом уровне!");
        }
    }
}
