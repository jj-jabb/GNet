print g13

def KeyPressed(device, key, keyState):
    print key

def Write(k):
	print k

# g13.KeyPressed += KeyPressed

g13.KeyPressed += lambda d, k, s: Write(k)