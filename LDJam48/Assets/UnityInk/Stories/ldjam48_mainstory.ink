INCLUDE ldjam48_functions.ink
INCLUDE ldjam48_journal.ink
INCLUDE ldjam48_elevator.ink
INCLUDE ldjam48_levels.ink
INCLUDE ldjam48_puzzles.ink
INCLUDE ldjam48_radiotalks.ink
INCLUDE ldjam48_lootables.ink

// Default main story object - always have a stitch named ==start

{debug:
->debugPuzzles
}

==start

{Max} Hello? Did you make it *skrr* … *skrrr* okay? {VoiceClip("MaxVoice1")}

{Stevie} Max? You’re breaking up.

{Max} *skrr* reception is shot *whine skrr* storm coming… {VoiceClip("MaxVoice2")}

{Stevie} Figures.

{Max} *skrrrr* {VoiceClip("MaxVoice4")}

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

==EndGame
{RadioLevel6.Talk>0:
#bestWin
}
{died:
{Stevie} Oh thank god. Air.

{Max} You complete tosser.

{Max} You had me worried. I had to come haul your blue-faced butt out of there myself.

{Stevie} I got...carried away.

{Max} Yeah yeah. Let's go.

->WinGame
}

{OpenElevator.GoToLevel3<1:
{Max} I'm glad you decided to come back up.

{Stevie} We had a job to do and I did it.

{Stevie} Not worth to risk life and limb for.

{Max} No need to tell me twice. Let's get up in the air. I've been promised gruel!

{Stevie} Yes. Who wants adventures, anyway.
->WinGame
}

{Stevie} Oh thank god. Fresh air. Next time I want to do something like that, please talk me out of it.

{Max} I mean, I told you so. You just don’t listen.

{Stevie} I listen.

{Max} Literally never. Not once.

{Stevie} Oh hush. 

{Max} So what happens now? 

{Stevie} Well to start with, we fly home and get some of that dinner I was promised.

{Stevie} After that… I don’t really know.

{Max} Tell me about it on the way?

{RadioLevel6.Talk>0:
{Stevie} I don’t really know where to begin, but I’ll do my best.
->WinGame
}

{Stevie} Sounds good, partner. 

->WinGame

=WinGame
#WinGame
Thank you for playing.
->DONE

