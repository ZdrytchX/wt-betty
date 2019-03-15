#### Changelog  

************************************************************************************
#### 15th March 2019  
Altitude data now come from myState.H. altitude_hour and altitude_10k are now deprecated.
Simplified the content of button_setAltitude_current.  
Renamed both dispatchTimer to reflect capitalization made to methods eariler.  
Added a new rule for auto detect ground level in order to fix overestimating the ground level.  
If the plane is lower than the specified ground level, the ground level would decrease by 100m or the value used by low altitude warning. The lower bound is always 0.
  
************************************************************************************
#### 14th March 2019  
Added Stop() for PlayLooping before stopping dispatcherTimer_getData.  
Capitalized most methods. Standardized imperial-metric conversion to be either \* 3.2808 or / 3.2808.  
Renamed button_setAltitude to button_setAltitude_current.  
For labels displaying imperial conversion, the error output is now "ERROR".  
  
************************************************************************************
#### 4th February 2019
**General:**  
Additional rules to avoid unintended warning before spawn:  
No warning would be played if the plane is 'dummy_plane' or null.  
  
altitude_10k is now the prioritized source for Alt followed by altitude_hour.
Added checking and conversion for Alt.
Since the unit of Alt is based on the plane's country of origin (metric for German plane and imperial for British plane, etc.), unit conversion is necessary. In the end, the unit of Alt remains as feet.
  
All user input except ground level would be checked and the input would only be accepted if it is a non-zero positive integer.  
For ground level, any positive integer is acceptable. (Because 0m = sea level.)  
All user input is metric with a bound label displaying imperial equivalent.  
All newly added parameters can be saved and loaded via User.settings.  
  
**New UI element for:**  
Pull up warning  
Altitude warning  
Bingo Fuel warning  
  
**Pull up warning:**  
Pull up warning is now determined by sink rate a.k.a. descent rate and meters above ground level.  
Meters above ground level is obtained by subtracting the altitude by ground level.
Each plane type (turboprop or jet) has its own distinct rule to determine excessive sink rate. The rule is based on Honeywell's EGPWS which can be found [here](https://aerocontent.honeywell.com/aero/common/documents/Mk_VI_VIII_EGPWS.pdf#page=11).  
Each graph has 2 slopes and each slope is expressed in [two-point form](http://mathworld.wolfram.com/Two-PointForm.html).  
Both graphs have been converted manually from *feet against feet/minute* to *meters against meters/second*. Y-axis remains as altitude and X-axis remains as sink rate.  
Added a text box for ground level which can be edited manually.  
Ground level can be set to auto-detect. If auto-detect is active and the plane is stationary (throttle and mach number are both 0), programme will overwrite existing ground level with current altitude. This would technically work for airfield and carrier landing/takeoff. Additionally, it would work if you smash into the ground (don't test that).  
Added a button to set sea level as ground level.  
Added a button to set current altitude as ground level.  
The text box for ground level is disabled if auto-detect is active. To enable the text box again, you can:  
1. uncheck the auto-detect.  
2. click the 'Sea level button.   
3. click the 'Current altitude' button.   
  
**Bingo fuel warning:**  
Added a slider and a bound text box to control the threshold of bingo fuel warning.  
Added a global counter to repeat the aural warning and a bound text box to control the length of aural warning.  
Added a check box for an option to repeat the bingo fuel warning. The rule is a [geometric sequence](https://www.purplemath.com/modules/series3.htm) with a common ratio of 1/2.  
Say, the threshold of bingo fuel warning is 20%. If the check box for repeat is checked, the same warning would be heard when the remaining fuel percentage reached 10%, 5%, 2% and 1%.  
