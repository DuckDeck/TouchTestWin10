using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace TouchTest
{
   public class Crossline
   {
       private Color lineColor = Colors.White;

       public Color LineColor
       {
           get { return lineColor; }
           set
           {
               lineColor = value;
               horizontalPath.Stroke = new SolidColorBrush(lineColor);
               verticalPath.Stroke = new SolidColorBrush(lineColor);
           }
       }

       public float StrokeThickness
       {
           get { return strokeThickness; }
           set
           {
               strokeThickness = value;
               horizontalPath.StrokeThickness = this.strokeThickness;
               verticalPath.StrokeThickness = this.strokeThickness;
           }
       }

       public PointerPoint Point
       {
           get { return point; }
           set
           {
               point = value;
               ((horizontalPath.Data as LineGeometry).Transform as TranslateTransform).Y = point.Position.Y;
               ((verticalPath.Data as LineGeometry).Transform as TranslateTransform).X = point.Position.X;
           }
       }

       public uint Tag
       {
           get { return tag; }
           set { tag = value; }
       }

       public Path HorizontalPath
       {
           get { return horizontalPath; }
           set{ horizontalPath = value; }
       }

       public Path VerticalPath
       {
           get { return verticalPath; }
           set { verticalPath = value; }
       }

       private float strokeThickness =1 ;

       private PointerPoint point ;

       private uint tag = 0;

       private Path horizontalPath;

       private Path verticalPath;

       public Crossline(Color color, PointerPoint point) 
       {
             this.horizontalPath = new Path();
             this.verticalPath = new Path();
            this.point = point;
            horizontalPath.Stroke = new SolidColorBrush(color);
            verticalPath.Stroke = new SolidColorBrush(color);
            LineGeometry horizontalLg = new LineGeometry();
            horizontalLg.StartPoint = new Point(0, 0);
            horizontalLg.EndPoint = new Point(Window.Current.Bounds.Width, 0);
            TranslateTransform horizontalLineTrans = new TranslateTransform();
            horizontalLineTrans.Y = point.Position.Y;
            horizontalLineTrans.X = 0;
            horizontalLg.Transform = horizontalLineTrans;
            horizontalPath.Data = horizontalLg;
            horizontalPath.StrokeThickness = strokeThickness;
            LineGeometry verticalLg = new LineGeometry();
            verticalLg.StartPoint = new Point(0, 0);
            verticalLg.EndPoint = new Point(0, Window.Current.Bounds.Height);
            TranslateTransform verticalLineTrans = new TranslateTransform();
            verticalLineTrans.X = point.Position.X;
            verticalLineTrans.Y = 0;
            verticalLg.Transform = verticalLineTrans;
            verticalPath.Data = verticalLg;
            verticalPath.StrokeThickness = strokeThickness;


       }
    }
}
