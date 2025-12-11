using System.Collections.Generic;
using UnityEngine;

namespace Game.EnemyAI
{
    /// <summary>
    /// A*算法核心实现
    /// </summary>
    public class AStar
    {
        private List<AStarNode> openList;
        private List<AStarNode> closeList;
        private AStarNode[,] grid;
        private int gridWidth;
        private int gridHeight;
        private float nodeSize;

        public AStar(int width, int height, float size)
        {
            gridWidth = width;
            gridHeight = height;
            nodeSize = size;
            grid = new AStarNode[width, height];
            
            // 初始化网格
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector3 worldPos = new Vector3(x * size, 0, y * size);
                    bool isWalkable = !Physics.CheckSphere(worldPos, size * 0.4f);
                    grid[x, y] = new AStarNode(x, y, isWalkable, worldPos);
                }
            }
        }

        /// <summary>
        /// 查找路径
        /// </summary>
        public List<Vector3> FindPath(Vector3 startPos, Vector3 targetPos)
        {
            // 转换为网格坐标
            Vector2Int startGrid = WorldToGrid(startPos);
            Vector2Int targetGrid = WorldToGrid(targetPos);

            // 边界检查
            if (!IsInBounds(startGrid) || !IsInBounds(targetGrid))
                return null;

            // 获取起点和终点节点
            AStarNode startNode = grid[startGrid.x, startGrid.y];
            AStarNode targetNode = grid[targetGrid.x, targetGrid.y];

            // 初始化开放列表和关闭列表
            openList = new List<AStarNode> { startNode };
            closeList = new List<AStarNode>();

            // 重置所有节点
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    grid[x, y].gCost = float.MaxValue;
                    grid[x, y].fCost = float.MaxValue;
                    grid[x, y].parent = null;
                }
            }

            // 设置起点
            startNode.gCost = 0;
            startNode.hCost = CalculateDistance(startNode, targetNode);
            startNode.fCost = startNode.gCost + startNode.hCost;

            while (openList.Count > 0)
            {
                // 获取fCost最小的节点
                AStarNode currentNode = GetLowestFCostNode();
                
                // 到达终点
                if (currentNode == targetNode)
                {
                    return RetracePath(startNode, targetNode);
                }

                openList.Remove(currentNode);
                closeList.Add(currentNode);

                // 检查相邻节点
                foreach (AStarNode neighbor in GetNeighbors(currentNode))
                {
                    if (!neighbor.isWalkable || closeList.Contains(neighbor))
                        continue;

                    float newGCost = currentNode.gCost + CalculateDistance(currentNode, neighbor);
                    if (newGCost < neighbor.gCost)
                    {
                        neighbor.parent = currentNode;
                        neighbor.gCost = newGCost;
                        neighbor.hCost = CalculateDistance(neighbor, targetNode);
                        neighbor.fCost = neighbor.gCost + neighbor.hCost;

                        if (!openList.Contains(neighbor))
                        {
                            openList.Add(neighbor);
                        }
                    }
                }
            }

            // 没有找到路径
            return null;
        }

        /// <summary>
        /// 获取fCost最小的节点
        /// </summary>
        private AStarNode GetLowestFCostNode()
        {
            AStarNode lowest = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < lowest.fCost)
                {
                    lowest = openList[i];
                }
            }
            return lowest;
        }

        /// <summary>
        /// 计算两个节点之间的距离
        /// </summary>
        private float CalculateDistance(AStarNode nodeA, AStarNode nodeB)
        {
            int dx = Mathf.Abs(nodeA.x - nodeB.x);
            int dy = Mathf.Abs(nodeA.y - nodeB.y);
            return dx + dy; // 曼哈顿距离
        }

        /// <summary>
        /// 回溯生成路径
        /// </summary>
        private List<Vector3> RetracePath(AStarNode startNode, AStarNode endNode)
        {
            List<Vector3> path = new List<Vector3>();
            AStarNode current = endNode;

            while (current != startNode)
            {
                path.Add(current.worldPosition);
                current = current.parent;
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        /// 获取相邻节点
        /// </summary>
        private List<AStarNode> GetNeighbors(AStarNode node)
        {
            List<AStarNode> neighbors = new List<AStarNode>();

            // 四个方向
            int[,] directions = { { 0, -1 }, { 1, 0 }, { 0, 1 }, { -1, 0 } };
            
            for (int i = 0; i < directions.GetLength(0); i++)
            {
                int nx = node.x + directions[i, 0];
                int ny = node.y + directions[i, 1];
                
                if (IsInBounds(nx, ny))
                {
                    neighbors.Add(grid[nx, ny]);
                }
            }

            return neighbors;
        }

        /// <summary>
        /// 检查是否在网格范围内
        /// </summary>
        private bool IsInBounds(int x, int y)
        {
            return x >= 0 && x < gridWidth && y >= 0 && y < gridHeight;
        }

        private bool IsInBounds(Vector2Int pos)
        {
            return IsInBounds(pos.x, pos.y);
        }

        /// <summary>
        /// 世界坐标转网格坐标
        /// </summary>
        private Vector2Int WorldToGrid(Vector3 worldPos)
        {
            int x = Mathf.FloorToInt(worldPos.x / nodeSize);
            int y = Mathf.FloorToInt(worldPos.z / nodeSize);
            return new Vector2Int(x, y);
        }
    }

    /// <summary>
    /// A*算法节点类
    /// </summary>
    public class AStarNode
    {
        public int x;
        public int y;
        public bool isWalkable;
        public Vector3 worldPosition;
        public float gCost;
        public float hCost;
        public float fCost; // 修改为可写字段
        public AStarNode parent;

        public AStarNode(int x, int y, bool isWalkable, Vector3 worldPos)
        {
            this.x = x;
            this.y = y;
            this.isWalkable = isWalkable;
            this.worldPosition = worldPos;
        }
    }
}