using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/*
 * A class for receiving and updating the player status
 */
public class PlayerStatus : MonoBehaviour {

    public Text nameTag;
    public Text hpTag;

    [PunRPC]
    public void updateName(string name)
    {
        nameTag.text = name;
        Debug.Log("Player's name updated to: " + name);
    }

    [PunRPC]
    public void updateHP(int hp, int maxHp)
    {
        hpTag.text = hp + "/" + maxHp;
        Debug.Log("Player's hp updated to: " + hp + " out of " + maxHp);
    }

}
