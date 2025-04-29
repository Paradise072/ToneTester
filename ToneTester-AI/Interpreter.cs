namespace ToneTester_AI
{
    public struct SentenceData
    {
        public string type;
        public bool success;
        public float confidence;
        public float[] score;
    }
    public static class Interpreter
    {
        
        public static SentenceData GetMostProminent(string input, bool loadPieValues = false)
        {
            SentenceData data = new SentenceData();
            dynamic sampleData;
            dynamic result;
            sampleData = new Dataset.ModelInput()
            {
                Col0 = input,
            };
            result = Dataset.Predict(sampleData);

            if (result != null)
            {
                data.type = result.PredictedLabel;
                data.confidence = result.Score[1];
                data.score = result.Score;
                data.success = true;
            }

            return data;
        }
    }
}