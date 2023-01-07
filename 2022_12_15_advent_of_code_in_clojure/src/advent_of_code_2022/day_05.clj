(ns advent-of-code-2022.day-05
  (:require [clojure.string :as string])
  (:gen-class))

(defn parse-moves [lines]
  (map (fn [line]
         (let [[quantity from to] (map parse-long (re-seq #"\d+" line))]
           {:quantity quantity :from (dec from) :to (dec to)}))
       lines))

(defn parse-crates [lines]
  (let [[stack-numbers-line & crates-lines] (reverse lines)
        indexes (into [] (range 1 (dec (count stack-numbers-line)) 4))
        init-crates (into [] (repeat (count indexes) '()))]
    (reduce
     (fn [crates line]
       (reduce
        (fn [crates' [crate-index char-index]]
          (let [char (nth line char-index)]
            (if (not= char \space)
              (update-in crates' [crate-index] conj char)
              crates')))
        crates
        (map-indexed vector indexes)))
     init-crates
     crates-lines)))


(defn load-data [text]
  (let [[crates-lines [_ & move-lines]]
        (->> text
             string/split-lines
             (split-with (comp not string/blank?)))]
    {:crates (parse-crates crates-lines) :moves (parse-moves move-lines)}))



(defn move-one-by-one [from-stack to-stack quantity]
  (loop [n quantity
         [head & tail :as from-stack'] from-stack
         to-stack' to-stack]
    (if (zero? n)
      [from-stack' to-stack']
      (recur (dec n) tail (cons head to-stack')))))


(defn move-all-at-once [from-stack to-stack quantity]
  (loop [n quantity
         [head & tail :as from-stack'] from-stack
         moved-items-stack '()]
    (if (zero? n)
      [from-stack' (into to-stack moved-items-stack)]
      (recur (dec n) tail (cons head moved-items-stack)))))


(defn puzzle [text mover]
  (let [{:keys [crates  moves]} (load-data text)
        final-crates
        (reduce
         (fn [crates' move]
           (let [from-index (:from move)
                 to-index (:to move)
                 [from-stack to-stack]
                 (mover (get crates' from-index) (get crates' to-index)  (:quantity move))]
             (->
              crates'
              (assoc from-index from-stack)
              (assoc to-index to-stack))))
         crates
         moves)]
    (apply str (map first final-crates))))

(defn puzzle-1 [text]
  (puzzle text move-one-by-one))

(defn puzzle-2 [text]
  (puzzle text move-all-at-once))





(comment
  (def file-path "src/advent_of_code_2022/day_05.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)



;; (defn parse-moves [lines]
;;   (->>
;;    lines
;;    (map (partial re-seq #"\d+"))
;;    (map (fn [[quantity from to]] {:quantity quantity :from from :to to}))))


;; (let [{crates :crates [move] :moves} (load-data  text)]
;;   (def crates crates)
;;   (def move move)
;;   (def quantity (:quantity move))
;;   (def from (:from move))
;;   (def to (:to move))
;;   [crates move])


;; (defn puzzle-1 [text]
;;   (let [final-crates
;;         (let [{:keys [crates  moves]} (load-data text)]
;;           (reduce
;;            (fn [state move]
;;              (reduce
;;               (fn [state' _]
;;                 (let
;;                  [from-index (dec (:from move))
;;                   to-index (dec (:to move))
;;                   head (first (get state' from-index))]
;;                   (->
;;                    state'
;;                    (update from-index rest)
;;                    (update to-index conj head))))
;;               state
;;               (repeat (:quantity move) nil)))
;;            crates
;;            moves))]

;;     (apply str (map first final-crates))))


;; (defn move-one-by-one [from-stack to-stack quantity]
;;   (reduce
;;    (fn [[[head & tail] to-stack] _]
;;      [tail (cons head to-stack)])
;;    [from-stack to-stack]
;;    (repeat quantity nil)))