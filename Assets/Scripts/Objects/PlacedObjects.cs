using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public BuildingEntry Entry;
    public int TileId;

    public void Init(BuildingEntry entry, int tileId)
    {
        Entry = entry;
        TileId = tileId;
    }
}
