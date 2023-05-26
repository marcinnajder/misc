(ns advent-of-code-2022.day-13-packets
  (:require [clojure.string :as string])
  (:gen-class))


;; (read-string "1")

(defn load-data-parsing-line [text parse-line]
  (->>
   (string/split-lines text)
   (partition 2 3)
   (map (partial mapv parse-line))))

(defn load-data [text]
  (load-data-parsing-line text read-string))



;; (assert (= (compare-packages 1 1) 0))

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


;; https://www.typescriptlang.org/play?#code/GYVwdgxgLglg9mABBOBbADgQwE4FMByIqARrtgBSYBcYRp2ANIsVYrSWQJQDeAUIgMR4oIbEkyIAtMwDciAPTyhuEWMQSAvFuaIA-IgAMiVhIA8O-ZICMxxFZm8Avr16LEAKneIA7jCgALRBgwAGcoTEhcOGAXTGIw7ExoZAAbTBCQxAAFJIBrFT5BdXioROSUDBxccnRWHIh8qE5WdnoHZ14INIzEADVMFJBcesbEXAAPKFwwABNMkYL+QRRQ0pBoOAp0EGIUmAhEADcBoZo6LkRuRCWigRCQdDJyTgci5yKKrDwauryVZrY52wlxut2EoiQ6CCqwiECiwD6J2GfygoNugn0nyqhA4FACMBCADpjoNcEx0MSkZw0ejbGBcN5EAAZAlQBZQcgAbXxIQAupxCVjvugXqDHO0XF10pkWWF2WNJtM5tkUSCPggEusoJsajs9gc-LhUCEqOzOfzLtdaYh7o8KKK3qChdVaiqGv8WkC1bSVmEglNjYgNIgocEwrD4czWfL9BTDYHWDTrcnEDVoeHItFEaSY4hOeheXSQCkUqKk4hgDqUioghoDHIYOYALKYAKE1DBcg8wnxonVsAAcwCTF7AEJCf2h-4XkEANSznjlorVqDKe4pVfB7u9zkwXmCtBfapj3f814poLAch4der0faAyLi9glQQtfFqDnlPvWk-9HgtRnRxegu38AkewDPtpinEdIPHScAgdQRxScFw3E8Hw-H8OAQFXMNwkzGJeDiBIklXKUenZAAmQpBBI0oyOQQ8qh+N1GiogFWjIBxQXospyOYvAABU4H6UlWLEoZqM4oEv2KUjykE3ARNlDlXVU6TPVxCVOm6TJJORd0oCohUplmeYURop0NTWDYtj1fYjiRM5cU4S1y1tJ4kIEP8mMqYVfiMjitPob1-1fNQKWdESDNAgk3K8aw5JQ6z-OU0SkQkpFNMBXFaPC1QkCAoE4qJEkhnJSlSW8xBfOiuBVNYjTLJkvLywApB6UZZqgq5Hl+QPNKahq5wOgomVo0s0ylQsoKwqY1ZsC1HVtl2Rze1NSzzTcq4PIeLzktSo9WJyrjgXyl9CpDQajxU1lSoSqR7DFI6qhizLXQM06gQuooOsQbtnXILqozlSyuQLTgRte4SGvu9TJqC1r6F+wRKwoFda3rIJm1bfx207bdIInaDh2u3sScHRCG3nJ8L0xm8PyDAGwKJHc9xuli40g08autGAr0ZjdEHvYNH1RlN-qFz8l2Q8tfL+iKiqU4CniJo0oKp-xKophDp0OjpXCUDCQkwDBqxCFxQEgWAEAVXAIFw6pgCoAAxcBoHgMA6YEX04GrCc4AHchgEJbUAGVSmCYPOCYK8oZ03hMd6LJma+yyHExplU+DHr2J4zHDisIxgxBlPyGL6lC6sABWZmy6yCua9FQu69Lhk+kb5ueImB2nfIVM3I0AA+I5i8574i8fUVe8dqYB+eINR6nifqiL5uZ-GPv58Hpex4MVfyEODee63ufql3ke2A77OuWPpgp4GoGQdvzl7-3-kE94Wf+8v0eX8bm-GuD9i5PyUsDG+gD37H0-pvbeF9F5XwAXfYBH8BTP0gSgkBNdYGn3gQvIe-9MFAJAQYMBQ1kEkP3g-HBUM4HnwIXvSh0Di40PIcdZhqDH50LwQwv+19upQNoYfY+X8f470QcvGuh9OG4JcOIhBhCBHMiEewliU8xFn1-pIo40iMGCLvqAnhLggA

;; function compareNumber(a:number, b: number){
;;     return a - b; // return a === b ? 0 : a < b ? -1 : 1;
;; }

;; // ** with instanceof

;; abstract class Packet{
;;     abstract compare(p: Packet): number;
;; }

;; class ValuePacket extends Packet{
;;     constructor(public value:number) { 
;;         super();
;;     }
;;     compare(p: Packet): number {
;;         return p instanceof ValuePacket
;;             ? compareNumber(this.value, p.value)
;;             : new ListPacket([this]).compare(p);
;;     };
;; }

;; class ListPacket extends Packet {
;;     constructor(public items:Packet[]) { 
;;         super();
;;     }
;;     compare(p: Packet): number {
;;         const items = p instanceof ListPacket ? p.items : 
;;                         (p instanceof ValuePacket ? [p] : null);

;;         for(let i=0; i< Math.min(this.items.length, items!.length); i++){
;;             let result = this.items[i].compare(items![i]);
;;             if(result !== 0){
;;                 return result;
;;             }
;;         }
;;         return compareNumber(this.items.length, items!.length);
;;     };
;; }

;; // ** without instanceof

;; abstract class Packet2{
;;     abstract compare(p: Packet2): number;

;;     abstract compareToValue(p: ValuePacket2): number;
;;     abstract compareToList(p: ListPacket2): number;
;; }

;; class ValuePacket2 extends Packet2{
;;     constructor(public value:number) { 
;;         super();
;;     }
;;     compare(p: Packet2): number {
;;         return p.compareToValue(this) * -1;
;;     };

;;     compareToValue(p: ValuePacket2): number{
;;         return compareNumber(this.value, p.value);
;;     }
;;     compareToList(p: ListPacket2): number{
;;         return new ListPacket2([this]).compare(p);
;;     }
;; }

;; class ListPacket2 extends Packet2 {
;;     constructor(public items:Packet2[]) { 
;;         super();
;;     }
;;     compare(p: Packet2): number {
;;         return p.compareToList(this) * -1;
;;     }
;;     compareToValue(p: ValuePacket2): number{
;;         return this.compare(new ListPacket2([p]));
;;     }
;;     compareToList(p: ListPacket2): number{
;;         for(let i=0; i< Math.min(this.items.length, p.items.length); i++){
;;             let result = this.items[i].compare(p.items[i]);
;;             if(result !== 0){
;;                 return result;
;;             }
;;         }
;;         return compareNumber(this.items.length, p.items.length);
;;     }
;; }


;; // ** samples

;; function execute(f:Function){
;;     console.log(f.toString(), f());
;; }


;; let VP = ValuePacket2;
;; let LP = ListPacket2;

;; let v10 = new VP(10)
;; let v15 = new VP(15);
;; let v5 = new VP(5);

;; execute( () => v10.compare(v10));
;; execute( () => v10.compare(v15));
;; execute( () => v10.compare(v5));

;; execute( () => new LP([v5, v10]).compare(new LP([v5, v10])));
;; execute( () => new LP([v5, v10]).compare(new LP([v5, v5])));
;; execute( () => new LP([v5, v10]).compare(new LP([v5, v15])));

;; execute( () => new LP([v5, v10]).compare(new LP([v5, v10, v5])));
;; execute( () => new LP([v5, v10, v5]).compare(new LP([v5, v10])));

;; execute( () => new LP([v5]).compare(v5));
;; execute( () => v5.compare(new LP([v5])));

;; execute( () => new LP([v5]).compare(v10));
;; execute( () => v5.compare(new LP([v10])));

