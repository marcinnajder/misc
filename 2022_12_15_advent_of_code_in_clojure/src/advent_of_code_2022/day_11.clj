(ns advent-of-code-2022.day-11
  (:require [clojure.string :as string])
  (:gen-class))

(defn parse-numbers [line]
  (mapv parse-long (re-seq #"\d+" line)))

(defn load-data [text]
  (->>
   text
   string/split-lines
   (partition-all 7)
   (map vec)
   (mapv (fn [lines]
           (assoc
            (zipmap [:op-value :if-test :if-true :if-false]
                    (map #(first (parse-numbers (get lines %))) [2 3 4 5]))
            :op-op (if (string/includes? (get lines 2) "+") + *)
            :worry-levels (parse-numbers (get lines 1))
            :inspects-count 0)))))

(defn play-monkey [monkeys index level-reducer]
  (let [{:keys [worry-levels op-value op-op if-test if-true if-false]} (get monkeys index)
        monkeys (reduce
                 (fn [monkeys worry-level]
                   (let [worry-level (op-op (or op-value worry-level) worry-level)
                         worry-level (level-reducer worry-level)
                         monkey-index (if (= (mod worry-level if-test) 0)
                                        if-true
                                        if-false)]
                     (update-in monkeys [monkey-index :worry-levels] conj worry-level)))
                 monkeys
                 worry-levels)]

    (->
     monkeys
     (update-in [index :inspects-count] + (count worry-levels))
     (assoc-in [index :worry-levels] []))))


(defn play-round [monkeys level-reducer]
  (reduce #(play-monkey %1 %2 level-reducer) monkeys (range 0 (count monkeys))))


(defn puzzle [monkeys number-of-rounds level-reducer]
  (->>
   (range number-of-rounds)
   (reduce
    (fn [monkeys _]
      (play-round monkeys level-reducer))
    monkeys)
   (map :inspects-count)
   (sort >)
   (take 2)
   (apply *)))


(defn puzzle-1 [text]
  (puzzle (load-data text) 20 #(quot % 3)))

(defn puzzle-2 [text]
  (let [monkeys (load-data text)
        magic-number (apply * (map :if-test monkeys))
        modulo-by-magic-number (fn [level] (mod level magic-number))]
    (puzzle monkeys 10000 modulo-by-magic-number)))


(comment
  (def file-path "src/advent_of_code_2022/day_11.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)