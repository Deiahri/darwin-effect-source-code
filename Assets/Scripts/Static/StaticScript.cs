using System.Collections;
using System.Collections.Generic;
// using UnityEngine;

public class StaticScript
{
    static public List<float> foxData;
    static public List<float> bunnyData;
    static public bool musicOn = true;
    static public List<List<float>> foxSimData = new List<List<float>>{
        new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}
    };
    public static int foxDataCount = 0;
    static public List<List<float>> bunnySimData = new List<List<float>>{
        new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}
    };
    public static int bunnyDataCount = 0;

    public void assignFoxData(List<float> sentData) {
        foxData = sentData;
    }
    public void assignBunnyData(List<float> sentData) {
        bunnyData = sentData;
    }

    public static void addToFoxSimData(float num, float speed, float sight, float love, float longitivity, float beauty) {
        foxSimData[0].Add(num);
        foxSimData[1].Add(speed);
        foxSimData[2].Add(sight);
        foxSimData[3].Add(love);
        foxSimData[4].Add(longitivity);
        foxSimData[5].Add(beauty);
        foxDataCount++;
    }

    public static void addToBunnySimData(float num, float speed, float sight, float love, float longitivity, float beauty) {
        bunnySimData[0].Add(num);
        bunnySimData[1].Add(speed);
        bunnySimData[2].Add(sight);
        bunnySimData[3].Add(love);
        bunnySimData[4].Add(longitivity);
        bunnySimData[5].Add(beauty);
        bunnyDataCount++;
    }

    public static int getNumRabbits() {
        try {
            return (int)bunnySimData[0][bunnyDataCount-1];
        } catch {
            return 0;
        }
    }

    public static List<float> getData(string entityType, string dataType) {
        int index = -1;
        switch (dataType) {
            case "num":
                index = 0;
                break;
            case "speed":
                index = 1;
                break;
            case "sight":
                index = 2;
                break;
            case "love":
                index = 3;
                break;
            case "longitivity":
                index = 4;
                break;
            case "beauty":
                index = 5;
                break;
        }
        if(index == -1) {
            return null;
        }

        if(entityType == "Fox") {
            return foxSimData[index];
        } else if (entityType == "Rabbit") {
            return bunnySimData[index];
        } else {
            return null;
        }
    }

    public static void clearSimArrays() {
        foxSimData.Clear();
        bunnySimData.Clear();

        foxSimData = new List<List<float>>{
        new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}
    };
        bunnySimData = new List<List<float>>{
        new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}, new List<float>{}
    };
    }

    public static void toggleValue(string valueName, bool value) {
        switch (valueName) {
            case "music":
                musicOn = value;
                break;
        }
    }
}
