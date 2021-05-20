using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Windows.Threading;

namespace Balloon_Pop
{
    public partial class MainWindow : Window
    
    // Detta bestämmer hur snabbt och hur ofta ballongerna ska komma in i spelet //
    { 
        // Variabler för spelet (tid, snabbhet och intervall)// 
        DispatcherTimer gameTimer = new DispatcherTimer();

        int speed = 3;
        int intervals = 90;
        Random rand = new Random();

        // Ballongerna som har gått förbi toppen av canvasen läggs till i itemremover och det tar bort dom från canvasen //
        List<Rectangle> itemRemover = new List<Rectangle>();

        // Importerar bakrundsbild //
        ImageBrush backgroundImage = new ImageBrush();

        // Ändrar ballongfärger och animerar ballongerna så de går höger och vänster //
        int balloonSkins;
        int i;

        // Gör så att man vet när spelet är aktivt och när det är över, när man missar ett antal balonger då är spelet över //
        int missedBalloons;

        bool gameIsActive;

        int score;

        MediaPlayer player = new MediaPlayer();



        public MainWindow()
            // Ett event som ska hålla koll på alla ballonger //
        {
            InitializeComponent();

            gameTimer.Tick += GameEngine;
            gameTimer.Interval = TimeSpan.FromMilliseconds(20);

            // Lokaliserar var bakgrundbilden ligger i mappen files //
            backgroundImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/background-Image.jpg"));
            MyCanvas.Background = backgroundImage;

            RestartGame();
        }

        private void GameEngine(object sender, EventArgs e)
        {
            // GameEngine ska hålla koll på alla balonger //
            // Visar uppdaterad poängtavla. //
            // Om ballonoskin numret är 1,2,3,5 då kan man knyta en image(bild) till Imagebrush och sedan till rectangle //
            scoreText.Content = "Score: " + score;

            intervals -= 10;

            if (intervals < 1)
            {
                ImageBrush balloonImage = new ImageBrush();

                balloonSkins += 1;

            // Behövs inte högre nummer då det bara finns 5 olika ballonger som existerar. //
                if (balloonSkins > 5)
                {
                    balloonSkins = 1;
                }

                // Detta lokaliserar ballongerna som ligger i files //
                // switch sats påminner om if sats men används för att hantera olika cases(utfall) som här till exempel //
                switch (balloonSkins)
                {
                    case 1:
                        balloonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/balloon1.png"));
                        break;
                    case 2:
                        balloonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/balloon2.png"));
                        break;
                    case 3:
                        balloonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/balloon3.png"));
                        break;
                    case 4:
                        balloonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/balloon4.png"));
                        break;
                    case 5:
                        balloonImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/files/balloon5.png"));
                        break;
                }
                // Här definieras properties(egenskaper) för rectangle bland annat höjd och bredd. //
                // Den kommer direkt att länka till balloonimage så det nummer som är knyten till det kommer ladda den bilden och knytas till den nya ballongen //
                Rectangle newBalloon = new Rectangle
                {
                    Tag = "balloon",
                    Height = 70,
                    Width = 50,
                    Fill = balloonImage
                };

                // x och y postionen av ballongen. Left är ett random nummer medans resten har ett specifikt nummer. //
                Canvas.SetLeft(newBalloon, rand.Next(50, 400));
                Canvas.SetTop(newBalloon, 600);

                MyCanvas.Children.Add(newBalloon);

                // Varje gång vi skapar en ballong (under 1) följer den alla instruktioner och skapar en ny ballong får det ett nytt nummer //
                intervals = rand.Next(90, 150);
            }

            // Här letas det efter rectangle med (tag = balloon) alla rectangles med denna taggen kan man flytta upp och åt sidan. //
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                if ((string)x.Tag == "balloon")
                {
                    i = rand.Next(-5, 5);

                    Canvas.SetTop(x, Canvas.GetTop(x) - speed);
                    Canvas.SetLeft(x, Canvas.GetLeft(x) - (i * -1));
                }
                    
                if (Canvas.GetTop(x) < 20)
                {
                    itemRemover.Add(x);

                    missedBalloons += 1;
                }

            }
            // ballongerna som når toppen och ligger i itemremover försvinner //
            foreach (Rectangle y in itemRemover)
            {
                MyCanvas.Children.Remove(y);
            }

            // Missar man 10 balonger förlorar man och spelet är inte aktivt då får man ett meddelande för att kunna starta om spelet igen //
            if (missedBalloons > 10)
            {
                gameIsActive = false;
                gameTimer.Stop();
                MessageBox.Show("Game over!!! You missed 10 balloons" + Environment.NewLine + "Click to play again");

                RestartGame();
            }

            // om man får mer än 10 poäng ska det gå lite snabbare //
            if (score > 10)
            {
                speed = 7;
            }

        }

        private void popBalloons(object sender, MouseButtonEventArgs e)
        {
            //Man ska kunna klicka ballongerna så de spricks när spelet är aktivt och höra ljudet när de spricker //
            // När detta händer får man ett poäng //
            if (gameIsActive)
            {
                if (e.OriginalSource is Rectangle)
                {
                    Rectangle activeRec = (Rectangle)e.OriginalSource;

                    player.Open(new Uri("../../files/pop_sound.mp3", UriKind.RelativeOrAbsolute));
                    player.Play();

                    MyCanvas.Children.Remove(activeRec);

                    score += 1;
                }
            }

        }

        private void StartGame()
        {
            // När spelet startar ska det vara inga missade ballonger, 0 poäng, specifikt tempo och då är spelet aktivt // 
            gameTimer.Start();

            missedBalloons = 0;
            score = 0;
            intervals = 90;
            gameIsActive = true;
            speed = 3;

        }

        private void RestartGame()
        { 
            // om det existerar rectangles ska de läggas till i itemremover, det är två foreach loopar som används till detta//
            foreach (var x in MyCanvas.Children.OfType<Rectangle>())
            {
                itemRemover.Add(x);
            }

            // Alla rectangles som är i itemremover kommer tas bort från canvasen, då resetar det till 0 och man kan köra igen //
            foreach (Rectangle y in itemRemover)
            {
                MyCanvas.Children.Remove(y);
            }

            itemRemover.Clear();

            StartGame();

        }
    }
}
