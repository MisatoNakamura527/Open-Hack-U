using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TrySceneManager : MonoBehaviour
{
    
    [SerializeField] Button backButton;

    string nextscene;
    [SerializeField] string titlescenename;

    // Start is called before the first frame update
    void Start()
    {
        backButton.onClick.AddListener(() =>
       {
            nextscene = titlescenename;
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
