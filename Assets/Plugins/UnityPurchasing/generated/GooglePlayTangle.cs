#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("rywiLR2vLCcvrywsLbNfqepw3Q8FXy2yPliBml1TxcDBURcSJZn9kg0U4fnRUVgFYZsb2Knzgc2sW/TSHa8sDx0gKyQHq2Wr2iAsLCwoLS5sT6lUSzbeYAtzwyMCgYNkadvGSOBF4rWACuaxJkTQaPeJeBnv4y+8xYbRghRs2StVbj8tihICMvhfEJ4dPgaIflv7zGKcJarM0R6yXU7at/3/nd1VwsJtoHGz4Y4SNon0mjasarrDqZcsp1Zxsf+KKdTXHohAoPsXyI/E/hu8rxGk0IUka/85/uFO/v+us/nLrozRh8VOGXyYbpIiMko7MUctDp0Jn4MgRkzymQcpojfhp1yMFj6voAtEsF2/u+s7wn6pv0nwS1YuU5KFcNP4di8uLC0s");
        private static int[] order = new int[] { 1,10,5,10,6,5,6,13,8,13,10,13,12,13,14 };
        private static int key = 45;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
