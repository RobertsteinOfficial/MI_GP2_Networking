using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;
    [SerializeField] private RectTransform unitSelectionArea;

    [SerializeField] private List<Unit> selectedUnits = new List<Unit>();
    public List<Unit> SelectedUnits { get { return selectedUnits; } }

    private Vector2 startPosition;
    private RTSPlayer player;

    private void OnEnable()
    {
        Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    

    private void OnDisable()
    {
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartSelectionArea();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            ClearSelectionArea();
        }
        else if (Mouse.current.leftButton.isPressed)
        {
            UpdateSelectionArea();
        }


    }

    private void StartSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(true);

        startPosition = Mouse.current.position.ReadValue();

        if (!Keyboard.current.leftShiftKey.isPressed)
        {
            //Starto la selezione
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.DeSelect();
            }

            selectedUnits.Clear();
        }

        UpdateSelectionArea();
    }

    private void ClearSelectionArea()
    {
        unitSelectionArea.gameObject.SetActive(false);

        if (unitSelectionArea.sizeDelta.magnitude <= 0.01f)
        {

            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, 100, layerMask)) return;
            if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;
            if (!unit.isOwned) return;
            if (selectedUnits.Contains(unit)) return;

            selectedUnits.Add(unit);

            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Select();
            }

            return;
        }

        Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
        Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

        foreach (Unit unit in player.MyUnits)
        {
            if (selectedUnits.Contains(unit)) continue;

            Vector3 screenPosition = Camera.main.WorldToScreenPoint(unit.transform.position);

            Rect rect = new Rect(min.x, min.y, max.x - min.x, max.y - min.y);

            if (rect.Contains(screenPosition))
            {
                SelectedUnits.Add(unit);
                unit.Select();
            }
        }
    }

    private void UpdateSelectionArea()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        float areaWidth = mousePosition.x - startPosition.x;
        float areaHeight = mousePosition.y - startPosition.y;

        unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), Mathf.Abs(areaHeight));
        unitSelectionArea.anchoredPosition = startPosition + new Vector2(areaWidth / 2, areaHeight / 2);
    }

    private void AuthorityHandleUnitDespawned(Unit unit)
    {
        selectedUnits.Remove(unit);
    }

    private void ClientHandleGameOver(string obj)
    {
        enabled = false;
    }
}
