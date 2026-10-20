using System.Collections.Generic;

namespace Extensions.Logic
{
    /** <summary>How a composite condition combines its parts.</summary> */
    public enum CompositeRule
    {
        /** <summary>Holds when every part holds.</summary> */
        All,
        /** <summary>Holds when any one part holds.</summary> */
        Any,
        /** <summary>Holds when its one part doesn't.</summary> */
        Not,
    }

    /** <summary>A condition made of other conditions, such as And, Or and Not, so tools can walk the whole tree.</summary> */
    public interface ICompositeCondition<TContext>
    {
        /** <summary>Its parts; a slot may be empty.</summary> */
        IReadOnlyList<ICondition<TContext>> Children { get; }

        /** <summary>How it combines them.</summary> */
        CompositeRule Rule { get; }
    }
}
