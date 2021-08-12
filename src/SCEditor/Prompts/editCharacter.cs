using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCEditor.ScOld;

namespace SCEditor.Prompts
{
    public partial class editCharacter : Form
    {
        private ScObject _data;
        private bool zoomed;
        private double[] position;
        private double[] size;

        public editCharacter()
        {
            InitializeComponent();
            this.pictureBox1.MouseWheel += pictureBox1_MouseWheel;

            position = new double[4] { 0, 0, 0, 0};
            size = new double[2] { 0, 0 };
        }   

        public void Render()
        {
            if (treeView1.SelectedNode?.Tag != null)
            {
                RenderingOptions options = new RenderingOptions()
                {
                    ViewPolygons = false
                };

                ScObject selectedData = (ScObject)treeView1.SelectedNode.Tag;

                pictureBox1.Image = selectedData.Render(options);
                pictureBox1.SizeMode = PictureBoxSizeMode.AutoSize;

                if (selectedData.Children == null && selectedData.Bitmap != null)
                {
                    pictureBox1.Size = new Size(selectedData.Bitmap.Width, selectedData.Bitmap.Height);
                }
                else if (selectedData.Children != null)
                {
                }

                pictureBox1.Refresh();
            }
        }

        private void resizeData(editType type)
        {
            float sf = 0.25F;

            if (treeView1.SelectedNode?.Tag != null)
            {
                ScObject selectedData = (ScObject) treeView1.SelectedNode.Tag;

                if (selectedData.GetDataType() == 0)
                {
                    Shape shapeData = (Shape) selectedData;

                    for (int s = 0; s < shapeData.GetChunks().Count; s++)
                    {
                        PointF[] xyData = ((ShapeChunk) shapeData.GetChunks()[s]).XY;

                        for (int i = 0; i < xyData.Length; i++)
                        {
                            switch (type)
                            {
                                case editType.PositionUp:
                                    xyData[i].Y = xyData[i].Y > 0 ? xyData[i].Y - sf : ((Math.Abs(xyData[i].Y) + sf) * -1);
                                    break;

                                case editType.PositionDown:
                                    xyData[i].Y += sf;
                                    break;

                                case editType.PositionLeft:
                                    xyData[i].X = xyData[i].X > 0 ? xyData[i].X -= sf : ((Math.Abs(xyData[i].X) + sf) * -1);
                                    break;

                                case editType.PositionRight:
                                    xyData[i].X += sf;
                                    break;
                            }
                        }
                    }

                    Render();
                }
            }  
        }

        public void addData()
        {
            treeView1.Populate(new List<ScObject> { _data });
        }

        public void setScData(ScObject data)
        {
            _data = data;
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            pictureBox1.Image = null;
            Render();
            zoomed = false;
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            ScObject node = (ScObject)treeView1.SelectedNode.Tag;

            if (node == null)
                return;

            int width = 0;
            int height = 0;

            if (node.Bitmap != null)
            {
                width = node.Bitmap.Width;
                height = node.Bitmap.Height;
            } 
            else if (node.GetDataType() == 7 || node.GetDataType() == 0)
            {
                List<PointF> A = new List<PointF>();

                if (node.GetDataType() == 7)
                {
                    foreach (Shape s in node.Children)
                    {
                        PointF[] pointsXY = s.Children.SelectMany(chunk => ((ShapeChunk)chunk).XY).ToArray();
                        A.AddRange(pointsXY.ToArray());
                    }
                }
                else
                {
                    PointF[] pointsXY = ((Shape)node).GetChunks().SelectMany(chunk => ((ShapeChunk)chunk).XY).ToArray();
                    A.AddRange(pointsXY.ToArray());
                }

                var xyPath = new GraphicsPath();

                xyPath.AddPolygon(A.ToArray());

                var xyBound = Rectangle.Round(xyPath.GetBounds());

                width = xyBound.Width;
                height = xyBound.Height;
            }

            if (zoomed != true)
            {
                pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                pictureBox1.Size = new Size(width, height);
            }

            zoomed = true;

            double sf = 0.125;

            if (e.Delta > 0)
            {
                if ((width * 5.5) >= pictureBox1.Width)
                {
                    pictureBox1.Size = new Size((pictureBox1.Size.Width + (int)(pictureBox1.Size.Width * sf)), (pictureBox1.Size.Height + (int)(pictureBox1.Size.Height * sf)));
                    pictureBox1.Width = pictureBox1.Width + (int)(pictureBox1.Width * sf);
                    pictureBox1.Height = pictureBox1.Height + (int)(pictureBox1.Height * sf);
                }
            }
            else
            {
                if ((height / 5.5) <= pictureBox1.Width)
                {
                    pictureBox1.Size = new Size((pictureBox1.Size.Width - (int)(pictureBox1.Size.Width * sf)), (pictureBox1.Size.Height - (int)(pictureBox1.Size.Height * sf)));
                    pictureBox1.Width = pictureBox1.Width - (int)(pictureBox1.Width * sf);
                    pictureBox1.Height = pictureBox1.Height - (int)(pictureBox1.Height * sf);
                }
            }
        }

        private void editCharacterButtonSizeIncrease_Click(object sender, EventArgs e)
        {
            size[0] += 0.05;
            resizeData(editType.IncreaseSize);
        }

        private void editCharacterButtonSizeDecrease_Click(object sender, EventArgs e)
        {
            size[1] += 0.05;
            resizeData(editType.DecreaseSize);
        }

        private void editCharacterButtonUp_Click(object sender, EventArgs e)
        {
            position[0] += 0.05;
            resizeData(editType.PositionUp);
        }

        private void editCharacterButtonLeft_Click(object sender, EventArgs e)
        {
            position[2] += 0.05;
            resizeData(editType.PositionLeft);
        }

        private void editCharacterButtonRight_Click(object sender, EventArgs e)
        {
            position[3] += 0.05;
            resizeData(editType.PositionRight);
        }

        private void editCharacterButtonDown_Click(object sender, EventArgs e)
        {
            position[1] += 0.05;
            resizeData(editType.PositionDown);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("OK", "OK");
        }

        private enum editType
        {
            IncreaseSize,
            DecreaseSize,
            PositionUp,
            PositionDown,
            PositionLeft,
            PositionRight
        }
    }
}
