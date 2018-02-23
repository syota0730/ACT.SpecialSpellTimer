using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using ACT.SpecialSpellTimer.Config.Views;
using ACT.SpecialSpellTimer.Image;
using FFXIV.Framework.Common;
using FFXIV.Framework.Dialog;
using FFXIV.Framework.Extensions;
using Prism.Commands;
using Prism.Mvvm;

namespace ACT.SpecialSpellTimer.RaidTimeline
{
    [XmlType(TypeName = "Style")]
    [Serializable]
    public class TimelineStyle :
        BindableBase
    {
        private string name = string.Empty;

        /// <summary>
        /// スタイルの名前
        /// </summary>
        public string Name
        {
            get => this.name;
            set => this.SetProperty(ref this.name, value);
        }

        private bool isDefault;

        /// <summary>
        /// 規定のスタイル？
        /// </summary>
        public bool IsDefault
        {
            get => this.isDefault;
            set => this.SetProperty(ref this.isDefault, value);
        }

        private FontInfo font = new FontInfo();

        /// <summary>
        /// フォント
        /// </summary>
        public FontInfo Font
        {
            get => this.font;
            set
            {
                if (!Equals(this.font, value))
                {
                    this.font = value;
                    this.RaisePropertyChanged();
                }
            }
        }

        private Color color = Colors.White;

        /// <summary>
        /// カラー
        /// </summary>
        [XmlIgnore]
        public Color Color
        {
            get => this.color;
            set
            {
                if (this.SetProperty(ref this.color, value))
                {
                    this.RaisePropertyChanged(nameof(this.ColorBrush));
                }
            }
        }

        [XmlIgnore]
        public SolidColorBrush ColorBrush => new SolidColorBrush(this.Color);

        /// <summary>
        /// カラー
        /// </summary>
        [XmlElement(ElementName = "Color")]
        public string ColorText
        {
            get => this.Color.ToString();
            set => this.Color = this.Color.FromString(value);
        }

        private Color outlineColor = Colors.Navy;

        /// <summary>
        /// アウトラインのカラー
        /// </summary>
        [XmlIgnore]
        public Color OutlineColor
        {
            get => this.outlineColor;
            set
            {
                if (this.SetProperty(ref this.outlineColor, value))
                {
                    this.RaisePropertyChanged(nameof(this.OutlineColorBrush));
                }
            }
        }

        [XmlIgnore]
        public SolidColorBrush OutlineColorBrush => new SolidColorBrush(this.OutlineColor);

        /// <summary>
        /// アウトラインのカラー
        /// </summary>
        [XmlElement(ElementName = "OutlineColor")]
        public string OutlineColorText
        {
            get => this.OutlineColor.ToString();
            set => this.OutlineColor = this.OutlineColor.FromString(value);
        }

        private Color barColor = Colors.OrangeRed;

        /// <summary>
        /// バーのカラー
        /// </summary>
        [XmlIgnore]
        public Color BarColor
        {
            get => this.barColor;
            set
            {
                if (this.SetProperty(ref this.barColor, value))
                {
                    this.RaisePropertyChanged(nameof(this.BarColorBrush));
                }
            }
        }

        [XmlIgnore]
        public SolidColorBrush BarColorBrush => new SolidColorBrush(this.BarColor);

        /// <summary>
        /// バーのカラー
        /// </summary>
        [XmlElement(ElementName = "BarColor")]
        public string BarColorText
        {
            get => this.BarColor.ToString();
            set => this.BarColor = this.BarColor.FromString(value);
        }

        private double barHeight = 3;

        /// <summary>
        /// バーの高さ
        /// </summary>
        public double BarHeight
        {
            get => this.barHeight;
            set => this.SetProperty(ref this.barHeight, value);
        }

        public bool isCircleStyle = false;

        /// <summary>
        /// プログレスCircleを使用する？
        /// </summary>
        public bool IsCircleStyle
        {
            get => this.isCircleStyle;
            set
            {
                if (this.SetProperty(ref this.isCircleStyle, value))
                {
                    this.RaisePropertyChanged(nameof(this.IsBarStyle));
                }
            }
        }

        [XmlIgnore]
        public bool IsBarStyle => !this.IsCircleStyle;

        private string icon = string.Empty;

        /// <summary>
        /// ICON
        /// </summary>
        public string Icon
        {
            get => this.icon;
            set
            {
                if (this.SetProperty(ref this.icon, value))
                {
                    this.RaisePropertyChanged(nameof(this.IconFile));
                    this.RaisePropertyChanged(nameof(this.IconImage));
                    this.RaisePropertyChanged(nameof(this.ExistsIcon));
                }
            }
        }

        [XmlIgnore]
        public bool ExistsIcon => !string.IsNullOrEmpty(this.Icon);

        [XmlIgnore]
        public string IconFile =>
            string.IsNullOrEmpty(this.Icon) ?
            string.Empty :
            IconController.Instance.GetIconFile(this.Icon)?.FullPath;

        [XmlIgnore]
        public BitmapImage IconImage =>
            string.IsNullOrEmpty(this.Icon) ?
            null :
            IconController.Instance.GetIconFile(this.Icon)?.CreateBitmapImage();

        private double iconSize = 24;

        public double IconSize
        {
            get => this.iconSize;
            set => this.SetProperty(ref this.iconSize, value);
        }

        public TimelineStyle Clone()
        {
            var clone = this.MemberwiseClone() as TimelineStyle;
            clone.Font = this.Font.Clone() as FontInfo;
            return clone;
        }

        #region Change Default

        private ICommand changeDefaultCommand;

        public ICommand ChangeDefaultCommand =>
            this.changeDefaultCommand ?? (this.changeDefaultCommand = new DelegateCommand(() =>
            {
                if (this.IsDefault)
                {
                    foreach (var style in TimelineSettings.Instance.Styles)
                    {
                        if (style != this)
                        {
                            style.IsDefault = false;
                        }
                    }
                }
                else
                {
                    if (!TimelineSettings.Instance.Styles.Any(x => x.IsDefault))
                    {
                        TimelineSettings.Instance.Styles.FirstOrDefault().IsDefault = true;
                    }
                }
            }));

        #endregion Change Default

        #region Change Font

        private ICommand changeFontCommand;

        public ICommand ChangeFontCommand =>
            this.changeFontCommand ?? (this.changeFontCommand = new DelegateCommand<TimelineStyle>((style) =>
            {
                if (style == null)
                {
                    return;
                }

                var result = FontDialogWrapper.ShowDialog(style.Font);
                if (result.Result)
                {
                    style.Font = result.Font;
                }
            }));

        #endregion Change Font

        #region Change Colors

        private ICommand changeFontColorCommand;

        public ICommand ChangeFontColorCommand =>
            this.changeFontColorCommand ?? (this.changeFontColorCommand = new DelegateCommand<TimelineStyle>((style) =>
            {
                if (style != null)
                {
                    var result = ColorDialogWrapper.ShowDialog(style.Color, true);
                    if (result.Result)
                    {
                        style.Color = result.Color;
                    }
                }
            }));

        private ICommand changeFontOutlineColorCommand;

        public ICommand ChangeFontOutlineColorCommand =>
            this.changeFontOutlineColorCommand ?? (this.changeFontOutlineColorCommand = new DelegateCommand<TimelineStyle>((style) =>
            {
                if (style != null)
                {
                    var result = ColorDialogWrapper.ShowDialog(style.OutlineColor, true);
                    if (result.Result)
                    {
                        style.OutlineColor = result.Color;
                    }
                }
            }));

        private ICommand changeBarColorCommand;

        public ICommand ChangeBarColorCommand =>
            this.changeBarColorCommand ?? (this.changeBarColorCommand = new DelegateCommand<TimelineStyle>((style) =>
            {
                if (style != null)
                {
                    var result = ColorDialogWrapper.ShowDialog(style.BarColor, true);
                    if (result.Result)
                    {
                        style.BarColor = result.Color;
                    }
                }
            }));

        #endregion Change Colors

        #region Change Icon

        private ICommand selectIconCommand;

        public ICommand SelectIconCommand =>
            this.selectIconCommand ?? (this.selectIconCommand = new DelegateCommand<TimelineStyle>((style) =>
            {
                if (style == null)
                {
                    return;
                }

                var view = new IconBrowserView();

                view.SelectedIconName = style.Icon;

                if (view.ShowDialog() ?? false)
                {
                    style.Icon = view.SelectedIconName;
                }
            }));

        #endregion Change Icon

        #region Super Default Style

        private static TimelineStyle superDefaultStyle;

        /// <summary>
        /// スーパーDefaultスタイル（デバッグ用またはまったく設定がないときに適用されるスタイル）
        /// </summary>
        public static TimelineStyle SuperDefaultStyle =>
            superDefaultStyle ?? (superDefaultStyle = CreateSuperDefaultStyle());

        private static TimelineStyle CreateSuperDefaultStyle()
        {
            var style = new TimelineStyle()
            {
                Name = "Default",
            };

            if (WPFHelper.IsDesignMode)
            {
                style.Font = new FontInfo("メイリオ", 22, "Normal", "Black", "Normal");
            }
            else
            {
                style.Font = new FontInfo("Arial", 18, "Normal", "Bold", "Normal");
            }

            return style;
        }

        #endregion Super Default Style
    }
}