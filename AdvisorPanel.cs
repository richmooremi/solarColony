/*
    This class is used to control the advisor panel in the bottom right corner of
    the UI panel and contains the tutorial text messages.

    RICH MOORE
    March 3, 2016
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class AdvisorPanel : MonoBehaviour {

    #region variables
    private bool newMessage = false;        //true when a new message is available to be read
    private bool textPanelOpen = false;     //true when the advisor's speech panel is open
    private int currentMessageIndex;        //the index number of the message being displayed

    public Text advisorText;                //the advisor's text label
    public Text speechText;                 //the text to be displayed in the advisor's speech panel
    public GameObject speechPanel;          //the advisor's speech panel

    public GameObject launchButton;         //the launch button that ends the game, shown after the player meets his goals
    public GameObject goalsPanel;           //the panel containing player goals, hidden after the launch button is shown

    Dictionary<string, string> messages;    //holds all of the messages which the advisor can display, accessed via keyword
    List<string> newMessages;               //holds all of the advisor's unread messages

    //variables related to the tutorial
    public bool inTutorial, tutGas, tutRock, newPlanet, lookingForGas,
        foundRock, foundGas, clickedMine;
    #endregion


    void Start () {

        //initialize message lists and set messages in dictionary
        messages = new Dictionary<string, string>();
        newMessages = new List<string>();
        SetMessages();

        inTutorial = false; //deactivate old tutorial
        //StartTutorial();

        //set introduction and mission objectives tutorial
        //necessary because the old tutorial is inactive
        SetNewMessage("Welcome");
        SetNewMessage("Tutorial1");

        //set advisor color to black, welcome message red immediately after the mini tutorial
        advisorText.color = Color.black;
    }

    public void StartTutorial()
    {
        //set tutorial flags
        inTutorial = true;
        tutGas = false;
        tutRock = false;
        newPlanet = false;
        lookingForGas = false;
        foundRock = false;
        foundGas = false;
        clickedMine = false;

        //set advisor color to red, welcome message changes it to black immediately
        advisorText.color = Color.red;

        //add the welcome message to the new message queue
        SetNewMessage("Tutorial1");
        SetNewMessage("Tutorial2");
        SetNewMessage("Tutorial3");
    }

    
    public void SetNewMessage(string msgName)
    {
        //this method accepts the name of a message, adds it to
        //the new method queue, and flashes the advisor
        newMessages.Add(messages[msgName]);

        //if this new message is the only one, overwrite the "No new messages warning"
        if (newMessages.Count == 1)
        {
            speechText.text = newMessages[0];
        }

        //set newMessage flag and flash the advisor
        if (newMessage == false)
        {
            newMessage = true;
            StartCoroutine(FlashAdvisor());
        }
    }

    private IEnumerator FlashAdvisor()
    {
        //this method flahes the advisor text label
        //red while there is still an unread message
        while (newMessage)
        {
            if (advisorText.color == Color.black)
                advisorText.color = Color.red;
            else
                advisorText.color = Color.black;

            yield return new WaitForSeconds(1);
        }
    }

    public void OnClick()
    {
        //this method is called when the advisor button is clicked
        //and resets the new message bool, opens the advisor's 
        //speech panel to the message at the current index
        if (!textPanelOpen)
        {
            textPanelOpen = true;
            advisorText.color = Color.black;
            speechPanel.SetActive(true);
            speechText.text = newMessages[currentMessageIndex];
        }

        else
        {
            newMessage = false;
            advisorText.color = Color.black;
            speechPanel.SetActive(false);
            textPanelOpen = false;
        }
    }

    public void NextMessage()
    {
        if (currentMessageIndex < newMessages.Count -1)
        {
            currentMessageIndex++;
            speechText.text = newMessages[currentMessageIndex];
            Debug.Log(currentMessageIndex);
        }

        //if this message is the last new message, stop advisor from flashing
        if (currentMessageIndex == newMessages.Count -1)
        {
            newMessage = false;
            advisorText.color = Color.black;
        }
    }

    public void PrevMessage()
    {
        if (currentMessageIndex > 0)
        {
            currentMessageIndex--;
            speechText.text = newMessages[currentMessageIndex];
            Debug.Log(currentMessageIndex);
        }
    }

    public void DeleteMessage()
    {
        //this method is called when the delete message button is clicked
        //and handles removing messages from the newMessages list

        //clear the list and reset the counter is the shift key is held
        if (Input.GetKey(KeyCode.LeftShift)|Input.GetKey(KeyCode.RightShift))
        {
            #region tutorial
            //end the tutorial, if it is in progress
            if (inTutorial)
                inTutorial = false;
            #endregion

            newMessages.Clear();

            //stop advisor from flashing
            newMessage = false;
            advisorText.color = Color.black;
            
            currentMessageIndex = 0;
            speechText.text = "No New Messages";
            return;
        }

        //otherwise, if there are messages, remove only the current message from the list
        if (newMessages.Count > 0)
        {
            newMessages.RemoveAt(currentMessageIndex);          

            //if at the end of the list, move the index counter back accordingly
            if (currentMessageIndex > newMessages.Count -1)
            {
                currentMessageIndex = newMessages.Count -1;
            }

            //otherwise, display the new message at the current index
            if (newMessages.Count > 0)
                speechText.text = newMessages[currentMessageIndex];

            //display default text if there are no new messages
            else
            {
                speechText.text = "No New Messages";
                //stop advisor from flashing
                newMessage = false;
                advisorText.color = Color.black;
            }
        }
    }

    public void ShowLaunch()
    {
        //this message is called from Main() when the player has completed their objectives
        //and replaces the objectives panel with the launch button that ends the round
        goalsPanel.SetActive(false);
        launchButton.SetActive(true);
    }

    private void SetMessages()
    {
        //this method is called in Start() and adds all
        //the possible advisor messages to the message dictionary
        messages.Add("Welcome", "Hello and welcome to your new position as Admiral of the Resource Aquisition Fleet. My name is Mack and I'll be acting as your advisor during your stay in this star system");
        messages.Add("Tutorial1", "Your mission is to acquire resources from this system before shipping out to another. But remember, building more ships costs resources.");
        messages.Add("Tutorial2", "Clicking the X icon in this panel will delete the current message. If you want to clear out your message log (or skip this tutorial), shift- click on the X icon.");
        messages.Add("Tutorial3", "Why don't you start by mining some IRON? A rocky planet would be perfect for that. Use the left and right arrows to move between planets.");
        messages.Add("TutorialNP", "Click on the planet to bring up the menu, then click on the E to send an exploration satellite.");
        messages.Add("TutorialVENUS", "The atmosphere here is comprised mostly of carbon dioxide, not what we're looking for. Let's search further out.");
        messages.Add("TutorialVENUS2", "The atmosphere here is too acidic for us to send any mining robots to the surface. Let's look for something a little more hospitable.");
        messages.Add("TutorialROCK", "Great! This planet has plenty of IRON. Click the M to begin the mining process.");
        messages.Add("TutorialROCK2", "Hmm, this is another rocky planet and doesn't have much HYDROGEN. Let's try another one.");
        messages.Add("TutorialROCK3", "Okay, we still need some IRON. Let's look for a rocky planet again.");
        messages.Add("TutorialMINE", "Now click a spot on the planet's surface to send out a mining robot.");
        messages.Add("TutorialMINE2", "Good. You are now collecting IRON from this planet. IRON is used to make more space craft.");
        messages.Add("TutorialGAS1","Hmm, it looks like this planet is mostly gas and doesn't have a solid core. That's ok, we can syphon HYDROGEN from it. Click the S to continue.");
        messages.Add("TutorialGAS2", "Great! This planet has plenty of HDROGEN. Click on the S to begin syphoning it.");
        messages.Add("TutorialGAS3","That's it. You are now syphoning HYDROGEN from this planet. HYDROGEN can be used as fuel to move the spacecraft around.");
        messages.Add("TutorialGAS4", "Now let's find a source of HYDROGEN. Try to find a large, gas planet.");
        messages.Add("TutorialGAS5", "Hmm, this is another gas planet with no solid core. Try to find a small rocky planet. Look closer to the sun.");
        messages.Add("TutorialSATURN", "The atmosphere on this planet is mostly carbon dioxid with some sulfur. Not quite what we're looking for. Keep looking.");
        messages.Add("TutorialCASH","If you're ever short on money, you can buy or sell resources by clicking on the market panel.");
        messages.Add("TutorialEND","It looks like you can handle things from here. I'll let you get to work, but I'll keep an eye out in case you need any assistance.");
        messages.Add("GoalsMet", "Congratulations! You've met your objective. You can continue in this system or click the \"Launch\" button to go to another system.");
    }
}
