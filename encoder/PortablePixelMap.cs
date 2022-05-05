using System.IO;
using System.Windows.Media;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows;
using System;

namespace Secret_Decoder {
    public class PortablePixelMap {
        //THE BITMAP'S SIZE.
        private int _width;
        private int _height;

        //THE PIXEL ARRAY.
        private byte[] _pixelData;

        //THE NUMBER OF BYTES PER ROW.
        private int _stride;

        //REF FILEPATH
        private string _readFirstLn = string.Empty;

        #region PROPERTIES
        public int Width {
            get { return _width; }
        }//end property
        public int Height {
            get { return _height; }
        }//end property
        #endregion

        //Constructors 
        public PortablePixelMap() {
            //parameterless 
        }//end constructor 

        public PortablePixelMap(string filePath) {
            //string parameter
            IdentifyType(filePath);
        }//end constructor

        private void IdentifyType(string filePath) {
            _readFirstLn = File.ReadLines(filePath).First();

            if (_readFirstLn.Contains("P3")) {
                ReadP3(filePath);
            } else if (_readFirstLn.Contains("P6")) {
                ReadP6(filePath);
            } //end if

        }//end method 

        private void ReadP3(string filePath) {
            StreamReader reader = new StreamReader(filePath);

            //Line 1 - Check if 'P3'
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            if (reader.ReadLine().Contains("P3")) {

                //Skip Line 2 - GIMP line 
                reader.ReadLine();

                //Line 3 - Get char values of width and height
                string[] dimensionSize = reader.ReadLine().Split(" ");
                string stringWidth = dimensionSize[0];
                string stringHeight = dimensionSize[1];
                _width = String2Int(stringWidth);
                _height = String2Int(stringHeight);

                //Line 4 - Checks for max RGB
                if (reader.ReadLine().Contains("255")) {
                    //Obtain image stride and pixel data
                    ImageData();

                    for (int y = 0; y < _height; y++) {
                        for (int x = 0; x < _width; x++) {
                            Color currentColor = new Color();

                            //Read each byte and save to color object
                            int red = String2Int(reader.ReadLine());
                            int green = String2Int(reader.ReadLine());
                            int blue = String2Int(reader.ReadLine());

                            //Convert int to byte
                            currentColor.R = Convert.ToByte(red);
                            currentColor.G = Convert.ToByte(green);
                            currentColor.B = Convert.ToByte(blue);

                            //Set pixel in place (retrieve for saving)
                            SetPixel(x, y, currentColor.R, currentColor.G, currentColor.B);
                        }//end for 
                    }//end for 

                    #region ORIGINAL IMPLEMENTATION 
                    /*
                    int counter = 0; 
                     
                    while (!reader.EndOfStream) {
                        Color currentColor = new Color();

                        //Read each line in file and convert str to int 
                        //Note: Sequence of file's color is read blue,green, then red
                        #pragma warning disable CS8604 // Possible null reference argument.
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
                    */
                    #endregion

                }//end if
            }//end if 

            //Close streamreader
            reader.Close();
        }//end method 

        private void ReadP6(string filePath) {
            #pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.

            StreamReader asciiReader = new StreamReader(filePath);
            string countTo = string.Empty;

            //Line 1: Read file type
            string ppmType = asciiReader.ReadLine();

            if (!ppmType.Contains("P6")) {
                throw new ArgumentException("Could not read ppm file");
            }//end if
            countTo += ppmType + "\n";

            //Line 2: Skip publisher line
            string publisher = asciiReader.ReadLine();
            countTo += publisher + "\n";

            //Line 3: Get image dimensions 
            string size = asciiReader.ReadLine();
            string[] dimensionSize = size.Split(" ");
            string stringWidth = dimensionSize[0];
            string stringHeight = dimensionSize[1];
            _width = String2Int(stringWidth);
            _height = String2Int(stringHeight);
            countTo += size + "\n";

            //Line 4: Max RGB 
            string maxRGB = asciiReader.ReadLine();
            countTo += maxRGB + "\n";

            //Close stream reader
            asciiReader.Close();

            //Obtain image stride and pixel data
            ImageData();

            #pragma warning restore CS8600 // Converting null literal or possible null value to non-nullable type.

            //Line 5+: 
            int counter = 0;
            BinaryReader rawReader = new BinaryReader(new FileStream(filePath, FileMode.Open));

            //Iterate to index position left off 
            while (counter < countTo.Length) {
                char read = rawReader.ReadChar();
                counter++;
            }//end while 

            //Reset counter 
            counter = 0;

            for (int y = 0; y < _height; y++) {
                for (int x = 0; x < _width; x++) {
                    Color currentColor = new Color();

                    //Read each byte and save to color object
                    currentColor.R = rawReader.ReadByte();
                    currentColor.G = rawReader.ReadByte();
                    currentColor.B = rawReader.ReadByte();

                    //Set pixel in place (retrieve for saving)
                    SetPixel(x, y, currentColor.R, currentColor.G, currentColor.B);
                }//end for 
            }//end for            

            //Close binaryreader
            rawReader.Close();
        }//end method 

        #region ORIGINAL IMPLEMENTATION 
        /*
        //Basestream: Exposes access to the underlying stream of the BinaryReader
        //Determine if end of file has been reached; current position compared to pixel length of image
        while (rawReader.BaseStream.Position != rawReader.BaseStream.Length) {
            Color currentColor = new Color();

            //Read each byte and save to color object
            //Color lines in file are arranged in reverse order 
            currentColor.B = rawReader.ReadByte();
            currentColor.G = rawReader.ReadByte();
            currentColor.R = rawReader.ReadByte();

            //Set color in current position 
            _pixelData[counter++] = currentColor.R;
            _pixelData[counter++] = currentColor.G;
            _pixelData[counter++] = currentColor.B;
            _pixelData[counter++] = 255;
        }//end while
        */
        #endregion

        private void ImageData() {
            // Create the pixel array.
            _pixelData = new byte[_width * _height * 4];

            // Calculate the stride.
            _stride = _width * 4;
        }//end method 

        private int String2Int(string input) {
            int output;
            int.TryParse(input, out output);
            return output;
        }//end method 

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

        public void SetPixel(int x, int y, byte red, byte green, byte blue) {
            SetPixel(x, y, red, green, blue, 255);
        }//end method

        public void SetPixel(int x, int y, byte red, byte green, byte blue, byte alpha) {
            int index = y * _stride + x * 4;
            _pixelData[index++] = blue;
            _pixelData[index++] = green;
            _pixelData[index++] = red;
            _pixelData[index++] = alpha;
        }//end method
    #pragma warning restore CS8602 // Dereference of a possibly null reference.

    }//end class
}//end namespace
