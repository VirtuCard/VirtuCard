using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LandingPageManager : MonoBehaviour
{
    // this dropdown should be populated with all the game names
    public Dropdown gameChoiceDropdown;

    // Start is called before the first frame update
    void Start()
    {
        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Freeplay"));
        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Uno"));
        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Go Fish"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
