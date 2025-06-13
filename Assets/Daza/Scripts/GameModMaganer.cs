using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Si usas el nuevo sistema de Input
using UnityEngine.XR.Management; // Para la gestión de VR

public class GameModeManager : MonoBehaviour
{
    [Header("Referencias a las Cámaras")]
    public GameObject vrCameraRig; // El GameObject que contiene tu cámara y controladores de VR (por ejemplo, OVRCameraRig, XR Origin, etc.)
    public GameObject firstPersonCamera; // Tu cámara de primera persona (por ejemplo, Main Camera, FirstPersonController)

    [Header("Referencias a los Controles")]
    public GameObject firstPersonController; // El GameObject que tiene el script de movimiento de primera persona
    public GameObject vrControllerSystem; // Cualquier sistema de control específico de VR (por ejemplo, el Input System de VR)

    [Header("Elementos del UI")]
    public GameObject mainMenuUI; // El Canvas o Panel que contiene los botones del menú

    private void Start()
    {
        // Asegúrate de que el menú esté visible al inicio
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
        }

        // Desactiva ambos modos al inicio para que el usuario elija
        DisableAllGameModes();
    }

    public void ActivateVRMode()
    {
        Debug.Log("Activando Modo VR...");
        DisableAllGameModes();

        // 1. Activa el XR Subsystem
        StartCoroutine(StartXR());

        // 2. Activa los objetos de VR
        if (vrCameraRig != null)
        {
            vrCameraRig.SetActive(true);
        }
        if (vrControllerSystem != null)
        {
            vrControllerSystem.SetActive(true); // O habilita componentes específicos
        }
        
        // 3. Desactiva el controlador de primera persona
        if (firstPersonController != null)
        {
            firstPersonController.SetActive(false);
        }
        if (firstPersonCamera != null)
        {
            firstPersonCamera.SetActive(false);
        }

        // Oculta el menú
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(false);
        }

        // Ajusta el Input System si es necesario
        // Ejemplo: Si usas Input System y quieres deshabilitar los controles de primera persona
        if (firstPersonController != null)
        {
            // Busca el componente PlayerInput y deshabilita la acción
            var playerInput = firstPersonController.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap("XR"); // O el mapa de acciones de VR
            }
        }
    }

    public void ActivateFirstPersonMode()
    {
        Debug.Log("Activando Modo Primera Persona...");
        DisableAllGameModes();

        // 1. Detiene el XR Subsystem
        StopXR();

        // 2. Activa los objetos de primera persona
        if (firstPersonCamera != null)
        {
            firstPersonCamera.SetActive(true);
        }
        if (firstPersonController != null)
        {
            firstPersonController.SetActive(true);
        }

        // 3. Desactiva los objetos de VR
        if (vrCameraRig != null)
        {
            vrCameraRig.SetActive(false);
        }
        if (vrControllerSystem != null)
        {
            vrControllerSystem.SetActive(false);
        }

        // Oculta el menú
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(false);
        }

        // Ajusta el Input System si es necesario
        if (firstPersonController != null)
        {
            var playerInput = firstPersonController.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                playerInput.SwitchCurrentActionMap("Player"); // O el mapa de acciones de primera persona
            }
        }
    }

    private void DisableAllGameModes()
    {
        if (vrCameraRig != null) vrCameraRig.SetActive(false);
        if (firstPersonCamera != null) firstPersonCamera.SetActive(false);
        if (firstPersonController != null) firstPersonController.SetActive(false);
        if (vrControllerSystem != null) vrControllerSystem.SetActive(false);
    }

    // Funciones para iniciar y detener el subsistema XR
    private System.Collections.IEnumerator StartXR()
    {
        Debug.Log("Inicializando XR...");
        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();

        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
        {
            Debug.LogError("No XR Loader is active. Please ensure XR is enabled in Project Settings.");
        }
        else
        {
            Debug.Log("XR Loader initialized. Starting XR...");
            XRGeneralSettings.Instance.Manager.StartSubsystems();
        }
    }

    private void StopXR()
    {
        Debug.Log("Deteniendo XR...");
        if (XRGeneralSettings.Instance.Manager.isInitializationComplete) // Check if XR is initialized
        {
            XRGeneralSettings.Instance.Manager.StopSubsystems();
            XRGeneralSettings.Instance.Manager.DeinitializeLoader();
        }
    }

    // Asegúrate de detener XR al salir de la aplicación
    private void OnApplicationQuit()
    {
        StopXR();
    }
}