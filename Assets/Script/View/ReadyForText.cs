using Script.Presenter;
using UnityEngine;
using UniRx;

namespace Script.View
{
    public class ReadyForText : MonoBehaviour
    {
        [SerializeField] private MainARScenePresenter _mainARScenePresenter;
        private void Start()
        {
            _mainARScenePresenter.readyForTextObservable
                .Subscribe(isActive =>
                {
                    gameObject.SetActive(isActive);
                }).AddTo(this);
        }
    }
}