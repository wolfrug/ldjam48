INCLUDE ldjam48_functions.ink
INCLUDE ldjam48_journal.ink

// Default main story object - always have a stitch named ==start

{debug:
->start
}

==start

{Max} Hello? Did you make it *skrr* … *skrrr* okay?

{Stevie} Max? You’re breaking up.

{Max} *skrr* reception is shot *whine skrr* storm coming…

{Stevie} Figures.

{Max} *skrrrr*

{Stevie} All right. I need to find a spot with better reception.

<color=green>[TUTORIAL: Walk around by clicking the mouse until you find a spot where the signal is clear.]</color>
#openinventory
<color=green>[TUTORIAL: Then open the inventory (lower left) and click on the Walkie-Talkie to use it!.]

->finishRadio

==finishRadio
->DONE

==TryRadio
{tunerID:
-"MaxTalk1":
->MaxTalk.Talk1
- else:
{Stevie} Ugh. No reception. Typical.
}
->finishRadio

==MaxTalk

=Talk1
{Stevie} Max? Do you copy?

{Max} Stevie, finally! What’s up with the signal done there?

{Stevie} Eh, who knows. 

{Stevie} These old research stations are built like tanks, it’s probably just interference from the hull.

{Max} Pre-war engineering at it’s finest.

{Stevie} You can say that again.

{Max} Well whatever you do, do it fast. Looks like a storm is browning, and you know how those get this close to the Rift.

{Stevie} Just need to find a console to reboot the system, and then we’re out of here.

{Max} Alright partner, there should be an access point on the second floor. 

{Max} Try the elevator, and if you need anything I’m just a whisper away. Max out!

{Stevie} Heh, idiot. 

->finishRadio
