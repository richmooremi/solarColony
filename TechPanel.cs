using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechPanel : MonoBehaviour {

    private Main main;

    bool extended = false;              //true when the panel is on the screen
    //public Button panelButton;        //button object used to collapse and expand the planet panel

    public Text upgradeCostText;        //used to display the cost of the current upgrade
    private int upgradecost = 100;      

    private void Start()
    {
        main = GameObject.FindObjectOfType<Main>();

        upgradeCostText.text = "Cost:\n" + upgradecost;
    }

    public void movePanel()
    {
        //this method is used to collapse and expand the planet panel

        //if the panel is collapsed, move into the expanded position
        if (!extended)
        {
            //panelButton.transform.localEulerAngles = new Vector3(0, 0, 180);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(80.9f, 107.7f);
            extended = true;
        }

        //if the panel is expanded, move back into the collapsed position
        else
        {
            //panelButton.transform.localEulerAngles = new Vector3(0, 0, 0);
            this.GetComponent<RectTransform>().anchoredPosition = new Vector2(80.9f, 131f);
            extended = false;
        }

    }

    public void techIncreaseClick(int i)
    {
        if (true && (main.getTechLevel() == i))  
        {
            if (Main.playerResources[2] < upgradecost)
            {
                main.sendMiniMessage("You don't have enough money to buy level " + (main.getTechLevel()));
                return;
            }

            //upgrade player tech level and gray out this button
            main.setTechLevel(i+1);
            Image img = GameObject.Find("TechButton" + i).GetComponent<Image>();
            img.GetComponent<Image>().color = new Vector4(img.color.r, img.color.b, img.color.g, 0.5f);

            Main.playerResources[2] -= upgradecost;

            upgradecost = calulateUpgradeCost();

            return;
        }

        else if (main.getTechLevel() < i)
        {
            main.sendMiniMessage("You must first buy level " + (main.getTechLevel()) );
            return;
        }

        else if (main.getTechLevel() > i)
        {
            main.sendMiniMessage("You already bought this upgrade");
            return;
        }
    }

    private int calulateUpgradeCost()
    {
        if (main.getTechLevel() != 6)
        {
            int v = upgradecost * 2;
            upgradeCostText.text = "Cost:\n" + v;
            return v;
        }

        else
            upgradeCostText.text = "Cost:\n N/A";

        return -1;
    }
}
