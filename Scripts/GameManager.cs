using System.Collections.Generic;
using UnityEngine;

namespace Scripts
{
    public class GameManager : MonoBehaviour
    {
        private const string pIDPrefix = "Player [";
        private const string pIDSuffix = "]";

        private static Dictionary<string, Player> players = new Dictionary<string, Player>();

        public MatchSettings matchSettings;

        public static GameManager instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                return;
            }

            Debug.LogError("Plus d'une instance de GameManager dans la scène !");
        }

        public static void RegisterPlayer(string netID, Player player)
        {
            string playerId = pIDPrefix + netID + pIDSuffix;
            players.Add(playerId, player); // Ajout d'ID Unique du joueur ds le dictionnaire de tout les joueurs du serveur
            player.transform.name = playerId; // Rename definitive player
        }

        public static void UnregisterPlayer(string playerId)
        {
            players.Remove(playerId);
        }

        private void OnGUI() // Propre à Unity, équivalent à un DrawText & DrawRect sur FiveM
        {
            GUILayout.BeginArea(new Rect(1, 200, 200, 500)); // Coords d'affichage
            GUILayout.BeginVertical(); // Affichage à la verticale

            foreach (string playerId in players.Keys) // Boucle de recup du pID dans le dictionnaire
            {
                GUILayout.Label("Joueur présent : " + playerId);
            }

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        public static Player GetPlayer(string playerId) // Recup d'un joueur en particulier
        {
            return players[playerId];
        }
    }
}

