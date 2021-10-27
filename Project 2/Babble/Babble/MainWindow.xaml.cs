using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Collections;

namespace BabbleSample
{
    public partial class MainWindow : Window
    {
        private string input;                                           // input file
        private string[] words;                                         // input file broken into array of words
        private int wordCount = 200;                                    // number of words to babble
        private Dictionary<string, ArrayList> hashTable = new Dictionary<string, ArrayList>();
        private List<string> keyStrings = new List<string>(new string[5]);

        public MainWindow()
        {
            InitializeComponent();
        }

        /* Handle 'Load' button click by loading a file
         *  and invoking the analyzeInput() method
         * 
         * Input conditions:
         *  - Text file to analyze exists on filesystem
         * Output conditions:
         *  - Text file has been retrieved from the filesystem
         *  - File has been analyzed at the selected order and entered into a dictionary
         */
        private void loadButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.FileName = "Sample";                                    // Default file name
            ofd.DefaultExt = ".txt";                                    // Default file extension
            ofd.Filter = "Text documents (.txt)|*.txt";                 // Filter files by extension

            if ((bool)ofd.ShowDialog())                                 // Show dialog box
            {
                textBlock1.Text = "Loading file " + ofd.FileName + "\n";
                input = System.IO.File.ReadAllText(ofd.FileName);       // Read file
                words = Regex.Split(input, @"\s+");                     // Split into array of words
            }

            analyzeInput(orderComboBox.SelectedIndex);
        }

        /* Analyze currently loaded text file at selected order. Called when
         *  a file is first loaded and whenever the selected order changes
         * 
         * Input conditions:
         *  - Text file has been loaded
         * Output conditions:
         *  - Hash table has been formed from text file contents and selected order
         *  - Text statistics (total words and unique dictionary keys) are displayed
         */
        private void analyzeInput(int order)
        {
            if (words == null) return;                                  // Prevent analysis when no file has been selected
            
            // Analyze text based on the selected order
            switch (order)                                              
            {
                case 1:
                    for (int i = 0; i < words.Length - 1; i++)
                    {
                        if (!hashTable.ContainsKey(words[i]))
                        {
                            hashTable.Add(words[i], new ArrayList());
                        }
                        hashTable[words[i]].Add(words[i+1]);
                    }
                    break;

				case 2:
					for (int i = 1; i < words.Length - 1; i++)
					{
                        string key = words[i - 1]+ " " + words[i];
                        if (!hashTable.ContainsKey(key))
                        {
                            hashTable.Add(key, new ArrayList());
                        }
                        hashTable[key].Add(words[i + 1]);
					}
					break;

                case 3:
                    for (int i = 2; i < words.Length - 1; i++)
                    {
                        string key = words[i - 2]
                            + " " + words[i - 1]
                            + " " + words[i];
                        if (!hashTable.ContainsKey(key))
                        {
                            hashTable.Add(key, new ArrayList());
                        }
                        hashTable[key].Add(words[i + 1]);
                    }
                    break;

                case 4:
                    for (int i = 3; i < words.Length - 1; i++)
                    {
                        string key = words[i - 3]
                            + " " + words[i - 2]
                            + " " + words[i - 1]
                            + " " + words[i];
                        if (!hashTable.ContainsKey(key))
                        {
                            hashTable.Add(key, new ArrayList());
                        }
                        hashTable[key].Add(words[i + 1]);
                    }
                    break;

                case 5:
                    for (int i = 4; i < words.Length - 1; i++)
                    {
                        string key = words[i - 4]
                            + " " + words[i - 3]
                            + " " + words[i - 2]
                            + " " + words[i - 1]
                            + " " + words[i];
                        if (!hashTable.ContainsKey(key))
                        {
                            hashTable.Add(key, new ArrayList());
                        }
                        hashTable[key].Add(words[i + 1]);
                    }
                    break;
            }

            // Display text statistics
            if (textBlock1 != null)
            {
                textBlock1.Text += $"\nAnalyzed at Order {orderComboBox.SelectedIndex}, ready to babble";
                keyCountLabel.Content = $"{hashTable.Count()}";
                wordCountLabel.Content = $"{words.Length}";
            }
        }

        /* Handle 'Babble' button click.  This function uses the hash table
         *  to create a nonsensical output based on the frequency of word groups.
         *  The more frequently a word occurs after another or another group
         *  (depending on order), the more likely it will be added to the output.
         * 
         * Input conditions:
         *  - A hash table has been created for the text file at the selected order
         * Output conditions:
         *  - Babbled text is displayed
         */
        private void babbleButton_Click(object sender, RoutedEventArgs e)
        {
            textBlock1.Text = "";
            Random rnd = new Random();

            // Babble text, using selected index
            switch (orderComboBox.SelectedIndex)
            {
                // Order 0 - display unmodified text file
                case 0:
                    for (int i = 0; i < Math.Min(wordCount, words.Length); i++)
                    {
                        textBlock1.Text += words[i] + " ";
                    }
                    break;

                // Order 1
                case 1:             
                    string currentString;
                    currentString = words[0];
                    textBlock1.Text += currentString + " ";
                    for (int i = 0; i < wordCount; i++)
                    {
                        if (hashTable.ContainsKey(currentString))
                        {
                            int numValues = hashTable[currentString].Count;
                            int selectedIndex = rnd.Next(0, numValues - 1);
                            textBlock1.Text += hashTable[currentString][selectedIndex] + " ";
                            currentString = (string)hashTable[currentString][selectedIndex];
                        }
                        // if the end has been reached, start over at the beginning
                        else
                        {
                            currentString = words[0];
                        }
                        
                    }
                    break;

                // Order 2
                case 2:
                    // Populate keyStrings array
                    keyStrings[0] = words[1];

                    currentString = words[0] + " " + words[1];
                    textBlock1.Text += currentString + " ";

                    // Generate output by searching hash table for the last 2 words
                    for (int i = 1; i < wordCount; i++)
                    {
                        if (hashTable.ContainsKey(currentString))
                        {
                            int numValues = hashTable[currentString].Count;
                            int selectedIndex = rnd.Next(0, numValues - 1);
                            textBlock1.Text += hashTable[currentString][selectedIndex] + " ";
                            keyStrings[1] = (string)hashTable[currentString][selectedIndex];
                            currentString = keyStrings[0] + " " + hashTable[currentString][selectedIndex];
                            keyStrings[0] = keyStrings[1];
                            keyStrings[1] = null;
                        }
                        // if the end has been reached, start over at the beginning
                        else
                        {
                            keyStrings[0] = words[1];
                            currentString = words[0] + " " + words[1];
                        }
                        
                    }
                    break;

                // Order 3
				case 3:
                    // Populate keyStrings array
                    keyStrings[0] = words[1];
                    keyStrings[1] = words[2];

					currentString = words[0]
                        + " " + words[1]
                        + " " + words[2];
					textBlock1.Text += currentString + " ";

                    // Generate output by searching hash table for the last 3 words
                    for (int i = 1; i < wordCount; i++)
					{
                        if (hashTable.ContainsKey(currentString))
                        {
                            int numValues = hashTable[currentString].Count;
                            int selectedIndex = rnd.Next(0, numValues - 1);
                            textBlock1.Text += hashTable[currentString][selectedIndex] + " ";
                            keyStrings[2] = (string)hashTable[currentString][selectedIndex];
                            currentString = keyStrings[0]
                                + " " + keyStrings[1]
                                + " " + hashTable[currentString][selectedIndex];
                            keyStrings[0] = keyStrings[1];
                            keyStrings[1] = keyStrings[2];
                            keyStrings[2] = null;
                        }
                        // if the end has been reached, start over at the beginning
                        else
                        {
                            keyStrings[0] = words[1];
                            keyStrings[1] = words[2];
                            currentString = words[0]
                                + " " + words[1]
                                + " " + words[2];
                        }
					}
					break;

                // Order 4
				case 4:
                    // Populate keyStrings array
                    keyStrings[0] = words[1];
                    keyStrings[1] = words[2];
                    keyStrings[2] = words[3];

                    currentString = words[0]
                        + " " + words[1]
                        + " " + words[2]
                        + " " + words[3];
                    textBlock1.Text += currentString + " ";

                    // Generate output by searching hash table for the last 4 words
                    for (int i = 1; i < wordCount; i++)
                    {
                        if (hashTable.ContainsKey(currentString))
                        {
                            int numValues = hashTable[currentString].Count;
                            int selectedIndex = rnd.Next(0, numValues - 1);
                            textBlock1.Text += hashTable[currentString][selectedIndex] + " ";
                            keyStrings[3] = (string)hashTable[currentString][selectedIndex];
                            currentString = keyStrings[0]
                                + " " + keyStrings[1]
                                + " " + keyStrings[2]
                                + " " + hashTable[currentString][selectedIndex];
                            keyStrings[0] = keyStrings[1];
                            keyStrings[1] = keyStrings[2];
                            keyStrings[2] = keyStrings[3];
                            keyStrings[3] = null;
                        }
                        // if the end has been reached, start over at the beginning
                        else 
                        {
                            keyStrings[0] = words[1];
                            keyStrings[1] = words[2];
                            keyStrings[2] = words[3];
                            currentString = words[0]
                                + " " + words[1]
                                + " " + words[2]
                                + " " + words[3];
                        }
                    }
                    break;

                // Order 5
                case 5:
                    // Populate keyStrings array
                    keyStrings[0] = words[1];
                    keyStrings[1] = words[2];
                    keyStrings[2] = words[3];
                    keyStrings[3] = words[4];

                    currentString = words[0]
                        + " " + words[1]
                        + " " + words[2]
                        + " " + words[3]
                        + " " + words[4];
                    textBlock1.Text += currentString + " ";

                    // Generate output by searching hash table for the last 5 words
                    for (int i = 1; i < wordCount; i++)
                    {
                        if (hashTable.ContainsKey(currentString))
                        {
                            int numValues = hashTable[currentString].Count;
                            int selectedIndex = rnd.Next(0, numValues - 1);
                            textBlock1.Text += hashTable[currentString][selectedIndex] + " ";
                            keyStrings[4] = (string)hashTable[currentString][selectedIndex];
                            currentString = keyStrings[0]
                                + " " + keyStrings[1]
                                + " " + keyStrings[2]
                                + " " + keyStrings[3]
                                + " " + hashTable[currentString][selectedIndex];
                            keyStrings[0] = keyStrings[1];
                            keyStrings[1] = keyStrings[2];
                            keyStrings[2] = keyStrings[3];
                            keyStrings[3] = keyStrings[4];
                            keyStrings[4] = null;
                        }
                        // if the end has been reached, start over at the beginning
                        else
                        {
                            keyStrings[0] = words[1];
                            keyStrings[1] = words[2];
                            keyStrings[2] = words[3];
                            keyStrings[3] = words[4];
                            currentString = words[0]
                                + " " + words[1]
                                + " " + words[2]
                                + " " + words[3]
                                + " " + words[4];
                        }
                    }
                    break;
            }
        }
        private void orderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
			hashTable.Clear();
			analyzeInput(orderComboBox.SelectedIndex);
		}
    }
}
