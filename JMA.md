# JMA

- Avast ?!
- 80 colonnes > args sur une seule ligne ? / Nombre max d'args ?
	- cf `IniFile.Reader` => mérite `static` mais du coup beaucoup d'args (ou private inner class pour éviter de passer 1000 args ?)
	-  cf `IniFile.Reader` => Messages d'exception super longs mais pas envie de dupliquer le `filePath` dans `State`...?
- 😱 `Network.Connection`:
	- devenu un peu huge (300 lignes 🙈) à cause des `{}`, des `catch` et des `gate` => ça reste good ?
	- Bon usage de inner class dans `Connection` (inner class == `State`) ?

---

1. Enums / Inner class
2. Attributs statiques
3. Attributs public
4. Attributs private
5. Constructor
