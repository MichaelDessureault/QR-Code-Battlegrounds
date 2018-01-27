using UnityEngine;

public class Music : MonoBehaviour {
    
    public GameObject musicOn;
    public GameObject musicOff;
    
    private bool state;

    public void Start()
    {
        if (musicOn == null || musicOff == null)
        {
            Debug.LogWarning("MusicOn or MusicOff object are not found in music script attachted to " + gameObject.name + " the music icons will not work correctly");
        }

        // load previous state from json file
        state = (PlayerPrefs.GetInt("MusicState", 1) == 1) ? true : false;
        UpdateMusicGameObjects();
    }
    
    public void Toggle ()
    {
        state = !state;

        PlayerPrefs.SetInt("MusicState", ((state) ? 1 : 0));
        
        // Do something based on state
        if (state)
        {
            // music is on turn on music
        } else
        {
            // turn off music
        }

        UpdateMusicGameObjects(); 
    }

    private void UpdateMusicGameObjects()
    {
        if (musicOn == null || musicOff == null)
            return;

        // musicOn object gets set to the current state (either on or off)
        // musicOff gets set to the opposite state
        musicOn.SetActive(state);
        musicOff.SetActive(!state);
    }
}
