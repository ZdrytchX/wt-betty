using Newtonsoft.Json;

namespace wt_betty
{
    public class indicator
    {
        public string timestamp { get; set; }
        public string valid { get; set; }
        public string type { get; set; }
        public string speed { get; set; }
        public string pedals1 { get; set; }
        public string pedals2 { get; set; }
        public string stick_elevator { get; set; }
        public string stick_ailerons { get; set; }
        public string vario { get; set; }
        public string altitude_hour { get; set; }
        public string altitude_min { get; set; }
        public string altitude_10k { get; set; }
        public string aviahorizon_roll { get; set; }
        public string aviahorizon_pitch { get; set; }
        public string bank { get; set; }
        public string turn { get; set; }
        public string compass { get; set; }
        public string clock_hour { get; set; }
        public string clock_min { get; set; }
        public string clock_sec { get; set; }
        public string manifold_pressure { get; set; }
        public string rpm { get; set; }
        public string oil_pressure { get; set; }
        public string oil_temperature { get; set; }
        public string water_temperature { get; set; }
        public string carb_temperature { get; set; }
        public string fuel1 { get; set; }
        public string fuel_pressure { get; set; }
        public string gears { get; set; }
        public string gears_lamp { get; set; }
        public string trimmer { get; set; }
        public string throttle { get; set; }
        public string weapon1 { get; set; }
        public string ammo_counter1 { get; set; }
        public string ammo_counter2 { get; set; }
        public string ammo_counter3 { get; set; }
        public string ammo_counter4 { get; set; }

    }
    public class state
    {
        public string timestamp { get; set; }

        [JsonProperty(PropertyName = "valid")]
        public string valid { get; set; }

        [JsonProperty(PropertyName = "aileron, %")]
        public string aileron { get; set; }

        [JsonProperty(PropertyName = "elevator, %")]
        public string elevator { get; set; }

        [JsonProperty(PropertyName = "rudder, %")]
        public string rudder { get; set; }

        [JsonProperty(PropertyName = "flaps, %")]
        public string flaps { get; set; }

        [JsonProperty(PropertyName = "gear, %")]
        public string gear { get; set; }

        [JsonProperty(PropertyName = "H, m")]
        public string H { get; set; }

        [JsonProperty(PropertyName = "TAS, km/h")]
        public string TAS { get; set; }

        [JsonProperty(PropertyName = "IAS, km/h")]
        public string IAS { get; set; }

        [JsonProperty(PropertyName = "Mfuel, kg")]
        public string Mfuel { get; set; }

        [JsonProperty(PropertyName = "Mfuel0, kg")]
        public string Mfuel0 { get; set; }

        [JsonProperty(PropertyName = "M")]
        public string M { get; set; }

        [JsonProperty(PropertyName = "AoA, deg")]
        public string AoA { get; set; }

        [JsonProperty(PropertyName = "AoS, deg")]
        public string AoS { get; set; }
        public string Ny { get; set; }

        [JsonProperty(PropertyName = "Vy, m/s")]
        public string Vy { get; set; }

        [JsonProperty(PropertyName = "Wx, deg/s")]
        public string Wx { get; set; }

        [JsonProperty(PropertyName = "throttle 1, %")]
        public string throttle_1 { get; set; }

        [JsonProperty(PropertyName = "RPM throttle 1, %")]
        public string RPM_throttle_1 { get; set; }

        [JsonProperty(PropertyName = "mixture 1, %")]
        public string mixture_1 { get; set; }

        [JsonProperty(PropertyName = "radiator 1, %")]
        public string radiator_1 { get; set; }

        [JsonProperty(PropertyName = "compressor stage 1")]
        public string compressor_stage_1 { get; set; }

        [JsonProperty(PropertyName = "magneto 1")]

        public string magneto_1 { get; set; }

        [JsonProperty(PropertyName = "power 1, hp")]
        public string power_1_hp { get; set; }

        [JsonProperty(PropertyName = "RPM 1")]
        public string RPM_1 { get; set; }

        [JsonProperty(PropertyName = "manifold pressure 1, atm")]
        public string manifold_pressure_1_atm { get; set; }

        [JsonProperty(PropertyName = "oil temp 1, C")]
        public string oiltemp_1_C { get; set; }

        [JsonProperty(PropertyName = "pitch 1, deg")]
        public string pitch1_deg { get; set; }

        [JsonProperty(PropertyName = "thrust 1, kgs")]
        public string thrust1_kgs { get; set; }

        [JsonProperty(PropertyName = "efficiency 1, %")]
        public string efficiency1 { get; set; }

    }
}
