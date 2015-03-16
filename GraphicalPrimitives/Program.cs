using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SdlDotNet.Graphics;
using SdlDotNet.Core;
using System.Drawing;


namespace GraphicalPrimitives
{
    public class SDL
    {
        private static Surface m_VideoScreen;
        private static Surface m_Background;
        private static Surface m_Foreground;
        private static Point m_ForegroundPosition;

        public static void Main(string[] args)
        {
            m_VideoScreen = Video.SetVideoMode(300, 300, 32, false, false, false, true);

            LoadImages();

            Events.Quit += new EventHandler<QuitEventArgs>(ApplicationQuitEventHandler);
            Events.Tick += new EventHandler<TickEventArgs>(ApplicationTickEventHandler);
            Events.Run();
        }

        private static void LoadImages()
        {
            m_Background = (new Surface(300, 300)).Convert(m_VideoScreen, true, false);
            m_Foreground = (new Surface(300, 300)).Convert(m_VideoScreen, true, false);
            m_Background.Fill(new Rectangle(0, 0, 300, 300) , Color.White);
            m_Foreground.Transparent = true;
            NonSymCDALine(5, 5, 50, 5, Color.Red);
            BrezengamLine(5, 10, 50, 10, Color.Green);
            SimpleLine(15, 45, 150, 45, Color.Magenta);
            DrawWuLine(5, 20, 50, 20, Color.Aquamarine);

            NonSymCDALine(50, 5, 95, 25, Color.Red);
            BrezengamLine(50, 10, 95, 30, Color.Green);
            SimpleLine(150, 45, 285, 105, Color.Magenta);
            DrawWuLine(50, 20, 95, 40, Color.Aquamarine);
            //BrezengamCircle(40, 40, 25, Color.Khaki);
            BrezengamCircle(60, 60, 30, Color.LawnGreen);
        
           
            m_ForegroundPosition = new Point(m_VideoScreen.Width / 2 - m_Foreground.Width / 2,
                                              m_VideoScreen.Height / 2 - m_Foreground.Height / 2);
        }

        private static void ApplicationTickEventHandler(object sender, TickEventArgs args)
        {
            m_VideoScreen.Blit(m_Background);
            m_VideoScreen.Blit(m_Foreground, m_ForegroundPosition);
            m_VideoScreen.Update();
        }

        private static void ApplicationQuitEventHandler(object sender, QuitEventArgs args)
        {
            Events.QuitApplication();
        }


private static void SimpleLine(double _x1, double _y1, double _x2, double _y2, Color col)
{
    //making pseudo pixels
    Color[,] color;
    color = new Color[1, 1];
    color[0, 0] = col;
    
    double one = 3;
    double x1 = _x1;
    double y1 = _y1 ;
    double x2 = _x2 ;
    double y2 = _y2 ;
    double L;
    ////////////////////////////////////////////////////////////
    if (Math.Abs(_x2 - _x1) > Math.Abs(_y2 - _y1))
    {
        L = Math.Abs(_x2 - _x1);
    }
    else
    {
        L = Math.Abs(_y2 - _y1);
    }
    ////////////////////////////////////////
    double x = x1;
    double y = y1;
            
    m_Foreground.SetPixels(new Point((int)x,(int) y), color);
    for (int i = 0; i < L; i++)
    {
        if (x1 != x2)
        {
            double dx = (x2 - x1) / L;
            x = x + dx;
        }

        if (y1 != y2)
        {
            double dy = (y2 - y1) / L;
            y = y +  dy;
        }
        m_Foreground.SetPixels(new Point((int)x, (int)y), color);
    }
}

       
//        С помощью ЦДА решается дифференциальное уравнение отрезка, имеющее вид: 
// dY
//--------------------------------------------------------------------------------
//dX
// =  Py
//--------------------------------------------------------------------------------
//Px
// ,  
//где Py = Yk - Yn - приращение координат отрезка по оси Y, а Px = Xk - Xn - приращение координат отрезка по оси X. 
//При этом ЦДА формирует дискретную аппроксимацию непрерывного решения этого дифференциального уравнения. 
//В обычном ЦДА, используемом, как правило, в векторных устройствах, тем или иным образом определяется количество узлов N, 
//используемых для аппроксимации отрезка. Затем за N циклов вычисляются координаты очередных узлов: 
//X0 =   Xn;     Xi+1 = Xi + Px/N.  
//Y0 =   Yn;     Yi+1 = Yi + Py/N.  
//Получаемые значения Xi, Yi преобразуются в целочисленные значения координаты очередного подсвечиваемого пиксела либо округлением, либо отбрасыванием дробной части. 
//Генератор векторов, использующий этот алгоритм, имеет тот недостаток, что точки могут прописываться дважды, что увеличивает время построения. 
//Кроме того из-за независимого вычисления обеих координат нет предпочтительных направлений и построенные отрезки кажутся не очень красивыми. 
//Аппаратная реализация этого алгоритма изложена в пункте 8.1 первой части курса. 
//Субъективно лучше смотрятся вектора с единичным шагом по большей относительной координате (несимметричный ЦДА). Для Px > Py (при Px, Py > 0) это означает, что координата по X направлению должна увеличиться на 1 Px раз, а координата по Y-направлению должна также Px раз увеличиться, но на Py/Px. 

        /*-----------------------------------------------------  V_DDA
 * void V_DDA (int xn, int yn, int xk, int yk)
 *
 * Подпрограмма построения вектора из точки (xn,yn)
 * в точку (xk, yk) в первом квадранте методом
 * несимметричного цифрового дифференциального анализатора
 * с использованием только целочисленной арифметики.
 *
 * Обобщение на другие квадранты труда не составляет.
 *
 * Построение ведется от точки с меньшими  координатами
 * к точке с большими координатами с единичным шагом по
 * координате с большим приращением.
 *
 * Отдельно выделяется случай вектора с dx == dy
 *
 * Всего надо выдать пикселы в dx= xk - xn + 1 позиции
 * по оси X и в dy= yk - yn + 1 позиции по оси Y.
 *
 * Для определенности рассмотрим случай dx > dy
 *
 * При приращении X-координаты на 1 Y-координата должна
 * увеличиться на величину меньшую единицы и равную dy/dx.
 *
 * После того как Y-приращение станет больше или равно 1.0,
 * то Y-координату пиксела надо увеличить на 1, а из
 * накопленного приращения вычесть 1.0 и продолжить построения
 * Т.е. приращение Y на 1 выполняется при условии:
 * dy/dx + dy/dx + ... + dy/dx >= 1.0
 * Т.к. вычисления в целочисленной арифметике быстрее, то
 * умножим на dx обе части и получим эквивалентное условие:
 * dy + dy + ... + dy >= dx
 *
 * Эта схема и реализована в подпрограмме.
 *
 * При реализации на ассемблере можно избавиться от
 * большинства операторов внутри цикла while.
 * Для этого перед циклом надо домножить dy на величину,
 * равную 65536/dx.
 * Тогда надо увеличивать Y на 1 при признаке переноса
 * после вычисления s, т.е. операторы
 *       s= s + dy;
 *       if (s >= dx) { s= s - dx;  yn= yn + 1; }
 * заменяются командами ADD и ADC
 *
 */
private static void NonSymCDALine(int _x1, int _y1, int _x2, int _y2, Color col)
{
    //making pseudo pixels
    Color[,] color;
    color = new Color[3, 3];
    color[0, 0] = col;
    color[0, 1] = col;
    color[0, 2] = col;
    color[1, 0] = col;
    color[1, 1] = col;
    color[1, 2] = col;
    color[2, 0] = col;
    color[2, 1] = col;
    color[2, 2] = col;
    int one = 3;
    int x1 = _x1 * 3;
    int y1 = _y1 * 3;
    int x2 = _x2 * 3;
    int y2 = _y2 * 3;
    //////////////////////////////////////////////////////////////////////////
    int dx; 
    int dy;
    int s;
    if (x1 > x2)
    {
        s = x1; x1 = x2; x2 = s;
        s= y1;  y1= y2;  y2= s;
    }
    dx = x2 - x1; dy = y2 - y1;
    if (dx == 0 && dy == 0) return;

    dx = dx + one; dy = dy + one;
    m_Foreground.SetPixels(new Point(x1, y1), color);
    if (dy == dx) {                 /* Наклон == 45 градусов */
        while (x1 < x2)
        {
            x1 = x1 + one;
            m_Foreground.SetPixels(new Point(x1, x1), color);
        }
    }
    else if (dx > dy)
    {           /* Наклон <  45 градусов */
        s = 0;
        while (x1 < x2)
        {
            x1 = x1 + one;
            s = s + dy;
            if (s >= dx) { s = s - dx; y1 = y1 + one; }
            m_Foreground.SetPixels(new Point(x1, y1), color); ;
        }
    }
    else
    {                        /* Наклон >  45 градусов */
        s = 0;
        while (y1 < y2)
        {
            y1 = y1 + one;
            s = s + dx;
            if (s >= dy) { s = s - dy; x1 = x1 + one; }
            m_Foreground.SetPixels(new Point(x1, y1), color);
        }
    }
}





        

//Брезенхем в работе [] предложил алгоритм, не требующий деления, как и в алгоритме несимметричного ЦДА,
//но обеспечивающий минимизацию отклонения сгенерированного образа от истинного отрезка, 
//как в алгоритме обычного ЦДА. Основная идея алгоритма состоит в том, что если угловой коэффициент прямой < 1/2,
//то естественно точку, следующую за точкой (0,0), поставить в позицию (1,0) (рис. а), 
//а если угловой коэффициент > 1/2, то - в позицию (1,1) 
//Для принятия решения куда заносить очередной пиксел вводится величина отклонения Е точной позиции от середины между 
//двумя возможными растровыми точками в направлении наименьшей относительной координаты. Знак Е используется как критерий 
//для выбора ближайшей растровой точки. 
       
 /*
 * Подпрограмма иллюстрирующая построение вектора из точки
 * (xn,yn) в точку (xk, yk) методом Брезенхема.
 *
 * Построение ведется от точки с меньшими  координатами
 * к точке с большими координатами с единичным шагом по
 * координате с большим приращением.
 *
 * В общем случае исходный вектор проходит не через вершины
 * растровой сетки, а пересекает ее стороны.
 * Пусть приращение по X больше приращения по Y и оба они > 0.
 * Для очередного значения X нужно выбрать одну двух ближайших
 * координат сетки по Y.
 * Для этого проверяется как проходит  исходный  вектор - выше
 * или ниже середины расстояния между ближайшими значениями Y.
 * Если выше середины,  то Y-координату  надо  увеличить на 1,
 * иначе оставить прежней.
 * Для этой проверки анализируется знак переменной s,
 * соответствующей разности между истинным положением и
 * серединой расстояния между ближайшими Y-узлами сетки.
 */
private static void BrezengamLine(int _x1, int _y1, int _x2, int _y2, Color col)
{
    //making pseudo pixels
    Color[,] color;
    color = new Color[3, 3];
    color[0, 0] = col;
    color[0, 1] = col;
    color[0, 2] = col;
    color[1, 0] = col;
    color[1, 1] = col;
    color[1, 2] = col;
    color[2, 0] = col;
    color[2, 1] = col;
    color[2, 2] = col;
    int one = 3;
    int x1 = _x1 * 3;
    int y1 = _y1 * 3;
    int x2 = _x2 * 3;
    int y2 = _y2 * 3;
    ///////////////////////////////////////////////////////////////
    int dx, dy, s, sx, sy, kl, swap, incr1, incr2;
    sx = 0;
    /* Вычисление приращений и шагов */
    if ((dx= x2-x1) < 0) {dx= -dx; sx-=one;} else if (dx>0) sx+=one;
    sy= 0;
    if ((dy= y2-y1) < 0) {dy= -dy; sy-=one;} else if (dy>0) sy+=one;

    /* Учет наклона */
    swap= 0;
    if ((kl= dx) < (s= dy)) {
        dx= s;  dy= kl;  kl= s; ++swap;
    }
    s= (incr1= 2*dy)-dx; /* incr1 - констан. перевычисления */
                        /* разности если текущее s < 0  и  */
                        /* s - начальное значение разности */
    incr2= 2*dx;         /* Константа для перевычисления    */
                        /* разности если текущее s >= 0    */
        /* Первый  пиксел вектора       */
    m_Foreground.SetPixels(new Point(x1, y1), color);
    while ((kl-=one) >= 0) {
        if (s >= 0) {
            if (swap > 0) x1+= sx; else y1+= sy;
            s-= incr2;
        }
        if (swap>0) y1+= sy; else x1+= sx;
        s+=  incr1;
        m_Foreground.SetPixels(new Point(x1, y1), color); /* Текущая  точка  вектора   */
    }
}


        

       

private static void BrezengamCircle(int _xc, int _yc, int _r, Color col)
{
    Color[,] color;
    color = new Color[3, 3];
    color[0, 0] = col;
    color[0, 1] = col;
    color[0, 2] = col;
    color[1, 0] = col;
    color[1, 1] = col;
    color[1, 2] = col;
    color[2, 0] = col;
    color[2, 1] = col;
    color[2, 2] = col;
    int xc = _xc * 3;
    int yc = _yc * 3;
    int r = _r * 3;
    int one = 3;
    int x, y, z, Dd;
    x = 0;
    y = r;
    int d = 9-2*y;
    while (x <= y)
    {
        Pixel_circle(xc, yc, x, y, col);
        if (d < 0)
        {
            d = d + 4 * x + 6 * one;
        }
        else
        {
            d = d + 4 * (x - y) + 10;
            y = y - one;
        }
        x = x + one;
    }
}

static void Pixel_circle (int _xc, int _yc, int _x, int  _y,  Color col)
{
    Color[,] color;
    color = new Color[3, 3];
    color[0, 0] = col;
    color[0, 1] = col;
    color[0, 2] = col;
    color[1, 0] = col;
    color[1, 1] = col;
    color[1, 2] = col;
    color[2, 0] = col;
    color[2, 1] = col;
    color[2, 2] = col;
    int xc = _xc;
    int yc = _yc;
    int x = _x;
    int y = _y;
            
    m_Foreground.SetPixels(new Point(xc + x, yc + y), color);
    m_Foreground.SetPixels(new Point(xc + y, yc + x), color);
    m_Foreground.SetPixels(new Point(xc + y, yc - x), color);
    m_Foreground.SetPixels(new Point(xc + x, yc - y), color);
    m_Foreground.SetPixels(new Point(xc - x, yc - y), color);
    m_Foreground.SetPixels(new Point(xc - y, yc - x), color);
    m_Foreground.SetPixels(new Point(xc - y, yc + x), color);
    m_Foreground.SetPixels(new Point(xc - x, yc + y), color);
    }  /* Pixel_circle */


   
//Line by Ву
public static void DrawWuLine(int _x0, int _y0, int _x1, int _y1, Color clr)
{
    int x0 = _x0 * 3;
    int x1 = _x1 * 3;
    int y0 = _y0 * 3;
    int y1 = _y1 * 3;
    //Вычисление изменения координат
    int dx = (x1 > x0) ? (x1 - x0) : (x0 - x1);
    int dy = (y1 > y0) ? (y1 - y0) : (y0 - y1);
    //Если линия параллельна одной из осей, рисуем обычную линию - заполняем все пикселы в ряд
    if (dx == 0 || dy == 0)
    {
        BrezengamLine(_x0, _y0, _x1, _y1, clr);
        return;
    }

    //Для Х-линии (коэффициент наклона < 1)
    if (dy < dx)
    {
        //Первая точка должна иметь меньшую координату Х
        if (x1 < x0)
        {
            x1 += x0; x0 = x1 - x0; x1 -= x0;
            y1 += y0; y0 = y1 - y0; y1 -= y0;
        }
        //Относительное изменение координаты Y
        float grad = (float)dy / dx;
        //Промежуточная переменная для Y
        float intery = y0 + grad;
        //Первая точка
        PutPixel(clr, x0, y0, 255);

        for (int x = x0 + 1; x < x1; x++)
        {
            //Верхняя точка
            PutPixel(clr, x, IPart(intery), (int)(255 - FPart(intery) * 255));
            //Нижняя точка
            PutPixel(clr, x, IPart(intery) + 1, (int)(FPart(intery) * 255));
            //Изменение координаты Y
            intery += grad;
        }
        //Последняя точка
        PutPixel(clr, x1, y1, 255);
    }
    //Для Y-линии (коэффициент наклона > 1)
    else
    {
        //Первая точка должна иметь меньшую координату Y
        if (y1 < y0)
        {
            x1 += x0; x0 = x1 - x0; x1 -= x0;
            y1 += y0; y0 = y1 - y0; y1 -= y0;
        }
        //Относительное изменение координаты X
        float grad = (float)dx / dy;
        //Промежуточная переменная для X
        float interx = x0 + grad;
        //Первая точка
        PutPixel(clr, x0, y0, 255);

        for (int y = y0 + 1; y < y1; y++)
        {
            //Верхняя точка
            PutPixel(clr, IPart(interx), y, 255 - (int)(FPart(interx) * 255));
            //Нижняя точка
            PutPixel(clr, IPart(interx) + 1, y, (int)(FPart(interx) * 255));
            //Изменение координаты X
            interx += grad;
        }
        //Последняя точка
        PutPixel( clr, x1, y1, 255);
    }
}

private static void PutPixel(Color col, int  x, int  y, int alpha)
{
    Color[,] color;
    color = new Color[3, 3];
    color[0, 0] = Color.FromArgb(alpha, col);
    color[0, 1] = Color.FromArgb(alpha, col);
    color[0, 2] = Color.FromArgb(alpha, col);
    color[1, 0] = Color.FromArgb(alpha, col);
    color[1, 1] = Color.FromArgb(alpha, col);
    color[1, 2] = Color.FromArgb(alpha, col);
    color[2, 0] = Color.FromArgb(alpha, col);
    color[2, 1] = Color.FromArgb(alpha, col);
    color[2, 2] = Color.FromArgb(alpha, col);
    m_Foreground.SetPixels(new Point(x, y), color);
}

//Целая часть числа
private static int IPart(float x)
{
    return (int)x;
}
//дробная часть числа
private static float FPart(float x)
{
    while (x >= 0)
        x--;
    x++;
    return x;
}


    }


}
