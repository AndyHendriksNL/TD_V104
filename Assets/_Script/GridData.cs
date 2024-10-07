using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridData
{
    Dictionary<Vector3Int, PlacementData> placedObjects = new();

    public void AddObjectAt(Vector3Int gridPosition,
                            Vector2Int objectSize,
                            int ID,
                            int placedObjectIndex,
                            Quaternion rotation)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize, rotation);
        PlacementData data = new PlacementData(positionToOccupy, ID, placedObjectIndex);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                throw new Exception($"Dictionary already contains this cell position {pos}");
            placedObjects[pos] = data;
        }
    }

    private List<Vector3Int> CalculatePositions(Vector3Int gridPosition, Vector2Int objectSize, Quaternion rotation)
    {
        List<Vector3Int> returnVal = new();
        Vector2Int rotatedSize = GetRotatedSize(objectSize, rotation);
        for (int x = 0; x < rotatedSize.x; x++)
        {
            for (int y = 0; y < rotatedSize.y; y++)
            {
                returnVal.Add(gridPosition + new Vector3Int(x, 0, y));
            }
        }
        return returnVal;
    }

    private Vector2Int GetRotatedSize(Vector2Int originalSize, Quaternion rotation)
    {
        // Controleer of het object 90 of 270 graden is geroteerd
        if (Mathf.Approximately(rotation.eulerAngles.y, 90) || Mathf.Approximately(rotation.eulerAngles.y, 270))
        {
            return new Vector2Int(originalSize.y, originalSize.x);
        }

        return originalSize; // Geen rotatie of 180 graden
    }

    public bool CanPlaceObjectAt(Vector3Int gridPosition, Vector2Int objectSize, Quaternion rotation)
    {
        List<Vector3Int> positionToOccupy = CalculatePositions(gridPosition, objectSize, rotation);
        foreach (var pos in positionToOccupy)
        {
            if (placedObjects.ContainsKey(pos))
                return false;
        }
        return true;
    }

    internal int GetRepresentationIndex(Vector3Int gridPosition)
    {
        if (placedObjects.ContainsKey(gridPosition) == false)
            return -1;
        return placedObjects[gridPosition].PlacedObjectIndex;
    }

    internal void RemoveObjectAt(Vector3Int gridPosition)
    {
        foreach (var pos in placedObjects[gridPosition].occupiedPositions)
        {
            placedObjects.Remove(pos);
        }
    }
}

public class PlacementData
{
    public List<Vector3Int> occupiedPositions;
    public int ID { get; private set; }
    public int PlacedObjectIndex { get; private set; }

    public PlacementData(List<Vector3Int> occupiedPositions, int iD, int placedObjectIndex)
    {
        this.occupiedPositions = occupiedPositions;
        ID = iD;
        PlacedObjectIndex = placedObjectIndex;
    }
}
