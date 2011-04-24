#= Filename:
#= Name: Untitled
#= Description:
#= Language: Boo
#= Device: G13
#= Lock: False
#= Executables:
#= KeyboardHook: None
#= MouseHook: None

import GNet.Lib

static def Run(d as G13Device):
	d.KeyPressed += def(key):
		k as uint = key
		print k