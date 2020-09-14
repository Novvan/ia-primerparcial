using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    private int _currentLife;
    public int maxLife;
    public int keys = 0;
    public GameObject missionHolder;
    private TextMeshProUGUI _missionText;
    private void Start()
    {
        _currentLife = maxLife;
        _missionText = missionHolder.GetComponent<TextMeshProUGUI>();
        _missionText.text = "Collect all keys: " + keys + "/6";
    }

    public void CollectKey()
    {
        keys++;
        //Debug.Log(keys);
        if (keys < 6) _missionText.text = "Collect all keys: " + keys + "/6";
        else _missionText.text = "Get to the car!";
    }
}