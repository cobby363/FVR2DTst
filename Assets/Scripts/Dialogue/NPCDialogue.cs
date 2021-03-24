using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCDialogue //Objects Attributes
{
    //Attributes found in Json file relating to NPC Dialogues
    public string Name;
    public string Description;
    public string Building;
    public string Pastry;
    public string Introduction;
    public string Acquaintance1;
    public string Acquaintance2;
    public string Friend1;
    public string Friend2;
    public string Friend3;
    public string Election;
    public string BestFriend1;
    public string BestFriend2;
    public string BuildingQ;
    public string BuildingComplete;
    public string NPC;
    public string Job;
    public string Quest;
    public string QuestComplete;

    public int rLevel = 0; //Create attribute storing Players Relationship Level to the NPC

    //public static string CheckRLevel(string name, Dialogues dialogues)
    //{
    //    string dialogue = null;
    //    foreach(NPCDialogue npc in npcDialogues.dialogues)
    //    {
    //        if (name.Equals(npc.Name))
    //        {
    //            Debug.Log($"Name: {npc.Name}, Relationship Level: {npc.rLevel}");
    //            switch (npc.rLevel) //Selected dialogue based on relationship level [ >=5 cases, compiler should convert to JumpTable @ compile time?]
    //            {
    //                case 0: { dialogue = npc.Introduction; break; }
    //                case 1: { dialogue = npc.Acquaintance1; break; }
    //                case 2: { dialogue = npc.Acquaintance2; break; }
    //                case 3: { dialogue = npc.Friend1; break; }
    //                case 4: { dialogue = npc.Friend2; break; }
    //                case 5: { dialogue = npc.Friend3; break; }
    //                case 6: { dialogue = npc.BestFriend1; break; }
    //                case 7: { dialogue = npc.BestFriend2; break; }
    //                case 8: { dialogue = npc.BuildingQ; break; }
    //                case 9: { dialogue = npc.BuildingComplete; break; }
    //                case 10: { dialogue = npc.Quest; break; }
    //                case 11: { dialogue = npc.QuestComplete; break; }
    //                default: { dialogue = "End Of Dialogue Switch"; npc.rLevel = 0; break; }
    //            }
    //        }
    //        break;
    //    }
    //    return dialogue;
    //}

    public override string ToString()
    {
        return string.Format($"Name: {Name}, Description: {Description}, Building: {Building}, Pastry: {Pastry},\n" +
            $" Introduction: {Introduction}, Acquaintance1: {Acquaintance1}, Acquaintance2: {Acquaintance2}, Friend1: {Friend1},\n" +
            $" Friend2: {Friend2}, Friend3: {Friend3}, Election: {Election}, BestFriend1: {BestFriend1},\n" +
            $" BestFriend2: {BestFriend2}, BuildingQ: {BuildingQ}, BuildingComplete: {BuildingComplete}, NPC: {NPC},\n" +
            $" Job: {Job}, Quest: {Quest}, QuestComplete: {QuestComplete}");
    }

    public string NPCDetails()
    { 
        return string.Format($"NPC Name: {Name}, \nNPC Description: {Description}, \nHome Building: {Building}, \nFavorite Pastry: {Pastry}");
    }
}
