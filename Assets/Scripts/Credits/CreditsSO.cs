using System;
using System.Collections.Generic;
using UnityEngine;

namespace Credits
{
    [Serializable]
    public class CreditSection
    {
        public string title;
        public string[] members;
        public GameObject imagePrefab;
    }

    [CreateAssetMenu(menuName = "Credits/Data")]
    public class CreditsSO : ScriptableObject
    {
        public List<CreditSection> credits;
    }
}
