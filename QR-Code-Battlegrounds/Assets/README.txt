In case something is not functioning correctly this document will explain how each sence is setup.

Steps to load a scene
  1. Drag the ScenesController gameobject into the 'on click' button event
  2. Select ScenesController script and the LoadScene function
  3. Drag the "GetScenesEnumFromInspector" script into the functions field
  4. Set the GetScenesEnumFromInspector scripts enum to the scene that will be loaded.

Main menu 
  - Play button: Loads map scene
  - Backpack button: Loads backpack scene
  - Music ring: Dragon Music script into the 'On Click' event and select Music script and Toggle function
	    Music script requires the MusicOn and MusicOff gameobjects within the Music gameobject hierarchy
  - Potion icon: Load store scene
  - Information ring: Load information scene

Information Scene
  - Back arrow: Loads main menu scene

Backpack Scene
  - Back arrow: Loads main menu scene
  - Use Potion Button: Drag 'GLOBAL' gameobject into 'On Click' event, select Backpack script with UsePotion function
  - Potion icon: Loads store scene
  - Character 1: Drag 'GLOBAL' gameobject into 'On Click' event, select Backpack script with Heal Character function, drag the Index Data script into the field and set index data value to 0
  - Character 2: same as character 1 with index data value to 1
  - Character 3: same as character 1 with index data value to 2
  - Healbar Wrappers: Each HealthBarWithCoroutineScript requires the "CurrentHealth" Object within it's hierarchy and the "HPText" (ratio text) to be populated... leave the other 2 fields empty

Store Scene
  - Buy now buttons: Drag 'GLOBAL' gameobject into 'On Click' event, select Store script with MakePurchase function, dragon Index Data script into the field and set the inded data value in chronological order from top to bottom starting at index 0
  - Back arrow: Drag 'GLOBAL' gameobject into 'On Click' event, select ScenesController script with LoadPreviousScene

QR Scene
  - EnemyImage and Characters object in hierarchy must be set to disabled (hidden) before starting the program
  - Global GameObject:
	- Scanner Continuous: Drag Cameras Image gameobject into Image field, Drag main camera into Audio field
	- QR Scene Manager: Select Dealth colour, drag Characters gameobject into Character selection container field, drag Assassin gameobject (inside Characters > AssasinBoarder hierarchy) into assassin image field, repeat for other 2 characters
  - Back arrow: Drag 'GLOBAL' gameobject into 'On Click' event, select ScannerContinuous script with ClickBack (this function will stop the camera before leaving)
  - EnemyImage: Dragon Global gameobject into 'On Click' event, select QRScaneManager script with EnemyClicked function
  - Foreach Character: Select the image icon, multiple 'On Click' events will be connected with each object
	1. Drag AssassinBoarder into event, select RawImage with enabled, set to (true or false - depending on which character is selected... if assassin is selected this is true, else false)
	2. Drag GuardianBoarder into event, select RawImage with enabled, set to (true or false - depending on which character is selected... if guardian is selected this is true, else false)
	3. Drag WizardBoarder   into event, select RawImage with enabled, set to (true or false - depending on which character is selected... if wizard is selected this is true, else false)
	4. Drag 'GLOBAL' gameobject into 'On Click' event, select QRScaneManager script with CharacterSelected function, drag the Index Data script into the field and set index data value to (0 - 2) depending on which character is selected

Map
  - Back arrow: Load mainmenu scene
  - Face icon: Drag Camera gameobject into 'On Click' event, select SwitchCamera script with Toggle function
  - Camera icon (by default it's hidden select within hierarchy): same setup as face icon
  - Radar ping icon: Drag 'MapSceneManager' into 'On Click' event, select RadarPing script with Ping function

Backpack scene
  - BattleSceneManager gameobject contains main scripts
	1. BattleSceneController --> scripts do not need to be populated done within script can drag them within if wanted, set choice message to "Which will you choose?"
	2. HealthBarController
	   - Drag PlayerStatsContainer into player health bar
	   - Drag EnemyStatsContainer into enmeies health bar
	   - Drag Battle Gui Manager into Battle GUI Manager
	3. ExperienceBarController --> Drag PlayerStatsContainer into ExperienceBar field
	4. CombatController --> wait time set in script current value 0.2
	5. BattleGUIManager (Custom inspector view)
	   - Text wait time default 0.5
	   - Action Component field drag ActionWindow into it
	   - Foreach Ability Window drag the corresponding ability button values into the fields
		- Within the hierarchy Text-Action-Selection-Container > Action and Selection container > SelectWindow > MoveSelectionWindow > Wrapper > (TopLeft > Ability1Button or TopRight for ability2button, etc...)
	   - Text window
		- drag the textwindow gameobject into component field
		- under TextWindow hierarchy > RedWrapper > inner-whitewrapper... drag the TextWindowText component into Text field
	   - Sprites 
		- Drag the playersprite gameobject into the player sprite field
		- Drag the enemysprite gameobject into the enemy sprite field
  - EnemyStatsContainer 
	- Contains HealthBarWithCoroutine Script 
		- Drag the CurrentHealth image (found within hierarchy EnemyStatsContainer > OutterWrapper > InnerWrapper > HealthWrapper > Right > HealthBar > currentHealth) into the healthbar field
		- Drag the HpText into the ratio text (found in the same hierarchy as currenthealth image)
		- Drag the Name component into the Charactername field (found in hierarchy EnemyStatsContainer > OutterWrapper > InnerWrapper > Top > Left > Name)
		- Drag the level component into the Characterlevel field (found in hierarchy EnemyStatsContainer > OutterWrapper > InnerWrapper > Top > Right > Level)
  - PlayerStatsContainer
	- Containers HealthBarWithCoroutine (same setup with enemystatscontainer using the hierarchy begining with PlayerStatsContainer)
	- Contains ExperienceBar 
		- under EXPWrapper the Exp component is found for the xp bar field
		- Level field is found within the PlayerStatsContainer > OutterWrapper > InnerWrapper > Top > right > Level
  - Attack Button: drag selectionwindow into on click event and select GameObject > setactive to true
  - Heal Button: drag battlescenemanager into on click event and select battleguimanager script with abilityselected, drag index data with index value of -1 into the field
  - Run Button: Load map scene
  - Ability Selection window
	- Back button: drag selectionwindow into on click event and select GameObject > setactive to false
	- Foreach ability: 
		- Drag battlescenemanager into on click event select battleguimanager script with abilityselected function, drag index data value into field, index data value is select to the ability button value -1 (0 - 3)
		- Drag selectionwindow into on click event and select GameObject > setactive to false


Notes: 
After written this readme i found that many scripts required other scripts to be dragged into them, i will change that to find it through code instead making less inspector interaction required
Also we will look into seeing if we can make less hierarchy objects within battle scene (removing the Left and Right stuff)


















