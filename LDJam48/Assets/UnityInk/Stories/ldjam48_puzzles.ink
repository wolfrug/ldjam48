// For nitty gritty mechanics

=debugPuzzles
Debug which puzzle?
+ [Level0Interactable 1]
->InteractableLevel1.Interact1
+ [Level0Interactable 2]
->InteractableLevel1.Interact2
+ [Level4Interactable 1]
->InteractableLevel4.Interact1
+ [Level4Interactable 2]
->InteractableLevel4.Interact2
+ [Level4Interactable 3]
->InteractableLevel4.Interact3
+ [Level4Interactable 4]
->InteractableLevel4.Interact4


==stopInteract
{debug: ->debugPuzzles}
->DONE

/// TEMPLATE
===InteractableLevel0
{Stevie} What was I doing?
->stopInteract
=Interact1
{Complete1<1:
{Stevie} Right.
+ [Fix it.]
{Stevie} All fixed.
->Complete1
- else:
{Stevie} I already fixed this.
}
+ [Leave it.]
{Stevie} Nah, later.
->stopInteract

=Complete1
{Stevie} Easy peasy.
->CompleteAll

=Interact2
{Complete2<1:
{Stevie} Right.
+ [Fix it.]
{Stevie} All fixed.
->Complete2
- else:
{Stevie} I already fixed this.
}
+ [Leave it.]
{Stevie} Nah, later.
->stopInteract

=Complete2
{Stevie} Easy peasy.
->CompleteAll

=CompleteAll
{Complete1 && Complete2:
{Stevie} Solved! On to the next level.
{AddElevatorLevel(Level1)}
- else:
{Stevie} Still stuff to be done.
}
->stopInteract

// END TEMPLATE


===InteractableLevel1
->stopInteract
=Interact1
{RadioLevel1.Talk < 1:
{Stevie} I need to get back in touch with Max first before I poke at this...
->stopInteract
}
{Complete1<1:
{Stevie} Bad news.

{Max} It’s busted?

{Stevie} Mega-busted.

{Max} Great. All right. I’ll put the call through to head office and tell them we need clearance for Level 2.

{Stevie} And I’ll just waste oxygen down here until then.

{Max} There should be some extra canisters down there. Why don’t you fill up?

{CheckItem(item_smalloxygen, 1) > 1:
{Stevie} Way ahead of you.
-else:
{Stevie} Good plan.
}

{Max} All right. Mosey on over to the elevator doors when you’re done taking in the sights.

+ [Over and out.]
->Complete1
- else:
{Stevie} Not much to do here. Still busted. I should head towards the elevator.
}
->stopInteract
=Complete1
->CompleteAll

=Interact2
{RadioLevel1.Talk < 1:
{Stevie} I bet Max would love this. Which reminds me...I better get back in touch.
->stopInteract
}
{Complete2<1:
{Max} See anything interesting down there?

{Stevie} So much.

{Stevie} Over here are some tubes. And over there, yep, some more tubes. 

{Stevie} Why, are you getting bored?

{Max} No way, I’ve got my feet up on the dash, catching up on some light reading of um, oceanic current patterns? It’s riveting. I feel like I’m on a mini-holiday.

{Stevie} Oooh, oceanic currents. Saucy.

{Stevie} Save me a seat would you.

{Max} If you want to join, you’d better hurry up. This storm is starting to threaten my dreams of suntanning on top of the plane.

{Stevie} Oh trust me, I can’t <i>wait</i> to get out of this suit.

{Stevie} Who says we have to wear these things all the time anyway.

{Max} It’s in the manual!

{Stevie} Manual schmanual.

{Stevie} I’m a glorified handyman, not a deep sea diver.

{Max} Stevie...

{Stevie} I know, I know!

{Stevie} Rule 1: Thou shalt not enter a facility without the appropriate equipment.

{Stevie} Rule 2: Thou shalt not proceed below the upper levels of the facility without express permission from Head Office.

{Stevie} Rule 3: Thou shalt not--

{Max} --Stevie.

{Stevie} Yes?

{Max} Shut up and get back to work would you, I’m trying to read important literature over here.

{Stevie} Rude.

->Complete2
- else:
{Stevie} I should stop dragging my feet.
}
->stopInteract
=Complete2
->CompleteAll

=CompleteAll
{Complete1 && Complete2:
{Max} Oh hey, wait! Jeesh, that was fast. Head office usually takes ages to respond to requests.

{Stevie} Do we have Level 2 access?

{Max} Looks like it. Head back to the elevator whenever you’re done.

{AddElevatorLevel(Level2)}
- else:
{Stevie} Now just to wait. I might as well take in the sights...
}
->stopInteract

===InteractableLevel2
{Stevie} What was I doing?
->stopInteract
=Interact1
{Complete1:
{Stevie} Nothing more to do here.
->stopInteract
}
{Stevie} Alright, I’m here. Let’s hope this one works.

{Stevie} Booting her up now. Are you seeing anything yet?

{Max} Sliding into the station system logs now. Why hello there my little beauty. 

{Stevie} Stop sweet-talking the machinery.

{Max} But the machinery responds better when you have a good bedside manner, every maintenance technician worth their salt knows that.

{Stevie} Is that right?

{Max} Yeah it is. They’re a bit like you, tough on the outside, but with a little bit of--

{Stevie} Yeah yeah, Casanova, we get the picture.

{Stevie} So tell me, what sweet nothings is the system whispering in your ear. Any clue as to why it's malfunctioning?.

{Max} Right now...It’s less a whisper, and more the screeching of a depressurising rift-breach. Seriously, ouch, this data is all over the place.

{Stevie} Do you think you can fix it? I’d hate to call in a clean-up crew. Those guys give me the creeps, too much rift radiation in their blood…

{Max} Never fear, Max the miracle worker is here. Just wait while I hard reset this baby, and we’ll be back at the Hub in time for dinner, you’ll see.

{Stevie} Is that a promise?

{Max} Just try and stop me. Lukewarm soup rations, and rehydrated crackers here I come.

{Stevie} You’re an idiot.

{Max} Damn right I am. That’s why you keep me around.

{Stevie} …

{Max} Soo….slight issue.

{Max} To reset this, we need to flip the failsafe in the other oom.

{Stevie} Of course we do.

{Max} Mosey on over there. We’ll have to do it kind-of simultaneously though.

{Stevie} Sure, we’ll do a one-two-three on the radio.

{Max} Unless the radio is on the fritz.

{Stevie} I’ll find you, don’t worry.

{Max} Roger that.
->Complete1

=Complete1
->CompleteAll

=Interact2
{Complete2>1:
{Stevie} Nothing more to do here.
->stopInteract
}
{Complete1<1:
{Stevie} It's some kind of breaker or something. Won't help Max get into the station mainframe though.
->stopInteract
}
{Interact2>1 && RadioLevel2.Talk<1:
{Stevie} No point coming back here until I've gotten back in contact with Max.
->stopInteract
}
{RadioLevel2.Talk<1:
{Stevie} All right Max, I'm here.

{Max} *static*

{Stevie} Typical.

{Max} *crackle* *whine*

{Stevie} I got it, I got it.

{Stevie} I know I heard a good spot here somewhere...
->stopInteract
}
{Stevie} All right...I'm at the breaker.

{Max} Great. On three.

{Max} One.

{Max} Two.

{Voice} Please...

{Stevie} What--

{Max} Three!

* [Pull the breaker.]
->Complete2

=Complete2
{Stevie} Wow. Okay, well.

{Stevie} It's definitely doing something. How does it look on your side?

{Max} Great. Primo. Yeah. All systems as nominal as they can be in this damned Rift-storm, anyway...
->CompleteAll

=CompleteAll
{Complete1 && Complete2:
{Max} Huh. Weird.

{Stevie} What?

{Max} The breaker must have broken something. It's opened access to level 3.

{Stevie} So I could go down and explore if I wanted to.

{Max} That would...not strictly be according to protocol.

{Stevie} Something might still be broken down here. Right? We should check it out.

{Max} Please Stevie. Just...come back up.

{Stevie} Over and out.

{AddElevatorLevel(Level0)}
{AddElevatorLevel(Level3)}
- else:
{Interact2>1:
{Stevie} I bet the breaker is the one I saw in the other room.
- else:
{Stevie} Usually the breakers are on the other side of the level.
}
}
->stopInteract

===InteractableLevel3
->stopInteract
=Interact1
{Interact1>2:
{CheckItem(item_keycard, 1):
{Stevie} Well would you look at that.

{Max} You found the keycard?

{Stevie} Yep, I have full access to the lower floors now.

{Stevie} I’ll still need to unlock them by going down one at a time, but the way seems clear.

{Max} Well just look at you, cracking security systems all by your lonesome. You’re the pilot, the technician, and now the programmer as well.

{Max} I feel superfluous.

{Max} Soon you won’t need me at all!

{Stevie} You big loon, of course you’re useful. You make the best pancakes. I wouldn’t get rid of a pancake maker of your calibre on some mere whim.

{Stevie} Anyway let’s not get too excited. I literally just pushed a card into a reader, big whoop.
->Complete1
- else:
{Stevie} Guh. Guessing passwords isn't going to work. I'll need a keycard.
->stopInteract
}
}

{CheckItem(item_keycard, 1):
{Stevie} Well would you look at that.

{Max} You found the computer?

{Stevie} Yep, and thanks to a little keycard I found over yonder, I have full access to the lower floors.

{Stevie} I’ll still need to unlock them by going down one at a time, but the way seems clear.

{Max} Well just look at you, cracking security systems all by your lonesome. You’re the pilot, the technician, and now the programmer as well.

{Max} I feel superfluous.

{Max} Soon you won’t need me at all!

{Stevie} You big loon, of course you’re useful. You make the best pancakes. I wouldn’t get rid of a pancake maker of your calibre on some mere whim.

{Stevie} Anyway let’s not get too excited. I literally just pushed a card into a reader, big whoop.
->Complete1
-else:

{Stevie} Well the good news is I found a computer.

{Max} And the bad news?

{Stevie} It wants a keycard of some kind. Don’t suppose they left one of those lying around.

{Max} Well, it’s worth a look. Rummage around, you might get lucky.
->stopInteract
}
->stopInteract
=Complete1
{Consume(item_keycard, 1)}
{Stevie} ...All right. The elevator to level 4 should be open now.
{AddElevatorLevel(Level4)}
->stopInteract

/*
=Interact2
{Complete2<1:
{Stevie} Right.
+ [Fix it.]
{Stevie} All fixed.
->Complete2
- else:
{Stevie} I already fixed this.
}
+ [Leave it.]
{Stevie} Nah, later.
->stopInteract

=Complete2
{Stevie} Easy peasy.
->CompleteAll

=CompleteAll
{Complete1 && Complete2:
{Stevie} Solved! On to the next level.
{AddElevatorLevel(Level1)}
- else:
{Stevie} Still stuff to be done.
}
->stopInteract
*/

===InteractableLevel4
->stopInteract
=Interact1
{not RadioLevel4.Talk:
{Stevie} Hm. I need to find the signal, not look at spigots.
->stopInteract
}
{not Complete1:
{Stevie} Right. Here goes nothing.
* [Turn the spigot]
#spigot1
{Stevie} All right. Something is definitely happening.
->Complete1
- else:
{Stevie} I wish all spigots turned as nicely as you.
->stopInteract
}
=Complete1
->CompleteAll

=Interact2
{not RadioLevel4.Talk:
{Stevie} Hm. I need to find the signal, not look at spigots.
->stopInteract
}
{not Complete2:
{Stevie} Rusty. But not too rusty yet. I think.
* [Turn the spigot]
#spigot2
{Stevie} That definitely turned something on.
->Complete2
- else:
{Stevie} Still open. Still good.
->stopInteract
}

=Complete2
->CompleteAll

=Interact3
{not RadioLevel4.Talk:
{Stevie} Hm. I need to find the signal, not look at spigots.
->stopInteract
}
{not Complete3:
{Stevie} I guess this is the one. Let's see if it turns.
* [Turn the spigot]
#spigot3
{Stevie} I hope the machinery can handle it...
->Complete3
- else:
{Stevie} You were a good little spigot.
->stopInteract
}

=Complete3
->CompleteAll


=Interact4
{not RadioLevel4.Talk:
{Stevie} Hm. I need to find the signal, not look at spigots.
->stopInteract
}
{not Complete4:
{Stevie} All right, time to do this...

+ [Turn the spigot]
{Stevie} Hngggg!

{Stevie} Nope. Not with my bare hands. Too rusted.

{CheckItem(item_wrench, 1):
{Stevie} Luckily I don't come unprepared.
* [Turn the spigot with wrench]
#spigot4
{Stevie} Hah! Take that!
->Complete4
- else:
{Stevie} Well shit. I am going to need leverage.

{Stevie} I'm sure there's something. Maybe not in the lab though.

{Stevie} I should look around in the upper levels..
->stopInteract.
}
- else:
{Stevie} Not even you can withstand the power of leverage.
->stopInteract
}

=Complete4
->CompleteAll

=CompleteAll
{Complete1 && Complete2 && Complete3 && Complete4:
{Stevie} Whoa, listen to that...

{Stevie} It's working. And fast, too. Where is it pumping it all?

{Stevie} Right. Just two more levels.

{AddElevatorLevel(Level5)}
- else:
~temp doneSpigots = Complete1 + Complete2 + Complete3 + Complete4
~temp spigotsLeft = 4-doneSpigots
{Stevie} That's... {print_num(doneSpigots)} down, {print_num(spigotsLeft)} to go...
}
->stopInteract

==InteractableLevel5
->stopInteract
=Interact1
{Interact1<2:
{Stevie} Are you there?

{Voice} I’m here.

{Stevie} I-- I think I see a whale...outside. An actual whale.

{Voice} Hmm, yes, I can see her too. Beautiful, isn’t she.

{Stevie} I’ve never seen one before. Only in books.

{Stevie} I thought they were all gone.

{Voice} So did I, once. The ocean is good at keeping secrets.

{Stevie} This station certainly has enough to go around.

{Voice} Yes, but not for long.

{Stevie} Voice…?

{Voice} Yes, Stevie?

{Stevie} The whale...it's glowing. I can see it’s bones glowing through the skin. Like fire. Like rift-light.

{Voice} Sometimes to survive, you have to change what you are.

{Voice} But don’t worry, Stevie. It doesn’t hurt. Nothing hurts anymore.
- else:
{Stevie} It looks almost close enough to touch.
}
->stopInteract

=Interact2
{not CheckItem(item_key, 1):
{Stevie} I think...I see something in there.

{Stevie} But the damned thing's locked. And too sturdy to bash open.

{RadioLevel5.Talk:
{Stevie} What did she say?

{Stevie} The coat of the dead man? Great...
- else:
{Stevie} Maybe our friendly voice has a suggestion...
}
- else:
{Stevie} Well, here goes nothing. Let's see if it fits...

{Stevie} Annd success!
#lockerOpened
{Consume(item_key, 1)}
<color=green>[You can now loot the locker.]</color>
{Stevie} Let's see what we've got.
}
->stopInteract

=Interact3
{RadioLevel5.Talk:
{Stevie} Ugh, well that’s definitely a dead guy. Let me just ...*unngh*... reach into that pocket and, huh.... It’s a note.

{Stevie} "Forgive me. I couldn't leave them alone in the dark."

{Voice} Try the other pocket

{Stevie} Ah! Sorry, you -- you startled me. What does he mean? Who was this?

{Voice} I suppose it’s only polite to introduce you. This is Dr. Alexei Kovachs, Head Scientist of Rift Station no. 2213. 

{Voice} A brilliant man. An ambitious man. And in the end...a merciful man. Perhaps more than he could have imagined.

{Stevie} You worked with him here on the Station? 

{Voice} I suppose you could say I was a part of his work, yes.

{Stevie} But I don’t understand. Why weren’t the two of you evacuated when the Station was shut down? Where you down here all this time, alone? How did you survive? How did--?

{Voice} Hush, child. Those aren’t the questions you should be focussing on.

{Stevie} Then what are the right questions.

{Voice} Soon, I promise. But first, try the other pocket.

{Stevie} ... 

{Stevie} A key? What’s this for?

{Voice} I’m sure you can figure it out.

#foundKeys
<color=green>[You can now loot the corpse.]</color>
- else:

{Stevie} Poor guy. He's been dead a while though. Must've floated here peacefully until I...

{Stevie} Anyway...
}
->stopInteract
