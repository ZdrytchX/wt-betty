﻿using System;
using System.Windows;
using System.Windows.Threading;
using System.Globalization;
using System.Windows.Documents;
using System.IO;
using System.Windows.Data;

namespace wt_betty
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// NEW BETTY
    ///
    //todo: fuel warning (Done)
    //todo: eula, help,

    //TODO (ZdrytchX):
    // read/write json data per-aircraft type into sub folder
    // Rewrite code in python or lua or something for multi-OS friendliness

    public partial class MainWindow : Window
    {

        private indicator myIndicator = new indicator();
        private state myState = new state();
        string indicatorsurl = "http://localhost:8111/indicators";
        string statesurl = "http://localhost:8111/state";
        DispatcherTimer dispatcherTimer1 = new DispatcherTimer();
        DispatcherTimer dispatcherTimer2 = new DispatcherTimer();
        CultureInfo culture = new CultureInfo("en-US");
        FlowDocument myFlowDoc = new FlowDocument();
        Paragraph par = new Paragraph();

        public MainWindow()
        {
            InitializeComponent();
            cbx_g.IsChecked = User.Default.EnableG;
            slider_G.Value = Convert.ToDouble(User.Default.GForce);
            textBox_gSlider.Text = slider_G.Value.ToString();
            cbx_a.IsChecked = User.Default.EnableA;
            slider_A.Value = Convert.ToDouble(User.Default.AoA);
            textBox_aSlider.Text = slider_A.Value.ToString();
            cbx_overSpeed.IsChecked = User.Default.EnableOverSpeed;
            tbx_overSpeed.Text = User.Default.OverSpeed.ToString();
            cbx_pullup.IsChecked = User.Default.EnablePullUp;
            cbx_fuel.IsChecked = User.Default.EnableFuel;
            cbx_gear.IsChecked = User.Default.EnableGear;
            tbx_gearDown.Text = User.Default.GearDown.ToString();
            tbx_gearUp.Text = User.Default.GearUp.ToString();

            dispatcherTimer1.Tick += new EventHandler(dispatcherTimer1_Tick);
            dispatcherTimer1.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer2.Tick += new EventHandler(dispatcherTimer2_Tick);
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 5);
        }

        private void dispatcherTimer2_Tick(object sender, EventArgs e)
        {
            WTConnect();
        }

        public void WTConnect()
        {
            try
            {
                if (BaglantiVarmi("localhost", 8111))
                {
                    myState = JsonSerializer._download_serialized_json_data<state>(statesurl);
                    if (myState.valid == "true")
                    {
                        dispatcherTimer2.Stop();
                        dispatcherTimer1.Start();
                        tbx_msgs.Text = ("Running");
                    }
                    else if (myState.valid == "false")
                    {
                        dispatcherTimer2.Start();
                        dispatcherTimer1.Stop();
                        tbx_msgs.Text = "Waiting for a flight...";
                    }
                    button_start.IsEnabled = false;
                    button_stop.IsEnabled = true;

                }
                else
                {
                    //Dinlemeye geç
                    dispatcherTimer2.Start();
                    dispatcherTimer1.Stop();
                    tbx_msgs.Text = ("War Thunder is not running...");

                    button_start.IsEnabled = true;
                    button_stop.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {
                tbx_msgs.Text = ex.Message;
                dispatcherTimer1.Stop();
                dispatcherTimer2.Start();
                button_start.IsEnabled = true;
                button_stop.IsEnabled = false;

            }
        }

        private void dispatcherTimer1_Tick(object sender, EventArgs e)
        {
            getData();
        }

        private bool BaglantiVarmi(string adres, int port)
        {
            try
            {
                System.Net.Sockets.TcpClient baglanti = new System.Net.Sockets.TcpClient(adres, port);
                baglanti.Close();

                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// JSON saver
        /// </summary>
        ///
        /*
        private void saveToJSON()
        {
            //TODO
            try
            {
                myState = JsonSerializer._download_serialized_json_data<state>(statesurl);
                if ((myState.valid == "true") && (myIndicator.valid == "true") && (myIndicator.type != "dummy_plane") && (myIndicator.type != null))
                {
                    //using (StreamReader file = File.OpenText(myIndicator.type + ".json"));
                    
                    //string json = File.ReadAllText("myobjects.json");
                    //var playerList = JsonConvert.DeserializeObject<List<Player>>(json);
                    
                    string filePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\")) + @"Data\Country.json";
                    
                    string _countryJson = File.ReadAllText(filePath);
                    
                    //var _country = JsonConvert.DeserializeObject<List<Country>>(_countryJson);
                    

                    //save file thing here
                    //File.WriteAllText("myobjects.json", JsonConvert.SerializeObject(playerList));
                }
            }
            catch (Exception ex)
            {
                tbx_msgs.Text = ex.Message;
                dispatcherTimer1.Stop();
                dispatcherTimer2.Start();
            }
        }
    */

        private void getData()
        {
            try
            {
                myIndicator = JsonSerializer._download_serialized_json_data<indicator>(indicatorsurl);
                myState = JsonSerializer._download_serialized_json_data<state>(statesurl);

                if ((myState.valid == "true") && (myIndicator.valid == "true") && (myIndicator.type != "dummy_plane") && (myIndicator.type != null))
                {

                    decimal G = Convert.ToDecimal(myState.Ny, culture);
                    decimal AoA = Convert.ToDecimal(myState.AoA, culture);
                    decimal Vspeed = Convert.ToDecimal(myState.Vy, culture);
                    int Fuel = Convert.ToInt32(myState.Mfuel) * 1000;//MFuel and MFuel0 are given in integers
                    int FuelFull = Convert.ToInt32(myState.Mfuel0);
                    int Throttle = Convert.ToInt32(Convert.ToDecimal(myIndicator.throttle, culture) * 100);//TODO throttle variable only avialble in single engine aircraft
                    int gear = Convert.ToInt32(myState.gear);
                    int Alt = Convert.ToInt32(myState.H, culture);
                    int IAS = Convert.ToInt32(myState.IAS);
                    int flaps = Convert.ToInt32(myState.flaps);

                    Console.Write(myState.AoS);
                    //tbx_msgs.Text = myState.AoS;
                    decimal AoS = Convert.ToDecimal(myState.AoS);
                    int TAS = Convert.ToInt32(myState.TAS);
                    label.Content = myIndicator.type;

                    //BINGO FUEL
                    if (cbx_fuel.IsChecked == true &&  Fuel / FuelFull < 103 && Fuel / FuelFull > 100 && Throttle > 0)
                    {
                        System.Media.SoundPlayer myPlayer;
                        myPlayer = new System.Media.SoundPlayer(Properties.Resources.Bingo);
                        myPlayer.PlaySync();
                    }

                    //STALL WARNING
                    if (cbx_a.IsChecked == true)
                    {   //Stall Warning Mandatory pre-definitnions
                        System.Media.SoundPlayer stall1;
                        System.Media.SoundPlayer stall2;
                        stall1 = new System.Media.SoundPlayer(Properties.Resources.AngleOfAttackOverLimit);
                        stall2 = new System.Media.SoundPlayer(Properties.Resources.MaximumAngleOfAttack);

                        if (AoA > User.Default.AoA * 0.8 && AoA < User.Default.AoA + 10 && (myIndicator.gears_lamp == "1" || IAS > 100))
                        {
                            if (AoA < User.Default.AoA)
                            {
                                stall1.Stop();
                                stall2.PlayLooping();
                            }
                            else
                            {
                                stall2.Stop();
                                stall1.PlayLooping();
                            }//multi-layer AoA warnings as a variable-pitch isn't supported by MS's package
                        }
                        else
                        { stall1.Stop(); stall2.Stop(); }
                    }
                    
                    //G OVERLOAD
                    if (cbx_g.IsChecked == true)
                    {
                        System.Media.SoundPlayer G1;
                        System.Media.SoundPlayer G2;
                        G1 = new System.Media.SoundPlayer(Properties.Resources.OverG);
                        G2 = new System.Media.SoundPlayer(Properties.Resources.GOverLimit);
                        if (G > User.Default.GForce || (double)G < -0.4 * User.Default.GForce)
                        {
                            if (G > User.Default.GForce + 3 - User.Default.GForce / (decimal)3)
                            {
                                G1.Stop();
                                G2.PlaySync();
                            }
                            else
                            {
                                G2.Stop();
                                G1.PlaySync();
                            }
                        }
                    }
                    
                    //PULL UP Ground/sea level Proximity Warning
                    //desirable to have about 3 seconds before crash
                    if (cbx_pullup.IsChecked == true && 0 - Vspeed * (2 + (decimal)Math.Pow(IAS / 100, 0.7)) > Alt)
                    {
                        System.Media.SoundPlayer myPlayer;
                        myPlayer = new System.Media.SoundPlayer(Properties.Resources.PullUp);
                        myPlayer.PlaySync();
                    }

                    //Overspeed
                    if (cbx_overSpeed.IsChecked == true && IAS > User.Default.OverSpeed)
                    {
                        System.Media.SoundPlayer myPlayer;
                        myPlayer = new System.Media.SoundPlayer(Properties.Resources.OverSpeed);
                        myPlayer.PlaySync();
                    }

                    //GEAR UP/DOWN
                    if (User.Default.EnableGear == true)
                    {
                        if (gear == 100 && IAS > User.Default.GearUp && myIndicator.gears_lamp == "0")
                        {
                            System.Media.SoundPlayer myPlayer;
                            myPlayer = new System.Media.SoundPlayer(Properties.Resources.GearUp);
                            myPlayer.PlaySync();
                        }

                        if ((AoA < 20 || Vspeed > -10) &&
                            IAS < User.Default.GearDown && IAS > 40/*Alt < 500 && flaps > 20*/)
                        {
                            float Deg2Rad = (float)(Math.PI / 180f);
                            float driftSpeed = (float)(TAS * Math.Sin(Deg2Rad * (float)AoS));

                            if (gear == 0 && myIndicator.gears_lamp != "0" && Throttle < 20)
                            {
                                System.Media.SoundPlayer myPlayer;
                                myPlayer = new System.Media.SoundPlayer(Properties.Resources.GearDown);
                                myPlayer.PlaySync();
                            }
                            //Sink rate warning: WT has a global vertical gear speed limit of 10m/s
                            else if (Vspeed < -8 && Throttle < 60)
                            {
                                System.Media.SoundPlayer myPlayer;
                                myPlayer = new System.Media.SoundPlayer(Properties.Resources.SinkRate);
                                myPlayer.PlaySync();
                            }
                            //drift april fools
                            else if (driftSpeed < -50)
                            {
                                tbx_msgs.Text = "ドリフト! ";
                                System.Media.SoundPlayer myPlayer;
                                myPlayer = new System.Media.SoundPlayer(Properties.Resources.Running90s);
                                myPlayer.PlaySync();
                            }
                            else if (driftSpeed > 50)
                            {
                                tbx_msgs.Text = "ドリフト! ";
                                System.Media.SoundPlayer myPlayer;
                                myPlayer = new System.Media.SoundPlayer(Properties.Resources.ManuelGGG);
                                myPlayer.PlaySync();
                            }
                        }
                    }
                }
                else
                {
                    dispatcherTimer1.Stop();
                    dispatcherTimer2.Start();
                }
            }
            catch (Exception ex)
            {
                tbx_msgs.Text = ex.Message;
                dispatcherTimer1.Stop();
                dispatcherTimer2.Start();
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dispatcherTimer1.Stop();
            dispatcherTimer2.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WTConnect();
        }

        private void button_start_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer2.Start();
            if (dispatcherTimer2.IsEnabled)
            {
                button_start.IsEnabled = false;
                button_stop.IsEnabled = true;
            }
        }

        //TODO: User-assigned key binding for toggle of the program
        private void button_stop_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer1.Stop();
            dispatcherTimer2.Stop();
            button_start.IsEnabled = true;
            button_stop.IsEnabled = false;
            System.Media.SoundPlayer myPlayer1;
            System.Media.SoundPlayer myPlayer2;
            myPlayer1 = new System.Media.SoundPlayer(Properties.Resources.AngleOfAttackOverLimit);
            myPlayer2 = new System.Media.SoundPlayer(Properties.Resources.MaximumAngleOfAttack);
            myPlayer1.Stop();myPlayer2.Stop();
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User.Default.EnableG = cbx_g.IsChecked.Value;
                User.Default.GForce = Convert.ToInt32(slider_G.Value);
                User.Default.EnableA = cbx_a.IsChecked.Value;
                User.Default.AoA = Convert.ToInt32(slider_A.Value);
                User.Default.EnablePullUp = cbx_pullup.IsChecked.Value;
                User.Default.EnableOverSpeed = cbx_overSpeed.IsChecked.Value;
                User.Default.OverSpeed = Convert.ToInt32(tbx_overSpeed.Text);
                User.Default.EnableFuel = cbx_fuel.IsChecked.Value;
                User.Default.EnableGear = cbx_gear.IsChecked.Value;
                User.Default.GearDown = Convert.ToInt32(tbx_gearDown.Text);
                User.Default.GearUp = Convert.ToInt32(tbx_gearUp.Text);
                User.Default.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_reset_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User.Default.EnableG = true;
                User.Default.GForce = 6;
                User.Default.EnableA = true;
                User.Default.AoA = 12;
                User.Default.EnablePullUp = true;
                User.Default.EnableFuel = true;
                User.Default.EnableGear = true;
                User.Default.EnableOverSpeed = true;
                User.Default.OverSpeed = 820;
                User.Default.GearDown = 270;
                User.Default.GearUp = 290;
                User.Default.Save();

                cbx_g.IsChecked = User.Default.EnableG;
                slider_G.Value = Convert.ToDouble(User.Default.GForce);
                textBox_gSlider.Text = slider_G.Value.ToString();
                cbx_a.IsChecked = User.Default.EnableA;
                slider_A.Value = Convert.ToDouble(User.Default.AoA);
                tbx_overSpeed.Text = User.Default.OverSpeed.ToString();
                cbx_overSpeed.IsChecked = User.Default.EnableOverSpeed;
                textBox_aSlider.Text = slider_A.Value.ToString();
                cbx_pullup.IsChecked = User.Default.EnablePullUp;
                cbx_fuel.IsChecked = User.Default.EnableFuel;
                cbx_gear.IsChecked = User.Default.EnableGear;
                tbx_gearDown.Text = User.Default.GearDown.ToString();
                tbx_gearUp.Text = User.Default.GearUp.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void button_help_Click(object sender, RoutedEventArgs e)
        {
            var helpFile = Path.Combine(Path.GetTempPath(), "wt-betty-help.txt");
            File.WriteAllText(helpFile, Properties.Resources.wt_betty_help);
            System.Diagnostics.Process.Start(helpFile);
        }
    }

    public class kphTomphConversion : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int kph = System.Convert.ToInt32(value.ToString());
                double mph = kph / 1.609;
                double kts = kph / 1.852;
                String rounded_result = "(" + String.Format("{0:0}", Math.Truncate(mph * 10) / 10) + "mph / " + String.Format("{0:0}", Math.Truncate(kts * 10) / 10) +"kts)";
                return rounded_result;
            }
            catch (Exception/* ex*/)
            {
                //MessageBox.Show(ex.Message); //Annoying to have in a practical sense, keep it out of the builds
                return "ERROR";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}
