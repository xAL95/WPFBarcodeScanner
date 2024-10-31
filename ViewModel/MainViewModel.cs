using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using WPFBarcodeScanner.Services.Command;
using ZXing;
using BarcodeReader = ZXing.Presentation.BarcodeReader;


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


        private BitmapImage _imageSource;
        public BitmapImage ImageSource
        {
            get => _imageSource;
            set
            {
                _imageSource = value;
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


        private void DecodeImageSource()
        {
            if (ImageSource == null)
            {
                return;
            }

            var result = barcodeReader.Decode(ImageSource);
            if (result != null)
            {
                BarcodeText = result.Text;
            }
            else
            {
                BarcodeText = "ERROR";
            }
        }


        private void OnLoadBarcodeImage()
        {
            // Open a file dialog to select an image
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png";

            if (openFileDialog.ShowDialog() == true)
            {
                // load and set image source
                ImageSource = new BitmapImage(new Uri(openFileDialog.FileName));

                // decode barcode 
                DecodeImageSource();
            }
        }


    }



}

