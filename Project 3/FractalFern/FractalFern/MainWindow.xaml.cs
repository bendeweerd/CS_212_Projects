using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FractalFern
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Fern f = new Fern(5, canvas);
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Fern f = new Fern(5, canvas);
        }
    }

    /*
     * this class draws a fractal fern when the constructor is called
     */
    class Fern
    {
        private static int SEGMENTS = 3;   //number of segments to draw in each branch
        private static double DELTATHETA = -1 * Math.PI / 64;
        private double SEGLENGTH;
        private static int RECURSIONLEVELS = 4;

        public Fern(double segments, Canvas canvas)
        {
            canvas.Children.Clear();
            SEGLENGTH = canvas.Height / SEGMENTS;
            branch((int)(canvas.Width / 2), (int)(canvas.Height * 0.95), Math.PI * 63/64, 0, 0, 0, true, canvas); 
        }

        private void branch(int from_x, int from_y, double theta, int recursion_level, int branch_segment, int parent_branch_segment, bool right, Canvas canvas)
        {
            int to_x = from_x, to_y = from_y;
            double length = (double)(SEGLENGTH / Math.Pow(2, recursion_level));

            if (branch_segment > 0)
            {
                length /= branch_segment;
            }

            if (parent_branch_segment > 0)
            {
                length /= Math.Pow(1.25, parent_branch_segment);
            }

            for (int i = 0; i < SEGMENTS; i++)
            {
                if (right)
                {
                    theta -= DELTATHETA;
                }
                else
                {
                    theta += DELTATHETA;
                }
                from_x = to_x; 
                from_y = to_y;
                to_x = from_x + (int)((length / (Math.Pow(1.25,i))) * Math.Sin(theta));
                to_y = from_y + (int)((length / (Math.Pow(1.25,i))) * Math.Cos(theta));

                byte red = (byte)(100);
                byte green = (byte)(220);
                line(from_x, from_y, to_x, to_y, red, green, 0, 5/Math.Sqrt((recursion_level+1)), canvas);

                if (recursion_level < RECURSIONLEVELS && i < (SEGMENTS - 1))
                {
                    if (i % 2 == 0)
                    {
                        branch(to_x, to_y, theta - (Math.PI / 2.5), recursion_level + 1, i + 1, branch_segment, true, canvas);
                    }
                    else
                    {
                        branch(to_x, to_y, theta + (Math.PI / 2.5), recursion_level + 1, i + 1, branch_segment, false, canvas);
                    }
                }
            }
        }

		/*
         * draw a line segment (x1,y1) to (x2,y2) with given color, thickness on canvas
         */
		private void line(int x1, int y1, int x2, int y2, byte r, byte g, byte b, double thickness, Canvas canvas)
        {
            Line myLine = new Line();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, r, g, b);
            myLine.X1 = x1;
            myLine.Y1 = y1;
            myLine.X2 = x2;
            myLine.Y2 = y2;
            myLine.Stroke = mySolidColorBrush;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.HorizontalAlignment = HorizontalAlignment.Left;
            myLine.StrokeThickness = thickness;
            canvas.Children.Add(myLine);
        }
    }
}

/*
 * this class is needed to enable us to set the center for an ellipse (not built in?!)
 */
public static class EllipseX
{
    public static void SetCenter(this Ellipse ellipse, double X, double Y)
    {
        Canvas.SetTop(ellipse, Y - ellipse.Height / 2);
        Canvas.SetLeft(ellipse, X - ellipse.Width / 2);
    }
}

