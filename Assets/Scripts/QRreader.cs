using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class QRreader : MonoBehaviour
{
    string _result = null;
    bool flag = false;
    WebCamTexture _webCam;
    RawImage _image;

    IEnumerator Start(){
        _image = GameObject.Find("img_display").GetComponent<RawImage>();

        yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
        if (Application.HasUserAuthorization(UserAuthorization.WebCam) == false)
        {
            Debug.LogFormat("no camera.");
            yield break;
        }
        Debug.LogFormat("camera ok.");
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices == null || devices.Length == 0)
            yield break;
        _webCam = new WebCamTexture(devices[1].name, Screen.width, Screen.height, 12);
        _webCam.Play();
    }

    void Update()
    {
        if (_webCam != null && !flag)
        {
            _result = QRCodeHelper.Read(_webCam);
            if (_result != "error"){
                Debug.LogFormat("result : " + _result);
                flag = true;
                Search(_result);
            }
            
        }
    }

    void Search(string jancode){
        string URL = "https://shopping.yahooapis.jp/ShoppingWebService/V3/itemSearch?appid=dj00aiZpPTRmYThYVnlXZ1NiTiZzPWNvbnN1bWVyc2VjcmV0Jng9YmE-&jan_code=" + jancode;

        StartCoroutine("OnSend", URL);
    }

    IEnumerator OnSend(string url){
        //URLをGETで用意
        UnityWebRequest webRequest = UnityWebRequest.Get(url);
        //URLに接続して結果が戻ってくるまで待機
        yield return webRequest.SendWebRequest();

        //エラーが出ていないかチェック
        if (webRequest.isNetworkError){
            //通信失敗
            Debug.Log(webRequest.error);

        }else{
            //通信成功
            // Debug.Log(webRequest.downloadHandler.text);
            string json = webRequest.downloadHandler.text;
            JObject jobj = (JObject)JsonConvert.DeserializeObject(json);

            if (jobj["hits"] != null){
                // 正常終了
                List<JObject> list =
                    JsonConvert.DeserializeObject<List<JObject>>(jobj["hits"].ToString());
                foreach (var item in list){
                    if(item["image"] != null){
                        Dictionary<string, string> dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(item["image"].ToString());
                        string img_url = dict["small"];
                        // Debug.Log("<color=blue>img:" + img_url + "</color>");
                        StartCoroutine("GetImage", img_url);
                    }
                }
            }
        }

    }

    IEnumerator GetImage(string img_url){
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(img_url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError){
            Debug.Log(www.error);
        }else{
            //取得した画像のテクスチャをRawImageのテクスチャに張り付ける
            _image.texture = ((DownloadHandlerTexture)www.downloadHandler).texture;
        }

    }

}