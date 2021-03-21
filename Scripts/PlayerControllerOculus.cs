using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Oculus;
using OVR;
using TMPro;
using Photon.Pun;

public class PlayerControllerOculus : MonoBehaviour
{
    // Actions for Player
    [SerializeField] private bool shoot;
    // Game Objects
    [SerializeField] private GameObject ChoppaGun;
    [SerializeField] private GameObject NightHawk;
    [SerializeField] private GameObject OVRPlayerController;
    [SerializeField] private GameObject OVRCameraRig;
    [SerializeField] private GameObject DriverSeat;
    [SerializeField] private GameObject ShooterSeat;
    [SerializeField] private GameObject NetworkManager;


    [SerializeField] private TextMeshProUGUI positionDebug;

    // boolean values 
    [SerializeField] private bool reverseHawk;
    [SerializeField] private bool dropped;
    [SerializeField] private bool nearDriverPosition;
    [SerializeField] private bool nearShooterPosition;
    [SerializeField] private bool enteredCar;

    // Car values
    [SerializeField] private float accelerateValue = 0.0f;
    [SerializeField] private float reverseValue = 0.0f;
    [SerializeField] private float brakeValue = 0.0f;
    [SerializeField] private float exitCarValue = 0.0f;

    [HideInInspector] public enum PlayerState {Shooter, Driver, Both, MainMenu};
    [SerializeField] private PlayerState playerState = PlayerState.Both;

    // IF CAN WALK
    //[SerializeField] private GameObject DriverPosition;
    //[SerializeField] private GameObject ShooterPosition;
    //[SerializeField] private GameObject ExitDriverPosition;
    //[SerializeField] private GameObject ExitShooterPosition;

    private void Awake()
    {
        NightHawk = GameObject.FindGameObjectWithTag("Hawk");
        OVRPlayerController = GameObject.FindGameObjectWithTag("OVRPlayerController");
        OVRCameraRig = GameObject.FindGameObjectWithTag("OVRCameraRig");
        DriverSeat = GameObject.FindGameObjectWithTag("DriverSeat");
        ShooterSeat = GameObject.FindGameObjectWithTag("ShooterSeat");
        NetworkManager = GameObject.FindGameObjectWithTag("NetworkManager");
    }

    void Start()
    {
    }

    private void Update()
    {
        if (ChoppaGun == null)
        {
            ChoppaGun = GameObject.FindGameObjectWithTag("Choppa");
        }
        #region PlayerStates
        switch (playerState)
        {
            case PlayerState.Both:
                PlacePlayer(DriverSeat.transform);
                UseBothInputs();
                return;
            case PlayerState.Driver:
                UseDriverInputs(PlayerState.Driver);
                return;
            case PlayerState.Shooter:
                PlacePlayer(ShooterSeat.transform);
                UseShooterInputs(PlayerState.Shooter);
                return;
            case PlayerState.MainMenu:
                UseMainMenuInputs();
                return;
            // do stuff
            default:
                return;
        }
        #endregion
    }

    private void FixedUpdate()
    {
        //positionDebug.text = OVRPlayerController.transform.position.ToString();
        //if (!enteredCar)
        //{
        //    CheckIfEnteredCar(PlayerState.Driver, nearDriverPosition, DriverSeat.transform);
        //    CheckIfEnteredCar(PlayerState.Shooter, nearShooterPosition, ShooterSeat.transform);
        //}
    }
    #region Inputs
    void UseDriverInputs(PlayerState state)
    {
        // TODO: Use only when the walking inputs are enabled
        //var ovrController = OVRPlayerController.GetComponent<OVRPlayerController>();
        //ovrController.Acceleration = 0.0f;
        //ovrController.RotationAmount = 0.0f;
        //ovrController.RotationRatchet = 0.0f;

        // Change layer to Player
        //ChangeLayer(OVRPlayerController, 12);

        // Use the X button to accelerate the car
        #region DriverState
        if (state == PlayerState.Driver)
        {
            var accelerate = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
            if (accelerate >= 0.1f)
            {
                //accelerateValue += 0.1f;
                //accelerateValue = Mathf.Clamp(accelerateValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().accelerateHawk = true;
                NightHawk.GetComponent<NightHawk>().acceleration = accelerate * 10.0f;
            }
            else
            {
                //accelerateValue -= 0.1f;
                //accelerateValue = Mathf.Clamp(accelerateValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().accelerateHawk = false;
                NightHawk.GetComponent<NightHawk>().acceleration = accelerate * 5.0f;
            }

            // Use the A button to reverse the car
            var reverse = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
            if (reverse >= 0.1f)
            {
                //reverseValue += 0.1f;
                //reverseValue = Mathf.Clamp(reverseValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().reverseHawk = true;
                NightHawk.GetComponent<NightHawk>().reverse = reverse * 20.0f;
            }
            else
            {
                //reverseValue -= 0.1f;
                //reverseValue = Mathf.Clamp(reverseValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().reverseHawk = false;
                NightHawk.GetComponent<NightHawk>().reverse = reverse * 5.0f;
            }

            // Use the LThumbstick to brake the car
            var brake = OVRInput.Get(OVRInput.RawButton.LThumbstick);
            if (brake)
            {
                brakeValue += 0.1f;
                brakeValue = Mathf.Clamp(brakeValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().brakeHawk = true;
                NightHawk.GetComponent<NightHawk>().brakeDeceleration = brakeValue * 20.0f;
            }
            else
            {
                brakeValue -= 0.1f;
                brakeValue = Mathf.Clamp(brakeValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().brakeHawk = false;
                NightHawk.GetComponent<NightHawk>().brakeDeceleration = brakeValue * 20.0f;
            }
        }
        #endregion
        #region BothState
        else if (state == PlayerState.Both)
        {
            var accelerate = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
            if (accelerate >= 0.1f)
            {
                //accelerateValue += 0.1f;
                //accelerateValue = Mathf.Clamp(accelerateValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().accelerateHawk = true;
                NightHawk.GetComponent<NightHawk>().acceleration = accelerate * 10.0f;
            }
            else
            {
                //accelerateValue -= 0.1f;
                //accelerateValue = Mathf.Clamp(accelerateValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().accelerateHawk = false;
                NightHawk.GetComponent<NightHawk>().acceleration = accelerate * 5.0f;
            }

            // Use the A button to reverse the car
            var reverse = OVRInput.Get(OVRInput.RawButton.X);
            if (reverse)
            {
                reverseValue += 0.1f;
                reverseValue = Mathf.Clamp(reverseValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().reverseHawk = true;
                NightHawk.GetComponent<NightHawk>().reverse = reverseValue * 20.0f;
            }
            else
            {
                reverseValue -= 0.1f;
                reverseValue = Mathf.Clamp(reverseValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().reverseHawk = false;
                NightHawk.GetComponent<NightHawk>().reverse = reverseValue * 5.0f;
            }

            // Use the LThumbstick to brake the car
            var brake = OVRInput.Get(OVRInput.RawButton.LThumbstick);
            if (brake)
            {
                brakeValue += 0.1f;
                brakeValue = Mathf.Clamp(brakeValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().brakeHawk = true;
                NightHawk.GetComponent<NightHawk>().brakeDeceleration = brakeValue * 20.0f;
            }
            else
            {
                brakeValue -= 0.1f;
                brakeValue = Mathf.Clamp(brakeValue, 0.0f, 1.0f);
                NightHawk.GetComponent<NightHawk>().brakeHawk = false;
                NightHawk.GetComponent<NightHawk>().brakeDeceleration = brakeValue * 20.0f;
            }
        }
        #endregion
        #region LEAVE CAR
        // TODO: FOR WALKING LATER 
        //var exitCar = OVRInput.Get(OVRInput.RawButton.X) && OVRInput.Get(OVRInput.RawButton.A);
        //if (exitCar)
        //{
        //    exitCarValue += 0.1f; 
        //    if (exitCarValue >= 1.0f && playerState == PlayerState.Driver)
        //    {
        //        enteredCar = false;
        //        RemovePlayer(ExitDriverPosition.transform);
        //        ChangeState(PlayerState.Walking);
        //    }

        //    else if (exitCarValue >= 1.0f && playerState == PlayerState.Shooter)
        //    {
        //        enteredCar = false;
        //        RemovePlayer(ExitShooterPosition.transform);
        //        ChangeState(PlayerState.Walking);
        //    }
        //}
        //else
        //{
        //    exitCarValue = 0.0f;
        //}
        #endregion LEAVE CAR
    }
    void UseShooterInputs(PlayerState state)
    {
        // TODO: Use only when walking inputs are available
        //var ovrController = OVRPlayerController.GetComponent<OVRPlayerController>();
        //ovrController.Acceleration = 0.1f;
        //ovrController.RotationAmount = 0.0f;
        //ovrController.RotationRatchet = 0.0f;

        //// Change layer to Player
        //ChangeLayer(OVRPlayerController, 12);
        if (state == PlayerState.Shooter && ChoppaGun != null)
        {
            var rightTrigger = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
            var leftTrigger = OVRInput.Get(OVRInput.RawAxis1D.LIndexTrigger);
            var choppaScript = ChoppaGun.GetComponent<Choppa>();

            if (rightTrigger >= 0.1f && choppaScript.grab == Choppa.handGrab.RightHand)
            {
                choppaScript.shootingRight = true;
            }
            else
            {
                choppaScript.shootingRight = false;
            }

            if (leftTrigger >= 0.1f && choppaScript.grab == Choppa.handGrab.LeftHand)
            {
                choppaScript.shootingLeft = true;
            }
            else
            {
                choppaScript.shootingLeft = false;
            }
        }
        else if (state == PlayerState.Both && ChoppaGun != null)
        {
            var rightTrigger = OVRInput.Get(OVRInput.RawAxis1D.RIndexTrigger);
            var choppaScript = ChoppaGun.GetComponent<Choppa>();
            if (rightTrigger >= 0.1f && choppaScript.grab == Choppa.handGrab.RightHand)
            {
                choppaScript.shootingRight = true;
            }
            else
            {
                choppaScript.shootingRight = false;
            }
            //if (leftTrigger >= 0.1f && choppaScript.grab == Choppa.handGrab.LeftHand)
            //{
            //    choppaScript.shootingLeft = true;
            //}
            //else
            //{
            //    choppaScript.shootingLeft = false;.
            //}
        }
        #region LEAVECAR
        //var exitCar = OVRInput.Get(OVRInput.RawButton.X) && OVRInput.Get(OVRInput.RawButton.A);
        //if (exitCar)
        //{
        //    exitCarValue += 0.1f;
        //    if (exitCarValue >= 1.0f && playerState == PlayerState.Driver)
        //    {
        //        enteredCar = false;
        //        RemovePlayer(ExitDriverPosition.transform);
        //        ChangeState(PlayerState.Walking);
        //    }

        //    else if (exitCarValue >= 1.0f && playerState == PlayerState.Shooter)
        //    {
        //        enteredCar = false;
        //        RemovePlayer(ExitShooterPosition.transform);
        //        ChangeState(PlayerState.Walking);
        //    }
        //}
        #endregion
    }
    void UseBothInputs()
    {
        // TODO: ADD THIS WHEN WALKING INPUTS ARE BACK
        //var ovrController = OVRPlayerController.GetComponent<OVRPlayerController>();
        //ovrController.Acceleration = 0.1f;
        //ovrController.RotationAmount = 1.5f;
        //ovrController.RotationRatchet = 45f;
        //ChangeLayer(OVRPlayerController, 0);
        UseDriverInputs(PlayerState.Both);
        UseShooterInputs(PlayerState.Both);
    }

    void UseMainMenuInputs()
    {
        var networkScript = NetworkManager.GetComponent<NetworkManager>();
        if (OVRInput.Get(OVRInput.RawButton.A))
        {
            networkScript.connect = true;
        }
        else
        {
            networkScript.connect = false;
        }
    }
    public void ChangeState(PlayerState state)
    {
        Debug.Log(state);
        this.playerState = state;
    }
    #endregion
    #region Other functions
    void ChangeLayer(GameObject gObject, int layerNumber)
    {
        gObject.layer = layerNumber;
        int childrenNumber = transform.childCount;
        for (int i = 0; i < childrenNumber; i++)
        {
            transform.GetChild(i).gameObject.layer = layerNumber;
        }
    }
    // TODO: ONLY ADD WHEN WALKING INPUTS ARE BACK
    void PlacePlayer(Transform seat)
    {
        //OVRPlayerController.transform.SetParent(NightHawk.transform);
        OVRCameraRig.transform.position = seat.position;
        OVRCameraRig.transform.rotation = seat.rotation;
    }


    //void RemovePlayer(Transform removePosition)
    //{
    //    OVRPlayerController.transform.SetParent(null);
    //    OVRPlayerController.transform.position = removePosition.position;
    //    OVRPlayerController.transform.rotation = removePosition.rotation;
    //}

    // ONLY USE THIS WHEN WALKING INPUTS ARE BACK
    //void CheckIfEnteredCar(PlayerState state, bool nearPosition, Transform seat)
    //{
    //    bool leftTrigger = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.LTouch);
    //    bool rightTrigger = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger, OVRInput.Controller.RTouch);
    //    if (nearPosition && leftTrigger)
    //    {
    //        Debug.Log("EnteredCar");
    //        ChangeState(state);
    //        PlacePlayer(seat);
    //        enteredCar = true;
    //    }
    //    else if (nearPosition && rightTrigger)
    //    {
    //        ChangeState(state);
    //        PlacePlayer(seat);
    //        enteredCar = true;
    //    }
    //}
    #endregion
    #region Collision
    //// todo: only use this when walking inputs are back
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.tag == "DriverPosition")
    //    {
    //        nearDriverPosition = true;
    //    }

    //    if (other.gameObject.tag == "ShooterPosition")
    //    {
    //        nearShooterPosition = true;
    //    }
    //}
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.tag == "DriverPosition")
    //    {
    //        nearDriverPosition = false;
    //    }

    //    if (other.gameObject.tag == "ShooterPosition")
    //    {
    //        nearShooterPosition = false;
    //    }
    //}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.gameObject.name);
    }
    #endregion
}