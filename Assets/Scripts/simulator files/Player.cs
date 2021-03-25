using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("NPC"))
                {
                    DialogueManager.Instance.NPCInteraction(hit.collider.gameObject.name);
                }
            }
            else 
            {
                DialogueManager.Instance.TurnOffDialogueTextBox();
            }
        }
    }

    public void CompleteQuest()//pressing the completed quest button to sim this situation happening in the game
    {
        DialogueManager.Instance.StopAllCoroutines();
        DialogueManager.Instance.CompletedQuestBool = true;
        DialogueManager.Instance.getQuestBool = false;
    }

    public void CompleteQuestPeter()
    {
        CompleteQuest();
        DialogueManager.Instance.NPCQuestCompleted("Peter");
    }

    public void CompleteQuestFlynn()
    {
        CompleteQuest();
        DialogueManager.Instance.NPCQuestCompleted("Flynn");
    }

    public void CompleteQuestPoppy()
    {
        CompleteQuest();
        DialogueManager.Instance.NPCQuestCompleted("Poppy");
    }
}
