using System.Collections.Generic;
using Niantic.ARDK.AR.HitTest;

namespace Script.Presenter.Interface
{
    public interface IMainARPresentable
    {
        void OnTapLocalizeStartButton();
        void OnTapScreen(IReadOnlyCollection<IARHitTestResult> hitTestResults);
    } 
}