using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundFXManager : MonoBehaviour
{
    public static SoundFXManager Instance;

    [SerializeField] 
    AudioSource soundFXObject;
    [SerializeField]
     List<SoundVolumeEntry> volumeList;
    [SerializeField] private AudioMixerGroup sfxMixerGroup;

    Dictionary<SoundType, object> soundFXDict;
    // Dictionary for volume on sounds
    Dictionary<SoundType, float> soundVolumeDict = new Dictionary<SoundType, float>();
    // For Looping soundFX
    Dictionary<GameObject, LoopSoundInstance> activeLoopsByObject = new Dictionary<GameObject, LoopSoundInstance>();

    public class LoopSoundInstance
    {   // Values for the looping soundfX's
        public AudioSource source;
        public GameObject obj;          // The GameObject holding the AudioSource
        public float fadeTimer = 2;
        public float fadeDuration = 0;
        public bool stop;
        public float targetVolume;
    }

    [System.Serializable]
    public class SoundVolumeEntry
    {
        public SoundType soundType;  // The sound effect type (key)
        [Range(0, 1)] public float volume;         // The volume level (value)
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        foreach (var entry in volumeList)
        {
            soundVolumeDict[entry.soundType] = entry.volume;
        }

        InitializeSounds();
    }
    private void Update()
    {
        if(activeLoopsByObject.Any(kv => !kv.Value.stop || kv.Value.fadeTimer > 0f))
        {
            LoopFade();
        }
    }

    private void OnValidate()
    {   // To change volume while playing
        if (volumeList != null)
        {
            soundVolumeDict = new Dictionary<SoundType, float>();
            foreach (var entry in volumeList)
            {
                soundVolumeDict[entry.soundType] = entry.volume;
            }
        }
    }

    private void InitializeSounds()
    {   // To find and add soundFX in assets
        soundFXDict = new Dictionary<SoundType, object>
            {
                //Single AudioClips
                { SoundType.ButtonSelect, Resources.Load<AudioClip>("Sounds/Effects/ButtonSelect") },
                { SoundType.Chain, Resources.Load<AudioClip>("Sounds/Effects/Chain") },
                { SoundType.ChestCreak, Resources.Load<AudioClip>("Sounds/Effects/ChestCreak") },
                { SoundType.ChestOpen, Resources.Load<AudioClip>("Sounds/Effects/ChestOpen") },
                { SoundType.Death, Resources.Load<AudioClip>("Sounds/Effects/Death") },
                { SoundType.Fire, Resources.Load<AudioClip>("Sounds/Effects/Fire") },
                { SoundType.KeyFound, Resources.Load<AudioClip>("Sounds/Effects/KeyFound") },
                { SoundType.PuzzleSolved, Resources.Load<AudioClip>("Sounds/Effects/PuzzleSolved") },
                { SoundType.PuzzleSolvedFully, Resources.Load<AudioClip>("Sounds/Effects/PuzzleSolvedFully") },
                { SoundType.DoorOpen, Resources.Load<AudioClip>("Sounds/Effects/DoorOpen") },
                { SoundType.PushBlock, Resources.Load<AudioClip>("Sounds/Effects/PushBlock") },
                
                //AudioClips
                { SoundType.ButtonSound, Resources.LoadAll<AudioClip>("Sounds/Effects/ButtonSound") },
                { SoundType.Break,Resources.LoadAll<AudioClip>("Sounds/Effects/Break") },
                { SoundType.PressurePlate,Resources.LoadAll<AudioClip>("Sounds/Effects/PressurePlate") },
                { SoundType.RandomScary,Resources.LoadAll<AudioClip>("Sounds/Effects/RandomScary") },
                { SoundType.Walk, Resources.LoadAll<AudioClip>("Sounds/Effects/Walk") },
            };
    }

    private void LoopFade()
    {
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
            loop.source.volume = Mathf.Clamp01(t) * loop.targetVolume;
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
                           //SoundType     // Position for soundFX    // soundFX minDistance  // soundFX MaxDistance   // If 2D or 3D
    public void PlaySoundFX(SoundType type, Vector3? position = null, float minDistance = 1f, float maxDistance = 50f, float spatialBlend = 1f)
    {
        if (!soundFXDict.ContainsKey(type)) return;

        // volume if its not in the inspector
        float volume = 1.0f;
        if (soundVolumeDict != null && soundVolumeDict.ContainsKey(type))
        {
            volume = soundVolumeDict[type];
        }

        AudioClip clip = null;
        if (soundFXDict[type] is AudioClip singleClip)
        {   // If single soundFX
            clip = singleClip;
        }
        else if (soundFXDict[type] is AudioClip[] clipArray)
        {   // If Multible soundFX
            clip = clipArray[Random.Range(0, clipArray.Length)];
        }

        if (clip == null) return;

        if (position.HasValue)
        {
            // Manual PlayClipAtPoint with custom size
            GameObject tempGO = new GameObject($"SFX_{type}");
            tempGO.transform.position = position.Value;

            AudioSource aSource = tempGO.AddComponent<AudioSource>();
            aSource.outputAudioMixerGroup = sfxMixerGroup;
            aSource.clip = clip;
            aSource.spatialBlend = spatialBlend;
            aSource.minDistance = minDistance;
            aSource.maxDistance = maxDistance;
            aSource.volume = volume;
            aSource.Play();

            Object.Destroy(tempGO, clip.length);
        }
        else
        {
            soundFXObject.PlayOneShot(clip, volume);
        }
    }

    public void StartLoopFor(GameObject owner, SoundType type,Transform parent = null)
    {
        if (activeLoopsByObject.ContainsKey(owner)) return; // If there's already a loop for this object, don't start another

        float volume = 1.0f;
        if (soundVolumeDict != null && soundVolumeDict.ContainsKey(type))
        {
            volume = soundVolumeDict[type];
        }
        AudioClip clip = null;
        if (!soundFXDict.ContainsKey(type))
        {
            Debug.LogError($"[SoundFXManager] soundFXDict does not contain key: {type}");
            return;
        }

        if (soundFXDict[type] is AudioClip singleClip)
        {
            clip = singleClip;
        }
        else if (soundFXDict[type] is AudioClip[] clipArray)
        {
            clip = clipArray[Random.Range(0, clipArray.Length)];
        }

        if (clip == null)
        {
            Debug.LogError($"[SoundFXManager] AudioClip for {type} is null.");
            return;
        }
        GameObject tempGO = new GameObject($"3D_Loop_{owner.name}_{type}");
        if (parent != null)
        {
            tempGO.transform.SetParent(parent);
            tempGO.transform.localPosition = Vector3.zero; // So it's centered on parent
        }
        AudioSource source = tempGO.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = sfxMixerGroup;
        source.clip = clip;
        source.loop = true;
        source.spatialBlend = 1f;
        source.minDistance = 1f;
        source.maxDistance = 50f;
        source.volume = volume;
        source.playOnAwake = false;
        source.Play();

        LoopSoundInstance instance = new LoopSoundInstance
        {
            source = source,
            obj = tempGO,
            fadeTimer = 0f,
            fadeDuration = 0.1f,
            stop = false,
            targetVolume = volume // ✅ Now volume is accessible
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
}

public enum SoundType
{
    // Array Of Sounds
    Break,
    ButtonSound,
    ButtonSelect,
    Chain,
    ChestCreak,
    ChestOpen,
    Death,
    DoorOpen,
    Fire,
    KeyFound,
    PushBlock,
    PuzzleSolved,
    PuzzleSolvedFully,
    PressurePlate,
    RandomScary,
    Walk
}

