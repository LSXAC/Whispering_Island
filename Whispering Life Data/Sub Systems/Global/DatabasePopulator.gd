@tool
extends Node

class_name DatabasePopulator

# ----- Editor-Checkbox -----
func _get_property_list() -> Array:
	if not Engine.is_editor_hint():
		return []
	
	return [
		{
			"name": "Populate Database",
			"type": TYPE_BOOL,
			"usage": PROPERTY_USAGE_EDITOR | PROPERTY_USAGE_CHECKABLE
		}
	]

func _set(property: StringName, value: Variant) -> bool:
	if property == "Populate Database" and value == true:
		call_deferred("_populate_database")
		return true
	return false

func _populate_database() -> void:
	populate_database()

# ----- Datenbank füllen -----
func populate_database() -> void:
	if not Engine.is_editor_hint():
		return
	
	var database = get_parent()
	if database == null:
		push_error("DatabasePopulator: No parent node found!")
		return
	
	print("DatabasePopulator: Starting to scan resources...")
	
	var items: Array = []
	var researches: Array = []
	var buildings: Array = []
	var recipes: Array = []
	
	scan_directory("res://", items, researches, buildings, recipes)
	
	database.item_info_list = items
	database.item_research_list = researches
	database.building_menu_list_objects = buildings
	database.crafting_recipies_list = recipes
	
	print("DatabasePopulator: Done!")
	print("  - Items: %d" % items.size())
	print("  - Researches: %d" % researches.size())
	print("  - Buildings: %d" % buildings.size())
	print("  - Recipes: %d" % recipes.size())
	print("DatabasePopulator: Save the scene to keep changes.")

# ----- Verzeichnis rekursiv durchsuchen -----
func scan_directory(path: String, items: Array, researches: Array, buildings: Array, recipes: Array) -> void:
	var dir = DirAccess.open(path)
	if dir == null:
		print("DatabasePopulator: Cannot open directory: ", path)
		return
	
	dir.list_dir_begin()
	var file_name = dir.get_next()
	
	while file_name != "":
		# versteckte Dateien ignorieren
		if file_name.begins_with("."):
			file_name = dir.get_next()
			continue
		
		var full_path = path.path_join(file_name)
		
		if dir.dir_exists(full_path):
			# rekursiv Unterordner prüfen
			scan_directory(full_path, items, researches, buildings, recipes)
		elif file_name.ends_with(".tres"):
			# Ausgabe jedes Tres-Files
			print("DatabasePopulator: Found file -> ", full_path)
			check_and_load_resource(full_path, items, researches, buildings, recipes)
		
		file_name = dir.get_next()

# ----- Prüft jede Ressource auf die gewünschten Klassen -----
func check_and_load_resource(path: String, items: Array, researches: Array, buildings: Array, recipes: Array) -> void:
	var res = load(path)
	if res == null:
		print("DatabasePopulator: Failed to load resource: ", path)
		return

	if res is ItemInfo:
		if res.id != 0:
			items.append(res)
			print("  -> Loaded ItemInfo: ", path)
	elif res is ItemResearch:
		if res.id != 0:
			researches.append(res)
			print("  -> Loaded ItemResearch: ", path)
	elif res is Building_Menu_List_Object:
		if res.scene_building_id != 0:
			buildings.append(res)
			print("  -> Loaded Building_Menu_List_Object: ", path)
	elif res is CraftingRecipe:
		recipes.append(res)
		print("  -> Loaded CraftingRecipe: ", path)
