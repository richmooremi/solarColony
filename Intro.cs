/*
    This class controls the various inputs on the intro screen,
    passes values into PlayerStats, and passes values back into
    the input fields when the intro screen is reloaded

    RICH MOORE
    April 9, 2016
*/

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour {

    public PlayerStats stats;                   //the persistent PlayerStats object, holds values to be carried between games
    private Button[] buttons;                   //array of all button objects, used to isolate avatar buttons
    public Text placeHolderText;                //the placeholder text of the input box, uded to set the previous value on reload
    public Slider difficultySlider;             //the difficult slider, used to set the previous value on reload

    //error messages that appear on the screen if the player
    //triest to start the game without selecting a name or avatar
    private GameObject nameError;
    private GameObject avatarError;

    //gameobjects identifying the currently and previous avatar
    public GameObject previouslySlecetedAvatar;
    public GameObject currentlySelectedAvatar;

    private void Awake()
    {
        //find the PlayerStats object
        stats = FindObjectOfType<PlayerStats>();

        //populate buttons array with all buttons on the screen
        buttons = FindObjectsOfType<Button>();

        avatarError = GameObject.Find("AvatarError");
        nameError = GameObject.Find("NameError");

        //if the player has seen the intro screen before, load stats from the manager
        if (!stats.firstPlay)
        {
            //set name and difficulty to previously set values
            placeHolderText.text = stats.playerName;
            placeHolderText.color = new Color(0, 0, 0, 1f);
            difficultySlider.value = stats.playerDifficulty;

            //color each completed difficulty
            foreach (int i in stats.completedDifficulties)
            {
                GameObject diff = GameObject.Find(i.ToString());
                diff.GetComponent<Text>().color = Color.gray;
            }

            //highlight the current player avatar
            GameObject.Find(stats.playerAvatar.name).GetComponent<Button>().image.color = new Color(255, 255, 255, 0.5f);
        }

        //get the default value of the difficulty slider, fixes a bug when a new value isn't selected
        stats.playerDifficulty = (int)difficultySlider.value;
    }

    private void Start()
    {
        //make sure error messages are hidden
        nameError.GetComponent<Text>().enabled = false;
        avatarError.GetComponent<Text>().enabled = false;
    }

    #region sets
    public void SetName(string name)
    {
        //sets player name to selected name
        stats.playerName = name;
    }

    public void SetDifficulty(Slider difficulty)
    {
        //sets player difficulty to selected difficulty
        stats.playerDifficulty = (int)difficulty.value;
    }

    public void SetAvatar()
    {
        //sets player avatar to selected avatar
        stats.playerAvatar = EventSystem.current.currentSelectedGameObject.GetComponent<Button>().image.sprite;

        //swap the currently and previously selected avatars
        previouslySlecetedAvatar = currentlySelectedAvatar;
        currentlySelectedAvatar = EventSystem.current.currentSelectedGameObject;

        //change the alpha of the previous avatar to 100% and the current one to 50%
        if (previouslySlecetedAvatar != null)
            previouslySlecetedAvatar.GetComponent<Button>().image.color = new Color(255, 255, 255, 1f);
        currentlySelectedAvatar.GetComponent<Button>().image.color = new Color(255, 255, 255, 0.5f);

    }
    #endregion

    public bool isReady()
    {
        if (stats.playerName != "" && stats.playerAvatar != null)
            return true;

        return false;
    }

    public void onClicked()
    {
        avatarError.GetComponent<Text>().enabled = false;
        nameError.GetComponent<Text>().enabled = false;

        //load the first map
        if (isReady())
        {
            SceneManager.UnloadSceneAsync(0);
            SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
            
        }

        else
        {
            if (stats.playerAvatar == null)
                avatarError.GetComponent<Text>().enabled = true;

            if (stats.playerName == "")
                nameError.GetComponent<Text>().enabled = true;
        }
    }

    public void Quit()
    {
        //close the appliaction
        Application.Quit();
    }
}
