using UnityEngine;

public class SFXManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] audioClips; //0 StartGame, 1 CardDraw, 2 Armor, 3 Attack, 4 RobotExplosion
    [SerializeField] private AudioSource aud;

    public static SFXManager Instance;


    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void PlaySFX(int clipNumb)
    {
        aud.clip = audioClips[clipNumb];
        aud.Play();
    }



}
