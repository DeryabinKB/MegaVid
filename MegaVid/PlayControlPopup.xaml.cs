using Xamarin.Forms;
using Rg.Plugins.Popup.Pages;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.CommunityToolkit.UI.Views;

namespace MegaVid
{
    public partial class PlayControlPopup : PopupPage
    {
        private MediaElement _mediaElement;

        public PlayControlPopup(MediaElement mediaElement)
        {
            InitializeComponent();
            _mediaElement = mediaElement;
        }

        private void OnPlayClicked(object sender, EventArgs e)
        {
            _mediaElement.Play();
            PopupNavigation.Instance.PopAsync(true);
        }

        private void OnPauseClicked(object sender, EventArgs e)
        {
            _mediaElement.Pause();
            PopupNavigation.Instance.PopAsync(true);
        }

        private void OnStopClicked(object sender, EventArgs e)
        {
            _mediaElement.Stop();
            PopupNavigation.Instance.PopAsync(true);
        }

        private void OnPreviousVideoClicked(object sender, EventArgs e)
        {
            // Реализация для предыдущего видео
            PopupNavigation.Instance.PopAsync(true);
        }

        private void OnNextVideoClicked(object sender, EventArgs e)
        {
            // Реализация для следующего видео
            PopupNavigation.Instance.PopAsync(true);
        }
    }
}