(ns advent-of-code-2022.day-02
  (:require [clojure.string :as string])
  (:gen-class))

(defn load-data [text]
  (->> text
       string/split-lines
       (map #(string/split % #"\s+"))))

(def points {"X" 1 "Y" 2 "Z" 3})

(defn calculate [[player-1 player-2]]
  (->>
   (condp = [player-1 player-2]
     ["A" "Y"] 6
     ["A" "Z"] 0
     ["B" "Z"] 6
     ["B" "X"] 0
     ["C" "X"] 6
     ["C" "Y"] 0
     3)
   (+ (get points player-2))))


(defn reverse-winner [[palyer-1 palyer-2]]
  (condp = [palyer-1 palyer-2]
    ["A" "X"] ["A" "Z"]
    ["A" "Z"] ["A" "Y"]
    ["B" "X"] ["B" "X"]
    ["B" "Z"] ["B" "Z"]
    ["C" "X"] ["C" "Y"]
    ["C" "Z"] ["C" "X"]

    ["A" "Y"] ["A" "X"]
    ["B" "Y"] ["B" "Y"]
    ["C" "Y"] ["C" "Z"]))

(defn play [text transform-play]
  (->> text
       load-data
       (map transform-play)
       (map calculate)
       (apply +)))

(defn puzzle-1 [text]
  (play text identity))

(defn puzzle-2 [text]
  (play text reverse-winner))



(comment
  (def file-path "src/advent_of_code_2022/day_02.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)
