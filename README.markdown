## FFXIV Model Viewer

---

FFXIV Model Viewer is a utility for viewing the models and other files used by Square Enix's MMORPG, Final Fantasy XIV.

*This software DOES NOT modify the game or the data used by the game in any way.*

### Installation

Before installing the FFXIV Model Viewer you must have the following installed:  

* [Microsoft .NET 3.5 SP1](http://www.microsoft.com/downloads/en/details.aspx?FamilyId=AB99342F-5D1A-413D-8319-81DA479AB0D7&displaylang=en "Microsoft .NET 3.5 SP1")
* [SlimDX (June 2010) End User Runtime](http://slimdx.org/download_june10.php "SlimDX (June 2010)")

Once the prerequisites are installed simply run the installer and choose a destination folder.

If you have FFXIV installed the program should detect its location via a registry key.  Additionally, the program will initially cache data used for rendering NPCs/monsters which may take a little bit of time.  This process will be repeated whenever an updated version of FFXIV is detected.

### Using the Model Viewer

Left panel:  
Shows contents of files opened view File > Open.  Most files in the FFXIV data and client folders can be viewed and some allow additional functionality:

       Shader/File - View disassembled shader (DirectX shader assembly)
              GTEX - View texture
              MDLC - View model (untextured)
      Vorbis/ADPCM - Play Sound
    skl (Skeleton) - Visualize skeleton (static or animated)
        Lua Script - Save script

Center panel:  
Allows you to search for NPCs and monsters and, ideally, display them as they appear in-game.

Right panel:  
Displays list of models located in the /client/chara folders.  mon = monsters, bgobj = background objects, pc = players/armor, wep = weapons  
Not all folders are able to be viewed using this method.

View > Cartographer:
Allows you to view and save images of the maps used in game

### Controls

\* Not all controls are available in all viewers

      Click+Drag - Rotate model
     Mouse wheel - Resize model
               W - Camera forward
               A - Camera strafe left
               S - Camera backwards
               D - Camera strafe right
         Page Up - Camera move up
       Page Down - Camera move down
               R - Reset camera position to default
             1-6 - Switch texture for a given model part
      Shift+ 1-6 - Toggle visibility of a given model part
               X - Toggle skeleton visualization
               P - Toggle rendering of PGRPs
           Space - Pause animation
            Left - Reverse animation by one step
           Right - Move animation forward one step
       Alt+Enter - Toggle fullscreen mode
    Print Screen - Save a screenshot (Screenshots are saved to your 'My Pictures' folder)

### Version History

__v1.0.0__ - Initial Release

### License

FFXIV Model Viewer is licensed under the MIT license, the text of which is located within the LICENSE file that should be included with this source distribution.