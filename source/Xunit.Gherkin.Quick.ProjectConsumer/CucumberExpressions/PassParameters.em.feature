
# language: em

📚: pass parameters
  In order to simplify use of regex
  I want to use cucumber expressions in feature steps


📕: Pass positive integer to step function
  🎬 I pass the integer value 10 using cucumber expression
  🙏 the received integer value is 10 using regex


📕: Pass negative integer to step function
  🎬 I pass the integer value -22 using cucumber expression
  🙏 the received integer value is -22 using regex


📕: Pass integer zero to step function
  🎬 I pass the integer value 0 using cucumber expression
  🙏 the received integer value is 0 using regex


📕: Pass positive float to step function
  🎬 I pass the float value 10.25 using cucumber expression
  🙏 the received float value is 10.25 using regex


📕: Pass negative float to step function
  🎬 I pass the float value -20.25 using cucumber expression
  🙏 the received float value is -20.25 using regex


📕: Pass float zero to step function
  🎬 I pass the float value 0 using cucumber expression
  🙏 the received float value is 0 using regex


📕: Pass float zero dot zero to step function
  🎬 I pass the float value 0.0 using cucumber expression
  🙏 the received float value is 0.0 using regex


📕: Pass word to step function
  🎬 I pass the word Max using cucumber expression
  🙏 the received word is Max using regex


📕: Pass double quoted string to step function
  🎬 I pass the string "11 and A B C" using cucumber expression
  🙏 the received string is "11 and A B C" using regex


📕: Pass double quoted string with single quotes to step function
  🎬 I pass the string "11 and 'A' or 'B' or 'C'" using cucumber expression
  🙏 the received string is "11 and 'A' or 'B' or 'C'" using regex


📕: Pass empty string to step function
  🎬 I pass the string "" using cucumber expression
  🙏 the received string is "" using regex


📕: Pass single quoted string to step function
  🎬 I pass the string '11 and A B C' using cucumber expression
  🙏 the received string is "11 and A B C" using regex


📕: Pass single quoted string with double quotes to step function
  🎬 I pass the string '11 and "A" and "B" and "C"' using cucumber expression
  🙏 the received string is '11 and "A" and "B" and "C"' using single quotes regex


📕: Pass single quoted empty string to step function
  🎬 I pass the string '' using cucumber expression
  🙏 the received string is "" using regex


📕: Pass anything to step function
  🎬 I say Quasimodo the Grand
  🙏 it is the same as "Quasimodo the Grand"