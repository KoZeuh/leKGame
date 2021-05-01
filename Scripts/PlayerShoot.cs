using UnityEngine;
using Mirror;

namespace Scripts
{
    public class PlayerShoot : NetworkBehaviour
    {
        public PlayerWeapon weapon;
        [SerializeField]
        private Camera isCam;
        [SerializeField]
        private LayerMask mask;
        void Start()
        {
            if (isCam == null)
            {
                Debug.Log("Pas de caméra assignée");
                this.enabled = false; // Désactive le script sur le joueur
            }
            else
            {

            }

        }

        private void Update()
        {
            if (Input.GetButtonDown("Fire1")) // Bouton clique gauche (Edit->Project Settings->Manager->Fire1)
            {
                Shoot();
            }
        }

        [Client] // (Comme FiveM, t'appuies sur une touche c'est client)
        private void Shoot()
        {
            RaycastHit hit;

            if (Physics.Raycast(isCam.transform.position, isCam.transform.forward, out hit, weapon.range, mask)) // forward = vers l'avant
            {
                if (hit.collider.tag == "Player")
                {
                    CmdPlayerShoot(hit.collider.name, weapon.damage);
                }
            }
        }

        [Command] // Command = Envoie les informations aux serveurs en provenance du client (équivalent du TriggerServerEvent)
        private void CmdPlayerShoot(string playerID, float damage)
        {
            Debug.Log(playerID + " a été touché");

            Player player = GameManager.GetPlayer(playerID);

            player.RpcTakeDamage(damage);
        }

    }
}