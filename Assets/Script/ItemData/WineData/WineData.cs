using UnityEngine;

[CreateAssetMenu(fileName = "WineData", menuName = "Scriptable Objects/WineData")]
public class WineData : ScriptableObject
{
    
    public WineType wineType;


    public enum WineType
    {
        Empty,
        Piquette,
        Clairet,
        Chateau,
        TurnedWine

    }
}