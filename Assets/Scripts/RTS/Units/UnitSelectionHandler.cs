using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitSelectionHandler : MonoBehaviour
{
    [SerializeField] LayerMask layerMask;

    private List<Unit> selectedUnits = new List<Unit>();
    public List<Unit> SelectedUnits { get { return selectedUnits; } }


    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            //Starto la selezione
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.DeSelect();
            }

            selectedUnits.Clear();
        }
        else if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            //clear
            //seleziono
            ClearSelectionArea();
        }

    }

    private void ClearSelectionArea()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, 100, layerMask)) return;
        if (!hit.collider.TryGetComponent<Unit>(out Unit unit)) return;
        if (!unit.isOwned) return;

        selectedUnits.Add(unit);

        foreach (Unit selectedUnit in selectedUnits)
        {
            selectedUnit.Select();
        }
    }
}
