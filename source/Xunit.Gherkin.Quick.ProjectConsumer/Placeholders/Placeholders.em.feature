
# language: em

📚: Placeholders Feature
  
📖: Placeholders in DataTables are replaced
    😐 I have supplied a DataTable with
        | MealExtra   |
        | <Fruit>     |
    🎬 I execute a scenario outline
    🙏 the DataTables MealExtra column should contain <Fruit>
    📓:
        | Fruit |
        | Apple |
        | Pear  |