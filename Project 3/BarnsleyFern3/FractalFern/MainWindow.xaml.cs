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
        List<string> computerFacts = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            computerFacts.Add("The first electronic computer ENIAC weighed more than 27 tons and took up 1800 square feet.");
            computerFacts.Add("TYPEWRITER is the longest word that you can write using the letters only on one row of the keyboard of your computer.");
            computerFacts.Add("Doug Engelbart invented the first computer mouse in around 1964 which was made of wood.");
            computerFacts.Add("There are more than 5000 new computer viruses released every month.");
            computerFacts.Add("If there was a computer as powerful as the human brain, it would be able to do 38 thousand trillion operations per second and hold more than 3580 terabytes of memory.");
            computerFacts.Add("The password for the computer controls of nuclear tipped missiles of the U.S was 00000000 for eight years.");
            computerFacts.Add("HP, Microsoft and Apple have one very interesting thing in common – they were all started in a garage.");
            computerFacts.Add("The first 1GB hard disk drive was announced in 1980 which weighed about 550 pounds, and had a price tag of $40, 000.");
            computerFacts.Add("The original name of windows was Interface Manager.");
            computerFacts.Add("he first microprocessor created by Intel was the 4004. It was designed for a calculator, and in that time nobody imagined where it would lead.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Fern f = new Fern(canvas, (int)resolutionSlider.Value, leanSlider.Value * -1, sizeSlider.Value);
        }


        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Fern f = new Fern(canvas, (int)resolutionSlider.Value, leanSlider.Value * -1, sizeSlider.Value);
        }
    }

    /*
     * this class draws a fractal fern when the constructor is called
     */
    class Fern
    {
        public Fern(Canvas canvas, int resolution, double lean, double size)
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
                Dot(canvas, x * size, y * size);
            }
        }

        private void Dot(Canvas canvas, double x, double y)
        {
            int radius = 1;
            Ellipse myEllipse = new Ellipse();
            SolidColorBrush mySolidColorBrush = new SolidColorBrush();
            mySolidColorBrush.Color = Color.FromArgb(255, 0, 255, 0);
            myEllipse.Fill = mySolidColorBrush;
            myEllipse.StrokeThickness = 1;
            myEllipse.Stroke = Brushes.Green;
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

