using UnityEngine;
using System.Collections;

public class TapElementController : MonoBehaviour
{
    public Collider2D tapZone;
    public int row, column;
    public bool isAvailable { get; private set; }

    void OnEnable()
    {
        isAvailable = true;
    }
    void OnMouseDown()
    {
        {
            Debug.Log("touched " + row + " and " + column);
            EventManager.Instance.Call(EventManager.events.CellTaped, new object[] { row, column });
        }
    }

    public Pair GetPosition()
    {
        return new Pair() { _row = row, _column = column };
    }

    public void FillCell()
    {
        isAvailable = false;
    }


    public struct Pair
    {
        public int _row, _column;
    }

    public void SetFree()
    {
        isAvailable = true;
    }
}
