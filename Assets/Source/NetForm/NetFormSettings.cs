using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace NetDive.NetForm
{
    [CreateAssetMenu]
    public class NetFormSettings : ScriptableObject
    {
        public float sourceRange = 5f;
        public List<Sprite> netFormIcons = new();

        [ColorUsage(hdr: true, showAlpha: false)]
        public List<Color> netFormColors = new();

        public LayerMask netFormLayer = 1 << 8;

        public Sprite GetIcon(NetFormType type)
        {
            return type == NetFormType.None ? null : netFormIcons[(int)type];
        }
    }
}