using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    Texture texture;

    // Start is called before the first frame update
    void Start()
    {
        mainpanel.SetActive(true);
        designpanel.SetActive(false);
        // 貼り付けるテクスチャのシェーダーをセット

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

    void ChangeScene(){
        SceneManager.LoadScene(nextscene);
    }

    public void CalldesignPlane(Button b){
        var img  = b.image;
        // ボタンのimageを書き込むテキストにセットする。

        mainpanel.SetActive(false);
        designpanel.SetActive(true);
        SetTexture(img);
    }

    public void BackmainPanel(){
        mainpanel.SetActive(true);
        designpanel.SetActive(false);
    }

    private void SetTexture(Image img){
        // 受け取ったImageをPlaneにセット
        texture = img.mainTexture;
        // Debug.Log(texture);
        Texture t = GameObject.Find("Plane").GetComponent<Renderer>().material.mainTexture;
        //  = texture;
    }
}