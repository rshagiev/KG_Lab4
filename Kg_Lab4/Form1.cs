using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Platform.Windows;
using Tao.FreeGlut;

namespace Kg_Lab4
{
    public partial class Form1 : Form
    {
        
        float rotationSpeed = 0.2f;
        float rotationAngle = 20;
        float scale = 0.8f;
        //Rotation
        float rotateX;
        float rotateY;
        float McoordX = 0, McoordY = 0;
        float MlastX = 0, MlastY = 0;
        bool mouseButton = false;

        HyperPar pv1 = new HyperPar();
        EliPar pv2 = new EliPar();
        CSincSurface pv3 = new CSincSurface();

        public Form1()
        {
            InitializeComponent();
            AnT.InitializeContexts();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // инициализация Glut
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            // настройка параметров OpenGL для визуализации
           // Gl.glEnable(Gl.GL_CULL_FACE);
            Gl.glCullFace(Gl.GL_BACK);
            Gl.glFrontFace(Gl.GL_CCW);
            comboBox1.SelectedIndex = 0;
            ViewMode.SelectedIndex = 0;
            SurfacePick.SelectedIndex = 0;
            label1.Text = ("Rotation speed: ") + rotationSpeed;
            // Запуск таймера
            RenderTimer.Start();
        }

        private void RenderTimer_Tick(object sender, EventArgs e)
        {
            DrawSurface();
            AnT.Invalidate();
        }

        public struct Vertex
        {
            public float x, y, z;
            public float nx, ny, nz;
        }
        private void SetupMaterial()
        {
            int side = Gl.GL_FRONT;
            float[] m_diffuse = new float[] { 0.0f, 0.5f, 0.0f, 1 };
            float[] m_ambient = new float[] { 0.0f, 0.2f, 0.0f, 1 };
            float[] m_spevular = new float[] { 0.3f, 0.2f, 0.3f, 1 };
            float m_shininess = 1;

            Gl.glMaterialfv(side, Gl.GL_DIFFUSE, m_diffuse);
            Gl.glMaterialfv(side, Gl.GL_AMBIENT, m_ambient);
            Gl.glMaterialfv(side, Gl.GL_SPECULAR, m_spevular);
            Gl.glMaterialf(side, Gl.GL_SHININESS, m_shininess);
        }

        private void SetupLight()
        {
            int light = Gl.GL_LIGHT0;

            float[] lightDirection = new float[] { 2, 2, 2, 0 };

            float[] l_diffuse = new float[] { 0.5f, 0.5f, 0.5f, 1 };
            float[] l_ambient = new float[] { 0.5f, 0.5f, 0.5f, 1 };
            float[] l_specular = new float[] { 0.5f, 0.5f, 0.5f, 1 };

            Gl.glLightfv(light, Gl.GL_POSITION, lightDirection);
            Gl.glLightfv(light, Gl.GL_DIFFUSE, l_diffuse);
            Gl.glLightfv(light, Gl.GL_AMBIENT, l_ambient);
            Gl.glLightfv(light, Gl.GL_SPECULAR, l_ambient);

            Gl.glEnable(Gl.GL_LIGHTING);
            Gl.glEnable(light);
        }

        private void SetupCamera()
        {
            
            Gl.glLoadIdentity();

            rotationAngle = (rotationAngle + rotationSpeed) % 360;
            Glu.gluLookAt(
                           12, 12, 8,
                           0,  0,  0,
                           0,  0,  1);
            
            switch (ViewMode.SelectedIndex)
            {
                case 0:
                    Gl.glRotatef(rotationAngle, 0, 0, 1);
                    break;
                case 1:
                    Gl.glRotatef(rotationAngle, 0, 0, 1);
                    Gl.glRotatef(rotateX, 1, 0, 0);
                    Gl.glRotatef(rotateY, 0, 1, 0);
                    break;
                case 2:
                    Gl.glRotatef(rotateX, 1, 0, 0);
                    Gl.glRotatef(rotateY, 0, 1, 0);
                    break;
            }
            
        }
        private void DrawSurface()
        {
            int s_displayList = 0;
            int s_columns = 50;
            int s_rows = 50;

            float s_xMin = -10;
            float s_xMax = 10;
            float s_yMin = -10;
            float s_yMax = 10;

            float ScaleKof = scale;

            const float ZNEAR = 1f;
            const float ZFAR = 40;
            const float FIELD_OF_VIEW = 60;
            float aspect = (float)AnT.Width / (float)AnT.Height;
            

            Gl.glClearColor(0, 0, 0, 0);
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);

            SetupLight();
            SetupMaterial();
            SetupCamera();

            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_LINE);
                    Gl.glPolygonMode(Gl.GL_BACK, Gl.GL_LINE);
                    break;
                case 1:
                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_FILL);
                    Gl.glPolygonMode(Gl.GL_BACK, Gl.GL_FILL);
                    break;
                case 2:
                    Gl.glPolygonMode(Gl.GL_FRONT, Gl.GL_POINT);
                    Gl.glPolygonMode(Gl.GL_BACK, Gl.GL_POINT);
                    break;
            }

            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();

            Glu.gluPerspective(FIELD_OF_VIEW, aspect, ZNEAR, ZFAR);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);

            Gl.glPushMatrix();
            Gl.glScalef(ScaleKof, ScaleKof, ScaleKof);
            
                    


                    if (s_displayList == 0)
            {
                s_displayList = Gl.glGenLists(1);
                Gl.glNewList(s_displayList, Gl.GL_COMPILE_AND_EXECUTE);

                float dy = (s_yMax - s_yMin) / (s_rows - 1);
                float dx = (s_xMax - s_xMin) / (s_columns - 1);

                float y = s_yMin;

                for (int row = 0; row < s_rows - 1; ++row, y += dy)
                {
                    Gl.glBegin(Gl.GL_TRIANGLE_STRIP);
                    float x = s_xMin;
                    switch (SurfacePick.SelectedIndex)
                    {
                        case 0:
                            for (int column = 0; column <= s_columns; ++column, x += dx)
                            {
                                Vertex v0 = pv1.CalculateVertex(x, y + dy);
                                Vertex v1 = pv1.CalculateVertex(x, y);

                                Gl.glNormal3f(v0.nx, v0.ny, v0.nz);
                                Gl.glVertex3f(v0.x, v0.y, v0.z);

                                Gl.glNormal3f(v1.nx, v1.ny, v1.nz);
                                Gl.glVertex3f(v1.x, v1.y, v1.z);
                            }
                            break;
                        case 1:
                            for (int column = 0; column <= s_columns; ++column, x += dx)
                            {
                                Vertex v0 = pv2.CalculateVertex(x, y + dy);
                                Vertex v1 = pv2.CalculateVertex(x, y);

                                Gl.glNormal3f(v0.nx, v0.ny, v0.nz);
                                Gl.glVertex3f(v0.x, v0.y, v0.z);

                                Gl.glNormal3f(v1.nx, v1.ny, v1.nz);
                                Gl.glVertex3f(v1.x, v1.y, v1.z);
                            }
                            break;
                        case 2:
                            for (int column = 0; column <= s_columns; ++column, x += dx)
                            {
                                Vertex v0 = pv3.CalculateVertex(x, y + dy);
                                Vertex v1 = pv3.CalculateVertex(x, y);

                                Gl.glNormal3f(v0.nx, v0.ny, v0.nz);
                                Gl.glVertex3f(v0.x, v0.y, v0.z);

                                Gl.glNormal3f(v1.nx, v1.ny, v1.nz);
                                Gl.glVertex3f(v1.x, v1.y, v1.z);
                            }
                            break;

                    }
                   
                    Gl.glEnd();
                }
                Gl.glEndList();
            }
        }


        private void button1_Click(object sender, EventArgs e)
        {
             rotateX = 0;
             rotateY = 0;
             McoordX = 0;
             McoordY = 0;
             MlastX = 0;
             MlastY = 0;
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            rotationSpeed = (float)trackBar1.Value / 10;
            label1.Text = ("Rotation speed: ") + rotationSpeed;
        }
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            scale = (float)trackBar2.Value / 10;
        }

        private void A_TextChanged(object sender, EventArgs e)
        {
            try {
                int a = Convert.ToInt32(A.Text);
                switch (SurfacePick.SelectedIndex)
                {
                    case 0:
                        pv1.A = a;
                        break;
                    case 1:
                        pv2.A = a;
                        break;
                }
                if (a == 0)    
                    MessageBox.Show("Variable can't be 0!");
                
            }
            catch (FormatException a)
            {

            }
        }

        private void B_TextChanged(object sender, EventArgs e)
        {
            try
            {
                int b = Convert.ToInt32(B.Text);
                switch (SurfacePick.SelectedIndex)
                {
                    case 0:
                        pv1.B = b;
                        break;
                    case 1:
                        pv2.B = b;
                        break;
                }
                if (b == 0)
                    MessageBox.Show("Variable can't be 0!");
                
            }
            catch (FormatException b)
            {

            }
            
        }

        private void AnT_MouseMove(object sender, MouseEventArgs e)
        {
            // сохраняем координаты мыши
            float dx = e.X - McoordX;
            float dy = e.Y - McoordY;
            if (mouseButton)
            {
                rotateX = -(dy * 180) / AnT.Width + MlastX;
                rotateY = -(dx * 180) / AnT.Height + MlastY;
                DrawSurface();
            }
        }

        private void AnT_MouseDown(object sender, MouseEventArgs e)
        {
            mouseButton = true;
            McoordX = e.X;
            McoordY = e.Y;
        }

        private void AnT_MouseUp(object sender, MouseEventArgs e)
        {
            mouseButton = false;
            MlastX = rotateX;
            MlastY = rotateY;
        }
    }
}