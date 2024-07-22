using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;


public class GetText : MonoBehaviour
{
    public Transform contentWindow;
    public Text recallTextObject;
    private List<GameObject> instantiatedObjects = new List<GameObject>();
    [SerializeField]private Mes mesInstance; // 用於存儲 Mes 類的實例
    private string imagesFolderPath = @"C:\Users\USER\Test3\Assets\StreamingAssets\Images"; // 資料夾路徑

    void Awake()
    {
        // 獲取 Mes 類的實例
        //mesInstance = FindObjectOfType<Mes>();
    }

    void Update()
    {
        if (mesInstance == null)
        {
            Debug.LogError("Mes instance not found");
            return;
        }

        // 獲取 ReadMes 的值
        string readMesValue = mesInstance.ReadMes;
        recallTextObject.text = readMesValue;

        // 每次更新時清除之前顯示的內容
        //ClearOldObjects();

        /*
        // 顯示 ReadMes 的內容或圖片
        if (!string.IsNullOrEmpty(readMesValue))
        {
            string imagePath = Path.Combine(imagesFolderPath, readMesValue);

            if (File.Exists(imagePath))
            {
                // 加載圖片
                byte[] fileData = File.ReadAllBytes(imagePath);
                Texture2D texture = new Texture2D(2, 2);
                texture.LoadImage(fileData); // 注意: LoadImage 會覆蓋掉 texture 的像素

                // 創建新的圖片物件
                GameObject newImageObject = new GameObject("ImageObject");
                newImageObject.transform.SetParent(contentWindow, false);
                Image imageComponent = newImageObject.AddComponent<Image>();
                imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                instantiatedObjects.Add(newImageObject); // 儲存新創建的物件
            }
            else
            {
                // 創建新的文本物件
                GameObject newRecallTextObject = Instantiate(recallTextObject, contentWindow);
                newRecallTextObject.GetComponent<Text>().text = readMesValue;
                instantiatedObjects.Add(newRecallTextObject); // 儲存新創建的物件
            }
        
        }
        */
    }

    void ClearOldObjects()
    {
        foreach (GameObject obj in instantiatedObjects)
        {
            Destroy(obj); // 刪除之前創建的物件
        }
        instantiatedObjects.Clear(); // 清空列表
    }
}
