// ELEVATOR CONTROLS

LIST accessLevels = (Level0), (Level1), (Level2), (Level3), (Level4), (Level5), (Level6)
VAR currentLevel = Level1

==function AddElevatorLevel(level)==
// Add a new level to access!
~accessLevels += level

==OpenElevator
#autoContinue
#openelevator
Choose your level
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
+ [{UseButton("Level2")} Storage]
->GoToLevel2
- else:
+ [{DisableButton()}{UseButton("Level2")} Storage]
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



+ [Nevermind.]
->CloseElevator

=CloseElevator
#closeelevator
<b></b>
->DONE

=GoToLevel0
~currentLevel = Level0
->CloseElevator
=GoToLevel1
~currentLevel = Level1
->CloseElevator
=GoToLevel2
~currentLevel = Level2
->CloseElevator
=GoToLevel3
~currentLevel = Level3
->CloseElevator
=GoToLevel4
~currentLevel = Level4
->CloseElevator
=GoToLevel5
~currentLevel = Level5
->CloseElevator
=GoToLevel6
~currentLevel = Level6
->CloseElevator
