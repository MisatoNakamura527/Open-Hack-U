using UnityEngine;
using System.Collections;
using System.IO;

using UnityEngine.UI;

public class NailDesign : MonoBehaviour
{
    Texture2D drawTexture;
    Color[] buffer;
    bool touching = false;
    Vector2 prevPoint;

    GameObject pl;
    DesignSceneManager designSceneManager;
    public enum Tools{
        pen_small, pen_normal, pen_big,
        eraser_small, eraser_normal, eraser_big,
        fill
    }
    public Tools tool;
    public string color_code;


    public int brushSize;
    public static int brush_small = 1, brush_nornal = 3, brush_big = 5;
    public Color selected_color;

    void Start()
    {   

        pl = GameObject.Find("BackGround");
        designSceneManager = pl.GetComponent<DesignSceneManager>();

        // tool = Tools.pen_normal;
        tool = Tools.pen_big;
        brushSize = brush_big;
        selected_color = Color.red;
        
    }

    public void DrawLine(Vector2 p, Vector2 q)
    {
        // 線pqを書く
        var lerpNum = 10;
        for(int i=0; i < lerpNum + 1; i++)
        {
            var r = Vector2.Lerp(p, q, i * (1.0f / lerpNum));
            Draw(r);
        }
    }

    public void Draw(Vector2 p)
    {
        // 点pに点をぬる
        p.x = (int)p.x;
        p.y = (int)p.y;

        // 消しゴムならclear
        Color color = Color.black;

        // if(tool == Tools.eraser_small || tool == Tools.eraser_normal || tool == Tools.eraser_big) color = Color.clear;
        // else color = selected_color;

        Debug.Log(color);

        for (int x = Mathf.Max(0, (int)(p.x - brushSize-1)); x < Mathf.Min(drawTexture.width, (int)(p.x + brushSize+1)); x++)
        {
            for (int y = Mathf.Max(0, (int)(p.y - brushSize-1)); y < Mathf.Min(drawTexture.height, (int)(p.y + brushSize+1)); y++)
            {
                if (Mathf.Pow(p.x - x,2) + Mathf.Pow(p.y -y, 2) < Mathf.Pow(brushSize, 2))
                {   
                    // if(drawTexture.GetPixel(x, y).a == 1.0f){
                    //     // 透明のとこはむし
                    //     continue;
                    // }
                    buffer.SetValue(color, x + drawTexture.width * y);
                }
            }
        }
    }

    void Fillin(){
        // 塗りつぶし
        // マテリアルの色を選択された色にする。大丈夫なのかなこれ
        GetComponent<Renderer>().material.color = selected_color;
    }

    void Changedrowtexture(){
        // パネルが変わった時にplaneのテクスチャを変更する。
        Texture2D mainTexture = (Texture2D)GetComponent<Renderer>().material.mainTexture;

        Color[] pixels = mainTexture.GetPixels();

        buffer = new Color[pixels.Length];
        pixels.CopyTo(buffer, 0);

        drawTexture = new Texture2D(mainTexture.width, mainTexture.height, TextureFormat.RGBA32, false);
        drawTexture.filterMode = FilterMode.Point;

    }

    void Update()
    {

        if(designSceneManager.changepanel){
            Changedrowtexture();
            designSceneManager.changepanel = false;
        }

        if(touching){
            Debug.Log("touched!!!");
        }

        if(tool == Tools.fill){
            Fillin();
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            // Debug.Log(Physics.Raycast(Input.mousePosition,ray.direction, 100.0f));

            if (Physics.Raycast(ray, out hit, 1000.0f))
            {
                // タップにぶつかったらその場所を出力
                Debug.Log("butukaru! " + hit.collider.gameObject.transform.position);
                var drawPoint = new Vector2(hit.textureCoord.x * drawTexture.width, hit.textureCoord.y * drawTexture.height);
                if (touching) {
                    DrawLine(prevPoint, drawPoint);
                }else{
                    Draw(drawPoint);
                }
                prevPoint = drawPoint;
                touching = true;
            }else
            {
                touching = false;
            }
            drawTexture.SetPixels(buffer);
            drawTexture.Apply();
            GetComponent<Renderer>().material.mainTexture = drawTexture;
        }else
        {
            touching = false;
        }
    }
}