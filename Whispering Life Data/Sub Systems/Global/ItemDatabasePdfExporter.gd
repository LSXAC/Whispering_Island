@tool
extends Node

class_name ItemDatabasePdfExporter

const EXPORT_PATH = "user://ItemDatabase.html"
const ASSETS_DIR = "user://ItemDatabase_assets/"

var texture_map = {}

func _get_property_list():
	if not Engine.is_editor_hint():
		return []
	
	return [
		{
			"name": "Export Items to PDF",
			"type": TYPE_BOOL,
			"usage": PROPERTY_USAGE_EDITOR
		},
		{
			"name": "Open Exported PDF",
			"type": TYPE_BOOL,
			"usage": PROPERTY_USAGE_EDITOR
		}
	]

func _set(property, value):
	if property == "Export Items to PDF" and value:
		call_deferred("export_items")
		return true
	elif property == "Open Exported PDF" and value:
		call_deferred("open_pdf")
		return true
	return false

func export_items():
	print("ItemDatabasePdfExporter: Starting export...")
	
	var database = get_parent()
	if not database:
		print("Error: Parent is not Database")
		return
	
	var items = null
	if "item_info_list" in database:
		items = database.item_info_list
	else:
		print("Error: Database has no item_info_list")
		return
	
	if not items or items.is_empty():
		print("Error: No items to export")
		return
	
	# Sort items by name
	var sorted_items = items.duplicate()
	sorted_items.sort_custom(func(a, b):
		if not a or not b:
			return false
		var name_a = _safe_get_item_name(a)
		var name_b = _safe_get_item_name(b)
		if name_a == null or name_b == null:
			return false
		return name_a < name_b
	)
	
	# Copy textures
	texture_map.clear()
	_copy_textures(sorted_items)
	
	# Generate HTML
	var html = _generate_html(sorted_items)
	if not html:
		print("Error: HTML generation failed")
		return
	
	# Save to file
	var path = ProjectSettings.localize_path(EXPORT_PATH)
	var file = FileAccess.open(path, FileAccess.WRITE)
	if file:
		file.store_string(html)
		print("Export complete: ", path)
	else:
		print("Error: Could not write file")

func open_pdf():
	var path = ProjectSettings.localize_path(EXPORT_PATH)
	if not FileAccess.file_exists(path):
		print("Error: PDF not found at ", path)
		return
	
	var os_name = OS.get_name()
	if os_name == "Windows":
		OS.shell_open(path)
	elif os_name == "X11":
		OS.execute("xdg-open", [path])
	elif os_name == "macOS":
		OS.execute("open", [path])
	else:
		print("OS not supported: ", os_name)

func _copy_textures(items):
	var dir = DirAccess.open(ProjectSettings.localize_path(ASSETS_DIR).get_base_dir())
	if dir:
		dir.make_dir(ProjectSettings.localize_path(ASSETS_DIR))
	
	for item in items:
		if not item:
			continue
		
		var texture = null
		if "texture" in item:
			texture = item.texture
		
		if not texture:
			continue
		
		var orig_path = null
		if "resource_path" in texture:
			orig_path = texture.resource_path
		
		if not orig_path or texture_map.has(orig_path):
			continue
		
		var filename = orig_path.get_file()
		var new_path = ASSETS_DIR + filename
		var local_new_path = ProjectSettings.localize_path(new_path)
		
		if DirAccess.copy_absolute(orig_path, local_new_path) == OK:
			texture_map[orig_path] = "ItemDatabase_assets/" + filename

func _generate_html(items):
	var html = "<!DOCTYPE html>\n<html>\n<head>\n"
	html += "<meta charset='UTF-8'>\n"
	html += "<title>Item Database</title>\n"
	html += _generate_css()
	html += "</head>\n<body>\n"
	html += "<div class='container'>\n"
	html += "<h1>Item Database</h1>\n"
	html += "<div class='grid'>\n"
	
	for item in items:
		if item:
			html += _generate_item_card(item)
	
	html += "</div>\n</div>\n</body>\n</html>"
	return html

func _generate_css():
	var css = "<style>\n"
	css += "body { font-family: Arial, sans-serif; background: #f5f5f5; margin: 20px; }\n"
	css += ".container { max-width: 1200px; margin: 0 auto; background: white; padding: 20px; border-radius: 8px; }\n"
	css += ".grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(300px, 1fr)); gap: 20px; }\n"
	css += ".card { border: 1px solid #ddd; border-radius: 4px; padding: 15px; background: #f9f9f9; }\n"
	css += ".card-header { display: flex; gap: 10px; margin-bottom: 10px; }\n"
	css += ".icon { width: 50px; height: 50px; border: 1px solid #ccc; border-radius: 4px; display: flex; align-items: center; justify-content: center; }\n"
	css += ".icon img { max-width: 100%; max-height: 100%; }\n"
	css += ".name { font-weight: bold; font-size: 1.1em; }\n"
	css += ".desc { color: #666; margin: 10px 0; font-size: 0.9em; }\n"
	css += ".stats { display: flex; gap: 10px; flex-wrap: wrap; }\n"
	css += ".stat { background: #e0e0e0; padding: 5px 10px; border-radius: 3px; font-size: 0.85em; }\n"
	css += ".attrs { margin-top: 10px; padding-top: 10px; border-top: 1px solid #ddd; }\n"
	css += "@media print { body { background: white; } }\n"
	css += "</style>\n"
	return css

func _generate_item_card(item):
	if not item:
		return ""
	
	var card = "<div class='card'>\n"
	
	# Header with icon
	card += "<div class='card-header'>\n"
	var texture = null
	if "texture" in item:
		texture = item.texture
	
	if texture:
		var img_path = texture.resource_path
		if texture_map.has(img_path):
			img_path = texture_map[img_path]
		card += "<div class='icon'><img src='" + img_path + "'></div>\n"
	else:
		card += "<div class='icon'>?</div>\n"
	
	card += "<div><div class='name'>" + _safe_get_item_name(item) + "</div>\n"
	if "id" in item:
		var item_id = item.id
		if item_id:
			card += "<div style='font-size: 0.85em; color: #999;'>ID: " + str(item_id) + "</div>\n"
	card += "</div></div>\n"
	
	# Description
	if "description" in item:
		var desc = item.description
		if desc:
			var desc_str = str(desc)
			if not desc_str.is_empty():
				card += "<div class='desc'>" + desc_str + "</div>\n"
	
	# Stats
	card += "<div class='stats'>\n"
	if "value" in item:
		card += "<div class='stat'>Value: " + str(item.value) + "</div>\n"
	if "max_stackable_size" in item:
		card += "<div class='stat'>Stack: " + str(item.max_stackable_size) + "</div>\n"
	card += "</div>\n"
	
	# Attributes
	if "attributes" in item:
		var attrs = item.attributes
		if attrs and attrs.size() > 0:
			card += "<div class='attrs'><strong>Attributes:</strong><br/>\n"
			for attr in attrs:
				if attr:
					var attr_name = _get_attribute_name(attr)
					card += "- " + attr_name + "<br/>\n"
			card += "</div>\n"
	
	card += "</div>\n"
	return card

func _get_attribute_name(attr):
	if not attr:
		return "Unknown"
	
	# Try to get class_name from C# class
	var attr_class = attr.get_class()
	if attr_class and attr_class != "Resource":
		return attr_class
	
	# Try to get from script
	var script = attr.get_script()
	if script:
		var script_path = script.resource_path
		if script_path:
			# Extract class name from path, e.g., "ResourceAttribute.cs" -> "ResourceAttribute"
			var filename = script_path.get_file()
			var name_without_ext = filename.trim_suffix(".cs").trim_suffix(".gd")
			if name_without_ext:
				return name_without_ext
	
	return attr_class

func _safe_get_item_name(item):
	if not item:
		return "Unknown"
	if "name" in item and item.name:
		return str(item.name)
	return "Unknown"
