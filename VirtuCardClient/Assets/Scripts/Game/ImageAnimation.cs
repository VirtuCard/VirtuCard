using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageAnimation : MonoBehaviour
{
    public RectTransform logo;
    // public GameObject check;
    public CanvasGroup checking;
    private float width = 600;
    private float height = 600;
    // Start is called before the first frame update
    void Start()
    {
        logo.sizeDelta = new Vector2(600, 600);
    }

    // Update is called once per frame
    void Update()
    {
        if (checking.GetComponent<CanvasGroup>().alpha == 0) {
            height = 600;
            width = 600;
        }
        else {
            height += 75 * Time.deltaTime;
            width += 75 * Time.deltaTime;
            logo.sizeDelta = new Vector2(width, height);
        }
    }
}
