using Android.App;
using Android.OS;
using Android.Widget;
using Android.Graphics;
using Xamarin.Controls;
using Android.Views;
using System;
using Android;
using System.IO;
using AndroidX.Core.Content;

namespace SignatureToImage
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : Activity
    {
        SignaturePadView signatureView;
        Button summary, clear;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.activity_main);
            initializeViews();

            checkPermission();
        }

        /// <summary>
        /// init views from Main.axml file
        /// </summary>
        private void initializeViews()
        {
            summary = FindViewById<Button>(Resource.Id.summary);
            signatureView = FindViewById<SignaturePadView>(Resource.Id.signatureView);
            clear = FindViewById<Button>(Resource.Id.clear);

            clear.Click += delegate
            { signatureView.Clear(); };

            summary.Click += delegate
            {
                signatureModification();

                Bitmap getBitmap = createBitmapFromView(signatureView);
                ExportBitmapAsPNG(getBitmap, "Signature.png");
            };
        }
        /// <summary>
        /// check R/W permissions
        /// </summary>
        private void checkPermission()
        {
           if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Android.Content.PM.Permission.Granted)
                    {
                        RequestPermissions(new string[] { Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, 0);
                    }
           
        }
        /// <summary>
        /// modify signature pad view before saving
        /// </summary>
        private void signatureModification()
        {
            signatureView.CaptionText = String.Empty;
            signatureView.SignatureLineColor = Color.Transparent;
        }
        /// <summary>
        /// create bitmap from given View
        /// </summary>
        public Bitmap createBitmapFromView(View bm)
        {
            Bitmap bitmap = Bitmap.CreateBitmap(bm.Width, bm.Height, Bitmap.Config.Argb8888);
            Canvas canvas = new Canvas(bitmap);
            bm.Draw(canvas);
            return bitmap;
        }

        /// <summary>
        /// save Image to local storage
        /// </summary>
        void ExportBitmapAsPNG(Bitmap bmp, string imgName)
        {
            try 
            { 
            //var sdCardPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            var locaFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            var filePath = System.IO.Path.Combine(locaFolder, imgName);
            var stream = new FileStream(filePath, FileMode.Create);
            bmp.Compress(Bitmap.CompressFormat.Png, 100, stream);
            stream.Close();
            }
            catch (Exception ex)
            {
                Toast.MakeText(this, $"Error: {ex}", ToastLength.Long).Show();
            }
        }
    }
}