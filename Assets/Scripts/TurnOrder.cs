using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnOrder : MonoBehaviour
{
    private Unit[] combatUnits;
    private System.Random random = new System.Random();

    public void MakeTurnOrder(Unit[] units)
    {
        combatUnits = units;
        RollForTurnOrder();

        Debug.Log("Turn order for the round based on rolled speed:");
        foreach (var unit in combatUnits)
        {
            Debug.Log(unit.unitName + " with total speed: " + unit.currentSpeed);
        }
    }

    public Unit GetNextUnit(int turnIndex)
    {
        return combatUnits[turnIndex % combatUnits.Length];
    }

    public void RollForTurnOrder()
    {
        // Roll a d8 for each unit and add it to their original speed
        foreach (var unit in combatUnits)
        {
            int rolledValue = random.Next(1, 9); // Rolling a d8 (values 1 to 8)
            unit.currentSpeed = unit.speed + rolledValue; // Add rolled value to original speed
        }

        // Sort units by currentSpeed in ascending order
        System.Array.Sort(combatUnits, (unit1, unit2) => unit2.currentSpeed.CompareTo(unit1.currentSpeed));
    }

    public void ResetSpeeds()
    {
        foreach (var unit in combatUnits)
        {
            unit.currentSpeed = unit.speed; // Reset to the original speed
        }
    }
}