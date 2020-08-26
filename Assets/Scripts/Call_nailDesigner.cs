using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Call_nailDesigner : MonoBehaviour
{
    [SerializeField] GameObject mainpanel;
    [SerializeField] GameObject designpanel;
    // Start is called before the first frame update
    void Start()
    {
        mainpanel.SetActive(true);
        designpanel.SetActive(false);
    }

    public void CalldesignPanel(){
        mainpanel.SetActive(false);
        designpanel.SetActive(true);
    }

    public void BackmainPanel(){
        mainpanel.SetActive(true);
        designpanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
