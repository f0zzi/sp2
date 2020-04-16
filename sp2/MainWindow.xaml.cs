using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Diagnostics;

namespace sp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    public partial class MainWindow : Window
    {
        delegate void Operation(Config config);
        Config config = null;
        ObservableCollection<int> list_fibo = new ObservableCollection<int>();
        ObservableCollection<int> list_fact = new ObservableCollection<int>();
        ObservableCollection<int> list_simple = new ObservableCollection<int>();
        Thread fibo = null;
        Thread fact = null;
        Thread simple = null;
        public MainWindow()
        {
            InitializeComponent();
            lbFibo.ItemsSource = list_fibo;
            lbFact.ItemsSource = list_fact;
            lbSimple.ItemsSource = list_simple;
        }
        void Fibo(int number, int delay)
        {
            lbFibo.Dispatcher.Invoke(() => list_fibo.Clear());

            int last = 1, curr = 1, next = 0;
            for (int i = 0; i < number; i++)
            {
                lbFibo.Dispatcher.Invoke(() => {
                    list_fibo.Add(last);
                    lbFibo.ScrollIntoView(last);
                });
                try
                {
                    next = checked(last + curr);
                }
                catch (Exception) { break; }
                last = curr;
                curr = next;
                Thread.Sleep(delay);
            }
            btfibo.Dispatcher.Invoke(() =>
            {
                btfibo.Content = "Start";
                btfibo.Background = Brushes.LightGreen;
            });
        }
        void Fact(int number, int delay)
        {
            lbFact.Dispatcher.Invoke(() => list_fact.Clear());
            int tmp = 1;
            for (int i = 1; i <= number; i++)
            {
                try
                {
                    tmp = checked(tmp * i);
                }
                catch (Exception) { break; }
                lbFact.Dispatcher.Invoke(() => {
                    list_fact.Add(tmp);
                    lbFact.ScrollIntoView(tmp);
                });
                Thread.Sleep(delay);
            }
            btfact.Dispatcher.Invoke(() =>
            {
                btfact.Content = "Start";
                btfact.Background = Brushes.LightGreen;
            });
        }
        void Simple(int number, int delay)
        {
            lbSimple.Dispatcher.Invoke(() => list_simple.Clear());
            for (int i = 1; lbSimple.Items.Count < number; i++)
            {
                if (i < 4 || Simple_helper(i))
                {
                    lbFact.Dispatcher.Invoke(() => {
                        list_simple.Add(i);
                        lbSimple.ScrollIntoView(i);
                    });
                    Thread.Sleep(delay);
                }
            }
            btsimple.Dispatcher.Invoke(() =>
            {
                btsimple.Content = "Start";
                btsimple.Background = Brushes.LightGreen;
            });
        }
        bool Simple_helper(int number)
        {
            for (int i = 2; i < number; i++)
            {
                if (number % i == 0)
                    return false;
            }
            return true;
        }
        void Calc(object obj)
        {
            Config config = (obj as Config);
            switch (config.Name)
            {
                case "fibo":
                    Fibo(config.Number, config.Delay);
                    break;
                case "fact":
                    Fact(config.Number, config.Delay);
                    break;
                case "simple":
                    Simple(config.Number, config.Delay);
                    break;
                default:
                    break;
            }
        }
        private void Start(object sender, RoutedEventArgs e)
        {
            Operation operation = null;
            Button but = (sender as Button);
            switch (but.Name)
            {
                case "btfibo":
                    if (fibo == null || !fibo.IsAlive)
                    {
                        fibo = new Thread(new ParameterizedThreadStart(Calc));
                        config = new Config
                        {
                            Number = (String.IsNullOrWhiteSpace(tbFiboDepth.Text) ? int.MaxValue : Convert.ToInt32(tbFiboDepth.Text)),
                            Delay = (String.IsNullOrWhiteSpace(tbFiboDelay.Text) ? 50 : Convert.ToInt32(tbFiboDelay.Text)),
                            Name = "fibo"
                        };
                        fibo.IsBackground = true;
                        operation = fibo.Start;
                    }
                    else
                    {
                        if (fibo.ThreadState.HasFlag(System.Threading.ThreadState.Suspended))
                           Pause(pfibo, null);
                        fibo.Abort();
                        btfibo.Content = "Start";
                        btfibo.Background = Brushes.LightGreen;
                    }
                    break;
                case "btfact":
                    if (fact == null || !fact.IsAlive)
                    {
                        fact = new Thread(new ParameterizedThreadStart(Calc));
                        config = new Config
                        {
                            Number = (String.IsNullOrWhiteSpace(tbFactDepth.Text) ? int.MaxValue : Convert.ToInt32(tbFactDepth.Text)),
                            Delay = (String.IsNullOrWhiteSpace(tbFactDelay.Text) ? 50 : Convert.ToInt32(tbFactDelay.Text)),
                            Name = "fact"
                        };
                        fact.IsBackground = true;
                        operation = fact.Start;
                    }
                    else
                    {
                        if (fact.ThreadState.HasFlag(System.Threading.ThreadState.Suspended))
                            Pause(pfact, null);
                        fact.Abort();
                        btfact.Content = "Start";
                        btfact.Background = Brushes.LightGreen;
                    }
                    break;
                case "btsimple":
                    if (simple == null || !simple.IsAlive)
                    {
                        simple = new Thread(new ParameterizedThreadStart(Calc));
                        config = new Config
                        {
                            Number = (String.IsNullOrWhiteSpace(tbSimpleDepth.Text) ? int.MaxValue : Convert.ToInt32(tbSimpleDepth.Text)),
                            Delay = (String.IsNullOrWhiteSpace(tbSimpleDelay.Text) ? 50 : Convert.ToInt32(tbSimpleDelay.Text)),
                            Name = "simple"
                        };
                        simple.IsBackground = true;
                        operation = simple.Start;
                    }
                    else
                    {
                        if (simple.ThreadState.HasFlag(System.Threading.ThreadState.Suspended))
                            Pause(psimple, null);
                        simple.Abort();
                        btsimple.Content = "Start";
                        btsimple.Background = Brushes.LightGreen;
                    }
                    break;
                default:
                    break;
            }
            if (operation != null)
            {
                operation(config);
                but.Content = "Abort";
                but.Background = Brushes.IndianRed;
            }
        }

        private void Pause(object sender, RoutedEventArgs e)
        {
            Button but = (sender as Button);
            switch (but.Name)
            {
                case "pfibo":
                    if (fibo != null && fibo.IsAlive)
                    {
                        if (fibo.ThreadState.HasFlag(System.Threading.ThreadState.Suspended))
                        {
                            fibo.Resume();
                            pfibo.Background = Brushes.LightGray;
                            pfibo.Content = "Pause";
                        }
                        else
                        {
                            fibo.Suspend();
                            pfibo.Background = Brushes.LightYellow;
                            pfibo.Content = "Resume";
                        }
                    }
                    break;
                case "pfact":
                    if (fact != null && fact.IsAlive)
                    {
                        if (fact.ThreadState.HasFlag(System.Threading.ThreadState.Suspended))
                        {
                            fact.Resume();
                            pfact.Background = Brushes.LightGray;
                            pfact.Content = "Pause";
                        }
                        else
                        {
                            fact.Suspend();
                            pfact.Background = Brushes.LightYellow;
                            pfact.Content = "Resume";
                        }
                    }
                    break;
                case "psimple":
                    if (simple != null && simple.IsAlive)
                    {
                        if (simple.ThreadState.HasFlag(System.Threading.ThreadState.Suspended))
                        {
                            simple.Resume();
                            psimple.Background = Brushes.LightGray;
                            psimple.Content = "Pause";
                        }
                        else
                        {
                            simple.Suspend();
                            psimple.Background = Brushes.LightYellow;
                            psimple.Content = "Resume";
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}

