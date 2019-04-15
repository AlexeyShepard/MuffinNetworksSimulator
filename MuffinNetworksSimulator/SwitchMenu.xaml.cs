using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MuffinNetworksSimulator.Networks.Frames;

namespace MuffinNetworksSimulator
{
    /// <summary>
    /// Логика взаимодействия для SwitchMenu.xaml
    /// </summary>
    public partial class SwitchMenu : Window
    {
        /// <summary>
        /// Инициализация таймера реального времени
        /// </summary>
        //static TimerCallback tm = new TimerCallback(RealTime);
        //Timer timer = new Timer(tm, 0, 0, 1000);
        static CanvasDevice CanvasDeviceCash;

        /// <summary>
        /// Конструктор для инициализации окна
        /// </summary>
        /// <param name="CanvasDevice"></param>
        public SwitchMenu(object CanvasDevice)
        {
            InitializeComponent();
            CanvasDeviceCash = (CanvasDevice)CanvasDevice;
            TxbMacaddress.Text = CanvasDeviceCash.DeviceObject.MACAdress;
            //List<Frame> CashListBPDU = ((CanvasDevice)CanvasDevice).DeviceObject.Cash.ToList();
            DGCash.ItemsSource = ((CanvasDevice)CanvasDevice).DeviceObject.Sniffer.ToList();
            TbIsRoot.Text = ((Switch)CanvasDeviceCash.DeviceObject).RootSwitch.ToString();
            TbBridgePriority.Text = ((Switch)CanvasDeviceCash.DeviceObject).Id.ToString();

            Title = ((CanvasDevice)CanvasDevice).DeviceObject.Type.ToString() + " " + ((CanvasDevice)CanvasDevice).DeviceObject.MACAdress;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        /// <summary>
        /// Срабатывает, каждый интервал срабатывания таймера
        /// </summary>
        /// <param name="obj">Просто, какой объект</param>
        private static void RealTime(object obj)
        {
            //DGCash.ItemsSource = CanvasDeviceCash.DeviceObject.Cash.ToList();          
        }

        /// <summary>
        /// Изменение MAC адреса устройства
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEditMacaddress_Click(object sender, RoutedEventArgs e)
        {
            char[] symbol = new char[17] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', ':' };
            bool Access1 = false, Access2 = false;
            foreach (char chr in TxbMacaddress.Text)
            {
                Access2 = false;
                for (int i = 0; i < 17; i++)
                {
                    if (chr == symbol[i])
                    {
                        Access2 = true;
                        break;
                    }
                }
                if (Access2) Access1 = true;
                else
                {
                    Access1 = false;
                    break;
                }
            }
            if (Access1)
            {
                CanvasDeviceCash.DeviceObject.MACAdress = TxbMacaddress.Text;
                Title = CanvasDeviceCash.DeviceObject.Type.ToString() + " " + CanvasDeviceCash.DeviceObject.MACAdress;
            }
            else
            {
                MessageBox.Show("MAC адрес содержит недопустимые символы!");
                TxbMacaddress.Text = CanvasDeviceCash.DeviceObject.MACAdress;
            }
        }

        /// <summary>
        /// Отмена измененного текста в текстбоксе
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancelMacaddress_Click(object sender, RoutedEventArgs e)
        {
            TxbMacaddress.Text = CanvasDeviceCash.DeviceObject.MACAdress;
        }

        /// <summary>
        /// Сгенерировать случайный MAC адрес
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRandomMacaddress_Click(object sender, RoutedEventArgs e)
        {
            TxbMacaddress.Text = Other.GenerateMacAdress();
        }
    }
}
