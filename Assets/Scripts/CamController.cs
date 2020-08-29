namespace OpenCvSharp
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEngine;
    using UnityEngine.UI;
    using OpenCvSharp;
    using System.IO;
    using System;
    using System.Text;
    using Newtonsoft.Json;

    public class CamController : MonoBehaviour{
        int width = 1920;
        int height = 1080;
        int fps = 30;
        int cnt = 500;
        Texture2D cap_tex;
        Texture2D out_tex;
        WebCamTexture webcamTexture;
        Color32[] colors = null;
        Process process;
        StreamWriter streamWriter;
        StreamReader streamReader;

        string pyExePath = @"/Users/yoshidaairi/.pyenv/shims/python";
        string pyCodePath = @"./Assets/nailtracking/nail_detect.py";

        IEnumerator Init()
        {
            while (true)
            {
                if (webcamTexture.width > 16 && webcamTexture.height > 16)
                {
                    colors = new Color32[webcamTexture.width * webcamTexture.height];
                    cap_tex = new Texture2D(webcamTexture.width, webcamTexture.height, TextureFormat.RGBA32, false);
                    break;
                }
                yield return null;
            }
        }
        // Start is called before the first frame update
        void Start()
        {
            WebCamDevice[] devices = WebCamTexture.devices;
            webcamTexture = new WebCamTexture(devices[1].name, this.width, this.height, this.fps);
            webcamTexture.Play();

            StartCoroutine(Init());

            //外部プロセスの設定
            ProcessStartInfo processStartInfo = new ProcessStartInfo() {
                FileName = pyExePath, //実行file(python)
                UseShellExecute = false,//shellの利用
                CreateNoWindow = true, //windowを開かない
                RedirectStandardOutput = true, //テキスト出力をStandardOutputストリームに書き込むかどうか
                // StandardOutputEncoding = Encoding.UTF8,
                RedirectStandardInput = true, 
                Arguments = pyCodePath, //実行するスクリプト 空白挟んで引数(複数可)
            };

            //外部プロセスの開始
            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.OutputDataReceived += OutputHandler;
            process.Start();
            // process = Process.Start(processStartInfo);
            streamWriter = process.StandardInput;
            // streamReader = process.StandardOutput;
            process.BeginOutputReadLine();
        }

        // Update is called once per frame
        void Update(){
            cnt++;
            
            if (colors != null){
                webcamTexture.GetPixels32(colors);
                cap_tex.SetPixels32(this.colors);
                cap_tex.Apply();

                if(cnt > 330){  
                    // pythonへの入力
                    byte[] png = cap_tex.EncodeToPNG();
                    string b64_png = Convert.ToBase64String(png);              
                    streamWriter.WriteLine(b64_png);

                    //pythonから出力を得る
                    // string json1 = streamReader.ReadLine();
                    // UnityEngine.Debug.Log("<color=red>json:" + json1 + ":</color>");
                    cnt = 0;
                }

                GetComponent<RawImage>().texture = cap_tex;
            }
        }

        static void OutputHandler(object o, DataReceivedEventArgs args) {
            UnityEngine.Debug.Log("<color=green>" + args.Data + "</color>");
        }

        void OnApplicationQuit(){
            //外部プロセスの終了
            UnityEngine.Debug.Log("<color=blue>Quit</color>");
            streamWriter.Close();
            // streamReader.Close();
            process.Dispose();
        }
    }

    class nailData{
        public decimal boxnum { get; set; }
        public List<Result> result { get; set; }

        public override string ToString()
        {
            string result_str = "";
            foreach(Result r in result)
            {
                result_str += "[" + r + "]";
            }
            return "{boxnum: " + boxnum + ", result: " + result_str + "}";
        }
    }

    class Result{
        public decimal startX { get; set; }
        public decimal endX { get; set; }
        public decimal startY { get; set; }
        public decimal endY { get; set; }

        public override string ToString()
        {
            return "{startX: " + startX + ", endX: " + endX + ", startY: " + startY + ", endY: " + endY + "}";
        }
    }

}
