using System;
using System.Collections.Generic;
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
        private byte ups = 10;
        private int speed = 1;
        private int waterLvl = 0;

        private bool isWorking = false;
        private bool isDangerZone = false;

        private List<Pump> listPums = new() 
        {
            new(0,0, true),
            new(1,20),
            new(2,20),
            new(3,20),
            new(4,20),
            new(5,20),
        };

        private List<Task> listTasks = new();

        private readonly object lockObj = new object();

        public int WaterLvl
        {
            get => waterLvl;
            set
            {
                if (value != waterLvl)
                {
                    waterLvl = value;
                    OnPropertyChanged(nameof(WaterLvl));
                }
            }
        }


        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
        }



        private void pump1_Click(object sender, RoutedEventArgs e)
        {
            int currID = int.Parse(((Button)sender).Content.ToString());
            listPums[currID].IsOn = !listPums[currID].IsOn;
        }

        private void StartStop_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StopProgram()
        {
            isWorking = false;
            isDangerZone = false;
            foreach (var task in listTasks)
            {
                task.Wait();
                task.Dispose();
            }
            listTasks.Clear();
        }

        private void StartProgram()
        {
            isWorking = true;   
            foreach (var pump in listPums)
            {
                listTasks.Add(Task.Run(() =>
                {
                    while (isWorking)
                    {
                        lock (lockObj)
                        {
                            if (pump.IsTap)
                                UpWater(pump);
                            else
                                DownWater(pump);
                        }
                    }
                    Thread.Sleep(1000 / (ups * speed));
                }));
            }
        }

        private void UpWater(Pump tap)
        {
            if(WaterLvl < bar.Maximum)
                WaterLvl = tap.Pumping(waterLvl * ups) / ups;
            if (waterLvl >= bar.Maximum * 3 / 4)
                isDangerZone = true;
        }

        private void DownWater(Pump pump)
        {
            if (isDangerZone)
                WaterLvl = pump.Pumping(waterLvl * ups) / ups;
            if (waterLvl <= bar.Maximum * 1 / 4)
                isDangerZone = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
