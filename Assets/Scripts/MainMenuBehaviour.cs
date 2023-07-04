using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuBehaviour : MonoBehaviour
{
    public GameObject ButtonStart,
        ButtonCustomize,
        ButtonOptions;

    // Start is called before the first frame update
    void Start() {
        ButtonStart    .GetComponent<Button>().onClick.AddListener( () => StartPlaying()   );
        ButtonCustomize.GetComponent<Button>().onClick.AddListener( () => StartCustomize() );
        ButtonOptions  .GetComponent<Button>().onClick.AddListener( () => StartOptions()   );
    }

    // Unload the MainMenu Scene so that play can resume
    void StartPlaying() {
        ButtonOptions.SetActive( false );
        ButtonCustomize.SetActive( false );
        ButtonStart.SetActive( false );

        SceneManager.UnloadSceneAsync( "Add Menu" );
    }

    void StartCustomize() {
        Debug.Log( "\'Customize\' clicked. This button doesn't do anything yet!" );
    }

    void StartOptions() {
        Debug.Log( "\'Settings\' clicked. This button doesn't do anything yet!" );
    }
}
