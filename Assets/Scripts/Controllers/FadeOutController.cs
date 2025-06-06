using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOutController : MonoBehaviour
{
    [SerializeField] private FadingScript fadingScript;
    // Start is called before the first frame update
    void Start()
    {
        fadingScript.FadeOut();
    }

}
