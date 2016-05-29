using System.Collections.Generic;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Util;
using Com.Google.Atap.Tangoservice;
using Java.Lang;
using Exception = System.Exception;

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
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.ButtonStart);

            button.Click += (s, e) => StartSensorReading();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (_tango != null)
                _tango.Disconnect();
        }

        private void StartSensorReading()
        {
            _tango = new Tango(this);

            /* Tango Configuration */

            _tangoConfig = _tango.GetConfig(TangoConfig.ConfigTypeDefault);

            // Whether deep percetion should be enabled (point cloud)
            _tangoConfig.PutBoolean(TangoConfig.KeyBooleanDepth, true);

            // Whether the tango system should auto reset in case of failure
            _tangoConfig.PutBoolean(TangoConfig.KeyBooleanAutorecovery, true);

            // Whether the area should be learned while operating (can be saved as ADF later)
            _tangoConfig.PutBoolean(TangoConfig.KeyBooleanLearningmode, false);

            // Whether motion (translation / rotation) should be captured
            _tangoConfig.PutBoolean(TangoConfig.KeyBooleanMotiontracking, true);

            // If an ADF is existing this config option loads the appropriate area description file
            // _tangoConfig.PutString(TangoConfig.KeyStringAreadescription, "ADF-UUID");

            /* Create Tango Listener */
            var framePairs = new List<TangoCoordinateFramePair>();
            framePairs.Add(new TangoCoordinateFramePair(TangoPoseData.CoordinateFrameStartOfService, TangoPoseData.CoordinateFrameDevice));
            
            // Add this line if a ADF is loaded
            framePairs.Add(new TangoCoordinateFramePair(TangoPoseData.CoordinateFrameAreaDescription, TangoPoseData.CoordinateFrameDevice));

            _tangoUpdateListener = new TangoUpdateListener();
            _tango.ConnectListener(framePairs, _tangoUpdateListener);

            try
            {
                _tango.Connect(_tangoConfig);
            }
            catch (Exception ex)
            {
                Log.Error(Tag, Throwable.FromException(ex), ex.Message);
            }
        }
    }

    public class TangoUpdateListener : Java.Lang.Object, Tango.IOnTangoUpdateListener
    {
        public void OnFrameAvailable(int p0)
        {
            // Nothing
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

