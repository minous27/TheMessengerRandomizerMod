# TheMessengerRandomizerMod
---
## The beginning of rando for The Messenger! - Minous27

**This is the Work-in-Progress project for The Messenger Randomizer. Initial version of the mod currently in development.**

---
### Installation Instructions
---

Early builds of the randomizer mod will be fairly involved in their installation procedures. These will be improved with development of the mod.

#### Install Courier 

The Messenger Randomizer relies on the Courier Mod Loader to be properly loaded and handled by the game. If you have not already installed Courier, please follow the instructions to install it first then come back here to continue with the randomizer installation. Don't worry, I'll wait. :D

**_PLEASE NOTE THAT THE RANDOMIZER MAY ONLY WORK WITH CERTAIN VERSIONS OF COURIER DURING THE COURSE OF BOTH APPLICATIONS' DEVELOPMENTS. THE INITIAL VERSION OF THE MOD WAS BUILT ON COURIER 'v0.5-alpha' BUT MAKE SURE YOU CONFIRM THE COURIER VERSION FOR THE RANDOMIZER VERSION YOU ARE INSTALLING(SHOULD BE IN THE RELEASE NOTES)_**

[Courier Mod Loader](https://github.com/Brokemia/Courier#installation-instructions) 

#### Install Randomizer Mod

With Courier installed, it is time to install the randomizer mod. To install the mod:

1. Download the preferred version of the mod from the [Releases](https://github.com/minous27/TheMessengerRandomizerMod/releases) page.
    * You will need the zip file. Do not bother unzipping it, just download it. 
2. Once the file is downloaded, place the zip file directly into the _Mods_ directory for the game that the Courier installer created.
    * This location will vary based on where you have your game installed on your machine. For example, my destination was _'drive_name:\Steam\steamapps\common\The Messenger\Mods'_.

And the mod is now properly installed and ready to be loaded when you open up the game.

#### Setup Save File for Randomizer

The early versions of the randomizer mod are not setup to do really nice and fun things for you like properly prepare the game to be in the state we intend to start a randomizer seed in. Currently you start:

* In the Tower of Time HQ (right after Linear)
* You only have climbing claws
* Some of the initial Tower of Time HQ and map collection cutscenes have already been watched
* All of the portals are open

Until we get that working, you will need to set your game to be in this state to work well with the randomizer. If you are lazy like I am then I have a not-very-fun-but-still-more-fun-then-doing-it-yourself solution for you. Along with the zip file you grabbed for the randomizer mod there should also be a text file. This file is a properly configured game save file that contains three randomizer file slots ready to go for you. All you need to do is:

1. Download the 'RandomizerSaveGame.txt' file from the [Release](https://github.com/minous27/TheMessengerRandomizerMod/releases) you installed.
2. Place the file in save file location for the game
    * By default that is located at _'C:\Users\youruserhere\AppData\LocalLow\Sabotage Studio\The Messenger'_.
3. Backup your original 'SaveGame.txt' file.
4. Rename the randomizer save file to 'SaveGame.txt'.

You are all done! At this point you have the mod installed and the game in an apporiate state to begin running randomizer seeds. 

---
### Using the Randomizer
---

Now that you are all installed and ready to play, lets take a look some of the functionality the mod provides.

#### Generate a New Randomizer Seed

When you are ready to kick off a brand new seed: 

1. Start up the game in whatever way you normally do. 
2. Once you are on the Main Menu, select 'Options' -> 'Third Party Mod Options' -> 'Generate Random Seed'.
3. You will be presented with a screen asking for a save slot number.
    * Provide a number between 1-3 that corresponds to which randomizer save file you want to create a seed for.
4. Press your confirm button to continue
    * By default this is the 'Start' button on a controller or the 'Enter' key on keyboard.
5. If you are returned to the Third Party Content screen, then the seed generation completed successfully and you may now start the file of your choice and begin playing.

##### NOTE ABOUT EARLY RANDOMIZER SEEDS

Keep in mind that there is currently **NO LOGIC** in my seed generation code. This means that it is possible, and very likely, that a seed you generate will not be beatable and will cause a softlock at some point. If you are not really interested in messing around with likely unbeatable seeds then you can provide a pre-generated seed for your game to use. I...haven't added this into the game for you to do easily yet because I suck *BUT* I can show you how to do it behind the scenes.

#### Provide a Pre-Generated Seed

Courier provides the ability for mods like this one to store information in a save file to keep track of between play sessions so that you don't *HAVE* to finish it in single sitting. For the moment, we can use this functionality to provide pre-generated seed numbers to for the game to use so we can be certain what seed we will be working with. To do this:

1. Navigate to the same directory on your machine where the game's save files are located
    * As a refresher, the default location is _'C:\Users\youruserhere\AppData\LocalLow\Sabotage Studio\The Messenger'_.
2. Open the file 'ModSave.json' in whatever text editor you wish.
3. Provide the seed number in the 'minous27RandoSeeds' Mod Option
    * Depending on how familiar you are with JSON it may not be obvious how to do this and if this is your very first setup there may actually be nothing inside the 'Options' block. The value you will need to set is a pipe "|" deliminted string of numbers that represent seeds and the order they are in represent which file they are for.
    * If you have nothing in your 'Options', an easy way to initially fill it is to follow the above directions to generate a new seed and then back all the way out to the main menu to allow the game to save that data into the file for you. You can also type the string in yourself if you're feeling frisky.
    * Example mod option save string: 
    ```json 
    {"Options":[{"optionKey":"minous27RandoSeeds","optionValue":"|0|0|0"}]}
    ```
    * This example shows three '0's as seeds for all three file slots. You will replace that number with seed number you desire in the position that matches which file slot you intend to play on (so the first 0 represents the seed for the first save file).
4. Save the file when you are done.

Once these steps are complete if your game is already running you may have to restart it for these changes to take effect. You are now ready to select the applicable save file and begin playing some randomizer!

Here are some example seed(s) that have been tested to be beatable (i mostly promise :3)

* 39596

#### Checking The Game Logs

I have added quite a bit of logging into the mod to allow us to know things about the randomizer and the seed you are playing. For example, when you start the file and begin play or generate a new seed the spoiler log that shows what items are where gets printed to the games log. If you wish to view this log and see that information, you can find it in the game's install directory (the same place where the games .exe file is). The file is called 'log.txt'. 

As a reminder, my example install destination is _'drive_name:\Steam\steamapps\common\The Messenger\'_.
