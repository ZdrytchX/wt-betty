## wt-betty
War Thunder's Betty. Cockpit warning sounds for simulator pilots.

************************************************************************************
This sofware does not modify or comprimise the integrity of any of
the core War Thunder game files nor it does not have direct access to
memory or decompile binary files rather It simply reads Json data from
web client  at http://localhost:8111  which is available when the
game starts.
************************************************************************************

# Notes
This is a fork of ZdrytchX's work which itself is a fork of SoulMaril's work
 
# Improvement as of 4th February 2019
Additional rules to avoid unintented warning before spawn:
No warning would be played if the plane is 'dummy_plane' or null.

altitude_10k is now the prioritized source for Alt followed by altitude_hour.
Added checking and conversion for Alt.
Since the unit of alt is based on the plane's country of origin (meteric for german plane and imperial for british plane, etc.), unit conversion is necessary. The unit of alt would always be feet.

New UI element for:
Pull up warning
Altitude warning
Bingo Fuel warning

Pull up warning:
Pull up warning is now based on sink rate a.k.a. descent rate and altitude.
Each plane type(turboprop or jet) has its own distinct rule to determine excessive sink rate. The rule can be found [here](https://aerocontent.honeywell.com/aero/common/documents/Mk_VI_VIII_EGPWS.pdf#page=11). 
Ground level can be set to auto-detect. If auto-detect is active and the plane is stationary(throttle and mach number are both 0), programme will overwrite existing ground level with current altitude. This would technically works for airfield and carrier landing/takeoff. Additionally, it would works if you smash into the ground(don't test that).
Added a button to set sea level as ground level.
Added a button to set current altitude as ground level.

Bingo fuel warning:
Added a slider to control the threshold of bingo fuel warning.
Added a counter to repeat the aural warning and a bound text box to control the length of aural warning.
Added a check box for an option to repeat the bingo fuel warning. The rule is a geometric sequence with a common ratio of 1/2.
Say, the threshold of bingo fuel warning is 20%. If repeat is checked, the same warning would be heard when the remaining fuel percentage reached 10%, 5% , 2% and 1%.

Gear down warning:
Still unsure about the rule.
 
All newly added parameters can be saved and loaded via User.settings
