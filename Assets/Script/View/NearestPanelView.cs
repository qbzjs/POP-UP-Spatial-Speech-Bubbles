using Niantic.ARDK.VPSCoverage;
using Script.Presenter;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Script.View
{
    public class NearestPanelView : MonoBehaviour
    {
        
        [SerializeField] private MainARScenePresenter _mainARScenePresenter;
        [SerializeField] private Text _nearestAreaText;
        [SerializeField] private GameObject _nearestAreaPanel;

        private void Start()
        {
            _mainARScenePresenter.nearestSpotObservable
                .Subscribe(nearestSpot =>
                {
                    ShowNearestPanel(nearestSpot);
                })
                .AddTo(this);

            _mainARScenePresenter.nearestSpotHiddenObservable
                .Subscribe(_ =>
                {
                    HiddenNearestPanel();
                }).AddTo(this);
        }

        public void ShowNearestPanel(LocalizationTarget target)
        {
            Debug.Log("ShowNearestPanel");
            _nearestAreaPanel.gameObject.SetActive(true);
            _nearestAreaText.text = $"スポットの名前: {target.Name}";
        }
    
        public void HiddenNearestPanel()
        {
            Debug.Log("HiddenNearestPanel");
            _nearestAreaPanel.gameObject.SetActive(false);
        }

    }
}