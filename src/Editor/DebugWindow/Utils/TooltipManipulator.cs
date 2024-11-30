using UnityEngine;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow.Utils
{
    public class TooltipManipulator : Manipulator
    {
        private VisualElement element;

        protected override void RegisterCallbacksOnTarget()
        {
            target.RegisterCallback<MouseEnterEvent>(MouseIn);
            target.RegisterCallback<MouseOutEvent>(MouseOut);
        }

        protected override void UnregisterCallbacksFromTarget()
        {
            target.UnregisterCallback<MouseEnterEvent>(MouseIn);
            target.UnregisterCallback<MouseOutEvent>(MouseOut);
        }

        private void MouseIn(MouseEnterEvent e)
        {
            if (element == null)
            {
                element = new VisualElement();
                element.style.backgroundColor = Color.blue;
                element.style.position = Position.Absolute;
                element.style.left = target.worldBound.center.x;
                element.style.top = target.worldBound.yMin;
                var label = new Label(target.tooltip);
                label.style.color = Color.white;

                element.Add(label);
                
                var root = target.panel.visualTree;
                root.Add(element);

            }
            element.style.visibility = Visibility.Visible;
            element.BringToFront();
        }

        private void MouseOut(MouseOutEvent e)
        {
            element.style.visibility = Visibility.Hidden;
        }
    }
}