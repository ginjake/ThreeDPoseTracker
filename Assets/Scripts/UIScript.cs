using SFB;
using System.Collections;
using System.Collections.Generic;
using System.IO;
//using UnityEditor.Recorder;
//using UnityEditor.Recorder.Input;
using UnityEngine;
using UnityEngine.UI;
using VRM;

public class UIScript : MonoBehaviour
{
    private Material skeletonMaterial;

    public List<AvatarSetting> AvatarList = new List<AvatarSetting>();
    private ConfigurationSetting configurationSetting = new ConfigurationSetting();

    private CameraMover cameraMover;
    private GameObject Menu;
    private AvatarSettingScript avatarSetting;
    private ConfigurationScript configuration;
    private VNectBarracudaRunner barracudaRunner;
    private VideoCapture videoCapture;

    private GameObject pnlVideoIF;
    private Button btnPause;
    private InputField ifFrame;
    private Text txFrameCount;
    private Button btnSkip;

    private Dropdown sourceDevice;
    private Dropdown avatars;
    private Button btnSourceDevice;
    private Text txtFPS;

    public RawImage BackgroundImage;
    public Texture BackgroundTexture;
    public Camera Maincamera;

    private float AppVer;

    public MessageBoxScript message;

    private VRInputEmulatorWrapper vrInputEmulator = null;
    private List<VirtualController> virtualControllers = new List<VirtualController>();
    private List<VirtualController> virtualTrackers = new List<VirtualController>();
    private bool VRChat = false;
    private bool VRChatMotion = false;

    private void Awake()
    {
        AppVer = PlayerPrefs.GetFloat("AppVer", 0.11f);
        var configs = PlayerPrefs.GetString("Configuration", "");
        string[] cCol = configs.Split(',');
        if (cCol.Length == 15)
        {
            int.TryParse(cCol[0], out configurationSetting.ShowSource);
            int.TryParse(cCol[1], out configurationSetting.ShowInput);
            int.TryParse(cCol[2], out configurationSetting.SkipOnDrop);
            int.TryParse(cCol[3], out configurationSetting.RepeatPlayback);
            float.TryParse(cCol[4], out configurationSetting.SourceCutScale);
            float.TryParse(cCol[5], out configurationSetting.SourceCutX);
            float.TryParse(cCol[6], out configurationSetting.SourceCutY);
            float.TryParse(cCol[7], out configurationSetting.LowPassFilter);
            int.TryParse(cCol[8], out configurationSetting.TrainedModel);

            int.TryParse(cCol[9], out configurationSetting.ShowBackground);
            configurationSetting.BackgroundFile = cCol[10];
            float.TryParse(cCol[11], out configurationSetting.BackgroundScale);
            int.TryParse(cCol[12], out configurationSetting.BackgroundR);
            int.TryParse(cCol[13], out configurationSetting.BackgroundG);
            int.TryParse(cCol[14], out configurationSetting.BackgroundB);
        }
        else
        {
            SaveConfiguration(configurationSetting);
        }

        message = GameObject.Find("pnlMessage").GetComponent<MessageBoxScript>();
        message.Init();
        message.Hide();
    }

    void Start()
    {
        skeletonMaterial = Resources.Load("Skeleton", typeof(Material)) as Material;

        barracudaRunner = GameObject.Find("BarracudaRunner").GetComponent<VNectBarracudaRunner>();
        barracudaRunner.ModelQuality = configurationSetting.TrainedModel;

        videoCapture = GameObject.Find("MainTexrure").GetComponent<VideoCapture>();

        Menu = GameObject.Find("Menu");
        cameraMover = GameObject.Find("MainCamera").GetComponent<CameraMover>();

        sourceDevice = GameObject.Find("SourceDevice").GetComponent<Dropdown>();
        WebCamDevice[] devices = WebCamTexture.devices;
        foreach (var d in devices)
        {
            sourceDevice.options.Add(new Dropdown.OptionData(d.name));
        }
        sourceDevice.value = 0;
        
        btnPause = GameObject.Find("btnPause").GetComponent<Button>();
        ifFrame = GameObject.Find("ifFrame").GetComponent<InputField>();
        txFrameCount = GameObject.Find("txFrameCount").GetComponent<Text>();
        btnSkip = GameObject.Find("btnSkip").GetComponent<Button>();
        pnlVideoIF = GameObject.Find("pnlVideoIF");
        pnlVideoIF.SetActive(false);

        btnSourceDevice = GameObject.Find("btnSourceDevice").GetComponent<Button>();
        txtFPS = GameObject.Find("txtFPS").GetComponent<Text>();

        avatars = GameObject.Find("Avatars").GetComponent<Dropdown>();

        avatarSetting = GameObject.Find("AvatarSetting").GetComponent<AvatarSettingScript>();
        avatarSetting.Init();
        avatarSetting.gameObject.SetActive(false);
        configuration = GameObject.Find("Configuration").GetComponent<ConfigurationScript>();
        configuration.Init();
        configuration.gameObject.SetActive(false);

        ReflectConfiguration(configurationSetting);

        var settings = PlayerPrefs.GetString("AvatarSettings", "");
        //settings = "";
        // Decode Avatar Setting
        string[] asStr = settings.Split(';');
        foreach (var s in asStr)
        {
            string[] col = s.Split(',');
            if (col.Length != 16)
            {
                continue;
            }
            var setting = new AvatarSetting();

            if (!int.TryParse(col[0], out setting.AvatarType))
            {
                continue;
            }
            if (setting.AvatarType < 0)
            {

            }
            else if (setting.AvatarType == 0)
            {
                setting.VRMFilePath = col[1];
            }
            else if (setting.AvatarType == 1)
            {
                setting.FBXFilePath = col[1];
            }
            setting.AvatarName = col[2];
            if (!float.TryParse(col[3], out setting.PosX))
            {
                continue;
            }
            if (!float.TryParse(col[4], out setting.PosY))
            {
                continue;
            }
            if (!float.TryParse(col[5], out setting.PosZ))
            {
                continue;
            }
            if (!float.TryParse(col[6], out setting.DepthScale))
            {
                continue;
            }
            if (!float.TryParse(col[7], out setting.Scale))
            {
                continue;
            }
            if (!float.TryParse(col[8], out setting.FaceOriX))
            {
                continue;
            }
            if (!float.TryParse(col[9], out setting.FaceOriY))
            {
                continue;
            }
            if (!float.TryParse(col[10], out setting.FaceOriZ))
            {
                continue;
            }
            if (!int.TryParse(col[11], out setting.SkeletonVisible))
            {
                continue;
            }
            if (!float.TryParse(col[12], out setting.SkeletonPosX))
            {
                continue;
            }
            if (!float.TryParse(col[13], out setting.SkeletonPosY))
            {
                continue;
            }
            if (!float.TryParse(col[14], out setting.SkeletonPosZ))
            {
                continue;
            }
            if (!float.TryParse(col[15], out setting.SkeletonScale))
            {
                continue;
            }

            AvatarList.Add(setting);
        };

        if (AvatarList.Count == 0)
        {
            var setting = new AvatarSetting()
            {
                AvatarType = -1,
                AvatarName = "unitychan",
                Avatar = GameObject.Find("unitychan").GetComponent<VNectModel>(),
            };
            setting.Avatar.SetNose(setting.FaceOriX, setting.FaceOriY, setting.FaceOriZ);
            AvatarList.Add(setting);
            barracudaRunner.InitVNectModel(setting.Avatar);
            setting.Avatar.PoseUpdated = PoseUpdated;

            setting = new AvatarSetting()
            {
                AvatarType = -2,
                AvatarName = "YukihikoAoyagi",
                Avatar = GameObject.Find("YukihikoAoyagi").GetComponent<VNectModel>(),
            };
            setting.Avatar.SetNose(setting.FaceOriX, setting.FaceOriY, setting.FaceOriZ);
            AvatarList.Add(setting);
            barracudaRunner.InitVNectModel(setting.Avatar);
            setting.Avatar.PoseUpdated = PoseUpdated;

            setting = new AvatarSetting()
            {
                AvatarType = -3,
                AvatarName = "Tait",
                Avatar = GameObject.Find("Tait").GetComponent<VNectModel>(),
            };
            setting.Avatar.SetNose(setting.FaceOriX, setting.FaceOriY, setting.FaceOriZ);
            AvatarList.Add(setting);
            barracudaRunner.InitVNectModel(setting.Avatar);
            setting.Avatar.PoseUpdated = PoseUpdated;
        }

        avatars.options.Clear();
        foreach (var setting in AvatarList)
        {
            if (setting.AvatarType >= 0)
            {
                LoadAvatar(setting);
            }
            else if (setting.AvatarType < 0)
            {
                avatars.options.Add(new Dropdown.OptionData(setting.AvatarName));

                switch (setting.AvatarType)
                {
                    case -1:
                        setting.Avatar = GameObject.Find("unitychan").GetComponent<VNectModel>();
                        break;

                    case -2:
                        setting.Avatar = GameObject.Find("YukihikoAoyagi").GetComponent<VNectModel>();
                        break;

                    case -3:
                        setting.Avatar = GameObject.Find("Tait").GetComponent<VNectModel>();
                        SetVRMBounds(setting.Avatar.ModelObject.transform);

                        break;

                }

                setting.Avatar.SetNose(setting.FaceOriX, setting.FaceOriY, setting.FaceOriZ);
                barracudaRunner.InitVNectModel(setting.Avatar);
                setting.Avatar.PoseUpdated = PoseUpdated;
            }
        }
        avatars.value = 0;
        ChangedAvatar(0);
    }

    public void NextAvatar()
    {
        var v = avatars.value + 1;
        if(AvatarList.Count <= v || AvatarList[v].Avatar == null)
        {
            v = 0;
        }
        avatars.value = v;
    }

    bool triggerDown = false;
    public float x0 = -0.8f;
    public float y0 = 1.3f;
    public float z0 = 0f;
    public float yaw0 = 90f;
    public float pitch0 = 0f;
    public float roll0 = 0f;
    public float x1 = 0.8f;
    public float y1 = 1.3f;
    public float z1 = 0f;
    public float yaw1 = -90f;
    public float pitch1 = 0f;
    public float roll1 = 0f;
    public float x2 = 0f;
    public float y2 = 0.8f;
    public float z2 = 0f;
    public float yaw2 = 0f;
    public float pitch2 = 0f;
    public float roll2 = 0f;
    public float x3 = -0.18f;
    public float y3 = 0f;
    public float z3 = 0f;
    public float yaw3 = 0f;
    public float pitch3 = 0f;
    public float roll3 = 0f;
    public float x4 = 0.18f;
    public float y4 = 0f;
    public float z4 = 0f;
    public float yaw4 = 0f;
    public float pitch4 = 0f;
    public float roll4 = 0f;

    void Update()
    {
        if(Menu != null && !Menu.activeSelf)
        {
            if(Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Escape))
            {
                Menu.gameObject.SetActive(true);
                cameraMover.CameraMoveActive = false;
            }

        }
        else if (barracudaRunner != null)
        {
            txtFPS.text = "FPS:" + barracudaRunner.FPS.ToString("0.0");
        }

        if (pnlVideoIF != null && pnlVideoIF.activeSelf && barracudaRunner.videoCapture.IsPlay())
        {
            ifFrame.text = barracudaRunner.videoCapture.VideoPlayer.frame.ToString();
            txFrameCount.text = barracudaRunner.videoCapture.VideoPlayer.frameCount.ToString();
        }

        if(VRChat)
        {
            if (Input.GetKey(KeyCode.Return))
            {
                if (virtualControllers.Count == 2)
                {
                    virtualControllers[1].ButtonEvent("press", VRInputEmulatorWrapper.EVRButtonId.k_EButton_System, 50);
                }
            }
            else if (Input.GetKey(KeyCode.Escape))
            {
                if (virtualControllers.Count == 2)
                {
                    virtualControllers[0].ButtonEvent("press", VRInputEmulatorWrapper.EVRButtonId.k_EButton_ApplicationMenu, 50);
                }
            }
            else if (Input.GetKey(KeyCode.P))
            {
                if (VRChatMotion)
                {
                    VRChatMotion = false;
                    if (virtualControllers.Count == 2)
                    {
                        virtualControllers[0].SetDevicePosition(-0.8f, 1.3f, 0f);
                        virtualControllers[0].SetDeviceRotation(90f, 0f, 0f);
                        virtualControllers[1].SetDevicePosition(0.8f, 1.3f, 0f);
                        virtualControllers[1].SetDeviceRotation(-90f, 0f, 0f);
                    }

                    if (virtualTrackers.Count == 5)
                    {
                        virtualTrackers[0].SetDevicePosition(0f, 0.8f, 0f);
                        virtualTrackers[1].SetDevicePosition(-0.18f, 0f, 0f);
                        virtualTrackers[2].SetDevicePosition(0.18f, 0f, 0f);
                    }
                }
                else
                {
                    VRChatMotion = true;
                }
            }
            else if (Input.GetKey(KeyCode.O))
            {
                if (!VRChatMotion)
                {
                    if (virtualControllers.Count == 2)
                    {
                        virtualControllers[0].SetDevicePosition(x0, y0, z0);
                        virtualControllers[0].SetDeviceRotation(yaw0, pitch0, roll0);
                        virtualControllers[1].SetDevicePosition(x1, y1, z1);
                        virtualControllers[1].SetDeviceRotation(yaw1, pitch1, roll1);
                    }
                    if (virtualTrackers.Count == 5)
                    {
                        virtualTrackers[0].SetDevicePosition(x2, y2, z2);
                        virtualTrackers[0].SetDeviceRotation(yaw2, pitch2, roll2);
                        virtualTrackers[1].SetDevicePosition(x3, y3, z3);
                        virtualTrackers[1].SetDeviceRotation(yaw3, pitch3, roll3);
                        virtualTrackers[2].SetDevicePosition(x4, y4, z4);
                        virtualTrackers[2].SetDeviceRotation(yaw4, pitch4, roll4);
                    }
                }
            }
            else if (Input.GetKey(KeyCode.Z))
            {
                if (virtualControllers.Count == 2)
                {
                    if (!triggerDown)
                    {
                        triggerDown = true;
                        virtualControllers[0].AxisEvent(1, "1", "0");
                        virtualControllers[1].AxisEvent(1, "1", "0");
                    }
                    else
                    {
                        triggerDown = false;
                        virtualControllers[0].AxisEvent(1, "0", "0");
                        virtualControllers[1].AxisEvent(1, "0", "0");
                    }
                }
            }
            else if (Input.GetKey(KeyCode.A))
            {
                VRChatMotion = false;
                if (!VRChatMotion)
                {
                    if (virtualControllers.Count == 2)
                    {
                        virtualControllers[0].SetDevicePosition(-0.8f, 1.3f, 0f);
                        virtualControllers[0].SetDeviceRotation(90f, 0f, 0f);
                        virtualControllers[1].SetDevicePosition(0.8f, 1.3f, 0f);
                        virtualControllers[1].SetDeviceRotation(40f, 23f, 0f);
                    }

                    if (virtualTrackers.Count == 3)
                    {
                        virtualTrackers[2].SetDevicePosition(0f, 0.8f, 0f);
                        virtualTrackers[3].SetDevicePosition(-0.18f, 0f, 0f);
                        virtualTrackers[4].SetDevicePosition(0.18f, 0f, 0f);
                    }
                }
            }
            else if (Input.GetKey(KeyCode.Space))
            {
                if (virtualControllers.Count == 2)
                {
                    if (!triggerDown)
                    {
                        triggerDown = true;
                        virtualControllers[1].AxisEvent(1, "1", "0");
                    }
                    else
                    {
                        triggerDown = false;
                        virtualControllers[1].AxisEvent(1, "0", "0");
                    }
                }
            }
        }
    }

    public void onCloseMenu()
    {
        Menu.gameObject.SetActive(false);
        cameraMover.CameraMoveActive = true;
    }

    public void onVideoPause()
    {
        if(barracudaRunner.videoCapture.IsPlay())
        {
            barracudaRunner.videoCapture.Pause();
        } 
        else if (barracudaRunner.videoCapture.IsPause())
        {
            barracudaRunner.videoCapture.Resume();
        }
    }

    public void onVideoSkip()
    {
        if (pnlVideoIF != null && pnlVideoIF.activeSelf)
        {
            long l = 0;
            if (long.TryParse(ifFrame.text, out l))
            {
                barracudaRunner.videoCapture.VideoPlayer.frame = l;
            }
        }
    }

    private void SetBackgroundImage(ConfigurationSetting config)
    {
        if(config.ShowBackground == 0)
        {
            BackgroundImage.gameObject.SetActive(false);
        }
        else
        {
            BackgroundImage.gameObject.SetActive(true);

            Texture texture;

            if (config.BackgroundFile != string.Empty)
            {
                texture = PngToTex2D(config.BackgroundFile);
            }
            else
            {
                texture = BackgroundTexture;
            }

            BackgroundImage.texture = texture;
            var rt = BackgroundImage.GetComponent< RectTransform>();
            rt.sizeDelta = new Vector2(texture.width, texture.height);
            BackgroundImage.rectTransform.localScale = new Vector3(config.BackgroundScale, config.BackgroundScale, config.BackgroundScale);
        }

        Maincamera.backgroundColor = new Color(config.BackgroundR / 255f, config.BackgroundG / 255f, config.BackgroundB / 255f );
    }

    Texture2D PngToTex2D(string path)
    {
        BinaryReader bin = new BinaryReader(new FileStream(path, FileMode.Open, FileAccess.Read));
        byte[] rb = bin.ReadBytes((int)bin.BaseStream.Length);
        bin.Close();
        int pos = 16, width = 0, height = 0;
        for (int i = 0; i < 4; i++) width = width * 256 + rb[pos++];
        for (int i = 0; i < 4; i++) height = height * 256 + rb[pos++];
        Texture2D texture = new Texture2D(width, height);
        texture.LoadImage(rb);
        return texture;
    }
    private void ReflectConfiguration(ConfigurationSetting config)
    {
        barracudaRunner.videoCapture.VideoScreen.gameObject.SetActive(config.ShowSource == 1);
        barracudaRunner.videoCapture.InputTexture.gameObject.SetActive(config.ShowInput == 1);
        barracudaRunner.videoCapture.VideoPlayer.skipOnDrop = (config.SkipOnDrop == 1);
        barracudaRunner.videoCapture.VideoPlayer.isLooping = (config.RepeatPlayback == 1);
        barracudaRunner.videoCapture.ResetScale(config.SourceCutScale, config.SourceCutX, config.SourceCutY);
        barracudaRunner.Smooth = config.LowPassFilter;

        SetBackgroundImage(config);
    }

    private void SaveConfiguration(ConfigurationSetting config)
    {
        PlayerPrefs.SetString("Configuration", config.ToString());
        PlayerPrefs.Save();
    }

    public void SetConfiguration(ConfigurationSetting config)
    {
        ReflectConfiguration(config);
        SaveConfiguration(config);
    }

    public void SourceDevice_Changed(int value)
    {
        if(value == 0)
        {
            btnSourceDevice.GetComponentInChildren<Text>().text = "Load Movie";
        }
        else
        {
            btnSourceDevice.GetComponentInChildren<Text>().text = "Start Cam";
        }
    }

    public void onSourceDevice()
    {
        barracudaRunner.PlayStop();

        if (sourceDevice.value == 0)
        {
            var extensions = new[]
            {
                new ExtensionFilter( "Movie Files", "mp4", "mov", "wmv" ),
            };
            var paths = StandaloneFileBrowser.OpenFilePanel("Open File", "", extensions, false);

            if (paths.Length != 0)
            {
                barracudaRunner.VideoPlayStart(paths[0]);
                pnlVideoIF.SetActive(true);
            }
        }
        else
        {
            barracudaRunner.CameraPlayStart(sourceDevice.value - 1);
            pnlVideoIF.SetActive(false);
        }
    }

    public void onConfiguration()
    {
        configuration.Show(this, configurationSetting);
    }

    public void Avatars_Changed(int value)
    {
        ChangedAvatar(value);
    }

    private void ChangedAvatar(int value)
    {
        DiactivateAvatars();
        var setting = AvatarList[value];
        setting.Avatar.gameObject.SetActive(true);
        setting.Avatar.SetSettings(setting);

        barracudaRunner.SetVNectModel(setting.Avatar);
    }

    public void onAddAvatar()
    {
        avatarSetting.ShowAddAvatar(this, new AvatarSetting());
    }

    public async void AddAvatar(AvatarSetting setting)
    {
        await onLoadVRMAsync(setting);

        AvatarList.Add(setting);
        avatars.value = avatars.options.Count - 1;
        SaveSetting();
    }

    public async void LoadAvatar(AvatarSetting setting)
    {
        await onLoadVRMAsync(setting);
        avatars.value = avatars.options.Count - 1;
    }

    public void onAvatarSetting()
    {
        avatarSetting.ShowAvatarSetting(this, AvatarList[avatars.value].Clone());
    }

    public void SetAvatar(AvatarSetting setting)
    {
        AvatarList[avatars.value] = setting;
        avatars.options[avatars.value].text = setting.AvatarName;
        ChangedAvatar(avatars.value);
        SaveSetting();
    }

    public void RemoveAvatar()
    {
        AvatarList.RemoveAt(avatars.value);
        avatars.options.RemoveAt(avatars.value);
        avatars.value = 0;
        SaveSetting();
    }

    private void SaveSetting()
    {
        var saveStr = "";
        foreach (var setting in AvatarList)
        {
            saveStr += setting.ToString();
        }

        PlayerPrefs.SetString("AvatarSettings", saveStr);
        PlayerPrefs.Save();
    }

    private async System.Threading.Tasks.Task onLoadVRMAsync(AvatarSetting setting)
    {
        videoCapture.PlayStop();

        var path = "";
        if (setting.AvatarType == 0)
        {
            path = setting.VRMFilePath;
        }
        else
        {
            path = setting.FBXFilePath;
        }

        if (path != "")
        {
            //ファイルをByte配列に読み込みます
            var bytes = File.ReadAllBytes(path);

            //VRMImporterContextがVRMを読み込む機能を提供します
            var context = new VRMImporterContext();

            // GLB形式でJSONを取得しParseします
            context.ParseGlb(bytes);

            // VRMのメタデータを取得
            var meta = context.ReadMeta(false); //引数をTrueに変えるとサムネイルも読み込みます

            //読み込めたかどうかログにモデル名を出力してみる
            Debug.LogFormat("meta: title:{0}", meta.Title);

            //非同期処理(Task)で読み込みます
            await context.LoadAsyncTask();

            //読込が完了するとcontext.RootにモデルのGameObjectが入っています
            var avatarObject = context.Root;
            avatarObject.name = setting.AvatarName;

            //モデルをワールド上に配置します
            avatarObject.transform.SetParent(transform.parent, false);

            SetVRMBounds(avatarObject.transform);

            //メッシュを表示します
            context.ShowMeshes();

            setting.Avatar = avatarObject.AddComponent<VNectModel>();
            setting.Avatar.ModelObject = avatarObject;
            setting.Avatar.SetNose(setting.FaceOriX, setting.FaceOriY, setting.FaceOriZ);
            setting.Avatar.SkeletonMaterial = skeletonMaterial;
            DiactivateAvatars();
            avatars.options.Add(new Dropdown.OptionData(setting.AvatarName));
            barracudaRunner.InitVNectModel(setting.Avatar);
            setting.Avatar.PoseUpdated = PoseUpdated;
        }
    }

    /// <summary>
    /// カメラでアップするとメッシュが消えてしまう場合の対応
    /// </summary>
    /// <param name="t"></param>
    private void SetVRMBounds(Transform t)
    {
        for (var i = 0; i < t.childCount; i++)
        {
            var child = t.GetChild(i);
            var smr = child.GetComponent<SkinnedMeshRenderer>();
            if (smr != null)
            {
                smr.localBounds = new Bounds(new Vector3(), smr.localBounds.size);
            }
            
            if(child.childCount > 0)
            {
                SetVRMBounds(child);
            }
        }
    }

    private void DiactivateAvatars()
    {
        foreach (var setting in AvatarList)
        {
            if (setting.Avatar != null)
            {
                setting.Avatar.Hide();
                setting.Avatar.gameObject.SetActive(false);
            }
        }
    }

    //private RecorderController m_RecorderController;
    private bool isRecording = false;

    public void onRecord()
    {

  /*      if (!isRecording)
        {
            var extensions = new[]
            {
                new ExtensionFilter( "Animation Files", "anim" ),
            };
            var path = StandaloneFileBrowser.SaveFilePanel("Save File", "", "My_Animation", extensions);

            if (path.Length == 0)
            {
                return;
            }
            string fileName = Path.GetFileName(path);
            
            var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
            if (m_RecorderController == null)
            {
                m_RecorderController = new RecorderController(controllerSettings);
            }

            // Animation
            var animationRecorder = ScriptableObject.CreateInstance<AnimationRecorderSettings>();
            //animationRecorder.name = "My Animation Recorder";
            animationRecorder.name = fileName;
            animationRecorder.Enabled = true;

            var setting = AvatarList[avatars.value];

            animationRecorder.AnimationInputSettings = new AnimationInputSettings
            {
                gameObject = setting.Avatar.gameObject,
                Recursive = true,
            };

            animationRecorder.AnimationInputSettings.AddComponentToRecord(typeof(Transform));

            //var animationOutputFolder = Path.Combine(Application.dataPath, "SampleRecordings");
            //animationRecorder.OutputFile = Path.Combine(animationOutputFolder, "anim_" + DefaultWildcard.GeneratePattern("GameObject") + "_v" + DefaultWildcard.Take);
            animationRecorder.OutputFile = path;

            controllerSettings.AddRecorderSettings(animationRecorder);

            controllerSettings.SetRecordModeToManual();
            controllerSettings.FrameRate = 30.0f;

            RecorderOptions.VerboseMode = false;
            m_RecorderController.StartRecording();
            isRecording = true;
            
        }
        else
        {
            m_RecorderController.StopRecording();
            isRecording = false;
        }
        */
    }

    public void onVRChat()
    {
        if (vrInputEmulator == null)
        {
            vrInputEmulator = new VRInputEmulatorWrapper();
            var res = vrInputEmulator.Connect();
            if (vrInputEmulator.Connect() > 0)
            {
                StartCoroutine(AddTrackedController());
            }
        }

        AvatarList[avatars.value].Avatar.StartCapture();
        VRChat = true;
        VRChatMotion = true;
    }


    IEnumerator AddTrackedController()
    {
        var cnt = 5;
        /**/
        AddVirtualController("OVRIE_Controller_L", true);
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1);
            if (virtualControllers[virtualControllers.Count - 1].GetOpenVRDeviceID() >= 0)
            {
                break;
            }
            cnt--;
        }
        if (cnt == 0)
        {
            yield break;
        }

        AddVirtualController("OVRIE_Controller_R", false);
        cnt = 5;
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1);
            if (virtualControllers[virtualControllers.Count - 1].GetOpenVRDeviceID() >= 0)
            {
                break;
            }
            cnt--;
        }
        if (cnt == 0)
        {
            yield break;
        }
        /*  */
        AddVirtualTracker("OVRIE_Tracker_1");
        cnt = 5;
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1);
            if (virtualTrackers[virtualTrackers.Count - 1].GetOpenVRDeviceID() >= 0)
            {
                break;
            }
            cnt--;
        }
        if (cnt == 0)
        {
            yield break;
        }

        AddVirtualTracker("OVRIE_Tracker_2");
        cnt = 5;
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1);
            if (virtualTrackers[virtualTrackers.Count - 1].GetOpenVRDeviceID() >= 0)
            {
                break;
            }
            cnt--;
        }
        if (cnt == 0)
        {
            yield break;
        }

        AddVirtualTracker("OVRIE_Tracker_3");
        cnt = 5;
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1);
            if (virtualTrackers[virtualTrackers.Count - 1].GetOpenVRDeviceID() >= 0)
            {
                break;
            }
            cnt--;
        }
        if (cnt == 0)
        {
            yield break;
        }
        /*
        AddVirtualTracker("OVRIE_Tracker_4");
        cnt = 5;
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1);
            if (virtualTrackers[virtualTrackers.Count - 1].GetOpenVRDeviceID() >= 0)
            {
                break;
            }
            cnt--;
        }
        if (cnt == 0)
        {
            yield break;
        }

        AddVirtualTracker("OVRIE_Tracker_5");
        cnt = 5;
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1);
            if (virtualTrackers[virtualTrackers.Count - 1].GetOpenVRDeviceID() >= 0)
            {
                break;
            }
            cnt--;
        }
        if (cnt == 0)
        {
            yield break;
        }
        
        AddVirtualTracker("OVRIE_Tracker_6");
        cnt = 5;
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1);
            if (virtualTrackers[virtualTrackers.Count - 1].GetOpenVRDeviceID() >= 0)
            {
                break;
            }
            cnt--;
        }
        if (cnt == 0)
        {
            yield break;
        }

        AddVirtualTracker("OVRIE_Tracker_7");
        cnt = 5;
        while (cnt > 0)
        {
            yield return new WaitForSeconds(1);
            if (virtualTrackers[virtualTrackers.Count - 1].GetOpenVRDeviceID() >= 0)
            {
                break;
            }
            cnt--;
        }
        if (cnt == 0)
        {
            yield break;
        }
        */
        if (virtualControllers.Count == 2)
        {
            virtualControllers[0].SetDevicePosition(-0.8f, 1.3f, 0f);
            virtualControllers[0].SetDeviceRotation(90f, 0f, 0f);
            virtualControllers[1].SetDevicePosition(0.8f, 1.3f, 0f);
            virtualControllers[1].SetDeviceRotation(-90f, 0f, 0f);
        }

        if (virtualTrackers.Count == 3)
        {
            virtualTrackers[0].SetDevicePosition(0f, 0.85f, 0f);
            virtualTrackers[1].SetDevicePosition(-0.12f, 0f, 0f);
            virtualTrackers[2].SetDevicePosition(0.12f, 0f, 0f);

            //virtualTrackers[3].SetDevicePosition(-0.14f, 0.4f, 0f);
            //virtualTrackers[4].SetDevicePosition(0.14f, 0.4f, 0f);
            //virtualTrackers[5].SetDevicePosition(-0.8f, 1.3f, 0f);
            //virtualTrackers[6].SetDevicePosition(0.8f, 1.3f, 0f);
        }
    }

    private VirtualController AddVirtualController(string serial, bool isL = true)
    {
        var ctrl = new VirtualController(vrInputEmulator);
        var idL = ctrl.AddTrackedController(serial);
        if (idL < 0)
        {
            idL = ctrl.GetDeviceID();
            ctrl.GetOpenVRDeviceID();
        }
        else
        {
            ctrl.SetControllerProperty(isL);
        }

        virtualControllers.Add(ctrl);

        return ctrl;
    }

    private VirtualController AddVirtualTracker(string serial)
    {
        var ctrl = new VirtualController(vrInputEmulator);
        var idL = ctrl.AddTrackedController(serial);
        if (idL < 0)
        {
            idL = ctrl.GetDeviceID();
            ctrl.GetOpenVRDeviceID();
        }
        else
        {
            ctrl.SetTrackerProperty();
        }

        virtualTrackers.Add(ctrl);

        return ctrl;
    }

    public void ShowMessage(string msg)
    {
        message.ShowMessage(msg);
    }

    public void PoseUpdated(VNectModel.JointPoint[] jointPoints)
    {
        if (VRChat && VRChatMotion)
        {
            if (virtualControllers.Count != 0)
            {
                virtualControllers[0].SetDevicePosition(jointPoints[PositionIndex.lHand.Int()].Transform.position);
                var lHand = jointPoints[PositionIndex.lHand.Int()].Transform.eulerAngles;
                virtualControllers[0].SetDeviceRotation(90f + lHand.y, lHand.z, lHand.x);

                virtualControllers[1].SetDevicePosition(jointPoints[PositionIndex.rHand.Int()].Transform.position);
                var rHand = jointPoints[PositionIndex.rHand.Int()].Transform.eulerAngles;
                virtualControllers[1].SetDeviceRotation(rHand.y - 90f, -rHand.z, rHand.x);
            }

            if (virtualTrackers.Count != 0)
            {
                virtualTrackers[0].SetDevicePosition(jointPoints[PositionIndex.abdomenUpper.Int()].Transform.position);
                var hip = jointPoints[PositionIndex.hip.Int()].Transform.eulerAngles;
                // virtualTrackers[0].SetDeviceRotation(180f + hip.y, hip.z, -hip.x);
                virtualTrackers[0].SetDeviceRotation(hip.y, hip.z, hip.x);

                virtualTrackers[1].SetDevicePosition(jointPoints[PositionIndex.lToe.Int()].Transform.position);
                var lf = jointPoints[PositionIndex.lToe.Int()].Transform.position - jointPoints[PositionIndex.lFoot.Int()].Transform.position;
                var lFoot = Quaternion.LookRotation(lf, Vector3.up).eulerAngles;
                virtualTrackers[1].SetDeviceRotation(lFoot.y, lFoot.x, lFoot.z);
                //var lFoot = jointPoints[PositionIndex.lFoot.Int()].Transform.eulerAngles;
                //virtualTrackers[1].SetDeviceRotation(180f + lFoot.y, lFoot.z, lFoot.x);

                virtualTrackers[2].SetDevicePosition(jointPoints[PositionIndex.rToe.Int()].Transform.position);
                var rf = jointPoints[PositionIndex.rToe.Int()].Transform.position - jointPoints[PositionIndex.rFoot.Int()].Transform.position;
                var rFoot = Quaternion.LookRotation(rf, Vector3.up).eulerAngles;
                virtualTrackers[2].SetDeviceRotation(rFoot.y, rFoot.x, rFoot.z);
                //var rFoot = jointPoints[PositionIndex.rFoot.Int()].Transform.eulerAngles;
                //virtualTrackers[2].SetDeviceRotation(180f + rFoot.y, rFoot.z, rFoot.x);
                /*
                virtualTrackers[3].SetDevicePosition(jointPoints[PositionIndex.lShin.Int()].Transform.position);
                //var rf = jointPoints[PositionIndex.rToe.Int()].Transform.position - jointPoints[PositionIndex.rFoot.Int()].Transform.position;
                //svar rFoot = Quaternion.LookRotation(rf, Vector3.up).eulerAngles;
                var lShin = jointPoints[PositionIndex.lShin.Int()].Transform.eulerAngles;
                virtualTrackers[3].SetDeviceRotation(180f + lShin.y, lShin.z, lShin.x);

                virtualTrackers[4].SetDevicePosition(jointPoints[PositionIndex.rShin.Int()].Transform.position);
                //var rf = jointPoints[PositionIndex.rToe.Int()].Transform.position - jointPoints[PositionIndex.rFoot.Int()].Transform.position;
                //svar rFoot = Quaternion.LookRotation(rf, Vector3.up).eulerAngles;
                var rShin = jointPoints[PositionIndex.rShin.Int()].Transform.eulerAngles;
                virtualTrackers[4].SetDeviceRotation(180f + rShin.y, rShin.z, rShin.x);
                */
                /*
                virtualTrackers[5].SetDevicePosition(jointPoints[PositionIndex.lForearmBend.Int()].Transform.position);
                //var rf = jointPoints[PositionIndex.rToe.Int()].Transform.position - jointPoints[PositionIndex.rFoot.Int()].Transform.position;
                //svar rFoot = Quaternion.LookRotation(rf, Vector3.up).eulerAngles;
                var lForearmBend = jointPoints[PositionIndex.lForearmBend.Int()].Transform.eulerAngles;
                virtualTrackers[5].SetDeviceRotation(180f + lForearmBend.y, lForearmBend.z, lForearmBend.x);

                virtualTrackers[6].SetDevicePosition(jointPoints[PositionIndex.rForearmBend.Int()].Transform.position);
                //var rf = jointPoints[PositionIndex.rToe.Int()].Transform.position - jointPoints[PositionIndex.rFoot.Int()].Transform.position;
                //svar rFoot = Quaternion.LookRotation(rf, Vector3.up).eulerAngles;
                var rForearmBend = jointPoints[PositionIndex.rForearmBend.Int()].Transform.eulerAngles;
                virtualTrackers[6].SetDeviceRotation(180f + rForearmBend.y, rForearmBend.z, rForearmBend.x);
                */
            }
        }
    }

    void OnDisable()
    {
        /*
        if (m_RecorderController != null)
        {
            m_RecorderController.StopRecording();
        }
        */
    }
}

public class AvatarSetting
{
    public int AvatarType;
    public string VRMFilePath;
    public string FBXFilePath;
    public string AvatarName;
    public float PosX;
    public float PosY;
    public float PosZ;
    public float DepthScale;
    public float Scale;
    public float FaceOriX;
    public float FaceOriY;
    public float FaceOriZ;
    public int SkeletonVisible;
    public float SkeletonPosX;
    public float SkeletonPosY;
    public float SkeletonPosZ;
    public float SkeletonScale;
    public VNectModel Avatar;

    public AvatarSetting()
    {
        AvatarType = 0;
        VRMFilePath = "";
        FBXFilePath = "";
        AvatarName = "New Avatar";
        PosX = 0f;
        PosY = 0f;
        PosZ = 0f;
        DepthScale = 0.8f;
        Scale = 1.0f;
        FaceOriX = 0.0f;
        FaceOriY = -0.001f;
        FaceOriZ = 0.01f;
        SkeletonVisible = 0;
        SkeletonPosX = -0.8f;
        SkeletonPosY = 0f;
        SkeletonPosZ = -0.5f;
        SkeletonScale = 0.005f;
        Avatar = null;
    }

    public AvatarSetting Clone()
    {
        return new AvatarSetting()
        {
            AvatarType = AvatarType,
            VRMFilePath = VRMFilePath,
            FBXFilePath = FBXFilePath,
            AvatarName = AvatarName,
            PosX = PosX,
            PosY = PosY,
            PosZ = PosZ,
            DepthScale = DepthScale,
            Scale = Scale,
            FaceOriX = FaceOriX,
            FaceOriY = FaceOriY,
            FaceOriZ = FaceOriZ,
            SkeletonVisible = SkeletonVisible,
            SkeletonPosX = SkeletonPosX,
            SkeletonPosY = SkeletonPosY,
            SkeletonPosZ = SkeletonPosZ,
            SkeletonScale = SkeletonScale,
            Avatar = Avatar,
        };
}

    public override string ToString()
    {
        var path = "";
        if(AvatarType < 0)
        {

        }
        else if(AvatarType == 0)
        {
            path = VRMFilePath;
        }
        else if (AvatarType == 1)
        {
            path = FBXFilePath;
        }
        return AvatarType.ToString() + "," + path + "," + AvatarName + "," + PosX.ToString() + "," + PosY.ToString() + "," + PosZ.ToString() + "," + DepthScale.ToString()
            + "," + Scale.ToString() + "," + FaceOriX.ToString() + "," + FaceOriY.ToString() + "," + FaceOriZ.ToString()
             + "," + SkeletonVisible.ToString() + "," + SkeletonPosX.ToString() + "," + SkeletonPosY.ToString() + "," + SkeletonPosZ.ToString() + "," + SkeletonScale.ToString() + ";";
    }
}


public class ConfigurationSetting
{
    public int ShowSource;
    public int ShowInput;
    public int SkipOnDrop;
    public int RepeatPlayback;
    public float SourceCutScale;
    public float SourceCutX;
    public float SourceCutY;
    public float LowPassFilter;
    public int TrainedModel;

    public int ShowBackground;
    public string BackgroundFile;
    public float BackgroundScale;
    public int BackgroundR;
    public int BackgroundG;
    public int BackgroundB;


    public ConfigurationSetting()
    {
        ShowSource = 1;
        ShowInput = 1;
        SkipOnDrop = 1;
        RepeatPlayback = 1;
        SourceCutScale = 1f;
        SourceCutX = 0f;
        SourceCutY = 0f;
        LowPassFilter = 0.1f;
        TrainedModel = 0;

        ShowBackground = 1;
        BackgroundFile = "";
        BackgroundScale = 1f;
        BackgroundR = 0;
        BackgroundG = 255;
        BackgroundB = 0;
    }

    public ConfigurationSetting Clone()
    {
        return new ConfigurationSetting()
        {
            ShowSource = ShowSource,
            ShowInput = ShowInput,
            SkipOnDrop = SkipOnDrop,
            RepeatPlayback = RepeatPlayback,
            SourceCutScale = SourceCutScale,
            SourceCutX = SourceCutX,
            SourceCutY = SourceCutY,
            LowPassFilter = LowPassFilter,
            TrainedModel = TrainedModel,
            ShowBackground = ShowBackground,
            BackgroundFile = BackgroundFile,
            BackgroundScale = BackgroundScale,
            BackgroundR = BackgroundR,
            BackgroundG = BackgroundG,
            BackgroundB = BackgroundB,
        };
    }

    public override string ToString()
    {
        return ShowSource.ToString() + "," + ShowInput.ToString() + "," + SkipOnDrop.ToString() + "," + RepeatPlayback.ToString()
            + "," + SourceCutScale.ToString() + "," + SourceCutX.ToString() + "," + SourceCutY.ToString() + "," + LowPassFilter.ToString()
            + "," + TrainedModel.ToString()
            + "," + ShowBackground.ToString() + "," + BackgroundFile + "," + BackgroundScale.ToString()
            + "," + BackgroundR.ToString() + "," + BackgroundG.ToString() + "," + BackgroundB.ToString();
    }
}