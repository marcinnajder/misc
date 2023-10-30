using AlgorithmsAndDataStructures;

LList<int> emptylist = [];
// LList<int> insList = [1, 2, 3, 4, 5];
// LList<string> stringList = ["a", "b", "c"];


// Console.WriteLine(emptylist.First());


// var list1 = new LList2<int>(1, new LList2<int>(2, new LList2<int>(1, null))); // [1, 2, 3]
// LList2<int> list2 = new(1, new(2, new(1, null))); // [1, 2, 3]


// Console.WriteLine(list1.Head); // 1
// Console.WriteLine(list2.Tail!.ToString()); // LList2 { Head = 2, Tail = LList2 { Head = 1, Tail =  } }
// Console.WriteLine(list1 == list2); // True

// Console.WriteLine(new Test().ToString()); // true
// Console.WriteLine(new Test()); // true


// LList<int> list3 = new(1, new(2, new(3, new())));
// LList<int> list4 = new(1, new(2, new(3, LList<int>.Empty)));


// var list5 = LList.Of(1, 2, 3);
// var list6 = LList.Of<int>(); // empty list

// LList<int> list7 = [1, 2, 3];
// LList<int> list8 = []; // empty list
// LList<int> list9 = [0, .. list7, 4, 5];

// LList<int> ints = [5, 10, 15, 20, 25];

// Console.WriteLine($"{ints[0]}, {ints[1]}, ..., {ints[^2]}, {ints[^1]}"); // 5, 10, ..., 20, 25

// Console.WriteLine(ints[1..3]); // [10, 15]
// Console.WriteLine(ints[1..]); // [10, 15, 20, 25]
// Console.WriteLine(ints[..^2]); // [5, 10, 15]

// Console.WriteLine(Object.ReferenceEquals(ints[1..], ints.Tail)); // True

// // LList<int> ints = [5, 10, 15, 20, 25];
// var q =
//     from i in ints
//     where i % 10 == 0
//     select $"{i:.00}";

// Console.WriteLine(q.ToLList()); // [10,00, 20,00]

// Console.WriteLine();



// // LList<int> list1 = [1, 2, 3];
// // Console.WriteLine(list1.ConcatL([4, 5, 6]));
// // LList<int> list2 = [0, .. list1, 0];






// // int[] row0 = [1, 2, 3];

// // int[] row1 = [4, 5, 6];
// // int[] row2 = [7, 8, 9];
// // //int[] single = [.. row0, .. row1, .. row2, .. [12, 12]];

// // LList<int> l1 = [1, 2, 3];
// // LList<int> l2 = [1, 2, 3, .. l1];


