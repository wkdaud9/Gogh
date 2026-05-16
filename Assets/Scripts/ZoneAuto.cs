using System.Collections.Generic;
using UnityEngine;

public class ZoneAuto : MonoBehaviour
{
    [Header("JSON")]
    public TextAsset jsonFile;

    [Header("Prefabs")]
    public GameObject horizontalArtworkPrefab;
    public GameObject verticalArtworkPrefab;

    [Header("Parents")]
    public Transform wallParent;
    public Transform artworkParent;

    [Header("Artwork Size")]
    public float horizontalArtworkWidth = 5.5f;
    public float verticalArtworkWidth = 4.5f;

    [Header("Auto Clear")]
    public bool clearExistingArtworksOnStart = true;

    private List<ArtworkData> artworks = new List<ArtworkData>();

    private void Start()
    {
        LoadJson();

        if (clearExistingArtworksOnStart)
            ClearExistingArtworks();

        PlaceArtworks();
    }

    private void LoadJson()
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON 파일이 없습니다.");
            return;
        }

        string wrappedJson = "{\"artworks\":" + jsonFile.text + "}";
        ArtworkDataList dataList = JsonUtility.FromJson<ArtworkDataList>(wrappedJson);

        if (dataList == null || dataList.artworks == null)
        {
            Debug.LogError("JSON 파싱 실패");
            return;
        }

        artworks = dataList.artworks;
    }

    private void ClearExistingArtworks()
    {
        if (artworkParent == null) return;

        for (int i = artworkParent.childCount - 1; i >= 0; i--)
        {
            Destroy(artworkParent.GetChild(i).gameObject);
        }
    }

    private void PlaceArtworks()
    {
        if (wallParent == null)
        {
            Debug.LogError("Wall Parent가 없습니다.");
            return;
        }

        if (artworkParent == null)
        {
            Debug.LogError("Artwork Parent가 없습니다.");
            return;
        }

        int artworkIndex = 0;
        ExhibitionWall[] walls = wallParent.GetComponentsInChildren<ExhibitionWall>();

        foreach (ExhibitionWall wallInfo in walls)
        {
            if (artworkIndex >= artworks.Count)
                break;

            if (!wallInfo.canPlaceArtwork)
                continue;

            Transform wall = wallInfo.transform;
            Renderer wallRenderer = wall.GetComponent<Renderer>();

            if (wallRenderer == null)
            {
                Debug.LogWarning($"{wall.name}에 Renderer 없음");
                continue;
            }

            Bounds bounds = wallRenderer.bounds;

            bool isHorizontalWall = bounds.size.x >= bounds.size.z;
            float wallLength = isHorizontalWall ? bounds.size.x : bounds.size.z;
            float usableLength = wallLength - wallInfo.sideMargin * 2f;

            if (usableLength <= 0)
                continue;

            int countOnWall = CalculateArtworkCount(
                usableLength,
                artworkIndex,
                wallInfo.maxArtworkCount,
                wallInfo.spacing
            );

            for (int i = 0; i < countOnWall; i++)
            {
                ArtworkData data = artworks[artworkIndex];

                GameObject prefab = data.orientation == "세로"
                    ? verticalArtworkPrefab
                    : horizontalArtworkPrefab;

                if (prefab == null)
                {
                    Debug.LogError("Artwork 프리팹 연결 안 됨");
                    return;
                }

                float currentArtworkWidth = data.orientation == "세로"
                ? verticalArtworkWidth
                : horizontalArtworkWidth;

                Vector3 position = GetPositionOnWall(
                    wall,
                    bounds,
                    wallInfo,
                    isHorizontalWall,
                    i,
                    countOnWall,
                    currentArtworkWidth
                );

                Quaternion rotation = GetRotationOnWall(wallInfo);

                GameObject obj = Instantiate(prefab, position, rotation, artworkParent);
                obj.name = $"Artwork_{artworkIndex + 1}_{data.id}";

                ArtworkDisplay display = obj.GetComponentInChildren<ArtworkDisplay>();

                if (display != null)
                {
                    display.ApplyData(data);
                }
                else
                {
                    Debug.LogWarning($"{obj.name}에 ArtworkDisplay 없음");
                }

                artworkIndex++;
            }
        }

        Debug.Log($"자동 배치 완료: {artworkIndex}/{artworks.Count}");
    }

    private int CalculateArtworkCount(
        float usableLength,
        int startIndex,
        int maxCount,
        float spacing
    )
    {
        int count = 0;
        float usedLength = 0f;

        for (int i = startIndex; i < artworks.Count; i++)
        {
            if (maxCount >= 0 && count >= maxCount)
                break;

            float width = artworks[i].orientation == "세로"
                ? verticalArtworkWidth
                : horizontalArtworkWidth;

            float need = width;

            if (count > 0)
                need += spacing;

            if (usedLength + need > usableLength)
                break;

            usedLength += need;
            count++;
        }

        return count;
    }

    private Vector3 GetPositionOnWall(
        Transform wall,
        Bounds bounds,
        ExhibitionWall wallInfo,
        bool isHorizontalWall,
        int index,
        int count,
        float artworkWidth
    )
    {
        Vector3 center = bounds.center;

        Vector3 faceDir = GetFaceDirection(wallInfo);

        Vector3 alongDir = isHorizontalWall
            ? Vector3.right
            : Vector3.forward;

        float wallLength = isHorizontalWall ? bounds.size.x : bounds.size.z;

        float start = -wallLength / 2f + wallInfo.sideMargin + artworkWidth / 2f;
        float end = wallLength / 2f - wallInfo.sideMargin - artworkWidth / 2f;

        float t = count == 1 ? 0.5f : (float)index / (count - 1);
        float offset = Mathf.Lerp(start, end, t);

        Vector3 pos =
            center
            + alongDir * offset
            + faceDir * wallInfo.wallOffset;

        pos.y = wallInfo.artworkY;

        return pos;
    }

    private Quaternion GetRotationOnWall(ExhibitionWall wallInfo)
    {
        Vector3 faceDir = GetFaceDirection(wallInfo);

        return Quaternion.LookRotation(faceDir, Vector3.up);
    }

    private Vector3 GetFaceDirection(ExhibitionWall wallInfo)
    {
        switch (wallInfo.faceDirection)
        {
            case WallFaceDirection.PlusX:
                return Vector3.right;

            case WallFaceDirection.MinusX:
                return Vector3.left;

            case WallFaceDirection.PlusZ:
                return Vector3.forward;

            case WallFaceDirection.MinusZ:
                return Vector3.back;

            default:
                return Vector3.forward;
        }
    }
}