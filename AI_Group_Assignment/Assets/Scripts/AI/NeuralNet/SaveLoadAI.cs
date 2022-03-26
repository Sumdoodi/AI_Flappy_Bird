using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Serialization;


public class AIData
{
    public int test;

    public AIData(int test)
    {
        this.test = test;
    }
}

public class SaveLoadAI
{
    public static void Save(AIData data)
    {
        //string dt = DateTime.Now.ToString("MM-dd-yyyy_HH-mm-ss");
        string rawJson = JsonUtility.ToJson(data);
        //File.WriteAllText($"AIData_{dt}.json", rawJson);
        File.WriteAllText("AIData.json", rawJson);
    }

    public static AIData Load()
    {
        string rawJson = File.ReadAllText("AIData.json");
        return JsonUtility.FromJson<AIData>(rawJson);
    }
}
