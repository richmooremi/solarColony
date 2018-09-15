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

public class Intro : MonoBehaviour {

    public PlayerStats stats;                   //the persistent PlayerStats object, holds values to be carried between games
    private Button[] buttons;                   //array of all button objects, used to isolate avatar buttons
    public Text placeHolderText;                //the placeholder text of the input box, uded to set the previous value on reload
    public Slider difficultySlider;             //the difficult slider, used to set the previous value on reload

    private void Awake()
    {
        //find the PlayerStats object
        stats = FindObjectOfType<PlayerStats>();

        //populate buttons array with all buttons on the screen
        buttons = FindObjectsOfType<Button>();

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
        //change all avatar buttons back to 100% alpha
        foreach (Button b in buttons)
        {
            if (b.name.Contains("avatar"))
            {
                b.image.color = new Color(255, 255, 255, 1f);
            }
        }

        //sets player avatar to selected avatar
        stats.playerAvatar = EventSystem.current.currentSelectedGameObject.GetComponent<Button>().image.sprite;

        //set currently selected avatar image to 50% alpha
        EventSystem.current.currentSelectedGameObject.GetComponent<Button>().image.color = new Color(255, 255, 255, 0.5f);
    }
    #endregion

    public void onClicked()
    {
        //load the first map
        Application.LoadLevel(1);
    }

    public void Quit()
    {
        //close the appliaction
        Application.Quit();
    }
}
