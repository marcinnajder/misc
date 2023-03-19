(ns advent-of-code-2022.day-13
  (:require [clojure.string :as string])
  (:gen-class))


(defn load-data-parsing-line [text parse-line]
  (->>
   (string/split-lines text)
   (partition 2 3)
   (map (partial mapv parse-line))))

(defn load-data [text]
  (load-data-parsing-line text read-string))


(defn compare-packages [left right]
  (cond
    (and (number? left) (number? right))
    (compare left right)

    (number? left)
    (compare-packages [left] right)

    (number? right)
    (compare-packages left [right])

    :else
    (let [result (some #{1 -1} (map compare-packages left right))]
      (if (nil? result)
        (compare (count left) (count right))
        result))))


(defn puzzle-1 [text]
  (->>
   (load-data text)
   (keep-indexed
    (fn [index [package-1 package-2]]
      (if (= -1 (compare-packages package-1 package-2))
        (inc index)
        nil)))
   (apply +)))


(defn puzzle-2 [text]
  (let [all-packages (mapcat identity (load-data text))
        {greater-1 1 lower-1 -1} (group-by #(compare-packages % [[2]]) all-packages)
        lower-2 (filter #(= -1 (compare-packages % [[6]])) greater-1)]
    (* (+ (count lower-1) 1) (+ (count lower-1) 1 (count lower-2) 1))))


(comment
  (def file-path "src/advent_of_code_2022/day_13.txt")

  (def text (slurp file-path))

  (load-data text)

  (puzzle-1 text)

  (puzzle-2 text)

  :rfc)





;; manual parsing input text without validating lists

(def tokens-regex #"\d+|\[|\]")

(defn read-tokens [tokens]
  (let [[first-token & rest-tokens] tokens]
    (cond
      (nil? first-token)
      ['() nil]

      (= first-token "]")
      ['(), rest-tokens]

      (= first-token "[")
      (let [[item rest-tokens] (read-tokens rest-tokens)
            [items rest-tokens] (read-tokens rest-tokens)]
        [(cons item items) rest-tokens])

      :else
      (let [[items rest-tokens] (read-tokens rest-tokens)]
        [(cons (parse-long first-token) items) rest-tokens]))))

(defn load-data-2 [text]
  (load-data-parsing-line text #(ffirst (read-tokens (re-seq tokens-regex %))))) ;; ffirst


;; manual parsing input text correctly validating lists

(declare read-list) ;; mutually recursive function

(defn read-token [tokens]
  (let [[first-token & rest-tokens] tokens]
    (cond
      (nil? first-token)
      [nil, nil]

      (= first-token "[")
      (read-list rest-tokens)

      :else
      [(parse-long first-token) rest-tokens])))


(defn read-list [tokens]
  (let [[first-token & rest-tokens] tokens]
    (cond
      (nil? first-token)
      (throw (Exception. "list in not closed"))

      (= first-token "]")
      ['() rest-tokens]

      :else
      (let [[item rest-tokens] (read-token tokens)
            [items rest-tokens] (read-list rest-tokens)]
        [(cons item items) rest-tokens]))))

(defn load-data-3 [text]
  (load-data-parsing-line text #(first (read-token (re-seq tokens-regex %))))) ;; first



(comment

  (read-tokens (re-seq tokens-regex "1,2")) ;; [(1 2) nil]
  (read-tokens (re-seq tokens-regex "[1,2")) ;; [(1 2) nil]
  (read-tokens (re-seq tokens-regex "1,[2,3],[[4]],5")) ;; [(1 (2 3) ((4)) 5) nil]

  (read-token (re-seq tokens-regex "1,2")) ;; [1 ("2")]
  (read-token (re-seq tokens-regex "[1,2")) ;; error: list in not closed
  (read-tokens (re-seq tokens-regex "[[2,3],[[4]]]")) ;; [(((2 3) ((4)))) nil]

  (->>
   (map = (load-data text) (load-data-2 text) (load-data-3 text))
   (into #{})) ;; => #{true} this means that all implementations return the same result

  :rfc)


