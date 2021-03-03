using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScreenController : MonoBehaviour
{
    public GameObject chatPanel;
    public Toggle chatToggle;

    // Start is called before the first frame update
    void Start()
    {
        chatToggle.SetIsOnWithoutNotify(HostData.isChatAllowed());
        chatToggle.onValueChanged.AddListener(delegate { ChatToggleValueChanged(chatToggle.isOn); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void ChatToggleValueChanged(bool toggleVal)
    {
        chatPanel.SetActive(!toggleVal);
    }
}
