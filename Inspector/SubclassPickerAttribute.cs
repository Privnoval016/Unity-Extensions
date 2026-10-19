using UnityEngine;

namespace Extensions.Inspector
{
    /**
     * <summary>Put on a <c>[SerializeReference]</c> field to choose its concrete type from a menu in the Inspector.
     * With Odin installed, Odin's own picker is used instead.</summary>
     */
    public sealed class SubclassPickerAttribute : PropertyAttribute { }
}
