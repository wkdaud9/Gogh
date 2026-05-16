using UnityEngine;

public enum WallFaceDirection
{
    PlusX,
    MinusX,
    PlusZ,
    MinusZ
}

public class ExhibitionWall : MonoBehaviour
{
    public bool canPlaceArtwork = true;

    [Tooltip("-1이면 제한 없음")]
    public int maxArtworkCount = -1;

    public float sideMargin = 0.8f;
    public float spacing = 1.0f;
    public float artworkY = 2.0f;
    public float wallOffset = 0.35f;

    [Header("작품이 튀어나올 방향")]
    public WallFaceDirection faceDirection;
}