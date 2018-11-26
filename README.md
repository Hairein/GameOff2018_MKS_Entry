## GameOff2018 MKS Entry - "Scamper Fields"
This is the Game Off 2018 Entry from Micah Koleoso Software (http://www.micahkoleoso.de)
Running from the 1st November to 1st December, 2018.

The entry consists of a client (Unity: GO2018_MKS_Client) and server (C# .NET: GO2018_MKS_Server) application.
Game page: http://www.micahkoleoso.de/?page_id=724

#Version Info
============

v1.0preAlpha - Initial Jam release.


#Introduction To Gameplay
===================
GOAL
----
 Collect as much Food and Tech resources as possible before the timer runs 
 out, more than your opponent.
 Most actions are automatically initiated when the Breeder or Drone is  
 near enough to the target.
 Food is constantly required by all units to survive. The Breeder must not  
 die of hunger or the round will be lost immediately!

4 UPGRADE LEVELS
----------------
 A) Faster Drone speed or reduce unit Food usage [Cost: F:1000 T:1125]
 B) Increase max collection volume for Food or Tech [Cost: F:1200 T:1125]
 C) Barricade build or break capability [Cost: F:2000 T:1125]
 D) Absorb opponent Food or Tech when stationary [Cost: F:2500 T:1125]

MOUSE COMMANDS
--------------
 LEFT - Select Breeder or Drone. Click and drag to rectangle-select.
 In barricade placement mode, the barricade is built at click location if  
 resources are sufficient
 MIDDLE - Pan the map
 RIGHT - Set navigation target for selected units.

KEY COMMANDS
------------
ESC - (toggle) Show/hide menu ingame
S - (press) Stop unit navigation immediately 
P - (hold) Build barricade mode [Breeder Resource Cost per Barricade placed: F: 250 T: 250]
B - (hold) Destroy barricade mode
F - (hold) Feed the Breeder Food from all nearby drones 
T - (hold) Feed the Breeder Tech from all nearby drones
D - (hold) Drone spawn mode [Breeder Resource Cost per Drone placed: F: 750 T: 500]


#Credits
=======
Gfx And Textures:
Original mouse and keyboard icon - http://chittagongit.com/icon/mouse-and-keyboard-icon-12.html
Original terrain tiles from http://texturelib.com/

Music:
All music by Eric Matyas https://soundimage.org/2014/02/hello-world/ with special thanks!

Sfx:
Original SFX "Warrior Battle Chants and Shouts" Copyright 2012 Iwan Gabovitch [http://qubodup.net]
    from https://freesound.org/people/qubodup/sounds/160769/
370961__cabled-mess__click-03-minimal-ui-sounds.wav https://freesound.org/people/cabled_mess/sounds/370961/ by https://freesound.org/people/cabled_mess/
422836__gamedevc__g-ui-button-hover-1.wav https://freesound.org/people/GameDevC/sounds/422836/ by https://freesound.org/people/GameDevC/
448086__breviceps__normal-click.wav https://freesound.org/people/Breviceps/sounds/448086/ by https://freesound.org/people/Breviceps/
22952__acclivity__cheer.wav https://freesound.org/people/acclivity/sounds/22952/ by https://freesound.org/people/acclivity/
51269__rutgermuller__electric-noise-2.wav https://freesound.org/people/RutgerMuller/sounds/51269/ by https://freesound.org/people/RutgerMuller/

#Installation
============
TODO
    custom server install
    setup
    configuration


#Game Features And Infos
=======================
4 Upgrade optional/choice levels 
Food upgrade costs: 1000, 1500, 2000, 2500
Tech upgrade costs: 1125, 1125, 1125, 1125
    A - Faster drone speed                
    A - Reduce unit food usage             
    B - More food collection max volume    
    B - More tech collection max volume    
    C - Barricade build and breaker        
    C - Barricade build and breaker        
    D - Opponent unit food resource stealer   
    D - Opponent unit tech resource stealer   
- Breed new drones, max. 8
- Configuration Options:
    Timed game session [on/off] 2:50, 5:00, 7:50, 10:00 minutes per Session
    Music [on/off/Volume]
    Sound FX [on/off/Volume]
- 3 maps (Morpholite, Sunset and Overlord)


#Development MAYBES and TODOs (Jam)
=======================
- Make breeder and drone animation for idle, walk
- Music and SFX
- Redo maps terrain and placements
- Make documentation, readme, credits for external assets
- Particles for most acitvities


#Bugs/Known Issues
=================
- Clicking on the map above terrain sets the navigation target to the terrain point if underneath
- Barricades can be placed anywhere on terrain and elements, restrict to floor
- Spawning drones can be placed anywhere on terrain and elements, restrict to floor
