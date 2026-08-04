namespace Extensions.Other
{
    /**
     * <summary>
     * Something that needs driving by the Unity loop, without caring who drives it.
     * <para>
     * <see cref="MonoBehaviourUpdatable" /> implements this by attaching its own
     * <see cref="MonoBehaviourUpdater" />, which is the convenient default. Where tick order matters
     * relative to other systems, a consumer can instead implement this directly, or construct a
     * <see cref="MonoBehaviourUpdatable" /> with no GameObject, and call the methods itself at the
     * exact point in its own pipeline that it needs them.
     * </para>
     * </summary>
     */
    public interface ITickable
    {
        /** <summary>Called once per rendered frame.</summary> */
        void Update();

        /** <summary>Called once per physics step.</summary> */
        void FixedUpdate();

        /** <summary>Called once per rendered frame, after all Update calls.</summary> */
        void LateUpdate();
    }
}
