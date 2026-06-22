using UnityEngine;

public class UnitTest_floorPrefab : MonoBehaviour
{

    [SerializeField] private GameObject floorPrefab;
    [SerializeField] private GameObject floor_NormalPrefab;
    [SerializeField] private GameObject floor_HolePrefab;
    [SerializeField] private GameObject floor_EndPrefab;

    private PlatformGenerator.PieceType normal = PlatformGenerator.PieceType.Normal;
    private PlatformGenerator.PieceType hole = PlatformGenerator.PieceType.Hole;
    private PlatformGenerator.PieceType end = PlatformGenerator.PieceType.End;

    private PlatformGenerator.PieceType[] test = new PlatformGenerator.PieceType[5];


    private void Start()
    {
        test[0] = normal;
        test[1] = hole;
        test[2] = normal;
        test[3] = hole;
        test[4] = end;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Generate();
        }

        if(Input.GetKeyDown(KeyCode.S)) {GetPiece(); }
    }

    private void Generate()
    {
        GameObject slotParent = Instantiate(floorPrefab, new Vector2(0,0), Quaternion.identity);

        for (int i = 0; i < test.Length; i++)
        {
            Transform slot = slotParent.transform.GetChild(i);

            GameObject piece = GetPieceType(test[i]);

            Instantiate(piece, slot, false);
        }
    }

    private void GetPiece()
    {
        Instantiate(floor_HolePrefab, new Vector2(0,0), Quaternion.identity);
    }

    private GameObject GetPieceType(PlatformGenerator.PieceType pieceType)
    {
        switch (pieceType)
        {
            case PlatformGenerator.PieceType.Normal:
                return floor_NormalPrefab;
            case PlatformGenerator.PieceType.Hole:
                return floor_HolePrefab;
            default:
                return floor_EndPrefab;
        }
    }
}
