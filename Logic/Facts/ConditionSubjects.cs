using System.Collections.Generic;

namespace Extensions.Logic
{
    /** <summary>Something a condition can read, such as a story flag or a quest. When it changes, conditions reading it are checked again.</summary> */
    public interface IConditionSubject { }

    /** <summary>A condition that says which subjects it reads.</summary> */
    public interface ISubjectReader
    {
        /**
         * <summary>Adds every subject this condition reads.</summary>
         * <returns>False when some part can't say, so the condition must be checked after any change.</returns>
         */
        bool TryCollectSubjects(ICollection<IConditionSubject> subjects);
    }

    /** <summary>The one place that asks a condition what it reads.</summary> */
    public static class ConditionSubjects
    {
        /**
         * <summary>Adds every subject a condition reads.</summary>
         * <returns>False when the condition, or part of it, doesn't say: check it after any change.</returns>
         */
        public static bool TryCollect<TContext>(ICondition<TContext> condition, ICollection<IConditionSubject> subjects) =>
            condition == null || (condition is ISubjectReader reader && reader.TryCollectSubjects(subjects));

        /** <summary>Adds the subjects of several conditions.</summary> <returns>False when any of them doesn't say.</returns> */
        public static bool TryCollectAll<TContext>(IEnumerable<ICondition<TContext>> conditions, ICollection<IConditionSubject> subjects)
        {
            var known = true;
            if (conditions == null) return true;
            foreach (var condition in conditions)
                known &= TryCollect(condition, subjects);
            return known;
        }
    }
}
