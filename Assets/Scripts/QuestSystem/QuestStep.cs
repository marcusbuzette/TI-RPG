using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestStep : MonoBehaviour {

    private bool isFinished = false;

    protected void FinishQuestStep() {
        if (!this.isFinished) isFinished = true;



        Destroy(this.gameObject);
    }
}
