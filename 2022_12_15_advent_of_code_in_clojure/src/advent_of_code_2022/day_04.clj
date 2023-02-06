(ns advent-of-code-2022.day-04
  (:require [clojure.set :as set]
            [clojure.string :as string])
  (:gen-class))

(defn load-data [text]
  (->> text
       string/split-lines
       (map #(string/split % #",|-"))
       (map #(map parse-long %))
       (map #(split-at 2 %))))


(defn range-fully-contain? [[a b] [c d]]
  (and (<= a c) (>= b d)))

(defn range-overlapping? [[a b] [c d]]
  (and (<= c b) (>= c a)))


(defn count-ranges [text pred]
  (->>
   text
   load-data
   (filter (fn [[range-1 range-2]] (or (pred range-1 range-2) (pred range-2 range-1))))
   count))

(defn puzzle-1 [text]
  (count-ranges text range-fully-contain?))

(defn puzzle-2 [text]
  (count-ranges text range-overlapping?))



(comment
  (def file-path "src/advent_of_code_2022/day_04.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)

