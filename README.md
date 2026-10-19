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

**Builds find assets by id through an `AssetCatalog`** (*Create > Asset Ids > Asset Catalog*), resolved through
`IAssetResolver`. The editor keeps every catalog filled: on import, before every build, and from
*Tools > Asset Ids > Rebuild Asset Catalog*. The catalog references the assets, so they stay loaded. An on-demand
resolver (Addressables, say) can replace it behind `IAssetResolver`.

**The audit** (*Tools > Asset Ids > Audit Asset Ids*) checks that each identified asset:

- has one id field;
- has an id;
- has a unique id;
- is in every catalog.

A build fails on a missing or shared id. *Tools > Asset Ids > Fix Asset Ids* fixes them all now.

## The type picker

Put `[SerializeReference, SubclassPicker]` on a field of an interface or base type to choose its concrete type from a
menu in the Inspector.

- **Generic types** are offered closed over the field's type arguments, so an `ICondition<IFacts>` field offers
  `AndCondition<IFacts>`.
- **With Odin installed,** Odin's own picker is used and this drawer is left out.
