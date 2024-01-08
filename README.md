# EXTRACT/INSERT MMBNLC DAT FILES

A command-line tool for extracting and reinserting files from [_Mega Man Battle Network Legacy Collection Vol. 1_](https://store.steampowered.com/app/1798010/Mega_Man_Battle_Network_Legacy_Collection_Vol_1/)'s and [_Mega Man Battle Network Legacy Collection Vol. 2_](https://store.steampowered.com/app/1798020/Mega_Man_Battle_Network_Legacy_Collection_Vol_2/)'s .dat files.
Based on the work of Prof9, this tool also allows you to reinsert

Building
========

In console, just type `dotnet build` (Powershell, VSC...)

Usage
=====

In console, type :

`ReplaceMMBNLCPictures.exe filename.dat` for extract (A folder will be created)

`ReplaceMMBNLCPictures.exe filename` for insert (The files in the "filename" folder will be inserted into "filename.dat" file, filename.dat required)

Example :

Copy&paste the vol1.dat file from `\steamapps\common\MegaMan_BattleNetwork_LegacyCollection_Vol1\launcher\data` into app folder

Execute `ReplaceMMBNLCPictures.exe vol1.dat` in the console will extract pictures from the Legacy Collection into the "vol1" folder

Notes
=======

vol1.dat/vol2.dat contains pictures from the Legacy Collection menus and some in-game images (like New game or Continue) 

f.dat contains font files for the Legacy Collection (NOT THE GAMES THEMSELVES)

v1.dat/v2.dat contains background videos

License
=======

[GPLv3](https://www.gnu.org/licenses/gpl-3.0.html) or later.

Thanks
======

* Prof9 for his python code used to extract these files
