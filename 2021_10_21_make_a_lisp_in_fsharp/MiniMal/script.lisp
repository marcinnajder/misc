;; podstawy
(+ 1 2)                                     ; -> 3
(/ (* 2 3) (+ 1 2))                         ; -> 2
(< 10 15)                                   ; -> true
(if (< 10 15) (+ 1 2) 5)                    ; -> 3

;; Environment
(def! a 1)                                  ; -> 1
(def! b (+ 1 2 3 ))                         ; -> 6
(if (= a b) 10 1000)                        ; -> 1000
(let* (x 100 y 10) (/ x y))                 ; -> 10
(let* (x 100 y (* 5 x)) (/ x y))            ; -> 0.2

;; Typy danych
(let* (s "text" b true n 5.5 ni nil) n )    ; -> 5.5

(string? "text")                            ; -> true
(number? 234)                               ; -> true
(nil? nil)                                  ; -> true
(true? true)                                ; -> true
(false? false)                              ; -> true

(list? (list (1 2 )))                       ; -> true
(vector? [1 2])                             ; -> true
(map? {})                                   ; -> true

(fn? +)                                     ; -> true
(symbol? (quote s))                         ; -> true

;; Listy, wektory
(def! list1 (list 10 40 100))          	    ; -> (10 40 100)
(def! list2 (list "text" true 5.5 nil))	    ; -> ("text" true 5.5 nil)

(def! vector1 (vector 10 40 100))       	; -> [10 40 100]
(def! vector1 [10 40 100])            	   	; -> [10 40 100]

(conj (list 6 7 8) 1 2 3)                 	; -> (3 2 1 6 7 8)
(conj [6 7 8] 1 2 3)                      	; -> [6 7 8 1 2 3]

(cons 0 (list 1 2 ))                        ; -> (0 1 2)
(first (list 8 7 5))                        ; -> 8
(rest (list 8 7 5))                         ; -> (7 5)
(count (list 0 1 2))                        ; -> 3
(empty? (list))                             ; -> true
(concat (list 1 2 ) (list 7 8 9))           ; -> (1 2 7 8 9)
(nth (list 8 7 5) 1)                        ; -> 7

;; Mapy 
(def! map1 {"name" "John" "age" 30})      		; -> {"name" "John" "age" 30}
(def! map1 (hash-map "name" "John" "age" 30))	; -> {"name" "John" "age" 30}

(get map1 "name")                           ; -> "John"
(get map1 "city")                           ; -> nil
(contains? map1 "age")                      ; -> true
(keys map1)                                 ; -> ("name" "age")
(vals map1)                                 ; -> ("John" 30)

(def! map2 (assoc map1 "city" "L.A."))      ; -> {"name "John" "age" 30 "city" "L.A."}
(def! map3 (dissoc map2 "city" ))           ; -> {"name" "John" "age" 30}

(= map1 map3)							    ; -> true
(= (list 1 2 3) (cons 1 (list 2 3)))        ; -> true

;; Funkcje

(def! inc (fn* (x) (+ x 1)))                ; -> #<function>
(inc 10)                                    ; -> 11

(def! add (fn* (a b) (+ a b)))              ; -> #<function>

(def! max (fn* (a b) (if (> a b) a b)))     ; -> #<function>

(def! hello-world (fn* () 
  (do 
    (println "hello") 
    (println "world") )))
  
; closure
(def! return-value (fn* (value) (fn* () value)))
(def! return-10 (return-value 10))
(return-10)                                 ; -> 10

;; Funkcja 'map'
(def! map (fn* (f xs) (if (empty? xs) xs  (cons (f (first xs)) (map f (rest xs))))))

(map inc (list 1 2 3) )                     ; -> (2 3 4)
(map (fn* (x) (* x 10)) (list 1 2 3) )      ; -> (10 20 30)




