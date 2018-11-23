# GameOff2018_MKS_Entry
This is the Game Off 2018 Entry from Micah Koleoso Software
Running from the 1st November to 1st December, 2018.

The entry consists of a client (Unity: GO2018_MKS_Client) and server (C# .NET: GO2018_MKS_Server) application.

TODO: Version Info

TODO: Introduce game play

Credits
=======
Original mouse and keyboard icon - http://chittagongit.com/icon/mouse-and-keyboard-icon-12.html

TODO: Write documentation to handle custom server install, setup and configuration

Open Game Features
=======================
- Enable drone upgrade path (food, tech):    
    * 4 Upgrade optional/choice levels 
    * Food upgrade costs: 1000, 1500, 2000, 2500
    * Tech upgrade costs: 1125, 1125, 1125, 1125
    A - Faster drone speed:                 
    A - Reduce unit food usage:             
    B - More food collection max volume:    
    B - More tech collection max volume:    
    C - Barricade build and breaker:        
    C - Barricade build and breaker:        
    D - Opponent unit food resource stealer:   
    D - Opponent unit tech resource stealer:   
- Breed new drones, max. 8. Keep upgrades for old and new drones
- Death of Breeder causes game loss
- Make 3 maps (48x48) initially for jam release
- Configuration Options:
    Fog of War [on/off]
    Timed game session [on/off] 2:50, 5:00, 7:50, 10:00 minutes per Session
    Music [on/off/Volume]
    Sound FX [on/off/Volume]
- Replace placeholder art!
- End round when timer runs out
- Actions (Hold Key to enable mode actioning on next click):
    *ESC - (toggle) Show/hide menu ingame
    *S - (press) Stop navigation immediately 
    *P - (hold) Build barricade mode [Breeder Resource Cost per Barricade placed: F: 250 T: 250]
    *B - (hold) Destroy barricade mode
    *F - (hold) Feed the Breeder Food from all nearby drones 
    *T - (hold) Feed the Breeder Tech from all nearby drones
    *D - (hold) Drone spawn mode [Breeder Resource Cost per Barricade placed: F: 750 T: 500]

Development TODOs
=================
- *Implement synched time coutdown from server
- *Implement session end on timeout
- *Implement session win/loss display with return to create or join screen as last chosen
- Implement network player navigation and actions cross communication during session
    *Unit navigation commands
    *Unit Food/Tech level changes
    *Unit drone spawns (only near Breeder)
    *Resource food/tech level changes
    *Barricade spawns
    *Barricade level changes
    *Calculate server-side scores and transmit to both clients with update message.
- *Breeder and Drone food usage 
- *Test food and tech feeding
- *Game session end on breeder death by hunger
- Make breeder and drone animation for idle, walk
- *Make title image for "Scamper Fields"
- Make delays at session ingame end and while transition to ending
- *Show end scores in win/loss screen 
- Music and SFX
- Redo maps terrain and placements
- Make documentation, readme, credits for external assets
- *Indicate mouse and keyboard required
- Particles for most acitvities
- *Adjust UI for screen safe zones
- *Add map display of units, resources and barricades
