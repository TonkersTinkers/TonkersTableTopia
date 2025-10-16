using UnityEngine;

/// <summary>
/// Component attached to each row GameObject in a Ux_TableLayout.
/// Holds the row index and can be extended for row-specific behavior or data.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class Ux_TonkersTableTopiaRow : MonoBehaviour
{
    [HideInInspector] public int rowNumberWhereShenanigansOccur = -1;
}