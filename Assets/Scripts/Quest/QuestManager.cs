using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestManager : MonoBehaviour
{
    #region Global Variables
    [SerializeField] public GameObject QuestBox = null;
    [SerializeField] public Text ActiveQuests = null;
    public TextAsset QuestJsonFile;
    public QuestInfo npcQuestInfo;
    public Dictionary<string, Quest> questSummaries = new Dictionary<string, Quest>();
    public List<string> QuestSummaryListToDisplay = new List<string>();
    public struct Quest
    {
        public string ASummary;
        public string FSummary;
        public string BFFSummary;
        public bool AQuestComplete;
        public bool FQuestComplete;
        public bool BFFQuestComplete;
    }
    #endregion

    #region Instance
    //creaing an Instance
    private static QuestManager instance;

    public static QuestManager Instance
    {
        get
        {
            return instance;
        }
    }

    void Awake()
    {
        //ensure that there's one and only one instance
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    private void Start()
    {
        QuestBox.SetActive(false); //hide the quest summary box until it's needed
        npcQuestInfo = JsonUtility.FromJson<QuestInfo>(QuestJsonFile.text); //convert json to an array
        foreach (NPCQuest quest in npcQuestInfo.questInfo)//for all the npc's in the array, add them into a dictionary to be able to access what's needed and add them into a list instead of multiple for eachs
        {
            Quest newQuestToAdd = new Quest
            {
                ASummary = quest.ASummary,
                FSummary = quest.FSummary,
                BFFSummary = quest.BFFSummary,
                AQuestComplete = false,
                FQuestComplete = false,
                BFFQuestComplete = false
            };
            questSummaries.Add(quest.Name, newQuestToAdd);
        }
    }

    public void QuestReplyForEachLevel(string CharName)//hand out quests to the player and populate the quest summary text box after adding to the list to display
    {
        foreach (NPCQuest quest in npcQuestInfo.questInfo)
        {
            if (CharName == quest.Name)
            {
                switch (DialogueManager.Instance.RelationshipDictionary[CharName].Level)//check the npc relationship level
                {
                    case 1:
                        {
                            DialogueManager.Instance.NPCReplyText = (CharName + ": " + quest.Aquaintance); //populate the reply box with a quest
                            QuestSummaryListToDisplay.Add(CharName + ": " + questSummaries[CharName].ASummary); //populate the list that will be displayed
                            break;
                        }
                    case 2:
                        {
                            DialogueManager.Instance.NPCReplyText = (CharName + ": " + quest.Friend);
                            QuestSummaryListToDisplay.Add(CharName + ": " + questSummaries[CharName].FSummary);
                            break;
                        }
                    case 3:
                        {
                            DialogueManager.Instance.NPCReplyText = (CharName + ": " + quest.BestFriend);
                            QuestSummaryListToDisplay.Add(CharName + ": " + questSummaries[CharName].BFFSummary);
                            break;
                        }
                    default:
                        {
                            DialogueManager.Instance.NPCReplyText = "Invalid text";
                            break;
                        }
                }
                DialogueManager.Instance.NewItemforDictionary(CharName); //say that a quest has been given
                QuestBox.SetActive(true);//show the quest summary box
                updateText();//update the text to show the list
                StartCoroutine(DialogueManager.Instance.AnimateText()); //run the method to animate text in the reply box
                break;
            }

        }
    }

    public void updateText() //update the text box to display the list of summaries. If there's more than one then leave a line in between
    {
        ActiveQuests.text = string.Empty;

        for (int i = 0; i < QuestSummaryListToDisplay.Count; i++)
        {
            ActiveQuests.text += QuestSummaryListToDisplay[i] + "\n";
        }
    }

    public void RemoveFromList(string CharName, int RelLevel) //whenever a player has finished a quest then remove it from the list, and update the dictionary to say that it has been completed
    {
        Debug.Log("Charname: " + CharName + ", rel = " + RelLevel);
        Quest newQuestToAdd = new Quest //ready to update the dictionary if there's been any changes such as a quest being completed
        {
            ASummary = questSummaries[CharName].ASummary,
            FSummary = questSummaries[CharName].FSummary,
            BFFSummary = questSummaries[CharName].BFFSummary,
            AQuestComplete = questSummaries[CharName].AQuestComplete,
            FQuestComplete = questSummaries[CharName].FQuestComplete,
            BFFQuestComplete = questSummaries[CharName].BFFQuestComplete
        };
        foreach (string summary in QuestSummaryListToDisplay)
        {
            Debug.Log(summary);
            if (RelLevel == 1)
            {
                if (summary == (CharName + ": " + questSummaries[CharName].ASummary))
                {
                    QuestSummaryListToDisplay.Remove(summary); //remove from list
                    newQuestToAdd.AQuestComplete = true; //set the bool for completion per level to true
                    updateText(); //update the text box to show the new updated list
                    break;
                }
            }
            else if (RelLevel == 2)
            {
                if (summary == (CharName + ": " + questSummaries[CharName].FSummary))
                {
                    QuestSummaryListToDisplay.Remove(summary);
                    newQuestToAdd.FQuestComplete = true;
                    updateText();
                    break;
                }
            }
            else if (RelLevel == 3)
            {
                if (summary == (CharName + ": " + questSummaries[CharName].BFFSummary))
                {
                    QuestSummaryListToDisplay.Remove(summary);
                    newQuestToAdd.BFFQuestComplete = true;
                    updateText();
                    break;
                }
            }
        }
        questSummaries[CharName] = newQuestToAdd; // update the dictionary
    }
}
