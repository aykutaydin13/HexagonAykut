using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BombCounterSprite : MonoBehaviour
{
    public static int bombCounter = 7;
    TextMeshProUGUI bombText;
    // Start is called before the first frame update
    void Start()
    {
        bombText = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        bombText.text = "Bomb Counter: " + bombCounter;
    }
}
