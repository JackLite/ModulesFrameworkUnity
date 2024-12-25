using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Widgets
{
    /// <summary>
    ///     Widget to create new reference objects
    ///     For Arrays using special widget
    /// </summary>
    public class CreateRefWidget : VisualElement
    {
        public Label label;
        public Button createBtn;

        public CreateRefWidget(string labelText, Action onBtnClick)
        {
            CreateLabel(labelText);
            CreateButton(onBtnClick);
        }

        private void CreateLabel(string labelText)
        {
            label = new Label(labelText);
            Add(label);
        }

        private void CreateButton(Action onBtnClick)
        {
            createBtn = new Button
            {
                text = "Create"
            };
            createBtn.clicked += onBtnClick;
            Add(createBtn);
        }
    }
}
