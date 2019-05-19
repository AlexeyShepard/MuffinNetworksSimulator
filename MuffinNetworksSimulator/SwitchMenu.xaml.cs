using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
using MuffinNetworksSimulator.Networks.ModelLayer;
using MuffinNetworksSimulator.Networks.Protocols;

namespace MuffinNetworksSimulator
{
    /// <summary>
    /// Логика взаимодействия для SwitchMenu.xaml
    /// </summary>
    public partial class SwitchMenu : Window
    {
        static CanvasDevice CanvasDeviceCash;   //Кэш для устройства
        static DataGrid DGSnifferCash;          //Кэш таблицы сниффера

        /// <summary>
        /// Конструктор для инициализации окна
        /// </summary>
        /// <param name="CanvasDevice"></param>
        public SwitchMenu(object CanvasDevice)
        {
            InitializeComponent();

            DGSniffer.ItemsSource = new List<Networks.Frames.Frame>();
            DGSnifferCash = DGSniffer;
            CanvasDeviceCash = (CanvasDevice)CanvasDevice;
            DGCash.ItemsSource = CanvasDeviceCash.DeviceObject.Cash.ToList();
            DGRoutingTable.ItemsSource = ((Switch)CanvasDeviceCash.DeviceObject).RoutingTable.ToList();
            TxbMacaddress.Text = CanvasDeviceCash.DeviceObject.MACAdress;
            DGSniffer.ItemsSource = ((CanvasDevice)CanvasDevice).DeviceObject.Sniffer.ToList();
            DGPorts.ItemsSource = ((CanvasDevice)CanvasDevice).DeviceObject.DataPorts.ToList();
            TbIsRoot.Text = ((Switch)CanvasDeviceCash.DeviceObject).RootSwitch.ToString();
            TbBridgePriority.Text = ((Switch)CanvasDeviceCash.DeviceObject).Id.ToString();
            TbRootIdToRetranslate.Text = ((Switch)CanvasDeviceCash.DeviceObject).DeviceIDToRetranslate.ToString();
            TbPathCostToRetranslate.Text = ((Switch)CanvasDeviceCash.DeviceObject).PathCostToRetranslate.ToString();
            TbRSConnection.Text = ((Switch)CanvasDeviceCash.DeviceObject).RSConnnection.ToString();

            Title = ((CanvasDevice)CanvasDevice).DeviceObject.Type.ToString() + " " + ((CanvasDevice)CanvasDevice).DeviceObject.MACAdress;

            if (CanvasDeviceCash.DeviceObject.IsSniffering) BtnSniffering.Content = "Stop Sniffer";
            else BtnSniffering.Content = "Start Sniffer";
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
                Port[] CashDataPort = CanvasDeviceCash.DeviceObject.DataPorts;
                CanvasDeviceCash.DeviceObject = new Switch(CanvasDeviceCash.DeviceObject.Id, DeviceType.Switch);
                CanvasDeviceCash.DeviceObject.DataPorts = CashDataPort;
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

        /// <summary>
        /// Обновление всех полей
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReload_Click(object sender, RoutedEventArgs e)
        {
            BindingOperations.EnableCollectionSynchronization(CanvasDeviceCash.DeviceObject.Sniffer, DGSnifferCash);
            DGSnifferCash.ItemsSource = CanvasDeviceCash.DeviceObject.Sniffer.ToList();
            BindingOperations.EnableCollectionSynchronization(CanvasDeviceCash.DeviceObject.Cash, DGCash);
            DGCash.ItemsSource = CanvasDeviceCash.DeviceObject.Cash.ToList();
            BindingOperations.EnableCollectionSynchronization(((Switch)CanvasDeviceCash.DeviceObject).RoutingTable, DGRoutingTable);
            DGRoutingTable.ItemsSource = ((Switch)CanvasDeviceCash.DeviceObject).RoutingTable.ToList();
            TbIsRoot.Text = ((Switch)CanvasDeviceCash.DeviceObject).RootSwitch.ToString();
            TbRootIdToRetranslate.Text = ((Switch)CanvasDeviceCash.DeviceObject).DeviceIDToRetranslate.ToString();
            TbPathCostToRetranslate.Text = ((Switch)CanvasDeviceCash.DeviceObject).PathCostToRetranslate.ToString();
            TbRSConnection.Text = ((Switch)CanvasDeviceCash.DeviceObject).RSConnnection.ToString();
        }

        /// <summary>
        /// Очищение истории
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClear_Click(object sender, RoutedEventArgs e)
        {
            CanvasDeviceCash.DeviceObject.Sniffer.Clear();
            BindingOperations.EnableCollectionSynchronization(CanvasDeviceCash.DeviceObject.Sniffer, DGSnifferCash);
            DGSnifferCash.ItemsSource = CanvasDeviceCash.DeviceObject.Sniffer.ToList();
        }

        /// <summary>
        /// Включение/Отключение режима снифферинга
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSniffering_Click(object sender, RoutedEventArgs e)
        {
            CanvasDeviceCash.DeviceObject.IsSniffering = !CanvasDeviceCash.DeviceObject.IsSniffering;

            if (CanvasDeviceCash.DeviceObject.IsSniffering) BtnSniffering.Content = "Stop Sniffer";
            else BtnSniffering.Content = "Start Sniffer";
        }

        /// <summary>
        /// Отправка фрейма для проверки связи
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSendEthernetFrame_Click(object sender, RoutedEventArgs e)
        {
            ChannelLevel channelLevel = new ChannelLevel();
            Networks.Frames.Ethernet ethernet = new Networks.Frames.Ethernet(CanvasDeviceCash.DeviceObject.MACAdress, TxbMacaddressToTest.Text, FrameType.Ethernet, DateTime.Now.TimeOfDay);
            channelLevel.ExecuteProtocol(new Networks.Protocols.Ethernet(), CanvasDeviceCash.DeviceObject, ethernet);
        }

        private void BtnResetStatusConnection_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
