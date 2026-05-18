using UnityEngine;

// 작품이 어느 방향에서 튀어나올지 선택하는 목록
public enum WallFaceDirection
{
    PlusX,
    MinusX,
    PlusZ,
    MinusZ
}

public class ExhibitionWall : MonoBehaviour
{
    public bool canPlaceArtwork = true; // 작품을 벽에 붙일지 말지 정하는 값

    [Tooltip("-1이면 제한 없음")]
    public int maxArtworkCount = -1;    // 한 벽에 몇 개의 작품을 붙일지 정하는 값

    public float sideMargin = 0.8f;     // 벽 끝에 여백을 얼마나 줄지(양쪽 side)
    public float spacing = 1.0f;        // 작품과 작품 사이 간격(캡션 생각하여 크게 잡음)
    public float artworkY = 2.0f;       // 작품이 배치될 높이
    public float wallOffset = 0.08f;    // 작품을 벽에서 얼마나 앞으로 띄울지 정하는 값(너무 작으면 액자가 벽에 들어감)

    [Header("작품이 튀어나올 방향")]
    public WallFaceDirection faceDirection;
}