using TMPro;
using UnityEngine;
using System.Collections;
[RequireComponent(typeof(TextMeshProUGUI))]
public class TypeWriterPOP : MonoBehaviour
{
    
    [Header("Settings")]
    [SerializeField] private bool playOnStart = true;
    [SerializeField] private float timeBetweenCharacters = 0.05f;
    [SerializeField] private float popDuration = 0.15f;
    [SerializeField] private AnimationCurve popCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Audio (Optional)")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip popSound;

    private TextMeshProUGUI m_textMeshPro;

    private void Awake()
    {
        m_textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (playOnStart && !string.IsNullOrEmpty(m_textMeshPro.text))
        {
            ShowText(m_textMeshPro.text);
        }
    }

    public void ShowText(string text)
    {
        StopAllCoroutines();
        StartCoroutine(TypeTextAndPop(text));
    }

    private IEnumerator TypeTextAndPop(string text)
    {
        // 1. Set text and force layout calculation so mesh information is generated
        m_textMeshPro.text = text;
        m_textMeshPro.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_textMeshPro.textInfo;
        int totalCharacters = textInfo.characterCount;

        // 2. Cache pristine original mesh data ONCE before altering any vertices
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();

        // 3. Start all visible characters completely hidden (scaled down to 0)
        for (int i = 0; i < totalCharacters; i++)
        {
            if (!textInfo.characterInfo[i].isVisible) continue;
            ScaleCharacterVertices(i, 0f, cachedMeshInfo);
        }
        m_textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

        // 4. Reveal and pop characters one by one
        for (int i = 0; i < totalCharacters; i++)
        {
            // Skip whitespaces/invisible markers
            if (!textInfo.characterInfo[i].isVisible)
            {
                yield return new WaitForSeconds(timeBetweenCharacters);
                continue;
            }

            // Play the letter pop sound if assigned
            if (audioSource && popSound)
            {
                audioSource.PlayOneShot(popSound);
            }

            // Animate scale up for the current character using cached original mesh data
            StartCoroutine(AnimateCharacterPop(i, cachedMeshInfo));

            yield return new WaitForSeconds(timeBetweenCharacters);
        }
    }

    private IEnumerator AnimateCharacterPop(int charIndex, TMP_MeshInfo[] cachedMeshInfo)
    {
        float elapsed = 0f;

        while (elapsed < popDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / popDuration);
            float currentScale = popCurve.Evaluate(t);

            ScaleCharacterVertices(charIndex, currentScale, cachedMeshInfo);
            m_textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
            yield return null;
        }

        // Ensure it ends perfectly at scale 1.0
        ScaleCharacterVertices(charIndex, 1.0f, cachedMeshInfo);
        m_textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);
    }

    private void ScaleCharacterVertices(int charIndex, float scale, TMP_MeshInfo[] cachedMeshInfo)
    {
        TMP_TextInfo textInfo = m_textMeshPro.textInfo;
        TMP_CharacterInfo charInfo = textInfo.characterInfo[charIndex];

        if (!charInfo.isVisible) return;

        int materialIndex = charInfo.materialReferenceIndex;
        int vertexIndex = charInfo.vertexIndex;

        Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
        Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;

        // Calculate the center point of the individual character from the original geometry
        Vector3 charBoundsCenter = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2f;

        // Apply scale transformations relative to the character's center
        for (int j = 0; j < 4; j++)
        {
            Vector3 origVertex = sourceVertices[vertexIndex + j];
            Vector3 offset = origVertex - charBoundsCenter;
            destinationVertices[vertexIndex + j] = charBoundsCenter + (offset * scale);
        }
    }
}