using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GetText : MonoBehaviour
{
    public Transform contentWindow;
    public GameObject recallImageObject; // 用來顯示圖片的 UI 元件
    private List<GameObject> instantiatedImageObjects = new List<GameObject>();
    private Mes mesInstance; // 用於存儲 Mes 類的實例
    private string imagePath;

    void Awake()
    {
        // 獲取 Mes 類的實例
        mesInstance = FindObjectOfType<Mes>();
        if (mesInstance == null)
        {
            Debug.LogError("Mes instance not found in the scene.");
        }

        // 設置圖片的相對路徑
        imagePath = Path.Combine(Application.dataPath, "Scenes");

        Debug.Log("Image path set to: " + imagePath);
    }

    void Update()
    {
        if (mesInstance == null)
        {
            return; // 如果 Mes 實例未找到，則不進行任何操作
        }

        // 獲取 ReadMes 的值
        string readMesValue = mesInstance.ReadMes;
        Debug.Log("ReadMes value: " + readMesValue);

        // 每次更新時清除之前顯示的內容
        ClearOldImageObjects();

        // 顯示對應的圖片
        if (!string.IsNullOrEmpty(readMesValue))
        {
            // 處理可能的非法字符
            string sanitizedFileName = SanitizeFileName(readMesValue);
            string filePath = Path.Combine(imagePath, sanitizedFileName + ".png");

            Debug.Log("Attempting to load image from: " + filePath);

            if (File.Exists(filePath))
            {
                try
                {
                    // 創建新物件並顯示圖片
                    GameObject newRecallImageObject = Instantiate(recallImageObject, contentWindow);
                    Image imageComponent = newRecallImageObject.GetComponent<Image>();
                    if (imageComponent == null)
                    {
                        Debug.LogError("The recallImageObject does not have an Image component.");
                        Destroy(newRecallImageObject);
                        return;
                    }

                    // 讀取圖片並轉換為 Texture2D
                    byte[] imageData = File.ReadAllBytes(filePath);
                    Debug.Log("Image data length: " + imageData.Length);
                    Texture2D texture = new Texture2D(2, 2); // 創建一個空的 Texture2D
                    if (texture.LoadImage(imageData))
                    {
                        Debug.Log("Texture loaded successfully.");
                        // 將 Texture2D 設置到 Image 組件
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        imageComponent.sprite = sprite;
                        imageComponent.SetNativeSize(); // 設置原始大小，以確保圖片正確顯示
                        instantiatedImageObjects.Add(newRecallImageObject); // 儲存新創建的物件
                        Debug.Log("Image loaded and displayed successfully.");
                    }
                    else
                    {
                        Debug.LogError("Failed to load image data from file: " + filePath);
                        Destroy(newRecallImageObject);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogError("Error reading or processing the image file: " + filePath + ". Exception: " + ex.Message);
                }
            }
            else
            {
                Debug.LogWarning("Image file not found: " + filePath);
            }
        }
    }

    void ClearOldImageObjects()
    {
        foreach (GameObject imageObject in instantiatedImageObjects)
        {
            Destroy(imageObject); // 刪除之前創建的物件
        }
        instantiatedImageObjects.Clear(); // 清空列表
    }

    // 函數：移除不合法的檔名字符
    string SanitizeFileName(string fileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            fileName = fileName.Replace(c, '_'); // 將不合法字符替換為下劃線
        }
        return fileName.Trim(); // 去除開頭和結尾的空白字符
    }
}
