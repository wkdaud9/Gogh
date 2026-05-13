using UnityEngine;

public class ArtworkInteraction : MonoBehaviour
{
    [Header("Look Settings")]
    public Camera playerCamera;
    public float lookRequiredTime = 1.0f;
    public float lookDistance = 5.0f;

    private bool playerInRange = false;
    private float lookTimer = 0f;
    private bool hasTriggeredLook = false;

    private void Update()
    {
        if (!playerInRange)
        {
            lookTimer = 0f;
            hasTriggeredLook = false;
            return;
        }

        if (IsLookingAtArtwork())
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= lookRequiredTime && !hasTriggeredLook)
            {
                hasTriggeredLook = true;
                Debug.Log("작품을 일정 시간 바라봄: " + transform.parent.name);
            }
        }
        else
        {
            lookTimer = 0f;
            hasTriggeredLook = false;
        }
    }

    private bool IsLookingAtArtwork()
    {
        if (playerCamera == null)
        {
            Debug.LogWarning("Player Camera가 연결되지 않았습니다.");
            return false;
        }

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, lookDistance))
        {
            return hit.collider.CompareTag("Artwork");
        }

        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            Debug.Log("작품 감상 범위에 들어옴: " + transform.parent.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            lookTimer = 0f;
            hasTriggeredLook = false;
            Debug.Log("작품 감상 범위에서 나감: " + transform.parent.name);
        }
    }
}