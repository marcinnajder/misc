(+ 1 2)

(def! inc (fn* (x) (+ x 1)))
(inc 10)

(load-file "./samples/map_filter_reduce.clj")

(map inc `(1 2 3) )


(def! positive? (fn* (x) (> x 0 )))
(filter positive? `(1 -2 3 0 4) )


(def! add (fn* (a b) (+ a b )))
(reduce add 0 `(1 -2 3 0 4) )


