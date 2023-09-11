using System;
using System.Collections.Generic;
using System.Linq;
using Niantic.ARDK;
using Niantic.ARDK.AR;
using Niantic.ARDK.AR.ARSessionEventArgs;
using Niantic.ARDK.AR.Configuration;
using Niantic.ARDK.AR.HitTest;
using Niantic.ARDK.AR.WayspotAnchors;
using Niantic.ARDK.LocationService;
using Niantic.ARDK.Utilities;
using Niantic.ARDK.VPSCoverage;
using Script.Model;
using Script.Model.Interface;
using Script.Presenter.Interface;
using Script.View;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;
using Unit = UniRx.Unit;


namespace Script.Presenter
{
    public class MainARScenePresenter: MonoBehaviour, IMainARPresentable 
    {
        [SerializeField] private GameObject messagePrefab;
        [SerializeField] private MainARView _arView;
        [SerializeField] private InputField _inputField;
        private IARSession _arSession;
        private WayspotAnchorService _wayspotAnchorService;
        private ILocationService _locationService;
        private IFirebaseRepository _firebaseRepository = new FirebaseRepository();
        private IWayspotRepository _wayspotRepository;
        private Dictionary<Guid, GameObject> _anchors = new Dictionary<Guid, GameObject>();
        private PostLetterData _postLetterData = new PostLetterData();
    
        public IObservable<LocalizationTarget> nearestSpotObservable => _nearestSpotSubject;
        private Subject<LocalizationTarget> _nearestSpotSubject = new Subject<LocalizationTarget>();
    
        public IObservable<Unit> nearestSpotHiddenObservable => _nearestSpotHiddenSubject;
        private Subject<Unit> _nearestSpotHiddenSubject = new Subject<Unit>();

        public IObservable<bool> sendLetterButtonHiddenObservable => _sendLetterButtonHiddenSubject;
        private Subject<bool> _sendLetterButtonHiddenSubject = new Subject<bool>();
    
        public IObservable<string> localizeStatusTextObservable => _localizeStatusTextSubject;
        private Subject<string> _localizeStatusTextSubject = new Subject<string>();
    
        public IObservable<bool> readyForTextObservable => _readyForTextSubject;
        private Subject<bool> _readyForTextSubject = new Subject<bool>();
        
        private void Start()
        {
            // ARSessionをインスタンス化
            _arSession = ARSessionFactory.Create();
            // インスタンス化をコールバック
            ARSessionFactory.SessionInitialized += OnSessionInitialized;
            _wayspotAnchorService.LocalizationStateUpdated += OnLocalizationStateUpdated;
        
            var locationService = LocationServiceFactory.Create(RuntimeEnvironment.Default);
            _wayspotRepository = new WayspotRepository(
                CoverageClientFactory.Create(),
                locationService);

            _arView.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    ChangeStatusText();
                }).AddTo(this);
            
            _inputField.onValueChanged.AsObservable()
                .Subscribe(text =>
                {
                    this._postLetterData.message = text;
                }).AddTo(this);
        }

        private void OnLocalizationStateUpdated(LocalizationStateUpdatedArgs args)
        {
            Debug.Log("OnLocalizationStateUpdated");
            var localizationState = args.State;
            if (localizationState != LocalizationState.Localized)
            {
                _sendLetterButtonHiddenSubject.OnNext(false);
                _nearestSpotHiddenSubject.OnNext(Unit.Default);
                _postLetterData.ResetData();
            }
            else
            {
                GetNearestSpot();  
            }
        }

        private void ChangeStatusText()
        {
            switch (_wayspotAnchorService.LocalizationState)
            {
                case LocalizationState.Failed:
                    _localizeStatusTextSubject.OnNext("Localization Failed");
                    break;
                case LocalizationState.Initializing:
                    _localizeStatusTextSubject.OnNext("Localization Initializing");
                    break;
                case LocalizationState.Localizing:
                    _localizeStatusTextSubject.OnNext("Localization In Progress");
                    break;
                case LocalizationState.Stopped:
                    _localizeStatusTextSubject.OnNext("Localization Stopped");
                    break;
                case LocalizationState.Uninitialized:
                    _localizeStatusTextSubject.OnNext("ARSession is uninitialized");
                    break;
                case LocalizationState.Localized:
                    break;
            }
        }

        private async void GetNearestSpot()
        {
            var spot = await _wayspotRepository.GetNearestPlace();
            _postLetterData.SetWaySpotId(spot.Identifier);
            RestoreWaySpot(spot);
        
            _localizeStatusTextSubject.OnNext("Localized !!");
            _nearestSpotSubject.OnNext(spot);
            _sendLetterButtonHiddenSubject.OnNext(true);
        }

        private void OnSessionInitialized(AnyARSessionInitializedArgs args)
        {
            Debug.Log("OnSessionInitialized");
            // ARSessionのConfigを初期化
            var configuration = ARWorldTrackingConfigurationFactory.Create();
            // VPSのドリフト(デバイスの位置や向きの推定が時間と共にズレる現象）
            configuration.WorldAlignment = WorldAlignment.Gravity;
            configuration.PlaneDetection = PlaneDetection.Horizontal;

            // 光源推定をfalse
            configuration.IsLightEstimationEnabled = false;
            // 自動フォーカスをfalse
            configuration.IsAutoFocusEnabled = false;
            // 深度推定をfalse
            configuration.IsDepthEnabled = false;
            // 共有ARをfalse
            configuration.IsSharedExperienceEnabled = false;

            // Ranのコールバックを受け取る
            _arSession.Ran += OnSessionRan;
            // ARSessionをRun
            _arSession.Run(configuration);
        }

        public void OnTapLocalizeStartButton()
        {
            // これをスタートするとバックグラウンドでローカライゼーションプロセスを自動的に開始する。
            if (_locationService != null) _locationService.Start();
            _wayspotAnchorService.Restart(); 
        }

        public void OnSessionRan(ARSessionRanArgs args)
        {
            // wayspotのConfigを初期化
            var wayspotAnchorsConfiguration = WayspotAnchorsConfigurationFactory.Create();

            // deviceの位置情報の許可が必要となるので ARSessionRuntimeEnvironment=> リモート可動化など
            _locationService = LocationServiceFactory.Create(_arSession.RuntimeEnvironment);

            // wayspot anchor serviceを作成 => ARSessionとlocationServiceとwaysportのconfigが必要
            _wayspotAnchorService = new WayspotAnchorService(
                arSession: _arSession,
                locationService: _locationService,
                wayspotAnchorsConfiguration: wayspotAnchorsConfiguration);
        }

        // ReSharper disable Unity.PerformanceAnalysis
        public void OnTapScreen(IReadOnlyCollection<IARHitTestResult> hitTestResults)
        {
            if (_wayspotAnchorService.LocalizationState != LocalizationState.Localized || 
                !_postLetterData.IsReady) return;
            // 一番最初にヒットした場所の姿勢行列
            Matrix4x4 poseMatrix = hitTestResults.FirstOrDefault()!.WorldTransform;
            // waySpotにマトリクスを送る
            var anchors = _wayspotAnchorService.CreateWayspotAnchors(poseMatrix);
            // process anchors
            foreach (var wayspotAnchor in anchors)
            {
                // すでにKeyが入っているのであればここはみる必要がない
                if (_anchors.ContainsKey(wayspotAnchor.ID)) return;
            
                // guidを取得
                var id = wayspotAnchor.ID;
            
                messagePrefab.GetComponent<SpatialSpeechBubble.TextWriter>().text = _postLetterData.message;
                var anchor = Instantiate(messagePrefab);
                anchor.transform.position = poseMatrix.ToPosition();
                _anchors.Add(id, anchor);

                // トラッキング状態が更新されたら呼び出すイベントを追加
                wayspotAnchor.TransformUpdated += OnTrackingStateUpdated;

                var pose = poseMatrix.ToPosition();
                // ローカライゼーションが完了している場合 && まだKeyに入っておらず保存していない場合はFirebaseに保存する。
                var letter = new LetterData
                {
                    wayspotId = _postLetterData.wayspotId,
                    message = _postLetterData.message,
                    payLoad = wayspotAnchor.Payload.Serialize(),
                    positionX = pose.x,
                    positionY = pose.y,
                    positionZ = pose.z
                };
                PostLetterData(letter: letter);
            }
        }
        private void PostLetterData(LetterData letter)
        {
            _firebaseRepository.PostLetterData(letter);
            _postLetterData.ResetMessage();
            _readyForTextSubject.OnNext(false);
        }
    
        private void OnTrackingStateUpdated(WayspotAnchorResolvedArgs args)
        {
            Debug.Log("OnTrackingStateUpdated");
            //　更新されるたびに今のTransformを取得
            var anchor = _anchors[args.ID].transform;
            anchor.rotation = args.Rotation;
            anchor.position = args.Position;
            anchor.gameObject.SetActive(true);
        }

        public void ReceiveLetter(string message)
        {
#if UNITY_IOS
            Debug.Log("ReceiveLetter" + message);
            _postLetterData.SetMessage(message);
            _readyForTextSubject.OnNext(true);
#endif
        }

        public async void RestoreWaySpot(LocalizationTarget target)
        {
            var letters = await _firebaseRepository.GetLetterData(target.Identifier); 
            var wayspotAnchors = MakeWaySpotAnchor(letters);
        
            for (int i = 0; i < wayspotAnchors.Length; i++)
            {
                var wayspotAnchor = wayspotAnchors[i];
                var letter = letters[i];
                // すでにKeyが入っているのであればここはみる必要がない
                if (_anchors.ContainsKey(wayspotAnchor.ID)) return;
                // guidを取得
                var id = wayspotAnchor.ID;
                messagePrefab.GetComponent<SpatialSpeechBubble.TextWriter>().text = letter.message;
                var anchor = Instantiate(messagePrefab);
                anchor.transform.position = new Vector3(
                    letter.positionX, 
                    letter.positionY, 
                    letter.positionZ);
                _anchors.Add(id, anchor);
                // トラッキング状態が更新されたら呼び出すイベントを追加
                wayspotAnchor.TransformUpdated += OnTrackingStateUpdated;
            }
        }
    
        private IWayspotAnchor[] MakeWaySpotAnchor(List<LetterData> list)
        {
            Debug.Log("LoadLetterData");
            List<WayspotAnchorPayload> payloads = new List<WayspotAnchorPayload>();
            foreach (var letter in list)
            {
                var payLoadData = letter.payLoad;
                var payload = WayspotAnchorPayload.Deserialize(payLoadData);
                payloads.Add(payload);
            }
            var wayspotAnchors = _wayspotAnchorService.RestoreWayspotAnchors(payloads.ToArray());
            return wayspotAnchors;
        }
    }
}
