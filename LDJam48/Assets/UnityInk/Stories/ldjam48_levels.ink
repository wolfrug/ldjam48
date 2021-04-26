// For level inspect stuff

->endInspect

==endInspect
->DONE

//// THIS IS THE TEMPLATE START - NOTE THIS IS LEVEL0, WE DON'T NEED IT
// adding generic item descriptions below so we have the right descriptive cues on standby.
==Inspect
->endInspect
=Item1 // Computer on
{Item1<2:
{Stevie} A computer terminal, I should probably take a look.
- else:
{Stevie} This is the computer for this level of the station.
}
->endInspect

=Item2 // Computer off
{Item2<2:
{Stevie} A computer terminal, but it looks like the screen is dark. Probably non-functional.
- else:
{Stevie} Better to try my luck elsewhere.
}
->endInspect

=Item3 //Desk
{Item3<2:
{Stevie} Hmm, an old desk. But it looks like it's been cleared out.
- else:
{Stevie} Anything of interest was removed long ago.
}
->endInspect

=Item4 //desk paper
{Item4<2:
{Stevie} Some paper. Could just be scrap, or it could be important.
- else:
{Stevie} Discarded paper.
}
->endInspect

=Item5 //Desk tablet
{Item5<2:
{Stevie} An old tablet. There might be something useful on there, I should take a closer look.
- else:
{Stevie} An old tablet.
}
->endInspect

=Item6 // dubious science thing
{Item6<2:
{Stevie} A leftover device from the lab, looks like it still has a few sample vials in it.
- else:
{Stevie} A old piece of lab equiptment.
}
->endInspect
=Item7 // dubious science thing 2
{Item7<2:
{Stevie} Seems to be some sort of storage tray, or maybe biometric device. 
- else:
{Stevie} An old device, left over from the lab.
}
->endInspect
=Item8 //Fungus
{Item8<2:
{Stevie} What the... are those mushrooms? Growing on the walls. Damn, hopefully there aren't any spores. There's no telling what kind of fungus might be growing down here.
- else:
{Stevie} Fungal mould on the walls, probably best to steer clear of it.
}
->endInspect
=Item9 //spigot
{Item9<2:
{Stevie} Wonder what this spigot was here for. I could try turning it...but no, that could end badly.
- else:
{Stevie} Still kinda want to turn it.
}
->endInspect
=Item10 //wall doc
{Item10<2:
{Stevie} A notice of some kind. What does it say?
- else:
{Stevie} Already seen it.
}
->endInspect
=Item11 //wall junk
{Item11<2:
{Stevie} Strange. Someone pinned up a bunch of paper scraps and pictures. Are they just haphazard desk notes, or something more?
- else:
{Stevie} Notes on the wall.
}
->endInspect
=Item12 // Poster
{Item12<2:
{Stevie} An anotomical poster showing some strange aquatic beast. 
- else:
{Stevie} An old poster.
}
->endInspect
=Item13 //jellyfish
{Item13<2:
{Stevie} Ooh, a jellyfish. It looks so pretty in the dark water. Wouldn't want to go swimming with one though.
- else:
{Stevie} A jellyfish, glowing in the dark.
}
->endInspect
=Item14 //Coelacanth
{Item14<2:
{Stevie} A Coelacanth! Wow, we must be a lot deeper than I realised.
- else:
{Stevie} A Coelacanth, drifting outside the window.
}
->endInspect
=Item15 //fish
{Item15<2:
{Stevie} Come here pretty little fish, and say hello to Aunty Stevie.
- else:
{Stevie} The fish isn't in a conversational mood.
}
->endInspect
=Item16 //flesh flower
{Item16<2:
{Stevie} Oh look, a coral grow---no, are those bones? Shit. It's a ribcage fused with some sort of fleshy flower formation. How did it even get out there? Where did it come from? Who... did it come from?
- else:
{Stevie} I'd rather not inspect that too closely.
}
->endInspect
=Item17 //mysterious shape
{Item17<2:
{Stevie} Is that something drifting out there in the water? A fish? Maybe seaweed? Or...no, let's just stick with the seaweed. 
- else:
{Stevie} It's hard to make it out in the murky water, but it's probably nothing.
}
->endInspect
=Item18 //nuclear whale
{Item18<2:
{Stevie} Is that...? Could that really be...?
- else:
{Stevie} It looks almost close enough to touch.
}
->endInspect

=Item19 //moon pool
{Item19<2:
{Stevie} Woah, a moonpool. It's kind of terrifying to think about the fact that the entire ocean is waiting right there through that little opening. Like...right there. All you'd have to do is slip into the water, and you'd be outside.
- else:
{Stevie} Don't think about dark waters rising up to crush you. It's fine. It's just a regular pool opening out onto the bottom of the ocean.
}
->endInspect
=Item20 //the window
{Item20<2:
{Stevie} All this glass, it seems so strange and fragile. One little breach and his whole Station would crumble like a cheap tin can.
- else:
{Stevie} If you stare at the window too long, it feels a bit like falling into a vortex. The ocean beyond seems endless.
}
->endInspect

=Item21 //the emergency lights
{Item21<2:
{Stevie} Huh. Emergency lights only.
- else:
{Stevie} Yep. Still no lights.
}
->endInspect

=Item22 //the wall
{Item22<2:
{Stevie} Hope this old thing holds up.
- else:
{Stevie} It's creaking mightily...
}
->endInspect
/// THIS IS THE TEMPLATE END


/*==InspectLevel1
->endInspect
=Item1
//computer terminal
{InspectLevel1.Item1<2:
{Stevie} Huh. A computer terminal, but it looks like a basic model. Probably not what we need to gain root access to the system.
- else:
{Stevie} I should probably try a different terminal.
}
->endInspect
->endInspect

=Item2
{InspectLevel1.Item2<2:
//a spigot
{Stevie} Looks like some kind of pressure valve, I wonder what it was used for.
- else:
{Stevie} Just an old valve.
}
->endInspect

=Item3
{InspectLevel1.Item3<2:
//section of wall
{Stevie} Look at the size of those supports, this place is built like a bunker. Which makes sense I guess. Good for keeping enemies out...and keeping secrets inside.
- else:
{Stevie} Wouldn't want to get trapped in one of these Stations.
}
->endInspect

=Item4
{InspectLevel1.Item4<2:
//A notice on the wall
{Stevie} Huh, looks like an old safety notice. I wonder what they were working on here before the Station was decomissioned. Then again, maybe it's better not knowing.
- else:
{Stevie} Standard war safety protocols, nothing new to see here.
}
->endInspect


==InspectLevel5
{Stevie} Hmmm...
->endInspect

=Item1
{Item1<2:
//computer terminal 
{Stevie} This is probably the access terminal Max mentioned. I should take a closer look and see if I can establish a connection to the Hub console in the seaplane up top.
- else:
{Stevie} This terminal provides a direct link to the Station systems.
}
->endInspect

=Item2
//A poster of a whale
{Item2<2:
{Stevie} An anatomical post of a...whale? Doesn't look like the whales in the history books, that's for sure. But what do I know.
- else:
{Stevie} Yep, seen that already. Still doesn't look right.
}
->endInspect


=Item3
//A section of wall/ceiling with cables or tubes
{Item3<2:
{Stevie} So many tubes and cables up there. The processing power of this place must have been off the charts. It's almost a pity they shut it all done. It's just a glorified weather station now.
- else:
{Stevie} This place must have been pretty impressive back in the day.
}
->endInspect
*/


