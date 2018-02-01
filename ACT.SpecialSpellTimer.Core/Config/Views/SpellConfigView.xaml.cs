using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ACT.SpecialSpellTimer.Config.ViewModels;
using ACT.SpecialSpellTimer.resources;
using FFXIV.Framework.Globalization;

namespace ACT.SpecialSpellTimer.Config.Views
{
    /// <summary>
    /// SpellConfigView.xaml の相互作用ロジック
    /// </summary>
    public partial class SpellConfigView : UserControl, ILocalizable
    {
        public SpellConfigView()
        {
            this.InitializeComponent();
            this.SetLocale(Settings.Default.UILocale);
        }

        public SpellConfigViewModel ViewModel => this.DataContext as SpellConfigViewModel;

        public void SetLocale(Locales locale) => this.ReloadLocaleDictionary(locale);

        private void TextBoxSelect(
            object sender,
            MouseButtonEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                if (!textBox.IsKeyboardFocusWithin)
                {
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private void TextBoxOnGotFocus(
            object sender,
            RoutedEventArgs e)
        {
            (sender as TextBox)?.SelectAll();
        }

        private void TabControl_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            this.ViewModel.IsActiveVisualTab = this.VisualTab.IsSelected;
        }
    }
}