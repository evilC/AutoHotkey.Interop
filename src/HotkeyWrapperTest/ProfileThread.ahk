#NoEnv
#Persistent

class ProfileHandler {
	DebugMode := 1
	DetectionState := 0
	
	Hotkeys := {}

	__New(ptr){
		this.ptr := ptr
	}
	
	SetDetectionState(state){
		if (state){
			Suspend, Off
		} else {
			Suspend, On
		}
		this.DetectionState := state
		return true
	}
	
	SetHotkey(hkName, i){
		if (this.Hotkeys.HasKey(hkName)){
			this.SetHotkeyState(hkName, this.Hotkeys[hkName], 0)
		}
		this.Hotkeys[hkName] := i
		this.SetHotkeyState(hkName, i, 1)
		return true
	}
	
	SetHotkeyState(hkName, i, state){
		static replacements := {33: "PgUp", 34: "PgDn", 35: "End", 36: "Home", 37: "Left", 38: "Up", 39: "Right", 40: "Down", 45: "Insert", 46: "Delete"}
		static pfx := "$*"
		static updown := [{e: 1, s: ""}, {e: 0, s: " up"}]
		
		code := Format("{:x}", i)
		if (ObjHasKey(replacements, i)){
			n := replacements[i]
		} else {
			n := GetKeyName("vk" code)
		}
		if (n = "")
			return false
		;~ msgbox % n
		Loop 2 {
			blk := this.DebugMode = 2 || (this.DebugMode = 1 && i <= 2) ? "~" : ""
			fn := this.InputEvent.Bind(this, hkName, updown[A_Index].e, i)
			hotkey, % pfx blk n updown[A_Index].s, % fn, % "On"
		}
	}
	
	InputEvent(hkName, e, i){
		;OutputDebug % "UCR| BindMode KBM IO Event: " e ", Code: " i ", IOClass: " this.ReturnIOClass
		;MsgBox % "AHK| Profile Handler Firing Callback for HK " hkName
		DllCall(this.ptr, "Str", hkName, "Uint", e, "Uint", i)
	}
}