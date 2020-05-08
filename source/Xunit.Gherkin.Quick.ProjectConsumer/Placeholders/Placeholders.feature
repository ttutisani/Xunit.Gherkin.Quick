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