(ns  clojure-for-js-developers.clojure-for-js-developers
  (:gen-class)
  (:require [clojure.string :as string]))


;; Global variables
(def first-name "marcin")
(println "my name is:" first-name)


;; First-class function
(def add
  (fn [a b] (+ a b)))

(add 1 2) ;; => 3
(add 1) ;; => Execution error (ArityException) at .. Wrong number of args (1) passed to ...
((fn [a b] (+ a b)) 10 2) ;; => 12


;; Function definition, defn macro
(defn add [a b]
  (+ a b))

(macroexpand '(defn add [a b] (+ a b))) ;; => (def add (clojure.core/fn ([a b] (+ a b))))


;; Macros
(def forms-1 '(/ (+ 10 5) 3))

(let [[op-1 [op-2 a b] c] forms-1]
  (println "operators:" op-1 op-2) ;; operators: / +
  (println "values: " a b c) ;; values:  10 5 3
  `(/ ~a ~b)) ;; => 2



;; , Variadic functions, comments
(defn add-at-least-2
  "function takes at least 2 arguments"
  [a b & rest-args]
  (+ a b (apply + rest-args)))

(add-at-least-2 1 2 3 4) ;; => 10




;; Control flow (if/then/else, variables, code blocks, simple loops)
(defn func-1 [a b]
  (if (> a b)
    (let [sum (+ a b)
          value (mod sum 10)]
      (println "value:" value)
      value)
    (do
      (dotimes [n 3]
        (println n "hej")
        (println n "yo"))
      (doseq [n '(3 4 5)]
        (println n "hej")
        (println n "yo"))
      666)))

(func-1 10 9) ;; => 9
(func-1 9 10) ;; => 666


;; 'apply' function
(apply add [1 2]) ;; => 3
(apply * [2 3]) ;; => 6


;; 'partial' function
(def increment
  (partial add 1))

(increment 10) ;; => 11

((partial add 1) 10) ;; => 11


;; 'comp' function (function composition)
(def increment-twice-then-to-string
  (comp str inc increment))

(increment-twice-then-to-string 10) ;; => "12"



;; Built-in functions

(nil? nil) ;; => true
(string? "") ;; => true
(boolean? true) ;; => true
(int? 1) ;; => true
(fn? +) ;; => true
(vector? [1 2 3]) ;; => true

(+ 1 2 3) ;; => 6
(* 1 2 3) ;; => 6
(< 1 2) ;; => true

(inc 10) ;; => 11
(dec 10) ;; => 9
(mod 10 3) ;; => 1
(abs -10) ;; => 10
(max 5 10 15) ;; => 15

(zero? 0) ;; => true
(pos? 1) ;; => true
(even? 10) ;; => true
(odd? 11) ;; => true

(identity 10) ;; => 10
(identity "mama") ;; => "mama"

((constantly 666) 1) ;; => 666
((constantly 666) 1 2 3) ;; => 666

((complement empty?) [])   ;; => false

(and (> 2 1) (string/ends-with? "tata" "ta") false) ;; => false
(or (> 2 1) (string/ends-with? "tata" "ta") false) ;; => true

;; - falshy - jest tylko false oraz nil (np 0 i "" nie)
(and "" 0) ;; => 0
(and "" 0 false) ;; => false
(and "" 0 nil) ;; => nil

(or "" 0) ;; => ""



;; Immutable singly-linked list
(def empty-list-1 '())
(def empty-list-2 (list))

(= empty-list-1 empty-list-2) ;; => true
(empty? empty-list-1) ;; => true

(def list-1 (cons 1 (cons 2 '())))
(def list-2 '(1 2))
(def list-3 (list 1 2))

(= list-1 list-2 list-3) ;; => true
(= '(1 (+ 1 1)) (list 1 (+ 1 1))) ;; => false


;; Vector
(def vector-1 [1 2])
(def vector-2 (vector 1 2))
(= vector-1 vector-2) ;; => true


;; Map
(def map-1 {:name "marcin" :age 123})
(def map-2 {:age 123 :name "marcin"})
(= map-1 map-2) ;; => true


;; Set
(def set-1 #{1 2})
(def set-2 (hash-set 1 2))
(= set-1 set-2);; 



;; Seq
(seq? "abc") ;; => false
(seq? (seq "abc")) ;; => true
(seq? (vector 1 2 2)) ;; => false
(seq? (seq (vector 1 2 2))) ;; => false
(seq? (list 1 2 3)) ;; => true




;; Collections operations
(count list-1) ;; => 2
(count vector-1) ;; => 2
(count map-1) ;; => 2
(count set-1) ;; => 2

(conj list-1 3 4 5) ;; => (5 4 3 1 2)
(conj vector-1 3 4 5) ;; => [1 2 3 3 4 5]
(conj set-1 3 4 5 1) ;; => #{1 4 3 2 5}

(into list-1 [3 4 5]) ;; => (5 4 3 1 2)
(into vector-1 [3 4 5]) ;; => [1 2 3 3 4 5]
(into set-1 [3 4 5 1]) ;; => #{1 4 3 2 5}

(let [coll vector-1]
  [(first coll)
   (rest coll)
   (empty? coll)
   (nth coll 1)]) ;; => [1 (2) false 2]


;; Map operations

(get map-1 :name) ;; => "marcin"
(map-1 :name) ;; => "marcin" (... mapa jest funkcja)
(:name map-1) ;; => "marcin" (... keyword jest funkcja)

(assoc map-1 :address "wroclaw" :id 1) ;; => {:name "marcin", :age 123, :address "wroclaw", :id 1}
(dissoc map-1 :name :id) ;; => {:age 123}

(assoc map-1 :name "lukasz") ;; => {:name "lukasz", :age 123}

(update map-1 :name (fn [value] (str value "!"))) ;; => {:name "marcin!", :age 123}
(update map-1 :name str "!") ;; => {:name "marcin!", :age 123}



;; Nested maps

(def line {:start {:x 1 :y 2} :end {:x 10 :y 20}})

(get-in line [:start :x]) ;; => 1
(assoc-in line [:start :x] 0) ;; => {:start {:x 0, :y 2}, :end {:x 10, :y 20}}

(update-in line [:start :x] inc) ;; => {:start {:x 2, :y 2}, :end {:x 10, :y 20}}
(update-in line [:start :x] + 1) ;; => {:start {:x 2, :y 2}, :end {:x 10, :y 20}}



;; Map operations and vectors

(def users [{:name "James" :age 26}  {:name "John" :age 43}])

(get users 1) ;; => {:name "John", :age 43}

(get-in users [1 :name]) ;; => "John"

(update-in users [1 :age] inc) ;; => [{:name "James", :age 26} {:name "John", :age 44}]



;; Seq operators (map, filter, reduce, ... )

(map
 (fn [x] (* x 10))
 (filter odd? [1 2 3 4 5 6])) ; => (10 30 50)

(map
 #(* % 10)
 (filter odd? [1 2 3 4 5 6])) ; => (10 30 50)



;; ->> ""thread-last"
(->>
 [1 2 3 4 5 6]
 (filter odd?)
 (map #(* % 10)))

(macroexpand
 '(->>
   [1 2 3 4 5 6]
   (filter odd?)
   (map #(* % 10))))

;; => (map (fn* [p1__8212#] (* p1__8212# 10)) (filter odd? [1 2 3 4 5 6]))



;; -> "thread-first" 
(assoc (assoc {} :name "marcin") :age 123) ;; => {:name "marcin", :age 123}

(->
 {}
 (assoc :name "marcin")
 (assoc :age 123))
;; => {:name "marcin", :age 123}


(macroexpand
 `(->
   {}
   (assoc :name "marcin")
   (assoc :age 123)))
;; => (clojure.core/assoc (clojure.core/assoc {} :name "marcin") :age 123)


;; Recursion
(defn sum-numbers [numbers]
  (if
   (empty? numbers)
    0
    (let [[head & tail] numbers]
      (+ head (sum-numbers tail)))))

(sum-numbers '())
(sum-numbers '(1 2 3))

(defn sum-numbers-with-tail-call [numbers total]
  (if
   (empty? numbers)
    total
    (let [[head & tail] numbers]
      (sum-numbers-with-tail-call tail (+ head total)))))

(sum-numbers-with-tail-call '(1 2 3) 0)

(defn sum-numbers-with-tail-call-recur [numbers total]
  (if
   (empty? numbers)
    total
    (let [[head & tail] numbers]
      (recur tail (+ head total)))))


(sum-numbers-with-tail-call-recur '(1 2 3) 0)

(defn sum-numbers-with-tail-loop [numbers]
  (loop [lst numbers
         total 0]
    (if
     (empty? lst)
      total
      (let [[head & tail] lst]
        (recur tail (+ head total))))))

(sum-numbers-with-tail-loop '(1 2 3))


;; Advent Of Code 2022 Day 1 