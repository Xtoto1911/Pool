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

namespace Pool
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 


    public partial class MainWindow : Window
    {
        private BackgroundWorker worker = new();
        private Dictionary<string, Thread> dictTh;
        private object lockObject = new object();
        private double maxPoolLevel;
        private double currPoolLevel;
        private double jetValue;
        public MainWindow()
        {
            InitializeComponent();
            InitWorker();
            InitSettings();
            InitThreads();
        }

        private void InitWorker()
        {
            worker.WorkerReportsProgress = true;
            worker.WorkerSupportsCancellation = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void InitSettings()
        {
            maxPoolLevel = bar.Maximum;
            jetValue = jet.Width;
            currPoolLevel = 0;
        }

        private void InitThreads()
        {
            for (int i = 0; i < 5; i++)
            {
                dictTh[i.ToString()] = new Thread(DecreaseWorkerThread);
                dictTh[i.ToString()].Start();
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            for (; currPoolLevel <= maxPoolLevel; currPoolLevel += jetValue)
            {
                // Задержка для эмуляции длительной работы
                Thread.Sleep(60);

                // Отправка прогресса в основной поток
                worker.ReportProgress((int)currPoolLevel);
            }
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // Обновление прогресс-бара в основном потоке
            bar.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            worker.RunWorkerAsync();
        }


        private void DecreaseWorkerThread()
        {
            while (true)
            {
                Thread.Sleep(200); // Эмуляция работы

                lock (this)
                {
                    if (currPoolLevel < maxPoolLevel * 3/4)
                    {
                        currPoolLevel -= 2;
                        if (currPoolLevel < 0)
                        {
                            currPoolLevel = 0;
                        }
                    }
                }

                Dispatcher.Invoke(() => bar.Value = currPoolLevel); // Обновление UI в основном потоке
            }
        }

        private void sl_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            jetValue =  jet.Width = (double)e.NewValue;
        }

        private void pump1_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
