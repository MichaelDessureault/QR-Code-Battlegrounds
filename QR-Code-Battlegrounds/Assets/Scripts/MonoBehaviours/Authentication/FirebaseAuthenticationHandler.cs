using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class FirebaseAuthenticationHandler : MonoBehaviour
{
    private ScenesController sceneController;

    //Firebase Stuff
    protected Firebase.Auth.FirebaseAuth auth;
    private Firebase.Auth.FirebaseAuth otherAuth;
    protected Dictionary<string, Firebase.Auth.FirebaseUser> userByAuth =
      new Dictionary<string, Firebase.Auth.FirebaseUser>();


    //Widgets
    public InputField input_display_name;
    public InputField input_email;
    public InputField input_password;
    public InputField input_confirm_password;
    public Text label_message;


    protected string email = "";
    protected string password = "";
    protected string displayName = "";

    Firebase.DependencyStatus dependencyStatus = Firebase.DependencyStatus.UnavailableOther;

    // Flag set when a token is being fetched.  This is used to avoid printing the token
    // in IdTokenChanged() when the user presses the get token button.
    private bool fetchingToken = false;


    // When the app starts, check to make sure that we have
    // the required dependencies to use Firebase, and if not,
    // add them if possible.
    public virtual void Start()
    {

        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = false;

        sceneController = FindObjectOfType<ScenesController>();
        Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == Firebase.DependencyStatus.Available)
            {
                InitializeFirebase();
            }
            else
            {
                Debug.LogError(
                  "Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }


    // Handle initialization of the necessary firebase modules:
    void InitializeFirebase()
    {
        auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        auth.IdTokenChanged += IdTokenChanged;
        AuthStateChanged(this, null);
    }


    void OnDestroy()
    {
        auth.StateChanged -= AuthStateChanged;
        auth.IdTokenChanged -= IdTokenChanged;
        auth = null;
        if (otherAuth != null)
        {
            otherAuth.StateChanged -= AuthStateChanged;
            otherAuth.IdTokenChanged -= IdTokenChanged;
            otherAuth = null;
        }
    }


    // Track state changes of the auth object.
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        Firebase.Auth.FirebaseUser user = null;
        if (senderAuth != null) userByAuth.TryGetValue(senderAuth.App.Name, out user);
        if (senderAuth == auth && senderAuth.CurrentUser != user)
        {
            bool signedIn = user != senderAuth.CurrentUser && senderAuth.CurrentUser != null;
            if (!signedIn && user != null)
            {
              //sceneController.LoadScene(ScenesEnum.login);
            }
            user = senderAuth.CurrentUser;
            userByAuth[senderAuth.App.Name] = user;
            if (signedIn)
            {
                sceneController.LoadScene(ScenesEnum.mainmenu);
                DebugLog("Signed in " + user.UserId);
                displayName = user.DisplayName ?? "";
                DisplayDetailedUserInfo(user, 1);
            }
        }
    }


    // Display a more detailed view of a FirebaseUser.
    void DisplayDetailedUserInfo(Firebase.Auth.FirebaseUser user, int indentLevel)
    {
        DisplayUserInfo(user, indentLevel);
        DebugLog("  Anonymous: " + user.IsAnonymous);
        DebugLog("  Email Verified: " + user.IsEmailVerified);
        var providerDataList = new List<Firebase.Auth.IUserInfo>(user.ProviderData);
        if (providerDataList.Count > 0)
        {
            DebugLog("  Provider Data:");
            foreach (var providerData in user.ProviderData)
            {
                DisplayUserInfo(providerData, indentLevel + 1);
            }
        }
    }


    // Display user information.
    void DisplayUserInfo(Firebase.Auth.IUserInfo userInfo, int indentLevel)
    {
        string indent = new String(' ', indentLevel * 2);
        var userProperties = new Dictionary<string, string> {
      {"Display Name", userInfo.DisplayName},
      {"Email", userInfo.Email},
      {"Photo URL", userInfo.PhotoUrl != null ? userInfo.PhotoUrl.ToString() : null},
      {"Provider ID", userInfo.ProviderId},
      {"User ID", userInfo.UserId}
    };
        foreach (var property in userProperties)
        {
            if (!String.IsNullOrEmpty(property.Value))
            {
                DebugLog(String.Format("{0}{1}: {2}", indent, property.Key, property.Value));
            }
        }
    }


    // Track ID token changes.
    void IdTokenChanged(object sender, System.EventArgs eventArgs)
    {
        Firebase.Auth.FirebaseAuth senderAuth = sender as Firebase.Auth.FirebaseAuth;
        if (senderAuth == auth && senderAuth.CurrentUser != null && !fetchingToken)
        {
            senderAuth.CurrentUser.TokenAsync(false).ContinueWith(
              task => DebugLog(String.Format("Token[0:8] = {0}", task.Result.Substring(0, 8))));
        }
    }


    // Log the result of the specified task, returning true if the task
    // completed successfully, false otherwise.
    bool LogTaskCompletion(Task task, string operation)
    {
        bool complete = false;
        if (task.IsCanceled)
        {
            DebugLog(operation + " canceled.");
        }
        else if (task.IsFaulted)
        {
            DebugLog(operation + " encounted an error.");
            foreach (Exception exception in task.Exception.Flatten().InnerExceptions)
            {
                string authErrorCode = "";
                Firebase.FirebaseException firebaseEx = exception as Firebase.FirebaseException;
                if (firebaseEx != null)
                {
                    authErrorCode = String.Format("AuthError.{0}: ",
                      ((Firebase.Auth.AuthError)firebaseEx.ErrorCode).ToString());
                    DisplayErrorMessage((Firebase.Auth.AuthError)firebaseEx.ErrorCode);
                }
                DebugLog(authErrorCode + exception.ToString());
            }
        }
        else if (task.IsCompleted)
        {
            DebugLog(operation + " completed");
            complete = true;
        }
        return complete;
    }


    private void DisplayErrorMessage(Firebase.Auth.AuthError authErrorCode)
    {
        switch (authErrorCode)
        {
            case Firebase.Auth.AuthError.EmailAlreadyInUse:
                label_message.text = "Email Already In Use";
                break;
            case Firebase.Auth.AuthError.InvalidEmail:
                label_message.text = "Invalid Email";
                break;
            case Firebase.Auth.AuthError.WrongPassword:
                label_message.text = "Invalid Email or Password";
                break;
            case Firebase.Auth.AuthError.NetworkRequestFailed:
                label_message.text = "Could Not Connect to Internet";
                break;
            default:
                label_message.text = authErrorCode.ToString();
                break;
        }
    }

    // Update the user's display name with the currently selected display name.
    public Task UpdateUserProfileAsync(string newDisplayName = null)
    {
        if (auth.CurrentUser == null)
        {
            DebugLog("Not signed in, unable to update user profile");
            return Task.FromResult(0);
        }
        displayName = newDisplayName ?? displayName;
        DebugLog("Updating user profile");
        return auth.CurrentUser.UpdateUserProfileAsync(new Firebase.Auth.UserProfile
        {
            DisplayName = displayName,
            PhotoUrl = auth.CurrentUser.PhotoUrl,
        }).ContinueWith(HandleUpdateUserProfile);
    }


    public Task SigninAsync()
    {
        DebugLog(String.Format("Attempting to sign in as {0}...", email));
        label_message.text = "Almost There!";
        return auth.SignInWithEmailAndPasswordAsync(email, password)
          .ContinueWith(HandleSigninResult);
    }


    void HandleUpdateUserProfile(Task authTask)
    {
        if (LogTaskCompletion(authTask, "User profile"))
        {
            DisplayDetailedUserInfo(auth.CurrentUser, 1);
        }
    }


    // Attempt to sign in anonymously.
    public Task SigninAnonymouslyAsync()
    {
        DebugLog("Attempting to sign anonymously...");
        return auth.SignInAnonymouslyAsync().ContinueWith(HandleSigninResult);
    }


    void LinkWithCredential()
    {
        if (auth.CurrentUser == null)
        {
            DebugLog("Not signed in, unable to link credential to user.");
            return;
        }
        DebugLog("Attempting to link credential to user...");
        Firebase.Auth.Credential cred = Firebase.Auth.EmailAuthProvider.GetCredential(email, password);
        auth.CurrentUser.LinkWithCredentialAsync(cred).ContinueWith(HandleLinkCredential);
    }



    void HandleLinkCredential(Task authTask)
    {
        if (LogTaskCompletion(authTask, "Link Credential"))
        {
            DisplayDetailedUserInfo(auth.CurrentUser, 1);
        }
    }


    public void ReloadUser()
    {
        if (auth.CurrentUser == null)
        {
            DebugLog("Not signed in, unable to reload user.");
            return;
        }
        DebugLog("Reload User Data");
        auth.CurrentUser.ReloadAsync().ContinueWith(HandleReloadUser);
    }


    void HandleReloadUser(Task authTask)
    {
        if (LogTaskCompletion(authTask, "Reload"))
        {
            DisplayDetailedUserInfo(auth.CurrentUser, 1);
        }
    }

    void HandleSigninResult(Task<Firebase.Auth.FirebaseUser> authTask)
    {
        if (LogTaskCompletion(authTask, "Sign-in"))
        {
            label_message.text = "Sign-In Succesful";
            //MainMenu scene started by AuthStateChanged after Sign-in
        }
    }

    void HandleGetUserToken(Task<string> authTask)
    {
        fetchingToken = false;
        if (LogTaskCompletion(authTask, "User token fetch"))
        {
            DebugLog("Token = " + authTask.Result);
        }
    }

    void GetUserInfo()
    {
        if (auth.CurrentUser == null)
        {
            DebugLog("Not signed in, unable to get info.");
        }
        else
        {
            DebugLog("Current user info:");
            DisplayDetailedUserInfo(auth.CurrentUser, 1);
        }
    }

    public void SignOut()
    {

        if (auth != null)
        {
            auth.SignOut();
        }
        sceneController.LoadScene(ScenesEnum.login);
    }


    public Task DeleteUserAsync()
    {
        if (auth.CurrentUser != null)
        {
            DebugLog(String.Format("Attempting to delete user {0}...", auth.CurrentUser.UserId));
            return auth.CurrentUser.DeleteAsync().ContinueWith(HandleDeleteResult);
        }
        else
        {
            DebugLog("Sign-in before deleting user.");
            // Return a finished task.
            return Task.FromResult(0);
        }
    }

    void HandleDeleteResult(Task authTask)
    {
        LogTaskCompletion(authTask, "Delete user");
    }


    // Show the providers for the current email address.
    public void DisplayProvidersForEmail()
    {
        auth.FetchProvidersForEmailAsync(email).ContinueWith((authTask) =>
        {
            if (LogTaskCompletion(authTask, "Fetch Providers"))
            {
                DebugLog(String.Format("Email Providers for '{0}':", email));
                foreach (string provider in authTask.Result)
                {
                    DebugLog(provider);
                }
            }
        });
    }


    // Send a password reset email to the current email address.
    public void SendPasswordResetEmail()
    {
        auth.SendPasswordResetEmailAsync(email).ContinueWith((authTask) =>
        {
            if (LogTaskCompletion(authTask, "Send Password Reset Email"))
            {
                DebugLog("Password reset email sent to " + email);
            }
        });
    }


    // Determines whether another authentication object is available to focus.
    public bool HasOtherAuth { get { return auth != otherAuth && otherAuth != null; } }


    // Swap the authentication object currently being controlled by the application.
    public void SwapAuthFocus()
    {
        if (!HasOtherAuth) return;
        var swapAuth = otherAuth;
        otherAuth = auth;
        auth = swapAuth;
        DebugLog(String.Format("Changed auth from {0} to {1}",
                               otherAuth.App.Name, auth.App.Name));
    }


    public Task CreateUserAsync()
    {
        DebugLog(String.Format("Attempting to create user {0}...", email));
        label_message.text = "Almost there!";
        // This passes the current displayName through to HandleCreateUserAsync
        // so that it can be passed to UpdateUserProfile().  displayName will be
        // reset by AuthStateChanged() when the new user is created and signed in.
        string newDisplayName = displayName;
        return auth.CreateUserWithEmailAndPasswordAsync(email, password)
          .ContinueWith((task) =>
          {
              return HandleCreateUserAsync(task, newDisplayName: newDisplayName);
          }).Unwrap();
    }


    //Used Handlers
    Task HandleCreateUserAsync(Task<Firebase.Auth.FirebaseUser> authTask,
                             string newDisplayName = null)
    {
        if (LogTaskCompletion(authTask, "User Creation"))
        {
            if (auth.CurrentUser != null)
            {
                label_message.text = "Registration Successful";
                DebugLog(String.Format("User Info: {0}  {1}", auth.CurrentUser.Email,
                                       auth.CurrentUser.ProviderId));
                return UpdateUserProfileAsync(newDisplayName: newDisplayName);
            }
        }
        // Nothing to update, so just return a completed Task.
        return Task.FromResult(0);
    }


    public void LoginButton()
    {
        email = input_email.text;
        password = input_password.text;
        if (String.IsNullOrEmpty(email))
        {
            label_message.text = "Empty Email";
            return;
        }
        if (String.IsNullOrEmpty(password))
        {
            label_message.text = "Empty Password";
            return;
        }
        if(auth.CurrentUser!=null)
        {
            auth.SignOut();
        }
        SigninAsync();
    }


    public void RegisterButton()
    {
        sceneController.LoadScene(ScenesEnum.registration);
    }


    public void SignUp()
    {
        string confirmedPassword = input_confirm_password.text;
        email = input_email.text;
        password = input_password.text;
        displayName = input_display_name.text;

        if (String.IsNullOrEmpty(displayName))
        {
            label_message.text = "Empty Name";
            return;
        }
        if (String.IsNullOrEmpty(email))
        {
            label_message.text = "Empty Email";
            return;
        }
        if (String.IsNullOrEmpty(password))
        {
            label_message.text = "Empty Password";
            return;
        }
        if (String.IsNullOrEmpty(password))
        {
            label_message.text = "Empty Password";
            return;
        }
        if (String.IsNullOrEmpty(confirmedPassword))
        {
            label_message.text = "Empty Confirm Password";
            return;
        }
        if (!password.Equals(confirmedPassword))
        {
            label_message.text = "Passwords not same";
            return;
        }
        CreateUserAsync();
    }


    // Output text to the debug log text field, as well as the console.
    public void DebugLog(string s)
    {
        Debug.Log(s);
    }

    public string GetUserID()
    {
        if(auth.CurrentUser!=null)
        {
            return auth.CurrentUser.UserId;
        }
        return null;
    }

}