# Blume
A game engine written in C# that runs in the windows command line

Creates a 3 dimensional grid of 'Units', each of which contain a unicode character, color value, and a mutable list of flags.
Uses a library called 'Pastel' for rendering ARGB colors in the console.

This grid is passed into a 'use once' Frame object every render cycle for post processing for lighting effects, ect. , which
is then passed into the renderer object. The renderer object will first render the Frame top to bottom and caches the Frame,
and afterwards contrasts the previous frame with the new for changes, and re-renders only the changed units.

## Current State
*This section needs to be updated, see "whats new" section below*
The program, as downloaded right now, will load a 50x50, 2 layered plane, with a 'player' Unit, and two luminant Units. You can move around, place walls and lights. As of now, it is very inconsistent with lighting, but is fun to play with.

Known issues include:
* Barriers not blocking light when on the same x or y axis.
* Strange light path finding when a barrier is at the corner of a luminant Unit.

## Fun stuff
* In the class FrameTools in the Frames namespace, there is a function called ReadImageBlocks, with the function signature
```c#
Frame FrameTools.ReadImageBlocks(string path, int w, int h)
```
Which takes in a image file path, and writes corresponding color values to a 1 layer grid of the dimensions passed to the function,
returning a Frame object of said grid, which can then be passed to the renderer with fairly accurate and cool to look at results.

* WASD to move.

* Press o to place a light, and p to place a wall. Play with the very inconsistent lighting system. Renders fast, though.

## Lighting Effect *WIP*
* Lighting effect, where units which have the flag 'luminant' will light units around it, with the exponential formula 
![Lumocity Formula](https://imgur.com/UtBaDq5.png)<br/>
, where L is the lumocity value of the unit relative to the luminant unit, and D is the distance between the two units.

## Whats new
* Fully functional contrast rendering.
* Added keyboard input
* Optimized renderer
* Optimized image approximation
* Added automatic saving of approximated images using serialization
* Started beginings of mouse input
* Broke the clock class, whoop
* Added use of sprites, which are smaller Grid objects, optionally stored inside Unit objects, to be added to the Frame pre-render. A unit may have multiple sprites stored and hot-swappable.
