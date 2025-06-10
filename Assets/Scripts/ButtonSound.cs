using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonSFX : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler, ISubmitHandler
{
    readonly float soundCooldown = 0.2f; // Cooldown time between sounds
    float nextPlayTime = 0f;
    readonly float soundCooldownSelect = 0.2f; // Cooldown time between sounds
    float nextPlayTimeSelect = 0f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayButtonSound();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        //PlayButtonSelect();
    }

    public void OnSubmit(BaseEventData eventData) 
    {
      //  PlayButtonSelect();
    }
    public void OnSelect(BaseEventData eventData)
    {
        PlayButtonSound();
    }

    void PlayButtonSelect()
    {
        if (Time.unscaledTime >= nextPlayTimeSelect)
        {
           
            SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSelect);
            nextPlayTimeSelect = Time.unscaledTime + soundCooldownSelect;
        }
    }

    void PlayButtonSound()
    {
        if (Time.unscaledTime >= nextPlayTime)
        {
           
            SoundFXManager.Instance.PlayButtonSoundFX(SoundType.ButtonSound);
            nextPlayTime = Time.unscaledTime + soundCooldown;
        }
    }
}