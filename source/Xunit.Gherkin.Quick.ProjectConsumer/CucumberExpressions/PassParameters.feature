Feature: pass parameters
  In order to simplify use of regex
  I want to use cucumber expressions in feature steps


Scenario: Pass positive integer to step function
  When I pass the integer value 10 using cucumber expression
  Then the received integer value is 10 using regex


Scenario: Pass negative integer to step function
  When I pass the integer value -22 using cucumber expression
  Then the received integer value is -22 using regex


Scenario: Pass integer zero to step function
  When I pass the integer value 0 using cucumber expression
  Then the received integer value is 0 using regex


Scenario: Pass positive float to step function
  When I pass the float value 10.25 using cucumber expression
  Then the received float value is 10.25 using regex


Scenario: Pass negative float to step function
  When I pass the float value -20.25 using cucumber expression
  Then the received float value is -20.25 using regex


Scenario: Pass float zero to step function
  When I pass the float value 0 using cucumber expression
  Then the received float value is 0 using regex


Scenario: Pass float zero dot zero to step function
  When I pass the float value 0.0 using cucumber expression
  Then the received float value is 0.0 using regex


Scenario: Pass word to step function
  When I pass the word Max using cucumber expression
  Then the received word is Max using regex


Scenario: Pass double quoted string to step function
  When I pass the string "11 and A B C" using cucumber expression
  Then the received string is "11 and A B C" using regex


Scenario: Pass double quoted string with single quotes to step function
  When I pass the string "11 and 'A' or 'B' or 'C'" using cucumber expression
  Then the received string is "11 and 'A' or 'B' or 'C'" using regex


Scenario: Pass empty string to step function
  When I pass the string "" using cucumber expression
  Then the received string is "" using regex


Scenario: Pass single quoted string to step function
  When I pass the string '11 and A B C' using cucumber expression
  Then the received string is "11 and A B C" using regex


Scenario: Pass single quoted string with double quotes to step function
  When I pass the string '11 and "A" and "B" and "C"' using cucumber expression
  Then the received string is '11 and "A" and "B" and "C"' using single quotes regex


Scenario: Pass single quoted empty string to step function
  When I pass the string '' using cucumber expression
  Then the received string is "" using regex


Scenario: Pass anything to step function
  When I say Quasimodo the Grand
  Then it is the same as "Quasimodo the Grand"