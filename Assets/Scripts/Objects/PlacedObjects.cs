using UnityEngine;

public class PlacedObject : MonoBehaviour
{
    public BuildingEntry Entry;
    IBuildable _buildable;
    public int TileId;

    public void Init(BuildingEntry entry, int tileId)
    {
        Entry = entry;
        TileId = tileId;

        TryGetComponent(out _buildable);
    }

    public IStatData GetData()
    {
        return _buildable?.GetData();
    }
}
