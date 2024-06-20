using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int movementRange;
    [SerializeField] private SpriteRenderer idleVisual;
    [SerializeField] private SpriteRenderer selectedVisual;
    private Vector2 targetPosition;
    private void Start() {
        Deselect();
        UpdateUnitPosition();
    }
    public void Move(Vector2 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
    private void Update() {
        UpdateUnitPosition();
    }
    private void UpdateUnitPosition()
    {
        if (transform.position != (Vector3)targetPosition)
        {
            transform.position = (Vector3)targetPosition;
        }
    }
    public void Select()
    {
        idleVisual.enabled = false;
        selectedVisual.enabled = true;
        Debug.Log($"selected visual enabled: {selectedVisual.enabled}");
    }

    public void Deselect()
    {
        idleVisual.enabled = true;
        selectedVisual.enabled = false;
        Debug.Log($"selected visual enabled: {selectedVisual.enabled}");
    }
}
