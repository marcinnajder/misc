(ns advent-of-code-2022.day-01
  (:require [clojure.string :as string])
  (:gen-class))

(defn load-data [text]
  (->> text
       string/split-lines
       (reduce (fn  [lists line]
                 (if (string/blank? line)
                   (cons '() lists)
                   (let [[inner-list & tail] lists]
                     (cons (cons (Integer/valueOf line) inner-list) tail))))
               '(()))))



(defn insert-sorted [xs x]
  (if (empty? xs)
    (list x)
    (let [[head & tail] xs]
      (if (< x head)
        (cons x xs)
        (cons head (insert-sorted tail x))))))

(defn insert-sorted-preserving-length [xs x]
  (if (<= x (first xs))
    xs
    (rest (insert-sorted xs x))))


(defn puzzle [text top-n]
  (->> text
       load-data
       (map #(apply + %))
       (reduce insert-sorted-preserving-length (into '() (repeat top-n 0)))
       (apply +)))

(defn puzzle-1 [text]
  (puzzle text 1))

(defn puzzle-2 [text]
  (puzzle text 3))


(comment

  (def file-path "src/advent_of_code_2022/day_01.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)


(apply)

(defn load-data-2 [text]
  (->> text
       string/split-lines
       (partition-by string/blank?)
       (filter (comp not string/blank? first))
       (map (partial map parse-long))))


(defn puzzle-1- [text]
  (->> text
       load-data
       (map #(apply + %))
       (apply max)))