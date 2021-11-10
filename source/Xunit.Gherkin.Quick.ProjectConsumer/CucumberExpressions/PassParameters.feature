Feature: pass parameters
  In order to simplify use of regex
  I want to use cucumber expressions in feature steps


Scenario: Pass positive integer to step function
  When I choose the integer value 10
  Then the triple value is 30


Scenario: Pass negative integer to step function
  When I choose the integer value -22
  Then the triple value is -66


Scenario: Pass integer zero to step function
  When I choose the integer value 0
  Then the triple value is 0


Scenario: Pass positive float to step function
  When I choose the float value 10.25
  Then 10.75 more is 21.00


Scenario: Pass negative float to step function
  When I choose the float value -20.25
  Then 10.75 more is -9.50


Scenario: Pass float zero to step function
  When I choose the float value 0.0
  Then 10.75 more is 10.75


Scenario: Pass word to step function
  Given the first word is Max
  When I choose the second word Motor
  Then the concatenation of them is MaxMotor


Scenario: Pass string to step function
  When I choose the string "A and B and C"
  Then the lower case string is "a and b and c"


Scenario: Pass empty string to step function
  When I choose the string ""
  Then the lower case string is ""

Scenario: Pass anything to step function
  When I say Quasimodo the Grand
  Then it is the same as "Quasimodo the Grand"