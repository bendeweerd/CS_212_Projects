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
     * this class draws a fractal fern when the constructor is called
     */
    class Fern
    {
        public Fern(Canvas canvas, int resolution, double lean, double size, double fallPercentage)
        {
            double x = 0;
            double y = 0;
            var rand = new Random();
            canvas.Children.Clear();

            for (int i = 0; i < resolution; i++)
            {
                x /= lean;
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
        }

        private void Dot(Canvas canvas, double x, double y, double fallPercentage)
        {
            var rand = new Random();
            int radius = 2;
            Ellipse myEllipse = new Ellipse();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();

            // Randomly distribute orange (fall) dots based on chosen percentage
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
    }
}

/*
 * this class is needed to enable us to set the center for an ellipse (not built in?!)
 */
public static class EllipseX
{
    public static void SetCenter(this Ellipse ellipse, double X, double Y)
    {
        Canvas.SetTop(ellipse, Y);
        Canvas.SetLeft(ellipse, X);
    }
}

