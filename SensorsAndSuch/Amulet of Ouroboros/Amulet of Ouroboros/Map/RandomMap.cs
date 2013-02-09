using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SensorsAndSuch.Sprites;
using Microsoft.Xna.Framework.Graphics;
using System;
using Microsoft.Xna.Framework.Content;
using SensorsAndSuch.Maps;
using SensorsAndSuch.Mobs;

namespace SensorsAndSuch.Maps
{

    public class RandomMap
    {
        private static int _mapWidth = 50;
        private static int _mapHeight = 50;

        private static int startX = 20;
        private static int startY = 20;

        List<BaseTile>[,] grid;

        #region Properties

        public int MapWidth
        {
            get { return _mapWidth; }
        }

        public int MapHeight
        {
            get { return _mapHeight; }
        }

        #endregion

        #region Methods

        public bool isFree(int i, int j)
        {
            if (i < 0 || i >= MapWidth) return false;
            if (j < 0 || j >= MapHeight) return false;
            return grid[i, j].Count == 1;
        }

        public bool isFree(Vector2 gridPos)
        {
            return isFree((int) gridPos.X, (int) gridPos.Y);
        }

        public RandomMap()
        {
            // grid = new BaseTile[MapWidth, MapHeight];
            grid = new List<BaseTile>[MapWidth, MapHeight];
            for (int x = 0; x < MapWidth; x++)
                for (int y = 0; y < MapHeight; y++)
                    grid[x, y] = new List<BaseTile>();

            //AddBorder(100);
            for (int i = 0; i < 40; i++)
            {
                AddRoom(i * MapWidth/ 10 + 1, 5);
                AddRandPoint();
            }
            Vector2 freePos;
            while (GetNumbAtHeight(1) > 20 * 25)
            {
                while (GetNumbAtHeight(1) > 20 * 25)
                    RemoveRandPoint();
                SetTilesToNotHit();
                freePos = GetRandomFreePos();
                HitAttatchedTiles((int)freePos.X, (int)freePos.Y);
                AddTheNotHit();

            }
            SetTilesToNotHit();
                freePos = GetRandomFreePos();
                HitAttatchedTiles((int)freePos.X, (int)freePos.Y);
                AddTheNotHit();

            AddBorder(100);
            for (int x = 0; x < MapWidth; x++)
                for (int y = 0; y < MapHeight; y++)
                    if (grid[x, y].Count == 0) 
                    {
                        grid[x, y].Add(new Dirt(new Vector2(x, y)));
                        grid[x, y].Add(new Wall(new Vector2(x, y)));
                    }
        }

        #region Change Map
        public void RemoveRandPoint()
        {
            int Xt = Globals.rand.Next(MapWidth), Yt = Globals.rand.Next(MapHeight);
            if (grid[Xt, Yt].Count == 0) { RemoveRandPoint(); return; }
            grid[Xt, Yt].Clear();
        }

        public void AddRandPoint()
        {
            int Xt = Globals.rand.Next(MapWidth), Yt = Globals.rand.Next(MapHeight);
            if (grid[Xt, Yt].Count != 0) { AddRandPoint(); return; }
            grid[Xt,Yt].Add( new Dirt( new Vector2( Xt, Yt)));

        }

        public void AddRoom(int xbound, int SizeRange)
        {
            int size = Globals.rand.Next(SizeRange) + 2;
            int Xt = Globals.rand.Next(5) + xbound, Yt = Globals.rand.Next(MapHeight);
            for (int i = 0; i < size && i + Xt < MapWidth; i++)
            {
                for (int j = 0; j < size && j + Yt < MapHeight; j++)
                {
                    grid[i + Xt, j + Yt].Add( new Dirt(new Vector2(i + Xt, j + Yt)));
                }
            }
        }

        public void AddBorder(int prob)
        {

            for (int j = 0; j < MapHeight; j++)
            {
                if (Globals.rand.Next(100) <= prob)
                    grid[0, j].Clear();
                if (Globals.rand.Next(100) <= prob)
                    grid[MapWidth - 1, j].Clear();


            }
            for (int i = 0; i < MapWidth; i++)
            {
                if (Globals.rand.Next(100) <= prob)
                    grid[i, 0].Clear();
                if (Globals.rand.Next(100) <= prob)
                    grid[i, MapHeight - 1].Clear();
            }


            /*
            for (int j = 0; j < MapHeight; j++)
            {
                if (Globals.rand.Next(100) <= prob)
                    grid[0, j].Add( new Dirt(new Vector2(0, j)));
                if (Globals.rand.Next(100) <= prob)
                    grid[MapWidth - 1, j].Add( new Dirt(new Vector2(MapWidth - 1, j)));


            }
            for (int i = 0; i < MapWidth; i++)
            {
                if (Globals.rand.Next(100) <= prob)
                    grid[i, 0].Add( new Dirt(new Vector2(i, 0)));
                if (Globals.rand.Next(100) <= prob)
                    grid[i, MapHeight - 1].Add( new Dirt(new Vector2(i, MapHeight - 1)));
            }*/
        }
        #endregion

        #region cover All Tiles
        public void SetTilesToNotHit()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (grid[i, j].Count != 0) grid[i, j][0].hit = false;
                }
            }
        }

        public void HitAttatchedTiles(int i, int j)
        {
            if (isInBounds(i, j) && grid[i, j].Count != 0)
            { 
                if (grid[i, j][0].hit) return;
                grid[i, j][0].hit = true;
                HitAttatchedTiles(i - 1, j);
                HitAttatchedTiles(i + 1, j);
                HitAttatchedTiles(i, j - 1);
                HitAttatchedTiles(i, j + 1);
            }
        }

        public void AddTheNotHit()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (grid[i, j].Count != 0 && !grid[i, j][0].hit)
                    {
                        Vector2 newDir = BaseTile.GetRandDir();
                        int Tx = i, Ty = j;
                        while (!isInBounds(Tx, Ty) || !grid[Tx, Ty][0].hit)
                        {
                            if (Globals.rand.Next(10) >= 8) newDir = BaseTile.GetRandDir();
                            if (isInBounds(Tx + (int)newDir.X, Ty + (int)newDir.Y))
                            {
                                Tx += (int)newDir.X;
                                Ty += (int)newDir.Y;
                            }
                            else 
                            { 
                                newDir = BaseTile.GetRandDir();
                            }
                            if (isInBounds(Tx, Ty) && grid[Tx, Ty].Count == 0)
                            {
                                grid[Tx, Ty].Add( new Dirt(new Vector2(Tx, Ty)));
                            }
                        }
                        HitAttatchedTiles(i, j);
                    }
                }
            }
        }
        
        private bool AndAdjHit(int i, int j) {
            BaseTile tile;
            tile = GetTileBase(i + 1, j);
            if (tile != null && tile.hit) return true;

            tile = GetTileBase(i - 1, j);
            if (tile != null && tile.hit) return true;

            tile = GetTileBase(i, j + 1);
            if (tile != null && tile.hit) return true;

            tile = GetTileBase(i, j - 1);
            if (tile != null && tile.hit) return true;

            return false;
        }


        public Vector2? FindAtHeightFree(int i, int j, int height, int radius)
        {
            //if radius <= 0 then stop
            if (radius <= 0 || !isInBounds(i, j) || !grid[i, j][0].hit) return null;
            if (grid[i, j].Count != height) return null;
            if (Globals.Mobs.GetMobAt(i, j) == null) return new Vector2(i, j);
            grid[i, j][0].hit = true;
            
            Vector2? temp;
            temp = FindAtHeightFree(i - 1, j, height, radius -1);
            if (temp != null) return temp;

            temp = FindAtHeightFree(i + 1, j, height, radius -1);
            if (temp != null) return temp;

            temp = FindAtHeightFree(i, j - 1, height, radius -1);
            if (temp != null) return temp;

            return FindAtHeightFree(i, j + 1, height, radius - 1);

        }
        #endregion

        private bool isInBounds(int i, int j) {
            if (i < 0 || i >= MapWidth) return false;
            if (j < 0 || j >= MapHeight) return false;
            return true;
        }

        #region Get Tiles or columns

        private BaseTile GetTileBase(int i, int j)
        {
            if (i < 0 || i >= MapWidth) return null;
            if (j < 0 && j >= MapHeight) return null;
            return grid[i, j][0];
        }

        internal List<BaseTile> GetTileColumn(Vector2 vec)
        {
            return GetTileColumn((int) vec.X, (int) vec.Y);
        }

        internal List<BaseTile> GetTileColumn(int i, int j)
        {
            if (isInBounds(i, j))
                return grid[i, j];
            return null;
        }        
        
        public int GetNumbAtHeight(int height)
        {
            int amount = 0;
            for (int X = 0; X < MapWidth; X++)
            {
                for (int Y = 0; Y < MapWidth; Y++)
                {
                    if (GetTileColumn(X, Y) != null && GetTileColumn(X, Y).Count == height)
                        amount++;
                }
            }
            return amount;
        }

        public Vector2 GetRandomFreePos()
        {
            int i = Globals.rand.Next(MapWidth);
            int j = Globals.rand.Next(MapHeight);
            if (grid[i, j].Count != 1) return GetRandomFreePos();
            return new Vector2(i, j);
        }

        public List<List<BaseTile>> GetAdjColums(int i, int j) 
        {
            List<List<BaseTile>> adj = new List<List<BaseTile>>();
            if (GetTileColumn(i - 1, j) != null)
                adj.Add(GetTileColumn(i - 1, j));
            if (GetTileColumn(i + 1, j) != null)
                adj.Add(GetTileColumn(i + 1, j));
            if (GetTileColumn(i, j - 1) != null)
                adj.Add(GetTileColumn(i, j - 1));
            if (GetTileColumn(i, j + 1) != null)
                adj.Add(GetTileColumn(i, j + 1));
            return adj;
        }

        public List<Vector2> GetAdjGridPos(int i, int j)
        {
            List<Vector2> ret = new List<Vector2>();
            if (isInBounds(i - 1, j))
                ret.Add(new Vector2(i - 1, j));
            if (isInBounds(i + 1, j))
                ret.Add(new Vector2(i + 1, j));
            if (isInBounds(i, j - 1))
                ret.Add(new Vector2(i, j - 1));
            if (isInBounds(i, j + 1))
                ret.Add(new Vector2(i, j + 1));
            return ret;
        }

        private void DijkstrasAlgHelper(int X, int Y, int currDist, int MaxDist, Func<List<BaseTile>, bool> traversable)
        {
            if (currDist >= MaxDist) return;
            BaseTile CurrTile = grid[X, Y][0];
            List<List<BaseTile>> columns = GetAdjColums(X, Y);
            for (int i = 0; i < columns.Count; i++)
            {
                if (traversable(columns[i]))
                {
                    if (currDist < columns[i][0].dist)
                    {
                        columns[i][0].dist = currDist;
                        columns[i][0].dirToShortest = CurrTile.GridPos;
                        DijkstrasAlgHelper((int)columns[i][0].GridPos.X, (int)columns[i][0].GridPos.Y, currDist + 1, MaxDist, traversable);
                    }
                }
            }
        }
        private void ScreamHelper(BaseMonster mon, int X, int Y, int currDist, int MaxDist, Func<List<BaseTile>, bool> traversable)
        {
            if (currDist >= MaxDist) return;
            BaseTile CurrTile = grid[X, Y][0];
            List<List<BaseTile>> columns = GetAdjColums(X, Y);
            for (int i = 0; i < columns.Count; i++)
            {
                if (traversable(columns[i]))
                {
                    if (currDist < columns[i][0].dist)
                    {
                        columns[i][0].dist = currDist;
                        columns[i][0].dirToShortest = CurrTile.GridPos;
                        DijkstrasAlgHelper((int)columns[i][0].GridPos.X, (int)columns[i][0].GridPos.Y, currDist + 1, MaxDist, traversable);
                    }
                }
            }
        }

        //DijkstrasAlg
        public bool Scream(int X, int Y, int MaxDist, BaseMonster mon)
        {
            BaseTile CurrTile = grid[X, Y][0];
            Func<List<BaseTile>, bool> traversable = tileCol => tileCol.Count == 1;
            List<Vector2> retDirections = new List<Vector2>();
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (grid[i, j].Count != 0) grid[i, j][0].dist = int.MaxValue;
                }
            }
            CurrTile.dist = 0;

            DijkstrasAlgHelper(X, Y, 1, MaxDist, traversable);
            BaseMonster Listener;
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    Listener = Globals.Mobs.GetMobAt(i, j);
                    if (grid[i, j][0].dist != int.MaxValue && Listener != null && Listener.Listen(mon));
                }
            }
            return true;
        }

        public List<Vector2> GetPath(int X, int Y, int X2, int Y2) {

            List<Vector2> retDirections = new List<Vector2>();
            while (X2 != X || Y2 != Y)
            {
                BaseTile tile = grid[X2, Y2][0];
                Vector2 dir = (-1 * tile.dirToShortest + new Vector2(X2, Y2));
                tile.adjColor = Color.Gray;
                retDirections.Add(dir.Times(-1));
                X2 = (int)tile.dirToShortest.X;
                Y2 = (int)tile.dirToShortest.Y;
            } 
            return retDirections.Flip();
        }
        //DijkstrasAlg
        public List<Vector2> GetShortestPath(int X, int Y, int X2, int Y2, int MaxDist, Func<List<BaseTile>, bool> traversable)
        {
            BaseTile CurrTile = grid[X, Y][0];
            List<Vector2> retDirections = new List<Vector2>();
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (grid[i, j].Count != 0) grid[i, j][0].dist = int.MaxValue;
                }
            }
            CurrTile.dist = 0;

            DijkstrasAlgHelper(X, Y, 1, MaxDist, traversable);

            if (grid[X2, Y2][0].dist == int.MaxValue) { return null; }
            while (X2 != X || Y2 != Y) 
            {
                BaseTile tile = grid[X2, Y2][0];
                Vector2 dir = (-1*tile.dirToShortest + new Vector2(X2, Y2));
                retDirections.Add(dir);
                X2 = (int) tile.dirToShortest.X;
                Y2 = (int) tile.dirToShortest.Y;
            }

            return retDirections;
        }
        public float GetDistBetween(Vector2 v1, Vector2 v2) 
        {
            return ((v1.X - v2.X) * (v1.X - v2.X)) + ((v1.Y - v2.Y) * (v1.Y - v2.Y));
        }
        public List<Vector2> GetShortestPathFromType(int X, int Y, Func<BaseMonster, bool> GoodMon, int MaxDist, Func<List<BaseTile>, bool> traversable)
        {
            BaseTile CurrTile = grid[X, Y][0]; 
            float tempDist = int.MaxValue;
            int X2 = 10, Y2 = 10;
            List<Vector2> retDirections = new List<Vector2>();
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    if (grid[i, j].Count != 0) grid[i, j][0].dist = int.MaxValue;
                    BaseMonster mon = Globals.Mobs.GetMobAt(i, j);

                    if (mon != null && GoodMon(mon)) 
                    {
                        if (GetDistBetween(mon.GridPos, new Vector2(X, Y)) < tempDist)
                        {
                            tempDist = GetDistBetween(mon.GridPos, new Vector2(X, Y));
                            X2 = (int)mon.GridPos.X; Y2 = (int)mon.GridPos.Y;
                        }
                    }
                }
            }
            CurrTile.dist = 0;

            DijkstrasAlgHelper(X, Y, 1, MaxDist, traversable);

            if (grid[X2, Y2][0].dist == int.MaxValue) { return null; }
            while (X2 != X || Y2 != Y)
            {
                BaseTile tile = grid[X2, Y2][0];
                Vector2 dir = (-1 * tile.dirToShortest + new Vector2(X2, Y2));
                retDirections.Add(dir);
                X2 = (int)tile.dirToShortest.X;
                Y2 = (int)tile.dirToShortest.Y;
            }

            return retDirections;
        }
        
        #endregion

        public void Draw(SpriteBatch batch) {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {
                    
                   if ( grid[i, j].Count != 0) {
                       foreach (BaseTile tile in grid[i, j])
                       {
                           tile.Draw(batch);
                       }
                   }
                }
            }
        }
        
        public Vector2 TranslateToPos(Vector2 farseerPos)
        {
            Player player= Globals.player;
            Vector2 temp = new Vector2(((int)player.circle.Position.X) / 7, ((int)player.circle.Position.Y) / 7).Times(7);
            //gridIndex = gridIndex - player.GridPos + new Vector2(20, 15);
            temp = farseerPos - temp;// +new Vector2(20, 15);

            return temp * 100f;
        }

        internal void Update()
        {
            for (int i = 0; i < MapWidth; i++)
            {
                for (int j = 0; j < MapHeight; j++)
                {

                    if (grid[i, j].Count != 0)
                    {
                        foreach (BaseTile tile in grid[i, j])
                        {
                            tile.Update();
                        }
                    }
                }
            }
        }

        #endregion
    }
}
