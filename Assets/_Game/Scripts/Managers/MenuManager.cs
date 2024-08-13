using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Loading Panel")]
    [SerializeField] private float loadingTime;
    [SerializeField] private GameObject goLoadingPanel;
    [SerializeField] private GameObject goMenuPanel;
    [SerializeField] private Slider sliderLoadingBar;
    [Header("Manu Panel")]
    [SerializeField] private List<Sprite> avatarSprites;
    [SerializeField] private Transform avatarsHolderParent;
    [SerializeField] private GameObject avatarPrefab;
    [SerializeField] private Button buttonConfirmRestore;
    [SerializeField] private Button buttonStartGame;
    [SerializeField] private int gamePlaySceneIndex;
    // Start is called before the first frame update
    void Start()
    {
        InitVariables();
        StartCoroutine(StartLoadingBar());
        GameDelegate.Instance.Initialize();
    }
    private void OnEnable()
    {
        AvatarPicker.ActionCurrentAvatarIndex += GetSelectedAvatarIndex;
    }
    private void OnDisable()
    {
        AvatarPicker.ActionCurrentAvatarIndex -= GetSelectedAvatarIndex;
    }
    private void InitVariables()
    {
        int index = 0;
        foreach(var sprite in avatarSprites)
        {
            GameObject avatar = Instantiate(avatarPrefab, avatarsHolderParent);
            avatar.GetComponent<AvatarPicker>().Init(sprite, index);
            index++;
        }
        buttonConfirmRestore.onClick.AddListener(OnClickConfirmRestoreButton);
        buttonStartGame.onClick.AddListener(OnClickStartGameButton);
    }
    IEnumerator StartLoadingBar()
    {
        float elapsedTime = 0f;
        float startValue = sliderLoadingBar.value;
        float endValue = 1f;

        while (elapsedTime < loadingTime)
        {
            elapsedTime += Time.deltaTime;
            sliderLoadingBar.value = Mathf.Lerp(startValue, endValue, elapsedTime / loadingTime);
            yield return null; 
        }
        sliderLoadingBar.value = endValue;
        SetGameObjectState(goLoadingPanel, false);
        SetGameObjectState(goMenuPanel, true);
    }
    private void GetSelectedAvatarIndex(int index) //When any avatar click by user then this method call by a Action.
    {
        if (index == -1) return;
        int serial = 0;
        foreach (Transform avatar in avatarsHolderParent)
        {
            if(serial != index)
            {
                avatar.GetChild(1).gameObject.SetActive(false);
            }
            ++serial;
        }
        GameDelegate.Instance.PlayerSelectedAvatarIndex = index;
        buttonStartGame.gameObject.SetActive(true);
    }
    #region Buttons_OnClickMethods
    private void OnClickConfirmRestoreButton()
    {
        SoundManager.Instance.Play("click");
        PlayerPrefs.DeleteAll();
    }
    private void OnClickStartGameButton()
    {
        SoundManager.Instance.Play("click");
        SceneManager.LoadScene(gamePlaySceneIndex);
    }
    #endregion
    private void SetGameObjectState(GameObject go, bool state)
    {
        go.SetActive(state);
    }
}
