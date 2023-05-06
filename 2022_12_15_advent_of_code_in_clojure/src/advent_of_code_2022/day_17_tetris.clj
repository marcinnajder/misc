(ns advent-of-code-2022.day-17-tetris
  (:require [clojure.string :as string])
  (:gen-class))


(defn load-data [text]
  text)


(defn calc-visible-face-indexes [shape-content [first-index & rest-indexes]]
  (reduce
   (fn [[found-indexes [from-max to-max] :as state] index]
     (let [[from to] (get shape-content index)]
       (if (or (< from from-max) (> to to-max))
         [(conj found-indexes index) [(min from from-max) (max to to-max)]]
         state)))
   [[first-index] (get shape-content first-index)]
   rest-indexes))

(defn calc-shape-properties [shape-content]
  (let [width (count shape-content)
        [left-face-indexes [from to]] (calc-visible-face-indexes shape-content (range 0 width))
        [right-face-indexes _] (calc-visible-face-indexes shape-content (reverse (range 0 width)))]
    {:width width :height (inc (- to from)) \< left-face-indexes \> right-face-indexes}))

(def shape-types
  (->>
   (list
    {:type :horizontal :content [[0 0] [0 0] [0 0] [0 0]]}
    {:type :cross  :content [[1 1] [0 2] [1 1]]}
    {:type :corner  :content [[2 2] [2 2] [0 2]]}
    {:type :vertical  :content [[0 3]]}
    {:type :square  :content [[0 1] [0 1]]})
   (map #(merge % (calc-shape-properties (:content %))))))



;; function copied from day 14 :)
(defn merge-lines [[[from to :as head] & tail :as lines] [from-l to-l :as line]]
  (cond
    (empty? lines) (list line)
    (< (inc to) from-l) (cons head (merge-lines tail line))
    (< (inc to-l) from) (cons line lines)
    :else (merge-lines tail [(min from from-l) (max to to-l)])))

(defn line-overlapping? [[[from to] & tail :as lines] [from-l to-l :as line]]
  (cond
    (empty? lines) false
    (< to from-l) (recur tail line)
    (< to-l from) false
    :else true))

(defn move-out-of-bounds? [x shape-width move]
  (or
   (and (= move \<) (= 0 x))
   (and (= move \>) (= 7 (+ x shape-width)))))

(defn move-to-long [move]
  (case move \< -1 \> 1))

(defn place-the-line [line point]
  (mapv (partial + point) line))

(defn shape-overlapping? [board shape face-indexes [x y] x-offset y-offset]
  (->>
   face-indexes
   (some
    #(line-overlapping? (get board (+ x % x-offset)) (place-the-line (get (:content shape) %) (+ y y-offset))))))

(defn move-left-or-right [board shape [x y :as position] move]
  (if
   (move-out-of-bounds? x (:width shape) move)
    position
    (let [offset (move-to-long move)]
      (if
       (shape-overlapping? board shape (get shape move) position offset 0)
        position
        [(+ x offset) y]))))

(defn move-down [board shape [x y :as position]]
  (if
   (shape-overlapping? board shape (range 0 (:width shape)) position 0 1)
    position
    [x (inc y)]))

(defn insert-shape [board shape [x y]]
  (->>
   (range 0 (:width shape))
   (reduce
    (fn [board' index]
      (update board' (+ index x) merge-lines (place-the-line (get (:content shape) index) y)))
    board)))



(defn calc-init-postion [board shape moves]
  (let [width (:width shape)
        y (- (apply min (map ffirst board)) (:height shape))]
    (loop [counter 0
           x 2
           [move & rest-moves] moves]
      (let [new-x
            (if
             (move-out-of-bounds? x width move)
              x
              (+ x (move-to-long move)))]
        (if (= counter 3)
          [[new-x y] rest-moves]
          (recur (inc counter) new-x rest-moves))))))

(defn try-find-pattern-of-one-rep [stack-of-previous-items next-item]
  (loop [[item & rest-items :as all-items] stack-of-previous-items
         pattern '()]
    (cond
      (empty? all-items)  nil
      (= item next-item) (cons item pattern)
      :else (recur rest-items (cons item pattern)))))


(defn try-find-pattern-of-many-reps [stack-of-previous-items indexes-of-previous-items next-item index-of-next-item reps]
  (when-let [pattern (try-find-pattern-of-one-rep stack-of-previous-items next-item)]
    (let [pattern-length (count pattern)
          pattern-index (- index-of-next-item pattern-length)]
      (when (every?
             (fn [[i p]]
               (= (take reps (get indexes-of-previous-items p))
                  (take reps (iterate #(- % pattern-length) (+ pattern-index i)))))
             (map-indexed vector pattern))
        pattern))))

(defn calc-height-of-board [board]
  (abs (apply min (map ffirst board))))

(defn try-log-first-horizontal-shape-in-a-new-cycle [cycle-length board shape shape-counter position i prev-entry]
  (when (and (nil? position) (= (:type shape) :horizontal))
    (let [cycles (quot i  cycle-length)]
      (when (not= cycles (:cycles prev-entry))
        (let [height (calc-height-of-board board)
              h-diff (- height (:height prev-entry))
              s-diff (- shape-counter (:shapes prev-entry))
              no (rem i cycle-length)]
          {:id {:no no :h-diff h-diff :s-diff s-diff} :cycles cycles :height height :shapes shape-counter})))))

(defn speed-up-with-many-cycles [board shape-counter pattern final-number-of-shapes]
  (let [[pattern-height pattern-shapes]
        (reduce (fn [[h s] {:keys [h-diff s-diff]}] [(+ h h-diff) (+ s s-diff)]) [0 0] pattern)

        shapes-left (- final-number-of-shapes shape-counter)
        full-cycles-left (quot shapes-left pattern-shapes)
        speed-up-shapes (* full-cycles-left pattern-shapes)
        speed-up-height (* full-cycles-left pattern-height)
        board' (mapv #(map (partial mapv (partial + (- speed-up-height))) %) board)
        shape-counter' (+ shape-counter speed-up-shapes)]
    [board' shape-counter']))

(def init-board (into [] (repeat 7 (list [0 0]))))

(def init-log-entry {:height 0 :shapes 0 :cycles -1})


(defn puzzle [text final-number-of-shapes]
  (let [cycle-length (count text)]
    (loop [board init-board
           all-moves (cycle (load-data text))
           shape (first shape-types)
           rest-shapes (rest (cycle shape-types))
           shape-counter 0
           position nil
           i 0
           log-previous-entry init-log-entry
           log-ids '()
           log-map-of-indexes {}
           log-i 0]

      (let [{entry-id :id :as entry} (try-log-first-horizontal-shape-in-a-new-cycle cycle-length board shape shape-counter position i log-previous-entry)
            [pattern log' log-ids' log-map-of-indexes' log-i']
            (if
             (nil? entry)
              [nil log-previous-entry log-ids log-map-of-indexes log-i]
              [(try-find-pattern-of-many-reps log-ids log-map-of-indexes entry-id log-i  5)
               entry (cons entry-id log-ids) (update log-map-of-indexes entry-id (fnil conj '()) log-i) (inc log-i)])]

        (cond
          (= shape-counter final-number-of-shapes)
          (calc-height-of-board board)

          pattern
          (let [[board' shape-counter'] (speed-up-with-many-cycles board shape-counter pattern final-number-of-shapes)]
            (recur board' all-moves shape rest-shapes shape-counter' nil i init-log-entry '() {} 0))

          :else
          (let [[old-position new-position rest-moves i']
                (if
                 (nil? position)
                  (let [[p rest-moves] (calc-init-postion board shape all-moves)]
                    [p (move-down board shape p) rest-moves (+ i 4)])
                  (let [[move & rest-moves] all-moves
                        p (move-left-or-right board shape position move)]
                    [p (move-down board shape p) rest-moves (+ i 1)]))]
            (if
             (= (second old-position) (second new-position))
              (let [new-board (insert-shape board shape new-position)]
                (recur new-board rest-moves (first rest-shapes) (rest rest-shapes) (inc shape-counter) nil i' log' log-ids' log-map-of-indexes' log-i'))
              (recur board rest-moves shape rest-shapes shape-counter new-position i' log' log-ids' log-map-of-indexes' log-i'))))))))

(defn puzzle-1 [text]
  (puzzle text 2022))

(defn puzzle-2 [text]
  (puzzle text 1000000000000))


(comment
  (def file-path "src/advent_of_code_2022/day_17s.txt")

  (def text (slurp file-path))

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)






(defn find-shape-of-type [shapes shape-type]
  (first (filter #(= (:type %) shape-type) shapes)))

(defn normalize-board [board]
  (let [height (calc-height-of-board board)]
    (mapv #(list (mapv (partial + height) (first %))) board)))


((defn tests []

   ;; calc-visible-face
   (doseq [[shape-type [left-expected right-expected]]
           {:horizontal [[[0] [0 0]] , [[3] [0 0]]]
            :vertical [[[0] [0 3]], [[0] [0 3]]]
            :square [[[0] [0 1]], [[1] [0 1]]]
            :cross [[[0 1] [0 2]], [[2 1] [0 2]]]
            :corner [[[0 2] [0 2]], [[2] [0 2]]]}]
     (let [shape (find-shape-of-type shape-types shape-type)]
       (assert (= (calc-visible-face-indexes (:content shape) (range 0 (:width shape))) left-expected))
       (assert (= (calc-visible-face-indexes (:content shape) (reverse (range 0 (:width shape)))) right-expected))))

  ;;  line-overlapping?
   (assert (not (line-overlapping? [] [5 7])))

   (assert (not (line-overlapping? [[10 12]] [5 7])))
   (assert (not (line-overlapping? [[10 12]] [5 9])))

   (assert (not (line-overlapping? [[10 12] [15 17]] [13 14])))
   (assert (not (line-overlapping? [[10 12] [15 17]] [18 19])))

   (assert (line-overlapping? [[10 12] [15 17]] [0 10]))
   (assert (line-overlapping? [[10 12] [15 17]] [12 13]))
   (assert (line-overlapping? [[10 12] [15 17]] [14 16]))
   (assert (line-overlapping? [[10 12] [15 17]] [17 18]))
   (assert (line-overlapping? [[10 12] [15 17]] [14 18]))
   (assert (line-overlapping? [[10 12] [15 17]] [16 16]))

;; move-left-or-right, move-down, insert-shape
   (let [shape-square (find-shape-of-type shape-types :square)
         shape-cross (find-shape-of-type shape-types :cross)
         board
         (->
          init-board
          (update 2 merge-lines [-5 -2])
          (update 3 merge-lines [-4 -2]))]

    ;; move-left-or-right
     (assert (= (move-left-or-right board shape-square  [0 -10] \<) [0 -10]) "can't move")
     (assert (= (move-left-or-right board shape-square  [1 -10] \<) [0 -10]))

     (assert (= (move-left-or-right board shape-square  [5 -10] \>) [5 -10]) "can't move")
     (assert (= (move-left-or-right board shape-square  [4 -10] \>) [5 -10]))

     (assert (= (move-left-or-right board shape-square  [0 -5] \>) [0 -5]) "can't move")
     (assert (= (move-left-or-right board shape-square  [4 -4] \<)  [4 -4]) "can't move")
     (assert (= (move-left-or-right board shape-square  [4 -5] \<)  [4 -5]) "can't move")
     (assert (= (move-left-or-right board shape-square  [4 -6] \<)  [3 -6]))

     (assert (= (move-left-or-right board shape-square  [5 -5] \<)  [4 -5]))
     (assert (= (move-left-or-right board shape-square  [6 -5] \<)  [5 -5]))
     (assert (= (move-left-or-right board shape-square  [5 -4] \<)  [4 -4]))

     (assert (= (move-left-or-right board shape-cross  [4 -6] \<)  [3 -6]))
     (assert (= (move-left-or-right board shape-cross  [4 -5] \<)  [4 -5]) "can't move")

    ;; move-down
     (assert (= (move-down board shape-square  [0 -2]) [0 -2]) "can't move")
     (assert (= (move-down board shape-square  [2 -7]) [2 -7]) "can't move")
     (assert (= (move-down board shape-square  [2 -8]) [2 -7]))
     (assert (= (move-down board shape-square  [3 -6]) [3 -6]) "can't move")
     (assert (= (move-down board shape-square  [3 -7]) [3 -6]))

     (assert (= (move-down board shape-cross  [1 -8]) [1 -8]) "can't move")
     (assert (= (move-down board shape-cross  [1 -9]) [1 -8]))
     (assert (= (move-down board shape-cross  [0 -8]) [0 -7]))

     (assert (= (move-down board shape-cross  [2 -7]) [2 -7]) "can't move")
     (assert (= (move-down board shape-cross  [2 -8]) [2 -7]))

    ;; insert-shape
     (let [board' (insert-shape board shape-square  [0 -2])]
       (assert (= (get board' 0) '([-2 0])))
       (assert (= (get board' 1) '([-2 0]))))

     (let [board' (insert-shape board shape-square  [0 -3])]
       (assert (= (get board' 0) '([-3 -2] [0 0])))
       (assert (= (get board' 1) '([-3 -2] [0 0]))))

     (let [board' (insert-shape board shape-cross [5 -3])]
       (assert (= (get board' 5) '([-2 -2] [0 0])))
       (assert (= (get board' 6) '([-3 0])))
       (assert (= (get board' 7) '([-2 -2])))))

   (let [shape-square (find-shape-of-type shape-types :square)
         shape-horizontal (find-shape-of-type shape-types :horizontal)]


     (assert (= (calc-init-postion init-board shape-square  [\< \< \< \<]) [[0 -2] nil]))

     (assert (= (calc-init-postion init-board shape-square  [\< \< \> \>]) [[2 -2] nil]))
     (assert (= (calc-init-postion init-board shape-square  [\< \> \< \>]) [[2 -2] nil]))
     (assert (= (calc-init-postion init-board shape-square  [\< \< \> \>]) [[2 -2] nil]))

     (assert (= (calc-init-postion init-board shape-square  [\< \< \< \>]) [[1 -2] nil]))

     (assert (= (calc-init-postion init-board shape-square  [\> \> \> \>]) [[5 -2] nil]))

     (assert (= (calc-init-postion init-board shape-square  [\> \> \> \<]) [[4 -2] nil]))
     (assert (= (calc-init-postion init-board shape-square  [\> \> \< \>]) [[4 -2] nil]))
     (assert (= (calc-init-postion init-board shape-square  [\> \< \> \>]) [[4 -2] nil]))

     (assert (= (calc-init-postion init-board shape-horizontal  [\< \< \< \<]) [[0 -1] nil]))

     (assert (= (calc-init-postion init-board shape-horizontal  [\> \> \> \>]) [[3 -1] nil]))
     (assert (= (calc-init-postion init-board shape-horizontal  [\< \> \> \>]) [[3 -1] nil]))
     (assert (= (calc-init-postion init-board shape-horizontal  [\> \< \> \>]) [[3 -1] nil]))

     (assert (= (calc-init-postion init-board shape-horizontal  [\> \> \> \<]) [[2 -1] nil])))

   ;; normalize-board
   (assert (=
            (normalize-board [(list [-10 -5] [-3 0]) (list [-5 -5] [-3 0]) (list [0 0])])
            ['([0 5]) '([5 5]) '([10 10])]))

   ;; last-pattern
   (assert (= (try-find-pattern-of-one-rep (list) 10) nil))
   (assert (= (try-find-pattern-of-one-rep (list 1) 10) nil))
   (assert (= (try-find-pattern-of-one-rep (list 3 2 1) 10) nil))


   (assert (= (try-find-pattern-of-one-rep (list 3 2 1 0) 1) '(1 2 3)))
   (assert (= (try-find-pattern-of-one-rep (list 3 2 1 0) 2) '(2 3)))
   (assert (= (try-find-pattern-of-one-rep (list 3 2 1 0) 3) '(3)))

   ;; find-pattern
   (let [previous-items (list 0 1 2 3 1 2 3)
         stack-of-previous-items (reverse previous-items)
         indexes-of-previous-items  (reduce
                                     (fn [m [index item]]
                                       (update m item (fnil conj '())  index))
                                     {}
                                     (map-indexed vector previous-items))]

     (assert (= (try-find-pattern-of-many-reps stack-of-previous-items indexes-of-previous-items  1 (count previous-items)  1) '(1 2 3)))
     (assert (= (try-find-pattern-of-many-reps stack-of-previous-items indexes-of-previous-items  1 (count previous-items)  2) '(1 2 3)))
     (assert (= (try-find-pattern-of-many-reps stack-of-previous-items indexes-of-previous-items  1 (count previous-items)  3) nil))

     (assert (= (try-find-pattern-of-many-reps stack-of-previous-items indexes-of-previous-items  2 (count previous-items)  1) '(2 3)))
     (assert (= (try-find-pattern-of-many-reps stack-of-previous-items indexes-of-previous-items  2 (count previous-items)  2) nil))

     (assert (= (try-find-pattern-of-many-reps stack-of-previous-items indexes-of-previous-items  3 (count previous-items)  1) '(3)))
     (assert (= (try-find-pattern-of-many-reps stack-of-previous-items indexes-of-previous-items  3 (count previous-items)  2) nil))

     (assert (= (try-find-pattern-of-many-reps stack-of-previous-items indexes-of-previous-items  4 (count previous-items)  1) nil))

     (assert (= (try-find-pattern-of-many-reps stack-of-previous-items indexes-of-previous-items  4 (count previous-items)  1) nil)))
   :rfc))
