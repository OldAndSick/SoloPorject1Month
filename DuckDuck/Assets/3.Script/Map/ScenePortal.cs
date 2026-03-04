using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class ScenePortal : MonoBehaviour, Interact
{
    [Header("Scene Settings")]
    public string nextSceneName = "2Stage_test";

    [Header("Fade Settings")]
    public Image fadeImage; 
    public float fadeDuration = 1.5f;

    public Text loadingText;

    private bool isFading = false;

    public void Interact(PlayerController player)
    {
        if (isFading) return;
        player.SavePlayerDataToTransfer();
        StartCoroutine(FadeAndLoadScene());
    }

    private IEnumerator FadeAndLoadScene()
    {
        isFading = true;

        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float timer = 0f;
            Color startColor = fadeImage.color;
            Color endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                fadeImage.color = Color.Lerp(startColor, endColor, timer / fadeDuration);
                yield return null;
            }
            fadeImage.color = endColor;
        }
        if(loadingText != null)
        {
            loadingText.gameObject.SetActive(true);
            loadingText.text = "Loading...0%";
        }
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(nextSceneName);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
            if(loadingText != null)
            {
                loadingText.text = $"Lading...{Mathf.RoundToInt(progress * 100)}%";
            }
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }
    }
}
