//Some default functions for the InkWriter

VAR debug = true
VAR sayStarted = false

VAR died = false

VAR checkItem = -1

LIST characters = Sadie, George, Ashley, Peter, Egroeg

LIST items = item_battery, item_keycard, item_walkietalkie, item_crowbar, item_smalloxygen, item_knife, item_tape, item_note, item_diode, item_wrench, item_key, item_usb, item_none

VAR debugInventory = item_walkietalkie

VAR tunerID = ""

VAR Stevie = "<color=grey>Stevie:</color>"
VAR Max = "<color=yellow>Max:</color>"
VAR Voice = "<color=purple>Voice:</color>"

EXTERNAL CheckHasItem(x,y)
EXTERNAL ConsumeItem(x,y)
EXTERNAL AddItem(x,y)
EXTERNAL VoiceClip(x)

==function Consume(item, amount)==
{CheckItem(item, amount)>=amount:
{ConsumeItem(ConvertToString(item), amount)}
- else:
[Cannot consume item - not enough in Player inventory!]
}
===function Add(item, amount)===
{AddItem(ConvertToString(item), amount)}
==function AddItem(item, amount)===
~debugInventory += item
==function CheckItem(item, amount)==
// Helper function
{CheckHasItem(ConvertToString(item), "checkItem")}
~return checkItem

===function ConsumeItem(itemName, amount)===
(Attempted consumption of {amount} {itemName})

===function CheckHasItem(itemName, returnVar)===
{debugInventory?ConvertToItem(itemName):
~checkItem = 1
- else:
~checkItem = 0
}

===function VoiceClip(id)===
[Play Voice clip {id}]

===function ConvertToString(targetItem)===
// Add more items to this list as needed
~temp returnVar = item_none
{targetItem:
- item_battery:
~returnVar = "item_battery"
- item_keycard:
~returnVar = "item_keycard"
- item_walkietalkie:
~returnVar = "item_walkietalkie"
- item_crowbar:
~returnVar = "item_crowbar"
- item_tape:
~returnVar = "item_tape"
- item_knife:
~returnVar = "item_knife"
- item_smalloxygen:
~returnVar = "item_smalloxygen"
- item_note:
~returnVar = "item_note"
- item_diode:
~returnVar = "item_diode"
- item_wrench:
~returnVar = "item_wrench"
- item_key:
~returnVar = "item_key"
- item_usb:
~returnVar = "item_usb"
}
// and return
~return returnVar

==function ConvertToItem(targetItemString)===
~temp returnVar = item_note
{targetItemString:
- "item_battery":
~returnVar = item_battery
- "item_keycard":
~returnVar = item_keycard
- "item_walkietalkie":
~returnVar = item_walkietalkie
- "item_crowbar":
~returnVar = item_crowbar
- "item_tape":
~returnVar = item_tape
- "item_knife":
~returnVar = item_knife
- "item_smalloxygen":
~returnVar = item_smalloxygen
- "item_note":
~returnVar = item_note
- "item_diode":
~returnVar = item_diode
- "item_wrench":
~returnVar = item_wrench
- "item_key":
~returnVar = item_key
- "item_usb":
~returnVar = item_usb
}
// and return
~return returnVar

===function UseButton(buttonName)===
<>{not debug:
\[useButton.{buttonName}]
}
===function DisableButton()===
<>{not debug:
\[disable\]
}
===function UseText(textName)===
<>{not debug:
\[useText.{textName}]
}

===function ReqS(currentAmount, requiredAmount, customString)===
// used to enable/disable options [{Req(stuffYouNeed, 10, "Stuffs")}!]
{currentAmount>=requiredAmount:<color=green>|<color=red>}
<>{not debug:
\[{currentAmount}/{requiredAmount}\ {customString}]</color>
- else:
({currentAmount}/{requiredAmount} {customString})</color>
}

// convenience function that assumes min 0 and max 1000 on any value
===function alter(ref value, change)===

// if you need to alter values of things outside of checks, use this instead of directly changing them
// use (variable, change (can be negative), minimum (0) maximum (1000...or more).
{alterValue(value, change, -10000, 10000, value)}

===function alterValue(ref value, newvalue, min, max, ref valueN) ===
~temp finalValue = value + newvalue
~temp change = newvalue
{finalValue > max:
{value !=max: 
    ~change = finalValue-max
- else:
    ~change = 0
}
    ~value = max
- else: 
    {finalValue < min:
    ~change = -value
    ~value = min
- else:
    ~value = value + newvalue
    }
}
~temp changePositive = change*-1
{change!=0:
#autoContinue
{change > 0:
        <i><color=yellow>Gained {print_num(change)} {print_var(valueN, change, false)}.</color></i>
    -else:
        <i><color=yellow>Lost {print_num(changePositive)} {print_var(valueN, change, false)}.</color></i>
}
}

// prints a var, capital, non capital, plural or singular
==function print_var(ref varN, amount, capital)==
{amount<0:
// Make amount always positive, in case it's a negative amount.
~amount = amount * -1
}
{varN:
-"AnyString":
{amount==1:
    {capital:
    ~return "Anystring"
    - else:
    ~return "anystring"
    }
- else:
    {capital:
    ~return "Anystrings"
    - else:
    ~return "anystrings"
    }
}
}

===function StartSay()===
#startSay
~sayStarted = true
<i></i>

===function EndSay()===
#endSay #changeportrait
~sayStarted = false
<b></b>

==Say(text, character)==
// ->Say("Text", character)->
{not sayStarted: {StartSay()}}
#changeportrait

//{character:
//- Valfrig:
//#spawn.portrait.valfrig
//}
{character!="": {character}: {text}|<i>{text}</i>}
{EndSay()}
->->

// prints a number as pretty text
=== function print_num(x) ===
// print_num(45) -> forty-five
{ 
    - x >= 1000:
        {print_num(x / 1000)} thousand { x mod 1000 > 0:{print_num(x mod 1000)}}
    - x >= 100:
        {print_num(x / 100)} hundred { x mod 100 > 0:and {print_num(x mod 100)}}
    - x == 0:
        zero
    - else:
        { x >= 20:
            { x / 10:
                - 2: twenty
                - 3: thirty
                - 4: forty
                - 5: fifty
                - 6: sixty
                - 7: seventy
                - 8: eighty
                - 9: ninety
            }
            { x mod 10 > 0:<>-<>}
        }
        { x < 10 || x > 20:
            { x mod 10:
                - 1: one
                - 2: two
                - 3: three
                - 4: four        
                - 5: five
                - 6: six
                - 7: seven
                - 8: eight
                - 9: nine
            }
        - else:     
            { x:
                - 10: ten
                - 11: eleven       
                - 12: twelve
                - 13: thirteen
                - 14: fourteen
                - 15: fifteen
                - 16: sixteen      
                - 17: seventeen
                - 18: eighteen
                - 19: nineteen
            }
        }
}
// prints a number as pretty text but with a capital first letter
=== function print_Num(x) ===
// print_num(45) -> forty-five
{ 
    - x >= 1000:
        {print_num(x / 1000)} thousand { x mod 1000 > 0:{print_num(x mod 1000)}}
    - x >= 100:
        {print_num(x / 100)} hundred { x mod 100 > 0:and {print_num(x mod 100)}}
    - x == 0:
        zero
    - else:
        { x >= 20:
            { x / 10:
                - 2: Twenty
                - 3: Thirty
                - 4: Forty
                - 5: Fifty
                - 6: Sixty
                - 7: Seventy
                - 8: Eighty
                - 9: Ninety
            }
            { x mod 10 > 0:<>-<>}
        }
        { x < 10 || x > 20:
            { x mod 10:
                - 1: One
                - 2: Two
                - 3: Three
                - 4: Four        
                - 5: Five
                - 6: Six
                - 7: Seven
                - 8: Eight
                - 9: Nine
            }
        - else:     
            { x:
                - 10: Ten
                - 11: Eleven       
                - 12: Twelve
                - 13: Thirteen
                - 14: Fourteen
                - 15: Fifteen
                - 16: Sixteen      
                - 17: Seventeen
                - 18: Eighteen
                - 19: Nineteen
            }
        }
}