using UnityEngine;
using TMPro;

public class ArtworkDisplay : MonoBehaviour
{
    [Header("Artwork Info")]
    public string artworkId;
    public string title;
    public string subtitle;
    public string year;
    public string medium;
    [TextArea]
    public string description;

    [Header("Image / Audio")]
    public string imagePath;
    public string audioPath;
    public Texture2D artworkTexture;
    public AudioClip docentAudio;

    [Header("Frame Renderer")]
    public Renderer imageRenderer;

    [Header("UI Text")]
    public TMP_Text titleText;
    public TMP_Text subtitleText;
    public TMP_Text yearText;
    public TMP_Text mediumText;
    public TMP_Text descriptionText;

    public void ApplyData(ArtworkData data)
    {
        artworkId = data.id;
        title = data.title;
        subtitle = data.subtitle;
        year = data.year;
        medium = data.medium;
        description = data.description;
        imagePath = data.imagePath;
        audioPath = data.audioPath;

        artworkTexture = Resources.Load<Texture2D>(imagePath);
        docentAudio = Resources.Load<AudioClip>(audioPath);

        Interaction interaction = GetComponentInChildren<Interaction>();

        if (interaction != null)
        {
            interaction.SetAudioClip(docentAudio);
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}에서 ArtworkInteraction을 찾지 못했습니다.");
        }

        if (imageRenderer != null && artworkTexture != null)
        {
            imageRenderer.material.mainTexture = artworkTexture;
        }

        if (titleText != null) titleText.text = title;
        if (subtitleText != null) subtitleText.text = subtitle;
        if (yearText != null) yearText.text = year;
        if (mediumText != null) mediumText.text = medium;
        if (descriptionText != null) descriptionText.text = description;
    }
}