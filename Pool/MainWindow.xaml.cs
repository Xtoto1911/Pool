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
        private static int speed = 1;
        private static int waterLvl = 10;
        private static int maxLvl;
        private static int forse;

        private static bool isWorking = false;
        private static bool isDangerZone = false;

        private ObservableCollection<Plumpung> listPlumpung = new() 
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
            get => listPlumpung[0].Forse;
            set
            {
                if (forse != value)
                {
                    listPlumpung[0].Forse = value;
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
                    foreach (var item in listPlumpung)
                        item.Speed = speed;
                    OnPropertyChanged(nameof(Speed));
                }
            }
        }

        public ObservableCollection<Plumpung> Plumpung
        {
            get => listPlumpung;
            set
            {
                if (listPlumpung != value)
                {
                    listPlumpung = value;
                    OnPropertyChanged(nameof(Padding));
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
                        foreach (var pump in listPlumpung)
                        {
                            if (pump.IsPowered && pump.Forse < 0)
                                pump.StartThred();
                        }
                    }
                    else
                    {
                        foreach (var pump in listPlumpung)
                        {
                            if(pump.IsPowered && pump.Forse < 0)
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

        public void SetWater(int forse, int ups)
        {
            WaterLvl = (WaterLvl * ups + forse)/ups;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!IsWorking)
            {
                IsWorking = true;
                foreach (var pump in listPlumpung)
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
                IsWorking = false;
                DangerZone = false;
                foreach (var pump in listPlumpung)
                {
                    pump.StopThred();
                    pump.SetWater -= SetWater;
                }
            }
        }

        private void PumpOnOff_Click(object sender, RoutedEventArgs e)
        {
            Plumpung t = (sender as Button).Tag as Plumpung;
            t.IsPowered = !t.IsPowered;
        }
    }
}
