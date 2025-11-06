using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UIElements;


public class MainManuEvents : MonoBehaviour
{
    private UIDocument _document;
    private Button _button;

    private List<Button> _menuButtons = new List<Button>();

    private AudioSource _audioSource;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _document = GetComponent<UIDocument>();
        //_button = _document.rootVisualElement.Q("StartGameButton") as Button;
        //_button.RegisterCallbackOnce<ClickEvent>(OnPlayGameClick);

        _menuButtons = _document.rootVisualElement.Query<Button>().ToList();

        for (int i = 0; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].RegisterCallback<ClickEvent>(OnAllButtonsClick);
        }
    }

    private void OnDnable()
    {
        //_button.UnregisterCallback<ClickEvent>(OnPlayGameClick);

        for (int i = 1; i < _menuButtons.Count; i++)
        {
            _menuButtons[i].UnregisterCallback<ClickEvent>(OnAllButtonsClick);
        }


    }


    private void OnAllButtonsClick(ClickEvent evt)
    {
        Debug.Log("Button Clicked: " + (evt.currentTarget as Button)?.name);

        // **VIKTIG ENDRING:** Spill lyden her!
        if (_audioSource != null)
        {
            _audioSource.Play();
        }
    }

    //private void OnPlayGameClick(ClickEvent evt)
    //{
    //    _audioSource.Play();
    //}
}
