using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RegisteredSceneManager : MonoBehaviour
{
    
    [SerializeField] Button backButton;
    [SerializeField] Button tryButton;

    string nextscene;
    [SerializeField] string titlescenename;
    [SerializeField] string tryscenename;

    // Start is called before the first frame update
    void Start()
    {
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

    void OnDestroy()
   {
    //    alertButton.onClick.RemoveAllListeners();
   }

}
