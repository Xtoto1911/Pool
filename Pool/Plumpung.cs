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

        public int ID { get; set; }
        private int ups = 10;
        private int forse;
        private int speed = 1;
        private bool isOn = true;
        private bool isPowered = true;
        private bool isDangerZone = false;

        public static Mutex mutex = new();
        public Task taskPump;

        public  delegate void PumpEvent(int forse, int ups);
        public  event PumpEvent SetWater;
        
        public int Forse 
        {
            get => forse;
            set => forse = value;
        }
        
        public int Speed
        {
            get => speed;
            set => speed = value;
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
                    else if(isPowered && (Forse > 0 || Forse < 0 && DangerZone))
                        StartThred();
                    OnPropertyChanged(nameof(IsPowered));
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
                    if (IsPowered && DangerZone && Forse < 0)
                        StartThred();
                    else if (IsPowered && !DangerZone && Forse < 0)
                        StopThred();
                }
            }
        }

        public Plumpung(int ID, int forse)
        {
            this.ID = ID;
            Forse = forse;
            IsOn = true;
        }

        public void StartThred()
        {
            if (!IsPowered || taskPump != null)
                return;

            IsOn = true;
            taskPump = new Task( async() =>
            {
                while (IsOn)
                {
                    mutex.WaitOne();
                    SetWater(Forse,ups);
                    mutex.ReleaseMutex();
                    await Task.Delay(1000/(ups * Speed));
                }
            });
            taskPump.Start();
        }

        public async void StopThred()
        {
            IsOn = false;
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
