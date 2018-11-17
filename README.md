# VRM Live Motion Streamer

## Tested Environment
- Unity 2017.4.14f1
- Windows 10

## Third party assets
The following assets are included in this project.
- [UniVRM v0.43](https://github.com/dwango/UniVRM/releases/tag/v0.43)  
Licensed under the MIT License.
Copyright (c) DWANGO Co., Ltd.  

You need to import the following assets from Unity Asset Store.
- [Photon Voice 2](https://assetstore.unity.com/packages/tools/audio/photon-voice-2-130518)
    - Photon Voice : v2.1 (November 8th, 2018)
    - PUN2 : v2.4 (24th October 2018)
- [Kinect v2 Examples with MS-SDK and Nuitrack SDK](https://assetstore.unity.com/packages/3d/characters/kinect-v2-examples-with-ms-sdk-and-nuitrack-sdk-18708) (Optional)
    - You need to change 'sensorAlwaysAvailable' at Kinect2Interface.cs to false,  
      if you aren't using Kinect-v2 only and want to check for available sensors. 

## License
- [MIT License](https://github.com/sotanmochi/VRMLiveMotionStreamer/blob/master/LICENSE.txt)
