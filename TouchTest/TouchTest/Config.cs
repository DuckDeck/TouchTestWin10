using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.Graphics.Display;
namespace TouchTest
{
   public class Config
   {

       public static Setting<bool> isNeedShowTrace = new Setting<bool>("isNeedShowTrace", true);
       public static Setting<bool> isReserveTrace = new Setting<bool>("isKeepTrace", false);
       public static Setting<float> TraceThickness = new Setting<float>("TraceThickness", 7);
       public static Setting<int> supportMaxTouchs = new Setting<int>("supportMaxTouchs", 0);
       public static Setting<bool> isShowCoord = new Setting<bool>("isShowCoord", false);
       public static Windows.Foundation.Rect bounds = Window.Current.Bounds;
       public static double dpiRatio = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
       //public static double scaleFactor = bounds.Width * dpiRatio / 480;这样是不对的,直接用 dpiRatio就行了,刚好合适
       public static double scaleFactor = dpiRatio;
       public static double TOUCH_Y_OFFSET = 20 * scaleFactor;
   }
}
