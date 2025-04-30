using System;
using System.Collections.Generic;
using ModulesFramework;
using ModulesFramework.Utils.Types;
using ModulesFrameworkUnity.Debug.Utils;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Modules
{
    public class ModulesListElement : VisualElement
    {
        private readonly Foldout _foldout;
        private List<ModulesListElement> _submodules = new List<ModulesListElement>();
        private Button _initDestroyBtn;
        private Button _activateBtn;

        public Type ModuleType { get; private set; }

        public ModulesListElement()
        {
            _foldout = new Foldout
            {
                value = false,
                text = "Not initialized",
                style =
                {
                    marginLeft = 10,
                    marginBottom = 4,
                }
            };

            Add(_foldout);
            CreateInitBtn();
            CreateActivateBtn();
            RegisterHover();
        }

        private void CreateInitBtn()
        {
            _initDestroyBtn = CreateControlBtn("Init");
            _initDestroyBtn.clicked += OnInitClicked;
        }

        private void CreateActivateBtn()
        {
            _activateBtn = CreateControlBtn("Activate");
            _activateBtn.clicked += OnActivateClicked;
        }

        private Button CreateControlBtn(string label)
        {
            var btn = new Button
            {
                text = label,
                style =
                {
                    height = 20
                }
            };

            var foldoutText = _foldout.Q<Label>(className: "unity-foldout__text");
            foldoutText.parent.Add(btn);
            foldoutText.style.unityTextAlign = TextAnchor.MiddleLeft;
            foldoutText.style.paddingBottom = 1;
            btn.style.paddingLeft = 6;
            btn.visible = false;
            return btn;
        }

        private void RegisterHover()
        {
            var toggle = _foldout.Q<Toggle>(className: "unity-foldout__toggle");
            toggle.RegisterCallback<MouseOverEvent, Toggle>((_, hoverElement) =>
            {
                hoverElement.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.1f));
            }, toggle);

            toggle.RegisterCallback<MouseOutEvent, Toggle>((_, hoverElement) =>
            {
                hoverElement.style.backgroundColor = new StyleColor(StyleKeyword.None);
            }, toggle);
        }

        public void SetModule(Type moduleType)
        {
            ModuleType = moduleType;
            _foldout.text = moduleType.GetTypeName() + " ";
        }

        public void AddChild(ModulesListElement child)
        {
            _foldout.Add(child);
            _submodules.Add(child);
        }

        public void FinalizeView()
        {
            if (_submodules.Count != 0)
                return;

            var arrow = _foldout.Q<VisualElement>(className: "unity-foldout__checkmark");
            arrow.visible = false;
        }

        public void OnModuleInit()
        {
            _initDestroyBtn.text = "Destroy";
            _activateBtn.visible = true;
            var foldoutText = _foldout.Q<Label>(className: "unity-foldout__text");
            foldoutText.style.unityFontStyleAndWeight = FontStyle.Italic;
        }

        public void OnModuleActivated()
        {
            _activateBtn.text = "Deactivate";
            var foldoutText = _foldout.Q<Label>(className: "unity-foldout__text");
            foldoutText.style.unityFontStyleAndWeight = FontStyle.BoldAndItalic;
        }

        public void OnModuleDeactivated()
        {
            _activateBtn.text = "Activate";
            var foldoutText = _foldout.Q<Label>(className: "unity-foldout__text");
            foldoutText.style.unityFontStyleAndWeight = FontStyle.Italic;
        }

        public void OnModuleDestroyed()
        {
            _initDestroyBtn.text = "Init";
            _activateBtn.visible = false;
            var foldoutText = _foldout.Q<Label>(className: "unity-foldout__text");
            foldoutText.style.unityFontStyleAndWeight = FontStyle.Normal;
        }

        public void SetPlayMode()
        {
            _initDestroyBtn.visible = true;
            _activateBtn.visible = true;
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
    }
}
