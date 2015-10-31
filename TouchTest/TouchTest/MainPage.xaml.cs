using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

//“空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 上有介绍

namespace TouchTest
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {

        protected List<Crossline> points;
        protected List<TextBlock> textBlocks;
        protected List<Color> colors;
        protected Random random;
        public ObservableDict<uint, Polyline> LineDictionary { get; set; }
        public List<Polyline> lines { get; set; }
        //public ObservableDict<uint, Polyline> TempPointerDictionary { get; set; }
        public List<Polyline> tempLines { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            points = new List<Crossline>();
            colors = new List<Color>();
            textBlocks = new List<TextBlock>();
            random = new Random();
            LineDictionary = new ObservableDict<uint, Polyline>();
            lines = new List<Polyline>();
            tempLines = new List<Polyline>();
            //TempPointerDictionary = new ObservableDict<uint, Polyline>();
            var cls = typeof(Colors).GetTypeInfo().DeclaredProperties.ToList();
            foreach (PropertyInfo propertyInfo in cls)
            {
                colors.Add((Color)propertyInfo.GetValue(null));
            }
            this.DataContext = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Window.Current.SizeChanged += Current_SizeChanged;
            ResetMenu();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            Window.Current.SizeChanged -= Current_SizeChanged;
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e)
        {
            Debug.WriteLine(e.Size.Width);
            Canvas.SetLeft(ellipseHand, e.Size.Width / 2 - 100);
            Canvas.SetTop(ellipseHand, e.Size.Height / 2 - 100);
            e.Handled = true;
        }

        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            Debug.WriteLine(e.OriginalSource.GetType().Name);
            Debug.WriteLine("The Poine Id is :" + e.Pointer.PointerId);
            if (e.OriginalSource is Grid)
            {
                Debug.WriteLine((e.OriginalSource as Grid).Name);
            }
            base.OnPointerPressed(e);
            if (this.gridCover.Visibility == Visibility.Visible)
            {
                if (gridSetting.Visibility == Visibility.Visible)
                {
                  
                  //  if (e.OriginalSource is Grid){
                        //  Debug.WriteLine((e.OriginalSource as Grid).Name);
                        if ((e.OriginalSource as FrameworkElement).Name == "layoutRoot")
                        {
                            Config.TraceThickness.Value = (float)this.SliderTraceThickness.Value;
                            Config.isNeedShowTrace.Value = this.SwitchShowTrace.IsOn;
                            Config.isReserveTrace.Value = this.SwitchKeepTrace.IsOn;
                            Config.isShowCoord.Value = this.SwitchShowCoord.IsOn;
                            this.gridSetting.Visibility = Visibility.Collapsed;
                            this.gridCover.Visibility = Visibility.Collapsed;
                        }
                        else
                        {
                            return;
                        }

                           
                  //  }
                   
                }
                else
                {
                    this.gridCover.Visibility = Visibility.Collapsed;
                }
        
            }
            PointerPoint point = e.GetCurrentPoint(this);
            var line = new Crossline(Colors.White, point);
            line.Tag = point.PointerId;
            Color color = colors[random.Next(0, colors.Count - 1)];
            line.LineColor = color;
            CanvasLine.Children.Add(line.HorizontalPath);
            CanvasLine.Children.Add(line.VerticalPath);
            points.Add(line);
            if (points.Count > Config.supportMaxTouchs.Value)
            {
                Config.supportMaxTouchs.Value = points.Count;
            }
            if (Config.isNeedShowTrace.Value)
            {
                //var pointForTrace = new Point(point.Position.X, point.Position.Y - Config.TOUCH_Y_OFFSET);
                var pointForTrace = new Point(point.Position.X, point.Position.Y);
                var polyline = new Polyline();
                polyline.Stroke = new SolidColorBrush(color);
                polyline.StrokeThickness = Config.TraceThickness.Value;
                CanvasLine.Children.Add(polyline);
                LineDictionary.AddValue(point.PointerId, polyline);
            }
            if (Config.isShowCoord.Value)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.TextWrapping = TextWrapping.Wrap;
                textBlock.Width = StackPanelCoords.Width;
                textBlock.Height = 60;
                textBlock.Tag = point.PointerId;
                textBlock.Foreground = new SolidColorBrush(color);
                textBlocks.Add(textBlock);
                textBlock.Text = string.Format("Point{0}:  x:{1:N0} y:{2:N0}", textBlocks.IndexOf(textBlock) + 1, point.Position.X * Config.scaleFactor,
                    point.Position.Y * Config.scaleFactor);
                StackPanelCoords.Children.Add(textBlock);
            }
            ResetMenu();
        }

        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);
            PointerPoint point = e.GetCurrentPoint(this);
            Crossline line = points.Find(s => s.Tag == point.PointerId);
            if (line == null) return;
            line.Point = point;
            if (LineDictionary.ContainsKey(point.PointerId))
            {
                //PointerDictionary[point.PointerId].Points.Add(new Point(point.Position.X,point.Position.Y-Config.TOUCH_Y_OFFSET));
                LineDictionary[point.PointerId].Points.Add(new Point(point.Position.X, point.Position.Y));
            }
            if (Config.isShowCoord.Value)
            {
                TextBlock textBlock = textBlocks.Find(s => Convert.ToUInt16(s.Tag) == point.PointerId);
                if (textBlock != null)
                {
                    textBlock.Text = string.Format("Point{0}:  x:{1:N0} y:{2:N0}", textBlocks.IndexOf(textBlock) + 1, point.Position.X * Config.scaleFactor,
                    point.Position.Y * Config.scaleFactor);
                }
            }
        }


        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);
            PointerPoint point = e.GetCurrentPoint(this);
            Crossline line = points.Find(s => s.Tag == point.PointerId);
            if (line == null) return;
            this.CanvasLine.Children.Remove(line.HorizontalPath);
            this.CanvasLine.Children.Remove(line.VerticalPath);
            points.Remove(line);

            if (!Config.isReserveTrace.Value)
            {
                if (LineDictionary.ContainsKey(point.PointerId))
                {
                    CanvasLine.Children.Remove(LineDictionary[point.PointerId]);
                    LineDictionary.RemoveValue(point.PointerId);
                }
            }
            else
            {
                if (LineDictionary.ContainsKey(point.PointerId))
                {
                    Polyline polyline = LineDictionary[point.PointerId];
                    LineDictionary.Remove(point.PointerId);
                    lines.Add(polyline);
                }
            }
           
            
        
            if (Config.isShowCoord.Value)
            {
                TextBlock textBlock = textBlocks.Find(s => Convert.ToUInt16(s.Tag) == point.PointerId);
                if (textBlock != null)
                {
                    StackPanelCoords.Children.Remove(textBlock);
                    textBlocks.Remove(textBlock);
                }
            }
            ResetMenu();
        }

        protected override void OnPointerExited(PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);
            PointerPoint point = e.GetCurrentPoint(this);
            if (!points.Exists(s => s.Tag == point.PointerId)) return;
            Crossline line = points.Find(s => s.Tag == point.PointerId);
            if (line == null) return;
            this.CanvasLine.Children.Remove(line.HorizontalPath);
            this.CanvasLine.Children.Remove(line.VerticalPath);
            points.Remove(line);
            if (!Config.isReserveTrace.Value)
            {
                if (LineDictionary.ContainsKey(point.PointerId))
                {
                    CanvasLine.Children.Remove(LineDictionary[point.PointerId]);
                    LineDictionary.RemoveValue(point.PointerId);
                }

            }
            else
            {
                if (LineDictionary.ContainsKey(point.PointerId))
                {
                    Polyline polyline = LineDictionary[point.PointerId];
                    LineDictionary.Remove(point.PointerId);
                    lines.Add(polyline);
                }
            }
            if (Config.isShowCoord.Value)
            {
                TextBlock textBlock = textBlocks.Find(s => Convert.ToUInt16(s.Tag) == point.PointerId);
                if (textBlock != null)
                {
                    StackPanelCoords.Children.Remove(textBlock);
                    textBlocks.Remove(textBlock);
                }
            }
            ResetMenu();
        }

        private void gridCover_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (gridSetting.Visibility == Visibility.Visible)
            {
                Config.TraceThickness.Value = (float)this.SliderTraceThickness.Value;
                Config.isNeedShowTrace.Value = this.SwitchShowTrace.IsOn;
                Config.isReserveTrace.Value = this.SwitchKeepTrace.IsOn;
                Config.isShowCoord.Value = this.SwitchShowCoord.IsOn;
            }
            this.gridCover.Visibility = Visibility.Collapsed;
            this.gridSetting.Visibility = Visibility.Collapsed;

        }

        private void btnSetting_Click(object sender, RoutedEventArgs e)
        {
            this.SwitchShowTrace.IsOn = Config.isNeedShowTrace.Value;
            this.SwitchKeepTrace.IsOn = Config.isReserveTrace.Value;
            this.SwitchShowCoord.IsOn = Config.isShowCoord.Value;
            if (!Config.isNeedShowTrace.Value)
                this.SwitchKeepTrace.IsEnabled = false;
            this.SliderTraceThickness.Value = Config.TraceThickness.Value;
            this.RunPoiunt.Text = Config.supportMaxTouchs.Value.ToString();
            gridCover.Visibility = Visibility.Visible;
            ellipseHand.Visibility = Visibility.Collapsed;
            gridSetting.Visibility = Visibility.Visible;
         
        }

        private void SwitchShowTrace_Toggled(object sender, RoutedEventArgs e)
        {
            if (this.SwitchShowTrace.IsOn)
                this.SwitchKeepTrace.IsEnabled = true;
            else
            {
                this.SwitchKeepTrace.IsEnabled = false;
                this.SwitchKeepTrace.IsOn = false;
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            this.CanvasLine.Children.Clear();
            LineDictionary.ClearValue();
            //TempPointerDictionary.ClearValue();
            points.Clear();
            lines.Clear();
            tempLines.Clear();
            if (StackPanelCoords.Children.Count > 0)
            {
                textBlocks.Clear();
                StackPanelCoords.Children.Clear();
            }
            ResetMenu();
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if (tempLines.Count <= 0) return;
            Polyline line = tempLines[tempLines.Count - 1];
            tempLines.RemoveAt(tempLines.Count - 1);
            CanvasLine.Children.Add(line);
            lines.Add(line);
            ResetMenu();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if (lines.Count <= 0) return;
            Polyline line = lines[lines.Count-1];
            lines.RemoveAt(lines.Count - 1);
            CanvasLine.Children.Remove(line);
            tempLines.Add(line);
            ResetMenu();
        }

        private void ResetMenu()
        {
            if(lines.Count > 0)
            {
                btnDelete.IsEnabled = true;
                btnBack.IsEnabled = true;
            }
            else
            {
                btnDelete.IsEnabled = false;
                btnBack.IsEnabled = false;
            }
            if (tempLines.Count > 0)
            {
                btnNext.IsEnabled = true;
            }
            else
            {
                btnNext.IsEnabled = false;
            }
        }

    }
  
}
