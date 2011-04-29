Original Readme
http://lglcdnet.codeplex.com/
Project updated for LCDSDK 3.06
---------------

LCD_DLL

This is a simple DLL library to expose the static library provided by logitech as a DLL so that we can
provide Interop calls from .NET.

In order to build this project, be sure to install the Logitech G15 software and SDK, and add
the sdk library path to your Visual Studio library paths.

This directory is typically located here:
	C:\Program Files\Logitech\GamePanel Software\LCD Manager\SDK\LCDSDK_2.02.101\SDK\Libs\x86
	
The setting for library paths in Visual Studio can be found like this:
	- Click on the "Tools" menu
	- Choose "Options"
	- Expand the "Projects and Solutions" tree view item 
	- Click the "VC++ Directories" item
	- Change the "Show Directories For" dropdown to "Library Files"
	- Click the "New Line" item above the list to add an item
	- Browse to the SDK library path, typically located at:
		C:\Program Files\Logitech\GamePanel Software\LCD Manager\SDK\LCDSDK_2.02.101\SDK\Libs\x86