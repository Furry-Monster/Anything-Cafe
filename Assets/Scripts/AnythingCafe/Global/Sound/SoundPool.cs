using UnityEngine;

public class SoundPool : Singleton<SoundPool>
{
    public SoundPool Initialize(GameObject SourceParent)
    {
        return Instance;
    }
}
