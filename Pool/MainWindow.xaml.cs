using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
using System.Windows.Threading;

namespace Pool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        static int speed = 1;
        static int waterLvl = 10;
        static int maxLvl;
        static int forse;

        static bool isWorking = false;
        static bool isDangerZone = false;

        private ObservableCollection<Pump> listPums = new() 
        {
            new(0,100),
            new(1,-20),
            new(2,-20),
            new(3,-20),
            new(4,-20),
            new(5,-20),
        };

        public int WaterLvl
        {
            get => waterLvl;
            set
            {
                if (waterLvl != value)
                {
                    waterLvl = Math.Min(maxLvl,value);
                    if (waterLvl < 0)
                        waterLvl = 1;
                    if (waterLvl >= maxLvl * 3/4)
                        DangerZone = true;
                    if (waterLvl <= maxLvl * 1/4)
                        DangerZone = false;
                    OnPropertyChanged(nameof(WaterLvl));
                }
            }
        }

        public int Forse
        {
            get => listPums[0].Forse;
            set
            {
                if (forse != value)
                {
                    listPums[0].Forse = value;
                    OnPropertyChanged(nameof(Forse));
                }
            }
        }

        public int Speed
        {
            get => speed;
            set
            {
                if(value != speed)
                {
                    speed = value;
                    foreach (var item in listPums)
                        item.Speed = speed;
                    OnPropertyChanged(nameof(Speed));
                }
            }
        }

        public ObservableCollection<Pump> Pumps
        {
            get => listPums;
            set
            {
                if (listPums != value)
                {
                    listPums = value;
                    OnPropertyChanged(nameof(Pumps));
                }
            }
        }

        public bool DangerZone
        {
            get => isDangerZone;
            set
            {
                if (isDangerZone != value)
                {
                    isDangerZone = value;
                    if (DangerZone)
                    {
                        foreach (var pump in listPums)
                        {
                            if (pump.IsPowered)
                                pump.StartThred();
                        }
                    }
                    else
                    {
                        foreach (var pump in listPums)
                        {
                            if(pump.IsPowered)
                                pump.StopThred();
                        }
                    }
                    OnPropertyChanged(nameof(DangerZone));
                }
            }
        }

        public bool IsWorking
        {
            get => isWorking;
            set
            {
                if (isWorking != value)
                {
                    isWorking = value;
                    OnPropertyChanged(nameof(IsWorking));
                }
            }
        }

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            maxLvl = (int)Bar.Maximum;
        }

        public void SetWater(object sender, int level)
        {
            WaterLvl = level;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!isWorking)
            {
                isWorking = true;
                foreach (var pump in listPums)
                {
                    pump.SetWater += SetWater;
                    if (pump.Forse >= 0 && pump.IsPowered)
                    {
                        pump.StartThred();
                    }
                }
            }
            else
            {
                foreach (var pump in listPums)
                {
                    pump.StopThred();
                    pump.SetWater -= SetWater;
                    isWorking = false;
                }
            }
        }

        private void PumpOnOff_Click(object sender, RoutedEventArgs e)
        {
            Pump t = (sender as Button).Tag as Pump;
            t.IsPowered = !t.IsPowered;
        }
    }
}
