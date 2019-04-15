using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuffinNetworksSimulator
{
    /// <summary>
    /// Содержит в себе статические методы для разных нужд
    /// </summary>
    public static class Other
    {
        /// <summary>
        /// Генерация случайного MAC адреса
        /// </summary>
        /// <returns>MAC адрес</returns>
        public static string GenerateMacAdress()
        {
            string MAC = "";
            char[] symbol = new char[16] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' }; 
            Random rnd = new Random();
            int j = 0;
            for (int i = 0; i < 12; i++)
            {
                int value = rnd.Next(0, 16);
                MAC += symbol[value];
                j++;
                if (j == 2 && i != 11)
                {
                    MAC += ":";
                    j = 0;
                }
            }

            return MAC;
        }
    }
}
