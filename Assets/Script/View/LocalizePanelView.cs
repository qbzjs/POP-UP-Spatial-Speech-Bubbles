using Script.Presenter;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Script.View
{
    public class LocalizePanelView : MonoBehaviour
    {
        [SerializeField] private MainARScenePresenter _mainARScenePresenter;
        [SerializeField] private Button _localizeStartButton;
        [SerializeField] private TextMeshProUGUI _localizeStatusText;
        [SerializeField] private TextMeshProUGUI _localizeStartButtonText;
        private void Start()
        {
            _localizeStartButton.enabled = true;
            _localizeStartButtonText.text = "Restart Localization!";

            _mainARScenePresenter.localizeStatusTextObservable
                .Subscribe(statusText =>
                {
                    Debug.Log(statusText + "statusText!!");
                    _localizeStatusText.text = statusText;
                }).AddTo(this);
        }
    } 
}

