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
            Fern f = new Fern(canvas, (int)resolutionSlider.Value, leanSlider.Value * -1, sizeSlider.Value, fallSlider.Value);
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Fern f = new Fern(canvas, (int)resolutionSlider.Value, leanSlider.Value * -1, sizeSlider.Value, fallSlider.Value);
        }
    }

    /*
     * Draw a Barnsley fern, with a brown base and a sun in the upper right-hand corner.
     */
    class Fern
    {
        public Fern(Canvas canvas, int resolution, double lean, double size, double fallPercentage)
        {
            double x = 0;
            double y = 0;
            var rand = new Random();
            canvas.Children.Clear();

            //Randomly choose background color
            byte red = (byte)Math.Floor(rand.NextDouble() * 255);
            byte green = (byte)Math.Floor(rand.NextDouble() * 255);
            byte blue = (byte)Math.Floor(rand.NextDouble() * 255);
            canvas.Background = new SolidColorBrush(Color.FromArgb(50, red, green, blue));

            //Draw sun body
            Ellipse sun = new Ellipse();
            sun.Stroke = new SolidColorBrush(Colors.Yellow);
            sun.Fill = new SolidColorBrush(Colors.Yellow);
            sun.Width = 200;
            sun.Height = 200;
            sun.VerticalAlignment = VerticalAlignment.Center;
            sun.HorizontalAlignment = HorizontalAlignment.Center;
            sun.SetCenter(540, -100);
            canvas.Children.Add(sun);

            //Draw sun rays
            line(500, 40, 400, 60, 255, 255, 0, 10, canvas);
            line(600, 140, 580, 240, 255, 255, 0, 10, canvas);
            line(534, 106, 468, 172, 255, 255, 0, 10, canvas);

            //Draw fern as a series of dots, resolution is determined by a slider.
            for (int i = 0; i < resolution; i++)
            {
                x /= lean;
                //Randomly choose which part of the fern to create a dot for (Barnsley Fern)
                double r = rand.NextDouble();
                if (r < 0.01)
                {
                    x = (0 * x) + (0 * y);
                    y = (0 * x) + (0.16 * y);
                }
                else if (r < 0.86)
                {
                    x = (0.85 * x) + (0.04 * y);
                    y = (-0.04 * x) + (0.85 * y) + 1.6;
                }
                else if (r < 0.93)
                {
                    x = (0.20 * x) - (0.26 * y);
                    y = (0.23 * x) + (0.22 * y) + 1.6;
                }
                else
                {
                    x = (-0.15 * x) + (0.28 * y);
                    y = (0.26 * x) + (0.24 * y) + 0.44;
                }
                x *= lean;
                Dot(canvas, x * size, y * size, fallPercentage);
            }

            //Add a brown rectangle for the base
            Rectangle rect = new Rectangle();
            rect.Stroke = new SolidColorBrush(Colors.Brown);
            rect.Fill = new SolidColorBrush(Colors.Brown);
            rect.Width = 560;
            rect.Height = 20;
            Canvas.SetLeft(rect, 40);
            Canvas.SetTop(rect, 460);
            canvas.Children.Add(rect);
        }

        /*
         * Helper function to create a dot, or part of the fern, at the specified location.
         */
        private void Dot(Canvas canvas, double x, double y, double fallPercentage)
        {
            var rand = new Random();
            int radius = 2;
            Ellipse myEllipse = new Ellipse();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            //Randomly distribute orange (fall) dots based on chosen percentage
            if (rand.NextDouble() < fallPercentage)
            {
                mySolidColorBrush.Color = Color.FromArgb(255, 255, 81, 0);
                myEllipse.Stroke = Brushes.Firebrick;
            }
            else
            {
                mySolidColorBrush.Color = Color.FromArgb(255, 0, 255, 0);
                myEllipse.Stroke = Brushes.Green;
            }

            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 1;
            myEllipse.HorizontalAlignment = HorizontalAlignment.Center;
            myEllipse.VerticalAlignment = VerticalAlignment.Center;
            myEllipse.Width = 2 * radius;
            myEllipse.Height = 2 * radius;

            myEllipse.SetCenter(320 - (x * 75), 480 - (y * 48));
            canvas.Children.Add(myEllipse);
        }

        /*
         * Helper function to draw a line at specified coordinates with
         * specified characteristics.
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
 * This class is needed to enable us to set the center for an ellipse (not built in?!)
 */
public static class EllipseX
{
    public static void SetCenter(this Ellipse ellipse, double X, double Y)
    {
        Canvas.SetTop(ellipse, Y);
        Canvas.SetLeft(ellipse, X);
    }
}

