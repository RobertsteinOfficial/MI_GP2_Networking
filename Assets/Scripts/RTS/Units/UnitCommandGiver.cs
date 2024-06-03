using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler;
    [SerializeField] private LayerMask layerMask;

    private void OnEnable()
    {
        GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
    }

    private void OnDisable()
    {
        GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
    }

   
    private void Update()
    {
        if (!Mouse.current.rightButton.wasPressedThisFrame) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, 100, layerMask)) return;

        if (hit.collider.TryGetComponent<Targetable>(out Targetable newTarget))
        {
            if (newTarget.isOwned)
            {
                TryMove(hit.point);
                return;
            }

            TryTarget(newTarget);
            return;
        }


        TryMove(hit.point);
    }

    private void TryMove(Vector3 point)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.Movement.CmdMove(point);
        }
    }

    private void TryTarget(Targetable newTarget)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.Targeter.CmdSetTarget(newTarget.gameObject);
        }
    }

    private void ClientHandleGameOver(string obj)
    {
        enabled = false;
    }

}
