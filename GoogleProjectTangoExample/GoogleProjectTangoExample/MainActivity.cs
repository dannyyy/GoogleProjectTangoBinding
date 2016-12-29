using System.Collections.Generic;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Com.Google.Atap.Tangoservice;
using Java.Lang;
using Exception = System.Exception;
using Com.Projecttango.Tangosupport;

namespace GoogleProjectTangoExample
{
    [Activity(Label = "GoogleProjectTangoExample", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        public static string Tag = "MainActivity";

        private Tango _tango;
        private TangoConfig _tangoConfig;
        private TangoUpdateListener _tangoUpdateListener;

        protected override void OnCreate(Bundle bundle)
        {
            Log.Debug(Tag, "OnCreate");
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Log.Debug(Tag, "OnCreate Done");
        }

        protected override void OnDestroy()
        {
            Log.Debug(Tag, "OnDestroy");
            base.OnDestroy();
        }

        protected override void OnPause()
        {
            Log.Debug(Tag, "OnPause");
            base.OnPause();
            _tango.Disconnect();
        }

        protected override void OnResume()
        {
            Log.Debug(Tag, "OnResume");
            base.OnResume();

            _tango = new Tango(this, new Runnable(() =>
            {
                Log.Debug(Tag, "TangoRunnable");
                //try
                //{
                TangoSupport.Initialize();
                    _tangoConfig = SetupTangoConfig(_tango);
                    _tango.Connect(_tangoConfig);
                    startupTango();
                //}
                //catch (TangoOutOfDateException e)
                //{
                //    Log.Error(Tag, GetString(R.
                //    string.exception_out_of_date),
                //    e)
                //    ;
                //}
                //catch (TangoErrorException e)
                //{
                //    Log.Error(Tag, GetString(R.
                //    string.exception_tango_error),
                //    e)
                //    ;
                //}
                //catch (TangoInvalidException e)
                //{
                //    Log.Error(Tag, GetString(R.
                //    string.exception_tango_invalid),
                //    e)
                //    ;
                //}
            }));
        }

        private void startupTango()
        {
            var framePairs = new List<TangoCoordinateFramePair>()
            {
                new TangoCoordinateFramePair(
                    TangoPoseData.CoordinateFrameStartOfService,
                    TangoPoseData.CoordinateFrameDevice)
            };
            _tangoUpdateListener = new TangoUpdateListener();
            _tango.ConnectListener(framePairs, _tangoUpdateListener);
        }

        private TangoConfig SetupTangoConfig(Tango tango)
        {
            // Create a new Tango Configuration and enable the MotionTrackingActivity API.
            TangoConfig config = tango.GetConfig(TangoConfig.ConfigTypeDefault);
            config.PutBoolean(TangoConfig.KeyBooleanMotiontracking, true);
            // Tango service should automatically attempt to recover when it enters an invalid state.
            config.PutBoolean(TangoConfig.KeyBooleanAutorecovery, true);
            return config;
        }
    }

    public class TangoUpdateListener : Java.Lang.Object, Tango.IOnTangoUpdateListener
    {
        public void OnFrameAvailable(int p0)
        {
            // Nothing
        }

        public void OnPointCloudAvailable(TangoPointCloudData p0)
        {
            
        }

        public void OnPoseAvailable(TangoPoseData p0)
        {
            if (p0.StatusCode != TangoPoseData.PoseValid)
                return;

            if (p0.Translation != null)
            {
                var x = p0.Translation[TangoPoseData.IndexTranslationX];
                var y = p0.Translation[TangoPoseData.IndexTranslationY];
                var z = p0.Translation[TangoPoseData.IndexTranslationZ];

                switch (p0.BaseFrame)
                {
                    case TangoPoseData.CoordinateFrameStartOfService:
                        // Do something with the coordinates and interprete them as relative to the start position (0/0/0)
                        break;
                    case TangoPoseData.CoordinateFrameAreaDescription:
                        // Do something with the coordinates and interprete them as relative to the ADF start position(0/0/0)
                        break;
                }
            }

            if (p0.Rotation != null)
            {
                var x = p0.Rotation[TangoPoseData.IndexRotationX];
                var y = p0.Rotation[TangoPoseData.IndexRotationY];
                var z = p0.Rotation[TangoPoseData.IndexRotationZ];
                var w = p0.Rotation[TangoPoseData.IndexRotationW];

                // Do somethine with these data (rotation as quaternion)
            }
        }

        public void OnTangoEvent(TangoEvent p0)
        {
            // Do somethinig with the status
        }

        public void OnXyzIjAvailable(TangoXyzIjData xyzIj)
        {
            for (var i = 0; i < xyzIj.Xyz.Capacity() - 3; i += 3)
            {
                var x = xyzIj.Xyz.Get(i);
                var y = xyzIj.Xyz.Get(i + 1);
                var z = xyzIj.Xyz.Get(i + 2);

                // Do somthing....
            }
        }
    }
}

