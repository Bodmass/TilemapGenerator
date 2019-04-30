Tilemap Generator
Version 0.7
Created by Aziz Arar
---------------------
Using Unity 2018.2.15f1 (64bit)

The Demo includes the 2d extras package from Unity found at https://github.com/Unity-Technologies/2d-extras using the 2018.2 branch
The purpose of 2d extras includes extra tiles such as animated, random and rule tiles, this is created by Unity themselves; however is not required for the generation to function.
Any Tile as part of the Tilemap system will plug in and work correctly.

This project will not work on any version of Unity below 2017.2.

0.7 Patch Notes:


Additions:	
	-Added Reset Button
	-Added Stone Generation with Height Option

Changes:
	-Updated 2D Platformer Generation Tiles
	-Moved Foliage Density to Simple Options

Fixes:
	-Tilemap Perspective Label "2D Tilemap" changed to "2D Topdown"
	-Tilemap Perspective Label "2D Platformer" changed to "2D Sidescroller"

Removals:
	-Isometric Selection (To be reintroduced, Project requires Unity Version Upgrade)
	-Town Selection (To be reintroduced)
	
----------------------------------------------------

Known Issues:
	-Water Tiles occasionaly rotate when Regenerating through the Editor
	-On topdown, when the grid width and height are not equal in size, the tile placement does not work correctly.




----------------------------------------------------

--Previous Patch Notes--
0.6

Additions:
	-Added 2D Platformer Integration to Generation Window
	-Added Foliage option to 2D Platformer
	
	-Added Foliage Density Option in Advanced Options

Changes:
	-Grid Size on 2D Topdown now ensures X and Y are the same due to bug

0.5

Additions:
	-Added Foliage to 2D Topdown : World Map (Demo Includes Random Tile)
	-Added Walls Generation to 2D Topdown : Dungeon
	-Added Grid Cap removal in Advanced Settings (*)

Changes:
	-All Tilemaps Generated now default at 60fps

0.4

Additions:
	-Added Collision Generation (Water and Dungeon BG)
	
Fixes:
	

0.3

	-Added BSP