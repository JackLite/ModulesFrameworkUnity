using System;
using ModulesFramework.Data;
using ModulesFrameworkUnity.DebugWindowFeature.Common;
using ModulesFrameworkUnity.DebugWindowFeature.Common.Data;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Modules
{
    public class EntityDrawer : FieldDrawer<Entity>
    {
        private readonly Foldout _foldout = new();
        private readonly VisualElement _deadContainer = new();
        private readonly VisualElement _aliveContainer = new();
        private readonly Label _deadLabel = new("Dead entity");
        
        private readonly Label _idLabel = new();
        private readonly Label _customIdLabel = new();
        private readonly Label _generationLabel = new();
        private readonly Button _chooseEntity = new();

        private Entity _entity;
        
        public EntityDrawer()
        {
            _deadContainer.Add(_deadLabel);
            _aliveContainer.Add(_idLabel);
            _aliveContainer.Add(_customIdLabel);
            _aliveContainer.Add(_generationLabel);
            _aliveContainer.Add(_chooseEntity);
            _foldout.Add(_deadContainer);
            _foldout.Add(_aliveContainer);
            
            _chooseEntity.text = "Choose";
            _chooseEntity.AddToClassList("modules--entity-drawer--choose-entity-btn");
            _chooseEntity.clicked += OnChooseEntityClicked;
            
            _foldout.Q<Toggle>().style.fontSize = 14;
            _foldout.Q<Toggle>().style.unityFontStyleAndWeight = FontStyle.Bold;
        }

        private void OnChooseEntityClicked()
        {
            var debugWindow = EditorWindow.focusedWindow as DebugWindow;
            if (debugWindow == null)
            {
                UnityEngine.Debug.LogError("[Modules.Debug] DebugWindow not found");
                return;
            }

            debugWindow.ChooseEntity(_entity);
        }

        protected override void Draw(
            string labelText,
            Entity value,
            VisualElement parent,
            Action<Entity, Entity> _)
        {
            _foldout.text = labelText;
            _entity = value;
            parent.Add(_foldout);
            UpdateLabels(value);
        }

        protected override void Update(Func<Entity> getter)
        {
            _entity = getter();
            UpdateLabels(getter());
        }

        private void UpdateLabels(Entity entity)
        {
            if (!entity.IsAlive())
            {
                _deadContainer.style.display = DisplayStyle.Flex;
                _aliveContainer.style.display = DisplayStyle.None;
            }
            else
            {
                _idLabel.text = $"Id: {entity.Id}";
                _customIdLabel.text = $"CustomId: {entity.GetCustomId()}";
                _generationLabel.text = $"Generation: {entity.generation}";
                _deadContainer.style.display = DisplayStyle.None;
                _aliveContainer.style.display = DisplayStyle.Flex;
            }
        }
    }
}