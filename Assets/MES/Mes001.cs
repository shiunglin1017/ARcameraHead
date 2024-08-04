using System;
using UnityEngine;

public class Mes001 : MonoBehaviour
{
    // 要隱藏或顯示的 CanvasGroup
    public CanvasGroup canvasGroup;

    // 淡入和淡出的速度（每秒）
    public float fadeInDuration = 0.4f;
    public float fadeOutDuration = 0.4f;

    private bool isFadingIn = false; // 記錄是否需要淡入
    private float fadeSpeed = 0f; // 用於動態設定的淡入淡出速度

    // Update is called once per frame
    void Update()
    {
        if (canvasGroup != null)
        {



            // 確保 messageString 至少有三個字符
            if (!string.IsNullOrEmpty(Mes.messageString) && Mes.messageString.Length > 2)
            {
                // 獲取第二個字符並進行比較
                char secondChar = Mes.messageString[0];
                if (secondChar == '1')
                {
                    isFadingIn = true;
                }
                else
                {
                    isFadingIn = false;
                }
            }
            else
            {
                isFadingIn = false;
            }

            // 根據淡入或淡出標記調整 alpha 值
            if (isFadingIn)
            {
                // 設定淡入速度
                fadeSpeed = 1f / fadeInDuration;
                // 增加 alpha 值，最大為 1
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1f, Time.deltaTime * fadeSpeed);
            }
            else
            {
                // 設定淡出速度
                fadeSpeed = 1f / fadeOutDuration;
                // 減少 alpha 值，最小為 0
                canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
            }

            // 根據 alpha 值設置 interactable 和 blocksRaycasts
            canvasGroup.interactable = canvasGroup.alpha > 0f;
            canvasGroup.blocksRaycasts = canvasGroup.alpha > 0f;

        }
        else
        {
            Debug.Log("CanvasGroup is null"); // 如果 CanvasGroup 是 null，記錄日誌
        }
    }
}
