using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SoundLibrary : MonoBehaviour
{

    public List<Sound> tracks = new List<Sound>();

    public FMOD.Studio.EventInstance mainMusic;

    // Start is called before the first frame update
    void Start()
    {
        mainMusic = FMODUnity.RuntimeManager.CreateInstance("");
        foreach (Sound track in tracks)
        {
            track.AutoSetPath();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Test()
    {
        Debug.Log("What");
    }

    public bool PlayTrack(int listIndex, Vector3 position)
    {
        if (listIndex >= tracks.Count)
        {
            return false;
        }
        else
        {
            FMODUnity.RuntimeManager.PlayOneShot(tracks[listIndex].path, position);
            return true;
        }
    }

    public bool PlayTrack(string trackName, Vector3 position)
    {
        int index = -1;
        for (int i = 0; i < tracks.Count; i++)
        {
            if (tracks[i].name == trackName)
            {
                index = i;
            }
        }
        return PlayTrack(index, position);
    }
}

[System.Serializable]
public class Sound
{
    public string name;
    public UnityEngine.AudioClip audioClip;
    public bool looping;
    public string path;

    Sound(string _name, UnityEngine.AudioClip soundClip, bool loop)
    {
        name = _name;
        audioClip = soundClip;
        looping = loop;
        path = UnityEditor.AssetDatabase.GetAssetPath(soundClip);
    }

    public void AutoSetPath()
    {
        if (audioClip != null)
        {
            path = UnityEditor.AssetDatabase.GetAssetPath(audioClip);
        }
    }
}
