# ThreeDPoseTracker (preparing now)
## Install and Tutorial
### Download and put files
1. Download the zip file from the releases page and unzip it.</br>

2. Download onnx from our home page by clicking following URL in our HP.</br>
   https://digital-standard.com/threedpose/models/HighQualityTrainedModel.nn
   
3. Move the downloaded HighQualityTrainedModel.nn file to ThreeDPoseTracker_Data/StreamingAssets in the unzipped folder.</br>

### How to Use
1. Since we haven't created an quit button yet, check Windowed in the first Configuration and press the Play button.</br></br>
![200319-01.png](image/200319-01.png)</br></br>

2. When the window opens, select whether to use a video file or a USB camera from the Source device.</br>
　　Here is a sample movie file.(https://github.com/digital-standard/ThreeDPoseTracker/tree/master/SampleVideo)</br>  
   If you connect a USB camera after starting the application, restart the application.</br>
   ![200324-01.png](image/200324-01.png)</br>

3. Configuraton Menu</br></br>
![200324-02.png](image/200324-02.png)</br></br>
Switch the trained model on the Configuration screen. Select high quality or low quality in the item of "Trained Model". High quality Model recommends GPU of GTX1070 or more.</br></br>

　　![200324-03.png](image/200324-03.png)</br>
　　If it exceeds 100fps, please adjust LowPassFilter to about 0.5 on the Configuration screen</br></br>

4. Add Avatar Menu</br>
  
![200323-01.png](image/200323-01.png)</br></br>

Select Avatar in the Avatar menu. You can add VRM Avatar on the "Add Avatar" screen.</br>
   Here is a sample VRM file.(https://github.com/digital-standard/ThreeDPoseTracker/tree/master/Avatars)</br></br>
![200323-02.png](image/200323-02.png)</br>


   Upload the file from the "File" button.</br>
   ※FBX files are not yet available.</br>
   Avatar name: Avatar name</br>
   Default position: Start position</br>
   Depth scale in: Depth scale of depth movement ratio</br>
   Scale: Avatar size</br>
   Skeleton: Skeleton Display</br>
   Default position: Skeleton start position</br>
   Scale: Skeleton size</br></br>

5. Close Menu</br>
The button to hide the menu. The menu is displayed again by pressing the space key.
While the menu is not displayed, you can change the camera angle by left-clicking the mouse and change the avatar by right-clicking.</br></br>
![200319-04.png](image/200319-04.png)</br></br>


## Source
Created with Unity ver 2019.3.13f1.</br>
We use Barracuda 1.0.0 to load onnx.</br>

Unity Standalone File Browser 1.2(https://github.com/gkngkc/UnityStandaloneFileBrowser)</br>
</br>
## Performance Report
### High Quality Trained Model </br>
GeForce RTX2070 SUPER ⇒ About 30 FPS </br>
GeForce GTX1070 ⇒ About 20 FPS </br>
### Low Quality Trained Model </br>
GeForce RTX2070 SUPER ⇒ About 60 FPS </br>


## License
### Non-commercial use</br>
・Please use it freely for hobbies and research. </br>
  When redistributing, it would be appreciated if you could enter a credit (Digital-  Standard Co., Ltd.).</br></br>
   
・The videos named as "Action_with_wiper.mp4"(
original video: https://www.youtube.com/watch?v=C9VtSRiEM7s) and "onegai_darling.mp4"(original video: https://www.youtube.com/watch?v=tmsK8985dyk) contained in this code are not copyright free.
  So you should not use those of files in other places without permission.</br></br>

### Unitychan</br>
We follow the Unity-Chan License Terms.</br>
https://unity-chan.com/contents/license_en/</br>
![Light_Frame.png](image/Light_Frame.png)</br></br>

### Commercial use</br>
・In the case of developping application including free application for advertising, it costs $500 per developer.
  About complicated case, please contact us.</br>
・In the case of using this on cloud server, please contact us.</br>
・In the case of creating motion or delivering the video, and the earnings surpass $500 or there is the expectation,
  it costs $500. Otherwise it is for free.</br></br>
  
## Contact</br>
If you have any questions, please feel free to contact us from following URL.</br>
Contact form:  https://digital-standard.com/contact/ </br>
E-Mail: info@digital-standard.com </br>
https://digi-rooms.com/
