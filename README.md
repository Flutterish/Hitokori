# Hitokori
Hitokori ( fire and ice ) is a custom [osu!](https://github.com/ppy/osu) ruleset based on `A Dance of Fire and Ice`. Click and hold to the rythm to watch the dance unfold!

# Features
Hitokori offers a variety of mods to spice up gameplay. Most of them are the same ones you'd exect from any other game mode, the unique ones are:
* `Stretched` - Tiles have bigger angles in between, makes the game look faster
* `Squashed` - Tiles have smaller angles in between, makes the game look slower
* `Double Tiles` - A tile is generated in between all tiles
* `Hold Tiles` - Adds tiles you need to hold
* `Spin tiles` - Adds tiles that you have to press multiple times at the same speed
* `Reverse Spin` - Makes the whole playfiels spin
* `Triplets` - Play with 3 orbitals ( removing hold tiles is recommended )

# Demonstration video ( 0.9 )
[Watch it on youtube!](https://www.youtube.com/watch?v=CD8K3mGTlO4&feature=youtu.be)

# Installing the gamemode
Open up osu!lazer and go into settings. On the very top, there is a big blue button with "Open osu! folder". In that folder, there is another one called "rulesets". Go into Hitokori releases, download the prebuilt `.dll` file and paste it there. Restart osu!lazer and you're done!

Note that the ruleset does not automatically update with osu!, you need to overwrite the `.dll` file each time you want to update Hitokori.

# Hitokori is still WIP!
The first build was made in 2 weeks, including me learning how to tame the osu! engine. While I do consider it pretty robust, expect a few bugs. For known bugs refer to the issues section.

# Contributing
All contributions are welcome! Good ones, that is. If you have any ideas how to improve the gamemode or a bug report, don't hesitate and fill out an issue. 

If you want to contribute code, do it over a pull request. Make sure to discuss your new ideas over on the issues section to make sure they are actually wanted first. And make sure they work. Thats a pretty important one.

# Setup
If you want to contribute code, you first have to set up your project:
* git clone this repo
* git clone osu!lazer master branch
* open osu master in your preferred IDE
* add this repo as a linked project
* add a project reference to this in osu.Desktop
* replace the osu.Game project reference of this project to your local osu.Game
* set osu.Desktop as the startup project
