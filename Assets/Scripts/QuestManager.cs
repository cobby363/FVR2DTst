using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//work on setting bools when done, friend level in QM, look into callbacks

public class QuestManager : MonoBehaviour
{
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

    private void Start()
    {
        

        QuestBox.SetActive(false);
        npcQuestInfo = JsonUtility.FromJson<QuestInfo>(QuestJsonFile.text);
        foreach (NPCQuest quest in npcQuestInfo.questInfo)
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

    

    public void QuestReplyForEachLevel(string CharName)
    {
        foreach (NPCQuest quest in npcQuestInfo.questInfo)
        {
            if (CharName == quest.Name)
            {
                switch (DialogueManager.Instance.RelationshipDictionary[CharName].Level)
                {
                    case 1:
                        {
                            DialogueManager.Instance.NPCReplyText = (CharName + ": " + quest.Aquaintance);
                            QuestSummaryListToDisplay.Add(CharName + ": " + questSummaries[CharName].ASummary);
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
                DialogueManager.Instance.NewItemforDictionary(CharName);
                QuestBox.SetActive(true);
                updateText();
                StartCoroutine(DialogueManager.Instance.AnimateText());
                break;
            }

        }
    }

    public void updateText()
    {
        ActiveQuests.text = string.Empty;

        for (int i = 0; i < QuestSummaryListToDisplay.Count; i++)
        {
            ActiveQuests.text += QuestSummaryListToDisplay[i] + "\n";
        }
    }

    public void RemoveFromList(string CharName, int RelLevel)
    {
        Debug.Log("Charname: " + CharName + ", rel = " + RelLevel);
        Quest newQuestToAdd = new Quest
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
                    QuestSummaryListToDisplay.Remove(summary);
                    newQuestToAdd.AQuestComplete = true;
                    updateText();
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
        questSummaries[CharName] = newQuestToAdd;
    }



}
