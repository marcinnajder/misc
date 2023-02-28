(ns advent-of-code-2022.day-03
  (:require [clojure.set :as set]
            [clojure.string :as string])
  (:gen-class))

(defn load-data [text]
  (string/split-lines text))

(def priority-of-a (int \a))
(def priority-of-A (int \A))

(defn calculate-priority-of-char [c]
  (inc (- (int c) (if (Character/isLowerCase c) priority-of-a (- priority-of-A 26)))))


(defn find-char-included-in-all-collections [[first-coll & other-colls]]
  (let [sets (map set other-colls)]
    (some #(and (every? (fn [s] (s %)) sets) %) first-coll)))


(defn find-char-included-in-all-collections-2 [other-colls]
  (->> other-colls (map set) (reduce set/intersection) first))


(defn sum-of-priorities [text partitioner]
  (->>
   text
   load-data
   partitioner
   (map find-char-included-in-all-collections)
   (map calculate-priority-of-char)
   (apply +)))

(defn puzzle-1 [text]
  (sum-of-priorities text (partial map #(split-at (/ (count %) 2) %))))

(defn puzzle-2 [text]
  (sum-of-priorities text (partial partition 3)))


(comment
  (def file-path "src/advent_of_code_2022/day_03.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)


;; (defn find-char-included-in-two-halves [line]
;;   (reduce (fn [[set-of-chars counter] next-char]
;;             (if (> counter 0)
;;               [(conj set-of-chars next-char) (dec counter)]
;;               (if (set-of-chars next-char)
;;                 (reduced next-char)
;;                 [set-of-chars counter])))
;;           [#{}, (/ (count line) 2)] line))
