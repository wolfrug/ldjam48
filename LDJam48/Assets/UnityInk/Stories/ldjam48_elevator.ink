// ELEVATOR CONTROLS

LIST accessLevels = Level0, (Level1), Level2, Level3, Level4, Level5, Level6
VAR currentLevel = Level1
VAR elevatorBlocked = false

==AddAllElevator==
{AddElevatorLevel(Level0)}
{AddElevatorLevel(Level1)}
{AddElevatorLevel(Level2)}
{AddElevatorLevel(Level3)}
{AddElevatorLevel(Level4)}
{AddElevatorLevel(Level5)}
{AddElevatorLevel(Level6)}
->->

==function AddElevatorLevel(level)==
// Add a new level to access!
~accessLevels += level

==OpenElevator
#autoContinue
#openelevator
{UseText("ObjectiveList")}To do:<br>-Get Max into mainframe.<br>-Flip some switches.
#autoContinue
{accessLevels?Level0 && currentLevel !=Level0:
+ [{UseButton("Level0")} Exit Hatch]
->GoToLevel0
- else:
+ [{DisableButton()}{UseButton("Level0")} Exit Hatch]
->CloseElevator
}
{accessLevels?Level1 && currentLevel !=Level1:
+ [{UseButton("Level1")} Entrance]
->GoToLevel1
- else:
+ [{DisableButton()}{UseButton("Level1")} Entrance]
->CloseElevator
}
{accessLevels?Level2 && currentLevel !=Level2:
+ [{UseButton("Level2")} Systems]
->GoToLevel2
- else:
+ [{DisableButton()}{UseButton("Level2")} Systems]
->CloseElevator
}
{accessLevels?Level3 && currentLevel !=Level3:
+ [{UseButton("Level3")} Offices]
->GoToLevel3
- else:
+ [{DisableButton()}{UseButton("Level3")} Offices]
->CloseElevator
}
{accessLevels?Level4 && currentLevel !=Level4:
+ [{UseButton("Level4")} Lab Level 1]
->GoToLevel4
- else:
+ [{DisableButton()}{UseButton("Level4")} Lab Level 1]
->CloseElevator
}
{accessLevels?Level5 && currentLevel !=Level5:
+ [{UseButton("Level5")} Lab Level 2]
->GoToLevel5
- else:
+ [{DisableButton()}{UseButton("Level5")} Lab Level 2]
->CloseElevator
}
{accessLevels?Level6 && currentLevel !=Level6:
+ [{UseButton("Level6")} Moonpool]
->GoToLevel6
- else:
+ [{DisableButton()}{UseButton("Level6")} Moonpool]
->CloseElevator
}
//+ [(Cheat) Add all levels.]
//->AddAllElevator->OpenElevator
* {elevatorBlocked && CheckItem(item_crowbar, 1)} [Pry open door]
{Stevie} Here goes nothing...

{Stevie} Hnngn...

{Stevie} Nice. Only to go now is down...
{AddElevatorLevel(Level5)}
{AddElevatorLevel(Level6)}
->OpenElevator
//->AddAllElevator->OpenElevator
+ [Nevermind.]
->CloseElevator

=CloseElevator
#closeelevator
#autoContinue
<b></b>
->DONE

=GoToLevel0
~currentLevel = Level0
->CloseElevator
=GoToLevel1
~currentLevel = Level1
{Stevie} Back again. Gotta love the sunlight.
->CloseElevator
=GoToLevel2
{Stevie} I really don’t know why they restrict access to this level. It’s just more of the same.
~currentLevel = Level2
->CloseElevator
=GoToLevel3
{Stevie} And the first super secret forbidden-by-the-employment-contract level is...an office. Underwhelming. I wanted mutant Mermen, and now nothing less will do.
~currentLevel = Level3
->CloseElevator
=GoToLevel4
{Stevie} Woah! This place is <i>definitely</i> as science experiment gone wrong. Thank goodness my oxygen is coming out of a tank because I wouldn’t want to breathe the air down here.
~currentLevel = Level4
->CloseElevator
=GoToLevel5
{GoToLevel5<2:
{Stevie}Oh....not good. Not good at all.

{Stevie} Shit. Door's jammed. Great.

{Stevie} I am not getting out of this...creepy place...unless I can pry these doors open.
~accessLevels = ()
~elevatorBlocked = true
- else:

{Stevie} Emergency lighting, that’s not a good sign. And somehow the mushrooms down here are even larger than the previous floor. Life prevails! Yay?
}
~currentLevel = Level5
->CloseElevator
=GoToLevel6
{Stevie} Well, this is it. The final level. Let’s go get some answers.
//->AddAllElevator->
~currentLevel = Level6
->CloseElevator
