using System;

namespace Extensions.Logic
{
    /**
     * <summary>What conditions read: the fact sources a game and its modules register (quest state, flags, the party),
     * looked up by type, and a signal when something conditions read has changed.</summary>
     */
    public interface IFacts
    {
        /** <summary>Raised when a subject conditions can read has changed, so conditions reading it can be checked again.</summary> */
        event Action<IConditionSubject> OnSubjectChanged;

        /** <summary>Finds the registered fact source of a type.</summary> <returns>False when none is registered.</returns> */
        bool TryGet<T>(out T facts) where T : class;
    }
}
