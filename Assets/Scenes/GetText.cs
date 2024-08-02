using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GetText : MonoBehaviour
{
    public Transform contentWindow;
    public GameObject recallImageObject; // �Ψ���ܹϤ��� UI ����
    private List<GameObject> instantiatedImageObjects = new List<GameObject>();
    private Mes mesInstance; // �Ω�s�x Mes �������
    private string imagePath;

    void Awake()
    {
        // ��� Mes �������
        mesInstance = FindObjectOfType<Mes>();
        if (mesInstance == null)
        {
            Debug.LogError("Mes instance not found in the scene.");
        }

        // �]�m�Ϥ����۹���|
        imagePath = Path.Combine(Application.dataPath, "Scenes");

        Debug.Log("Image path set to: " + imagePath);
    }

    void Update()
    {
        if (mesInstance == null)
        {
            return; // �p�G Mes ��ҥ����A�h���i�����ާ@
        }

        // ��� ReadMes ����
        string readMesValue = mesInstance.ReadMes;
        Debug.Log("ReadMes value: " + readMesValue);

        // �C����s�ɲM�����e��ܪ����e
        ClearOldImageObjects();

        // ��ܹ������Ϥ�
        if (!string.IsNullOrEmpty(readMesValue))
        {
            // �B�z�i�઺�D�k�r��
            string sanitizedFileName = SanitizeFileName(readMesValue);
            string filePath = Path.Combine(imagePath, sanitizedFileName + ".png");

            Debug.Log("Attempting to load image from: " + filePath);

            if (File.Exists(filePath))
            {
                try
                {
                    // �Ыطs�������ܹϤ�
                    GameObject newRecallImageObject = Instantiate(recallImageObject, contentWindow);
                    Image imageComponent = newRecallImageObject.GetComponent<Image>();
                    if (imageComponent == null)
                    {
                        Debug.LogError("The recallImageObject does not have an Image component.");
                        Destroy(newRecallImageObject);
                        return;
                    }

                    // Ū���Ϥ����ഫ�� Texture2D
                    byte[] imageData = File.ReadAllBytes(filePath);
                    Debug.Log("Image data length: " + imageData.Length);
                    Texture2D texture = new Texture2D(2, 2); // �Ыؤ@�ӪŪ� Texture2D
                    if (texture.LoadImage(imageData))
                    {
                        Debug.Log("Texture loaded successfully.");
                        // �N Texture2D �]�m�� Image �ե�
                        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
                        imageComponent.sprite = sprite;
                        imageComponent.SetNativeSize(); // �]�m��l�j�p�A�H�T�O�Ϥ����T���
                        instantiatedImageObjects.Add(newRecallImageObject); // �x�s�s�Ыت�����
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
            Destroy(imageObject); // �R�����e�Ыت�����
        }
        instantiatedImageObjects.Clear(); // �M�ŦC��
    }

    // ��ơG�������X�k���ɦW�r��
    string SanitizeFileName(string fileName)
    {
        char[] invalidChars = Path.GetInvalidFileNameChars();
        foreach (char c in invalidChars)
        {
            fileName = fileName.Replace(c, '_'); // �N���X�k�r�Ŵ������U���u
        }
        return fileName.Trim(); // �h���}�Y�M�������ťզr��
    }
}
