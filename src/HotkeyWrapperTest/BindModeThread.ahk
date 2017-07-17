#NoEnv
#Persistent

bh := new BindHandler()

class BindHandler {
	DebugMode := 1
	DetectionState := 0
	
	__New(ptr){
		this.ptr := ptr
		this.SetDetectionState(0)
		this.CreateHotkeys()
		;this.SetDetectionState(1)
	}
	
	SetDetectionState(state){
		;MsgBox % "AHK| " state
		if (state){
			Suspend, Off
		} else {
			Suspend, On
		}
		this.DetectionState := state
		return true
	}
	
	; Binds a key to every key on the keyboard and mouse
	; Passes VK codes to GetKeyName() to obtain names for all keys
	; List of VKs: https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx
	; Keys are stored in the settings file by VK number, not by name.
	; AHK returns non-standard names for some VKs, these are patched to Standard values
	; Numpad Enter appears to have no VK, it is synonymous with Enter (VK0xD). Seeing as VKs 0xE to 0xF are Undefined by MSDN, we use 0xE for Numpad Enter.
	CreateHotkeys(){
		static replacements := {33: "PgUp", 34: "PgDn", 35: "End", 36: "Home", 37: "Left", 38: "Up", 39: "Right", 40: "Down", 45: "Insert", 46: "Delete"}
		static pfx := "$*"
		static updown := [{e: 1, s: ""}, {e: 0, s: " up"}]
		; Cycle through all keys / mouse buttons
		Loop 256 {
			; Get the key name
			i := A_Index
			code := Format("{:x}", i)
			if (ObjHasKey(replacements, i)){
				n := replacements[i]
			} else {
				n := GetKeyName("vk" code)
			}
			if (n = "")
				continue
			; Down event, then Up event
			Loop 2 {
				blk := this.DebugMode = 2 || (this.DebugMode = 1 && i <= 2) ? "~" : ""
				fn := this.InputEvent.Bind(this, updown[A_Index].e, i)
				hotkey, % pfx blk n updown[A_Index].s, % fn, % "On"
			}
		}
		i := 14, n := "NumpadEnter"	; Use 0xE for Nupad Enter
		Loop 2 {
			blk := this.DebugMode = 2 || (this.DebugMode = 1 && i <= 2) ? "~" : ""
			fn := this.InputEvent.Bind(this, updown[A_Index].e, i)
			hotkey, % pfx blk n updown[A_Index].s, % fn, % "On"
		}
	}
	
	InputEvent(e, i){
		;OutputDebug % "UCR| BindMode KBM IO Event: " e ", Code: " i ", IOClass: " this.ReturnIOClass
		;~ MsgBox % "AHK| BindMode Handler Firing Callback"
		DllCall(this.ptr, "Uint", e, "Uint", i)
	}
}