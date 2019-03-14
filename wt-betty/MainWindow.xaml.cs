using System;
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
    //todo: fuel warning
    //todo: eula, help,

    public partial class MainWindow : Window
    {

        private indicator myIndicator = new indicator();
        private state myState = new state();
        string indicatorsurl = "http://localhost:8111/indicators";
        string statesurl = "http://localhost:8111/state";
        DispatcherTimer dispatcherTimer_getData = new DispatcherTimer();
        DispatcherTimer dispatcherTimer_connect = new DispatcherTimer();
        CultureInfo culture = new CultureInfo("en-US");
        FlowDocument myFlowDoc = new FlowDocument();
        Paragraph par = new Paragraph();

        public int nextBingoFuelWarning = -1;
        public int bingoFuelWarning_count = 0;

        public MainWindow()
        {
            InitializeComponent();
            cbx_g.IsChecked = User.Default.EnableG;
            slider_G.Value = Convert.ToDouble(User.Default.GForce);
            cbx_a.IsChecked = User.Default.EnableA;
            slider_A.Value = Convert.ToDouble(User.Default.AoA);
            cbx_pullUp.IsChecked = User.Default.EnablePullUp;
            comboBox_planeType.SelectedIndex = User.Default.PlaneType;
            textBox_groundLevel_m.Text = User.Default.GroundLevel.ToString();
            cbx_groundLevelAutoDetect.IsChecked = User.Default.EnableGroundLevelAutoDetect;
            cbx_altitude.IsChecked = User.Default.EnableAltitude;
            textBox_altitudeWarning_m.Text = User.Default.AltitudeWarning.ToString();
            cbx_bingoFuel.IsChecked = User.Default.EnableBingoFuel;
            textBox_bingoFuelWarningLength.Text = User.Default.BingoFuelWarningLength.ToString();
            slider_bingoFuel.Value = Convert.ToDouble(User.Default.BingoFuelPercentage);
            textBox_bingoFuelPercentage.Text = slider_bingoFuel.Value.ToString();
            cbx_bingoFuelRepeat.IsChecked = User.Default.EnableBingoFuelRepeat;
            cbx_gear.IsChecked = User.Default.EnableGear;
            textBox_gearUp.Text = User.Default.GearUp.ToString();
            textBox_gearDown.Text = User.Default.GearDown.ToString();

            dispatcherTimer_getData.Tick += new EventHandler(DispatcherTimer_getData_Tick);
            dispatcherTimer_getData.Interval = new TimeSpan(0, 0, 0, 0, 200);
            dispatcherTimer_connect.Tick += new EventHandler(DispatcherTimer_connect_Tick);
            dispatcherTimer_connect.Interval = new TimeSpan(0, 0, 5);
        }

        private void DispatcherTimer_connect_Tick(object sender, EventArgs e)
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
                        dispatcherTimer_connect.Stop();
                        dispatcherTimer_getData.Start();
                        tbx_msgs.Text = ("Running");
                    }
                    else if (myState.valid == "false")
                    {
                        dispatcherTimer_connect.Start();
                        dispatcherTimer_getData.Stop();
                        tbx_msgs.Text = "Waiting for a flight...";

                    }
                    button_start.IsEnabled = false;
                    button_stop.IsEnabled = true;
                }
                else
                {
                    //Dinlemeye geç
                    dispatcherTimer_connect.Start();
                    dispatcherTimer_getData.Stop();
                    tbx_msgs.Text = ("War Thunder is not running...");

                    button_start.IsEnabled = true;
                    button_stop.IsEnabled = false;
                }
            }
            catch (Exception ex)
            {

                tbx_msgs.Text = ex.Message;
                dispatcherTimer_getData.Stop();
                dispatcherTimer_connect.Start();
                button_start.IsEnabled = true;
                button_stop.IsEnabled = false;
            }
        }

        private void DispatcherTimer_getData_Tick(object sender, EventArgs e)
        {
            GetData();
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

        private void GetData()
        {
            try
            {
                myIndicator = JsonSerializer._download_serialized_json_data<indicator>(indicatorsurl);
                myState = JsonSerializer._download_serialized_json_data<state>(statesurl);

                switch (myIndicator.type)
                {
                    case null:
                        label.Content = "Cockpit Warning Sounds";
                        DEBUG_textBlock_state.Text = $"myState: {myState.valid} | myIndicator: {myIndicator.valid} | type: null | NOT UPDATING";
                        break;
                    case "dummy_plane":
                        label.Content = "Cockpit Warning Sounds";
                        DEBUG_textBlock_state.Text = $"myState: {myState.valid} | myIndicator: {myIndicator.valid} | type: dummy | NOT UPDATING";
                        break;
                    default:
                        DEBUG_textBlock_state.Text = $"myState: {myState.valid} | myIndicator: {myIndicator.valid} | type: {myIndicator.type}";
                        break;
                }

                if ((myState.valid == "true") && (myIndicator.valid == "true") && (myIndicator.type != "dummy_plane") && (myIndicator.type != null))
                {
                    decimal M = Convert.ToDecimal(myState.M, culture);  // Mach Number
                    decimal G = Convert.ToDecimal(myState.Ny, culture);
                    decimal AoA = Convert.ToDecimal(myState.AoA, culture);
                    decimal Alt;
                    if (Convert.ToInt32(myState.H) >= 1)
                    {
                        double Alt_unknownUnit;
                        if (myIndicator.altitude_10k != null)
                        {
                            DEBUG_textBlock_alt_source.Text = "altitude_10k";
                            Alt_unknownUnit = Convert.ToDouble(myIndicator.altitude_10k, culture);
                        }
                        else
                        {
                            DEBUG_textBlock_alt_source.Text = "altitude_hour";
                            Alt_unknownUnit = Convert.ToDouble(myIndicator.altitude_hour, culture);
                        }
                        DEBUG_textBlock_alt_unit.Text = $"alt is imperial";
                        if (Convert.ToInt32(myState.H) * 3 > Alt_unknownUnit)
                        {
                            // convert alt from meteric to imperial
                            DEBUG_textBlock_alt_unit.Text = $"alt is meteric";
                            Alt_unknownUnit = Alt_unknownUnit * 3.2808;
                        }
                        Alt = Convert.ToDecimal(Alt_unknownUnit);
                        DEBUG_textBlock_alt.Text = $"alt: {Alt}ft";
                    }
                    else
                    {
                        Alt = Convert.ToDecimal(myIndicator.altitude_hour, culture);
                    }
                    decimal Vspeed = Convert.ToDecimal(myState.Vy, culture);
                    double sinkRate = Convert.ToDouble(myState.Vy, culture);
                    int Fuel = Convert.ToInt32(myState.Mfuel);//MFuel and MFuel0 are given in integers
                    int FuelFull = Convert.ToInt32(myState.Mfuel0);
                    int Throttle = Convert.ToInt32(Convert.ToDecimal(myIndicator.throttle, culture) * 100);//TODO throttle variable only avialble in single engine aircraft
                    int gear = Convert.ToInt32(myState.gear);   // 0 = gear out, 1 = gear in, 2 retracting/deploying gear
                    int IAS = Convert.ToInt32(myState.IAS);//unreliable?
                    int flaps = Convert.ToInt32(myState.flaps);
                    label.Content = myIndicator.type;

                    int DEUBG_mAGL = Convert.ToInt32(Math.Round(Convert.ToInt32(Alt) / 3.2808, 0)) - Convert.ToInt32(textBox_groundLevel_m.Text);
                    DEBUG_textBlock_mAGL.Text = $"H: {myState.H} alt_hr:{myIndicator.altitude_hour} alt_10k:{myIndicator.altitude_10k} mAGL: {DEUBG_mAGL}";

                    //PULL UP Ground Proximity Warning
                    if (cbx_pullUp.IsChecked == true)
                    {
                        DEBUG_textBlock_sinkRate.Text = $"sinkRate: {sinkRate}";
                        if (sinkRate <= -7.62 && IsPositiveInteger(textBox_groundLevel_m.Text))
                        {
                            int meterAboveGroundLevel = Convert.ToInt32(Math.Round(Convert.ToInt32(Alt) / 3.2808, 0)) - Convert.ToInt32(textBox_groundLevel_m.Text);
                            if (comboBox_planeType.SelectedIndex == 0)
                            {
                                // Profile for prop/turboprop
                                if (meterAboveGroundLevel <= 596.8)
                                {
                                    if (meterAboveGroundLevel <= 105.4)
                                    {
                                        if (meterAboveGroundLevel < (105 / 1.35) * ((-sinkRate) - 7.62))
                                        {
                                            //first slope
                                            System.Media.SoundPlayer myPlayer;
                                            myPlayer = new System.Media.SoundPlayer(Properties.Resources.PullUp);
                                            myPlayer.PlaySync();
                                        }
                                    }
                                    else
                                    {
                                        if (meterAboveGroundLevel < ((491.34 / 41.83) * ((-sinkRate) - 8.97)) + 105.46)
                                        {
                                            //second slope
                                            System.Media.SoundPlayer myPlayer;
                                            myPlayer = new System.Media.SoundPlayer(Properties.Resources.PullUp);
                                            myPlayer.PlaySync();
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Profile for jet
                                if (meterAboveGroundLevel <= 748.2)
                                {
                                    if (meterAboveGroundLevel <= 57.9)
                                    {
                                        if (meterAboveGroundLevel < (57 / 1.35) * ((-sinkRate) - 7.62))
                                        {
                                            //first slope
                                            System.Media.SoundPlayer myPlayer;
                                            myPlayer = new System.Media.SoundPlayer(Properties.Resources.PullUp);
                                            myPlayer.PlaySync();
                                        }
                                    }
                                    else
                                    {
                                        if (meterAboveGroundLevel < ((691.3 / 26.73) * ((-sinkRate) - 8.97)) + 57)
                                        {
                                            //second slope
                                            System.Media.SoundPlayer myPlayer;
                                            myPlayer = new System.Media.SoundPlayer(Properties.Resources.PullUp);
                                            myPlayer.PlaySync();
                                        }
                                    }
                                }
                            }
                        }
                    }

                    //ALTITUDE Ground Proximity Warning
                    DEBUG_textBlock_altAutoDetect.Text = $"Throttle: {Throttle} | M: {M}";
                    if (cbx_groundLevelAutoDetect.IsChecked == true)
                    {
                        // if player is stationary
                        if (Throttle == 0 && M == 0)
                        {
                            int altitudeAutoDetect = Convert.ToInt32(Math.Round(Convert.ToInt32(Alt) / 3.2808, 0));
                            textBox_groundLevel_m.IsEnabled = false;
                            textBox_groundLevel_m.Text = altitudeAutoDetect.ToString();
                            DEBUG_textBlock_altAutoDetect.Text = $"Plane is stationary at {altitudeAutoDetect}m";
                        }
                    }

                    //ALTITUDE Ground Proximity Warning
                    DEUBG_mAGL = Convert.ToInt32(Math.Round(Convert.ToInt32(Alt) / 3.2808, 0)) - Convert.ToInt32(textBox_groundLevel_m.Text);
                    DEBUG_textBlock_altitudeWarning.Text = $"mAGL {DEUBG_mAGL} | gear {gear} | gears_lamp {myIndicator.gears_lamp}";
                    //                                    avoid conflict with pull up warning
                    if (cbx_altitude.IsChecked == true && sinkRate >= -7.62 && IsNonZeroPositiveInteger(textBox_groundLevel_m.Text))
                    {
                        int meterAboveGroundLevel = Convert.ToInt32(Math.Round(Convert.ToInt32(Alt) / 3.2808, 0)) - Convert.ToInt32(textBox_groundLevel_m.Text);
                        if (cbx_gear.IsChecked == false)
                        {
                            // no conflict with gear down warning
                            if (meterAboveGroundLevel < Convert.ToInt32(textBox_altitudeWarning_m.Text) && gear == 0 && myIndicator.gears_lamp != "0")
                            {
                                System.Media.SoundPlayer myPlayer;
                                myPlayer = new System.Media.SoundPlayer(Properties.Resources.Altitude);
                                myPlayer.PlaySync();
                            }
                        }
                        else
                        {
                            // avoid conflict with gear down warning
                            if (IsNonZeroPositiveInteger(textBox_gearDown.Text))
                            {
                                int gearDown = Convert.ToInt32(textBox_gearDown.Text);
                                if (meterAboveGroundLevel < Convert.ToInt32(textBox_altitudeWarning_m.Text) && gear == 0 && myIndicator.gears_lamp != "0" && IAS > gearDown)
                                {
                                    System.Media.SoundPlayer myPlayer;
                                    myPlayer = new System.Media.SoundPlayer(Properties.Resources.Altitude);
                                    myPlayer.PlaySync();
                                }
                            }
                        }
                    }

                    //STALL WARNING
                    if (cbx_a.IsChecked == true)
                    {   //Stall Warning Mandatory pre-definitnions
                        System.Media.SoundPlayer stall1;
                        System.Media.SoundPlayer stall2;
                        stall1 = new System.Media.SoundPlayer(Properties.Resources.AngleOfAttackOverLimit);
                        stall2 = new System.Media.SoundPlayer(Properties.Resources.MaximumAngleOfAttack);

                        if (AoA > User.Default.AoA && AoA < 20 && (myIndicator.gears_lamp == "1" || IAS > 100))
                        {
                            if (AoA < User.Default.AoA + 2)
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
                        if (G > User.Default.GForce)
                        {
                            if (G > User.Default.GForce + 4 - User.Default.GForce / (decimal)4)
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

                    //BINGO FUEL
                    decimal DEBUG_reFuelPer = Math.Round(((decimal)Fuel / (decimal)FuelFull) * 100, 1);
                    DEBUG_textBlock_bingoFuel.Text = $"{Fuel} / {FuelFull} | percentage {DEBUG_reFuelPer} | nextWarning {textBox_bingoFuelPercentage.Text}";
                    if (cbx_bingoFuel.IsChecked == true && IsNonZeroPositiveInteger(textBox_bingoFuelPercentage.Text) && IsNonZeroPositiveInteger(textBox_bingoFuelWarningLength.Text) && Throttle != 0 && M != 0)
                    {
                        int bingoFuelPercentage = Convert.ToInt32(textBox_bingoFuelPercentage.Text);
                        int bingoFuelWarningLength = Convert.ToInt32(textBox_bingoFuelWarningLength.Text);
                        decimal remainingFuelPercentage = Math.Round(((decimal)Fuel / (decimal)FuelFull) * 100, 1);
                        if (remainingFuelPercentage > 0 && (remainingFuelPercentage > bingoFuelPercentage || nextBingoFuelWarning == -1))
                        {
                            nextBingoFuelWarning = bingoFuelPercentage;
                        }
                        if (remainingFuelPercentage <= nextBingoFuelWarning && bingoFuelWarning_count < bingoFuelWarningLength)
                        {
                            System.Media.SoundPlayer myPlayer;
                            myPlayer = new System.Media.SoundPlayer(Properties.Resources.Bingo);
                            myPlayer.PlaySync();
                            bingoFuelWarning_count++;
                            DEBUG_textBlock_bingoFuelWarning_count.Text = $"bingoFuelWarning_count: {bingoFuelWarning_count}/{bingoFuelWarningLength}";
                        }
                        if (cbx_bingoFuelRepeat.IsChecked == true)
                        {
                            DEBUG_textBlock_bingoFuel.Text = $"{Fuel} / {FuelFull} | percentage {remainingFuelPercentage} | nextWarning {textBox_bingoFuelPercentage.Text} repeat";
                            if (remainingFuelPercentage < nextBingoFuelWarning && bingoFuelWarning_count >= bingoFuelWarningLength)
                            {
                                if ((nextBingoFuelWarning / 2) > remainingFuelPercentage)
                                {
                                    while ((nextBingoFuelWarning) > remainingFuelPercentage)
                                    {
                                        nextBingoFuelWarning = nextBingoFuelWarning / 2;
                                    }
                                }
                                else
                                {
                                    nextBingoFuelWarning = nextBingoFuelWarning / 2;
                                }
                                bingoFuelWarning_count = 0;
                                DEBUG_textBlock_bingoFuelNextWarning.Text = $"At {Fuel} / {FuelFull} | percentage {remainingFuelPercentage} changed nextWarning {nextBingoFuelWarning}";
                            }
                        }
                    }

                    //=========LOW PRIORITY WARNINGS=======
                    //GEAR UP/DOWN
                    if (cbx_gear.IsChecked == true && IsNonZeroPositiveInteger(textBox_gearUp.Text))
                    {
                        int gearUp = Convert.ToInt32(textBox_gearUp.Text);
                        if (gear == 100 && IAS > gearUp && myIndicator.gears_lamp == "0")
                        {
                            System.Media.SoundPlayer myPlayer;
                            myPlayer = new System.Media.SoundPlayer(Properties.Resources.GearUp);
                            myPlayer.PlaySync();
                        }
                    }

                    if (cbx_gear.IsChecked == true && IsNonZeroPositiveInteger(textBox_gearDown.Text) && IsNonZeroPositiveInteger(textBox_groundLevel_m.Text))
                    {
                        int gearDown = Convert.ToInt32(textBox_gearDown.Text);
                        int meterAboveGroundLevel = Convert.ToInt32(Math.Round(Convert.ToInt32(Alt) / 3.2808, 0)) - Convert.ToInt32(textBox_groundLevel_m.Text);
                        meterAboveGroundLevel = Convert.ToInt32(Math.Round(Convert.ToInt32(Alt) / 3.2808, 0)) - Convert.ToInt32(textBox_groundLevel_m.Text);
                        if ((AoA < 20 || Vspeed > -10) && gear == 0 && IAS < gearDown && IAS > 40 && Throttle < 20 && myIndicator.gears_lamp != "0" && meterAboveGroundLevel < 150/*Alt < 500 && flaps > 20*/)
                        {
                            System.Media.SoundPlayer myPlayer;
                            myPlayer = new System.Media.SoundPlayer(Properties.Resources.GearDown);
                            myPlayer.PlaySync();
                        }
                    }
                }
                else
                {
                    StopPlayLooping();
                    dispatcherTimer_getData.Stop();
                    dispatcherTimer_connect.Start();
                }
            }
            catch (Exception ex)
            {
                tbx_msgs.Text = ex.Message;
                dispatcherTimer_getData.Stop();
                dispatcherTimer_connect.Start();
            }
        }

        public void StopPlayLooping()
        {
            System.Media.SoundPlayer stall1 = new System.Media.SoundPlayer();
            System.Media.SoundPlayer stall2 = new System.Media.SoundPlayer();
            stall1.Stop();
            stall2.Stop();
        }

        public bool IsPositiveInteger(String input)
        {
            int testValue;
            if (int.TryParse(input, out testValue))
            {
                if (testValue >= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public bool IsNonZeroPositiveInteger(String input)
        {
            int testValue;
            if (int.TryParse(input, out testValue))
            {
                if (testValue > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            dispatcherTimer_getData.Stop();
            dispatcherTimer_connect.Stop();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WTConnect();
        }

        private void button_start_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer_connect.Start();
            if (dispatcherTimer_connect.IsEnabled)
            {
                button_start.IsEnabled = false;
                button_stop.IsEnabled = true;
            }
        }

        //TODO: User-assigned key binding for toggle of the program
        private void button_stop_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer_getData.Stop();
            dispatcherTimer_connect.Stop();
            button_start.IsEnabled = true;
            button_stop.IsEnabled = false;
            System.Media.SoundPlayer myPlayer1;
            System.Media.SoundPlayer myPlayer2;
            myPlayer1 = new System.Media.SoundPlayer(Properties.Resources.AngleOfAttackOverLimit);
            myPlayer2 = new System.Media.SoundPlayer(Properties.Resources.MaximumAngleOfAttack);
            myPlayer1.Stop(); myPlayer2.Stop();
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                User.Default.EnableG = cbx_g.IsChecked.Value;
                User.Default.GForce = Convert.ToInt32(slider_G.Value);
                User.Default.EnableA = cbx_a.IsChecked.Value;
                User.Default.AoA = Convert.ToInt32(slider_A.Value);
                User.Default.EnablePullUp = cbx_pullUp.IsChecked.Value;
                User.Default.PlaneType = comboBox_planeType.SelectedIndex;
                User.Default.GroundLevel = Convert.ToInt32(textBox_groundLevel_m.Text);
                User.Default.EnableGroundLevelAutoDetect = cbx_groundLevelAutoDetect.IsChecked.Value;
                User.Default.EnableAltitude = cbx_altitude.IsChecked.Value;
                User.Default.AltitudeWarning = Convert.ToInt32(textBox_altitudeWarning_m.Text);
                User.Default.EnableBingoFuel = cbx_bingoFuel.IsChecked.Value;
                User.Default.BingoFuelPercentage = Convert.ToInt32(slider_bingoFuel.Value);
                User.Default.BingoFuelWarningLength = Convert.ToInt32(textBox_bingoFuelWarningLength.Text);
                User.Default.EnableBingoFuelRepeat = cbx_bingoFuelRepeat.IsChecked.Value;
                User.Default.EnableGear = cbx_gear.IsChecked.Value;
                User.Default.GearUp = Convert.ToInt32(textBox_gearUp.Text);
                User.Default.GearDown = Convert.ToInt32(textBox_gearDown.Text);
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
                User.Default.PlaneType = 0;
                User.Default.GroundLevel = 75;
                User.Default.EnableGroundLevelAutoDetect = true;
                User.Default.EnableAltitude = true;
                User.Default.AltitudeWarning = 100;
                User.Default.EnableBingoFuel = true;
                User.Default.BingoFuelPercentage = 10;
                User.Default.BingoFuelWarningLength = 5;
                User.Default.EnableBingoFuelRepeat = true;
                User.Default.EnableGear = true;
                User.Default.GearDown = 270;
                User.Default.GearUp = 270;
                User.Default.Save();

                cbx_g.IsChecked = User.Default.EnableG;
                slider_G.Value = Convert.ToDouble(User.Default.GForce);
                cbx_a.IsChecked = User.Default.EnableA;
                slider_A.Value = Convert.ToDouble(User.Default.AoA);
                cbx_pullUp.IsChecked = User.Default.EnablePullUp;
                comboBox_planeType.SelectedIndex = User.Default.PlaneType;
                textBox_groundLevel_m.Text = User.Default.GroundLevel.ToString();
                cbx_groundLevelAutoDetect.IsChecked = User.Default.EnableGroundLevelAutoDetect;
                cbx_altitude.IsChecked = User.Default.EnableAltitude;
                textBox_altitudeWarning_m.Text = User.Default.AltitudeWarning.ToString();
                cbx_bingoFuel.IsChecked = User.Default.EnableBingoFuel;
                slider_bingoFuel.Value = Convert.ToDouble(User.Default.BingoFuelPercentage);
                textBox_bingoFuelWarningLength.Text = User.Default.BingoFuelWarningLength.ToString();
                cbx_bingoFuelRepeat.IsChecked = User.Default.EnableBingoFuelRepeat;
                cbx_gear.IsChecked = User.Default.EnableGear;
                textBox_gearUp.Text = User.Default.GearUp.ToString();
                textBox_gearDown.Text = User.Default.GearDown.ToString();
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

        private void button_setAltitude_current_Click(object sender, RoutedEventArgs e)
        {
            if ((myState.valid == "true") && (myIndicator.valid == "true") && (myIndicator.type != "dummy_plane") && (myIndicator.type != null))
            {
                string Alt;
                if (Convert.ToInt32(myState.H) >= 1)
                {
                    double Alt_unknownUnit;
                    if (myIndicator.altitude_10k != null)
                    {
                        Alt_unknownUnit = Convert.ToDouble(myIndicator.altitude_10k, culture);
                    }
                    else
                    {
                        Alt_unknownUnit = Convert.ToDouble(myIndicator.altitude_hour, culture);
                    }

                    if (Convert.ToInt32(myState.H) * 3 < Alt_unknownUnit)
                    {
                        // convert alt from imperial to meteric
                        Alt_unknownUnit = Alt_unknownUnit / 3.2808;
                        String meter_rounded = String.Format("{0:0}", Math.Truncate(Alt_unknownUnit * 10) / 10);
                        Alt_unknownUnit = Convert.ToDouble(meter_rounded);
                    }
                    Alt = Alt_unknownUnit.ToString();
                    textBox_groundLevel_m.Text = Alt;

                    if (cbx_groundLevelAutoDetect.IsChecked == true)
                    {
                        cbx_groundLevelAutoDetect.IsChecked = false;
                    }
                }
            }
        }

        private void button_setAltitude_seaLevel_Click(object sender, RoutedEventArgs e)
        {
            textBox_groundLevel_m.Text = "0";
            if (cbx_groundLevelAutoDetect.IsChecked == true)
            {
                cbx_groundLevelAutoDetect.IsChecked = false;
            }
        }

        private void textBox_bingoFuelPercentage_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            nextBingoFuelWarning = -1;
        }

        private void cbx_groundLevelAutoDetect_Unchecked(object sender, RoutedEventArgs e)
        {
            if (textBox_groundLevel_m.IsEnabled == false)
            {
                textBox_groundLevel_m.IsEnabled = true;
            }
        }

        private void cbx_groundLevelAutoDetect_Checked(object sender, RoutedEventArgs e)
        {
            if (textBox_groundLevel_m.IsEnabled == true)
            {
                textBox_groundLevel_m.IsEnabled = false;
            }
        }
    }

    public class mToftConversion : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                int meter = System.Convert.ToInt32(value.ToString());
                double feet = meter * 3.2808;
                String feet_rounded = String.Format("{0:0}", Math.Truncate(feet * 10) / 10) + "ft";
                return feet_rounded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "ERROR";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
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
                String mph_rounded = String.Format("{0:0}", Math.Truncate(mph * 10) / 10) + "mph";
                return mph_rounded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return "ERROR";
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return false;
        }
    }
}