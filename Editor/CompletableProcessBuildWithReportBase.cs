using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Kogane
{
    /// <summary>
    /// ビルドに失敗した時のコールバックも実装できる IPreprocessBuildWithReport / IPostprocessBuildWithReport
    /// OnSuccess の後に OnComplete が呼び出されます
    /// OnSuccess はビルドに成功した時にのみ呼び出されます
    /// OnComplete はビルドに成功した時も失敗した時も呼び出されます
    /// </summary>
    public abstract class CompletableProcessBuildWithReportBase :
        IPreprocessBuildWithReport,
        IPostprocessBuildWithReport
    {
        private bool m_isCompleted;

        int IOrderedCallback.callbackOrder => CallbackOrder;

        void IPreprocessBuildWithReport.OnPreprocessBuild( BuildReport report )
        {
            m_isCompleted = false;

            void OnUpdate()
            {
                if ( BuildPipeline.isBuildingPlayer ) return;
                EditorApplication.update -= OnUpdate;
                Complete();
            }

            EditorApplication.update += OnUpdate;
            OnStart( report );
        }

        void IPostprocessBuildWithReport.OnPostprocessBuild( BuildReport report )
        {
            OnSuccess( report );
            Complete();
        }

        private void Complete()
        {
            if ( m_isCompleted ) return;
            m_isCompleted = true;
            OnComplete();
        }

        protected virtual int CallbackOrder => 0;

        protected virtual void OnStart( BuildReport   report ) { }
        protected virtual void OnSuccess( BuildReport report ) { }
        protected virtual void OnComplete()                    { }
    }
}