# language: nl

#
# Feature: Given When Then
#	In order to use Given When Then and other attributes
#	As a Gherkin enthusiast
#	I want to ensure they map to methods
#
# In order to validate if the language attribute works I translated this 
# "Feature" to dutch as it is the only other language I know. Maybe the Emoji 
# (em) language is a better test case as not all developers understand dutch 
# but everybody understand unicode glyphs.
#

Functionaliteit: Stel Als Dan
	Om "Stel", "Als", "Dan" en andrere attributen te gebruiken
	Als een Gherkin gebruiker
	Wil ik valideren dat deze een relatie hebben tot de correcte methode

	Scenario: Valideer de volgorde van de stappen
		Stel Monster tekst voor Stel
		En Monster tekst voor En na Stel
		Maar Monster tekst voor Maar na Stel
		Als Monster tekst voor Als
		En Monster tekst voor En na Als
		Maar Monster tekst voor Maar na Als
		Dan Monster tekst voor Dan
		En Monster tekst voor En na Dan
		Maar Monster tekst voor Maar na Dan

#
# Scenario: Ensure order of steps
#	Given Sample text for Given
#	And Sample text for And after Given
#	But Sample text for But after Given
#	When Sample text for When
#	And Sample text for And after When
#	But Sample text for But after When
#	Then Sample text for Then
#	And Sample text for And after Then
#	But Sample text for But after Then
#
