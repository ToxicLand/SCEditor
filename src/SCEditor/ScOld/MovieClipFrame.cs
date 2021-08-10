namespace SCEditor.ScOld
{
    public unsafe class MovieClipFrame : ScObject
    {
        private ushort* _timeline;
        private string _name;
        private ScFile _scFile;
        public MovieClipFrame(ScFile scfile)
        {
            _scFile = scfile;
            _name = null;
        }

        public ushort* GetTimeline()
        {
            return _timeline;
        }
        public void SetTimeline(ushort* timeline)
        {
            this._timeline = timeline;
        }

        public void SetName(string name)
        {
            _name = name;
        }

        public void SetId(ushort id)
        {
            Id = id;
        }

        public string Name => _name;

        public int GetAmountOfChildMemoryNeeded(MovieClipFrame frame, int frameId, int previousFrameId)
        {
            int tmp = 0;

            for (int i = 0; i < frameId; i++)
            {
                ushort nextTimeline = frame._timeline[i * 3];

                for (int j = 0; j < previousFrameId; j++)
                {
                    ushort timeline = _timeline[i * 3];

                    if (timeline == nextTimeline)
                    {
                        tmp++;
                        break;
                    }
                }
            }

            return frameId + previousFrameId - tmp;
        }
    }
}
