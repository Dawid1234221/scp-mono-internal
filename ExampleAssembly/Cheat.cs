/*namespace BanPlayer
{
    using UnityEngine;
    using UnityEngine.Networking;

    public class UnBan : NetworkBehaviour //UnityEngine.MonoBehaviour
    {
        //
    }
}*/
using GameConsole;
using System.Collections.Generic;
using Dissonance.Audio.Playback;
using Dissonance.Integrations.UNet_HLAPI;

namespace ExampleAssembly
{
    using UnityEngine;
    using UnityEngine.Networking;
    using UnityEngine.UI;
    //using UnityEngine.CoreModule;
    //using UnityEngine.Color;

    public class Cheat : NetworkBehaviour //UnityEngine.MonoBehaviour
    {
        private Texture2D pixel;

        [Header("Player Properties")]
        public Camera plyCam;


        public float viewRange;
        private Transform spectCam;
        private Text n_text;
        private float transparency;

        public LayerMask raycastMask;

        private static int kCmdCmdSetNick;
        public static int showItems = 0;
        public static bool showFriends = true;

        private CharacterClassManager ccm;
        private void Start()
        {
            this.ccm = base.GetComponent<CharacterClassManager>();
            pixel = new Texture2D(1, 1);
            pixel.SetPixel(0, 0, Color.black);
            pixel.wrapMode = TextureWrapMode.Repeat;
            pixel.Apply();
            showFriends = true;
        }

        private bool isAltDown = false;
        private bool isFDown = false;

        private bool isVDown = false;
        private bool isNDown = false;
        private bool isAimbot = false;
        private bool isNoclip = false;
        private bool isFly = false;
        private bool goMTF = false;
        private bool goIntercom = false;
        private bool MUTE = false;
        private Vector3 aimTarget = Vector3.zero;

        private int chaseTP = 0;
        private NetworkIdentity chaseTarget;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.LeftAlt))
            {
                if (!isAltDown)
                {
                    isAltDown = true;
                    //showItems = !showItems;
                    showItems++;
                    if (showItems > 4) showItems = 0;
                }
            }
            else if (isAltDown) isAltDown = false;

            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!isFDown)
                {
                    isFDown = true;
                    showFriends = !showFriends;
                }
            }
            else if (isFDown) isFDown = false;


            if (Input.GetKeyDown(KeyCode.Mouse3))
            {
                if (!isVDown)
                {
                    isVDown = true;
                    isAimbot = !isAimbot;
                }
            }
            else if (isVDown)
            {
                isVDown = false;
                aimTarget = Vector3.zero;
            }


            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                if (!isNDown)
                {
                    isNDown = true;
                    isNoclip = true;// !isNoclip;
                }
            }
            else if (isNDown && Input.GetKeyUp(KeyCode.Mouse2))
            {
                isNDown = false;
                isNoclip = false;
            }

            if (Input.GetKeyDown(KeyCode.X))
            {
                isFly = !isFly;
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                goMTF = !goMTF;
            }
            if (Input.GetKeyDown(KeyCode.K))
            {
                goIntercom = !goIntercom;
            }

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                chaseTP = 0;
            }
            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                chaseTP = 1;
            }
            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                chaseTP = 2;
            }
            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                chaseTP = 3;
            }
            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                chaseTP = 4;
            }
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                chaseTP = 5;
            }

            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                //.FindObjectOfType<Scp914_Controller>().CallCmdSetupPickup(FindObjectOfType<Pickup>(), 20, Camera.main.transform.position);
                //Scp914_Controller da = UnityEngine.Object.FindObjectOfType<Scp914_Controller>();
                //da.CallCmdSetupPickup(FindObjectOfType<Pickup>(), 20, Camera.main.transform.position);
                FindObjectOfType<Scp914_Controller>().CallCmdSetupPickup(FindObjectOfType<Pickup>().ToString(), 20, Camera.main.transform.position);
            }

            if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                FindObjectOfType<Scp914_Controller>().CallCmdSetupPickup(FindObjectOfType<Pickup>().ToString(), 22, Camera.main.transform.position);
            }

            if (Input.GetKeyDown(KeyCode.Keypad9))
            {
                PlayerManager.localPlayer.GetComponent<CharacterClassManager>().SetClassID(2);
            }
            if (Input.GetKeyDown(KeyCode.Keypad8))
            {
                PlayerManager.localPlayer.GetComponent<CharacterClassManager>().SetClassID(1);
            }
            if (Input.GetKeyDown(KeyCode.Keypad7))
            {
                PlayerManager.localPlayer.GetComponent<CharacterClassManager>().SetClassID(0);
            }

        }

        private NetworkIdentity FindChaseTarget(int team, GameObject localPlayer)
        {
            GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < array.Length; i++)
            {
                GameObject gameObject = array[i];
                if (gameObject.GetComponent<NetworkIdentity>())
                {
                    NicknameSync _component = gameObject.transform.GetComponent<NicknameSync>();
                    CharacterClassManager ClassManager = _component.GetComponent<CharacterClassManager>();
                    if (_component != null)
                    {
                        NetworkIdentity posComponent = gameObject.GetComponent<NetworkIdentity>();
                        Vector3 origin = posComponent.transform.position;
                        int curTeam = ClassManager.curClass; // Team.MTF
                        if (curTeam < 0 || localPlayer == gameObject) continue;
                        if (gameObject.GetComponent<PlayerStats>().health <= 0)
                        {
                            continue;
                        }

                        int team_NUM = getTeamIDById(curTeam);

                        if (team == team_NUM)
                        {
                            return posComponent;
                        }
                    }
                }
            }
            return null;
        }

        private Vector3 calculateWorldPosition(Vector3 position, Camera camera)
        {
            //if the point is behind the camera then project it onto the camera plane
            Vector3 camNormal = camera.transform.forward;
            Vector3 vectorFromCam = position - camera.transform.position;
            float camNormDot = Vector3.Dot(camNormal, vectorFromCam.normalized);
            if (camNormDot <= 0f)
            {
                return Vector3.zero;
                //we are beind the camera, project the position on the camera plane
                float camDot = Vector3.Dot(camNormal, vectorFromCam);
                Vector3 proj = (camNormal * camDot * 1.01f);   //small epsilon to keep the position infront of the camera
                position = camera.transform.position + (vectorFromCam - proj);
            }

            return position;
        }

        private void OnGUI()
        {
            GUI.Label(new UnityEngine.Rect(10, 30, 500, 20), "SCP Hack v0.7");
            GUI.Label(new UnityEngine.Rect(10, 50, 500, 30), "Show Friends: " + (showFriends ? "ON" : "OFF"));
            GUI.Label(new UnityEngine.Rect(10, 70, 500, 40), "Show Items: " + (showItems == 0 ? "OFF" : (showItems == 1 ? "Cards" : (showItems == 2 ? "Ammo" : (showItems == 3 ? "Weapons" : "ALL")))));
            GUI.Label(new UnityEngine.Rect(10, 90, 500, 50), "Noclip: " + (isNoclip ? "ON" : "OFF"));
            // UnityEngine.GUI.Label(new UnityEngine.Rect(500, 30, 500, 50), "MUTE: " + (MUTE ? "ON" : "OFF"));
            GUI.Label(new UnityEngine.Rect(500, 30, 500, 50), "Menu: F5");

            Update();
            /*
             * SCP,
             * MTF,
             * CHI,
             * RSC,
             * CDP,
             * RIP,
             * TUT
             * */
            Camera mainCam = Camera.main;
            GameObject localPlayer = FindLocalPlayer();
            BanPlayer test = localPlayer.GetComponent<BanPlayer>();//.hardwareID;
            if (test != null)
            {
                //test.hardwareID = "this is not working, use memory write";
                UnityEngine.GUI.Label(new UnityEngine.Rect(10, 110, 500, 50), "HWID: " + (test.hardwareID));
                UnityEngine.GUI.Label(new UnityEngine.Rect(10, 130, 500, 50), "My health: " + (localPlayer.GetComponent<PlayerStats>().health.ToString()));
            }
            //PlayerStats myStats = localPlayer.GetComponent<PlayerStats>();
            //myStats.SetHPAmount(1000);
            /* else
            {
                try
                {
                    NetworkConnection con = UnityEngine.Object.FindObjectOfType<NetworkConnection>();
                    //con.PendingPlayer
                    if (con != null)
                    {
                        //UnityEngine.GUI.Label(new UnityEngine.Rect(10, 110, 500, 50), "Connection ADDR: " + (con.));
                    }
                    foreach (PlayerController current in con.playerControllers)
                    {
                        if (current.gameObject.tag == "Player")
                        {
                            GameObject result = current.gameObject;
                            test = result.GetComponent<BanPlayer>();
                            test.hardwareID = "aaa";
                            UnityEngine.GUI.Label(new UnityEngine.Rect(10, 110, 500, 50), "HWID (offline): " + (test.hardwareID));
                        }
                    }
                }
                catch
                {
                    //GameObject result = null;
                }
                //UnityEngine.GUI.Label(new UnityEngine.Rect(10, 110, 500, 50), "HWID (offline): " + (BanPlayer.NetworkhardwareID));
            }*/

            GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
            updateMuteMenu(array);
            CharacterClassManager myClassManager = localPlayer.GetComponent<CharacterClassManager>();
            WeaponManager myWeapon = localPlayer.GetComponent<WeaponManager>();
            if (myWeapon != null)
            {
                myWeapon.overallRecoilFactor = 0f;
                // myWeapon.weapons reloadingTime = 0f;

                if (Input.GetKeyDown(KeyCode.F7))
                    for (int i = 0; i < myWeapon.weapons.Length; i++)
                    {
                        myWeapon.weapons[i].reloadingTime = 0f;
                        myWeapon.weapons[i].bodyDamageMultipiler = 99.9f;
                        myWeapon.weapons[i].headDamageMultipiler = 99.9f;
                        myWeapon.weapons[i].legDamageMultipiler = 99.9f;
                        myWeapon.weapons[i].fireRate = 50f;
                        myWeapon.weapons[i].useFullauto = true;
                    }
                if (Input.GetKeyDown(KeyCode.F8))
                    for (int i = 0; i < myWeapon.weapons.Length; i++)
                    {
                        myWeapon.weapons[i].reloadingTime = 0.5f;
                        myWeapon.weapons[i].bodyDamageMultipiler = 99.9f;
                        myWeapon.weapons[i].headDamageMultipiler = 99.9f;
                        myWeapon.weapons[i].legDamageMultipiler = 99.9f;
                        myWeapon.weapons[i].fireRate = 15f;
                        myWeapon.weapons[i].useFullauto = true;
                    }
                for (int i = 0; i < myWeapon.weapons.Length; i++)
                    myWeapon.weapons[i].useFullauto = true;
                //myWeapon.curItem.durability = 5000f;
            }

            // GetComponentInParent<Radio>(); = for mute

            if (isNoclip)// || isFly)
            {
                localPlayer.transform.position += 0.5f * mainCam.transform.forward * (Input.GetAxis("Vertical") == 0 ? 1.0f : Input.GetAxis("Vertical")) + mainCam.transform.right * Input.GetAxis("Horizontal");
            }
            if (isFly)
            {
                array = GameObject.FindGameObjectsWithTag("PD_EXIT");
                localPlayer.transform.position = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
            }
            if (goMTF)
            {
                array = GameObject.FindGameObjectsWithTag("SP_MTF");
                localPlayer.transform.position = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
            }

            if (goIntercom)
            {
                // localPlayer.transform.position = GameObject.Find("IntercomSpeakingZone").transform.position;
                //goIntercom = false;
                Vector3 pos = GameObject.Find("IntercomSpeakingZone").transform.position;
                Vector3 newpos = new Vector3(
                    pos.x,
                    pos.y + 1f,
                    pos.z
                );
                if (UnityEngine.Random.Range(0, 1) > 0)
                {
                    localPlayer.transform.position = pos;
                }
                else
                {
                    localPlayer.transform.position = newpos;
                }
                //public Vector3[] arrayOfInts;
                //localPlayer.transform.position = array[UnityEngine.Random.Range(0, array.Length)].transform.position;
            }

            if (chaseTP > 0) {
                chaseTarget = FindChaseTarget(chaseTP, localPlayer);  //chaseTarget = posComponent;
                if (chaseTarget != null)
                {
                    Vector3 posed = chaseTarget.transform.position;
                    //posed.y += 2f; = v0.7 change
                    localPlayer.transform.position = posed;
                }
            }

            int myTeam = myClassManager.curClass;
            float aim_dist_min = 99999f;
            float aim_dist = aim_dist_min;
            string nickname = "Cookie☆";
            array = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < array.Length; i++)
            {
                GameObject gameObject = array[i];
                if (gameObject.GetComponent<NetworkIdentity>())
                {
                    NicknameSync _component = gameObject.transform.GetComponent<NicknameSync>();
                    //component.CallCmdSetNick("");
                    if (UnityEngine.Random.Range(0, 3) == 1) {
                        nickname = _component.myNick;
                    }
                    CharacterClassManager ClassManager = _component.GetComponent<CharacterClassManager>();
                    if (_component != null)
                    {
                        NetworkIdentity posComponent = gameObject.GetComponent<NetworkIdentity>();
                        Vector3 origin = posComponent.transform.position;

                        /*GameObject voice = GameObject.Find("Player " + gameObject.GetComponent<HlapiPlayer>().PlayerId + " voice comms");
                        if (voice != null && MUTE)
                        {
                            //voice.GetComponent<UnityEngine.AudioSource>().volume = 0f;
                            voice.GetComponent<UnityEngine.AudioSource>().mute = true;
                            UnityEngine.GUI.Label(new UnityEngine.Rect(500, 30 + 10 * i, 900, 20), nickname + " voice: " + voice.GetComponent<UnityEngine.AudioSource>().volume.ToString());
                        }
                        UnityEngine.GUI.Label(new UnityEngine.Rect(500, 30 + 10 * i, 900, 20), nickname + " voice: OFF");*/
                        //Radio radio = GetComponentInParent<Radio>();
                        //AudioModule AudioSource ads = radio.mySource;
                        // UnityEngine.Audio ad = radio.mySource;
                        //if (radio.mySource != null) {
                        //    VoicePlayback = radio.mySource.GetComponent<VoicePlayback>(); //.mySource.GetComponent<VoicePlayback>();
                        //}

                        //UnityEngine.GUI.Label(new UnityEngine.Rect(10, 30 + 10 * i, 500, 20), component.myNick + origin.ToString());

                        int curTeam = ClassManager.curClass; // Team.MTF
                        if (curTeam < 0 || localPlayer == gameObject) continue;
                        float tempDistance = Vector3.Distance(mainCam.transform.position, origin);
                        //if (tempDistance >= 350.1f) continue;
                        //int Distance = (int)tempDistance;
                        // Get distance ratio to player
                        /*float distance = 1.0f / Vector3.Distance(origin, mainCam.transform.position);
                        Vector3 pos = mainCam.WorldToScreenPoint(origin);
                        // Display ESP box around player (using "sharpcorners" GUIStyle)
                        Rect rect = new Rect(pos.x - (250f * distance), pos.y - (1300f * distance), 500f * distance, 1300f * distance);*/

                        Vector3 Pos = new Vector3(mainCam.WorldToScreenPoint(origin).x, mainCam.WorldToScreenPoint(origin).y, mainCam.WorldToScreenPoint(origin).z);
                        //Vector3 Pos = calculateWorldPosition(origin, mainCam);
                        //if (Pos == Vector3.zero) continue;
                        if (mainCam.WorldToScreenPoint(origin).z < 0.99f) continue;
                        //GUI.color = Color.white;//Color.yellow;
                        //if (myTeam == curTeam) GUI.color = Color.green;
                        //else GUI.color = Color.red;

                        GUI.color = getColorById(curTeam);

                        if (!showFriends && getTeamById(myTeam) == getTeamById(curTeam))
                        {
                            continue;
                        }

                        // if relative team BLUE
                        string title = GetTeamNameById(curTeam);
                        GUI.Label(new Rect((Pos.x - 50F), (Screen.height - Pos.y), 100F, 50F), ("" + title/*component.myNick + " #" + curTeam.ToString()*/ + " [" + tempDistance.ToString() + "]"));
                        GUI.color = (getTeamById(myTeam) != getTeamById(curTeam)) ? Color.red : Color.green;
                        GUI.Label(new Rect((Pos.x - 50F), (Screen.height - Pos.y - 60F), 100F, 50F), (_component.myNick + " " + gameObject.GetComponent<PlayerStats>().health.ToString()));
                        /*if (Input.GetButtonDown("LeftAlt"))
                        {
                            GUI.Label(new Rect((Pos.x - 50F), (Screen.height - Pos.y - 40F), 100F, 50F), ("DEBUG: id = " + curTeam.ToString()));
                        }*/

                        aim_dist = Mathf.Abs((Pos.x - Screen.width) + (Pos.y - Screen.width));
                        if (isAimbot && aimTarget == Vector3.zero && aim_dist < aim_dist_min)
                        {
                            // find target
                            aim_dist_min = aim_dist;
                            aimTarget = Pos;
                        }

                        // BOX
                        /*Pos.y = Screen.height - Pos.y; // Fix reversed y-axis
                        pixel.SetPixel(0, 0, Color.red);
                        pixel.Apply();
                        Rect rect = new Rect(Pos.x - (250f * tempDistance), Pos.y - (1300f * tempDistance), 500f * tempDistance, 1300f * tempDistance);
                        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, rect.width, 2), pixel);
                        GUI.DrawTexture(new Rect(rect.xMin, rect.yMin, 2, rect.height), pixel);
                        GUI.DrawTexture(new Rect(rect.xMax - 2, rect.yMin, 2, rect.height), pixel);
                        GUI.DrawTexture(new Rect(rect.xMin, rect.yMax - 2, rect.width, 2), pixel);*/
                    }
                }
            }

            //NicknameSync mcomponent = gameObject.transform.GetComponent<NicknameSync>();
            // component.CallCmdSetNick("");
            //mcomponent.CallCmdSetNick(nickname);

            if (isAimbot && aimTarget != Vector3.zero)
            {
                myWeapon.transform.LookAt(aimTarget);

                for (int i = 0; i < myWeapon.weapons.Length; i++)
                {
                    myWeapon.weapons[i].weaponCamera.transform.LookAt(aimTarget);

                    //myWeapon.weapons[i].gunAim;
                    //WeaponManager.Weapon wcam = weapon.weaponCamera;
                    //weapon.weaponCamera.transform.LookAt(aimTarget);
                    //mainCam.transform.LookAt(aimTarget);
                }
            }


            // RIGHT ESCAPE FROM POCKET
            array = GameObject.FindGameObjectsWithTag("PD_EXIT");
            for (int i = 0; i < array.Length; i++)
            {
                GameObject gameObject = array[i];
                if (gameObject.GetComponent<NetworkIdentity>())
                {
                    if (gameObject != null)
                    {
                        NetworkIdentity posComponent = gameObject.GetComponent<NetworkIdentity>();
                        Vector3 origin = posComponent.transform.position;
                        float tempDistance = Vector3.Distance(mainCam.transform.position, origin);
                        if (tempDistance >= 450.1f) continue;
                        int itemId = gameObject.GetComponent<Pickup>().id;
                        int distance = (int)tempDistance;
                        GUI.color = Color.white;
                        Vector3 Pos = new Vector3(mainCam.WorldToScreenPoint(origin).x, mainCam.WorldToScreenPoint(origin).y, mainCam.WorldToScreenPoint(origin).z);
                        GUI.Label(new Rect((Pos.x - 50F), (Screen.height - Pos.y), 100F, 50F), ("" + gameObject.GetType().ToString() + " #" + itemId + " [" + distance.ToString() + "]"));
                    }
                }
            }
            //array = GameObject.FindGameObjectsWithTag("PickupPD_EXIT");
            //GameObject gameObjectTeleport = PocketDimensionTeleport;
            //PocketDimensionTeleport portal = (PocketDimensionTeleport)FindObjectOfType(typeof(PocketDimensionTeleport));
            /*Object[] array2 = UnityEngine.Object.FindObjectsOfType(typeof(PocketDimensionTeleport));
            // array = (GameObject)GameObject.FindObjectsOfType<PocketDimensionTeleport>();
            for (int i = 0; i < array2.Length; i++)
            {
                PocketDimensionTeleport teleport = (PocketDimensionTeleport)array2[i];
                if (teleport.type == PocketDimensionTeleport.PDTeleportType.Exit)
                {
                    origin = teleport.
                }
            }*/

            /*if (Input.GetButtonDown("LeftAlt"))
            {
                array = GameObject.FindGameObjectsWithTag("Pickup");
                for (int i = 0; i < array.Length; i++)
                {
                    GameObject gameObject = array[i];
                    if (gameObject.GetComponent<NetworkIdentity>())
                    {
                        if (gameObject != null)
                        {
                            NetworkIdentity posComponent = gameObject.GetComponent<NetworkIdentity>();
                            Vector3 origin = posComponent.transform.position;
                            float tempDistance = Vector3.Distance(mainCam.transform.position, origin);
                            if (tempDistance >= 250.1f) continue;
                            GUI.color = Color.white;
                            Vector3 Pos = new Vector3(mainCam.WorldToScreenPoint(origin).x, mainCam.WorldToScreenPoint(origin).y, mainCam.WorldToScreenPoint(origin).z);
                            GUI.Label(new Rect((Pos.x - 50F), (Screen.height - Pos.y), 100F, 50F), ("" + gameObject.GetComponent<Pickup>().id + " [" + tempDistance.ToString() + "]"));
                        }
                    }
                }
            }*/
            if (showItems != 0)
            {

                array = GameObject.FindGameObjectsWithTag("Pickup");
                for (int i = 0; i < array.Length; i++)
                {
                    GameObject gameObject = array[i];
                    if (gameObject.GetComponent<NetworkIdentity>())
                    {
                        if (gameObject != null)
                        {
                            /*
                                0 - off
                                1 - cards
                                2 - ammo
                                3 - guns
                                4 - all
                            */

                            NetworkIdentity posComponent = gameObject.GetComponent<NetworkIdentity>();
                            Vector3 origin = posComponent.transform.position;

                            float tempDistance = Vector3.Distance(mainCam.transform.position, origin);
                            if (tempDistance >= 350.1f && showItems != 4)
                                continue;

                            int itemId = gameObject.GetComponent<Pickup>().id;


                            name = "undefined";

                            if (showItems == 1 || showItems == 4)
                            {
                                switch (itemId)
                                {
                                    case 0:
                                        name = "SCP1";
                                        break;
                                    case 1:
                                        name = "SCP2";
                                        break;
                                    case 2:
                                        name = "SCP3";
                                        break;
                                    case 3:
                                        name = "SCP4";
                                        break;
                                    case 4:
                                        name = "Security1";
                                        break;
                                    case 5:
                                        name = "Security2";
                                        break;
                                    case 6:
                                        name = "Security3";
                                        break;
                                    case 7:
                                        name = "Security4";
                                        break;
                                    case 8:
                                        name = "Administration1";
                                        break;
                                    case 9:
                                        name = "Administration2";
                                        break;
                                    case 10:
                                        name = "Administration3";
                                        break;
                                    case 11:
                                        name = "Administration4";
                                        break;
                                    case 14:
                                        name = "Medkit";
                                        break;

                                }
                            }

                            if (showItems == 2 || showItems == 4)
                            {
                                if (itemId == 22)
                                    name = "sniper ammo";
                                else if (itemId == 28)
                                    name = "m249 ammo";
                                else if (itemId == 29)
                                    name = "Lpist ammo";
                                else if (itemId == 14)
                                    name = "Medkit";
                            }

                            if (showItems == 3 || showItems == 4)
                            {
                                if (itemId == 12)
                                    name = "Radio";
                                else if (itemId == 13)
                                    name = "Pistol";
                                else if (itemId == 20)
                                    name = "Sniper";
                                else if (itemId == 24)
                                    name = "M249";
                                else if (itemId == 25)
                                    name = "EMP grenade";
                                else if (itemId == 26)
                                    name = "Smoke grenade";
                                else if (itemId == 27)
                                    name = "Disarmer";
                            }

                            if (name == "undefined")
                                if (showItems == 4)
                                    name = "undefined#" + itemId;
                                else
                                    continue;

                            int distance = (int)tempDistance;
                            GUI.color = Color.white;
                            Vector3 Pos = new Vector3(mainCam.WorldToScreenPoint(origin).x, mainCam.WorldToScreenPoint(origin).y, mainCam.WorldToScreenPoint(origin).z);
                            GUI.Label(new Rect((Pos.x - 50F), (Screen.height - Pos.y), 100F, 50F), ("" + name + " [" + distance.ToString() + "]"));
                        }
                    }
                }
            }

        }

        private string GetTeamNameById(int tid)
        {
            switch (tid)
            {
                case -1:
                    return "Server manager";
                case 0:
                    return "SCP-173 (Statue)";
                case 1:
                    return "Class D";
                case 3:
                    return "SCP-106 (Old)";
                case 4:
                    return "Chaos Elite";
                case 5:
                    return "SCP-049 (Doc)";
                case 6:
                    return "Scientist";
                case 8:
                    return "MTF";
                case 10:
                    return "SCP-049-2 (Zombie)";
                case 11:
                    return "Chaos Soldier";
                case 12:
                    return "Chaos Commander";
                case 13:
                    return "Chaos (L)";
                default:
                    return "undefined (#" + tid.ToString() + ")";
            }
        }

        private int getTeamIDById(int curTeam)
        {
            if (curTeam == 0 || curTeam == 3 || curTeam == 5 || curTeam == 10) return 3;
            if (curTeam == 1) return 1;
            if (curTeam == 8) return 5;
            if (curTeam == 6) return 2;
            if (curTeam == 4 || curTeam == 11 || curTeam == 12 || curTeam == 13) return 4;
            return 0;
        }

        private Color getColorById(int curTeam)
        {
            if (curTeam == 0 || curTeam == 3 || curTeam == 5 || curTeam == 10) return Color.red;
            if (curTeam == 1) return new Color(1.0F, 0.7F, 0.0F, 1.0F);
            if (curTeam == 8) return Color.green;
            if (curTeam == 6) return Color.white;
            if (curTeam == 4 || curTeam == 11 || curTeam == 12 || curTeam == 13) return Color.blue;
            return new Color(1.0F, 0.7F, 0.3F, 0.5F);
        }

        private int getTeamById(int iId)
        {
            switch (iId)
            {
                case 1:
                case 8:
                    return 0;  // D-class, GREEN
                case 4:
                case 6:
                case 11:
                case 12:
                case 13:
                    return 1;  // SIENTIST, BLUE
                default: return 2; // SCP
            }
        }

        private GameObject FindLocalPlayer()
        {
            GameObject[] array = GameObject.FindGameObjectsWithTag("Player");
            for (int i = 0; i < array.Length; i++)
            {
                GameObject gameObject = array[i];
                if (gameObject.GetComponent<NetworkIdentity>().isLocalPlayer)
                {
                    return gameObject;
                }
            }
            return null;
        }

        /*private void Update()
        {
            if (base.isLocalPlayer)
            {
                bool flag = false;
                RaycastHit raycastHit;
                if (Physics.Raycast(new Ray(this.spectCam.position, this.spectCam.forward), out raycastHit, this.viewRange, this.raycastMask))
                {
                    NicknameSync component = raycastHit.transform.GetComponent<NicknameSync>();
                    if (component != null && !component.isLocalPlayer)
                    {
                        CharacterClassManager component2 = component.GetComponent<CharacterClassManager>();
                        CharacterClassManager component3 = base.GetComponent<CharacterClassManager>();
                        flag = true;
                        //this.n_text.color = component2.klasy[component2.curClass].classColor;
                        this.n_text.text = string.Empty;
                        Text expr_AD = this.n_text;
                        expr_AD.text += component.myNick;
                        Text expr_C9 = this.n_text;
                        expr_C9.text = expr_C9.text + "\n" + ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? component2.klasy[component2.curClass].fullName : component2.klasy[component2.curClass].fullName_pl);
                        try
                        {
                            if (component2.klasy[component2.curClass].team == Team.MTF && component3.klasy[component3.curClass].team == Team.MTF)
                            {
                                int num = 0;
                                int num2 = 0;
                                if (component2.curClass == 4 || component2.curClass == 11)
                                {
                                    num2 = 200;
                                }
                                else if (component2.curClass == 13)
                                {
                                    num2 = 100;
                                }
                                else if (component2.curClass == 12)
                                {
                                    num2 = 300;
                                }
                                if (component3.curClass == 4 || component3.curClass == 11)
                                {
                                    num = 200;
                                }
                                else if (component3.curClass == 13)
                                {
                                    num = 100;
                                }
                                else if (component3.curClass == 12)
                                {
                                    num = 300;
                                }
                                Text expr_205 = this.n_text;
                                expr_205.text = expr_205.text + " (" + GameObject.Find("Host").GetComponent<NineTailedFoxUnits>().GetNameById(component2.ntfUnit) + ")\n\n<b>";
                                num -= component3.ntfUnit;
                                num2 -= component2.ntfUnit;
                                if (num > num2)
                                {
                                    Text expr_25F = this.n_text;
                                    expr_25F.text += ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? "YOU CAN GIVE ORDERS" : "MOŻESZ DAWAĆ ROZKAZY");
                                }
                                else if (num2 > num)
                                {
                                    Text expr_2B0 = this.n_text;
                                    expr_2B0.text += ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? "FOLLOW ORDERS" : "WYKONUJ ROZKAZY");
                                }
                                else if (num2 == num)
                                {
                                    Text expr_301 = this.n_text;
                                    expr_301.text += ((!(PlayerPrefs.GetString("langver", "en") == "pl")) ? "THE SAME PERMISSION LEVEL" : "RÓWNY POZIOM UPRAWNIEŃ");
                                }
                                Text expr_344 = this.n_text;
                                expr_344.text += "</b>";
                            }
                        }
                        catch
                        {
                            MonoBehaviour.print("Error");
                        }
                    }
                }
                this.transparency += Time.deltaTime * (float)((!flag) ? -3 : 3);
                if (flag)
                {
                    float max = (this.viewRange - Vector3.Distance(base.transform.position, raycastHit.point)) / this.viewRange;
                    this.transparency = Mathf.Clamp(this.transparency, 0f, max);
                }
                this.transparency = Mathf.Clamp01(this.transparency);
                CanvasRenderer component4 = this.n_text.GetComponent<CanvasRenderer>();
                component4.SetAlpha(this.transparency);
            }
        }*/

        private bool isMenuOpen = false;
        private bool isKeyMenuPressed = false;
        private KeyCode keyMenuOpen = KeyCode.F5;
        //private string[] playerNames = { "TestName1", "TestName2", "TestName8", "TestName22", "TestName66", "TestName12", "TestName2", "GoodHash", "GoodHashNash", "hckd", "Player", "NoobIvan" };
        //private bool[] isPlayerMuted = new bool[322/*maxPlayers?*/];
        private int menuPage = 0;
        private bool onNextPage = false;
        private bool onBackPage = false;
        private bool onExit = false;
        private bool[] isKeyDown = new bool[10];

        private enum MenuType
        {
            MAIN,
            MUTE,
            TEST
        };
        private MenuType menuType = MenuType.MAIN;

        private void updateMuteMenu(GameObject[] playerList)
        {
            GUI.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

            // Key OPEN or CLOSE MENU
            if (Input.GetKeyDown(keyMenuOpen))
            {
                if (!isKeyMenuPressed)
                {
                    isKeyMenuPressed = true;
                    isMenuOpen = !isMenuOpen;

                }
            }
            else if (isKeyMenuPressed && Input.GetKeyUp(keyMenuOpen))
                isKeyMenuPressed = false;

            if (!isMenuOpen)
                return;

            // Key EXIT
            if (!isKeyDown[0] && menuType == MenuType.MAIN)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0))
                {
                    isKeyDown[0] = true;
                    isMenuOpen = false;
                }
            }
            else if (isKeyDown[0] && Input.GetKeyUp(KeyCode.Alpha0))
                isKeyDown[0] = false;

            if (!isMenuOpen)
                return;

            // Key BACK
            if (!isKeyDown[8])
            {
                if (Input.GetKeyDown(KeyCode.Alpha8))
                {
                    isKeyDown[8] = true;
                    if (menuPage > 0) menuPage--;
                }
            }
            else if (isKeyDown[8] && Input.GetKeyUp(KeyCode.Alpha8))
                isKeyDown[8] = false;

            int playersCount = playerList.Length;

            // Key NEXT
            if (!isKeyDown[9])
            {
                if (Input.GetKeyDown(KeyCode.Alpha9))
                {
                    isKeyDown[9] = true;
                    if (menuPage < playersCount / 7)
                        menuPage++;
                }
            }
            else if (isKeyDown[9] && Input.GetKeyUp(KeyCode.Alpha9))
                isKeyDown[9] = false;

            // Show menu

            // If current menu is 'MuteMenu' - show players
            if (menuType == MenuType.MUTE)
            {
                //int pageLimiter = menuPage * 7;
                int perPage = 7;
                int currentPage = menuPage;

                //if (playersCount > perPage) playersCount = perPage;
                //if(playersCount/perPage>)
                int maxPages = playersCount / 7;

                if (playersCount / 7 < currentPage)
                {
                    currentPage = playersCount / 7;
                }

                // 'MENU TITLE'
                GUI.Label(new Rect(10, 120, 500, 50), "Mute menu; Page:" + menuPage + " of " + maxPages);

                int keyNum = 0;
                // 'ADD NAMES'
                /*for (int i = 0; i < array.Length; i++)
                {
                    GameObject gameObject = array[i];
                    if (gameObject.GetComponent<NetworkIdentity>())
                    {
                        NicknameSync _component = gameObject.transform.GetComponent<NicknameSync>();
                        CharacterClassManager ClassManager = _component.GetComponent<CharacterClassManager>();
                        if (_component != null)
                        {
                            NetworkIdentity netcomponent = gameObject.GetComponent<NetworkIdentity>();
                            if (netcomponent.isLocalPlayer) continue;
                            GameObject voice = GameObject.Find("Player " + gameObject.GetComponent<HlapiPlayer>().PlayerId + " voice comms");
                            if (voice != null && MUTE)
                            {
                                voice.GetComponent<UnityEngine.AudioSource>().mute = true;
                                // UnityEngine.GUI.Label(new UnityEngine.Rect(500, 30 + 10 * i, 900, 20), nickname + " voice: " + voice.GetComponent<UnityEngine.AudioSource>().volume.ToString());
                            }
                            //UnityEngine.GUI.Label(new UnityEngine.Rect(500, 30 + 10 * i, 900, 20), nickname + " voice: OFF");
                        }
                    }
                }*/
                for (int i = perPage * currentPage; i < perPage * (currentPage + 1); i++)
                {
                    if (i >= playersCount) continue;

                    GameObject gameObject = playerList[i];
                    if (gameObject.GetComponent<NetworkIdentity>())
                    {
                        NicknameSync _component = gameObject.transform.GetComponent<NicknameSync>();
                        CharacterClassManager ClassManager = _component.GetComponent<CharacterClassManager>();
                        if (_component != null)
                        {
                            NetworkIdentity netcomponent = gameObject.GetComponent<NetworkIdentity>();
                            if (netcomponent.isLocalPlayer) continue;
                            GameObject voice = GameObject.Find("Player " + gameObject.GetComponent<HlapiPlayer>().PlayerId + " voice comms");
                            bool muted = false;
                            if (voice != null)
                            {
                                if (voice.GetComponent<UnityEngine.AudioSource>().mute) muted = true;
                                // voice.GetComponent<UnityEngine.AudioSource>().mute = true;
                            }
                            string nickname = _component.myNick;

                            string menuText = (keyNum + 1) + " - [id:" + i + menuPage * 7 + "] " + nickname + "; muted:" + (muted ? "ON" : "OFF");
                            GUI.Label(new Rect(10, 140 + 20 * keyNum, 500, 50), menuText);

                            if (!isKeyDown[keyNum])
                            {
                                if (Input.GetKeyDown(KeyCode.Alpha1 + (keyNum)))
                                {
                                    isKeyDown[keyNum] = true;
                                    // remutePlayer(i);
                                    if (muted) voice.GetComponent<UnityEngine.AudioSource>().mute = false;
                                    else voice.GetComponent<UnityEngine.AudioSource>().mute = true;
                                    break;
                                }
                            }
                            else if (isKeyDown[keyNum] && Input.GetKeyUp(KeyCode.Alpha1 + (keyNum)))
                            {
                                isKeyDown[keyNum] = false;
                            }
                            keyNum++;
                        }
                    }
                }


                // 'MENU POST ITEMS'
                GUI.Label(new Rect(10, 140 + 20 * 8, 500, 50), "8 - Back Page");
                GUI.Label(new Rect(10, 140 + 20 * (8 + 1), 500, 50), "9 - Next Page");
                GUI.Label(new Rect(10, 140 + 20 * (8 + 2), 500, 50), "0 - Back to Main");

                if (!isKeyDown[0])
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0))
                    {
                        isKeyDown[0] = true;
                        menuType = MenuType.MAIN;
                    }
                }
                else if (isKeyDown[0] && Input.GetKeyUp(KeyCode.Alpha0))
                    isKeyDown[0] = false;

            }
            else
            if (menuType == MenuType.MAIN)
            {
                // 'MENU TITLE'
                GUI.Label(new Rect(10, 120, 500, 50), "Mute menu; Page:" + menuPage);

                // Item1 - open Mute Menu
                GUI.Label(new UnityEngine.Rect(10, 140, 500, 50), "1 - Mute Menu");
                if (!isKeyDown[1])
                {
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        isKeyDown[1] = true;
                        menuType = MenuType.MUTE;
                        menuPage = 0;
                    }
                }
                else if (isKeyDown[1] && Input.GetKeyUp(KeyCode.Alpha1))
                    isKeyDown[1] = false;

                // Item2 - open ESP Menu
                GUI.color = new Color(1.0f, 1.0f, 1.0f, 0.5f);
                GUI.Label(new UnityEngine.Rect(10, 160, 500, 50), "2 - ESP [NOT WORKING]");
                // Coming soon....

                // Item0 - exit key
                GUI.color = new Color(1.0f, 1.0f, 1.0f, 1f);
                GUI.Label(new UnityEngine.Rect(10, 180, 500, 50), "0 - Exit");

                if (!isKeyDown[0])
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0))
                    {
                        isKeyDown[0] = false;
                        isMenuOpen = false;
                    }
                }
                else if (isKeyDown[0] && Input.GetKeyUp(KeyCode.Alpha0))
                    isKeyDown[0] = false;
            }
        }
    }
}
