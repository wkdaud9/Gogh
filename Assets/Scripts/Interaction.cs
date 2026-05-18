using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Interaction : MonoBehaviour
{
    [Header("Look Settings")]
    public Camera playerCamera;
    public float lookRequiredTime = 1.0f;
    public float lookDistance = 5.0f;

    [Header("UI Settings")]
    public GameObject infoUI;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public Button audioButton;
    public TMP_Text audioButtonText;

    private bool playerInRange = false;
    private float lookTimer = 0f;
    private bool uiVisible = false;
    private bool closedByPlayer = false;



    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        EnsureAudioSource();

        if (infoUI != null)
        {
            infoUI.SetActive(false);
        }

        UpdateAudioButtonText();
    }
    private void Update()
    {
        if (!playerInRange)
        {
            lookTimer = 0f;

            if (uiVisible)
            {
                HideInfoUI();
            }

            return;
        }

        // UI가 이미 켜져 있으면 시선을 돌려도 유지
        if (uiVisible)
        {
            UpdateAudioButtonText();
            return;
        }

        if (closedByPlayer)
        {
            lookTimer = 0f;
            return;
        }

        if (IsLookingAtArtwork())
        {
            lookTimer += Time.deltaTime;

            if (lookTimer >= lookRequiredTime)
            {
                ShowInfoUI();
            }
        }
        else
        {
            lookTimer = 0f;
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

    private void ShowInfoUI()
    {
        if (infoUI != null)
        {
            infoUI.SetActive(true);
            uiVisible = true;
        }

        UpdateAudioButtonText();
    }

    public void HideInfoUI()
    {
        if (!uiVisible)
        {
            return;
        }

        if (infoUI != null)
        {
            infoUI.SetActive(false);
        }

        uiVisible = false;
        StopAudio();
        lookTimer = 0f;
    }

    public void ToggleAudio()
    {
        if (!HasAudio())
        {
            UpdateAudioButtonText();
            return;
        }

        if (audioSource.isPlaying)
        {
            PauseAudio();
        }
        else
        {
            if (audioSource.time > 0f)
            {
                audioSource.UnPause();
            }
            else
            {
                audioSource.Play();
            }
        }

        UpdateAudioButtonText();
    }

    public void PauseAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Pause();
        }

        UpdateAudioButtonText();
    }

    public void StopAudio()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        UpdateAudioButtonText();
    }

    private bool HasAudio()
    {
        return audioSource != null && audioSource.clip != null;
    }

    private void UpdateAudioButtonText()
    {
        if (audioButtonText == null)
        {
            return;
        }

        if (!HasAudio())
        {
            audioButtonText.text = "도슨트 준비중";

            if (audioButton != null)
            {
                audioButton.interactable = false;
            }

            return;
        }

        if (audioButton != null)
        {
            audioButton.interactable = true;
        }

        audioButtonText.text= audioSource.isPlaying ? "■ 도슨트 일시정지" : "▶ 도슨트 듣기";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            lookTimer = 0f;
            closedByPlayer = false;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            lookTimer = 0f;
            closedByPlayer = false;
            HideInfoUI();
        }
    }
    public void CloseByPlayer()
    {
        closedByPlayer = true;
        HideInfoUI();
    }
    public void SetAudioClip(AudioClip clip)
    {
        EnsureAudioSource();

        if (clip == null)
        {
            Debug.LogWarning($"{gameObject.name}에 연결할 도슨트 클립이 없습니다.");
            UpdateAudioButtonText();
            return;
        }

        audioSource.clip = clip;
        UpdateAudioButtonText();

        Debug.Log($"도슨트 클립 연결 완료: {clip.name}");
    }

    private void EnsureAudioSource()
    {
        if (audioSource != null)
            return;

        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }
}