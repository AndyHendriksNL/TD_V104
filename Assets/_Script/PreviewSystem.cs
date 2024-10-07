using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PreviewSystem : MonoBehaviour
{
    [SerializeField] private float previewYOffset = 0.06f;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private Material previewMaterialPrefab;
    
    private GameObject previewObject;    
    private Material previewMaterialInstance;
    private Renderer cellIndicatorRenderer;
    private Vector2Int currentObjectSize;
    private Vector3 currentRotationOffset = Vector3.zero;

    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }
    public void StartShowingPlacementPreview(GameObject prefab, Vector2Int size)
    {
        previewObject = Instantiate(prefab);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
        currentObjectSize = size;
    }

    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x, 1, size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }

    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }
    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if (previewObject != null)
            Destroy(previewObject);
    }

    public void UpdatePosition(Vector3 position, bool validity)
    {
        if (previewObject != null)
        {
            //Debug.Log($"UpdatePosition + {currentRotationOffset}");
            MovePreview(position);
            ApplyFeedbackToPreview(validity);

        }

        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }

    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }
    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }

    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }

    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(
            position.x,
            position.y + previewYOffset,
            position.z);
    }

    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }

    //public void SetPreview(GameObject previewObject)
    //{
    //    previewObject = previewObject;
    //}

    public void RotatePreview()
    {
        if (previewObject != null)
        {
            previewObject.transform.Rotate(0, 90, 0);
            //previewObject.transform.position = GetOffsetPosition(previewObject.transform.position, currentObjectSize, previewObject.transform.rotation);
        }
    }

    public Quaternion GetCurrentRotation()
    {
        if (previewObject != null)
        {
            return previewObject.transform.rotation;
        }
        return Quaternion.identity; // Standaard rotatie als er geen preview is
    }

    public Vector3 GetOffsetPosition(Vector3 originalPosition, Vector2Int objectSize, Quaternion rotation)
    {
        Vector3 offset = Vector3.zero;

        // Afhankelijk van de rotatie, bereken de juiste offset
        if (Mathf.Approximately(rotation.eulerAngles.y, 90))
        {
            offset = new Vector3(0, 0, objectSize.y);
        }
        else if (Mathf.Approximately(rotation.eulerAngles.y, 180))
        {
            offset = new Vector3(objectSize.x, 0, 0);
        }
        else if (Mathf.Approximately(rotation.eulerAngles.y, 270))
        {
            offset = new Vector3(0, 0, -objectSize.y);
        }
        else
        {
            offset = Vector3.zero;
        }

        // update rotation offset
        currentRotationOffset = offset;

        Debug.Log($"Rotation: {rotation.eulerAngles.y} | Original Pos: {originalPosition} | Offset: {offset}");

        return originalPosition + offset;
    }
}
