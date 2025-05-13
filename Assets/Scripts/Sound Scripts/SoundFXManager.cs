using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using static Unity.VisualScripting.Member;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] private AudioSource soundFXObject;
    [SerializeField] private AudioSource[] loopFXSources;  // Array to hold multiple looping audio sources
    private Dictionary<SoundType, object> soundFXDict;
    [SerializeField] private List<SoundVolumeEntry> soundVolumeList;
    private Dictionary<SoundType, float> soundVolumeDict;
    /*AudioSource source;
    float fadeDuration = 0;
    float fadeTimer = 2;
    bool stop;*/

    public class LoopSoundInstance
    {
        public AudioSource source;
        public GameObject obj;          // The GameObject holding the AudioSource
        public float fadeTimer = 2;
        public float fadeDuration = 0;
        public bool stop;
    }

    private Dictionary<GameObject, LoopSoundInstance> activeLoopsByObject = new Dictionary<GameObject, LoopSoundInstance>();

    public void StopLoop(LoopSoundInstance instance)
    {
        if (instance != null)
            instance.stop = true;
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        InitializeSounds();

    }

    [System.Serializable]
    public class SoundVolumeEntry
    {
        public SoundType soundType;  // The sound effect type (key)
        public float volume;         // The volume level (value)
    }

    private void OnValidate()
    {
        if (soundVolumeList != null)
        {
            soundVolumeDict = new Dictionary<SoundType, float>();
            foreach (var entry in soundVolumeList)
            {
                soundVolumeDict[entry.soundType] = entry.volume;
            }
        }
    }

    private void InitializeSounds()
    {
        soundFXDict = new Dictionary<SoundType, object>
            {
                //Single AudioClips
                { SoundType.Death, Resources.Load<AudioClip>("Sounds/Effects/Death") },
                { SoundType.Coin, Resources.Load<AudioClip>("Sounds/Effects/Coin") },
                { SoundType.Roll, Resources.Load<AudioClip>("Sounds/Effects/Roll") },
                { SoundType.RollingOther, Resources.Load<AudioClip>("Sounds/Effects/RollOther") },
                { SoundType.Chain, Resources.Load<AudioClip>("Sounds/Effects/Chain") },


                //AudioClips
                { SoundType.Break, Resources.LoadAll<AudioClip>("Sounds/Effects/Break") },
                { SoundType.Walk, Resources.LoadAll<AudioClip>("Sounds/Effects/Walk") },
                { SoundType.Boing, Resources.LoadAll<AudioClip>("Sounds/Effects/Boing") },
                { SoundType.Smack, Resources.LoadAll<AudioClip>("Sounds/Effects/Smack") },
                { SoundType.Bonk, Resources.LoadAll<AudioClip>("Sounds/Effects/Bonk") },
                { SoundType.Launch, Resources.LoadAll<AudioClip>("Sounds/Effects/Launch") },
                { SoundType.Slash, Resources.LoadAll<AudioClip>("Sounds/Effects/Slash") }

            };
    }

    private void Update()
    {
        /* timer += Time.deltaTime;
         if (source != null && !stop)
         {
             fadeTimer = Mathf.Clamp(fadeTimer, 0f, fadeDuration);
             fadeTimer += Time.deltaTime;
             float t = fadeTimer / fadeDuration;
             source.volume = Mathf.Clamp01(t);
         }
         if (source != null && stop)
         {
             fadeTimer = Mathf.Clamp(fadeTimer, 0f, fadeDuration);
             fadeTimer -= Time.deltaTime;
             float t = fadeTimer / fadeDuration;
             source.volume = Mathf.Clamp01(t);
         }*/
        // Update fading for each active loop
        List<GameObject> objectsToRemove = new List<GameObject>(); // List to store objects to be removed

        foreach (var entry in activeLoopsByObject)
        {
            LoopSoundInstance loop = entry.Value;

            if (!loop.stop)
            {
                loop.fadeTimer += Time.deltaTime;
            }
            else
            {
                loop.fadeTimer -= Time.deltaTime;
            }

            // Clamp fadeTimer to avoid it going out of bounds
            loop.fadeTimer = Mathf.Clamp(loop.fadeTimer, 0f, loop.fadeDuration);

            // Calculate volume based on fade
            float t = loop.fadeTimer / loop.fadeDuration;
            loop.source.volume = Mathf.Clamp01(t);

            // If fade-out is complete, stop and mark the object for later removal
            if (loop.stop && loop.fadeTimer <= 0f)
            {
                loop.source.Stop();
                Destroy(loop.obj); // Clean up the temporary GameObject
                objectsToRemove.Add(entry.Key); // ✅ ONLY mark for removal here
            }
        }

        // ✅ Now safe to remove from dictionary *after* iteration
        foreach (var obj in objectsToRemove)
        {
            activeLoopsByObject.Remove(obj);
        }

    }

    public void PlaySoundFX(SoundType type, Vector3? position = null)
    {
        if (!soundFXDict.ContainsKey(type)) return;

        float volume = soundVolumeDict.ContainsKey(type) ? soundVolumeDict[type] : 1.0f;
        if (soundFXDict[type] is AudioClip singleClip)
        {
            if (position.HasValue)
            {
                AudioSource.PlayClipAtPoint(singleClip, position.Value);
            }
            else
            {
                soundFXObject.PlayOneShot(singleClip, volume);
            }
        }
        else if (soundFXDict[type] is AudioClip[] clipArray)
        {
            int rand = Random.Range(0, clipArray.Length);
            var selectedClip = clipArray[rand];

            if (position.HasValue)
            {
                AudioSource.PlayClipAtPoint(selectedClip, position.Value, volume);
            }
            else
            {
                soundFXObject.PlayOneShot(selectedClip, volume);
            }
        }
    }

   /* public void Start3DLoop(SoundType type, Vector3 position, Transform parent = null)
    {
        // Load clip directly just to be 100% sure it’s valid
        AudioClip clip = Resources.Load<AudioClip>($"Sounds/Effects/{type}");

        if (clip == null)
        {
            Debug.LogError($"[SoundFXManager] AudioClip not found for: {type}");
            return;
        }

        GameObject tempGO = new GameObject("3D_Loop_" + type);
        if (parent != null)
        {
            tempGO.transform.SetParent(parent);
            tempGO.transform.localPosition = Vector3.zero; // So it's centered on parent
        }
        else
        {
            tempGO.transform.position = position;
        }


        source = tempGO.AddComponent<AudioSource>();
        //source.volume = 0;
        source.clip = clip;
        source.loop = true;

        source.spatialBlend = 1f;
        source.minDistance = 1f;
        source.maxDistance = 50f;
        source.playOnAwake = false;

        source.Play();

        LoopSoundInstance instance = new LoopSoundInstance
        {
            source = source,
            obj = tempGO,
            fadeTimer = 0f,
            stop = false,
            fadeDuration = 1f // or pass this in if needed
        };

        activeLoops.Add(instance);

        //return source;
    }*/

    public void StartLoopFor(GameObject owner, SoundType type, Vector3 position, Transform parent = null)
    {
        if (activeLoopsByObject.ContainsKey(owner)) return; // If there's already a loop for this object, don't start another

        // Load clip
        AudioClip clip = Resources.Load<AudioClip>($"Sounds/Effects/{type}");
        if (clip == null)
        {
            Debug.LogError($"[SoundFXManager] AudioClip not found for: {type}");
            return;
        }

        // Create new GameObject for the sound
        GameObject tempGO = new GameObject($"3D_Loop_{owner.name}_{type}");
        if (parent != null)
        {
            tempGO.transform.SetParent(parent);
            tempGO.transform.localPosition = Vector3.zero; // So it's centered on parent
        }
        else
        {
            tempGO.transform.position = position;
        }
        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.spatialBlend = 1f;
        source.minDistance = 1f;
        source.maxDistance = 50f;
        source.playOnAwake = false;
        source.Play();

        // Create a LoopSoundInstance to store the sound state
        LoopSoundInstance instance = new LoopSoundInstance
        {
            source = source,
            obj = tempGO,
            fadeTimer = 0f,
            fadeDuration = 0.1f, // Adjust this value as needed for fade-in time
            stop = false
        };

        // Add the LoopSoundInstance to the dictionary
        activeLoopsByObject.Add(owner, instance);
    }

    public void StopLoopFor(GameObject owner)
    {
        if (!activeLoopsByObject.ContainsKey(owner)) return; // No loop found for this object

        LoopSoundInstance instance = activeLoopsByObject[owner];
        instance.stop = true; // Mark it for stopping (fade out)
    }

    /* public void StopLoop()
     {
         if (source != null)
         {
             stop = true;
             StartCoroutine(DestroyObject());
         }
     }*/

    /* IEnumerator DestroyObject()
     {
         yield return new WaitUntil(() => fadeTimer <= 0f);
         stop = false;
         source.Stop();
         Destroy(source.gameObject);
     }*/

    /* private void StopLoop(SoundType type)
     {
         if (!soundFXDict.ContainsKey(type)) return;

         if (soundFXDict[type] is AudioClip singleClip)
         {
             foreach (var audioSource in loopFXSources)
             {
                 if (audioSource.clip == singleClip)
                 {
                     audioSource.Stop();
                     audioSource.loop = false;
                 }
             }
         }
     }*/
}

public enum SoundType
{
    // Array Of Sounds
    Break,
    Bonk,
    Boing,
    Coin,
    Death,
    Launch,
    Smash,
    Smack,
    Chain,
    Slash,
    Walk,
    Roll,
    RollingOther
}

