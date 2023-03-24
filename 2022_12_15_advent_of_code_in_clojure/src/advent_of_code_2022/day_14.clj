(ns advent-of-code-2022.day-14
  (:require [clojure.string :as string])
  (:gen-class))

(defn merge-lines [[[from to :as head] & tail :as lines] [from-l to-l :as line]]
  (cond
    (empty? lines) (list line)
    (< (inc to) from-l) (cons head (merge-lines tail line))
    (< (inc to-l) from) (cons line lines)
    :else (merge-lines tail [(min from from-l) (max to to-l)])))


(defn insert-straight-line [board [[x1 y1] [x2 y2]]]
  (if (= x1 x2) ;; is vertical line
    (update board x1 (fnil merge-lines '()) [(min y1 y2) (max y1 y2)])
    (reduce
     #(update %1 %2 (fnil merge-lines '()) [y1 y1])
     board
     (range (min x1 x2) (inc (max x1 x2))))))

(defn insert-snake-line [board line]
  (->>
   line
   (re-seq #"\d+")
   (map parse-long)
   (partition 2)
   (partition 2 1)
   (reduce insert-straight-line board)))

(defn load-data [text]
  (->>
   (string/split-lines text)
   (reduce insert-snake-line {})))



(defn find-free-space-in-column [[[from to] & tail :as lines] row]
  (cond
    (empty? lines) {:type :not-found}
    (< row from) {:type :found :row (dec from)}
    (<= row to) {:type :occupied}
    :else (recur tail row)))


(defn find-free-space-on-left-and-right [board [column row]]
  (let [[left-column left-row] [(dec column) (inc row)]
        left-result (find-free-space-in-column (get board left-column '()) left-row)]
    (condp = (:type left-result)
      :not-found {:type :not-found :column left-column}
      :found (recur board [left-column (:row left-result)])
      :occupied
      (let [[right-column right-row] [(inc column) (inc row)]
            right-result (find-free-space-in-column (get board right-column '()) right-row)]
        (condp = (:type right-result)
          :not-found {:type :not-found :column right-column}
          :found (recur board [right-column (:row right-result)])
          :occupied {:type :found :pos [column row]})))))


(defn drop-sand [board floor-row]
  (loop [board board
         index 0]
    (let [{:keys [type row]} (find-free-space-in-column (get board 500) 0)]
      (if (= type :occupied)
        index
        (let [{:keys [type column pos]} (find-free-space-on-left-and-right board [500 row])]
          (condp = type
            :occupied (throw "no worries, I won't be here")
            :found (recur (insert-straight-line board [pos pos]) (inc index))
            :not-found
            (if (nil? floor-row)
              index
              (recur (insert-straight-line board [[column (dec floor-row)] [column floor-row]]) (inc index)))))))))


(defn puzzle-1 [text]
  (drop-sand (load-data text) nil))

(defn puzzle-2 [text]
  (let [board (load-data text)
        max-hight (apply max (map (comp second last) (vals board)))]
    (drop-sand board (+ max-hight 2))))


(comment
  (def file-path "src/advent_of_code_2022/day_14.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  (merge-lines-tests)

  :rfc)



(defn merge-lines-tests []
  (assert (= (merge-lines (list) [6 8]) (list [6 8])) "empty list")

  (assert (= (merge-lines (list [10 12]) [6 8]) (list [6 8] [10 12])) "insert new line at the beginning")
  (assert (= (merge-lines (list [10 12]) [14 16]) (list [10 12] [14 16])) "insert new line at the end")
  (assert (= (merge-lines (list [10 12] [18 20]) [14 16]) (list [10 12] [14 16] [18 20])) "insert new line in the middle")

  (assert (= (merge-lines (list [10 12] [16 18]) [6 9]) (list [6 12] [16 18])) "joing two sticking lines 1")
  (assert (= (merge-lines (list [10 12] [16 18]) [13 14]) (list [10 14] [16 18])) "joing two sticking lines 2")

  (assert (= (merge-lines (list [10 12] [16 18]) [8 11]) (list [8 12] [16 18])) "joing two overlapping lines 1")
  (assert (= (merge-lines (list [10 12] [16 18]) [11 13]) (list [10 13] [16 18])) "joing two overlapping lines 2")

  (assert (= (merge-lines (list [10 12] [16 18]) [13 15]) (list [10 18])) "joining tree sticking lines")
  (assert (= (merge-lines (list [10 12] [16 18]) [11 17]) (list [10 18])) "joining tree overlapping lines")

  (assert (= (merge-lines (list [10 12] [16 18] [25 27]) [7 20]) (list [7 20] [25 27])) "swallowing two lines")
  (assert (= (merge-lines (list [10 12] [16 18] [25 27]) [7 30]) (list [7 30])) "swallowing all lines")


  (assert (= (find-free-space-in-column (list) 5) {:type :not-found}) "not found, no lines")
  (assert (= (find-free-space-in-column (list [10 12] [16 18]) 20) {:type :not-found}) "no found, not free space")

  (assert (= (find-free-space-in-column (list [10 12] [16 18]) 17) {:type :occupied}) "occupied, in the middle")
  (assert (= (find-free-space-in-column (list [10 12] [16 18]) 16) {:type :occupied}) "occupied, at the beginning")
  (assert (= (find-free-space-in-column (list [10 12] [16 18]) 18) {:type :occupied}) "occupied, at the end")

  (assert (= (find-free-space-in-column (list [10 12] [16 18]) 5) {:type :found, :row 9}) "found, before first line")
  (assert (= (find-free-space-in-column (list [10 12] [16 18]) 13) {:type :found, :row 15}) "found, between lines"))

