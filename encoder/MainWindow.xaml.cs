using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Microsoft.Win32;
using System;


namespace SecretEncryption {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        //Global variables       
        string filePath = "";
        PortablePixelMap ppmImg;

        public MainWindow() {
            InitializeComponent();
        }//end event 

        #region MENU BAR 
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

                //Save portable pixel map image to global variable
                ppmImg = ppm;

            }//end if
        }//end event 

        private void exitTab_Click(object sender, RoutedEventArgs e) {
            Close();
        }//end event 

        private void aboutTab_Click(object sender, RoutedEventArgs e) {
            //Declare Process object - enables start or stop task processing 
            Process webDoc = new Process();

            //Specifies os shell should be used to start process
            webDoc.StartInfo.UseShellExecute = true;

            //Reference file path 
            webDoc.StartInfo.FileName = "https://www.comptia.org/blog/what-is-steganography#:~:text=Steganography%20is%20the%20practice%20of,to%20friends%20using%20invisible%20ink.";

            //Run task 
            webDoc.Start();
        }//end event 
        #endregion


        #region PROGRAM EVENTS
        private void hideBtn_Click(object sender, RoutedEventArgs e) {
            //(y == (int)(ppmImg.Height - 1) / 2)
            if(secretMsgBox.Text == string.Empty || imgDisplay.Source == null) { // Valid input: empty text box or empty image
                MessageBox.Show("Upload a ppm image and enter a message.");
            } else {
                if (secretMsgBox.Text.Length > ppmImg.Height - 2 || secretMsgBox.Text.Length > ppmImg.Width - 2) { //Valid input: Message too large for image 
                    MessageBox.Show("Message is too long to encrypt in image.");
                } else {
                    int counter = 0; //count for pixel data 

                    //Setting up stopping point for decoding
                    string reverseMsg = Reverse(secretMsgBox.Text);
                    reverseMsg += "=";

                    //Split string to char array 
                    char[] msgArray = reverseMsg.ToCharArray();

                    for (int y = 0; y <= ppmImg.Height - 1; y++) {
                        for (int x = 0; x <= ppmImg.Width - 1; x++) {
                            //Get current pixel color 
                            Color pixelClr = ppmImg.GetPixelColor(x, y);

                            if (x > 0 && y < ppmImg.Height - 1) {//Paramter to where pixels can be set 
                                if (counter <= msgArray.Length - 1) {//Set stopping point for character encryption 
                                    //Char index to char 
                                    char character = msgArray[counter];

                                    //Get dec char value from user input 
                                    int decVal = character - 0;

                                    //Encrypt current char into pixel 
                                    pixelClr = Encode(decVal, pixelClr);

                                    //Set encrypted pixel into ppm
                                    ppmImg.SetPixel(x, y, pixelClr.R, pixelClr.G, pixelClr.B);

                                    //Keep track of index of secret message 
                                    counter++;
                                }//end if 
                            }//end if 

                        }//end for 
                    }//end for

                    //Save encrypted image
                    SaveEncode();
                }//end else
            } //end else if  

            //Remove stop trigger from display 
            string finalMsg = secretMsgBox.Text.Substring(0, secretMsgBox.Text.Length);
            secretMsgBox.Text = finalMsg;
        }//end event  

        private void secretMsgBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { //DISPLAYS TEXT COUNT 
            if (secretMsgBox.Text.Length <= 250) {
                string userInput = secretMsgBox.Text;

                //Get user input count 
                int charCount = userInput.Length;

                //Display count
                textCount.Text = charCount.ToString();
            }//end if
        }//end event
        #endregion

        #region METHODS
        private Color Encode(int decVal, Color pixelClr) { //ENCRYPT CURRENT CHARACTER INTO PIXEL 
            //Convert current char and RGBs' int values to binary
            string current = BinaryConversion(decVal);
            string R = BinaryConversion(pixelClr.R);
            string G = BinaryConversion(pixelClr.G);
            string B = BinaryConversion(pixelClr.B);

            //Split string to char[]
            char[] currentCharArray = current.ToCharArray();
            char[] redArray = R.ToCharArray();
            char[] greenArray = G.ToCharArray();
            char[] blueArray = B.ToCharArray();

            //Plug current char binary into 8 color components
            redArray[6] = currentCharArray[0];
            redArray[7] = currentCharArray[1];
            greenArray[5] = currentCharArray[2];
            greenArray[6] = currentCharArray[3];
            greenArray[7] = currentCharArray[4];
            blueArray[5] = currentCharArray[5];
            blueArray[6] = currentCharArray[6];
            blueArray[7] = currentCharArray[7];

            //Convert char arrays back to string
            R = new string(redArray);
            G = new string(greenArray);
            B = new string(blueArray);

            //Convert string to int
            int red = Convert.ToInt32(R, 2);
            int green = Convert.ToInt32(G, 2);
            int blue = Convert.ToInt32(B, 2);

            //Convert binary to int to byte 
            pixelClr.R = Convert.ToByte(red);
            pixelClr.G = Convert.ToByte(green);
            pixelClr.B = Convert.ToByte(blue);

            return pixelClr;
        }//end method 

        private void SaveEncode() { //SAVE NEW ENCRYPTED IMAGE 
            //Initiates save event if filepath isn't empty
            if (filePath != String.Empty) {
                //Create instance 
                SaveFileDialog saveFile = new SaveFileDialog();

                //Set default file extension 
                saveFile.FileName = "ppm";
                saveFile.DefaultExt = "ppm";

                //Filters for desired file type 
                saveFile.Filter =
                "Portable PixelMap P3 image (*.ppm)|*.ppm" +
                "|Portable PixelMap P6 image (*.ppm)|*.ppm";

                //Validates if filepath is established
                if (saveFile.ShowDialog() == true) {
                    //Specifies which ext type was choise
                    int filterIndex = saveFile.FilterIndex;                    

                    //Initiate Save
                    ppmImg.SaveImage(saveFile.FileName, filterIndex);

                    //Confirm encryption 
                    MessageBox.Show("Encryption was successful.");
                } else {
                    MessageBox.Show("Encryption wasn't successful.");
                }//end else 
            }//end if
        }//end method 

        private string BinaryConversion(int input) {
            //Format char's dec value to binary
            string binary = Convert.ToString(input, 2);

            //Reformat strings to have 8 digit placements
            binary = binary.PadLeft(8, '0');

            return binary;
        }//end method 

        public static string Reverse(string message) { //Reverse message for harder detection
            //Used to reverse message to better hide message
            char[] charArray = message.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }//end method
        #endregion


    }//end class 
}//end namespace 
