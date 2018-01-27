using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace GracesGames.SimpleFileBrowser.Scripts
{
    public class QRCodeManager : MonoBehaviour
    {
        //TestMode without location
        public bool testMode;

        // Eneny Image UI
        public Image enemyImage;

        //QR Code Image UI
        public Image qrCodeImage;

        // Use the file browser prefab
        public GameObject FileBrowserPrefab;

        //Message Text
        public Text message;

        // Define a file extension
        public string FileExtension;
        
        // Input field to get text to save
        public InputField input_qr_string;

        //Player Location Service 
        PlayerLocationService player_loc;

        //Fixed Image Width for UI
        public readonly static int IMAGE_WIDTH = 200;

        //Fixed Image Height for UI
        public readonly static int IMAGE_HEIGHT = 200;

        //Enemy Image Location
        public readonly static string ENEMY_IMAGE = "/EnemyImage.png";

        //Enemy Image Location
        public readonly static string QRCODE_IMAGE = "/QRImage.png";

        //png extention
        public readonly static string PNG = "png";

        //QR Code Generator URL
        public readonly static string QRCodeGeneratorURL = " https://api.qrserver.com/v1/create-qr-code/?size="+ IMAGE_WIDTH +"x" + IMAGE_HEIGHT +"&data=";

        //Potrait mode (set from inspector)
        public bool PortraitMode;

        //Firebase Database Connector
        public FirebaseDatabaseConnector firebaseDatabase;

        //Firebase Authentication Handler
        public FirebaseAuthenticationHandler firebaseAuth;

        private QRCode qrCode;

        // Use this for initialization
        void Start()
        {
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            firebaseDatabase = FindObjectOfType<FirebaseDatabaseConnector>();
            firebaseAuth = FindObjectOfType<FirebaseAuthenticationHandler>();
            SetEnemySpriteFromPNG();
            SetQRCodeSpriteFromPNG();
            StartLocationService();
            GameObject uiCanvas = GameObject.Find("MenuCanvas");
            qrCode = new QRCode();
            if (uiCanvas == null)
            {
                Debug.LogError("Make sure there is a canvas GameObject present in the Hierarcy (Create UI/Canvas)");
            }
        }
        

        // Open the file browser using boolean parameter so it can be called in GUI
        public void OpenFileBrowser(bool saving)
        {
            OpenFileBrowser(saving ? FileBrowserMode.Save : FileBrowserMode.Load);
        }


        // Callback for QR Code Lock Button
        public void LockQRCode()
        {
            GeoPoint loc = GetCurrentLocation();
            if (loc != null)
            {
                qrCode.Latitude = loc.lat_d;
                qrCode.Longitude = loc.lon_d;
                qrCode.UserID = firebaseDatabase.GetUserID();
                qrCode.username = firebaseDatabase.GetUserDisplayName();
                FirebaseDatabaseConnector.FileCallback callback = OnFileUplaod;
                string enemyImagePath = Application.persistentDataPath + ENEMY_IMAGE;
                byte[] bytes = LoadBytesFromPath(enemyImagePath);
                if (bytes != null)
                {
                    StartCoroutine(firebaseDatabase.UploadToFirebaseStorage(bytes, qrCode.QRCodeID + "." + PNG, FirebaseDatabaseConnector.ENEMY_IMAGES_FOLDER, callback));
                }
            }
            else
            {
                message.text = "Enable Location Service";
            }
        }


        //Load file into bytes from path
        private byte[] LoadBytesFromPath(string path)
        {
            if (System.IO.File.Exists(path))
            {
                return System.IO.File.ReadAllBytes(path);
            }
            return null;
        }


        //Callback For File Upload
        public void OnFileUplaod(string path, string reply)
        {
            message.text = reply;
            if (!String.IsNullOrEmpty(path))
            {
                qrCode.imageURL = path;
                firebaseDatabase.InsertQRCode(qrCode);
                message.text = "QR Code Locked to Location Latitude " + qrCode.lat_d + " Longitude " + qrCode.lon_d;
            }
            else
            {
                message.text = "Could Not Upload Image";
            }
        }


        //Callback for QR Code Generation Button
        public void GenerateQRCode()
        {
            qrCode.GenerateQRCode();
            string code = qrCode.QRCodeID;
            input_qr_string.text = code;
            GetQRCodeForString(code);
        }


        // Open a file browser to save and load files
        private void OpenFileBrowser(FileBrowserMode browserMode)
        {
            if (browserMode == FileBrowserMode.Save && String.IsNullOrEmpty(input_qr_string.text))
            {
                Debug.Log("Could not start file browser becuase QR has not been generated");
                message.text = "Generate QR Code First";
                return;
            } 
            // Create the file browser and name it
            GameObject fileBrowserObject = Instantiate(FileBrowserPrefab, transform);
            fileBrowserObject.name = "FileBrowser";

            // Set the mode to save or load
            FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
            fileBrowserScript.SetupFileBrowser(PortraitMode ? ViewMode.Portrait : ViewMode.Landscape);
            if (browserMode == FileBrowserMode.Save)
            {
                fileBrowserScript.SaveFilePanel(this, "SaveImageFromPath", input_qr_string.text, PNG);
            }
            else
            {
                fileBrowserScript.OpenFilePanel(this, "LoadImageFromPath", FileExtension);
            }
        }


        // Saves the image file using a path
        private void SaveImageFromPath(string path)
        {
            // Make sure path and _textToSave is not null or empty
            if (!String.IsNullOrEmpty(path) && !String.IsNullOrEmpty(input_qr_string.text))
            {
                string qrCodePath = Application.persistentDataPath + QRCODE_IMAGE;
                if (System.IO.File.Exists(qrCodePath))
                {
                    Texture2D texture = GetTextureFromPath(qrCodePath, IMAGE_WIDTH, IMAGE_HEIGHT);
                    if (texture != null)
                    {
                        StoreTextureAsPNG(texture, path);
                        message.text = "Saved image at " + path;
                    } 
                    else
                    {
                        Debug.Log("Could Not Load Texture from " + qrCodePath);
                    }
                }
            }
            else
            {
                Debug.Log("Invalid path or empty file given");
            }
        }


        // Loads a image file using a path
        private void LoadImageFromPath(string path)
        {
            if (path.Length != 0)
            {
                enemyImage.sprite = CreateSpriteFromPath(path, IMAGE_WIDTH, IMAGE_HEIGHT);
                StoreTextureAsPNG(GetTextureFromPath(path, IMAGE_WIDTH, IMAGE_HEIGHT), Application.persistentDataPath + ENEMY_IMAGE);
                message.text = "Loaded image from " + path;
            }
            else
            {
                Debug.Log("Invalid path or empty file given");
            }
        }

        
        //Create a sprite using path 
        private Sprite CreateSpriteFromPath(string path, int width, int height)
        {
            Texture2D texture = GetTextureFromPath(path, width, height);
            return Sprite.Create(texture, new Rect(0, 0, width, height), new Vector2(0, 0));
        }


        //Get Texture from Image on File System
        private Texture2D GetTextureFromPath(string path, int width, int height)
        {
            if (System.IO.File.Exists(path))
            {
                byte[] bytes = System.IO.File.ReadAllBytes(path);
                Texture2D texture = new Texture2D(width, height);
                texture.LoadImage(bytes);
                TextureScale.Bilinear(texture, width, height);
                return texture;
            }
            else
            {
                Debug.Log("File does not exist at " + path);
                return null;
            }

        }


        //Save Texture as PNG on FileSystem
        private void StoreTextureAsPNG(Texture2D texture, string path)
        {
            try
            {
                System.IO.File.WriteAllBytes(path, texture.EncodeToPNG());
            }
            catch (Exception e)
            {
                Debug.Log("Could not save image at " + path + " because " + e.Message);
            }
        }


        //Load Enemy Sprite From File Location
        private void SetEnemySpriteFromPNG()
        {
            try
            {
                string path = Application.persistentDataPath + ENEMY_IMAGE;
                if (System.IO.File.Exists(path))
                {
                    enemyImage.sprite = CreateSpriteFromPath(path, IMAGE_WIDTH, IMAGE_HEIGHT);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Could not Load Enemy Image");
            }
        }


        //Load QR Code Sprite From File Location
        private void SetQRCodeSpriteFromPNG()
        {
            try
            {
                string path = Application.persistentDataPath + QRCODE_IMAGE;
                if (System.IO.File.Exists(path))
                {
                    qrCodeImage.sprite = CreateSpriteFromPath(path, IMAGE_WIDTH, IMAGE_HEIGHT);
                }
            }
            catch (Exception e)
            {
                Debug.Log("Could not Load Enemy Image");
            }
        }


        // Retrieve Bar Code for String Data
        private void GetQRCodeForString(string data)
        {
            StartCoroutine(GetQRCode(data));
        }


        //Coroutine to make GET Request to QR Code Generator
        IEnumerator GetQRCode(string data)
        {
            WWW www = new WWW(QRCodeGeneratorURL + data);
            yield return www;
            StoreTextureAsPNG(www.texture, Application.persistentDataPath + QRCODE_IMAGE);
            qrCodeImage.sprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), new Vector2(0, 0));
        }


        //Retreive Current Location of Player
        private GeoPoint GetCurrentLocation()
        {
            if (player_loc != null && (Input.location.isEnabledByUser || testMode))
            {
                return player_loc.loc;
            }
            return null;
        }


        //Start Location Service 
        private void StartLocationService()
        {
            player_loc = FindObjectOfType<PlayerLocationService>();
            StartCoroutine(player_loc._StartLocationService());
            StartCoroutine(player_loc.RunLocationService());
        }

    }
}