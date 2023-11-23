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
        static byte ups = 10;
        static int speed = 1;
        static int waterLvl = 10;
        static int maxLvl;

        static bool isWorking = false;
        static bool isDangerZone = false;

        private ObservableCollection<Pump> listPums = new() 
        {
            new(0,100, true),
            new(1,20),
            new(2,20),
            new(3,20),
            new(4,20),
            new(5,20),
        };

        static Mutex mutex = new Mutex();

        public int WaterLvl
        {
            get => waterLvl;
            set
            {
                if (waterLvl != value)
                {
                    waterLvl = value;
                    if(waterLvl >= maxLvl * 3/4)
                        DangerZone = true;
                    else if(waterLvl <= maxLvl * 1/4)
                        DangerZone = false;
                    OnPropertyChanged(nameof(WaterLvl));
                }
            }
        }

        public int Forse
        {
            get
            {
                foreach (var pump in listPums)
                    if (pump.IsTap)
                        return pump.Force;
                return 1;
            }
            set
            {
                foreach (var pump in listPums)
                    if (pump.IsTap && value != pump.Force)
                        pump.Force = value;
                OnPropertyChanged(nameof(Forse));
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

        private void StopProgram()
        {
            
        }

        private void StartProgram()
        {
            
        }

        void UpWater(Pump tap)
        {
            
        }

        void DownWater(Pump pump)
        {
            
        }

        private void Work(Pump pump)
        {
            
        }

        private async void pump1_Click(object sender, RoutedEventArgs e)
        {
            int num = int.Parse(((Button)sender).Tag.ToString());
            
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {
            
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
