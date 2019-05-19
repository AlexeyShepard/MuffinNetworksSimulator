using System;
using MuffinNetworksSimulator.Wires;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MuffinNetworksSimulator.Networks.Frames;
using MuffinNetworksSimulator.Networks.ModelLayer;
using MuffinNetworksSimulator.Networks.Protocols; 

namespace MuffinNetworksSimulator
{
      
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        
        /// <summary>
        /// Перечисление всех режимов работы
        /// </summary>
        enum ToolMode
        {
            Cursor = 0,
            Delete
        }
        /// <summary>
        /// Перечисление всех возможных выбранных устройств
        /// </summary>
        enum DeviceSelected
        {
            Computer = 0,
            Router,
            Switch,
            Wire,
            Nothing
        }
        /// <summary>
        /// Перечисление отображающие процесс добавления витой пары
        /// </summary>
        enum AddWire
        {
            StartPoint = 0,
            LastPoint
        }

        /// <summary>
        /// Инициализация таймера
        /// </summary>
        static TimerCallback tm = new TimerCallback(RealTime);
        Timer timer = new Timer(tm, 0, 0, 1000);

        /*-----------------------------------------------------------------------------------------------------------------------------*/
        /*--------------------------------------------------------------ПЕРЕМЕННЫЕ-----------------------------------------------------*/
        /*-----------------------------------------------------------------------------------------------------------------------------*/

        static List<CanvasDevice> CanvasDeviceList = new List<CanvasDevice>();      //Лист хранящий в себе информацию обо всех объектах находящихся на канвасе
        static List<CanvasWire> CanvasWireList = new List<CanvasWire>();            //Лист хранящий в себе информацию обо всех проводах находящихся на канвасе

        static ToolMode CurrentMode;                                                //Переменная отображающая, какой режим выбран на данный момент
        static DeviceSelected CurrentDeviceSelected;                                //Переменная отображающая, какое устройство сейчас выбранно на добавление
        static AddWire AddWireState;                                                //Переменная отображающая, в каком состоянии находится добавлени витой пары

        public int SelectedCanvasObjectId;                                          //Хранится id выделенного объекта канваса
        public int LastId = 0;                                                      //Хранится самый последний введенный id
        public bool IsMoving = false;                                               //Хранится информация двигается объект или нет
        public bool IsSelecting = false;                                            //Хранится информация выбран объект или нет

        /// <summary>
        /// Переменные для кэширования
        /// </summary>

        public Path CashWire;                                                       //Хранится графическая часть провода, которые в режиме подключения
        public object CashCanvasDevice;                                             //Записываются данные графического представления устройства при нажатии при добавлении провода
        public Point CashStartPoint;                                                //Хранит начальную координату провода
        public object CashDeciceFisrt;                                              //Заполнение портов устройствами
        public object CashPort;                                                     //Хранит в себе кэш порта
        public bool AddWireAccess = false;                                          //Для проверки, есть ли свободные порты      
        public double StartLocationX, StartLocationY;                               //Хранят начальные координаты передвижения объекта канваса

        /*-----------------------------------------------------------------------------------------------------------------------------*/
        /*--------------------------------------------------------------СОБЫТИЯ--------------------------------------------------------*/
        /*-----------------------------------------------------------------------------------------------------------------------------*/

        /// <summary>
        /// Конструктор главного окна, срабатывает при запуске программы
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            //Инициализация логики окна
            LbTools.SelectedIndex = 0;
            CurrentMode = ToolMode.Cursor;
            CurrentDeviceSelected = DeviceSelected.Nothing;
            AddWireState = AddWire.StartPoint;

            
        }

        /// <summary>
        /// Замена мышки на крестик, если выбран объект LbObjects, при наведении на канвас
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!CurrentDeviceSelected.Equals(DeviceSelected.Nothing)) this.Cursor = Cursors.Cross;            
        }

        /// <summary>
        /// Изменение в CurrentDeviceSelected в зависимости от выбранного объекта в LbObjects
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LbObjects_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (LbObjects.SelectedIndex)
            {
                case -1: CurrentDeviceSelected = DeviceSelected.Nothing; break;
                case 0: CurrentDeviceSelected = DeviceSelected.Computer; break;
                case 1: CurrentDeviceSelected = DeviceSelected.Router; break;
                case 2: CurrentDeviceSelected = DeviceSelected.Switch; break;
                case 3: CurrentDeviceSelected = DeviceSelected.Wire; break;
            }
        }

        /// <summary>
        /// Изменение ToolMode в зависимости от выбранного объекта в LbTools
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LbTools_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (LbTools.SelectedIndex)
            {
                case 0:
                    {
                        this.Cursor = Cursors.Arrow;
                        CurrentMode = ToolMode.Cursor;
                        LbObjects.IsEnabled = true;
                        break;
                    }
                case 1:
                    {
                        this.Cursor = Cursors.No;
                        CurrentMode = ToolMode.Delete;

                        LbObjects.IsEnabled = false;
                        LbObjects.SelectedIndex = -1;

                        CurrentDeviceSelected = DeviceSelected.Nothing;
                        break;
                    }
            }
        }

        /// <summary>
        /// Установка Cursur.Cross при отведения курсора с CvsWorkspace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_MouseLeave(object sender, MouseEventArgs e)
        {
            //Установка курсора если не выбран объект в LbObjects и выбрана ToolMode.Cursor
            if (!CurrentDeviceSelected.Equals(DeviceSelected.Nothing) && CurrentMode.Equals(ToolMode.Cursor)) this.Cursor = Cursors.Arrow;
            //Отменяет перетягивание объекта
            if (IsMoving) IsMoving = false;
        }

        /// <summary>
        /// Выделение объекта на CvsWorkspace в зависимости от режима работа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasObject_LeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            switch (CurrentMode)
            {
                case ToolMode.Cursor: Select_CanvasDevice(sender); break;
                case ToolMode.Delete: Delete_CanvasDevice(sender); break;
            }
        }


        /// <summary>
        /// Добавление объекта в CvsWorkspace, 
        /// Cнятие выделения при нажатии за пределами объекта CvsWorkspace
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //Если выбран какой либо объект в LbObject(Кроме wire) и выбран объект Cursor в LbTools
            if (!CurrentDeviceSelected.Equals(DeviceSelected.Nothing) && !CurrentDeviceSelected.Equals(DeviceSelected.Wire) && !CurrentMode.Equals(ToolMode.Delete))
            {
                CvsWorkspace_AddDevice(e);
            }
            //else CanvasObject_EmptyFieldClick();
        }      

        /// <summary>
        /// Отменить добавление на канвас
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!CurrentDeviceSelected.Equals(DeviceSelected.Nothing))
            {
                LbObjects.SelectedIndex = -1;
                CurrentDeviceSelected = DeviceSelected.Nothing;
                this.Cursor = Cursors.Arrow;
            }

            if (AddWireState.Equals(AddWire.LastPoint))
            {
                CvsWorkspace.Children.Remove(CashWire);
                AddWireState = AddWire.StartPoint;
                AddWireAccess = false;
            }

            foreach (var CvsObj in CanvasDeviceList)
            {
                CvsObj.CanvasObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Transparent");
            }

            SelectedCanvasObjectId = -1;
        }
    
        /// <summary>
        /// Вводит в режим перемещения объекта при зажатии мышки по нему
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasObject_MouseDown(object sender, MouseEventArgs e)
        {
            //Если не выбран тип устройства провод
            if (!CurrentDeviceSelected.Equals(DeviceSelected.Wire))
            {
                IsMoving = true;
                StartLocationX = e.GetPosition(CvsWorkspace).X;
                StartLocationY = e.GetPosition(CvsWorkspace).Y;
            }
            //Иначе если выбрано устройство провод
            else if(CurrentDeviceSelected.Equals(DeviceSelected.Wire))
            {
                switch (AddWireState)
                {
                    case AddWire.StartPoint:
                        {
                            CvsWorkspace_AddWire_StartPoint(sender, e);
                            break;
                        }
                    case AddWire.LastPoint:
                        {
                            CvsWorkspace_AddWire_LastPoint(sender, e);
                            break;
                        }
                }              
            }
        }

        /// <summary>
        /// Выводит их режима перемещения объекта при отпускании мышки 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasObject_MouseUp(object sender, MouseEventArgs e)
        {
            IsMoving = false;
        }

        /// <summary>
        /// Перемещение объекта мышкой и провода при добавлении
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_MouseMove(object sender, MouseEventArgs e)
        {
            //Если выделен какой-либо объект и CanvasDeviceList.Count не пуст, осуществляется передвижение и текущий выбранный объект не провод
            if (SelectedCanvasObjectId != -1 && CanvasDeviceList.Count != 0 && IsMoving && !CurrentDeviceSelected.Equals(DeviceSelected.Wire))
            {
                Grid CvsObject = new Grid();
                foreach (var CvsObj in CanvasDeviceList) if(CvsObj.DeviceObject.Id == SelectedCanvasObjectId) CvsObject = CvsObj.CanvasObject;           

                double CashX = Canvas.GetLeft(CvsObject);
                double CashY = Canvas.GetTop(CvsObject);

                CashX += e.GetPosition(CvsWorkspace).X - StartLocationX;
                CashY += e.GetPosition(CvsWorkspace).Y - StartLocationY;

                Canvas.SetLeft(CvsObject, e.GetPosition(CvsWorkspace).X - 25);
                Canvas.SetTop(CvsObject, e.GetPosition(CvsWorkspace).Y - 25);

                //Отвечает за все подключенные провода к перемещаемуму устройства
                Point CashPoint = new Point();
                CanvasDevice CashCanvasDevice = null;
                foreach(var CvsDevice in CanvasDeviceList)
                {
                    if (CvsDevice.DeviceObject.Id == SelectedCanvasObjectId)
                    {
                        CashCanvasDevice = CvsDevice;
                        break;
                    } 
                }
                foreach(var Wire in CanvasWireList)
                {
                    LineGeometry lineGeometry = (LineGeometry)Wire.CanvasObject.Data;
                    CashPoint.X = e.GetPosition(CvsWorkspace).X;
                    CashPoint.Y = e.GetPosition(CvsWorkspace).Y;
                    if (Wire.Device1.Equals(CashCanvasDevice)) lineGeometry.StartPoint = CashPoint;
                    else if (Wire.Device2.Equals(CashCanvasDevice)) lineGeometry.EndPoint = CashPoint;

                }
            }
            else if (AddWireState.Equals(AddWire.LastPoint))
            {
                double PointX = Canvas.GetLeft((UIElement)CashCanvasDevice) + 25;
                double PointY = Canvas.GetTop((UIElement)CashCanvasDevice) + 25;
                Point StartPoint = new Point(PointX, PointY);
                CashStartPoint = StartPoint;
                PointX = e.GetPosition(CvsWorkspace).X;
                PointY = e.GetPosition(CvsWorkspace).Y;
                Point EndPoint = new Point(PointX, PointY);

                LineGeometry lineGeometry = new LineGeometry(StartPoint, EndPoint);
                CashWire.Stroke = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Black");
                CashWire.StrokeThickness = 3;
                CashWire.Data = lineGeometry;             
            }
        }

        /// <summary>
        /// Открытие меню объекта канваса
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasDevice_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Select_CanvasDevice(sender);
            foreach (var CanvasDevice in CanvasDeviceList)
            {
                if (CanvasDevice.CanvasObject.Equals(sender))
                {
                    switch (CanvasDevice.DeviceObject.Type)
                    {
                        case DeviceType.Computer:
                            {
                                break;
                            }
                        case DeviceType.Router:
                            {
                                break;
                            }
                        case DeviceType.Switch:
                            {
                                SwitchMenu SwitchMenu = new SwitchMenu(CanvasDevice);
                                SwitchMenu.Show();
                                break;
                            }
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Закрытие программы, со всеми её окнами
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        /*-----------------------------------------------------------------------------------------------------------------------------*/
        /*--------------------------------------------------------------ПРОЦЕДУРЫ------------------------------------------------------*/
        /*-----------------------------------------------------------------------------------------------------------------------------*/

        /// <summary>
        /// Добавление графического представление устройства на CvsWorkspace
        /// </summary>
        /// <param name="e">Параметры нажатия мышки</param>
        private void CvsWorkspace_AddDevice(MouseButtonEventArgs e)
        {        
            //Возвращает в переменную созданный канвас
            CanvasDevice canvasDevice = CanvasDevice.CreateObject(LastId++, (DeviceType)CurrentDeviceSelected);

            //Присваение событий
            canvasDevice.CanvasObject.MouseLeftButtonDown += CanvasObject_LeftMouseDown;
            canvasDevice.CanvasObject.MouseDown += CanvasObject_MouseDown;
            canvasDevice.CanvasObject.MouseUp += CanvasObject_MouseUp;
            canvasDevice.CanvasObject.MouseRightButtonDown += CanvasDevice_MouseRightButtonDown;

            //Добавление на рабочую область
            CvsWorkspace.Children.Add(canvasDevice.CanvasObject);
            Canvas.SetLeft(canvasDevice.CanvasObject, e.GetPosition(CvsWorkspace).X - 25);
            Canvas.SetTop(canvasDevice.CanvasObject, e.GetPosition(CvsWorkspace).Y - 25);
            Canvas.SetZIndex(canvasDevice.CanvasObject, 1);

            //Добавление в логику программы
            CanvasDeviceList.Add(canvasDevice);
            SelectedCanvasObjectId = canvasDevice.DeviceObject.Id;

            //Возвращение настроек интерфейса
            LbObjects.SelectedIndex = -1;
            CurrentDeviceSelected = DeviceSelected.Nothing;
            this.Cursor = Cursors.Arrow;
            IsSelecting = true;

            //Установка прозрачного фона для объекта канваса
            foreach (var CvsObjj in CanvasDeviceList)
            {
                if (CvsObjj.DeviceObject.Id != SelectedCanvasObjectId) CvsObjj.CanvasObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Transparent");
            }
        }

        /// <summary>
        /// Срабатывает при попытке выделения пустой области
        /// </summary>
        private void CanvasObject_EmptyFieldClick()
        {
            if(SelectedCanvasObjectId != -1)
            {
                foreach (var CvsObj in CanvasDeviceList) CvsObj.CanvasObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Transparent");
                SelectedCanvasObjectId = -1;
                IsSelecting = false;
            }      
        }

        /// <summary>
        /// Выделение объекта
        /// </summary>
        /// <param name="sender">Объект устройства, которое нужно выделить</param>
        private void Select_CanvasDevice(object sender)
        {
            foreach (var CvsObj in CanvasDeviceList)
            {
                if (CvsObj.CanvasObject.Equals(sender))
                {
                    CvsObj.CanvasObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("#b886d1");
                    SelectedCanvasObjectId = CvsObj.DeviceObject.Id;
                }
                else CvsObj.CanvasObject.Background = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Transparent");
            }
        }

        /// <summary>
        /// Удаление объекта
        /// </summary>
        /// <param name="sender">Объект устройства, которое нужно удалить</param>
        private void Delete_CanvasDevice(object sender)
        {
            Select_CanvasDevice(sender);
            foreach (var CvsObj in CanvasDeviceList.ToArray())
            {
                if (CvsObj.CanvasObject.Equals(sender))
                {
                    //Удаление соединенных проводов
                    foreach (var Wire in CanvasWireList.ToArray())
                    {
                        if (Wire.Device1.Equals(CvsObj))
                        {
                            //Освобождение портов
                            foreach (var Port in Wire.Device2.DeviceObject.DataPorts)
                            {
                                //УБРАТЬ УСЛОВИЕ, ЕСЛИ ВОЗНИКАЮТ ОШИБКИ ПРИ УДАЛЕНИИ
                                if (Port.Device != null && Port != null)
                                {
                                    if (Port.Device.Equals(CvsObj))
                                    {
                                        Port.Device = null;
                                        //Port.PortStpRole = PortSTPRole.NondesignatedPort;
                                        break;
                                    }
                                }                                
                            }
                            CanvasWireList.Remove(Wire);
                            CvsWorkspace.Children.Remove(Wire.CanvasObject);
                        }
                        else if (Wire.Device2.Equals(CvsObj))
                        {
                            //Освобождение портов
                            foreach (var Port in Wire.Device1.DeviceObject.DataPorts)
                            {
                                //УБРАТЬ УСЛОВИЕ, ЕСЛИ ВОЗНИКАЮТ ОШИБКИ ПРИ УДАЛЕНИИ
                                if (Port.Device != null && Port != null)
                                {
                                    if (Port.Device.Equals(CvsObj))
                                    {
                                        Port.Device = null;
                                        //Port.PortStpRole = PortSTPRole.NondesignatedPort;
                                        break;
                                    }
                                }                                   
                            }
                            CanvasWireList.Remove(Wire);
                            CvsWorkspace.Children.Remove(Wire.CanvasObject);
                        }
                    }
                    //Удаление устройства
                    CanvasDeviceList.Remove(CvsObj);
                    CvsWorkspace.Children.Remove(CvsObj.CanvasObject);
                    //Отключение таймера
                    CvsObj.DeviceObject.timer.Dispose();
                    SelectedCanvasObjectId = -1;
                }
            }
        }

        /// <summary>
        /// Начало добавления провода на канвас и в логику программы
        /// </summary>
        /// <param name="sender"></param>
        private void CvsWorkspace_AddWire_StartPoint(object sender, MouseEventArgs e)
        {
            foreach(var CvsObj in CanvasDeviceList)
            {
                if (CvsObj.CanvasObject.Equals(sender))
                {
                    foreach(var Port in CvsObj.DeviceObject.DataPorts)
                    {
                        if(Port.Device == null)
                        {
                            AddWireAccess = true;
                            break;
                        }
                    }
                    if (AddWireAccess)
                    {
                        foreach (var Port in CvsObj.DeviceObject.DataPorts)
                        {
                            if (Port.Device == null)
                            {
                                Path Wire = new Path();
                                Wire.Stroke = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Black");
                                Wire.StrokeThickness = 3;
                                double PointX = Canvas.GetLeft((UIElement)sender) + 25;
                                double PointY = Canvas.GetTop((UIElement)sender) + 25;
                                Point StartPoint = new Point(PointX, PointY);

                                PointX = e.GetPosition(CvsWorkspace).X;
                                PointY = e.GetPosition(CvsWorkspace).Y;
                                Point EndPoint = new Point(PointX, PointY);

                                LineGeometry lineGeometry = new LineGeometry(StartPoint, EndPoint);
                                Wire.Data = lineGeometry;
                                CvsWorkspace.Children.Add(Wire);

                                CashWire = Wire;
                                CashCanvasDevice = sender;
                                CashStartPoint = StartPoint;
                                CashPort = Port;

                                CashDeciceFisrt = CvsObj;

                                AddWireState = AddWire.LastPoint;
                                AddWireAccess = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        MessageBox.Show("Отсутствуют свободные порты!");
                        AddWireState = AddWire.StartPoint;
                    }
                    
                }
            }                  
        }

        /// <summary>
        /// Завершение добавления провода на канвас и в логику программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CvsWorkspace_AddWire_LastPoint(object sender, MouseEventArgs e)
        {
            foreach(var CvsObj in CanvasDeviceList)
            {
                if (CvsObj.CanvasObject.Equals(sender))
                {
                    foreach (var Port in CvsObj.DeviceObject.DataPorts)
                    {
                        if (Port.Device == null)
                        {
                            AddWireAccess = true;
                            break;
                        }
                    }
                    if (AddWireAccess)
                    {
                        foreach (var Port in CvsObj.DeviceObject.DataPorts)
                        {
                            if (Port.Device == null)
                            {
                                double PointX = Canvas.GetLeft((UIElement)sender) + 25;
                                double PointY = Canvas.GetTop((UIElement)sender) + 25;
                                Point EndPoint = new Point(PointX, PointY);
                                LineGeometry lineGeometry = new LineGeometry(CashStartPoint, EndPoint);
                                CashWire.Stroke = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Gray");
                                CashWire.StrokeThickness = 3;
                                CashWire.Data = lineGeometry;

                                CanvasWireList.Add(new CanvasWire(CashWire, (CanvasDevice)CashDeciceFisrt, CvsObj));
                                CanvasDevice CashDeciceFisrt1 = (CanvasDevice)CashDeciceFisrt;
                                Port.Device = CashDeciceFisrt1;
                                Port Port1 = (Port)CashPort;
                                Port1.Device = CvsObj;


                                this.Cursor = Cursors.Arrow;
                                LbObjects.SelectedIndex = -1;
                                CurrentDeviceSelected = DeviceSelected.Nothing;
                                AddWireState = AddWire.StartPoint;
                                AddWireAccess = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        CvsWorkspace.Children.Remove(CashWire);
                        AddWireState = AddWire.StartPoint;
                        AddWireAccess = false;
                        MessageBox.Show("Отсутствуют свободные порты!");
                    }
                }
            }                          
        }

        /// <summary>
        /// Срабатывает, каждый интервал срабатывания таймера
        /// </summary>
        /// <param name="obj">Просто, какой объект</param>
        private static void RealTime(object obj)
        {
            foreach(var Wire in CanvasWireList.ToList())
            {
                foreach(var Port in Wire.Device1.DeviceObject.DataPorts)
                {
                    if(Port.Device != null)
                    {
                        if (Port.PortStpRole.Equals(PortSTPRole.DesignatedPort) && Port.Device.Equals(Wire.Device2))
                        {
                            Wire.CanvasObject.Dispatcher.Invoke(new Action(() =>
                            {
                                Wire.CanvasObject.Stroke = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Black");
                            }));
                            break;
                        }
                        else if (Port.PortStpRole.Equals(PortSTPRole.DisabledPort) && Port.Device.Equals(Wire.Device2))
                        {
                            Wire.CanvasObject.Dispatcher.Invoke(new Action(() =>
                            {
                                Wire.CanvasObject.Stroke = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Gray");
                            }));
                        }
                    }                  
                }
                foreach (var Port in Wire.Device2.DeviceObject.DataPorts)
                {
                    if(Port.Device != null)
                    {
                        if (Port.PortStpRole.Equals(PortSTPRole.DesignatedPort) && Port.Device.Equals(Wire.Device1))
                        {
                            Wire.CanvasObject.Dispatcher.Invoke(new Action(() =>
                            {
                                Wire.CanvasObject.Stroke = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Black");
                            }));
                        }
                        else if (Port.PortStpRole.Equals(PortSTPRole.DisabledPort) && Port.Device.Equals(Wire.Device1))
                        {
                            Wire.CanvasObject.Dispatcher.Invoke(new Action(() =>
                            {
                                Wire.CanvasObject.Stroke = (Brush)System.ComponentModel.TypeDescriptor.GetConverter(typeof(Brush)).ConvertFromInvariantString("Gray");
                            }));
                        }
                    }                   
                }
            }   
        }
    }
}
