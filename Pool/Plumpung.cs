using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Pool
{
    public class Plumpung : INotifyPropertyChanged
    {
        public int Forse { get; set; }
        private bool isOn;//состояние работы
        private bool isPowered = true;//питание

        private static Mutex mutex = new();
        private Task taskPump;
        public static MainWindow Pool;

        public Plumpung(int forse)
        {
            Forse = forse;
            if(Forse < 0)
            {
                MainWindow.PoolDangerZoneOn += StartThred;
                MainWindow.PoolDangerZoneOf += StopThred;
            }
        }

        public bool IsPowered
        {
            get => isPowered;
            set
            {
                if (isPowered != value)
                {
                    isPowered = value;
                    if (!isPowered)
                        StopThred(Pool);
                    OnPropertyChanged(nameof(IsPowered));
                }
            }
        }

        public void StartThred(object sender)
        {
            if (!isPowered)
                return;
            isOn = true;
            taskPump =  Task.Run(() =>
            {
                while (isOn)
                {
                    mutex.WaitOne();
                    Pool.WaterLvl += Forse;
                    mutex.ReleaseMutex();
                    int delay = 1000 / (MainWindow.UPS * MainWindow.Speed);
                    Thread.Sleep(delay);
                }
            });
        }

        public async void StopThred(object sender)
        {
            isOn = false;
            if (taskPump != null)
            {
                await taskPump;
                taskPump.Dispose();
                taskPump = null;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
