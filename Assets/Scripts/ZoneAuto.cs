using System.Collections.Generic;
using UnityEngine;

public class ZoneAuto : MonoBehaviour
{
    // 작품 정보가 들어있는 JSON 파일 넣는 칸
    [Header("JSON")]
    public TextAsset jsonFile;

    // 자동 생성할 액자 프리팹 가로형, 세로형 넣는 칸
    [Header("Prefabs")] 
    public GameObject horizontalArtworkPrefab;
    public GameObject verticalArtworkPrefab;

    // 벽들이 들어있는 부모와 생성된 작품들이 들어갈 부모 넣는 칸
    [Header("Parents")]
    public Transform wallParent;
    public Transform artworkParent;

    // 자동 배치할 때 작품 하나가 벽에서 차지하는 폭
    [Header("Artwork Size")]
    public float horizontalArtworkWidth = 3.0f;
    public float verticalArtworkWidth = 2.0f;

    // 수동으로 넣어둔 작품이 있으면 삭제 할 것인지 여부
    [Header("Auto Clear")]
    public bool clearExistingArtworksOnStart = true;
    
    // JSON에서 읽어온 작품 데이터를 저장하는 리스트
    private List<ArtworkData> artworks = new List<ArtworkData>();

    private void Start()
    {
        LoadJson();

        if (clearExistingArtworksOnStart)
            ClearExistingArtworks();

        PlaceArtworks();
    }

    // JSON 파일 읽어서 artworks 리스트에 저장하는 함수
    private void LoadJson()
    {
        if (jsonFile == null)
        {
            Debug.LogError("JSON 파일이 없습니다.");
            return;
        }
        
        //JSON 내용을 읽어서, ArtworkDataList 클래스 형태의 객체로 변환
        ArtworkDataList dataList = JsonUtility.FromJson<ArtworkDataList>(jsonFile.text);

        if (dataList == null || dataList.artworks == null)
        {
            Debug.LogError("JSON 파싱 실패");
            return;
        }

        artworks = dataList.artworks;
    }

    // artworkParent에 기존에 만든 그림이 있다면 삭제
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

            Bounds bounds = wallRenderer.bounds;    // 벽의 실제 위치와 크기를 가져옴

            bool isHorizontalWall = bounds.size.x >= bounds.size.z; // 벽이 x축으로 긴 지, z축으로 긴 지 확인
            float wallLength = isHorizontalWall ? bounds.size.x : bounds.size.z;    // 벽의 형태에 따라 실제 길이 가져옴.
            float usableLength = wallLength - wallInfo.sideMargin * 2f; // 벽 전체 길이에서 양쪽 여백을 뺀 실제 사용 가능한 길이를 계산.

            if (usableLength <= 0)
                continue;

            int countOnWall = CalculateArtworkCount(
                usableLength,   // 사용 가능한 벽 길이
                artworkIndex,   // 몇 번째 작품부터인지
                wallInfo.maxArtworkCount,   // 이 벽에 최대 몇개까지 넣을지
                wallInfo.spacing    // 작품 사이 간격
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

    // 이 벽에 작품 몇 개 들어가는지 계산
    private int CalculateArtworkCount(
        float usableLength,
        int startIndex,
        int maxCount,
        float spacing
    )
    {
        int count = 0;  // 현재까지 이 벽에 몇 개 넣었는지 체크
        float usedLength = 0f;  // 벽의 길이를 얼마나 사용했는지 체크

        for (int i = startIndex; i < artworks.Count; i++)
        {
            if (maxCount >= 0 && count >= maxCount)
                break;

            float width = artworks[i].orientation == "세로"
                ? verticalArtworkWidth
                : horizontalArtworkWidth;

            float need = width;

            if (count > 0)  // 첫 작품은 앞 간격이 필요 없지만 두 번째부터는 필요.
                need += spacing;

            if (usedLength + need > usableLength)
                break;

            usedLength += need;
            count++;
        }

        return count;
    }

    // 작품을 벽의 어디에 놓을지 계산
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

        // 벽 총 길이가 20이라면 시작 지점은 -10, 끝 지점은 +10이다. 거기에 양쪽 끝 마진을 주고,
        // 작품이 벽을 넘지 않도록 작품사이즈/2를 해서 더한다.
        float start = (-wallLength / 2f) + (wallInfo.sideMargin) + (artworkWidth / 2f);
        float end = (wallLength / 2f) - (wallInfo.sideMargin) - (artworkWidth / 2f);

        // 작품이 1개면 0.5(센터) 배치, 아니면 비율에 맞게 배치
        float t = count == 1 ? 0.5f : (float)index / (count - 1);
        // start와 end 사이의 값을 t 비율로 구하는 함수, 0.5면 가운데
        float offset = Mathf.Lerp(start, end, t);

        Vector3 pos =
            center
            + alongDir * offset // 가로벽인지 세로벽인지 확인 후 offset 곱하여 좌표 이동
            + faceDir * wallInfo.wallOffset;    // 그림이 벽에서 튀어나와 보여질 좌표로 이동

        pos.y = wallInfo.artworkY;

        return pos;
    }

    // 작품을 어느 방향으로 회전할 지 계산
    private Quaternion GetRotationOnWall(ExhibitionWall wallInfo)
    {
        Vector3 faceDir = GetFaceDirection(wallInfo);

        // 앞쪽 방향과 위쪽 방향을 줘서, 그쪽을 바라보게 만드는 함수
        return Quaternion.LookRotation(faceDir, Vector3.up);
    }

    // plusX, minusX 같은 설정을 실제 방향 vector3로 변환
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