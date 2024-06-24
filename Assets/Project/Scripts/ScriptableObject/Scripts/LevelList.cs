using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(fileName = "Levels",menuName = "SO/AllLevels")]
    public class LevelList : ScriptableObject
    {
        public List<LevelData> Levels;
    }

