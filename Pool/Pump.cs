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
    public class Pump : INotifyPropertyChanged
    {

        public int ID { get; set; }
        private int ups = 10;
        private int forse;
        private int speed = 1;
        private static int currentLvl = 1;
        private bool isOn = true;
        private bool isPowered = true;  

        public static Mutex mutex = new();
        public Task taskPump;


        public delegate void PumpEvent(object sender, int level);
        public event PumpEvent SetWater;


        public int Forse 
        {
            get => forse;
            set
            {
                if (forse != value)
                {
                    forse = value;
                }
            }
        }
        

        public int Speed
        {
            get => speed;
            set
            {
                if (speed != value)
                {
                    speed = value;
                }
            }
        }

        public bool IsOn
        {
            get => isOn;
            set
            {
                if (isOn != value)
                {
                    isOn = value;
                    OnPropertyChanged(nameof(IsOn));
                }
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
                        StopThred();
                    else if(isPowered && SetWater != null)
                        StartThred();
                    OnPropertyChanged(nameof(IsPowered));
                }
            }
        }

        public Pump(int ID, int forse)
        {
            this.ID = ID;
            Forse = forse;
            IsOn = true;
        }

        public async void StartThred()
        {
            if (!IsPowered || taskPump != null)
                return;

            IsOn = true;
            taskPump = Task.Run(async () =>
            {
                while (IsOn)
                {
                    mutex.WaitOne();
                    currentLvl = NewLvl(currentLvl * ups) / ups;
                    SetWater(this,currentLvl);
                    mutex.ReleaseMutex();
                    await Task.Delay(1000/(ups * Speed));
                }
            });
        }

        private  int NewLvl(int lvl)
        {
           return lvl + Forse;
        }
        public void StopThred()
        {
            IsOn = false;
            if (taskPump != null)
            {
                taskPump.Wait();
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
