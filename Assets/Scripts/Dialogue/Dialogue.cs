using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Dialogue")]
public class Dialogue : ScriptableObject {

    [Serializable] public struct DialogueStruct {
        [TextArea(1, 3)]
        public string sentences;
        public string name;
        public Sprite image;
    }

    public DialogueStruct[] dialogue;


    
    
}
