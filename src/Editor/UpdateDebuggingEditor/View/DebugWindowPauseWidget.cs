using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.UpdateDebugging;
using ModulesFrameworkUnity.DebugWindow.Events;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.DebugWindow
{
    /// <summary>
    ///     Widget to pause/resume MF
    /// </summary>
    public class DebugWindowPauseWidget : VisualElement
    {
        private Button _resumeBtn;
        private Button _pauseBtn;
        private Button _stepSystemBtn;
        private Button _stepModuleBtn;
        private readonly DataWorld _world;

        public DebugWindowPauseWidget(DataWorld world)
        {
            _world = world;
        }

        public void Draw()
        {
            _resumeBtn = new Button
            {
                text = "Resume"
            };
            Add(_resumeBtn);
            _resumeBtn.clicked += OnResumeClick;

            _pauseBtn = new Button
            {
                text = "Pause"
            };
            Add(_pauseBtn);
            _pauseBtn.clicked += OnPauseClick;

            _stepSystemBtn = new Button
            {
                text = "Next System"
            };
            Add(_stepSystemBtn);
            _stepSystemBtn.clicked += OnStepSystemClick;

            _stepModuleBtn = new Button
            {
                text = "Next Module"
            };
            Add(_stepModuleBtn);
            _stepModuleBtn.clicked += OnStepModuleClick;
            UpdateButtonsVisibility();
        }

        public void Show()
        {
            style.display = DisplayStyle.Flex;
            UpdateButtonsVisibility();
        }

        public void Hide()
        {
            style.display = DisplayStyle.None;
            UpdateButtonsVisibility();
        }

        public void Refresh()
        {
            UpdateButtonsVisibility();
        }

        private void OnPauseClick()
        {
            _world.RiseEvent(new PauseChangedEvent
            {
                IsPaused = true
            });
            UpdateButtonsVisibility();
        }

        private void OnResumeClick()
        {
            _world.RiseEvent(new PauseChangedEvent
            {
                IsPaused = false
            });
            UpdateButtonsVisibility();
        }

        private void OnStepModuleClick()
        {
            _world.GetModule<UpdateDebuggingModule>().NextModule();
        }

        private void OnStepSystemClick()
        {
            _world.GetModule<UpdateDebuggingModule>().NextSystem();
        }

        private void UpdateButtonsVisibility()
        {
            var isPaused = IsPaused();
            var pauseDisplay = isPaused ? DisplayStyle.None : DisplayStyle.Flex;
            var resumeDisplay = isPaused ? DisplayStyle.Flex : DisplayStyle.None;
            _pauseBtn.style.display = pauseDisplay;
            _resumeBtn.style.display = resumeDisplay;
            _stepSystemBtn.style.display = resumeDisplay;
            _stepModuleBtn.style.display = resumeDisplay;
        }

        private bool IsPaused()
        {
            if (!MF.IsInitialized)
                return false;
            var debugData = _world.OneData<DebugData>();
            return debugData.isPause;
        }
    }
}