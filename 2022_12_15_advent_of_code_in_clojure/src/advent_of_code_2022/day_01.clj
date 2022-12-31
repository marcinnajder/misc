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

(defn puzzle-1 [text]
  (->> text
       load-data
       (map #(apply + %))
       (apply max)))

(defn insert-sorted [xs x]
  (if (empty? xs)
    (list x)
    (let [[head & tail] xs]
      (if (< x head)
        (cons x xs)
        (cons head (insert-sorted tail x))))))

(defn insert-top-3 [xs x]
  (if (<= x (first xs))
    xs
    (rest (insert-sorted xs x))))

(defn puzzle-2 [text]
  (->> text
       load-data
       (map #(apply + %))
       (reduce insert-top-3 (list 0 0 0))
       (apply +)))

(comment
  (def file-path "src/advent_of_code_2022/day_01.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)
