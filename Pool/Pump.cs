using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Pool
{
    public class Pump
    {
        private int ID {  get; set; }
        private int forse;
        private int Force
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
        private bool isOn = true;
        private bool isTap;
        public bool IsOn
        {
            get => isOn;
            set => isOn = value;
        }

        public bool IsTap
        {
            get => isTap;
            set => isTap = value;
        }
        public Pump(int id, int force, bool isTap = false)
        {
            IsTap = isTap;
            ID = id;
            Force = force;
        }

        public int Pumping(int waterLvL)
        {
            return IsOn ? waterLvL + Force : waterLvL;
        }
    }
}
