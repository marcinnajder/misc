(ns advent-of-code-2022.core
  (:require advent-of-code-2022.day-01)
  (:require advent-of-code-2022.day-02)
  (:require advent-of-code-2022.day-03)
  (:require advent-of-code-2022.day-04)
  (:require advent-of-code-2022.day-05)
  (:require advent-of-code-2022.day-06)
  (:require advent-of-code-2022.day-07)
  (:require advent-of-code-2022.day-08)
  (:require advent-of-code-2022.day-09)
  (:require advent-of-code-2022.day-10)
  (:require advent-of-code-2022.day-11)
  (:require advent-of-code-2022.day-12)
  (:gen-class))

(defn -main
  "I don't do a whole lot ... yet."
  [& args]
  (println "Hello, World!")

  (let [results
        [(let [text (slurp "src/advent_of_code_2022/day_01.txt")] [(advent-of-code-2022.day-01/puzzle-1 text) (advent-of-code-2022.day-01/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_02.txt")] [(advent-of-code-2022.day-02/puzzle-1 text) (advent-of-code-2022.day-02/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_03.txt")] [(advent-of-code-2022.day-03/puzzle-1 text) (advent-of-code-2022.day-03/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_04.txt")] [(advent-of-code-2022.day-04/puzzle-1 text) (advent-of-code-2022.day-04/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_05.txt")] [(advent-of-code-2022.day-05/puzzle-1 text) (advent-of-code-2022.day-05/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_06.txt")] [(advent-of-code-2022.day-06/puzzle-1 text) (advent-of-code-2022.day-06/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_07.txt")] [(advent-of-code-2022.day-07/puzzle-1 text) (advent-of-code-2022.day-07/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_08.txt")] [(advent-of-code-2022.day-08/puzzle-1 text) (advent-of-code-2022.day-08/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_09.txt")] [(advent-of-code-2022.day-09/puzzle-1 text) (advent-of-code-2022.day-09/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_10.txt")] [(advent-of-code-2022.day-10/puzzle-1 text) (advent-of-code-2022.day-10/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_11.txt")] [(advent-of-code-2022.day-11/puzzle-1 text) (advent-of-code-2022.day-11/puzzle-2 text)])
         (let [text (slurp "src/advent_of_code_2022/day_12.txt")] [(advent-of-code-2022.day-12/puzzle-1 text) (advent-of-code-2022.day-12/puzzle-2 text)])]]
    (println results)))
