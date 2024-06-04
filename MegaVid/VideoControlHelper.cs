using Xamarin.Forms;
using Xamarin.Essentials;
using System;
using System.Threading.Tasks;
using Xamarin.CommunityToolkit.UI.Views;

namespace MegaVid.Helpers
{
    public class VideoControlHelper
    {
        private readonly MediaElement _mediaElement;
        private readonly Button _playPauseButton;
        private readonly Slider _volumeSlider;
        private readonly Slider _progressSlider;
        private readonly Label _currentTimeLabel;
        private readonly Label _totalTimeLabel;
        private readonly StackLayout _controlPanel;
        private readonly StackLayout _progressPanel;
        private readonly Button _rotateButton;
        private readonly StackLayout _sidePanel;

        private bool _isPlaying;
        private bool _isControlPanelVisible;
        private readonly System.Timers.Timer _hideControlPanelTimer;

        public VideoControlHelper(MediaElement mediaElement, Button playPauseButton, Slider volumeSlider, Slider progressSlider, Label currentTimeLabel, Label totalTimeLabel, StackLayout controlPanel, StackLayout progressPanel, Button rotateButton, StackLayout sidePanel)
        {
            _mediaElement = mediaElement;
            _playPauseButton = playPauseButton;
            _volumeSlider = volumeSlider;
            _progressSlider = progressSlider;
            _currentTimeLabel = currentTimeLabel;
            _totalTimeLabel = totalTimeLabel;
            _controlPanel = controlPanel;
            _progressPanel = progressPanel;
            _rotateButton = rotateButton;
            _sidePanel = sidePanel;
            _mediaElement.Aspect = Aspect.Fill;
            _isControlPanelVisible = true;
            _hideControlPanelTimer = new System.Timers.Timer(5000); // 5 секунд
            _hideControlPanelTimer.Elapsed += (sender, args) => HideControlPanel();
            _hideControlPanelTimer.AutoReset = false;
        }

        public async Task SelectVideo()
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a video",
                FileTypes = FilePickerFileType.Videos
            });

            if (result != null)
            {
                _mediaElement.Source = result.FullPath;
                _mediaElement.Play();
                _isPlaying = true;
                _playPauseButton.Text = "⏸";
                Device.StartTimer(TimeSpan.FromSeconds(1), UpdateProgress);
                _hideControlPanelTimer.Start(); // Запуск таймера при воспроизведении видео
            }
        }

        public void TogglePlayPause()
        {
            if (_isPlaying)
            {
                _mediaElement.Pause();
                _playPauseButton.Text = "▶";
            }
            else
            {
                _mediaElement.Play();
                _playPauseButton.Text = "⏸";
                Device.StartTimer(TimeSpan.FromSeconds(1), UpdateProgress);
                _hideControlPanelTimer.Start(); // Запуск таймера при воспроизведении видео
            }
            _isPlaying = !_isPlaying;
        }

        public void Rewind()
        {
            _mediaElement.Position = _mediaElement.Position - TimeSpan.FromSeconds(10);
        }

        public void FastForward()
        {
            _mediaElement.Position = _mediaElement.Position + TimeSpan.FromSeconds(10);
        }

        public void ChangeVolume(double newValue)
        {
            _mediaElement.Volume = newValue;
        }

        public async Task Rotate()
        {
            var currentOrientation = DeviceDisplay.MainDisplayInfo.Orientation;
            if (currentOrientation == DisplayOrientation.Portrait)
            {
                DependencyService.Get<IOrientationHandler>().SetLandscape();
            }
            else
            {
                DependencyService.Get<IOrientationHandler>().SetPortrait();
            }
        }

        public void PreviousVideo()
        {
            // Реализация для предыдущего видео
        }

        public void NextVideo()
        {
            // Реализация для следующего видео
        }

        public void ToggleControlPanel()
        {
            if (!_isControlPanelVisible)
            {
                ShowControlPanel();
            }
            else
            {
                HideControlPanel();
            }
        }

        public void ShowControlPanel()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _controlPanel.IsVisible = true;
                _progressPanel.IsVisible = true;
                _rotateButton.IsVisible = true;
                _sidePanel.IsVisible = true;
                _isControlPanelVisible = true;
                _hideControlPanelTimer.Start();
            });
        }

        public void HideControlPanel()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                _controlPanel.IsVisible = false;
                _progressPanel.IsVisible = false;
                _rotateButton.IsVisible = false;
                _sidePanel.IsVisible = false;
                _isControlPanelVisible = false;
            });
        }

        public void AdjustControlPanelPosition(DisplayOrientation orientation)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (orientation == DisplayOrientation.Portrait)
                {
                    _controlPanel.Orientation = StackOrientation.Horizontal;
                    _controlPanel.HorizontalOptions = LayoutOptions.Center;
                    _controlPanel.VerticalOptions = LayoutOptions.End;

                    _progressPanel.Orientation = StackOrientation.Horizontal;
                    _progressPanel.HorizontalOptions = LayoutOptions.FillAndExpand;
                    _progressPanel.VerticalOptions = LayoutOptions.End;
                }
                else
                {
                    _controlPanel.Orientation = StackOrientation.Horizontal;
                    _controlPanel.HorizontalOptions = LayoutOptions.Center;
                    _controlPanel.VerticalOptions = LayoutOptions.End;

                    _progressPanel.Orientation = StackOrientation.Horizontal;
                    _progressPanel.HorizontalOptions = LayoutOptions.FillAndExpand;
                    _progressPanel.VerticalOptions = LayoutOptions.End;
                }
                UpdateControlPanelPosition();
                AdjustVideoSize(orientation);
            });
        }

        public void AdjustVideoSize(DisplayOrientation orientation)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
                if (orientation == DisplayOrientation.Portrait)
                {
                    _mediaElement.WidthRequest = mainDisplayInfo.Width;
                    _mediaElement.HeightRequest = mainDisplayInfo.Height * 0.6; // Максимальная высота без растяжения
                }
                else
                {
                    _mediaElement.WidthRequest = mainDisplayInfo.Height;
                    _mediaElement.HeightRequest = mainDisplayInfo.Width * 0.6; // Максимальная высота без растяжения
                }
                _mediaElement.VerticalOptions = LayoutOptions.Center;
                _mediaElement.HorizontalOptions = LayoutOptions.Center;
            });
        }

        private void UpdateControlPanelPosition()
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                var controlPanelPosition = new Thickness(0, 0, 0, _progressPanel.Height + 10); // немного выше слайдера воспроизведения
                _controlPanel.Margin = controlPanelPosition;
            });
        }

        private bool UpdateProgress()
        {
            if (_mediaElement.Source != null && _mediaElement.Duration.HasValue)
            {
                var position = _mediaElement.Position;
                var duration = _mediaElement.Duration.Value;

                _progressSlider.Value = position.TotalSeconds / duration.TotalSeconds;
                _currentTimeLabel.Text = position.ToString(@"mm\:ss");
                _totalTimeLabel.Text = duration.ToString(@"mm\:ss");

                if (!_isPlaying)
                    return false;
            }
            return true;
        }

        public void OnProgressChanged(object sender, ValueChangedEventArgs e)
        {
            if (_mediaElement.Duration.HasValue)
            {
                var duration = _mediaElement.Duration.Value;
                var newPosition = TimeSpan.FromSeconds(duration.TotalSeconds * e.NewValue);
                _mediaElement.Position = newPosition;
            }
        }
    }
}