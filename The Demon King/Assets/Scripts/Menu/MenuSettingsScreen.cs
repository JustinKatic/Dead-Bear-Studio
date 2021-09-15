using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuSettingsScreen : MonoBehaviour
{
    [SerializeField] private GameObject settings;
    [SerializeField] private GameObject soundSettings;
    [SerializeField] private GameObject playerSettings;
    //[SerializeField] private GameObject emoteSettings;

    private void OnEnable()
    {
        settings.SetActive(true);
        soundSettings.SetActive(false);
        playerSettings.SetActive(false);
        //emoteSettings.SetActive(false);
    }

    public void OnButtonClickActivateMenu(GameObject menuToActivate)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(menuToActivate.GetComponentInChildren<Selectable>().gameObject);
        
        menuToActivate.SetActive(true);
    }
    public void OnButtonClickDeactivateMenu(GameObject menuToActivate)
    {
        menuToActivate.SetActive(false);
    }
}
