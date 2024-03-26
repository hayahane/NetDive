using UnityEngine;

namespace NetDive.NetForm
{
    public class NetFormSource : MonoBehaviour
    {
        [field: SerializeField] public NetFormType SourceType { get; private set; }

        [field: SerializeField] public NetFormInstance Instances { get; } = new();
    }
}