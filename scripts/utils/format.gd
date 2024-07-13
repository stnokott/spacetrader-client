extends Object
class_name Format

static func Currency(n: int) -> String:
	var src = str(n)
	var s = ""
	for i in range(0, src.length()):
		if i != 0 and i % 3 == src.length() % 3:
			s += ","
		s += src[i]
	return s
