Feature: Placeholders Feature
  
Scenario Outline: Placeholders in DataTables are replaced
    Given I have supplied a DataTable with
        | MealExtra   |
        | <Fruit>     |
    When I execute a scenario outline
    Then the DataTables MealExtra column should contain <Fruit>
    Examples:
        | Fruit |
        | Apple |
        | Pear  |

Scenario Outline: Placeholders in DocStrings are replaced
  Given I have supplied a DocString with
    """
    <Date> Delicious <Meal> plan:
    The main dish for <Date> is: <MainDish>!
    Additionally prepare a <Fruit>-based smoothie with some <Addition>.
    Optionally add a cup of hot <HotDrink>? <HotDrinkNeeded>.
    Total portions needed: <MyPortions>+<OtherPortions>+<Provision>! 
    """
  When I execute a scenario outline
  Then the DocString should contain a <Meal> plan for <Date> for <MyPortions> plus <OtherPortions> portions and <Provision> extra, containing <MainDish>, an optional hot drink <HotDrinkNeeded> <HotDrink> and a description for a smoothie of a <Fruit> and <Addition> combo
  Examples:
    | Date          | Meal      | MainDish            | HotDrinkNeeded | HotDrink | Fruit  | Addition | MyPortions | OtherPortions | Provision |
    | 07.03.2024    | breakfast | omelette            | Yes            | coffee   | orange | mint     | 1          | 1             | 0         |
    | 07.03.2024    | lunch     | steak               | No             | none     | pear   | cocoa    | 1          | 0             | 2         |
    | 08.03.2024    | lunch     | omelette            | Yes            | tea      | apple  | mint     | 1          | 2             | 0         |
    | Sundays       | breakfast | yogurt with berries | Yes            | coffee   | banana | protein  | 2          | 1             | 0         |
    | any other day | breakfast | "french" omelette   | Yes            | tea      | orange | mint     | 1          | 1             | 1         |
