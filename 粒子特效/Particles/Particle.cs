/*
 * 可随意使用此文档，但是请保留作者信息
 * 生产粒子(Eclipse)的方法为参考网上代码，作者不详，还望见谅。
 * 后续修改整理为粒子级特效-by颜家大饼-ybx
 * AdornorControl引用自StackOverflow大神代码。
 * 欢迎各位继续补充完善此功能，并欢迎在此处附上补充功能的补丁信息。
 * 虽然有些简陋，但是粒子效果已经很炫，这是一扇门。Over。
 * 2016-7-1 10:26:25 颜家大饼。
 * */
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Particles
{
    /// <summary>
    /// 粒子
    /// </summary>
    public class Particle
    {
        /// <summary>
        /// 位置
        /// </summary>
        public Point3D Position { get; set; }
        /// <summary>
        /// 速度
        /// </summary>
        public Point3D Velocity { get; set; }
        /// <summary>
        /// 大小
        /// </summary>
        public double Size { get; set; }
        /// <summary>
        /// 椭圆粒子
        /// </summary>
        public Ellipse Ellipse { get; set; }
        /// <summary>
        /// 模糊特效
        /// </summary>
        public BlurEffect Blur { get; set; }
        /// <summary>
        /// 颜色画刷
        /// </summary>
        public Brush Brush { get; set; }
    }
    /// <summary>
    /// 3D坐标
    /// </summary>
    public class Point3D
    {
        /// <summary>
        /// 3D坐标X
        /// </summary>
        public double X { get; set; }
        /// <summary>
        /// 3D坐标Y
        /// </summary>
        public double Y { get; set; }
        /// <summary>
        /// 3D坐标Z
        /// </summary>
        public double Z { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <param name="Z"></param>
        public Point3D(double X, double Y, double Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }
    }

    /// <summary>
    /// 扩展double型Next方法
    /// </summary>
    public static class ExRandom
    {
        private static Random rand = new Random();
        /// <summary>
        /// double类型随机数
        /// </summary>
        /// <param name="midvalue"></param>
        /// <param name="variance"></param>
        /// <returns></returns>
        public static double Next(this Random r, double minValue, double maxValue)
        {
            return minValue + (Math.Abs(maxValue - minValue) * rand.NextDouble());
        }
    }

    /// <summary>
    /// 方向
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// 左
        /// </summary>
        Left = 0,
        /// <summary>
        /// 上
        /// </summary>
        Up = 1,
        /// <summary>
        /// 右
        /// </summary>
        Right = 2,
        /// <summary>
        /// 下
        /// </summary>
        Down = 3,
        /// <summary>
        /// 各个方向放射
        /// </summary>
        Shine = 4
    }
    /// <summary>
    /// 扩展：粒子画布
    /// </summary>
    public class ParticleCanvas : Canvas
    {
        #region 字段
        /// <summary>
        /// x1方向
        /// </summary>
        private static double[] X1D = new double[] { -1, 0, 1, 0, -1 };
        /// <summary>
        /// x2方向
        /// </summary>
        private static double[] X2D = new double[] { 0, 0, 0, 0, 1 };
        /// <summary>
        /// y1方向
        /// </summary>
        private static double[] Y1D = new double[] { 0, -1, 0, 1, -1 };
        /// <summary>
        /// y2方向
        /// </summary>
        private static double[] Y2D = new double[] { 0, 0, 0, 1, 1 };
        /// <summary>
        /// 生存周期粒子
        /// </summary>
        private List<Particle> activeParticles = new List<Particle>();
        /// <summary>
        /// 死亡周期粒子
        /// </summary>
        private List<Particle> deadParticles = new List<Particle>();
        /// <summary>
        /// 计时器
        /// </summary>
        private DispatcherTimer timer = new DispatcherTimer();
        /// <summary>
        /// 加速度
        /// </summary>
        public const double ELAPSED = 0.1;
        /// <summary>
        /// 随机数实例
        /// </summary>
        public Random Rand = new Random();
        #endregion

        #region 属性
        /// <summary>
        /// 粒子数量
        /// </summary>
        public int ParticlesCount
        {
            get { return (int)GetValue(ParticlesCountProperty); }
            set { SetValue(ParticlesCountProperty, value); }
        }
        /// <summary>
        /// 粒子画刷
        /// </summary>
        public Brush ParticlesBrush
        {
            get { return (Brush)GetValue(ParticlesBrushProperty); }
            set { SetValue(ParticlesBrushProperty, value); }
        }
        /// <summary>
        /// 是否显示粒子
        /// </summary>
        public bool IsShowParticles
        {
            get { return (bool)GetValue(IsShowParticlesProperty); }
            set { SetValue(IsShowParticlesProperty, value); }
        }
        /// <summary>
        /// 粒子发射起点X1
        /// </summary>
        public double X1
        {
            get { return (double)GetValue(X1Property); }
            set { SetValue(X1Property, value); }
        }
        /// <summary>
        /// 粒子发射起点X2    
        /// </summary>
        public double X2
        {
            get { return (double)GetValue(X2Property); }
            set { SetValue(X2Property, value); }
        }
        /// <summary>
        /// 粒子发射点Y1
        /// </summary>
        public double Y1
        {
            get { return (double)GetValue(Y1Property); }
            set { SetValue(Y1Property, value); }
        }
        /// <summary>
        /// 粒子发射点Y2 
        /// </summary>
        public double Y2
        {
            get { return (double)GetValue(Y2Property); }
            set { SetValue(Y2Property, value); }
        }
        /// <summary>
        /// 最小速度
        /// </summary>
        public double MinSpeed
        {
            get { return (double)GetValue(MinSpeedProperty); }
            set { SetValue(MinSpeedProperty, value); }
        }
        /// <summary>
        /// 最大速度
        /// </summary>
        public double MaxSpeed
        {
            get { return (double)GetValue(MaxSpeedProperty); }
            set { SetValue(MaxSpeedProperty, value); }
        }
        /// <summary>
        /// 最小型号
        /// </summary>
        public double MinSize
        {
            get { return (double)GetValue(MinSizeProperty); }
            set { SetValue(MinSizeProperty, value); }
        }
        /// <summary>
        /// 最大型号
        /// </summary>
        public double MaxSize
        {
            get { return (double)GetValue(MaxSizeProperty); }
            set { SetValue(MaxSizeProperty, value); }
        }
        /// <summary>
        /// 最小模糊半径
        /// </summary>
        public double MinRadius
        {
            get { return (double)GetValue(MinRadiusProperty); }
            set { SetValue(MinRadiusProperty, value); }
        }
        /// <summary>
        /// 最大模糊半径
        /// </summary>
        public double MaxRadius
        {
            get { return (double)GetValue(MaxRadiusProperty); }
            set { SetValue(MaxRadiusProperty, value); }
        }
        /// <summary>
        /// 最小透明值
        /// </summary>
        public double MinOpacity
        {
            get { return (double)GetValue(MinOpacityProperty); }
            set { SetValue(MinOpacityProperty, value); }
        }
        /// <summary>
        /// 最大透明值
        /// </summary>
        public double MaxOpacity
        {
            get { return (double)GetValue(MaxOpacityProperty); }
            set { SetValue(MaxOpacityProperty, value); }
        }
        /// <summary>
        /// 放射方向
        /// </summary>
        public Direction Direction
        {
            get { return (Direction)GetValue(DirectionProperty); }
            set { SetValue(DirectionProperty, value); }
        }

        public static readonly DependencyProperty DirectionProperty =
            DependencyProperty.Register("Direction", typeof(Direction), typeof(ParticleCanvas), new PropertyMetadata(Direction.Shine));
        public static readonly DependencyProperty MaxOpacityProperty =
            DependencyProperty.Register("MaxOpacity", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(1.0));
        public static readonly DependencyProperty MinOpacityProperty =
            DependencyProperty.Register("MinOpacity", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(0.1));
        public static readonly DependencyProperty MaxRadiusProperty =
            DependencyProperty.Register("MaxRadius", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(1.0));
        public static readonly DependencyProperty MinRadiusProperty =
            DependencyProperty.Register("MinRadius", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(0.0));
        public static readonly DependencyProperty MaxSizeProperty =
            DependencyProperty.Register("MaxSize", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(2.0));
        public static readonly DependencyProperty MinSizeProperty =
            DependencyProperty.Register("MinSize", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(1.0));
        public static readonly DependencyProperty MaxSpeedProperty =
            DependencyProperty.Register("MaxSpeed", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(10.0));
        public static readonly DependencyProperty MinSpeedProperty =
            DependencyProperty.Register("MinSpeed", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(5.0));
        public static readonly DependencyProperty Y2Property =
            DependencyProperty.Register("Y2", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(0.0));
        public static readonly DependencyProperty Y1Property =
            DependencyProperty.Register("Y1", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(0.0));
        public static readonly DependencyProperty X1Property =
            DependencyProperty.Register("X1", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(0.0));
        public static readonly DependencyProperty X2Property =
           DependencyProperty.Register("X2", typeof(double), typeof(ParticleCanvas), new PropertyMetadata(0.0));
        public static readonly DependencyProperty ParticlesCountProperty =
           DependencyProperty.Register("ParticlesCount", typeof(int), typeof(ParticleCanvas), new PropertyMetadata(30));
        public static readonly DependencyProperty ParticlesBrushProperty =
           DependencyProperty.Register("ParticlesBrush", typeof(Brush), typeof(ParticleCanvas), new PropertyMetadata((Brush)Brushes.OrangeRed));
        public static readonly DependencyProperty IsShowParticlesProperty =
            DependencyProperty.Register("IsShowParticles", typeof(bool), typeof(ParticleCanvas), new PropertyMetadata(false, IsShowParticlesChanged));
        /// <summary>
        /// 粒子效果启用回调
        /// </summary>
        /// <param name="d"></param>
        /// <param name="e"></param>
        private static void IsShowParticlesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as ParticleCanvas).ShowParticles();
        }
        #endregion

        /// <summary>
        /// 创建一个激活、死亡周期粒子List，DispatcherTimer，为控件发射粒子
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ShowParticles()
        {
            //卸载更新粒子方法
            timer.Tick -= new EventHandler(StartParticlesed);

            if (IsShowParticles)
            {
                //配置计数器，开始发射粒子
                timer.Interval = TimeSpan.FromMilliseconds(10);
                //挂载更新粒子方法
                timer.Tick += new EventHandler(StartParticlesed);
                //启动计时器
                timer.Start();
            }
            else
            {
                //释放粒子图形
                foreach (var item in activeParticles)
                {
                    (this as Canvas).Children.Remove(item.Ellipse);
                }
                //释放活动粒子列表
                activeParticles.Clear();
                //停止计时器
                timer.Stop();
            }

        }
        /// <summary>
        /// 更新粒子状态
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StartParticlesed(object sender, EventArgs e)
        {
            UpdateParticules(this as Canvas, activeParticles, deadParticles);
        }

        /// <summary>
        /// 创建、更新、释放粒子信息
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="live"></param>
        /// <param name="dead"></param>
        private void UpdateParticules(Canvas canvas, List<Particle> live, List<Particle> dead)
        {
            //更新粒子信息
            dead.Clear();
            foreach (Particle p in live)
            {
                //超出Left、Top、Right、Bottom的与Size范围边界则该粒子进入死亡期
                if (p.Position.Y < -p.Size || p.Position.X < -p.Size || p.Position.X > canvas.Width || p.Position.Y > canvas.Height)
                {
                    dead.Add(p);
                }
                else
                {
                    //更新位置
                    p.Position.X += p.Velocity.X * ELAPSED;
                    p.Position.Y += p.Velocity.Y * ELAPSED;

                    TranslateTransform t = (p.Ellipse.RenderTransform as TranslateTransform);
                    t.X = p.Position.X;
                    t.Y = p.Position.Y;

                    //更新颜色信息
                    p.Ellipse.Fill = p.Brush;
                    p.Ellipse.Effect = p.Blur;
                }
            }

            //创建新的粒子
            for (int i = 0; i < 10 && live.Count < ParticlesCount; i++)
            {
                //尝试回收使用已有的粒子Eclipse图形
                if (dead.Count - 1 >= i)
                {
                    SpawnParticle(canvas, dead[i].Ellipse, live);
                    dead[i].Ellipse = null;
                }
                else
                {
                    SpawnParticle(canvas, null, live);
                }
            }

            //释放死亡期粒子
            foreach (Particle p in dead)
            {
                if (p.Ellipse != null)
                    (canvas as Canvas).Children.Remove(p.Ellipse);
                live.Remove(p);
            }
        }
        /// <summary>
        /// 创建新粒子
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="e"></param>
        /// <param name="particles"></param>
        private void SpawnParticle(Canvas canvas, Ellipse e, List<Particle> particles)
        {
            //粒子型号
            double size = Rand.Next(MinSize, MaxSize);
            double x = Rand.Next(X1, X2) - size / 2;
            double y = Rand.Next(Y1, Y2) - size / 2;
            double z = 0;
            double speed = Rand.Next(MinSpeed, MaxSpeed);

            Particle p = new Particle()
            {
                Position = new Point3D(x, y, z),
                Size = size,
            };

            //模糊
            var blur = new BlurEffect();
            blur.RenderingBias = RenderingBias.Performance;
            blur.Radius = Rand.Next(MinRadius, MaxRadius);
            p.Blur = blur;

            //画刷
            var brush = ParticlesBrush.Clone();
            brush.Opacity = Rand.Next(MinOpacity, MaxOpacity);
            p.Brush = brush;

            //平移变换
            TranslateTransform t;

            if (e != null)
            {
                e.Fill = null;
                e.Width = e.Height = size;
                p.Ellipse = e;

                t = e.RenderTransform as TranslateTransform;
            }
            else
            {
                p.Ellipse = new Ellipse() { Width = size, Height = size, IsEnabled = false, IsHitTestVisible = false };
                canvas.Children.Add(p.Ellipse);

                t = new TranslateTransform();
                p.Ellipse.RenderTransform = t;
                p.Ellipse.RenderTransformOrigin = new Point(size / 2, size / 2);
            }

            t.X = p.Position.X;
            t.Y = p.Position.Y;

            double vX = Rand.Next(X1D[(int)Direction], X2D[(int)Direction]) * speed;
            double vY = Rand.Next(Y1D[(int)Direction], Y2D[(int)Direction]) * speed;
            p.Velocity = new Point3D(vX, vY, 0);

            particles.Add(p);
        }
    }
}
