using UnityEngine;
using System;
using System.Linq;
using System.IO;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class NailDesign : MonoBehaviour
{
    Texture2D texture;　// キャンパス
    Vector3 beforeMousePos;

    Color bgColor = Color.white;
    Color lineColor = Color.black;

    void Start()
    {
        var img = GetComponent<RawImage>();
        var rt = GetComponent<RectTransform>();
        var width = (int)rt.rect.width;
        var height = (int)rt.rect.height;
        // texture = ReadTexture("Assets/Materials/nail_plane.jpg", 600, 600);
        texture = ReadPng("Assets/Materials/nail_plane.png");

        // texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        // img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        // img.texture = texture;

        //背景が透明なTexture2Dを作る
        //http://d.hatena.ne.jp/shinriyo/20140520/p2
        // Color32[] texColors = Enumerable.Repeat<Color32>(bgColor, width * height).ToArray();
        // texture.SetPixels32(texColors);

        texture.Apply();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            beforeMousePos = GetPosition();
        }
        else if (Input.GetMouseButton(0))
        {
            Vector3 v = GetPosition();
            LineTo(beforeMousePos, v, lineColor);
            beforeMousePos = v;
            texture.Apply();
        }
    }

    /// <summary>
    /// UIのクリック座標のx、y座標を求める - 新しいguiシステムの画像 - Unity Answers
    /// https://answers.unity.com/questions/892333/find-xy-cordinates-of-click-on-uiimage-new-gui-sys.html
    /// </summary>
    public Vector3 GetPosition()
    {
        var dat = new PointerEventData(EventSystem.current);
        dat.position = Input.mousePosition;

        var rect1 = GetComponent<RectTransform>();
        var pos1 = dat.position;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rect1, pos1,
            null, out Vector2 localCursor))
            return localCursor;

        int xpos = (int)(localCursor.x);
        int ypos = (int)(localCursor.y);

        if (xpos < 0) xpos = xpos + (int)rect1.rect.width / 2;
        else xpos += (int)rect1.rect.width / 2;

        if (ypos > 0) ypos = ypos + (int)rect1.rect.height / 2;
        else ypos += (int)rect1.rect.height / 2;

        // Debug.Log("Correct Cursor Pos: " + xpos + " " + ypos);
        return new Vector3(xpos, ypos, 0);
    }

    /// Unityでお絵描きしてみる
    /// http://tech.gmo-media.jp/post/56101930112/draw-a-picture-with-unity


    public void LineTo(Vector3 start, Vector3 end, Color color)
    {
        float x = start.x, y = start.y;
        // color of pixels
        Color[] wcolor = { color };

        if (Mathf.Abs(start.x - end.x) > Mathf.Abs(start.y - end.y))
        {
            float dy = Math.Abs(end.x - start.x) < float.Epsilon ? 0 : (end.y - start.y) / (end.x - start.x);
            float dx = start.x < end.x ? 1 : -1;
            //draw line loop
            while (x > 0 && x < texture.width && y > 0 && y < texture.height)
            {
                try
                {
                    texture.SetPixels((int)x, (int)y, 1, 1, wcolor);
                    x += dx;
                    y += dx * dy;
                    if (start.x < end.x && x > end.x ||
                        start.x > end.x && x < end.x)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    break;
                }
            }
        }
        else if (Mathf.Abs(start.x - end.x) < Mathf.Abs(start.y - end.y))
        {
            float dx = Math.Abs(start.y - end.y) < float.Epsilon ? 0 : (end.x - start.x) / (end.y - start.y);
            float dy = start.y < end.y ? 1 : -1;
            while (x > 0 && x < texture.width && y > 0 && y < texture.height)
            {
                try
                {
                    texture.SetPixels((int)x, (int)y, 1, 1, wcolor);
                    x += dx * dy;
                    y += dy;
                    if (start.y < end.y && y > end.y ||
                        start.y > end.y && y < end.y)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    break;
                }
            }
        }
    }

    byte[] ReadPngFile(string path){
        Debug.Log("read start");

        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader bin = new BinaryReader(fileStream);
        byte[] values = bin.ReadBytes((int)bin.BaseStream.Length);

        bin.Close();

        return values;
    }

     Texture2D ReadPng(string path)
    {
        byte[] readBinary = ReadPngFile(path);
        Debug.Log("here");

        int pos = 16; // 16バイトから開始

        int width = 0;
        for (int i = 0; i < 4; i++)
        {
            width = width * 256 + readBinary[pos++];
        }

        int height = 0;
        for (int i = 0; i < 4; i++)
        {
            height = height * 256 + readBinary[pos++];
        }

        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(readBinary);

        return texture;
    }
}