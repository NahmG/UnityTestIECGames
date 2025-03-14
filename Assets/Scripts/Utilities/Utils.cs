using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using URandom = UnityEngine.Random;

public class Utils
{
    public static Item.eType GetRandomNormalType()
    {
        Array values = Enum.GetValues(typeof(Item.eType));
        Item.eType result = (Item.eType)values.GetValue(URandom.Range(0, values.Length));

        return result;
    }

    public static Item.eType GetRandomNormalTypeExcept(Item.eType[] types)
    {
        List<Item.eType> list = Enum.GetValues(typeof(Item.eType)).Cast<Item.eType>().Except(types).ToList();

        int rnd = URandom.Range(0, list.Count);
        Item.eType result = list[rnd];

        return result;
    }

    public static void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = URandom.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}
