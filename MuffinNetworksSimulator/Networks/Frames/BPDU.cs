using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinNetworksSimulator.Networks.Frames
{
    /// <summary>
    /// Описывает фрейм STP протокола
    /// </summary>
    class BPDU : Frame
    {
        public int BridgeID {get; set; }                    //ID моста отправителя
        public int RootBridgeId { get; set; }               //ID root моста отправителя
        public int RootPathCost { get; set; }               //Стоимость пути
        public int PortID { get; set; }                     //ID порта моста отправителя

        /// <summary>
        /// Конструктор пакета BPDU
        /// </summary>
        /// <param name="BridgeID">ID моста отправителя</param>
        /// <param name="RootDridgeID">ID root моста в подсети</param>
        /// <param name="PortID">ID порта моста отправителя</param>
        /// <param name="RootPathCost">Стоимость пути до root моста</param>
        public BPDU(int BridgeID, int RootDridgeID, int PortID, int RootPathCost, string SourceAddress)
        {
            this.BridgeID = BridgeID;
            this.RootBridgeId = RootBridgeId;
            this.RootPathCost = RootPathCost;
            this.PortID = PortID;
            this.FrameType = FrameType.BPDU;
            this.SourceAddress = SourceAddress;
        }

        public BPDU()
        {

        }
    }
}
