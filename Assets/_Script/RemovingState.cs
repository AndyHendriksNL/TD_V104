using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    Grid grid;
    PreviewSystem previewSystem;
    GridData objectData;
    ObjectPlacer objectPlacer;
    SoundFeedback soundFeedback;

    public RemovingState(Grid grid,
                         PreviewSystem previewSystem,
                         GridData objectData,
                         ObjectPlacer objectPlacer,
                         SoundFeedback soundFeedback)
    {
        this.grid = grid;
        this.previewSystem = previewSystem;
        this.objectData = objectData;
        this.objectPlacer = objectPlacer;
        this.soundFeedback = soundFeedback;
        previewSystem.StartShowingRemovePreview();
    }

    public void EndState()
    {
        previewSystem.StopShowingPreview();
    }

    public void OnAction(Vector3Int gridPosition)
    {
        GridData selectedData = null;

        // find the object in the GridData placedObjects list and return index
        gameObjectIndex = objectData.GetRepresentationIndex(gridPosition);

        // if no index is found
        if (gameObjectIndex != -1)
        {
            selectedData = objectData;
        }

        if (selectedData == null)
        {
            //sound
            soundFeedback.PlaySound(SoundType.wrongPlacement);
        }
        else
        {
            soundFeedback.PlaySound(SoundType.Remove);
            if (gameObjectIndex == -1)
                return;
            selectedData.RemoveObjectAt(gridPosition);
            objectPlacer.RemoveObjectAt(gameObjectIndex);
        }
        Vector3 cellPosition = grid.CellToWorld(gridPosition);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        // nothing to update
    }   
}
