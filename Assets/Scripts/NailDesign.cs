using UnityEngine;
using System.Collections;
using System.IO;

public class NailDesign : MonoBehaviour
{
    Texture2D drawTexture;
    Color[] buffer;
    bool touching = false;
    Vector2 prevPoint;

    GameObject pl;
    DesignSceneManager designSceneManager;



    void Start()
    {

        pl = GameObject.Find("BackGround");
        designSceneManager = pl.GetComponent<DesignSceneManager>();



    }

    public void DrawLine(Vector2 p, Vector2 q)
    {
        var lerpNum = 10;
        for(int i=0; i < lerpNum + 1; i++)
        {
            var r = Vector2.Lerp(p, q, i * (1.0f / lerpNum));
            Draw(r);
        }
    }

    public void Draw(Vector2 p)
    {
        p.x = (int)p.x;
        p.y = (int)p.y;

        var brushSize = 5;
        var color = Color.black;
        for (int x = Mathf.Max(0, (int)(p.x - brushSize-1)); x < Mathf.Min(drawTexture.width, (int)(p.x + brushSize+1)); x++)
        {
            for (int y = Mathf.Max(0, (int)(p.y - brushSize-1)); y < Mathf.Min(drawTexture.height, (int)(p.y + brushSize+1)); y++)
            {
                if (Mathf.Pow(p.x - x,2) + Mathf.Pow(p.y -y, 2) < Mathf.Pow(brushSize, 2))
                {
                    buffer.SetValue(color, x + drawTexture.width * y);
                }
            }
        }
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

        if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1500.0f))
            {
                // タップにぶつかったらその場所を出力
                Debug.Log(hit.collider.gameObject.transform.position);
                var drawPoint = new Vector2(hit.textureCoord.x * drawTexture.width, hit.textureCoord.y * drawTexture.height);
                if (touching) {
                    DrawLine(prevPoint, drawPoint);
                }
                else
                {
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