using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace UniBT.Editor
{
    public class NodeSearchWindowProvider : ScriptableObject, ISearchWindowProvider
    {
        private GraphView graphView;
        private EditorWindow graphEditor;
        private ShowOptionsSearch showOptionsSearch;
        private readonly NodeResolver nodeResolver = new NodeResolver();

        public void Initialize(GraphView graphView, EditorWindow graphEditor, ShowOptionsSearch options = null)
        {
            this.graphView = graphView;
            this.graphEditor = graphEditor;
            this.showOptionsSearch = options;
        }

        private bool IsShowNodesByType(Type type)
        {
            if (showOptionsSearch == null || !showOptionsSearch.IsShow || showOptionsSearch.SearchType == null)
            {
                return true;
            }

            return type.GetInterfaces().Any(iType => iType == showOptionsSearch.SearchType);
        }

        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            var entries = new List<SearchTreeEntry>();
            entries.Add(new SearchTreeGroupEntry(new GUIContent("Create Node")));

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type != typeof(Root) && !type.IsAbstract && type.IsSubclassOf(typeof(NodeBehavior)) && IsShowNodesByType(type))
                    {
                        entries.Add(new SearchTreeEntry(new GUIContent(type.Name))
                        {
                            level = 1,
                            userData = type
                        });
                    }
                }
            }

            return entries;
        }

        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            var type = searchTreeEntry.userData as Type;
            var node = this.nodeResolver.CreateNodeInstance(type);
            var worldMousePosition = this.graphEditor.rootVisualElement.ChangeCoordinatesTo(this.graphEditor.rootVisualElement.parent, context.screenMousePosition - this.graphEditor.position.position);
            var localMousePosition = this.graphView.contentViewContainer.WorldToLocal(worldMousePosition);
            node.SetPosition(new Rect(localMousePosition, new Vector2(100, 100)));

            this.graphView.AddElement(node);
            return true;
        }
    }
}