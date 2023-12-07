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
        private static byte speed = 1;
        private static byte ups = 60;
        private static int waterLvl = 10 * ups;
        private static int maxLvl = 2000 * ups;
        private static int forse = 80;

        private static bool isWorking;
        private bool isDangerZone;

        public delegate void PoolEvens(object sender);
        public static event PoolEvens? PoolDangerZoneOn;//событие, когда мы дошли до черты(3/4 объема)
        public static event PoolEvens? PoolDangerZoneOf;//событие, когда мы дошли до черты(1/4 объема)

        private ObservableCollection<Plumpung> listPlumpung = new()
        {
            new(forse),
            new(-20),
            new(-20),
            new(-20),
            new(-20),
            new(-20),
        };

        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            Plumpung.Pool = this;
        }

        public static byte UPS => ups;
        public static byte Speed => speed;

        public int UIWaterLvl
        {
            get => waterLvl / ups;
            set { }
        }

        public int UIMaxLvl
        {
            get => maxLvl / ups;
            set { }
        }

        public int UIForse
        {
            get => forse;
            set
            {
                if (forse != value)
                {
                    forse = value;
                    foreach (var pump in listPlumpung)
                        if (pump.Forse >= 0)
                            pump.Forse = value;
                    OnPropertyChanged(nameof(UIForse));
                }
            }
        }

        public byte UISpeed//ускорение
        {
            get => speed;
            set
            {
                if(speed != value)
                {
                    speed = value;
                    OnPropertyChanged(nameof(UISpeed));
                }
            }
        }

        public int WaterLvl
        {
            get => waterLvl;
            set
            {
                if(waterLvl != value)
                {
                    waterLvl = Math.Min(value, maxLvl);
                    OnPropertyChanged(nameof(UIWaterLvl));
                    if(!DangerZone && UIWaterLvl >= UIMaxLvl * 3/4)
                    {
                        PoolDangerZoneOn?.Invoke(this);
                        DangerZone = true;
                    }
                    else if (DangerZone && UIWaterLvl <= UIMaxLvl * 1 / 4)
                    {
                        PoolDangerZoneOf?.Invoke(this);
                        DangerZone = false;
                    }
                }
            }
        }

        public bool DangerZone//флаг перехода за границу
        {
            get => isDangerZone;
            set
            {
                if (isDangerZone != value)
                {
                    isDangerZone = value;
                    OnPropertyChanged(nameof(DangerZone));
                }
            }
        }

        public ObservableCollection<Plumpung> ListPlumpung
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

        private void SystemOnOf_Click(object sender, RoutedEventArgs e)
        {
            if (!IsWorking) {
                foreach (var pump in listPlumpung)
                    if (pump.Forse >= 0)
                        pump.StartThred(this);
            }
            else
            {
                foreach (var pump in listPlumpung)
                    pump.StopThred(this);
                DangerZone = false;
            }
            IsWorking = !IsWorking;
        }

        private void PumpOnOff_Click(object sender, RoutedEventArgs e)
        {
            Plumpung pump = (sender as Button).Tag as Plumpung;
            pump.IsPowered = !pump.IsPowered;
            if(IsWorking && pump.IsPowered)
                if (pump.Forse < 0 && DangerZone || pump.Forse >= 0)
                    pump.StartThred(this);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            foreach(var pumps in listPlumpung)
                pumps.StopThred(this);
        }
    }
}
