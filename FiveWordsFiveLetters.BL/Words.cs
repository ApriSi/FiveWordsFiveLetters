namespace FiveWordsFiveLetters.BL
{
    public delegate void GetUpdateProgress(int progress, int length);
    public delegate void FinishedProgress();
    public delegate void UpdateAllCombinations();
    public static class Words
    {
        public static event GetUpdateProgress? GetUpdateProgressEvent;
        public static event FinishedProgress? FinishedEvent;
        public static event UpdateAllCombinations? UpdateAllCombinationsEvent;

        public static string? Path;
        public static string? AllCombinations;

        private static string[] ReadFileToList(string path) {
            var file = File.ReadAllLines(path);
            return file;
        }
        private static string[] GetFilteredWords(string[] words) {
            return words.Where(w => w.All(char.IsLetter) && w.Distinct().Count() == w.Length && w.Length == 5).ToArray();
        }

        private static string[] RemoveAnagrams(string[] words) {

            var noAnagrams = new List<string>();
            var result = new List<string>();

            foreach (var word in words) {
                var arr1 = word.ToCharArray();
                Array.Sort(arr1);
                var sorted = new string(arr1);

                if (!noAnagrams.Contains(sorted))
                {
                    noAnagrams.Add(sorted);
                    result.Add(word);
                }
            }
            return result.ToArray();
        }

        private static uint GetBinaryWord(string word) {
            uint bits = 0;

            for (int i = 0; i < word.Length; i++)
            {
                bits += ((uint)1) << (word[i] - 'a');
            }

            return bits;
        }

        private static bool CompareBinary(uint binary1, uint binary2) {
            return (binary1 & binary2) == 0;
        }

        public static void CombineWords() {
            try {
                var words = ReadFileToList(Path);
                var filteredWords = RemoveAnagrams(GetFilteredWords(words));
                Dictionary<uint, string> wordDictionary = new Dictionary<uint, string>();
                var binaryWordList = new List<uint>();


                foreach (var word in filteredWords)
                {
                    var binaryWord = GetBinaryWord(word.ToLower());
                    wordDictionary.Add(binaryWord, word.ToLower());
                    binaryWordList.Add(binaryWord);
                }

                BinaryLoop(binaryWordList.ToArray(), wordDictionary);
            } catch (Exception ex) {
                throw new Exception(ex.Message);
            }
        }

        public static void BinaryLoop(uint[] binaryWords, Dictionary<uint, string> wordDictionary) {
            int splitCount = 4;

            for (int i = 0; i < splitCount; i++) {
                int startCount = (binaryWords.Count() / splitCount) * i;
                int endCount = (binaryWords.Count() / splitCount) * (i + 1);

                if (i == splitCount) {
                    endCount = binaryWords.Count();
                }

                var thready = new Thread(() => {
                    for (int x = startCount; x < endCount; x++) {
                        FindWords(binaryWords, wordDictionary, new List<uint>() { binaryWords[x] }, x, binaryWords[x]);
                        GetUpdateProgressEvent!.Invoke(1, binaryWords.Count());
                    }
                });

                thready.Start();
            }

            //FinishedEvent!.Invoke(); STUPID SHEIT NOT FUCKING WORKING, WELL IT IS BUT IT*S FUCKING😏😏😏😏😏😏 FUUUUUUCK 😡😡😡😡😡 😭😭😭😭😭😭
        }

        public static void FindWords(uint[] binaryWords, Dictionary<uint, string> wordDictionary, List<uint> currentBinary, int pos, uint arg)
        {
            if (currentBinary.Count() == 5)
            {
                foreach (uint word in currentBinary)
                {
                    //Console.Write(wordDictionary[word] + ", ");
                    AllCombinations += $"{wordDictionary[word]}, ";
                }

                AllCombinations += "\n";
                UpdateAllCombinationsEvent!.Invoke();
                return;
            }
            for (int i = pos + 1; i < binaryWords.Length; i++)
            {
                if ((arg & binaryWords[i]) == 0 && currentBinary.Count() < 5)
                {
                    List<uint> binaryList = new List<uint>();
                    binaryList.AddRange(currentBinary);
                    binaryList.Add(binaryWords[i]);

                    FindWords(binaryWords, wordDictionary, binaryList, i, arg | binaryWords[i]);
                }
            }
        }
    }
}