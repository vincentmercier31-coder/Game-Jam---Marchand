using UnityEngine;

[CreateAssetMenu(fileName = "ClientData", menuName = "Scriptable Objects/ClientData")]
public class ClientData : ScriptableObject
{
    public GameObject clientPrefab;
    public int scoreAmount;
    public Sprite bottleImage;
    public PickableItem PickableItem; // La bouteille que le client veut
    public Mesh clientMesh;
}
