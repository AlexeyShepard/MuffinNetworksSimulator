using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinNetworksSimulator
{
    /// <summary>
    /// Роль портов для протокола STP
    /// </summary>
    enum PortSTPRole
    {
        RootPort = 0,
        DesignatedPort,
        NondesignatedPort,
        DisabledPort,
    }
    
    /// <summary>
    /// Физический порт устройства
    /// </summary>
    class Port
    {
        public CanvasDevice Device;                      // Устройство подключенное по другую сторону витой пары
        public int ID { get; set; }                      //ID порта
        public PortSTPRole PortStpRole { get; set; }     //Роль порта в STP протоколе

        /// <summary>
        /// Инициализация порта
        /// </summary>
        public Port(int ID)
        {
            this.ID = ID;
            //Назначаем порт, как не размеченный
            this.PortStpRole = PortSTPRole.NondesignatedPort;    
        }
    }
}
