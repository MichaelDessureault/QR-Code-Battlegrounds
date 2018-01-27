using System;
using System.Collections;
using System.Collections.Generic;
using Firebase;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Unity.Editor;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;


public class FirebaseDatabaseConnector : MonoBehaviour
{

    //Firebase bucket to store enemies
    protected string EnemyBucketURL = "gs://barcodebattleground.appspot.com";

    //Firbase Dependency Status
    DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;

    //Firebase Database URL
    public readonly static string FIREBASE_DATABASE_URL = "https://barcodebattleground.firebaseio.com/";

    //Names of List in Database
    public readonly static string QRCODE_LIST = "QRCodes";
    public readonly static string PLAYERS = "Players";
    public readonly static string CHARACTERS = "Characters";
    public readonly static string STORE_DATA = "StoreData";
    public readonly static string LATITUDE = "lat_d";
    public readonly static string LONGITUDE = "lon_d";

    //Names of Storage Folders
    public readonly static string ENEMY_IMAGES_FOLDER = "EnemyImages";

    //Debug Logs Text
    private string logText = "";

    //Scene Controller to Control Scenes
    private ScenesController scenesController;

    //Firebase Storage
    protected FirebaseStorage storage;

    //Firebase Storage Root Reference
    protected StorageReference storage_ref;

    //Firebase Storage Location
    protected string firebaseStorageLocation;

    //Callback for QR Code Operations
    public delegate void QRCodeCallback(QRCode code, string message);

    public delegate void QRCodeListCallback(List<QRCode> codes, string message);

    //Callback for File Operations
    public delegate void FileCallback(string path, string message);

    //Callback for Database Operation
    public delegate void DatabaseCallback(bool isSuccessful, string message);

    // Use this for initialization
    void Awake()
    {
        scenesController = FindObjectOfType<ScenesController>();
        //Firebase Fix Dependencies
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                InitializeFirebaseDatabase();
                InitializeFirebaseStorage();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }


    // Initialize the Firebase database:
    protected virtual void InitializeFirebaseDatabase()
    {
        FirebaseApp app = FirebaseApp.DefaultInstance;
        app.SetEditorDatabaseUrl(FIREBASE_DATABASE_URL);
        if (app.Options.DatabaseUrl != null) app.SetEditorDatabaseUrl(app.Options.DatabaseUrl);
    }


    // Intialize the Firebase storage
    private void InitializeFirebaseStorage()
    {
        var appBucket = FirebaseApp.DefaultInstance.Options.StorageBucket;
        storage = FirebaseStorage.DefaultInstance;
        storage_ref = storage.GetReferenceFromUrl(EnemyBucketURL);
        if (!String.IsNullOrEmpty(appBucket))
        {
            EnemyBucketURL = String.Format("gs://{0}/", appBucket);
        }
    }


    // Upload File to Firebase Storage
    public IEnumerator UploadToFirebaseStorage(byte[] data, string fileName, string folder, FileCallback Callback)
    {
        StorageReference images = storage_ref.Child(folder);
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_4
    var task = reference.PutBytesAsync(Encoding.UTF8.GetBytes(data), null, null,
                                       default(System.Threading.CancellationToken), null);
#else
        var task = images.Child(fileName).PutBytesAsync(data);
#endif
        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsFaulted)
        {
            Callback(null, "Error Uploading Enemy Image");
            DebugLog(task.Exception.ToString());
            throw task.Exception;
        }
        else
        {
            Callback(task.Result.DownloadUrl.ToString(), "Enemy Image Uploaded Succesfully");
            DebugLog("Finished uploading... Download Url: " + task.Result.DownloadUrl.ToString());
        }
    }


    // Insert QR Code to list of QR Codes on Firebase
    public void InsertQRCode(QRCode code)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(QRCODE_LIST);
        string json = JsonUtility.ToJson(code);
        reference.Child(code.QRCodeID).SetRawJsonValueAsync(json).ContinueWith(task =>
        {
            if (task.IsCompleted && task.Exception == null)
            {
                Debug.Log("Stored QR Code " + json);
            }
        });
    }


    //Check if Scanned QR Code Exists in Firebase
    public void CheckValidQRCode(string id, QRCodeCallback callback)
    {
        if (!QRCode.VALIDATE_QR_CODE_ID(id))
        {
            callback(null, "Not a Game QR Code");
            return;
        }
        FirebaseDatabase.DefaultInstance.
            GetReference(QRCODE_LIST)
            .Child(id)
            .LimitToFirst(1)
            .GetValueAsync().ContinueWith((task) =>
            {
                if (task.IsFaulted)
                {
                    callback(null, "Could not check QR Code");
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot != null && snapshot.ChildrenCount > 0)
                    {
                        string json = snapshot.GetRawJsonValue();
                        QRCode code = CreateQRCodeFromJSON(json);
                        code.QRCodeID = snapshot.Key;
                        callback(code, "Valid QR Code");
                    }
                    else
                    {
                        callback(null, "QR Code Not Found");
                    }
                }
            });
    }


    //Retreive All QRCodes
    public void GetQRCodes(QRCodeListCallback callback)
    {
        FirebaseDatabase.DefaultInstance.
           GetReference(QRCODE_LIST)
           .GetValueAsync().ContinueWith((task) =>
           {
               if (task.IsFaulted)
               {
                   callback(null, "Could Not Load Enemy Locations");
               }
               else if (task.IsCompleted)
               {
                   List<QRCode> codes = new List<QRCode>();
                   DataSnapshot snapshot = task.Result;
                   if (snapshot != null && snapshot.ChildrenCount > 0)
                   {
                       foreach (var childSnapshot in snapshot.Children)
                       {
                           string json = childSnapshot.GetRawJsonValue();
                           QRCode code = CreateQRCodeFromJSON(json);
                           code.QRCodeID = childSnapshot.Key;
                           codes.Add(code);
                       }
                       callback(codes, "Enemy Location Loaded");
                   }
                   else
                   {
                       callback(null, "Enemies Not Found");
                   }
               }
           });
    }


    //Retreive All QRCodes Within Latitudes
    public void GetQRCodesWithinLatitudes(QRCodeListCallback callback, float start_lat, float end_lat)
    {
        FirebaseDatabase.DefaultInstance.
           GetReference(QRCODE_LIST)
           .OrderByChild(LATITUDE)
           .StartAt(start_lat)
           .EndAt(end_lat)
           .GetValueAsync()
           .ContinueWith((task) =>
           {
               if (task.IsFaulted)
               {
                   callback(null, "Could Not Load : Locations");
               }
               else if (task.IsCompleted)
               {
                   List<QRCode> codes = new List<QRCode>();
                   DataSnapshot snapshot = task.Result;
                   if (snapshot != null && snapshot.ChildrenCount > 0)
                   {
                       foreach (var childSnapshot in snapshot.Children)
                       {
                           string json = childSnapshot.GetRawJsonValue();
                           QRCode code = CreateQRCodeFromJSON(json);
                           code.QRCodeID = childSnapshot.Key;
                           codes.Add(code);
                       }
                       callback(codes, "Enemies Found Nearby");
                   }
                   else
                   {
                       callback(null, "No Enemies Nearby");
                   }
               }
           });
    }


    //Retreive All QRCodes Within Longitudes: Not Used Yet
    public void GetQRCodesWithinLongitudes(QRCodeListCallback callback, float start_lon, float end_lon)
    {
        FirebaseDatabase.DefaultInstance.
           GetReference(QRCODE_LIST)
           .OrderByChild(LONGITUDE)
           .StartAt(start_lon)
           .EndAt(end_lon)
           .GetValueAsync()
           .ContinueWith((task) =>
           {
               if (task.IsFaulted)
               {
                   callback(null, "Could Not Load Enemy Locations");
               }
               else if (task.IsCompleted)
               {
                   List<QRCode> codes = new List<QRCode>();
                   DataSnapshot snapshot = task.Result;
                   if (snapshot != null && snapshot.ChildrenCount > 0)
                   {
                       foreach (var childSnapshot in snapshot.Children)
                       {
                           string json = childSnapshot.GetRawJsonValue();
                           QRCode code = CreateQRCodeFromJSON(json);
                           code.QRCodeID = childSnapshot.Key;
                           codes.Add(code);
                       }
                       callback(codes, "Enemy Location Loaded");
                   }
                   else
                   {
                       callback(null, "Enemies Not Found");
                   }
               }
           });
    }


    public void GetCharacterDataFromDatabase()
    {
        FirebaseDatabase.DefaultInstance.
           GetReference(PLAYERS)
           .Child(GetUserID())
           .Child(CHARACTERS)
           .GetValueAsync()
           .ContinueWith((task) =>
           {
               if (task.IsFaulted)
               {
                   Debug.Log("Fuk you 2");
               }
               else if (task.IsCompleted)
               {
                   DataSnapshot snapshot = task.Result;
                   if (snapshot != null && snapshot.ChildrenCount > 0)
                   {
                       foreach (var childSnapshot in snapshot.Children)
                       {
                           Debug.Log(childSnapshot);
                       }
                   }
               }
           });
    }


    public void SaveCharacterData(DatabaseCallback callback)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(PLAYERS);
        reference.Child(this.GetUserID()).Child(CHARACTERS).SetRawJsonValueAsync(GetCharacterDataFromFiles()).ContinueWith(task =>
        {
            if (task.IsFaulted && task.Exception!=null)
            {
                callback(false, "Could Not Save Character Data");
            }
            else if (task.IsCompleted)
            {
                callback(true, "Saved Character Data");
            }
        });
    }


    public string GetCharacterDataFromFiles()
    {
        string output = "[";
        for (int i = 0; i < SaveData.characterSavePaths.Length; i++)
        {
            string path = Application.persistentDataPath + SaveData.characterSavePaths[i];
            if (System.IO.File.Exists(path))
            {
                output += System.IO.File.ReadAllText(path);
                if (i < SaveData.characterSavePaths.Length - 1)
                {
                    output += ",";
                }
            }
        }
        output += "]";
        return output;
    }


    public void SaveCharacterAbilitiesData(DatabaseCallback callback)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(PLAYERS);
        reference.Child(this.GetUserID()).Child("Abilities").SetRawJsonValueAsync(GetCharacterDataFromFiles()).ContinueWith(task =>
        {
            if (task.IsFaulted && task.Exception != null)
            {
                callback(false, "Could Not Save Character Abilities");
            }
            else if (task.IsCompleted)
            {
                callback(true, "Saved Character Abilities Data");
            }
        });
    }


    public string GetCharacterAbilitiesFromFiles()
    {
        string output = "[";
        for (int i = 0; i < SaveData.characterAbilitySetSavePaths.Length; i++)
        {
            string path = Application.persistentDataPath + SaveData.characterAbilitySetSavePaths[i];
            if (System.IO.File.Exists(path))
            {
                output += System.IO.File.ReadAllText(path);
                if (i < SaveData.characterSavePaths.Length - 1)
                {
                    output += ",";
                }
            }
        }
        output += "]";
        return output;
    }


    public void SaveStoreData(DatabaseCallback callback)
    {
        DatabaseReference reference = FirebaseDatabase.DefaultInstance.GetReference(PLAYERS);
        reference.Child(this.GetUserID()).Child(STORE_DATA).SetRawJsonValueAsync(GetStoreDataFromFiles()).ContinueWith(task =>
        {
            if (task.IsFaulted && task.Exception != null)
            {
                callback(false, "Could Not Save Store Data");
            }
            else if (task.IsCompleted)
            {
                callback(true, "Saved Store Data");
            }
        });
    }


    public string GetStoreDataFromFiles()
    {
        string output = "";
        string path = Application.persistentDataPath + SaveData.storeDataJsonFile;
        if (System.IO.File.Exists(path))
        {
            output += System.IO.File.ReadAllText(path);
        }
        return output;
    }


    //Create QRCode from JSON
    private QRCode CreateQRCodeFromJSON(string json)
    {
        return JsonUtility.FromJson<QRCode>(json);
    }


    // Output text to the debug log text to the console.
    public void DebugLog(string s)
    {
        Debug.Log(s);
        logText += s + "\n";
    }


    //Get User ID of Currenly Logged In User
    public string GetUserID()
    {
        if (Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            return Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.UserId;
        }
        return "Anonymous";
    }


    //Get Name of Currenly Logged In User
    public string GetUserDisplayName()
    {
        if (Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser != null)
        {
            return Firebase.Auth.FirebaseAuth.DefaultInstance.CurrentUser.DisplayName;
        }
        return "Anonymous"; //For Testing Only, replace with null
    }
}