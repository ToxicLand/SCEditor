using SCEditor.ScOld;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using static SCEditor.ScOld.MovieClip;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SCEditor.Features
{
    public class renderToPictureBox
    {
        private MovieClipState animationState;
        private ScFile _scFile;
        private Task _timerTask;
        private PeriodicTimer _timer;
        private CancellationTokenSource animationCancelToken = new CancellationTokenSource();
        private RenderingOptions renderOptions;
        private ScObject scData;
        private PictureBox pictureBox;

        public renderToPictureBox(ScFile scFile, RenderingOptions options, ScObject inScData, ref PictureBox pictureBox1)
        {
            _scFile = scFile;
            renderOptions = options;
            scData = inScData;
            pictureBox = pictureBox1;
        }

        public void Start()
        {
            ScObject data = scData;

            if (data.GetDataType() == 7)
                data = ((Export)scData).GetDataObject();

            if (data == null)
                throw new Exception("MainForm:Render() datatype is 1 or 7 but dataobject is null");

            ((MovieClip)data)._lastPlayedFrame = 0;

            TimeSpan interval = TimeSpan.FromMilliseconds((1000 / ((MovieClip)data).FPS));
            _timer = new PeriodicTimer(interval);

            _timerTask = renderAnimation(renderOptions, (MovieClip)data, pictureBox);
        }

        private async Task renderAnimation(RenderingOptions options, MovieClip data, PictureBox pictureBox1)
        {
            try
            {
                int totalFrameTimelineCount = 0;
                foreach (MovieClipFrame frame in (data).GetFrames())
                {
                    totalFrameTimelineCount += (frame.Id * 3);
                }

                if ((data).timelineArray.Length % 3 != 0 || (data).timelineArray.Length != totalFrameTimelineCount)
                {
                    await stopRendering();
                    MessageBox.Show("MoveClip timeline array length is not set equal to total frames count.");
                    return;
                }

                (data).initPointFList(null, animationCancelToken.Token);

                Console.WriteLine("Started Playing!");

                while (await _timer.WaitForNextTickAsync(animationCancelToken.Token))
                {
                    animationState = MovieClipState.Playing;

                    int frameIndex = (data)._lastPlayedFrame;

                    Bitmap image = (data).renderAnimation(new RenderingOptions() { /**ViewPolygons = viewPolygonsToolStripMenuItem.Checked**/ }, frameIndex);

                    if (image == null)
                    {
                        await stopRendering();
                        MessageBox.Show($"Frame Index {frameIndex} returned null image.");
                        return;
                    }

                    pictureBox1.Invoke((Action)(delegate
                    {
                        pictureBox1.Image = image;
                        pictureBox1.Refresh();
                    }));

                    image.Dispose();

                    if ((frameIndex + 1) != (data).GetFrames().Count)
                        (data)._lastPlayedFrame = frameIndex + 1;
                    else
                        (data)._lastPlayedFrame = 0;
                }

                if (animationCancelToken.IsCancellationRequested)
                    return;

            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(OperationCanceledException) && ex.GetType() != typeof(TaskCanceledException))
                {
                    MessageBox.Show(ex.Message);
                }
            }

            await stopRendering();
        }

        public async Task stopRendering()
        {
            if (_timerTask is null)
            {
                return;
            }

            animationCancelToken.Cancel();

            if (!_timerTask.IsCompleted && _timerTask.Status != TaskStatus.WaitingForActivation)
            {
                await _timerTask;
            }

            _timerTask = null;

            if (_scFile != null)
            {
                if (_scFile.CurrentRenderingMovieClips.Count > 0)
                {
                    foreach (MovieClip mv in this._scFile.CurrentRenderingMovieClips)
                    {
                        mv._animationState = MovieClipState.Stopped;
                        mv._lastPlayedFrame = 0;
                        mv.destroyPointFList();
                    }

                    _scFile.setRenderingItems(new List<ScObject>());
                }
            }

            if (animationState == MovieClipState.Playing)
                animationState = MovieClipState.Stopped;

            Console.WriteLine("Animation Stopped!");
        }
    }
}