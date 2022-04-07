using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImageSwap : MonoBehaviour
{
    [SerializeField]
    Sprite MobileImage;

    // Start is called before the first frame update
    void Start()
    {
 #if UNITY_IPHONE
        this.GetComponent<SpriteRenderer>().sprite = MobileImage;
 #endif
 #if UNITY_ANDROID
        this.GetComponent<SpriteRenderer>().sprite = MobileImage;
 #endif
    }
}
