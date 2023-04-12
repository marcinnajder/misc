(ns advent-of-code-2022.day-15
  (:require [clojure.string :as string])
  (:gen-class))


(defn manhattan-distance [[[x-s y-s] [x-b y-b]]]
  (+ (abs (- x-s x-b)) (abs (- y-s y-b))))

(defn load-data [text]
  (->>
   (string/split-lines text)
   (map #(mapv vec (partition 2 (map parse-long (re-seq #"-?\d+" %)))))
   (map #(conj % (manhattan-distance %)))))


;; function copied from day 14 :)
(defn merge-lines [[[from to :as head] & tail :as lines] [from-l to-l :as line]]
  (cond
    (empty? lines) (list line)
    (< (inc to) from-l) (cons head (merge-lines tail line))
    (< (inc to-l) from) (cons line lines)
    :else (merge-lines tail [(min from from-l) (max to to-l)])))



;; default 'keep' function is not lazy, it processes items in batches of 32 items
;; I wanted 'lazy-lines-in-row' function to be lazy because its usage inside 'search-gap' function
(defn lazy-keep [f coll]
  (if (empty? coll)
    '()
    (let [[head & tail] coll
          new-head (f head)]
      (if (nil? new-head)
        (lazy-seq (lazy-keep f tail))
        (cons new-head (lazy-seq (lazy-keep f tail)))))))


(defn lazy-lines-in-row [entries row]
  (lazy-keep
   (fn [[[x y] _ distance]]
     (let [distance-to-row (abs (- row  y))
           distance-diff (- distance distance-to-row)]
       (if (< distance-diff 0)
         nil
         [(- x distance-diff) (+ x distance-diff)])))
   entries))

(defn merge-lines-in-row [entries row]
  (reduce merge-lines '() (lazy-lines-in-row entries row)))

(defn count-points [lines]
  (->>
   lines
   (map (fn [[start end]] (inc (abs (- end start)))))
   (apply +)))

(defn count-beacons-in-row [entries row]
  (->>
   entries
   (keep (fn [[_ [x y] _]] (if (= y row) x nil)))
   distinct
   count))

(defn puzzle-1 [text]
  (let [row 2000000
        entries (load-data text)
        points (count-points (merge-lines-in-row entries row))
        beacons-in-row (count-beacons-in-row entries row)]
    (- points beacons-in-row)))





(defn intersection-points-of-two-lines [[[sx-1 sy-1] [sx-2 sy-2]] [[bx-1 by-1] [bx-2 by-2]]]
  (let [l (+ sx-1 sy-1)
        y  (/ (+ l (- by-1 bx-1)) 2)
        x  (/ (+ l (- bx-1 by-1)) 2)]
    (if
     (and (<= sx-1 x sx-2) (<= bx-1 x bx-2)
          (<= sy-2 y sy-1) (<= by-1 y by-2))
      (if (ratio? x)
        (let [x1 (int x) x2 (inc x1) y1 (int y) y2 (inc y1)]
          [[x1 y1] [x2 y1] [x1 y2] [x2 y2]])
        [[x y]])
      [])))


(defn lines-for-sides-of-expanded-diamond [[[x y] _ distance]]
  (let [expanded-distance (inc distance)
        left [(- x expanded-distance) y]
        right [(+ x expanded-distance) y]
        top [x (- y expanded-distance)]
        bottom [x (+ y expanded-distance)]]
    [[[left top] [bottom right]] [[top right] [left bottom]]]))

(defn intersection-points-for-entries [entries]
  (let [[slashes backslashes]
        (reduce
         #(mapv into %1 %2)
         ['() '()]
         (map lines-for-sides-of-expanded-diamond  entries))]
    (->>
     slashes
     (mapcat
      (fn [slash]
        (mapcat #(intersection-points-of-two-lines slash %) backslashes)))
     distinct)))

(defn detected-by-any-sensor? [entries point]
  (some
   (fn [[s _ distance]]
     (<= (manhattan-distance [point s]) distance))
   entries))


(defn puzzle-2 [text]
  (let [[from to] [0 4000000]
        entries (load-data text)
        [column row]
        (->>
         (intersection-points-for-entries entries)
         (filter #(and (<= from (first %) to) (<= from (second %) to)))
         (filter #(not (detected-by-any-sensor? entries %)))
         first)]
    (+ (* column 4000000) row)))



(comment
  (def file-path "src/advent_of_code_2022/day_15.txt")

  (def text (slurp file-path))

  (load-data text)
  (def entries (load-data text))

  (puzzle-1 text)

  (puzzle-2 text)

  (time
   (puzzle-2' text))

  (tests)

  :rfc)









;; ****************************************************************************************
;; different implementation of puzzle 1
;; it 'draws' all diamonds on the board using horizontal lines, it takes too long for huge diamonds

(defn insert-diamond [board [[x-s y-s] _ distance]]
  (let [start (- x-s distance)
        end (+ x-s distance)]
    (reduce
     (fn [board i]
       (->
        board
        (update  (+ y-s i) (fnil merge-lines '()) [(+ start i) (- end i)])
        (update  (- y-s i) (fnil merge-lines '()) [(+ start i) (- end i)])))
     (update board y-s (fnil merge-lines '()) [start end])
     (range 1 (inc distance)))))

(defn create-board [entries]
  (reduce insert-diamond {} entries))


(defn count-beacons-in-rows [entries]
  (update-vals
   (->>
    entries
    (map second)
    (distinct)
    (group-by second))
   count))

;; execute only for sample data, otherwise it runs forever
(defn puzzle-1' [text]
  (let [row 10
        entries (load-data text)
        board (create-board entries)
        points (count-points (get board row))
        beacons-in-row (get (count-beacons-in-rows entries) row)]
    (- points beacons-in-row)))



;; ****************************************************************************************
;; different implementation of puzzle 2
;; it goes row by row until reaches searched point, it executes 43 secs ;)


(defn line-inside-another-line? [[outer-start outer-end] [inner-start inner-end]]
  (and (<= outer-start inner-start) (>= outer-end inner-end)))

(defn line-overlapping? [[start-1 end-1] [start-2 end-2]]
  (if (< start-1 start-2)
    (>= end-1 start-2)
    (>= end-2 start-1)))

(defn search-gap-in-merged-lines [lines [search-start search-end :as search-line]]
  (when-some [[line-start line-end :as line]
              (->>
               lines
               (drop-while #(< (second %) search-start))
               (take-while #(<= (first %) search-end))
               (some (fn [l] (and (line-overlapping? l search-line) l))))]
    (if (line-inside-another-line? line search-line)
      nil
      (if (<= line-start search-start) (inc line-end) search-start))))

(defn search-gap [entries row search-line]
  (when-some [merged-lines
              (reduce
               (fn [lines line]
                 (if (line-inside-another-line? line search-line)
                   (reduced nil)
                   (merge-lines lines line)))
               '()
               (lazy-lines-in-row entries row))]
    (search-gap-in-merged-lines merged-lines search-line)))

(defn puzzle-2' [text]
  (let [[from to :as search-line] [0 4000000]
        entries (load-data text)
        [column row]
        (some
         #(when-some [column (search-gap entries % search-line)] [column %])
         (range from (inc to)))]
    (+ (* column 4000000) row)))









(defn tests []

  (assert (= (insert-diamond {} [[5 10] [5 11] 1]) {10 `([4 6]), 11 `([5 5]), 9 `([5 5])}))
  (assert (= (insert-diamond {} [[5 10] [5 12] 2]) {10 '([3 7]), 11 '([4 6]), 9 '([4 6]), 12 '([5 5]), 8 '([5 5])}))
  (assert (= (insert-diamond {} [[5 10] [5 12] 2]) (insert-diamond {} [[5 10] [5 8] 2])))

  (assert (= (line-inside-another-line? [10 15] [10 15]) true) "lines are exactly the same")
  (assert (= (line-inside-another-line? [10 15] [11 14]) true) "line is inside another")
  (assert (= (line-inside-another-line? [10 15] [9 14]) false) "lines are overlapping 1")
  (assert (= (line-inside-another-line? [10 15] [10 16]) false) "lines are overlapping 2")
  (assert (= (line-inside-another-line? [10 15] [5 7]) false) "lines are not overlapping 1")
  (assert (= (line-inside-another-line? [10 15] [17 20]) false) "lines are not overlapping 2")


  (doseq [[line-1 line-2 expected-result]
          [[[5 10] [7 12] true]
           [[5 10] [5 12] true]
           [[5 10] [5 10] true]
           [[5 10] [3 12] true]
           [[5 10] [12 15] false]]]
    (assert (= (line-overlapping? line-1 line-2) expected-result))
    (assert (= (line-overlapping? line-2 line-1) expected-result)))


  (assert (=
           (lines-for-sides-of-expanded-diamond [[0 0] [0 0] 1])
           [[[[-2 0] [0 -2]] [[0 2] [2 0]]] [[[0 -2] [2 0]] [[-2 0] [0 2]]]]))

  (assert (= (intersection-points-of-two-lines  [[3 7] [5 4]] [[4 4] [10 10]]) [[5 5]]))
  (assert (= (intersection-points-of-two-lines  [[3 7] [5 4]] [[3 5] [7 9]]) [[4 6]]))
  (assert (= (intersection-points-of-two-lines  [[3 7] [5 4]] [[0 8] [4 12]]) []))
  (assert (= (intersection-points-of-two-lines  [[0 10] [5 4]] [[0 8] [4 12]]) [[1 9]]))

  :rfc)
