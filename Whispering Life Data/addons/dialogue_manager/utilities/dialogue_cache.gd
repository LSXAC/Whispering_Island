@tool
class_name DMCache
extends RefCounted

# -------------------------------
# Editor guard
# -------------------------------
static var _IS_EDITOR := Engine.is_editor_hint()

# -------------------------------
# Cache & helper variables
# -------------------------------
static var _cache: Dictionary = {}
static var _update_dependency_timer
static var _update_dependency_paths: PackedStringArray = []
static var _files_marked_for_reimport: PackedStringArray = []
static var known_static_ids: Dictionary = {}

# -------------------------------
# Public API
# -------------------------------

# Build initial cache
static func prepare() -> void:
	if not _IS_EDITOR:
		return

	# Timer for deferred dependency updates
	_update_dependency_timer = Timer.new()
	_update_dependency_timer.one_shot = true
	_update_dependency_timer.timeout.connect(_on_dependency_timer_timeout)

	# Add timer to DMPlugin if it exists
	if Engine.has_singleton("DMPlugin"):
		var plugin = Engine.get_singleton("DMPlugin")
		if plugin:
			plugin.add_child(_update_dependency_timer)

	# Find all dialogue files
	var current_files = _get_dialogue_files_in_filesystem()
	for file in current_files:
		add_file(file)

	# Scan for static IDs
	var key_regex = RegEx.create_from_string("\\[ID:(?<key>.*?)\\]")
	for file_path in get_files():
		var text = FileAccess.get_file_as_string(file_path)
		for line in text.split("\n"):
			var found = key_regex.search(line)
			if found:
				known_static_ids[found.strings[found.names.key]] = file_path

# Mark files for reimport
static func mark_files_for_reimport(files: PackedStringArray) -> void:
	if not _IS_EDITOR:
		return
	for file in files:
		if not _files_marked_for_reimport.has(file):
			_files_marked_for_reimport.append(file)

# Reimport files safely
static func reimport_files(and_files: PackedStringArray = []) -> void:
	if not _IS_EDITOR:
		return

	for file in and_files:
		if not _files_marked_for_reimport.has(file):
			_files_marked_for_reimport.append(file)

	if _files_marked_for_reimport.is_empty():
		return

	# Safe access to EditorInterface
	if Engine.has_singleton("EditorInterface"):
		var filesystem = Engine.get_singleton("EditorInterface").get_resource_filesystem()
		if filesystem and not filesystem.is_scanning():
			filesystem.reimport_files(_files_marked_for_reimport)
			_files_marked_for_reimport.clear()
		else:
			_schedule_deferred_reimport.call_deferred()

# Add a dialogue file to cache
static func add_file(path: String, compile_result = null) -> void:
	_cache[path] = {
		"path": path,
		"dependencies": [],
		"errors": []
	}

	if compile_result != null:
		_cache[path].dependencies = PackedStringArray(Array(compile_result.imported_paths).filter(func(d): return d != path))
		_cache[path].compiled_at = Time.get_ticks_msec()

	queue_updating_dependencies(path)

# Accessors
static func get_files() -> PackedStringArray:
	return PackedStringArray(_cache.keys())

static func has_file(path: String) -> bool:
	return _cache.has(path)

static func add_errors_to_file(path: String, errors: Array) -> void:
	if not _cache.has(path):
		_cache[path] = {
			"path": path,
			"dependencies": [],
			"errors": errors
		}
	else:
		_cache[path].errors = errors

static func get_files_with_errors() -> Array:
	var result: Array = []
	for data in _cache.values():
		if data.errors.size() > 0:
			result.append(data)
	return result

# Queue dependency updates
static func queue_updating_dependencies(of_path: String) -> void:
	if not _IS_EDITOR:
		return
	if not _update_dependency_paths.has(of_path):
		_update_dependency_paths.append(of_path)
	_update_dependency_timer.start(0.5)

# Move file path in cache
static func move_file_path(from_path: String, to_path: String) -> void:
	if not _cache.has(from_path):
		return
	if to_path != "":
		_cache[to_path] = _cache[from_path].duplicate(true)
	_cache.erase(from_path)

# Get files that depend on a given path
static func get_files_with_dependency(imported_path: String) -> Array:
	return _cache.values().filter(func(d): return d.dependencies.has(imported_path))

# Get dependent paths needing reimport
static func get_dependent_paths_for_reimport(on_path: String) -> PackedStringArray:
	return get_files_with_dependency(on_path).filter(
		func(d): return Time.get_ticks_msec() - d.get("compiled_at", 0) > 3000
	).map(func(d): return d.path)

# -------------------------------
# Editor helper functions
# -------------------------------
static func _schedule_deferred_reimport() -> void:
	if not _IS_EDITOR:
		return
	if _files_marked_for_reimport.is_empty():
		return

	if Engine.has_singleton("EditorInterface"):
		var filesystem = Engine.get_singleton("EditorInterface").get_resource_filesystem()
		if filesystem.is_scanning():
			await Engine.get_main_loop().create_timer(0.1).timeout
			_schedule_deferred_reimport()
			return
		filesystem.reimport_files(_files_marked_for_reimport)
		_files_marked_for_reimport.clear()

# Recursively find dialogue files
static func _get_dialogue_files_in_filesystem(path: String = "res://") -> PackedStringArray:
	var files := PackedStringArray()
	if not DirAccess.dir_exists_absolute(path):
		return files
	var dir = DirAccess.open(path)
	dir.list_dir_begin()
	var file_name = dir.get_next()
	while file_name != "":
		var file_path = (path + "/" + file_name).simplify_path()
		if dir.current_is_dir():
			if file_name not in [".godot", ".tmp"]:
				files.append_array(_get_dialogue_files_in_filesystem(file_path))
		elif file_name.get_extension() == "dialogue":
			files.append(file_path)
		file_name = dir.get_next()
	return files

# Dependency timer callback
static func _on_dependency_timer_timeout() -> void:
	if not _IS_EDITOR:
		return

	var import_regex = RegEx.create_from_string("import \"(?<path>.*?)\"")
	for path in _update_dependency_paths:
		var file = FileAccess.open(path, FileAccess.READ)
		if not file:
			continue
		var found_imports = import_regex.search_all(file.get_as_text())
		var dependencies = PackedStringArray()
		for found in found_imports:
			dependencies.append(found.strings[found.names.path])
		_cache[path].dependencies = dependencies
	_update_dependency_paths.clear()
