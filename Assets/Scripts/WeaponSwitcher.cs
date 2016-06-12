using UnityEngine;
using System.Collections;

public class WeaponSwitcher : MonoBehaviour {

    public GameObject[] weapons;

    void Awake()
    {
        // set to default weapon when Awake
        changeWeapon(0);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            changeWeapon(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            changeWeapon(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            changeWeapon(2);
        }
    }

    public void changeWeapon(int index)
    {
        if (index >= weapons.Length) return;
        disableAll();
        weapons[index].SetActive(true);
    }

    public void disableAll()
    {
        foreach (GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
    }

}
