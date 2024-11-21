using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppleTree
{
    public partial class Form1 : Form
    {
        Timer hitTimer = new Timer();
        Timer fallTimer = new Timer();

        int requiredHits = 10;
        int priceHit = 30;
        int storageCapacity = 20;
        int priceStorage = 10;

        int direction = 0;//1: left -1: right
        int facing = 1;
        double hitDirection = 0;

        int hitCount = 0;
        int hitsWaiting = 0;
        bool treeHit = false;
        bool holding = false;
        bool inTheTree = true;

        int apples = 0;

        public Form1()
        {
            InitializeComponent();
            Start();
        }
        void Start()
        {
            apple.Hide();
            this.KeyPreview = true;
            AddEvents();
            DefineTimers();
        }
        void AddEvents()
        {
            this.KeyDown += DownEvent;
            this.KeyUp += UpEvent;
            this.MouseClick += ClickEvent;
            StorageUpgrade.Click += UpgradeStorage;
            HitUpgrade.Click += UpgradeHits;
            restartButton.Click += Restart;
        }
        void Restart(object s, EventArgs e) {
            requiredHits = 10;
            priceHit = 30;
            storageCapacity = 20;
            priceStorage = 10;
            apples = 0;
            UpdateAppleLable();
            UpdateHitLable();
            UpdateStorageLabel();
            fallTimer.Stop();
            apple.Hide();
            apple.Top = lomb.Bottom - apple.Height * 2;
            inTheTree = true;
        }
        void UpgradeHits(object s, EventArgs e) {
            if (apples >= priceHit)
            {
                if (requiredHits > 3)
                {
                    requiredHits -= 1;
                    apples -= priceHit;
                    UpdateAppleLable();
                    priceHit += 30;
                    UpdateHitLable();
                    if (requiredHits == 3)
                    {
                        HitUpgrade.Enabled = false;
                    }
                }
                
            }
        }
        void UpdateHitLable() {
            HitUpgrade.Text = $"Ár: {priceHit} alma";
            HitLabel.Text = $"szükséges ütések száma: {requiredHits}";
        }
        void UpgradeStorage(object s, EventArgs e) {
            if (apples >= priceStorage)
            {
                apples -= priceStorage;
                UpdateAppleLable();
                priceStorage += 2;
                storageCapacity += 5;
                UpdateStorageLabel();
            }
        }
        void UpdateStorageLabel() {
            CapacityLabel.Text = $"A kosár teherbírása: {storageCapacity} alma";
            StorageUpgrade.Text = $"Ár: {priceStorage} alma";
        }
        void DefineTimers()
        {
            Timer time = new Timer();
            time.Interval = 23;
            time.Tick += MoveEvent;
            time.Start();
            hitTimer.Interval = 10;
            hitTimer.Tick += HitEvent;
            fallTimer.Interval = 20;
            fallTimer.Tick += FallEvent;
        }

        void ClickEvent(object s, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Hit();
            }
            if (e.Button == MouseButtons.Right)
            {
                if (apple.Top > kez.Top && apple.Visible)
                {
                    Drop();
                }
            }
        }
        void Drop()
        {
            fallTimer.Start();
        }
        void FallEvent(object s, EventArgs e)
        {
            if (inTheTree)
            {
                apple.Top += 4;
                if (apple.Top > kez.Top)
                {
                    fallTimer.Stop();
                    inTheTree = false;
                    holding = true;
                }
            }
            else
            {
                holding = false;
                apple.Top += 4;
                if (apple.Bounds.IntersectsWith(kosar.Bounds) && apple.Top > kosar.Top)
                {
                    fallTimer.Stop();
                    apple.Hide();
                    apple.Top = lomb.Bottom - apple.Height * 2;
                    inTheTree = true;
                    AddApple();
                    UpdateAppleLable();
                }
                else if (apple.Top > ground.Top)
                {
                    fallTimer.Stop();
                    apple.Hide();
                    apple.Top = lomb.Bottom - apple.Height * 2;
                    inTheTree = true;
                    UpdateAppleLable();
                }
            }

        }
        void Hit()
        {
            if (hitDirection != 0)
            {
                hitsWaiting++;
            }
            else if (hitDirection == 0 && !holding)
            {
                hitTimer.Start();
            }
            
        }
        void updateHitLabel() {
            utesek.Text = $"ütések száma: {hitCount}";
        }
        void HitEvent(object s, EventArgs e)
        {
            if (hitDirection >= 0)
            {
                kez.Left -= 3 * facing;
                hitDirection += 0.2;
                if (kez.Bounds.IntersectsWith(torzs.Bounds))
                    treeHit = true;
                if (hitDirection > 1)
                    hitDirection = -1;
            }
            else
            {
                kez.Left += 3 * facing;
                hitDirection -= 0.2;
                if (hitDirection < -2)
                {
                    if (hitsWaiting == 0)
                        hitTimer.Stop();
                    else
                        hitsWaiting--;

                    hitDirection = 0;
                    if (treeHit)
                    {
                        hitCount++;
                        updateHitLabel();
                        treeHit = false;
                        if (hitCount == requiredHits)
                        {
                            apple.Show();
                            if (facing == 1)
                                apple.Left = kez.Left;
                            else
                                apple.Left = kez.Right - apple.Width;
                            hitCount = 0;
                            updateHitLabel();
                            fallTimer.Start();
                        }
                    }
                }
            }
        }
        void AddApple() {
            if (apples < storageCapacity)
            {
                apples++;
            }
        }
        void UpdateAppleLable() {
            applesLable.Text = $"Gyűjtött almák száma: {apples}";
        }
        void MoveEvent(object s, EventArgs e)
        {
            int moveLength = 5;
            if (hitDirection == 0)
            {
                if (direction == 1)
                {
                    kez.Left = test.Left + test.Width / 2 - kez.Width;
                    if ((kez.Left - moveLength) > 0)
                    {
                        kez.Left -= moveLength;
                        test.Left -= moveLength;
                        fej.Left -= moveLength;
                        if (apple.Visible)
                            apple.Left = kez.Left;
                    }
                }
                if (direction == -1)
                {
                    kez.Left = test.Left + test.Width / 2;
                    if ((kez.Right + moveLength) < this.ClientSize.Width)
                    {
                        kez.Left += moveLength;
                        test.Left += moveLength;
                        fej.Left += moveLength;
                        if (apple.Visible)
                            apple.Left = kez.Right - apple.Width;
                    }
                }
            }
        }
        void DownEvent(object s, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1)
            {
                new Form2().Show();
            }
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
            {
                direction = 1;
                facing = 1;
            }

            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
            {
                direction = -1;
                facing = -1;
            }
        }
        void UpEvent(object s, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.Left)
                direction = 0;
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.Right)
                direction = 0;
        }
    }
}
