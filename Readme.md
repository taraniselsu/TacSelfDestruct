A Self Destruct from Thunder Aerospace Corporation (TAC),
designed by Taranis Elsu. Slightly modified by DrSpaceMonkey.

For use with the Kerbal Space Program, http://kerbalspaceprogram.com/

This mod is made available under the Attribution-NonCommercial-ShareAlike 3.0 (CC
BY-NC-SA 3.0) creative commons license. See the LICENSE.txt file.

Source code for the original is available at https://github.com/taraniselsu/TacSelfDestruct

Modified source is available at: https://github.com/DrSpaceMonkey/TacSelfDestruct

### Any problems experienced with this version are not Taranis Elsu's fault. Please leave him alone if I broke something.

### Features

* Self Destruct - makes the entire vessel explode! Make sure you EVA your crew first!
* Explode - only makes the Self Destruct part explode. Useful for creating a little
      explosion or getting rid of the extra weight. Not responsible for damage to the
      vessel.
* Can now be activated from EVA.
* The Self Destruct now waits 10 seconds, and goes through a countdown sequence,
      before activating.
	  
* Can now blow up it's parent part via the "Detonate Parent!" functionality, 
      effectively making a decoupler out of whatever you stick it to
* Can toggle the part's staging action between self destructing the vessel, and
      detonating the parent part.


### Installation procedure 

1) Copy everything in the GameData directory to the {KSP}/GameData directory.

Optional:
Add the following lines to any part that you want to have the Self Destruct functionality:

     MODULE
     {
        name = TacSelfDestruct
        timeDelay = 10.0
        canStage = false
     }
