Group Members: 
	Michael Dessureault
	Farry Saini

Why QR Code Battlegrounds?
	QR Code Battlegrounds was my Capstone project during my final semester of my 3-year program at Sheridan College.  Where you scan QR codes to engage a battle scene to fight and enemy with the players 3 characters, each character has their own base level and exp.

How do you play? 
	Create an account or login (works on android only) 
	You can upload your own QR codes for boss fights with a picture these will be public for people within the area to fight.

	Upon clicking play you are taken to the map, the map uses GEO Location to track were the player is and the character updates its location when they person moves around. (Uses googles static maps API)
	
	Walk around to find the QR Codes in the area.
	Click the radar in the top right to see the nearby QR Codes.  (This is a radar ping finding previously scanned QR codes in the area)
	Once you find a QR code attempt to scan it.  It will say if it’s a game QR code or not (indicating if it’s found in the database or not) if it’s not try and upload the QR code through the upload feature.  If it’s found then you are taking to the battle scene to fight the enemy.  
	
	(You can press the skip arrow in the top right to skip the QR code feature, for demo purposes it will be a boss battle)

	To start a battle, select your character from one of the three: Assassin, Guardian or Wizard to fight against the enemy

	Once you have selected a character, select one of the three options: Attack, Heal, Run. But, if the enemy is a Boss, instead of Run the 3rd button will be Swap Character that will bring up the selection screen to select a new or the same character. 
	If you select Attack the ability window will appear for that character select an ability to attack the enemy with.  
	If you selected Heal it will use a potion on the character if their health is not already full and your turn will be over.
	The health bars will be affected based on the amount of damage your character/enemy takes.
	If your character wins (enemy health = 0), the character that you won the fight with will gain exp and if they level up to the required level for a new ability to be learned.  You will be prompted to either learn the new ability and remove an old one or give up on the new ability.

	If your character dies (character health = 0), you will be taken to back to the map screen that shows your current location and you can continue walking around to find more QR Codes.

Can't find a QR code?
	Since the radar ping checks for QR codes within your area, if the database doesn't have any then nothing will be displayed.  
	Try and upload a QR code first from the QR Code upload scene.

QR code upload scene
	Select an image you wish to make the enemy, can be anything if left default a random enemy sprite will be the boss image.
	Click Generate, to create the QR Code ID.  The QR code will change it's image upon generating new ids. 
	Save the QR Code image then print that up and post it around your location for others to scan. 
	Make sure to click lock to save the QR code within the database, it saves it by using your GEO location so you require internet access for this to work.
	Once locked if you go into the map scene and use the radar ping it will find that newly created QR code.

Battlepack Scene
	In here you heal up your characters to full health and also restore their abilities power points.

Store Scene
	Purchase coins (not real money) to then be used to purchase more potions (Health potion, Ether)
	Upon purchases coins an alert screen will pop up on the phone saying successful.

What platform is it for?
	It's an android application built within Unity. The application can run on pc but some features will not work (such as login and alert message when purchasing coins)
