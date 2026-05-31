# JMA

-> 80 colonnes > args sur une seule ligne ? / Nombre max d'args ?
---> cf `IniFile.Reader` => mérite `static` mais du coup beaucoup d'args (ou private inner class pour éviter de passer 1000 args ?)
---> cf `IniFile.Reader` => Messages d'exception super longs mais pas envie de dupliqué le `filePath` dans `State`...?
-> Bon usage de inner class dans `ConnectionManager` (inner class == `Connection`) ?

---

1. Enums / Inner class
2. Attributs statiques
3. Attributs public
4. Attributs private
5. Constructor
