using System.IO;
using System.Windows.Media;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Text;
using System;

namespace SecretEncryption {
    public class PortablePixelMap {
        //THE BITMAP'S SIZE.
        private int _width;
        private int _height;

        //THE PIXEL ARRAY.
        private byte[] _pixelData;

        //THE NUMBER OF BYTES PER ROW.
        private int _stride;

        //PPM REFERENCE
        private PortablePixelMap _ppm;

        #region PROPERTIES
        public int Length { 
            get { return _pixelData.Length; } 
        }//end property

        public int Width {
            get { return _width; }
        }//end property
        public int Height {
            get { return _height; }
        }//end property
        #endregion

        #region CONSTRUCTORS
        public PortablePixelMap() {
            //parameterless 
        }//end constructor 

        public PortablePixelMap(string filePath) {
            //string parameter
            LoadImage(filePath);
        }//end constructor
        #endregion

        private void LoadImage(string filePath) {
            string readFirstLn = File.ReadLines(filePath).First();

            if (readFirstLn.Contains("P3")) {
                ReadP3(filePath);
            } else if (readFirstLn.Contains("P6")) {
                ReadP6(filePath);
            } //end if

        }//end method
         
        public void SaveImage(string savePath, int selectedType) {
            //Saves to specific ppm file 
            if (selectedType == 1) {
                SaveP3(savePath);
            } else if (selectedType == 2) {
                SaveP6(savePath);
            }//end else if    
        }//end method 

        private void ImageData() {
            // Create the pixel array.
            _pixelData = new byte[_width * _height * 4];

            // Calculate the stride.
            _stride = _width * 4;
        }//end method 

        private void ReadP3(string filePath) {
            StreamReader reader = new StreamReader(filePath);

            //Line 1 - Check if 'P3'
            if (reader.ReadLine().Contains("P3")) {

                //Skip Line 2 - GIMP line 
                reader.ReadLine();

                //Line 3 - Get char values of width and height
                string[] dimensionSize = reader.ReadLine().Split(" ");
                _width = String2Int(dimensionSize[0]);
                _height = String2Int(dimensionSize[1]);

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

                    #region ORIGINAL
                    /*
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
                    */
                    #endregion
                }//end if
            }//end if 

            //Close streamreader
            reader.Close();
        }//end method 

        private void ReadP6(string filePath) {
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

            //Get stride and pixel data  
            ImageData();

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

            #region ORIGINAL
            //Basestream: Exposes access to the underlying stream of the BinaryReader
            //Determine if end of file has been reached; current position compared to pixel length of image
            //while (rawReader.BaseStream.Position != rawReader.BaseStream.Length) {
            //    Color currentColor = new Color();
            //
            //    //Read each byte and save to color object
            //    //Color lines in file are arranged in reverse order 
            //    currentColor.B = rawReader.ReadByte();
            //    currentColor.G = rawReader.ReadByte();
            //    currentColor.R = rawReader.ReadByte();
            //
            //    //Set color in current position 
            //    _pixelData[counter++] = currentColor.R;
            //    _pixelData[counter++] = currentColor.G;
            //    _pixelData[counter++] = currentColor.B;
            //    _pixelData[counter++] = 255;
            //}//end while
            #endregion

            //Close binaryreader
            rawReader.Close();
        }//end method 

        private void SaveP3(string savePath) {
            //Use a streamwriter to write the text part of the encoding
            StreamWriter asciiWriter = new StreamWriter(savePath);

            //Line 1: PPM Type 
            asciiWriter.Write("P3" + "\n");

            //Line 2: Author line
            asciiWriter.Write("# Created by GIMP version 2.10.30 PNM plug-in" + "\n");

            //Line 3: Width and Height
            asciiWriter.Write($"{_width} {_height}" + "\n");

            //Line 4: Max RGB 
            asciiWriter.Write("255" + "\n");

            //Line 5+: Write ascii byte values 
            for (int y = 0; y < _height; y++) {
                for (int x = 0; x < _width; x++) {
                    Color currentColor = GetPixelColor(x, y);

                    //Write ascii data into text file 
                    asciiWriter.Write(currentColor.R.ToString() + "\n");
                    asciiWriter.Write(currentColor.G.ToString() + "\n");
                    asciiWriter.Write(currentColor.B.ToString() + "\n");
                }//end for
            }//end for 

            asciiWriter.Close();
        }//end method

        public void SaveP6(string savePath) {
            //Use a streamwriter to write the text part of the encoding
            StreamWriter writer = new StreamWriter(savePath);

            //Line 1: PPM Type 
            writer.Write("P6" + "\n");

            //Line 2: Author line
            writer.Write("# Created by GIMP version 2.10.30 PNM plug-in" + "\n");

            //Line 3: Width and Height
            writer.Write($"{_width} {_height}" + "\n");

            //Line 4: Max RGB 
            writer.Write("255" + "\n");

            writer.Close();

            //Switch to binary writer and add-on to file 
            BinaryWriter binaryWriter = new BinaryWriter(new FileStream(savePath, FileMode.Append));            

            //Iterate thru image's pixel and saving as byte values onto txt file 
            for (int y = 0; y < _height; y++) {
                for (int x = 0; x < _width; x++) {
                    Color currentColor = GetPixelColor(x, y);

                    //Print RGB values into txt file 
                    binaryWriter.Write(currentColor.R);
                    binaryWriter.Write(currentColor.G);
                    binaryWriter.Write(currentColor.B);

                }//end for
            }//end for 

            //Close binary writer
            binaryWriter.Close();
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

    }//end class

}//end namespace 
