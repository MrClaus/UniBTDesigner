using UnityEditor;
using UnityEngine;
using System.Linq;

namespace UniBT.Editor
{
    [CustomEditor(typeof(BehaviorTree))]
    public class BehaviorTreeEditor : UnityEditor.Editor
    {
        private bool _showNodesByType;
        private BehaviorTree _bt;

        private void OnEnable()
        {
            _bt = target as BehaviorTree;
            _showNodesByType = _bt.ShowNodesByType;
        }

        private void OnShowNodesByType(bool isShow, System.Type btType)
        {
            _bt.ShowNodesByType = isShow;
            if (isShow)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Behavior Tree Type:", $"{btType}");
                EditorGUI.EndDisabledGroup();
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI ();

            System.Type btType = _bt?.GetComponent<IBehaviorTreeType>()?.GetType().GetInterfaces()
                .SingleOrDefault(iType => iType != typeof(IBehaviorTreeType) && typeof(IBehaviorTreeType).IsAssignableFrom(iType));

            _showNodesByType = EditorGUILayout.Toggle("Show Nodes By Type", _showNodesByType);
            OnShowNodesByType(_showNodesByType, btType);

            if (GUILayout.Button("Open Behavior Tree"))
            {
                GraphEditorWindow.Show(_bt, new ShowOptionsSearch(_showNodesByType, btType));
            }
        }
    }
}