using ModulesFramework;
using ModulesFramework.Data;
using ModulesFrameworkUnity.Debug;
using ModulesFrameworkUnity.Debug.UpdateDebugging;
using ModulesFrameworkUnity.Debug.UpdateDebugging.Events;
using UnityEngine.UIElements;

namespace ModulesFrameworkUnity.UpdateDebuggingEditor.View
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
        private Label _currentStepLabel;
        private DataWorld _world;

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

            _currentStepLabel = new Label();
            _currentStepLabel.AddToClassList("mf--pause-widget--run-type-label");
            Add(_currentStepLabel);
            UpdateElementsVisibility(IsPaused());
        }

        public void Show()
        {
            style.display = DisplayStyle.Flex;
            UpdateElementsVisibility(IsPaused());
        }

        public void Hide()
        {
            style.display = DisplayStyle.None;
        }

        public void SetWorld(DataWorld world)
        {
            _world = world;
            Refresh();
        }

        public void Refresh()
        {
            UpdateElementsVisibility(IsPaused());
        }

        private void OnPauseClick()
        {
            _world.RiseEvent(new PauseChangedEvent
            {
                IsPaused = true
            });
        }

        private void OnResumeClick()
        {
            _world.RiseEvent(new PauseChangedEvent
            {
                IsPaused = false
            });
        }

        private void OnStepModuleClick()
        {
            _world.GetModule<UpdateDebuggingModule>().NextModule();
        }

        private void OnStepSystemClick()
        {
            _world.GetModule<UpdateDebuggingModule>().NextSystem();
        }

        private void UpdateElementsVisibility(bool isPaused)
        {
            var pauseDisplay = isPaused ? DisplayStyle.None : DisplayStyle.Flex;
            var resumeDisplay = isPaused ? DisplayStyle.Flex : DisplayStyle.None;
            _pauseBtn.style.display = pauseDisplay;
            _resumeBtn.style.display = resumeDisplay;
            _stepSystemBtn.style.display = resumeDisplay;
            _stepModuleBtn.style.display = resumeDisplay;
            _currentStepLabel.style.display = resumeDisplay;
        }

        private bool IsPaused()
        {
            if (!MF.IsInitialized)
                return false;
            var debugData = _world.OneData<DebugData>();
            return debugData.isPause;
        }

        public void SetPause(bool isPause)
        {
            UpdateElementsVisibility(isPause);
        }

        internal void SetModuleRunType(ModuleRunType moduleRunType)
        {
            _currentStepLabel.text = moduleRunType switch
            {
                ModuleRunType.Run => "Run type: Run",
                ModuleRunType.PostRun => "Run type: Post Run",
                ModuleRunType.FrameEnd => "Run type: Frame End",
                _ => _currentStepLabel.text
            };
        }
    }
}