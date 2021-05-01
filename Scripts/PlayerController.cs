using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(ConfigurableJoint))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] // Affiche la valeur dans la page Unity
    private float speed = 3f;
    [SerializeField] // Affiche la valeur dans la page Unity
    private float mouseSensitivityX = 3f;
    [SerializeField]
    private float mouseSensitivityY = 3f;
    [SerializeField]
    private float thrusterForce = 1000f; // JetPack

    [Header("Joint Options")]
    [SerializeField]
    private float jointSpring = 15f;
    [SerializeField]
    private float jointMaxForce = 50f;

    private PlayerMotor motor;
    private ConfigurableJoint joint;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>(); // Permet de récupérer le contenu du PlayerController dans la var "motor"
        joint = GetComponent<ConfigurableJoint>(); // Permet de récupérer le contenu du ConfigurableJoint dans la var "joint"
        Debug.Log("PlayerController started");

        SetJointSettings(jointSpring); // Activation des paramètres du jetpack au start du jeu
    }

    private void Update()
    {
        CalculMovePlayer();
        CalculRotatePlayer(); // + totate caméra
        JumpForJetpack();
    }

    private void CalculMovePlayer()
    {
        // Calculer la vitesse du mouvement de notre joueur
        float xMov = Input.GetAxisRaw("Horizontal");
        float zMov = Input.GetAxisRaw("Vertical");


        Vector3 moveHorizontal = transform.right * xMov;
        Vector3 moveVertical = transform.forward * zMov;

        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        motor.Move(velocity);
    }

    private void CalculRotatePlayer()
    {
        // On calcule la rotation du joueur
        float yRot = Input.GetAxisRaw("Mouse X");

        Vector3 rotation = new Vector3(0, yRot, 0) * mouseSensitivityX;
        motor.Rotate(rotation);

        // On calcule la rotation de la caméra joueur
        float xRot = Input.GetAxisRaw("Mouse Y");

        float cameraRotationX = xRot * mouseSensitivityY;
        motor.CameraRotate(cameraRotationX);
    }

    private void JumpForJetpack()
    {
        Vector3 thrusterVelocity = Vector3.zero;
        // Applique la var thrusterForce (pour le jetpack)
        if (Input.GetButton("Jump"))
        {
            thrusterVelocity = Vector3.up * thrusterForce;
            SetJointSettings(0f); // Gravité de 0
        }
        else
        {
            SetJointSettings(jointSpring); // Gravité remise vu que le joueur ne saute pas
        }

        motor.ApplyThruster(thrusterVelocity);
    }

    private void SetJointSettings(float _jointSpring)
    {
        joint.yDrive = new JointDrive { positionSpring = _jointSpring, maximumForce = jointMaxForce };
    }

}