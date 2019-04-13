using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinNetworksSimulator.Networks.Protocols
{
    /// <summary>
    /// Интерфейс протокола
    /// </summary>
    interface IProtocols
    {
        void Execute(Device Device);
    }
}
