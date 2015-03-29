# APIs for vrim components

## Big Pieces

### Text.cs
Represents each open buffer, provides a basic API for text access and
manipulation (insert, remove, move) via the **point** (the cursor). All
access/manipulation methods on Text access text at the point. Text also handles
regex search

### Vrim.cs
Not sure what this should actually do besides be an entrypoint to the editor
program, maybe capture keystrokes for Frontend.cs to map to Text functions?

### Frontend.cs
Defines the mapping between keys and basic editor functions (move point, access
text, edit text, etc). The actual keymap should be defined in a modular and
easily configurable way, perhaps in a seprate config file? 
