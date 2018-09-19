using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechPanel : MonoBehaviour {

    private Main main;

    bool extended = false;              //true when the panel is on the screen
    public Button panelButton;          //button object used to collapse and expand the planet panel

    private void Start()
    {
        main = GameObject.FindObjectOfType<Main>();
    }

    public void movePanel()
    {
        //this method is used to collapse and expand the planet panel

        //if the panel is collapsed, move into the expanded position
        if (!extended)
        {
            panelButton.transform.localEulerAngles = new Vector3(0, 0, 180);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(51.10001f, 107.4f);
            extended = true;
        }

        //if the panel is expanded, move back into the collapsed position
        else
        {
            panelButton.transform.localEulerAngles = new Vector3(0, 0, 0);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(51.10001f, 133.8f);
            extended = false;
        }

    }

    public void techIncreaseClick(int i)
    {
        if (true && (main.getTechLevel() + 1 == i))   //replace this with a ca afford function RM//
        {
            //upgrade player tech level and gray out this button
            main.setTechLevel(i);
            Image img = GameObject.Find("TechButton" + i).GetComponent<Image>();
            img.GetComponent<Image>().color = new Vector4(img.color.r, img.color.b, img.color.g, 0.5f);
        }

        else
        {
            main.sendMiniMessage("You must first buy level " + (main.getTechLevel() + 1) );
        }
    }
}
