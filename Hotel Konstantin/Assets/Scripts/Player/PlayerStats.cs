
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] private Image StaminaBar;

    [SerializeField] private Player Player;

#if !UNITY_ANDROID
    private void Start()
    {
        Player.OnChanges += UpdateInfo;
        UpdateInfo();
    }

    public void UpdateInfo()
    {
        StaminaBar.fillAmount = Player._Stamina / 5f;
        
        if(Player._Stamina == 5)
        {
            StaminaBar.gameObject.SetActive(false);
        }
        else if (!StaminaBar.gameObject.activeSelf)
        {
            StaminaBar.gameObject.SetActive(true);
        }
    }
#endif
}
