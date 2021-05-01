using UnityEngine;
using Mirror;
using System.Collections;

namespace Scripts
{
    public class Player : NetworkBehaviour
    {
        [SerializeField]
        private float maxHealth = 100f;
        [SyncVar] // Permet de sync pour tt le monde la variable du joueur
        private float currentHealth;

        [SerializeField]
        private Behaviour[] disableOnDeath;
        private bool[] wasEnabledOnStart;

        [SyncVar] // Synchro, pour savoir quel joueur est mort
        private bool _isDead = false;
        public bool isDead
        {
            get { return _isDead; } // get&set sont des accesseurs
            protected set { _isDead = value; } // Protected pour protéger la modif de cette valeur en passant par ailleurs (uniquement la classe Player)
        }

        public void Setup()
        {
            wasEnabledOnStart = new bool[disableOnDeath.Length];
            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                wasEnabledOnStart[i] = disableOnDeath[i].enabled;
            }

            SetDefaults();
        }

        private void SetDefaults()
        {
            isDead = false;
            currentHealth = maxHealth;

            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].enabled = wasEnabledOnStart[i];
            }

            Collider col = GetComponent<Collider>(); // Récupère l'object qui crée les collisions entre joueurs (si plusieurs = Array+boucle)
            if (col != null) // Vérification si elle existe pour éviter les erreurs
            {
                col.enabled = true; // On l'active
            }
        }

        private IEnumerator Respawn()
        {
            yield return new WaitForSeconds(GameManager.instance.matchSettings.respawnTimer);
            SetDefaults();

            Transform spawnPoint = NetworkManager.singleton.GetStartPosition(); // Remise à un spawnpoint pour le respawn
            transform.position = spawnPoint.position;
            transform.rotation = spawnPoint.rotation;
        }
        private void Update()
        {
            if (!isLocalPlayer) { return; }

            if (Input.GetKeyDown(KeyCode.K))
            {
                RpcTakeDamage(50f);
            }
        }

        [ClientRpc] // ClientRpc = Server -> Client (Equivalent du TriggerClientEvent pour all )
        public void RpcTakeDamage(float amountDamage)
        {

            if (isDead) { return; }

            currentHealth -= amountDamage;
            Debug.Log(transform.name + " a maintenant " + currentHealth + " point(s) de vie");

            if (currentHealth <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;

            for (int i = 0; i < disableOnDeath.Length; i++)
            {
                disableOnDeath[i].enabled = false;
            }

            Collider col = GetComponent<Collider>(); // Récupère l'object qui crée les collisions entre joueurs (si plusieurs = Array+boucle)
            if (col != null) // Vérification si elle existe pour éviter les erreurs
            {
                col.enabled = false; // On les désactive à la mort
            }

            Debug.Log(transform.name + "a été éliminé.");

            StartCoroutine(Respawn());
        }

        private void OnGUI()
        {
            if (isDead)
            {
                GUILayout.BeginArea(new Rect(1, 500, 200, 500)); // Coords d'affichage
                GUILayout.BeginVertical(); // Affichage à la verticale
                GUILayout.Label("Tu es actuellement mort.");
                GUILayout.EndVertical();
                GUILayout.EndArea();
            }
        }
    }
}

