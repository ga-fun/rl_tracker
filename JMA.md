# JMA

## Projets

> Séparation `solution` -> `projet` -> `classe` existe aussi en **JAVA** ?
> Avis sur la séparation des **projets** `FileIni` vs `RlStatsApi` vs `RlTracker.Core` vs `RlTracker.WpfManager` ?

## Classes et sous-classes

> Quid des classes horizontales vs verticales ?
- Aujourd'hui : `Driver` - `ConnectionManager` - `Client`
- Idéeal (?) :
```
Driver
 └ Connection Manager
	└ Client
```

> Surtout pour les `Models` : d'après le Chat : inner class si "ne fait de sens qu'au sein de la classe" => Donc tous les models sont des inner class de `State` ?!

## Classes "énormes"

> 🤮 Exemple classe volumineuse : `RlTracker.Core` -> `Config` : split recherche Epic/Steam dans des classes statiques ?
> 🤮 Exemple beaucoup de classes : `RlStatsApi` -> `Payloads` : sous-classes au lieu de classes ?
> 🤔 Exemple d'inner class : `RlTracker.Core` -> `ConnectionManager` -> `Connection` : bon usage ?

## Verbosité des `try`/`catch`

> Exemple : `ConnectionLoopAsync()` (> 50 lignes !)
> Comment tu fais en Java pour éviter les `try`/`catch` à rallonge ?

> Mettre les `try`/`catch` dans des **handlers** dédiés ?!
Par exemple créer `SafeCloseAsync(Client client)` pour remplacer :
```cs
try
{
	await client.CloseAsync(CancellationToken.None);
}
catch
{}
```

## Utilitaires

> Quid des fonctions / helpers qui n'ont pas de place évidente
- cf `RlProcess.cs` juste pour `RlIsRunning()` ..?
- cf `Driver.Config` -> `UnsafeUpdateConfigAsync()` ?

## Lisibilité

> **Commentaires** pour séparer visuellement [attributs / méthodes], [public / private], ... ?
> Ou plutôt **inner class** (cf `RlTracker.Core` dans `ConnectionManager` -> `Connection`) ?
> cf `RlTracker.Core` -> `Config` : attributs sont un peu illisibles 🙈
