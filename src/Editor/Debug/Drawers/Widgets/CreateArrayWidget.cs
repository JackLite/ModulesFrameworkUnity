using System;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.Debug.Drawers.Widgets
{
    public class CreateArrayWidget : VisualElement
    {
        private CreateRefWidget _createRefWidget;
        private IntegerField _size;
        private readonly Action<int> _onBtnClick;

        public CreateArrayWidget(string labelText, Action<int> onBtnClick)
        {
            _createRefWidget = new CreateRefWidget(labelText, OnCreateClick);
            _size = new IntegerField();
            _onBtnClick = onBtnClick;
            _createRefWidget.Add(_size);
            _createRefWidget.createBtn.BringToFront();
            Add(_createRefWidget);
        }

        private void CreateSizeField()
        {
            _size = new IntegerField();
        }

        private void OnCreateClick()
        {
            _onBtnClick(_size.value);
        }
    }
}
