; # Podstawy

(+ 1 2)                                   ; -> 3
; 1 + 2 

(/ (* 2 3) (+ 1 2))                       ; -> 2
; (2 * 3) / (1 + 2)

(< 10 15)                                 ; -> true
; 10 < 15

(if (< 10 15) (+ 1 2) 5)                  ; -> 3
; 10 < 15 ? 1 + 2 : 5


; # Environment

(def! a 1)                                ; -> 1
; var a = 1 

(def! b (+ 1 2 3 ))                       ; -> 6
; var b = 1 + 2 + 3

(if (= a b) 10 1000)                      ; -> 1000
; a === b ? 10 : 1000

(let* (x 100 y 10) (/ x y))               ; -> 10
; (function () { var x = 100, y = 10; return x / y; } )()
; (() => (x = 100, y = 10, x / y)) ()

(let* (x 100 y (* 5 x)) (/ x y))          ; -> 0.2
; (() => (x = 100, y = 5 * x, x / y)) ()





; # Typy danych, listy, mapy, wektory

(let* (s "text" b true n 5.5 ni nil) n )  ; -> 5.5
; (() => (s = "text", b = true, n = 5.5, ni = null, n)) ()

(def! items1 (list 10 40 100))            ; -> (10 40 100)
; var items1 = [10, 40, 100];

(def! items2 (list "text" true 5.5 nil))  ; -> ("text" true 5.5 nil)
; var items2 = [ "text", true, 5.5, null];

(def! t1  (list 10 40 100))               ; -> (10 40 100)
(def! t1 '(10 40 100))                    ; -> (10 40 100)

(def! t2 '(+ (- 10 5) z :k))              ; -> (+ (- 10 5) z :k)
; "(10 - 5) + Symbol("z") + k"

(def! p {:id 1 :name "John"})             ; -> {:id 1 :name "John"}
; var p = {id: 1, name: "John"}

'(10 50 100 false)                        ; -> (10 50 100 false)
[10 50 100 false]                         ; -> [10 50 100 false]


; # Funkcje, closure

(def! inc (fn* (x) (+ x 1)))
; var inc = x => x + 1;
(inc 10)                                  ; -> 11

(def! add (fn* (a b) (+ a b)))
; var add = (a, b) => a + b;

(def! max (fn* (a b) (if (> a b) a b)))
; var max = (a, b) => a > b ? a : b;

(def! hello-world (fn* () (do (prn "hello") (prn "world") )))
; var helloWorld = () => { console.log("hello"); console.log("world"); }
(hello-world)

((fn* (x) (* x 10)) 100)                ; => 1000
; (x => x * 10)(100) 





; # Funkcje dla list i wektorów

(count '(0 1 2))                          ; -> 3
(empty? '())                              ; -> true
(cons 0 '(1 2 ))                          ; -> (0 1 2)
(concat '(1 2 ) '(7 8 9))                 ; -> (1 2 7 8 9)
(nth '(8 7 5) 1)                          ; -> 7
(first '(8 7 5))                          ; -> 8
(rest '(8 7 5))                           ; -> (7 5)

(vector 1 2 3 "a" )                       ; -> [1 2 3 "a"]

(conj '(6 7 8) 1 2 3)                     ; -> (3 2 1 6 7 8)
(conj [6 7 8] 1 2 3)                      ; -> [6 7 8 1 2 3]



; # Funkcje dla map

(def! m1 {:name "John" :age 30})          ; -> {:name "John" :age 30}
(get m1 :name)                            ; -> "John"
(get m1 :address)                         ; -> nil
(contains? m1 :age)                       ; -> true
(keys m1)                                 ; -> (:name :age)
(vals m1)                                 ; -> ("John" 30)

(def! m2 (assoc m1 :address "New York"))  ; -> {:name "John" :age 30 :address "New York"}
m1                                        ; -> {:name "John" :age 30}
m2                                        ; -> {:name "John" :age 30 :address "New York"}
(def! m3 (dissoc m2 :address ))           ; -> {:name "John" :age 30}
m2                                        ; -> {:name "John" :age 30 :address "New York"}'
m3                                        ; -> {:name "John" :age 30}

(= {"x" 1 "y" 2} (hash-map "x" 1 "y" 2))  ; -> true


; # Funkcje sprawdzające typy

(string? "text")                          ; -> true
(number? 234)                             ; -> true
(nil? nil)                                ; -> true
(true? true)                              ; -> true
(false? false)                            ; -> true

(symbol? 's)                              ; -> true
(keyword? :k)                             ; -> true

(map? {})                                 ; -> true
(list? '(1 2 ))                           ; -> true
(vector? [1 2])                           ; -> true

(sequential? '(1 2))                      ; -> true
(sequential? [1 2])                       ; -> true

(fn? +)                                   ; -> true


; # atom
; "atom": ([mal]: MalType[]) => ok(atom(mal)),
; "atom?": ([mal]: MalType[]) => ok(mal.type === "atom" ? true_ : false_),
; "deref": ([atom]: MalType_atom[]) => ok(atom.mal),
; "reset!": ([atom, mal]: MalType_atom[]) => {
; "swap!": (mals: MalType[]) => {






(- (+ 10 5) (/ 100 50))

;; let
(let*  (z (+ 2 3))  (+ 1 z))


;; fun

(fn* (a) a) 

( (fn* (a) a) 7) 

( (fn* (a) (+ a 1)) 10) -> 11

( (fn* (a b) (+ a b)) 2 3) -> 5

