using Niantic.ARDK.AR;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.Utilities.Input.Legacy;
using Script.Presenter.Interface;
using UnityEngine;

namespace Script.View
{
    public class MainARView : MonoBehaviour
    {
        private IARSession _arSession;
        [SerializeField] private Camera mainCamera;
       
        private IMainARPresentable presenter;
      
        public void OnTapLocalizeStartButton()
        {
            presenter.OnTapLocalizeStartButton();
            Debug.Log("OnTapLocalizeStartButton");
        }

        private void Update()
        {
            // タップされていない状態
            if (PlatformAgnosticInput.touchCount <= 0)
            {
                return;
            }

            // タッチStructを取得
            var touch = PlatformAgnosticInput.GetTouch(0);

            // タッチした瞬間のみ
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("touchPhase is Began");
                OnTapScreen(touch); 
            }
        }

        private void OnTapScreen(Touch touch)
        {
            // ArSessionのフレームを取得
            var currentFrame = _arSession.CurrentFrame;

            // frameがなければReturn
            if (currentFrame == null)
            {
                Debug.Log("currentFrame is null");
                return;
            }

            var hitTestResults = currentFrame.HitTest(
                viewportWidth: mainCamera.pixelWidth,
                viewportHeight: mainCamera.pixelHeight,
                screenPoint: touch.position,
                types: ARHitTestResultType.EstimatedHorizontalPlane);

            // どこにもヒットしなければReturn
            if (hitTestResults.Count < 0) return;
            presenter.OnTapScreen(hitTestResults);
        }
    }
}
