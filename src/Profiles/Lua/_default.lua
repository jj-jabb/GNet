-- Name: Default
-- Description: The default profile.
-- Executables:

poll = true

-- NOTE: currently family is ALWAYS 'lhc' since the only currently supported
-- device is the G13 left-handed-controller.
function OnEvent(event, arg, family)
	if poll and (event == 'PROFILE_ACTIVATED' or event == 'M_RELEASED') then
		SetMKeyState(1)
		Sleep(25)
	end
	
	if event == 'PROFILE_DEACTIVATED' then
		OutputLogMessage('Ran for '..GetRunningTime()..' milliseconds.')
	end
	
    if event == 'G_PRESSED' then
		if arg == 20 then
			ClearLog()
		end

        if arg == 4 then
            PressKey('w')
        end
        if arg == 10 then
            PressKey('a')
        end
        if arg == 11 then
            PressKey('s')
        end
        if arg == 12 then
            PressKey('d')
        end
        if arg == 22 then
            PressKey('spacebar')
        end
		if arg == 9 then
			PressKey('lshift')
		end
		if arg == 15 then
			PressKey('lctrl')
		end
        if arg == 1 then
            PressKey('tilde')
        end
        if arg == 2 then
            PressKey('tab')
        end
        if arg == 3 then
            PressKey('q')
        end
        if arg == 5 then
            PressKey('e')
        end
        if arg == 6 then
            PressKey('r')
        end
        if arg == 13 then
            PressKey('f')
        end

    elseif event == 'G_RELEASED' then
        if arg == 4 then
            ReleaseKey('w')
        end
        if arg == 10 then
            ReleaseKey('a')
        end
        if arg == 11 then
            ReleaseKey('s')
        end
        if arg == 12 then
            ReleaseKey('d')
        end
        if arg == 22 then
            ReleaseKey('spacebar')
        end
		if arg == 9 then
			ReleaseKey('lshift')
		end
		if arg == 15 then
			ReleaseKey('lctrl')
		end
        if arg == 1 then
            ReleaseKey('tilde')
        end
        if arg == 2 then
            ReleaseKey('tab')
        end
        if arg == 3 then
            ReleaseKey('q')
        end
        if arg == 5 then
            ReleaseKey('e')
        end
        if arg == 6 then
            ReleaseKey('r')
        end
        if arg == 13 then
            ReleaseKey('f')
        end
    end

    --OutputLogMessage(event..' : '..arg..' : '..family)
end
