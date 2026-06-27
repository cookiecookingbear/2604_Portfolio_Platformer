using UnityEngine;

public class FloorPrefab : MonoBehaviour
{
    [SerializeField] private int floor;
    public int Floor => floor;
    public void GetFloor(int floor)
    {
        this.floor = floor;
    }
}
