using System;
using System.Collections.Generic;
using System.Linq;
using ModulesFramework;
using ModulesFrameworkUnity.Debug.Utils;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    public class ModuleNode : Node
    {
        private readonly List<ModuleNode> _submodules = new();
        private Vector2 _position;
        private readonly ModulesNodeTitle _modulesNodeTitle;
        private VisualElement _composedOfContainer;
        public ModuleNode ParentModule { get; private set; }

        public float SelfHeight { get; private set; }
        public float AdjustedHeight { get; private set; }

        public float Width { get; private set; }
        public int Level { get; private set; }
        public Type ModuleType { get; private set; }
        public bool IsParent => _submodules.Count > 0;
        public int SubmodulesCount => _submodules.Count;

        public ModuleNode(int level, Type moduleType)
        {
            Level = level;
            ModuleType = moduleType;
            _modulesNodeTitle = new ModulesNodeTitle(titleButtonContainer);
            _modulesNodeTitle.OnInitDestroyClick += OnInitClicked;
            _modulesNodeTitle.OnActivateClick += OnActivateClicked;
        }

        private void OnActivateClicked()
        {
            var module = DebugUtils.GetCurrentWorld().GetModule(ModuleType);
            if (module.IsActive)
            {
                DebugUtils.GetCurrentWorld().DeactivateModule(ModuleType);
            }
            else
            {
                DebugUtils.GetCurrentWorld().ActivateModule(ModuleType);
            }
        }

        private void OnInitClicked()
        {
            var module = DebugUtils.GetCurrentWorld().GetModule(ModuleType);
            if (module.IsInitialized)
            {
                DebugUtils.GetCurrentWorld().DestroyModule(ModuleType);
            }
            else
            {
                DebugUtils.GetCurrentWorld().InitModule(ModuleType);
            }
        }

        public void RefreshWidth()
        {
            const float buttonsMax = 160;
            Width = 10 * title.Length + buttonsMax;
            var inputPortMaxWidth = GetMaxWidth(inputContainer.Children().Where(v => v is Port).Cast<Port>());
            var outputPortMaxWidth = GetMaxWidth(outputContainer.Children().Where(v => v is Port).Cast<Port>());
            Width = Math.Max(Width, inputPortMaxWidth + outputPortMaxWidth);
            style.width = Width;
        }

        private float GetMaxWidth(IEnumerable<Port> ports)
        {
            return ports.Select(port => 10 * port.portName.Length).Prepend(0).Max();
        }

        public void AddChild(ModuleNode childNode)
        {
            _submodules.Add(childNode);
            childNode.SetParent(this);
        }

        public void AddComposed(string moduleName)
        {
            if (_composedOfContainer == null)
            {
                _composedOfContainer = new VisualElement();
                _composedOfContainer.AddToClassList("modules-tab--graph--composed-of-container");
                mainContainer.Add(_composedOfContainer);
                _composedOfContainer.PlaceInFront(titleContainer);
            }
            var label = new Label
            {
                text = moduleName
            };
            _composedOfContainer.Add(label);
        }

        private void SetParent(ModuleNode parentNode)
        {
            ParentModule = parentNode;
        }

        public void RefreshAdjustedHeight()
        {
            float defaultHeight = Level == 0 &&_submodules.Count == 0 ? 40 : 65;
            const float submoduleHeight = 24;
            AdjustedHeight = defaultHeight;
            SelfHeight = defaultHeight;

            if (_submodules.Count > 0)
            {
                AdjustedHeight = 0;
                SelfHeight += _submodules.Count * submoduleHeight;
                foreach (var moduleNode in _submodules)
                {
                    AdjustedHeight += moduleNode.AdjustedHeight;
                }
            }
            else if (ParentModule != null)
            {
                AdjustedHeight += submoduleHeight;
                SelfHeight += submoduleHeight;
            }
        }

        public void SetX(float x)
        {
            var oldRect = GetPosition();
            var oldPos = _position;
            _position = new Vector2(x, oldPos.y);
            SetPosition(new Rect(_position, oldRect.size));
        }

        public void SetY(float y)
        {
            var oldRect = GetPosition();
            var oldPos = _position;
            _position = new Vector2(oldPos.x, y);
            SetPosition(new Rect(_position, oldRect.size));
        }

        public void AlignChildren(float y, ref Dictionary<int, float> rowY)
        {
            if (!IsParent)
                return;

            rowY[Level + 1] = Math.Max(y, rowY[Level + 1]);
            foreach (var node in _submodules)
            {
                node.SetY(rowY[Level + 1]);
                rowY[Level + 1] += node.SelfHeight;
                node.AlignChildren(y, ref rowY);
            }
        }

        public float CalculateChildrenOffset()
        {
            var selfOffset = (AdjustedHeight - SelfHeight) / 2f;
            if (IsParent)
            {
                var childrenDiff = _position.y - _submodules.First()._position.y;
                selfOffset -= Math.Abs(childrenDiff);
                selfOffset = Math.Max(0, selfOffset);
            }

            return selfOffset;
        }

        public void OffsetChildrenY(float diff, ModuleNode start)
        {
            var index = _submodules.IndexOf(start);
            for (var i = index; i < _submodules.Count; i++)
                _submodules[i].AddY(diff);
        }

        public void AddY(float diff)
        {
            SetY(_position.y + diff);
        }

        public void SetPlayMode()
        {
            _modulesNodeTitle.ShowButtons();
            style.opacity = 0.5f;
        }

        public void OnModuleInit()
        {
            titleContainer.style.unityFontStyleAndWeight = FontStyle.Italic;
            _modulesNodeTitle.UpdateInit(true);
            style.opacity = 1;
        }

        public void OnModuleActivated()
        {
            titleContainer.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic;
            _modulesNodeTitle.UpdateActivate(true);
        }

        public void OnModuleDeactivated()
        {
            titleContainer.style.unityFontStyleAndWeight = FontStyle.Italic;
            _modulesNodeTitle.UpdateActivate(false);
        }

        public void OnModuleDestroyed()
        {
            titleContainer.style.unityFontStyleAndWeight = FontStyle.Normal;
            _modulesNodeTitle.UpdateInit(false);
            style.opacity = 0.5f;
        }
    }
}
