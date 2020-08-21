using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [SerializeField] Button designButton;
    [SerializeField] Button registeredButton;
    [SerializeField] Button nailButton;

    string nextscene;

    [SerializeField] string designscene;
    [SerializeField] string registeredscene;
    [SerializeField] string nailscene;



   void Start()
   {
       designButton.onClick.AddListener(() =>
       {
            nextscene = designscene;
            ChangeScene();
       });

       registeredButton.onClick.AddListener(() =>
       {
            nextscene = registeredscene;
            ChangeScene();
       });

       nailButton.onClick.AddListener(() =>
       {
           nextscene = nailscene;
            ChangeScene();
       });
   }

    void ChangeScene(){
        SceneManager.LoadScene(nextscene);
    }
   void OnDestroy()
   {
    //    alertButton.onClick.RemoveAllListeners();
   }
}
