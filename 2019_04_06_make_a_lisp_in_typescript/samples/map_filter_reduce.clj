
(def! map (fn* (f xs) 
  (if (empty? xs)
    xs 
    (cons (f (first xs)) (map f (rest xs))))))

(def! filter (fn* (f xs) 
  (if (empty? xs)
    xs
    (if (f (first xs))
      (cons (first xs) (filter f (rest xs))) 
      (filter f (rest xs)) ))))

(def! reduce (fn* (f total xs )  
  (if (empty? xs)
    total
    (reduce f (f total (first xs)) (rest xs)) )))















; (load-file "./map_filter_reduce.clj")

; (def! inc (fn* (x) (+ x 1 )))
; (map inc `(1 2 3) )
; (map inc `() )

; (def! positive? (fn* (x) (> x 0 )))
; (filter positive? `(1 -2 3 0 4) )
; (filter positive? `() )

; (def! add (fn* (a b) (+ a b )))
; (reduce add 0 `(1 -2 3 0 4) )
; (reduce add 0 `(1 2 ) )


(def! return-value (fn* (value) (fn* () value)))

(def! return-10 (return-value 10))
(def! return-100 (return-value 100))

(return-10)
(return-100)