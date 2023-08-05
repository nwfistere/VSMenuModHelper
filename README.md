# VSMenuModHelper
A library for enabling modifying Vampire Survivors' menu

## Features
Create a new custom tab with custom:
 - icon
 - title
 - button
 - slider
 - tickbox
 - dropdown
 - multiple choice buttons

## Installation as dependency
This library is NOT a mod for Vampire Survivors. It's a library that mods can utilize. Installing only this to Vampire Survivors will do nothing.

### Requirements
 - New Engine version of Vampire Survivors
 - MelonLoader installed
 - A mod for Vampire Survivors that utilizes this library.

### Install
1. Download dll (link)
2. Install dll into Vampire Survivors' mods directory along with the Mod dependent on this library.


## Developers: How to consume
1. Download and Install library
2. Point Visual Studio at dll
3. Utilize library (see [example](https://github.com/nwfistere/VSMenuModHelper/tree/main/examples))


## Example Result
![Custom Tab in the options menu](https://github.com/nwfistere/VSMenuModHelper/assets/9168048/6b4cccaf-058a-4598-8791-c2a2d174f741)

## Known Bugs

 - Tabs are currently displayed while in game, not only on the options menu.

## TODO
 - Add option to only display in option menu or in game.
 - Add option for library to handle Harmony patches
 - Add option to alter existing tabs
 - Custom UI elements
