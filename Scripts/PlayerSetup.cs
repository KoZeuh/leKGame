using UnityEngine;
using Mirror;

namespace Scripts

{
    public class PlayerSetup : NetworkBehaviour
    {
        [SerializeField]
        Behaviour[] componentsToDisable;

        [SerializeField]
        private string remoteLayerName = "RemotePlayer"; // Doit correspondre à Layer crée

        Camera sceneCamera;
        private void Start()
        {
            if (!isLocalPlayer) // Var lié à Mirror designant notre joueur
            {
                DisableComponents();
                AssignRemotePlayer();
            }
            else
            {
                sceneCamera = Camera.main;
                if (sceneCamera != null)
                {
                    sceneCamera.gameObject.SetActive(false); // Désative le Camera Sc-ne
                }
            }

            GetComponent<Player>().Setup();
        }

        public override void OnStartClient()
        {
            base.OnStartClient();

            string netID = GetComponent<NetworkIdentity>().netId.ToString(); // Stock l'ID Unique du joueur
            Player player = GetComponent<Player>(); // stock le Prefabs Player

            GameManager.RegisterPlayer(netID, player); // Enregistre l'ID + Le joueur en question
        }

        private void AssignRemotePlayer()
        {
            gameObject.layer = LayerMask.NameToLayer(remoteLayerName); // Assignation de l'object au layer
        }

        private void DisableComponents()
        {
            // Boucle permettant de désactiver les composants si ce n'est pas notre joueur
            for (int i = 0; i < componentsToDisable.Length; i++)
            {
                componentsToDisable[i].enabled = false;
            }

        }
        private void OnDisable()
        {
            if (sceneCamera != null)
            {
                sceneCamera.gameObject.SetActive(true); // Réactive la Camera Scène si joueur a deco
            }

            GameManager.UnregisterPlayer(transform.name);
        }
    }
}