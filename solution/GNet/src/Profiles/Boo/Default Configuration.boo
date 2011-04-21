import GNet.Lib

static def Run(d as G13Device):
	d.KeyPressed += def(key):
		k as uint = key
		print k