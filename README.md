# Unity-Extensions

Shared building blocks for Unity projects: the foundation the other Privnoval tools (Unity-StateMachine,
Unity-SaveSystem, Unity-QuestSystem) build on.

| Folder | What it holds |
|---|---|
| `EventBus/` | A typed global event bus |
| `Logic/` | Composable conditions (`ICondition<T>`, And / Or / Not), plus the facts conditions read (below) |
| `Ids/` | Stable asset ids, the asset catalog (below) |
| `Inspector/` | `[SubclassPicker]`, a type menu for `[SerializeReference]` fields (below) |
| `Modifiers/`, `Rules/`, `Timers/`, `Scheduling/`, `PushdownAutomata/`, `Singleton/`, `EntityComponent/`, `Patterns/`, `Utils/`, `Other/` | Modifiers and mediators, rules, timers, a priority queue, a stack state machine, singletons, an entity/component base, patterns, utilities |

Assemblies:

- `Extensions` (auto-referenced);
- `Extensions.Editor` (editor only: the id tooling and the type picker's drawer);
- `Extensions.Tests` (EditMode tests, compiled only with tests included).

## Conditions and facts

A condition is an `ICondition<TContext>`. Conditions shared across systems (quests, rewards, locks) are
`ICondition<IFacts>`:

- **`IFacts`** hands out fact sources by type: `facts.TryGet<IQuestFacts>(out var quests)`. Each module or game
  registers its own in a **`FactRegistry`**, so a condition can read quest state, story flags or the party without
  any module knowing about the others.
- **`IConditionSubject`** marks anything a condition reads, such as a flag or a quest. A condition that implements
  **`ISubjectReader`** says what it reads; And / Or / Not report their children's. `ConditionSubjects.TryCollect` asks.
- **Change-driven checks.** When a subject changes, the registry's `NotifyChanged(subject)` announces it, and only
  the conditions that read it need checking again. A condition that doesn't say what it reads is checked after any
  change, so custom conditions still work.
- **`ConditionWatcher`** watches one condition over a set of facts and calls back when its result changes, checking
  it only when something it reads changes. Locks, reactions and gated rewards all use it; dispose it when done.
- **One moment's facts.** `Facts.Of<IDefeatedFoe>(foe)` makes a set holding one fact source, and
  `Facts.Layered(top, bottom)` reads the top set first, then the bottom one. This is how an event's own details
  (which foe fell) reach a condition alongside the world's facts. `Facts.None` is the empty set.
- **And / Or / Not skip empty slots.** An empty And holds, an empty Or doesn't, and a Not with no child doesn't.
- **Walking a tree, for tools.** And / Or / Not are `ICompositeCondition<T>` (their parts and whether they need all,
  any or none of them). `ConditionTree.VisitLeaves` visits every other part with how it is used: negated or not, and
  required or one of several ways. `ConditionTree.Summarize` gives a one-line summary: a condition's `ToString`, or
  its type name when it has none. Write a short `ToString` on your conditions so tools can show them.
- **`NumberComparison`** (at least, equal, below) and `NumberComparisons.Holds` serve conditions on counts, levels and
  flags.

## Asset ids

An asset that saves, quests or tools point at implements `IIdentifiedAsset` and holds one `AssetId` field:

```csharp
public sealed class ItemDefinition : ScriptableObject, IIdentifiedAsset
{
    [Tooltip("Filled in by the editor.")]
    [SerializeField] private AssetId assetId;

    public AssetId AssetId => assetId;
}
```

**The editor fills the id in**, with no typing:

- a new asset gets its file's GUID;
- a duplicated asset gets an id of its own, and the original keeps its id;
- an id once stored never changes, even if the asset's `.meta` file is regenerated.

**Each import loads only the changed files** and those sharing their ids, and the project is scanned once per
session. When a type gains `IIdentifiedAsset`, its existing assets get ids after the next compile.

**Builds find assets by id through an `AssetCatalog`** (*Create > Privnoval > Asset Ids > Asset Catalog*),
resolved through `IAssetResolver`. The editor keeps every catalog filled: on import, before every build, and from
*Tools > Privnoval > Asset Ids > Rebuild Asset Catalog*. The catalog references the assets, so they stay loaded. An
on-demand resolver (Addressables, say) can replace it behind `IAssetResolver`.

**The audit** (*Tools > Privnoval > Asset Ids > Audit Asset Ids*) checks that each identified asset:

- has one id field;
- has an id;
- has a unique id;
- is in every catalog.

A build fails on a missing or shared id. *Tools > Privnoval > Asset Ids > Fix Asset Ids* fixes them all now.

**In data,** an id converts to and from its hex text through a type converter, so serializers that honour type
converters (Newtonsoft does) write it as text with no setup.

## Menus

Every Privnoval tool puts its entries under one heading, under its own name, the same way in all three menus:

| Menu | Path | Holds |
|---|---|---|
| Tools | *Tools > Privnoval > (tool)* | Windows first, then commands after a separator |
| Create | *Create > Privnoval > (tool)* | Its assets |
| Add Component | *Add Component > Privnoval > (tool)* | Its components. Components that code adds itself are hidden |

`ToolMenus` holds the heading and the two priorities (`Windows`, `Commands`). Each tool keeps its own paths in one
small class built from it (`AssetIdMenus`, `QuestMenus`, `SaveMenus`), so no path is typed twice.

## The type picker

Put `[SerializeReference, SubclassPicker]` on a field of an interface or base type to choose its concrete type from a
menu in the Inspector.

- **Generic types** are offered closed over the field's type arguments, so an `ICondition<IFacts>` field offers
  `AndCondition<IFacts>`.
- **With Odin installed,** Odin's own picker is used and this drawer is left out.
