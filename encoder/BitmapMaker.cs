using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;

namespace Secret_Decoder {
    public class BitmapMaker {
        //THE BITMAP'S SIZE.
        private int _width;
        private int _height;

        //THE PIXEL ARRAY.
        private byte[] _pixelData;

        //THE NUMBER OF BYTES PER ROW.
        private int _stride;

        //REF FILEPATH
        //private string _readFirstLn = string.Empty;

        #region PROPERTIES
        public int Width {
            get { return _width; }
        }//end property
        public int Height {
            get { return _height; }
        }//end property

        //public BitmapMaker Resize {
        //    get { return ResizeImage(string path, int widthInput); }
        //}
        #endregion

        #region CONSTRUCTORS

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="width">Width of the bitmap in pixels</param>
        /// <param name="height">Height of the bitmap in pixels</param>
        public BitmapMaker(int width, int height) {
            // Save the width and height.
            _width = width;
            _height = height;

            // Create the pixel array.
            _pixelData = new byte[width * height * 4];

            // Calculate the stride.
            _stride = width * 4;
        }//end constructor

        public BitmapMaker(string imagePath) {
            //Add-on: Reads first line for ppm type 
            //_readFirstLn = File.ReadLines(imagePath).First();
            //
            //if (_readFirstLn.Contains("P3")) {
            //    ReadP3(imagePath);
            //} else if (_readFirstLn.Contains("P6")) {
            //    ReadP6(imagePath);
            //} else {
            //Every other extension type 
            LoadImage(imagePath);
            //}//end else if 
        }//end constructor
        #endregion

        /*
        #region MY METHODS
        public void InitPPM(string savePath) {
            //string selectedType = Console.ReadLine("");
            //if (selectedType)
            if (_readFirstLn.Contains("P3")) {  

            } else if (_readFirstLn.Contains("P6")) {
                WriteP6(savePath);
            } //end else if 
        }//end method 

        private void WriteP6(string savePath) {
            //Use a streamwriter to write the text part of the encoding
            StreamWriter asciiWriter = new StreamWriter(savePath);

            //Line 1: PPM Type 
            asciiWriter.WriteLine("P3");

            //Line 2: Author line
            asciiWriter.WriteLine("Created by Secret Encryption beta test");

            //Line 3: Width and Height
            asciiWriter.WriteLine($"{_width} {_height}");

            //Line 4: Max RGB 
            asciiWriter.WriteLine("255");

            asciiWriter.Close();

            //Switch to binary writer to data 
            BinaryWriter binaryWriter = new BinaryWriter(new FileStream(savePath, FileMode.Append));

            for (int x = 0; x < _height; x++) {
                for (int y = 0; y < _width; y++) {
                    //Color color = ;

                    //binaryWriter.Write(color.);
                    //binaryWriter.Write(color.);
                    //binaryWriter.Write(color.);

                }//end for
            }//end for 

            binaryWriter.Close();
        }//end method 


        private void ReadP3(string filePath) {
            StreamReader reader = new StreamReader(filePath);

            //Line 1 - Check if 'P3'
            if (reader.ReadLine().Contains("P3")) {

                //Skip Line 2 - GIMP line 
                reader.ReadLine();

                //Line 3 - Get char values of width and height
                string[] dimensionSize = reader.ReadLine().Split(" ");
                string stringWidth = dimensionSize[0];
                string stringHeight = dimensionSize[1];
                _width = String2Int(stringWidth);
                _height = String2Int(stringHeight);
                int counter = 0;

                //Line 4 - Checks for max RGB
                if (reader.ReadLine().Contains("255")) {
                    // Create the pixel array.
                    _pixelData = new byte[_width * _height * 4];

                    // Calculate the stride.
                    _stride = _width * 4;

                    while (!reader.EndOfStream) {
                        Color currentColor = new Color();

                        //Read each line in file and convert str to int 
                        //Note: Sequence of file's color is read blue,green, then red
                        int blue = String2Int(reader.ReadLine());
                        int green = String2Int(reader.ReadLine());
                        int red = String2Int(reader.ReadLine());

                        //Convert int to byte
                        currentColor.R = Convert.ToByte(red);
                        currentColor.G = Convert.ToByte(green);
                        currentColor.B = Convert.ToByte(blue);

                        //Set color in current position 
                        _pixelData[counter++] = currentColor.R;
                        _pixelData[counter++] = currentColor.G;
                        _pixelData[counter++] = currentColor.B;
                        _pixelData[counter++] = 255;
                    }//end while 
                }//end if
            }//end if 

            //Close streamreader
            reader.Close();
        }//end method 

        private void ReadP6(string filePath) {
            BinaryReader reader = new BinaryReader(new FileStream(filePath, FileMode.Open));
            string stringWidth = "", stringHeight = "";
            char currentChar;
            int counter = 0;

            //Line 1 - Skip PPM Line
            if (reader.ReadChar() != 'P' || reader.ReadChar() != '6') {
                //throw new ArgumentException("Wrong file type");
                return;
            } //end if 

            reader.ReadChar(); //skip newline char
            char temp = reader.ReadChar();

            //Skip line 2 - Skip GIMP txt 
            while (temp != '\n') {
                temp = reader.ReadChar();
            }//end while 


            //Line 3 - Get char values of width and height and convert to int
            while ((currentChar = reader.ReadChar()) != ' ') { //width                                 
                stringWidth += currentChar;
            }//end while 

            while ((currentChar = reader.ReadChar()) != '\n') { //height - check condition ****
                stringHeight += currentChar;
            }//end while 

            _width = String2Int(stringWidth);
            _height = String2Int(stringHeight);

            //Line 4 - Check maximum rgb 255
            if (reader.ReadChar() != '2' || reader.ReadChar() != '5' || reader.ReadChar() != '5') {
                return;
            }//end if 
            temp = reader.ReadChar(); //skip newline char

            //Line 4 - Read each binary 
            BitmapMaker bmP6 = new BitmapMaker(_width, _height); //create instance

            // Create the pixel array.
            _pixelData = new byte[_width * _height * 4];

            // Calculate the stride.
            _stride = _width * 4;

            //Basestream: Exposes access to the underlying stream of the BinaryReader
            //Determine if end of file has been reached; current position compared to pixel length of image
            while (reader.BaseStream.Position != reader.BaseStream.Length) {
                Color currentColor = new Color();

                //Read each byte and save to color object
                //Color lines in file are arranged in reverse order 
                currentColor.B = reader.ReadByte();
                currentColor.G = reader.ReadByte();
                currentColor.R = reader.ReadByte();

                //Set color in current position 
                _pixelData[counter++] = currentColor.R;
                _pixelData[counter++] = currentColor.G;
                _pixelData[counter++] = currentColor.B;
                _pixelData[counter++] = 255;
            }//end while


                    //Assign byte value to each pixel location 
                    //currentColor.R = reader.ReadByte();
                    //_pixelData[counter++] = currentColor.R;
                    //
                    //currentColor.G = reader.ReadByte();
                    //_pixelData[counter++] = currentColor.G;
                    //
                    //currentColor.B = reader.ReadByte();
                    //_pixelData[counter++] = currentColor.B;
                    //
                    ////currentColor.A = 225;
                    //
                    ////Set current color into pixel location 
                    ////bmP6.SetPixel(x,y,currentColor);
                    //bmP6.SetPixel(x, y, currentColor.R, currentColor.G, currentColor.B);

        }//end method 

        private int String2Int(string input) {
            int output;
            int.TryParse(input, out output);
            return output;
        }//end method 
        #endregion
        */

        #region PUBLIC METHODS
        /// <summary>
        /// Get a pixel's component values in an array of buytes [r,g,b,a]
        /// </summary>
        /// <param name="x">The x position of the pixel to get data from.</param>
        /// <param name="y">The y position of the pixel to get data from.</param>
        /// <returns></returns>
        public byte[] GetPixelData(int x, int y) {
            //STARTING PIXEL INDEX
            int index = y * _stride + x * 4;

            //GET PIXEL COMPONENT VALUES 
            byte blu = _pixelData[index++];// ++ to march forward to get next component 
            byte grn = _pixelData[index++];
            byte red = _pixelData[index++];
            byte alp = _pixelData[index];

            //RETURN DATA
            return new byte[] { red, grn, blu, alp };
        }//end method

        /// <summary>
        /// Get a pixel's component values in a Color instance.
        /// </summary>
        /// <param name="x">The x position of the pixel to get data from.</param>
        /// <param name="y">The y position of the pixel to get data from.</param>
        /// <returns></returns>
        public Color GetPixelColor(int x, int y) {
            //GET PIXEL DATA
            byte[] pixelComponentData = GetPixelData(x, y);

            //CREAT COLOR INSTANCE
            Color returnColor = new Color();

            //POPULATE COLOR INSTANCE DATA THEN RETURN
            returnColor.R = pixelComponentData[0];
            returnColor.G = pixelComponentData[1];
            returnColor.B = pixelComponentData[2];
            returnColor.A = pixelComponentData[3];

            return returnColor;
        }//end method

        /// <summary>
        /// Set a pixel's color values
        /// </summary>
        /// <param name="x">zero based pixel x position</param>
        /// <param name="y">zero based pixel y position</param>
        /// <param name="red"> 0-255 level of red</param>
        /// <param name="green">0-255 level of green</param>
        /// <param name="blue">0-255 level of blue</param>
        /// <param name="alpha">0-255 level of alpha</param>
        public void SetPixel(int x, int y, byte red, byte green, byte blue, byte alpha) {
            int index = y * _stride + x * 4;
            _pixelData[index++] = blue;
            _pixelData[index++] = green;
            _pixelData[index++] = red;
            _pixelData[index++] = alpha;
        }//end method

        /// <summary>
        /// Set a pixel's color values
        /// </summary>
        /// <param name="x">zero based pixel x position</param>
        /// <param name="y">zero based pixel y position</param>
        /// <param name="red"> 0-255 level of red</param>
        /// <param name="green">0-255 level of green</param>
        /// <param name="blue">0-255 level of blue</param>
        public void SetPixel(int x, int y, byte red, byte green, byte blue) {
            SetPixel(x, y, red, green, blue, 255);
        }//end method

        /// <summary>
        /// Set a pixel's color values
        /// </summary>
        /// <param name="x">zero based pixel x position</param>
        /// <param name="y">zero based pixel y position</param>
        /// <param name="color">Color instance</param>
        public void SetPixel(int x, int y, Color color) {
            SetPixel(x, y, color.R, color.G, color.B, color.A);
        }//end method

        /// <summary>
        /// Set all pixels to a specific color.
        /// </summary>
        /// <param name="red"> 0-255 level of red</param>
        /// <param name="green">0-255 level of green</param>
        /// <param name="blue">0-255 level of blue</param>
        /// <param name="alpha">0-255 level of alpha</param>
        public void SetPixels(byte red, byte green, byte blue, byte alpha) {
            int byteCount = _width * _height * 4;
            int index = 0;

            while (index < byteCount) {
                _pixelData[index++] = blue;
                _pixelData[index++] = green;
                _pixelData[index++] = red;
                _pixelData[index++] = alpha;
            }//end while
        }//end method

        /// <summary>
        /// Set all pixels to a specific color.
        /// </summary>
        /// <param name="red"> 0-255 level of red</param>
        /// <param name="green">0-255 level of green</param>
        /// <param name="blue">0-255 level of blue</param>
        public void SetPixels(byte red, byte green, byte blue) {
            SetPixels(red, green, blue, 255);
        }//end method

        /// <summary>
        /// Create a WriteableBitmap
        /// </summary>
        /// <returns>WriteableBitmap inatance.</returns>
        public WriteableBitmap MakeBitmap() {
            // Create the WriteableBitmap.
            int dpi = 96;

            WriteableBitmap wbitmap = new WriteableBitmap(_width, _height, dpi, dpi, PixelFormats.Bgra32, null);

            // Load the pixel data.
            Int32Rect rect = new Int32Rect(0, 0, _width, _height);
            wbitmap.WritePixels(rect, _pixelData, _stride, 0);

            // Return the bitmap.
            return wbitmap;
        }//end method

        /// <summary>
        /// Returns a copy of the loaded pixel data in BGRA format.
        /// </summary>
        /// <returns>byte[]</returns>
        public byte[] GetPixelData() {
            byte[] returnData = new byte[_pixelData.Length];

            for (int index = 0; index < _pixelData.Length; index++) {
                returnData[index] = _pixelData[index];
            }//end for

            return returnData;
        }//end method 
        #endregion

        #region PRIVATE METHODS
        private void LoadImage(string path) {
            //OPEN THE IMAGE
            FileStream imageStream = File.OpenRead(path);

            BitmapDecoder pngDecoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.None, BitmapCacheOption.Default);

            //TODO CHECK IF IMAGE DATA FORMAT IS RGBA

            BitmapSource srcImage = pngDecoder.Frames[0];

            // SAVE THE WIDTH AND HEIGHT.
            _width = srcImage.PixelWidth;
            _height = srcImage.PixelHeight;

            // CALCULATE THE STRIDE.
            _stride = _width * 4;

            //CREATE AN ARRAY BIG ENOUGH TO HOLD THE PIXEL DATA
            _pixelData = new byte[_width * _height * 4];

            //STORE THE PIXEL DATA TO THE PRIVATE BACKING FIELD
            srcImage.CopyPixels(_pixelData, _stride, 0);
        }//end method

        //public void ResizeImage(string path, int widthInput) {//Created
        //    //OPEN THE IMAGE
        //    FileStream imageStream = File.OpenRead(path);
        //
        //    BitmapDecoder pngDecoder = BitmapDecoder.Create(imageStream, BitmapCreateOptions.None, BitmapCacheOption.Default);
        //
        //    BitmapSource srcImage = pngDecoder.Frames[0];
        //
        //    //Save the width and height
        //    _height = (int)Math.Ceiling((double)_height * widthInput / _width);
        //    _width = widthInput;
        //
        //    //Calculate the stride
        //    _stride = _width * 4;
        //
        //    //Create an array big enough to hold the pixel data 
        //    _pixelData = new byte[_width * _height * 4];
        //
        //    //Store the pixel data to the private backing field 
        //    srcImage.CopyPixels(_pixelData, _stride, 0);
        //
        //}//end method 

        #endregion

    }//end class
}//end namespace

