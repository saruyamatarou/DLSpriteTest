using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // UIを使用する場合はこの行が必要

public class DownloadImageAsSprite : MonoBehaviour
{
    public string imageUrl1024 = "ここに画像のURLを入力"; // 表示したい画像のURL
    public string imageUrl512 = "ここに画像のURLを入力"; // 表示したい画像のURL
    public string imageUrl256 = "ここに画像のURLを入力"; // 表示したい画像のURL
    public SpriteRenderer spriteRenderer; // SpriteRendererを使用する場合
    // public Image uiImage; // UIのImageコンポーネントを使用する場合

    // ログメッセージを保存するキュー
    private Queue<string> logQueue = new Queue<string>();

    // 画面に表示するログの最大数
    public int maxLogs = 20;

    // GUIスタイル（オプショナル）
    public GUIStyle guiStyle;


    void Start()
    {
        Debug.Log("開始");
        StartCoroutine(DownloadImage(imageUrl1024, 1.0f, 0.5f, 400.0f));
        StartCoroutine(DownloadImage(imageUrl512, 0.0f, 0.5f, 200.0f));
        StartCoroutine(DownloadImage(imageUrl256, -1.0f, 0.5f, 100.0f));
    }

    IEnumerator DownloadImage(string url, float x, float y, float scale)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("画像のダウンロードに失敗: " + request.error);
        }
        else
        {
            Debug.Log("ダウンロード成功");
            Texture2D texture = DownloadHandlerTexture.GetContent(request);

            Debug.Log("ダウンロードしたテクスチャのフォーマット: " + texture.format);
            texture.Compress(true);
            Debug.Log("テクスチャを圧縮しました。");
            Debug.Log("圧縮後のフォーマット: " + texture.format);

            // 新しいGameObjectを作成してSpriteRendererコンポーネントを追加
            GameObject newGameObject = new GameObject("DownloadedImage");
            SpriteRenderer spriteRenderer = newGameObject.AddComponent<SpriteRenderer>();

            // Texture2DからSpriteを生成してSpriteRendererに設定
            Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), scale);
            spriteRenderer.sprite = newSprite;

            // 画像の位置を設定
            newGameObject.transform.position = new Vector3(x, y, 0);
        }
    }

    bool IsCompressibleFormat(TextureFormat format)
    {
        switch (format)
        {
            case TextureFormat.DXT1:
            case TextureFormat.DXT5:
            case TextureFormat.PVRTC_RGB2:
            case TextureFormat.PVRTC_RGBA2:
            case TextureFormat.PVRTC_RGB4:
            case TextureFormat.PVRTC_RGBA4:
            case TextureFormat.ETC_RGB4:
            case TextureFormat.ETC2_RGB:
            case TextureFormat.ETC2_RGBA1:
            case TextureFormat.ETC2_RGBA8:
            case TextureFormat.ASTC_4x4:
            case TextureFormat.ASTC_5x5:
            case TextureFormat.ASTC_6x6:
            case TextureFormat.ASTC_8x8:
            case TextureFormat.ASTC_10x10:
            case TextureFormat.ASTC_12x12:
                return true;
            default:
                return false;
        }
    }

    void OnEnable()
    {
        Application.logMessageReceived += LogMessage;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= LogMessage;
    }

    void LogMessage(string message, string stackTrace, LogType type)
    {
        // 新しいメッセージをキューに追加
        logQueue.Enqueue(message);

        // キューが最大数を超えた場合、古いメッセージを削除
        while (logQueue.Count > maxLogs)
        {
            logQueue.Dequeue();
        }
    }

    void OnGUI()
    {
        // GUIスタイルが設定されていない場合、デフォルトスタイルを使用
        if (guiStyle == null)
        {
            guiStyle = new GUIStyle(GUI.skin.label);
            guiStyle.normal.textColor = Color.white;
            guiStyle.fontSize = 14;
        }

        // 画面の左上からログを表示する
        float height = 20;
        foreach (string log in logQueue)
        {
            GUILayout.Label(log, guiStyle, GUILayout.Width(Screen.width), GUILayout.Height(height));
        }
    }


}
