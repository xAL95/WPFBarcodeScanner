using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFBarcodeScanner.Services.Command;
using ZXing;
using BarcodeReader = ZXing.Presentation.BarcodeReader;
using BarcodeWriter = ZXing.Presentation.BarcodeWriter;
using BarcodeWriterGeometry = ZXing.Presentation.BarcodeWriterGeometry;

namespace WPFBarcodeScanner.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public ICommand LoadBarcodeImage { get; }
        public ICommand TakeBarcodeImage { get; }


        private readonly BarcodeReader barcodeReader;

        private BarcodeFormat barcodeFormat;

        private BitmapImage _imageGeneratedBarcode;
        public BitmapImage ImageGeneratedBarcode
        {
            get => _imageGeneratedBarcode;
            set
            {
                _imageGeneratedBarcode = value;
                OnPropertyChanged();
            }
        }

        private BitmapImage _imageBarcode;
        public BitmapImage ImageBarcode
        {
            get => _imageBarcode;
            set
            {
                _imageBarcode = value;
                OnPropertyChanged();
            }
        }

        private string _barcodeText;
        public string BarcodeText
        {
            get => _barcodeText;
            set
            {
                _barcodeText = value;
                OnPropertyChanged();
            }
        }

        public MainViewModel()
        {
            LoadBarcodeImage = new RelayCommand(OnLoadBarcodeImage);

            barcodeReader = new BarcodeReader
            {
                AutoRotate = true
            };
        }


        private void DecodeImageBarcode()
        {
            if (ImageBarcode == null)
            {
                return;
            }

            var result = barcodeReader.Decode(ImageBarcode);
            if (result != null)
            {
                BarcodeText = result.Text;

                GenerateBarcodeImage(result.BarcodeFormat);
            }
            else
            {
                BarcodeText = "ERROR";
            }

        }


        private void OnLoadBarcodeImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                ImageBarcode = new BitmapImage(new Uri(openFileDialog.FileName));

                // decode barcode 
                DecodeImageBarcode();
            }
        }

        private void GenerateBarcodeImage(BarcodeFormat barcodeFormat)
        {
            var writer = new BarcodeWriter
            {
                Format = barcodeFormat,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 138,
                    Width = 543,
                    Margin = 0
                }
            };

            var image = writer.Write(BarcodeText);
            ImageGeneratedBarcode = ConvertWriteableBitmapToBitmapImage(image);
        }

        private BitmapImage ConvertWriteableBitmapToBitmapImage(WriteableBitmap writeableBitmap)
        {
            // Initialize a memory stream to hold the image data
            using (var memoryStream = new MemoryStream())
            {
                var pngEncoder = new PngBitmapEncoder();
                pngEncoder.Frames.Add(BitmapFrame.Create(writeableBitmap));
                pngEncoder.Save(memoryStream);

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                memoryStream.Seek(0, SeekOrigin.Begin);
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.EndInit();
                bitmapImage.Freeze();

                return bitmapImage;
            }
        }


    }



}

