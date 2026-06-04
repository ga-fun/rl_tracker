# JMA

## QUESTIONS

- Certificat
- 😱 `Network.Connection`:
	- devenu un peu huge (300 lignes 🙈) à cause des `{}`, des `catch` et des `gate` => ça reste good ?
	- Bon usage de inner class dans `Connection` (inner class == `State`) ?
- Big classes:
	- `Connection`
	- `ApiEventHandler`

## REPONSES

Ordre:
1. Enums / Inner class
2. Attributs statiques
3. Attributs public
4. Attributs private
5. Constructor
