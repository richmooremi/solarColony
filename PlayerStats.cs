/*
    This class is persistent and holds various player
    defined values as well as earned achievements

    RICH MOORE
    April 29, 2016
*/


using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    //player defined variables
    public string playerName;           //the name the player chooses
    public int playerDifficulty;        //the difficulty the player chooses
    public Sprite playerAvatar;         //the avatar the player chooses

    //game defined variables
    public int badgeID;                                                                 //a randomly generated badge ID number
    public bool firstPlay = true;                                                       //true before the player finishes their first game
    public Dictionary<string, bool> achievements = new Dictionary<string, bool>();      //key: achivement name / value: true when the player has completed
    public List<int> completedDifficulties;                                             //the difficulties the player has completed the game at

    //used to toggle between mouse and touch control
    public bool useTouchInput = false;

    //a reference to this class, used to make this a singleton
    public static PlayerStats stats;

    void Awake ()
    {
        //if this is the first scene loaded and the stats manager doesn't exist yet
        if (stats == null)
        {
            DontDestroyOnLoad(gameObject);      //makes this object persistent
            stats = this;                       //makes this the current manager

            //generate a random 7 digit badge number for the player
            badgeID = (int)Mathf.Ceil(Random.Range(1000000, 9999998));

        }

        //if the stats manager already exists, destroy this object
        else if (stats != this)
            Destroy(gameObject);
    }

    public void setTouchControl()
    {
        useTouchInput = FindObjectOfType<Toggle>().isOn;
        Debug.Log(useTouchInput);
    }


}