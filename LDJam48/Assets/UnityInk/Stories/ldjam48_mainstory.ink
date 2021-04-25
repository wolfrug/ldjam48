INCLUDE ldjam48_functions.ink
INCLUDE ldjam48_journal.ink
INCLUDE ldjam48_elevator.ink
INCLUDE ldjam48_levels.ink
INCLUDE ldjam48_puzzles.ink
INCLUDE ldjam48_radiotalks.ink

// Default main story object - always have a stitch named ==start

{debug:
->RadioLevel1.Talk
}

==start

{Max} Hello? Did you make it *skrr* … *skrrr* okay? {VoiceClip("MaxVoice1")}

{Stevie} Max? You’re breaking up.

{Max} *skrr* reception is shot *whine skrr* storm coming… {VoiceClip("MaxVoice2")}

{Stevie} Figures.

{Max} *skrrrr* {VoiceClip("MaxVoice3")}

{Stevie} All right. I need to find a spot with better reception.

<color=green>[TUTORIAL: Walk around by clicking the mouse until you find a spot where the signal is clear.]</color>
#openinventory
<color=green>[TUTORIAL: Then open the inventory (lower left) and click on the Walkie-Talkie to use it!.]

->finishRadio

==finishRadio
->DONE

==TryRadio
{tunerID:
-"RadioLevel0":
->RadioLevel0.Talk
-"RadioLevel1":
->RadioLevel1.Talk
-"RadioLevel2":
->RadioLevel2.Talk
-"RadioLevel3":
->RadioLevel3.Talk
-"RadioLevel4":
->RadioLevel4.Talk
-"RadioLevel5":
->RadioLevel5.Talk
-"RadioLevel6":
->RadioLevel6.Talk
- else:
{Stevie} Ugh. No reception. Typical.
}
->finishRadio
