extends CenterContainer

func _ready():
	OS.window_maximized = true

func _on_StartGame_pressed():
# warning-ignore:return_value_discarded
	get_tree().change_scene("res://MainMenu.scn")