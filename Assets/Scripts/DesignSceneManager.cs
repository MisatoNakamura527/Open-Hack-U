using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEditor;

public class DesignSceneManager : MonoBehaviour
{
    
    [SerializeField] Button backButton;
    [SerializeField] Button tryButton;

    string nextscene;
    [SerializeField] string titlescenename;
    [SerializeField] string tryscenename;
    [SerializeField] GameObject mainpanel;
    [SerializeField] GameObject designpanel;

    GameObject plane;
    // Texture texture;
    string nowbuttonname;

    public bool changepanel = false;
    public int a = 100;

    // Start is called before the first frame update
    void Start()
    {
        mainpanel.SetActive(true);
        designpanel.SetActive(false);

        // 元の画像をコピー。それぞれのボタンのテクスチャにする。
        for(int i = 1; i <= 5; i++){
            string new_path = "Assets/Materials/nail " + (i.ToString())+".png";
            AssetDatabase.CopyAsset("Assets/Materials/nail_plane.png", new_path);
            // var tex = AssetDatabase.LoadAssetAtPath<Texture2D>(new_path);
            Sprite sp = AssetDatabase.LoadAssetAtPath<Sprite>(new_path);
            var b = GameObject.Find("nali " + i.ToString()).GetComponent<Image>();
            b.sprite = sp;
        }

        changepanel = false;

        backButton.onClick.AddListener(() =>
       {
            nextscene = titlescenename;
            ChangeScene();
       });

       tryButton.onClick.AddListener(() =>
       {
            nextscene = tryscenename;
            ChangeScene();
       });

    }

    void Update(){
        Debug.Log(changepanel);
    }

    void ChangeScene(){
        SceneManager.LoadScene(nextscene);
    }

    public void CalldesignPlane(Button b){
        // ボタンのimageを書き込むテキストにセットする。
        Texture texture = b.image.mainTexture;
        nowbuttonname = b.name;

        mainpanel.SetActive(false);
        designpanel.SetActive(true);
    
        GameObject.Find("Plane").GetComponent<Renderer>().material.mainTexture = texture; 
        changepanel = true;
        Debug.Log("set texture is " + GameObject.Find("Plane").GetComponent<Renderer>().material.mainTexture.name);
    }

    public void BackmainPanel(){    
        
        // 現在のPlaneのテクスチャをアセット内に保存
        Texture2D tex = (Texture2D)GameObject.Find("Plane").GetComponent<Renderer>().material.mainTexture;
        byte[] png = tex.EncodeToPNG();
        // EditorUtility.SaveFilePanel("Save texture ", "", nowbuttonname+".png", "png");
        File.WriteAllBytes("Assets/Materials/"+nowbuttonname+".png", png);

        // 保存したテクスチャをボタンのテクスチャにする。

        mainpanel.SetActive(true);
        designpanel.SetActive(false);

        GameObject.Find(nowbuttonname).GetComponent<Image>().sprite = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Materials/"+nowbuttonname+".png");
        Debug.Log("new button image is "+GameObject.Find(nowbuttonname).GetComponent<Image>().sprite.name);

    }

    public void test(){
        Debug.Log("testtest");
    }
}