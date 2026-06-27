using UnityEngine;

/// <summary>
/// 여러 인수들을 가지고 다음 생성할 플랫폼의 조각 정보를 담은 배열을 제너레이터에 토해낸다.(제너레이터쪽에서 메서드 호출)
/// </summary>
public class PlatformRatioGenerator : MonoBehaviour
{
    private Player player;

    private float highestPlayerY;


    private float normalProbs = 300f;
    private float holeProbs;

    

    private void Awake()
    {
        player = GameObject.FindAnyObjectByType<Player>();
    }

    private void Update()
    {
        Update_HighestPlayerY();
    }

    private void Update_HighestPlayerY()
    {
        highestPlayerY = player.HighestYPos;

        holeProbs = highestPlayerY;
    }

    private PlatformGenerator.PieceType DecidePiece()
    {
        float[] probs = {normalProbs, holeProbs};
        float total = 0;

        foreach (float a in probs)
        {
            total += a;
        }

        float randomPoint = Random.value * total;

        for(int i = 0;i< probs.Length; i++)
        {
            if(randomPoint < probs[i])
            {
                return (PlatformGenerator.PieceType)i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }

        return (PlatformGenerator.PieceType)probs.Length - 1;
        
    }

    public PlatformGenerator.PieceType[] MakePlatform()
    {
        PlatformGenerator.PieceType[] platform = new PlatformGenerator.PieceType[5];

        for(int i = 0; i < platform.Length - 1; i++)
        {
            platform[i] = DecidePiece();
        }

        platform[platform.Length - 1] = PlatformGenerator.PieceType.End;

        

        return platform;
    }

}
