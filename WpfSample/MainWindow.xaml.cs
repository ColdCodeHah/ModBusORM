using ModBusORM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfSample
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window,INotifyPropertyChanged
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }


        private Device1 _device;
        public Device1 Device
        {
            get { return _device; }
            set { _device = value; RaisePropertyChanged("Device"); }
        }

        private uint _year;
        public uint Year
        {
            get { return _year; }
            set { if (value != _year) { _year = value; RaisePropertyChanged("Year"); } }
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {

            ModbusConnection con = new ModbusConnection
            {
                ComNo = "COM2",
                ComType=ComType.SerialPort
            };
            con.Open();
            ModbusDevice<Device1> dev = new ModbusDevice<Device1>();
            con.Register(dev);

            while (true)
            {               
                Device = dev.Read();
                Year = dev.Read(x => x.Year);                
                await Task.Delay(50);
            }
            
        }
    }
}
