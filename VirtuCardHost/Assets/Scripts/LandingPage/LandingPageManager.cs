using FirebaseScripts;
using UnityEngine;
using UnityEngine.UI;

public class LandingPageManager : MonoBehaviour
{
    // this dropdown should be populated with all the game names
    public Dropdown gameChoiceDropdown;

    // this is the UI toggle component that allows the host to join or not
    public Toggle canHostJoinToggle;

    // this is the button that is pressed when a user wants to create a game
    public Button createGameBtn;

    // Start is called before the first frame update
    void Start()
    {
        // add event listener for the value changing
        canHostJoinToggle.onValueChanged.AddListener(
            delegate { CanHostJoinToggleValueChanged(canHostJoinToggle.isOn); });

        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Freeplay"));
        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Uno"));
        gameChoiceDropdown.options.Add(new Dropdown.OptionData("Go Fish"));
        //Console.WriteLine("HELLLLLOOOOO");


        
        // FirebaseInit.InitializeFirebase(task =>
        // {
        //     AuthUser.RegisterAccount("testing", "hello@ohio.edu", "topsecret",
        //         task => { Debug.Log("Hi " + task); 
        //         if (AuthUser.Login("testing", "topsecret")) {
        //                 Debug.Log("True");
        //             }
        //             else {
        //                 Debug.Log("False");
        //             }
        //         });
        // });

        // FirebaseInit.InitializeFirebase(task =>
        // {
        //     AuthUser.Login("seodongjune00@gmail.com", "poop",
        //         task => {
        //         Debug.Log("hello " + task);
        //         if (task) {
        //             Debug.Log("TRUE JUNIBEAR");
        //             //Console.WriteLine("TRUE JUNIBEAR");
        //         }
        //         else {
        //             Debug.Log("FALSE JUNIEBEAR invalid credentials");
        //             //Console.WriteLine("FALSE JUNIBEAR");
        //         }
        //     });

        // });
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    /// <summary>
    /// This is the callback for when the canHostJoinToggle's value is changed.
    /// It sets the corresponding boolean in the HostData class
    /// </summary>
    /// <param name="state"></param>
    private void CanHostJoinToggleValueChanged(bool state)
    {
        HostData.setCanHostJoinGame(state);
    }
}