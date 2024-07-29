extends GdUnitTestSuite

func test_currency() -> void:
	var tests: Array[Dictionary] = [
		{"in": 0, "expected": "0"},
		{"in": 1, "expected": "1"},
		{"in": 123, "expected": "123"},
		{"in": 1234, "expected": "1,234"},
		{"in": 12345, "expected": "12,345"},
		{"in": 123456789, "expected": "123,456,789"}
	]
	for test in tests:
		assert_str(Format.Currency(test["in"])).is_equal(test["expected"])
