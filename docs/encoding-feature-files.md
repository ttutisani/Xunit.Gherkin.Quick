# Feature File character encoding

An [issue](https://github.com/ttutisani/Xunit.Gherkin.Quick/issues/82) was logged claiming that feature files and corresponding tests would not be *discovered* if the feature file contained hidden unicode characters.

This proved to be a time consuming problem for the user. The root of the problem was a consequence of copy-pasting from a PDF directly into a new feature file. All other feature files and tests would run, but the new tests were non-discoverable. Even when the erroneous feature file had been stripped right back to a simple *Hello World* test, the problem persisted.

The advice is to avoid copy-pasting from PDFs, Word docs etc. And, if having similar problems, copy a known good feature file, get it running, then apply new Scenarios one at a time.
