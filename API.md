# APIs for vrim components

## Big Pieces

### Text.cs
Represents each open buffer, provides a basic API for text access and
manipulation (insert, remove, move) via the **point** (the cursor). All
access/manipulation methods on Text access text at the point. Text also handles
regex search. The actual API should be an answer to 'what do we want to do with
a buffer'. Currently it is - 
+ insert
+ regex search
+ write to file
+ display (returns "displayable" version of buffer contents, maybe should be
moved out)
+ point motion

### Vrim.cs
Not sure what this should actually do besides be an entrypoint to the editor
program, maybe capture keystrokes for Frontend.cs to map to Text functions?

### Frontend.cs
Defines the mapping between keys and basic editor functions (move point, access
text, edit text, etc). The actual keymap should be defined in a modular and
easily configurable way, perhaps in a seprate config file? 
