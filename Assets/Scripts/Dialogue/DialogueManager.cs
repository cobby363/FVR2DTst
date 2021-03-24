using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    #region Instance
    //creaing an Instance
    private static DialogueManager instance;

    public static DialogueManager Instance
    {
        get
        {
            return instance;
        }
    }
    #endregion

    #region  Global Variables
    [SerializeField] public GameObject ReplyBox;
    [SerializeField] float TextSpeed = 0.5f;
    [SerializeField] AudioSource audioSource;
    Dialogues npcDialogue;

    public int currentlyDisplayingText = 0;
    public TextAsset jsonFile;
    public string NPCReplyText = null;
    public Text textBox;

    string[] NPCReplies = new string[3];

    public struct RelationshipDetails
    {
        public int Level { get; set; }
        public bool QuestGiven { get; set; }
        public bool CurrentQuestCompleted { get; set; }
    }

    public Dictionary<string, RelationshipDetails> RelationshipDictionary = new Dictionary<string, RelationshipDetails>();
    #endregion

    #region Opening Methods
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
        npcDialogue = JsonUtility.FromJson<Dialogues>(jsonFile.text); //Convert Json Data into Dialogues Array
        ReplyBox.SetActive(false); //disable the canvas from displaying
        foreach (NPCDialogue npc in npcDialogue.dialogues) //Foreach object within' Json File, add an npc into a dictionary
        {
            RelationshipDetails ThrowMeIn = new RelationshipDetails { Level = 0, QuestGiven = false, CurrentQuestCompleted = false };
            RelationshipDictionary.Add(npc.Name, ThrowMeIn);
        }
    }
    #endregion

    public void NPCInteraction(string CharName) //method ran whenever the player interacts with any NPC. 
    {
        StopAllCoroutines(); //stop text from typing if there is any currently running
        if (!RelationshipDictionary[CharName].QuestGiven) // if no quest has been given
            if (RelationshipDictionary[CharName].Level == 0)//for the intro, give the general intro followed by increasing the NPC's level of friendship
            {
                GeneralDialogueReply(CharName);
                IncreaseNPCLevel(CharName);
            }
            else
                QuestManager.Instance.QuestReplyForEachLevel(CharName); //give a quest per the level of friendship
        else if (RelationshipDictionary[CharName].QuestGiven && !RelationshipDictionary[CharName].CurrentQuestCompleted)//if a quest has been given out but not yet completed, give a general piece of dialogue based on the friendship level
            GeneralDialogueReply(CharName);
        else if (RelationshipDictionary[CharName].QuestGiven && RelationshipDictionary[CharName].CurrentQuestCompleted)//if the quest has been given out and completed, thank the player with the correct dialogue
            NPCQuestCompletionReply(CharName);
    }

    public NPCDialogue GetNPCDetails(string npcName) //Single method for getting npc from array
    {
        foreach (NPCDialogue npc in npcDialogue.dialogues)
        {
            if (npc.Name.Equals(npcName)) //If NPC interacted with is present in the array
                return npc; //Return the NPC's details
        }
        return null; //return null if npc not found
    }

    #region Populate the dialogue reply box methods for general replies
    
    public void GeneralDialogueReply(string CharName)
    {
        Debug.Log(CharName); //for testing purposes
        switch (RelationshipDictionary[CharName].Level) //checking to see what relationship level the player has with the specific NPC
        {
            case 0://if relationship is at 0 (intro stage and never met)
                IfNPCIsOnTheirIntroduction(CharName);
                break;
            case 1://if Acquaintance
                IfNPCIsAcquaintance(CharName);
                break;
            case 2://if NPCand player are friends
                IfNPCIsFriend(CharName);
                break;
            case 3://if they're best friends
                ifNPCIsBestFriend(CharName);
                break;
        }
        StartCoroutine(AnimateText());//start the typewriter effect
    }

    public void IfNPCIsOnTheirIntroduction(string CharName)
    {
        string response = GetNPCDetails(CharName).Introduction; //Get introduction dialogue
        NPCReplyText = $"{CharName} : {response}";//When they meet the first time, read out the introduction
    }

    public void IfNPCIsAcquaintance(string CharName)
    {
        if (GetNPCDetails(CharName).Acquaintance2 != "") // included due to Flynn only having one acquaintence reply
        {
            NPCReplies[0] = GetNPCDetails(CharName).Acquaintance1;//add the different replies into an array for randomness
            NPCReplies[1] = GetNPCDetails(CharName).Acquaintance2;//add the different replies into an array for randomness
            NPCReplyText = (GetNPCDetails(CharName).Name + ": " + NPCReplies[UnityEngine.Random.Range(0, 2)]);//choose a random one of the two possible replies
        }
        else
            NPCReplyText = (GetNPCDetails(CharName).Name + ": " + GetNPCDetails(CharName).Acquaintance1);

    }

    public void IfNPCIsFriend(string CharName)
    {
        if (CharName == GetNPCDetails(CharName).Name)
        {
            NPCReplies[0] = GetNPCDetails(CharName).Friend1;//add the different replies into an array for randomness
            NPCReplies[1] = GetNPCDetails(CharName).Friend2;
            NPCReplies[2] = GetNPCDetails(CharName).Friend3;
            NPCReplyText = (GetNPCDetails(CharName).Name + ": " + NPCReplies[UnityEngine.Random.Range(0, 3)]);//choose a random one of the three possible replies

        }
    }

    public void ifNPCIsBestFriend(string CharName)
    {
        if (CharName == GetNPCDetails(CharName).Name)
        {
            NPCReplies[0] = GetNPCDetails(CharName).BestFriend1;//add the different replies into an array for randomness
            NPCReplies[1] = GetNPCDetails(CharName).BestFriend2;
            NPCReplyText = (GetNPCDetails(CharName).Name + ": " + NPCReplies[UnityEngine.Random.Range(0, 2)]);//choose a random one of the two possible replies

        }
    }

    #endregion

    #region Change the details in the dictionary
    public void NewItemforDictionary(string CharName) //whenevr a quest has been given out
    {
        RelationshipDetails ThrowMeIn = new RelationshipDetails { Level = RelationshipDictionary[CharName].Level, QuestGiven = true, CurrentQuestCompleted = false };
        RelationshipDictionary[CharName] = ThrowMeIn;
    }

    public void NPCQuestCompleted(string CharName) //whenevr a quest has been given and completed
    {
        RelationshipDetails ThrowMeIn = new RelationshipDetails { Level = RelationshipDictionary[CharName].Level, QuestGiven = true, CurrentQuestCompleted = true };
        RelationshipDictionary[CharName] = ThrowMeIn;
    }

    public void NPCQuestReset(string CharName) //whenevr a quest has been completed and needs to be reset
    {
        RelationshipDetails ThrowMeIn = new RelationshipDetails { Level = RelationshipDictionary[CharName].Level, QuestGiven = false, CurrentQuestCompleted = true };
        RelationshipDictionary[CharName] = ThrowMeIn;
    }

    public void IncreaseNPCLevel(string npcName) //Increment relationship level of NPC
    {
        RelationshipDictionary[npcName] = new RelationshipDetails //Overwrite info relating to npc relationship level
        {
            Level = RelationshipDictionary[npcName].Level + 1, //current relationship level +1 
            QuestGiven = RelationshipDictionary[npcName].QuestGiven, //Retain current value
            CurrentQuestCompleted = RelationshipDictionary[npcName].CurrentQuestCompleted //Retain current value
        };
    }

    #endregion


    public void NPCQuestCompletionReply(string CharName)//if the quest completed bool has been set to true run this
    {
        foreach (NPCQuest quest in QuestManager.Instance.npcQuestInfo.questInfo)
        {
            if (CharName == quest.Name)
            {
                if (RelationshipDictionary[CharName].Level == 1)
                    NPCReplyText = (quest.Name + ": " + quest.AquaintanceComplete);
                else if (RelationshipDictionary[CharName].Level == 2)
                    NPCReplyText = (quest.Name + ": " + quest.FriendComplete);
                else if (RelationshipDictionary[CharName].Level == 3)
                    NPCReplyText = (quest.Name + ": " + quest.BestFriendComplete);
                else
                    NPCReplyText = "Invalid text";
                QuestManager.Instance.RemoveFromList(CharName, RelationshipDictionary[CharName].Level);
                if (!StringIncludesLetters(QuestManager.Instance.ActiveQuests.text))
                {
                    QuestManager.Instance.ActiveQuests.text = QuestManager.Instance.ActiveQuests.text.Replace("\n", string.Empty); //remove from the quest summary box whatever that npc's summary said
                }
            }
            if (QuestManager.Instance.ActiveQuests.text == string.Empty) //if there's nothing in the text box, hide it
                QuestManager.Instance.QuestBox.SetActive(false);
        }
        if (RelationshipDictionary[CharName].Level < 3)
        {
            IncreaseNPCLevel(CharName); //incriment the npc level if below 3 (best friend level)
        }
        NPCQuestReset(CharName);
        StartCoroutine(AnimateText()); //start the text animation
    }

    #region Getter Methods
    public string GetRelationshipLevel(string NPCName)
    {
        string relationshipLevel = null;
        switch (RelationshipDictionary[NPCName].Level)
        {
            case 0:
                relationshipLevel = "Introduction";
                break;
            case 1:
                relationshipLevel = "Acquaintance";
                break;
            case 2:
                relationshipLevel = "Friend";
                break;
            case 3:
                relationshipLevel = "BestFriend";
                break;
        }
        return relationshipLevel;
    }

    public bool GetQuestGiven(string NPCName)
    {
        return RelationshipDictionary[NPCName].QuestGiven;
    }

    #endregion

    public void TurnOffDialogueTextBox() //use this whgen you want the text box to disappear, potentially when the player clicks away from the NPC
    {
        NPCReplyText = null;
        ReplyBox.SetActive(false);
        StopAllCoroutines();
    }

    private static bool StringIncludesLetters(String str) //check to see if there's any alphabet text left in the textbox. This therefore ignores spaces and "\n"
    {
        return QuestManager.Instance.ActiveQuests.text.Any(x => char.IsLetter(x));
    }

    public IEnumerator AnimateText() //typewriter effect
    {
        ReplyBox.SetActive(true);
        for (int i = 0; i < (NPCReplyText.Length + 1); i++)
        {
            if (i < (NPCReplyText.Length))
            {
                textBox.text = NPCReplyText.Substring(0, i) + "|";
            }
            else
            {
                textBox.text = NPCReplyText.Substring(0, i);
            }
            audioSource.Play();

            yield return new WaitForSeconds(TextSpeed);
        }
    }


    #region SimulationOnly

    #region buttonBools
    public bool getQuestBool = false;
    public bool CompletedQuestBool = false;
    public bool JobBool = false;
    public bool ElectionBool = false;
    public bool BuildingQBool = false;
    public bool BuildingCompleteBool = false;
    #endregion

    public void SimulatorWithButtons(string CharName)
    {
        foreach (NPCDialogue npc in npcDialogue.dialogues) //Foreach object within' Json File
        {
            if (CharName == npc.Name)
            { //check if a button has been used and therefore need to set the reply to this first
                if (getQuestBool)
                    NPCReplyText = npc.Quest;
                else if (CompletedQuestBool)
                    NPCQuestCompleted(CharName);
                else if (JobBool)
                    NPCReplyText = npc.Job;
                else if (ElectionBool)
                    NPCReplyText = npc.Election;
                else if (BuildingQBool)
                    NPCReplyText = npc.BuildingQ;
                else if (BuildingCompleteBool)
                    NPCReplyText = npc.BuildingComplete;
                StartCoroutine(AnimateText());//start the typewriter effect
                break;//stop the foreach loop continuing after we've found our target npc
            }
        }
    }
    #endregion
}

