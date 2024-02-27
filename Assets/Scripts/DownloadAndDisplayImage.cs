using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI; // UIを使用する場合はこの行が必要

public class DownloadImageAsSprite : MonoBehaviour
{
    public string imageUrl = "ここに画像のURLを入力"; // 表示したい画像のURL
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
        StartCoroutine(DownloadImage(imageUrl));
    }

    IEnumerator DownloadImage(string url)
    {


        UnityWebRequest request = UnityWebRequest.Get(url);
        //UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("画像のダウンロードに失敗: " + request.error);
        }
        else
        {
            Debug.Log("ダウンロード成功");



            // ダウンロードしたデータを取得
            byte[] imageData = request.downloadHandler.data;

            // 新しいTexture2Dを作成し、画像データをロードする
            Texture2D texture = new Texture2D(2, 2); // 初期サイズは適当でOK、LoadImage時に適切なサイズにリサイズされる
            if (texture.LoadImage(imageData))
            {
                // テクスチャの変換に成功した場合の処理
                // 例: テクスチャを表示するなど
                Debug.Log("画像をTexture2Dに変換成功");
            }
            else
            {
                Debug.LogError("画像データのTexture2Dへの変換に失敗");
            }

            // ダウンロードした画像をTexture2Dとして取得
            //Texture2D texture = DownloadHandlerTexture.GetContent(request);

            // テクスチャフォーマットを確認
            Debug.Log("ダウンロードしたテクスチャのフォーマット: " + texture.format);

            // テクスチャが圧縮可能なフォーマットであるかを確認して、可能であれば圧縮する
            //if (IsCompressibleFormat(texture.format))
            //{
                texture.Compress(true); // 高品質の圧縮を指示
                Debug.Log("テクスチャを圧縮しました。");
                Debug.Log("圧縮後のフォーマット: " + texture.format);
            //}
            //else
            //{
            //    Debug.LogWarning("テクスチャのフォーマットは圧縮可能ではありません: " + texture.format);
            //}


            // Texture2DからSpriteを生成
            Sprite newSprite = Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            // SpriteRendererまたはUIのImageコンポーネントに新しいSpriteを割り当て
            spriteRenderer.sprite = newSprite;
            // uiImage.sprite = newSprite; // UIのImageコンポーネントを使用する場合はこの行を有効に
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
