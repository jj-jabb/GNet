-- Name: <Untitled>
-- Description: 
-- Language: Lua
-- Device: G13
-- LockForExecutables: False
-- Executables: 

poll = true

function WriteLcd(text)
	ClearGraphics()
	DrawString(text, 'Microsoft Sans Serif', 10, 0, '#FFFFFF', 0, 0)
end

-- NOTE: currently family is ALWAYS 'lhc' since the only currently supported
-- device is the G13 left-handed-controller.
function OnEvent(event, arg, family)
	if poll and (event == 'PROFILE_ACTIVATED' or event == 'M_RELEASED') then
		SetMKeyState(1)
		Sleep(25)
	end
	
	if event == 'PROFILE_DEACTIVATED' then
		OutputLogMessage('\r\nRan for '..GetRunningTime()..' milliseconds.')
	end
	
    if event == 'G_PRESSED' then
		if arg == 20 then
			ClearLog()
		end

        if arg == 4 then
        	WriteLcd('Forward (w)')
        	SetBacklightColor(255, 0, 0)
        	SetMLight(0) -- clear MLights
            PressKey('w')
        end
        if arg == 10 then
        	WriteLcd('Strafe Left (a)')
        	SetBacklightColorHtml('#50FF50')
        	SetMLight(1) -- M1
            PressKey('a')
        end
        if arg == 11 then
        	WriteLcd('Backward (s)')
        	SetBacklightColorHtml('#5050FF')
        	SetMLight(2) -- M2
            PressKey('s')
        end
        if arg == 12 then
        	WriteLcd('Strafe Right (d)')
        	SetBacklightColorHtml('#FFFFFF')
        	SetMLight(4) -- M4
            PressKey('d')
        end
        if arg == 22 then
        	WriteLcd('Jump (space)')
        	SetBacklightColorHtml('#000000') -- Backlight off (black)
        	SetMLight(8) -- MR
            PressKey('spacebar')
        end
		if arg == 9 then
        	WriteLcd('Walk (lshift)')
        	SetMLight(15) -- All MLights (all 4 bits set)
			PressKey('lshift')
		end
		if arg == 15 then
        	WriteLcd('Crouch (lctrl)')
			PressKey('lctrl')
		end
        if arg == 1 then
        	WriteLcd('Console (tilde)')
            PressKey('tilde')
        end
        if arg == 2 then
        	WriteLcd('Map (tab)')
            PressKey('tab')
        end
        if arg == 3 then
        	WriteLcd('Switch Weapon (q)')
            PressKey('q')
        end
        if arg == 5 then
        	WriteLcd('Use (e)')
            PressKey('e')
        end
        if arg == 6 then
        	WriteLcd('Reload (r)')
            PressKey('r')
        end
        if arg == 13 then
        	WriteLcd('Fire (f)')
            PressKey('f')
        end

    elseif event == 'G_RELEASED' then
        if arg == 4 then
        	ClearLcd()
            ReleaseKey('w')
        end
        if arg == 10 then
        	ClearLcd()
            ReleaseKey('a')
        end
        if arg == 11 then
        	ClearLcd()
            ReleaseKey('s')
        end
        if arg == 12 then
        	ClearLcd()
            ReleaseKey('d')
        end
        if arg == 22 then
        	ClearLcd()
            ReleaseKey('spacebar')
        end
		if arg == 9 then
        	ClearLcd()
			ReleaseKey('lshift')
		end
		if arg == 15 then
        	ClearLcd()
			ReleaseKey('lctrl')
		end
        if arg == 1 then
        	ClearLcd()
            ReleaseKey('tilde')
        end
        if arg == 2 then
        	ClearLcd()
            ReleaseKey('tab')
        end
        if arg == 3 then
        	ClearLcd()
            ReleaseKey('q')
        end
        if arg == 5 then
        	ClearLcd()
            ReleaseKey('e')
        end
        if arg == 6 then
        	ClearLcd()
            ReleaseKey('r')
        end
        if arg == 13 then
        	ClearLcd()
            ReleaseKey('f')
        end
    end

    --OutputLogMessage(event..' : '..arg..' : '..family)
end
