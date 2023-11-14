using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pool
{
    public class Pump : INotifyPropertyChanged
    {
        private int forse;
        private bool isOn = true;
        private bool isTap;
        public int Num {  get; set; }

        public int Force
        {
            get => forse;
            set
            {
                if ((IsTap && value < 0) || (!IsTap && value > 0))
                {
                    forse = -value;
                    return;
                }
                forse = value;

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

        public bool IsTap
        {
            get => isTap;
            set => isTap = value;
        }

        public Pump(int num, int force, bool isTap = false)
        {
            IsTap = isTap;
            Force = force;
            Num = num;
        }

        public int NewWaterLvL(int waterLvL)
        {
            return IsOn ? waterLvL + Force : waterLvL;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
