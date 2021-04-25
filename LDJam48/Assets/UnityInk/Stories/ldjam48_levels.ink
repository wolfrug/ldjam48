LIST Character = Max, Stevie

// For level inspect stuff

->endInspect

==endInspect
->DONE

//// THIS IS THE TEMPLATE START - NOTE THIS IS LEVEL0, WE DON'T NEED IT
==InspectLevel0
{Stevie} Hmmm...
->endInspect
=Item1
{InspectLevel0.Item1<2:
{Stevie} Curious.
- else:
{Stevie} I've seen it already.
}
->endInspect
=Item2
{InspectLevel0.Item2<2:
{Stevie} Huh.
- else:
{Stevie} Still there.
}
->endInspect
/// THIS IS THE TEMPLATE END


==InspectLevel1


->endInspect
=Item1
//found radio signal
{InspectLevel1.Item1<2:
{Max} Stevie? Do you copy?

* {Stevie} There you are!
* {Stevie} Finally!

-{Max} What’s up with the signal down there?

*{Stevie} It’s probably just interference, the station hull is pretty thick.

-{Max} These old research stations are built like tanks, military engineering at its finest.

*{Stevie} You can say that again.

*{Stevie} Alright, I just need to find a console to reboot the system, and then we’re out of here.

-{Max} If there’s nothing nearby, use the elevator and try the second floor. If you need anything I’m just a whisper away. Max out!

*{Stevie} Hey, we agreed! No more stupid sign-off phrases.
*{Stevie} Such a child...

-{Stevie} Maybe I should take a quick look around, there might be something useful. You never know with these old places.

}
->endInspect

=Item2
{InspectLevel1.Item2<2:
//Level 1 computer terminal
{Stevie} Huh. A computer terminal. Wonder if this has what I need.
{Stevie} Andddd guess not. Lights are on, but nobody is home. 
- else:
{Stevie} Yep, still nothing. I'll have to try elsewhere.
}
->endInspect

=Item3
{InspectLevel1.Item3<2:
//level one admiring the engineering
{Stevie} How old is this hulk anyway?

{Max}  Huh, I don’t actually know. Pre-war maybe, but not by much. They were built to analyse the initial Rift anomalies.

*{Stevie} Yeah, I remember.
*{Stevie} So at least a decade or two. 

-{Max} Yep, this is some vintage tech. Can’t believe it’s been that long already.

*{Stevie} We’re getting old.
*{Stevie} Sometimes it feels like a million years ago.
    {Stevie} We were different people back then. The world was different.

-{Max} Tell me about it.

*{Stevie} Hey. Remember that story Dr. L used to tell in the barracks?

-{Max} Uhh...

*{Stevie} You know. About what happens when you mix water and rift radiation.

-{Max} Those were just stories, Stevie. 

*{Stevie} Maybe.

-{Stevie} But so were trans-dimensional rifts once upon a time, and just look at us now. Besides, who wouldn't want to live forever?

-{Max} …

*{Stevie} Max? Still there?

-{Max} Yes, I’m still here.Just thinking.

*{Stevie} About what?

-{Max} I don't know. I guess... eternity. Yeah, I'm thinking about that.

*{Stevie} Well don't hurt yourself.
{Max} Har har, very funny. 

*{Stevie} Sorry, I forgot.
{Stevie} Dr. L was there wasn't she, on the last day, on the beach.
{Max} Yeah, she was. But it's okay. I'm okay.

**{Stevie} Honest?
{Max} They're just memories. They can't hurt me now.

- else:
{Stevie} Look at that machinery. These Research Stations really were built to withstand the apocalypse.
{Stevie} And I guess they did, in a way.
}
->endInspect

=Item4
{InspectLevel1.Item4<2:
//level one admiring the engineering
//Bit of wall with tubes?
{Max} See anything interesting down there?

*{Stevie} Not especially. 
*{Stevie} So much.

{Stevie} Over here are some tubes. And over there, yep, some more tubes. 
{Stevie} Why, are you getting bored?

-{Max} No way, I’ve got my feet up on the dash, catching up on some light reading of um, oceanic current patterns? It’s riveting. I feel like I’m on a mini-holiday.

*{Stevie} Oooh, oceanic currents. Saucy.
*{Stevie} Save me a seat would you.

-{Max} If you want to join, you’d better hurry up. This storm is starting to theaten my dreams of suntanning on to of the plane.

*{Stevie} Oh trust me, I can’t <i>wait</i> to get out of this suit. 
{Stevie} Who says we have to wear this things all the time anyway.

-{Max} It’s in the manual!

*{Stevie} Manual schmanual. 
 {Stevie} I’m a glorified handyman, not a deep sea diver.

*{Stevie} I know, I know!
{Stevie} Rule 1: Thou shalt not enter a facility without the appropriate equipment. 
{Stevie} Rule 2: Thou shalt not proceed below the upper levels of the facility without express permission from Head Office. 
{Stevie} Rule 3: Thou shalt not--

-{Max} --Stevie.

*{Stevie} Yes?

{Max} Shut up and get back to work would you, I’m trying to read important literature over here.

{Stevie} Rude.
- else:
{Stevie} Yep, the tubes are still there. 
*{Stevie} Still tubular. 
**{Stevie} So very, very tubey.
***{Stevie} Man, I've clearly been breathing pure oxygen for too long. 
}
->endInspect



==InspectLevel2
{Stevie} Hmmm...
->endInspect
=Item1
//computer terminal
{InspectLevel2.Item1<2:
{Stevie} Alright, I’m here. What next?

{Max} Type in your user code, and then I should be able to access the system remotely from up here.

*{Stevie} Okayyy, done!
*{Stevie} Easy enough.

-{Max} Why hello there my little beauty.

*{Stevie} Stop sweet-talking the machinery.

-{Max} But the machinery responds better when I sweet-talk it, every maintenance technician worth their salt knows that.

*{Stevie} Is that right?
{Max} They’re just like you, Tough on the outside, but--
**{Stevie} Yeah yeah, Casanova, we get the picture.
*{Stevie} I manage just fine without it.
{Max} That’s because you always have me by your side. I smooth out your rough edges.
**{Stevie} You wish.

-*{Stevie} So tell me, what sweet nothings is the system whispering to you now.

-{Max} Right now...It’s less a whisper, and more the screams of a depressurising rift-breach. Seriously, ouch. This data is all over the place.

*{Stevie} The other Stations in the area are still giving the all clear?

-{Max} Yep, systems nominal - rift radiation in acceptable levels, and even if there was, you’re wearing your adaptive suit. It’s probably just an instrument malfunction. A ghost in the proverbial machine.

*{Stevie} Weird though. We’ve been working this route for what, three years now and it’s the first time anything like this has happened. 
*{Stevie} The readings are never wrong. Something has to have set it off. 

-{Max} *static*... Stevie.

*{Stevie} Yeah?

-{Max} Everything is fine. It’s just a computer glitch, I’ll hard reset this baby, and we’ll be back at the Hub in time for dinner, you’ll see.

*{Stevie} Is that a promise?
*{Stevie} Care to wager on that?

-{Max} Just try and stop me. Lukewarm soup rations, and rehydrated crackers here I come.

*{Stevie} You’re an idiot.
*{Stevie} You’re incorrigible.

-{Max} Damn right I am. That’s why you keep me around.

- else:
{Stevie} I've already patched Max in to the network, nothing left to do here.
}


=Item3
//Bit of wall with tubes?
{InspectLevel2.Item2<2:
{Max} See anything interesting down there?

8{Stevie} Not especially. Just tubes. And over there, yep, some more tubes. You getting bored up there?

{Max} No way, I’ve got my feet up on the dash, catching up on some light reading of um, oceanic current patterns. It’s riveting. I feel like I’m on a mini-holiday.

*{Stevie} Oooh, oceanic currents. Saucy.
*{Stevie} Save me a seat would you.

{Max} If you want to join, you’d better hurry up. 

*{Stevie} Oh trust me, I can’t <i>wait</i> to get out of this suit. Who says we have to wear this things all the time anyway.

{Max} It’s in the manual!

*{Stevie} Manual schmanual. 
 {Stevie} I’m a glorified handyman, not a deep sea diver.
*{Stevie} I know, I know!
{Stevie} Rule #1 -Thou shalt not enter a facility without the appropriate equipment. 
{Stevie} Rule #2 - Thou shalt not proceed below the upper levels of the facility without express permission from Head Office. 
{Stevie} Rule #3--!

{Max} Stevie.

{Stevie} Yes, Max?

{Max} Shut up and get back to work would you, I’m trying to read over here.

{Stevie} Rude.
- else:
{Stevie} Yep, the tubes are still there. Still tubular. 
}
->endInspect
