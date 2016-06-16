using UnityEngine;
using System.Collections;

/**
 * A class to manage feeds and events
 */
public class FeedManager : MonoBehaviour {

    public InRoomChat chat;

    private const string BATTLE_FEED_PREFIX = "[Battle] ";

    public void addKillFeed(string target, string killer)
    {
        string msg = target + " was killed by " + killer;
        addToFeed(BATTLE_FEED_PREFIX, msg);
    }

    public void addDamageFeed(string target, string dmgFrom, int dmg)
    {
        string msg = dmgFrom + " hitted " + target + " inflicting " + dmg + " damage";
        addToFeed(BATTLE_FEED_PREFIX, msg);
    }

    private void addToFeed(string prefix, string msg)
    {
        chat.AddFeed(prefix + msg);
    }

}
