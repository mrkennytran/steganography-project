using System.Windows;
using Microsoft.Win32;
using System.Windows.Media;
using System;


namespace Secret_Decoder {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        //Global variables       
        private string filePath = "";
        private PortablePixelMap ppmImg;

        public MainWindow() {
            InitializeComponent();
        }//end event 

        #region MENU EVENTS 
        private void openTab_Click(object sender, RoutedEventArgs e) {
            //create instance of openfile dialog class
            OpenFileDialog imgSelection = new OpenFileDialog();

            //Filter for desired file types 
            imgSelection.Filter = "Portable PixelMap image (*ppm)|*ppm";

            //Displays dialog box and checks if actual is opened 
            bool isSelected = imgSelection.ShowDialog() == true;

            //Keep record of file path for later use 
            filePath = imgSelection.FileName;

            //Valid input: if user selected a file then open the image and send to imagebox
            if (isSelected) { //Read PPM files
                //Declare object
                PortablePixelMap ppm = new PortablePixelMap(filePath);

                //Display ppm thru xaml 
                imgDisplay.Source = ppm.MakeBitmap();

                //Reference thru other methods 
                ppmImg = ppm;

            }//end if
        }//end event 

        private void exitTab_Click(object sender, RoutedEventArgs e) {
            Close();
        }//end event
        #endregion

        private void decodeBtn_Click(object sender, RoutedEventArgs e) {
            string message = string.Empty;
            int counter = 0;
            int maxCharCount = ppmImg.Width * 2;
            string letter = string.Empty;
            string stop = string.Empty;

            //Iterate thru each pixel and decrypt the hidden message
            for (int y = 0; y <= ppmImg.Height - 1; y++) {
                for (int x = 0; x <= ppmImg.Width - 1; x++) {
                    Color pixelClr = ppmImg.GetPixelColor(x,y);

                    //(y == (int)(ppmImg.Height - 1) / 2)
                    if (x > 0 && y < ppmImg.Height - 1) { //Location of messagez
                        if (counter < maxCharCount) { //Stopping point
                            //Decode letter from pixel
                            letter = Decode(pixelClr);

                            //Add character to the message 
                            message += letter;

                            //Limit output of additional text on wpf textbox
                            counter++;
                        }//end if 
                    }//end if 
                }//end for 
            }//end for 

            //Trim message and make it readable
            string cleanMsg = CleanMsg(message);

            //Display message into wpf 
            secretMsgBox.Text = cleanMsg.Trim();
        }//end event 

        private string Decode(Color pixel) {
            char[] decodeBin = new char [8];

            //Convert RGB components to binary format
            string R = BinaryConversion(pixel.R);
            string G = BinaryConversion(pixel.G);
            string B = BinaryConversion(pixel.B);

            //Retrieve binary values and saving to string array 
            decodeBin[0] = R[6];
            decodeBin[1] = R[7];
            decodeBin[2] = G[5];
            decodeBin[3] = G[6];
            decodeBin[4] = G[7];
            decodeBin[5] = B[5];
            decodeBin[6] = B[6];
            decodeBin[7] = B[7];

            //Convert char array to string
            string binary = new string(decodeBin);

            //Convert binary value to 
            int decVal = Convert.ToInt32(binary, 2);

            //Convert int value to string
            string hiddenLetter = Convert.ToChar(decVal).ToString();

            return hiddenLetter;
        }//end method 

        private string BinaryConversion(int input) {
            string binary = Convert.ToString(input, 2);

            //Reformat strings to have 8 digit placements
            binary = binary.PadLeft(8, '0');

            return binary;
        }//end method 

        private string CleanMsg(string message) {
            string cleanMsg = string.Empty;

            foreach (char index in message) {
                bool isUppercase = index >= 65 && index <= 90;
                bool isLowercase = index >= 97 && index <= 122;
                bool isNumber = index >= 48 && index <= 57;
                bool isSpace = index == 32;
                bool magicKey = index == 61;

                if (isUppercase || isLowercase || isNumber || isSpace) {
                    cleanMsg += index;
                } else if (magicKey) { //Stopping point 
                    //Reverse backwards message 
                    cleanMsg = Reverse(cleanMsg);
                    return cleanMsg;
                } else {
                    cleanMsg += " ";
                }//end else 
            }//end foreach

            return cleanMsg;
        }//end method 

        public static string Reverse(string message) {
            //Used to reverse message to better hide message
            char[] charArray = message.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }//end method 
    }//end class 
}//end namespace
