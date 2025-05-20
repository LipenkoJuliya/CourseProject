using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Редактор_файловых_иерархических_сртруктур
{
    public partial class ChildForm : Form
    {
        private bool _drivesLoaded = false;
        private string _currentFilePath = null;
        private Stack<TreeViewState> undoStack = new Stack<TreeViewState>();
        private Stack<TreeViewState> redoStack = new Stack<TreeViewState>();
        private TreeNode _copiedNode = null; // Поле для хранения скопированного узла
        private bool _isCut = false; // Флаг, указывающий, была ли выполнена операция "вырезать"

        [Serializable] // Помечаем класс как сериализуемый
        public class TreeViewState
        {
            public List<TreeNodeData> Nodes { get; set; }
            public string SelectedNodePath { get; set; } // Путь к выбранному узлу
        }

        [Serializable]
        public class TreeNodeData
        {
            public string Text { get; set; }
            public bool IsExpanded { get; set; }
            public List<TreeNodeData> Children { get; set; }
        }

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            set { _currentFilePath = value; }
        }

        public ChildForm()
        {
            InitializeComponent();
            this.Width = 800;
            this.Height = 600;
            this.Load += ChildForm_Load;
            UpdateUndoRedoButtons();

            // Подписываемся на события Click пунктов меню
            renameToolStripMenuItem.Click += renameToolStripMenuItem_Click;
            copyToolStripMenuItem.Click += copyToolStripMenuItem_Click;
            cutToolStripMenuItem.Click += cutToolStripMenuItem_Click;
            pasteToolStripMenuItem.Click += pasteToolStripMenuItem_Click;
            deleteToolStripMenuItem.Click += deleteToolStripMenuItem_Click;
            addToolStripMenuItem.Click += addToolStripMenuItem_Click;
        }

        private void ChildForm_Load(object sender, EventArgs e)
        {
            Console.WriteLine("ChildForm_Load called");

            if (!_drivesLoaded)
            {
                LoadDrivesAsync();
                _drivesLoaded = true;
            }
        }

        private async Task LoadDrivesAsync()
        {
            // Get all drives
            DriveInfo[] drives = DriveInfo.GetDrives();

            // Create a root node for each drive
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady) // Check if the drive is ready (e.g., not a disconnected USB drive)
                {
                    await AddDriveNodeAsync(drive); // Load the drive node asynchronously
                }
            }
        }

        private async Task AddDriveNodeAsync(DriveInfo drive)
        {
            TreeNode driveNode = new TreeNode(drive.Name);
            driveNode.Tag = drive.Name; // Store the drive path in the Tag property

            // Add a dummy node to indicate that the drive has children
            try
            {
                // Use Task.Run to avoid blocking the UI thread during directory checks
                bool hasDirectories = await Task.Run(() => Directory.GetDirectories(drive.Name).Length > 0);
                bool hasFiles = await Task.Run(() => Directory.GetFiles(drive.Name).Length > 0);

                if (hasDirectories || hasFiles)
                {
                    driveNode.Nodes.Add(new TreeNode("Загрузка..."));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"UnauthorizedAccessException while checking drive {drive.Name}: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IOException while checking drive {drive.Name}: {ex.Message}");
            }

            treeView1.Nodes.Add(driveNode); // Add the drive node to the treeview
        }

        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;

            // Prevent multiple expansions
            if (node.Nodes.Count > 0 && node.Nodes[0].Text == "Загрузка...")
            {
                node.Nodes.Clear(); // Remove the "Loading..." node
                LoadChildNodesAsync(node); // Load the actual child nodes
            }
        }

        public void LoadHierarchyFromFile(string filePath)
        {
            try
            {
                treeView1.Nodes.Clear(); // Очищаем существующие узлы

                // Создаем корневой узел. Имя узла - имя файла
                TreeNode rootNode = new TreeNode(Path.GetFileNameWithoutExtension(filePath));
                rootNode.Tag = filePath; // Сохраняем путь к файлу в свойстве Tag

                // Загружаем данные из файла (здесь нужно реализовать логику разбора вашего формата *.hier)
                // Пример:
                LoadDataToRootNode(rootNode, filePath);

                treeView1.Nodes.Add(rootNode); // Добавляем корневой узел в TreeView
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке иерархии из файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadDataToRootNode(TreeNode rootNode, string filePath)
        {
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    rootNode.Nodes.Add(line); // Добавляем строки файла как дочерние узлы
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при чтении данных из файла: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void LoadChildNodesAsync(TreeNode node)
        {
            string path = node.Tag.ToString();

            try
            {
                TreeNode tempNode = await Task.Run(() => CreatePopulatedNode(path));

                if (!IsDisposed)
                {
                    UpdateTreeNode(node, tempNode);
                }
                Console.WriteLine($"Finished loading child nodes for {path}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading child nodes for {path}: {ex.Message}");
            }
        }

        private TreeNode CreatePopulatedNode(string path)
        {
            TreeNode tempNode = new TreeNode(Path.GetFileName(path));
            tempNode.Tag = path;

            PopulateNodes(tempNode, path, 3); // Call the synchronous PopulateNodes

            return tempNode;
        }

        private void UpdateTreeNode(TreeNode node, TreeNode tempNode)
        {
            if (treeView1.InvokeRequired)
            {
                treeView1.Invoke(new Action(() =>
                {
                    node.Nodes.Clear(); // Remove "Loading..." and any previous children
                    foreach (TreeNode child in tempNode.Nodes)
                    {
                        node.Nodes.Add(child);
                    }
                }));
            }
            else
            {
                node.Nodes.Clear(); // Remove "Loading..." and any previous children
                foreach (TreeNode child in tempNode.Nodes)
                {
                    node.Nodes.Add(child);
                }
            }
        }

        public void SaveHierarchyToFile(string filePath, out string savedFilePath)
        {
            try
            {
                // Создаем список строк для записи в файл
                List<string> lines = new List<string>();

                // Обходим все узлы в TreeView и добавляем их текст в список
                foreach (TreeNode node in treeView1.Nodes)
                {
                    CollectNodeText(node, lines); // Используем рекурсивный метод для обхода всех дочерних узлов
                }

                // Записываем список строк в файл
                File.WriteAllLines(filePath, lines);

                MessageBox.Show("Иерархия успешно сохранена в файл.", "Сохранение", MessageBoxButtons.OK, MessageBoxIcon.Information);

                savedFilePath = filePath; // Возвращаем сохраненный путь
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при сохранении иерархии в файл: {ex.Message}", "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                savedFilePath = null; // Возвращаем null в случае ошибки
            }
        }

        public void CopyNode()
        {
            if (treeView1.SelectedNode != null)
            {
                CopySelectedNode();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите узел для копирования.", "Копирование", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CopySelectedNode()
        {
            _copiedNode = (TreeNode)treeView1.SelectedNode.Clone(); // Клонируем выбранный узел
            _isCut = false; // Сбрасываем флаг "вырезать"
            MessageBox.Show("Узел скопирован.", "Копирование", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void CutNode()
        {
            if (treeView1.SelectedNode != null)
            {
                CutSelectedNode();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите узел для вырезания.", "Вырезание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void CutSelectedNode()
        {
            _copiedNode = (TreeNode)treeView1.SelectedNode.Clone(); // Клонируем выбранный узел
            _isCut = true; // Устанавливаем флаг "вырезать"
            MessageBox.Show("Узел вырезан.", "Вырезание", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void PasteNode()
        {
            if (_copiedNode != null && treeView1.SelectedNode != null)
            {
                PasteCopiedNode();
            }
            else
            {
                MessageBox.Show("Пожалуйста, сначала скопируйте или вырежьте узел, а затем выберите родительский узел для вставки.", "Вставка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void PasteCopiedNode()
        {
            TreeNode newNode = (TreeNode)_copiedNode.Clone();  // Клонируем скопированный узел (снова, для вставки)

            if (_isCut)
            {
                // Если это операция "вырезать", удаляем исходный узел
                RemoveCutNode();
            }

            treeView1.SelectedNode.Nodes.Add(newNode); // Добавляем новый узел в выбранный узел
            treeView1.SelectedNode.Expand(); // Раскрываем родительский узел
            MessageBox.Show("Узел вставлен.", "Вставка", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void RemoveCutNode()
        {
            TreeNode nodeToRemove = treeView1.SelectedNode;  // БЫЛА ОШИБКА - удаляли скопированный, а не вырезанный
            if (nodeToRemove.Parent != null)
            {
                nodeToRemove.Parent.Nodes.Remove(nodeToRemove);
            }
            else
            {
                treeView1.Nodes.Remove(nodeToRemove);  // Корень удаляем иначе
            }
            _isCut = false; // Сбрасываем флаг "вырезать"
            _copiedNode = null; // Очищаем скопированный узел, т.к. он уже вставлен и удален
        }

        public void DeleteNode()
        {
            if (treeView1.SelectedNode != null)
            {
                DeleteSelectedNode();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите узел для удаления.", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void DeleteSelectedNode()
        {
            TreeNode nodeToRemove = treeView1.SelectedNode;

            // Проверяем, является ли узел корневым (т.е. находится непосредственно в Nodes TreeView)
            if (nodeToRemove.Parent != null)
            {
                // Узел не корневой, удаляем его из родительского узла
                nodeToRemove.Parent.Nodes.Remove(nodeToRemove);
            }
            else
            {
                // Узел корневой, удаляем его непосредственно из TreeView
                treeView1.Nodes.Remove(nodeToRemove);
            }

            MessageBox.Show("Узел удален.", "Удаление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void RenameNode()
        {
            if (treeView1.SelectedNode != null)
            {
                StartNodeRenaming();
            }
            else
            {
                MessageBox.Show("Пожалуйста, выберите узел для переименования.", "Переименование", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void StartNodeRenaming()
        {
            treeView1.LabelEdit = true; // Включаем редактирование меток
            treeView1.SelectedNode.BeginEdit();
        }

        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            HandleAfterLabelEdit(e);
        }

        private void HandleAfterLabelEdit(NodeLabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                // Проверяем, что новый текст не пустой и не состоит только из пробелов
                if (e.Label.Trim().Length > 0)
                {
                    e.Node.Text = e.Label.Trim(); // Устанавливаем новый текст узла
                    e.Node.EndEdit(false); // Завершаем редактирование (false = сохранить изменения)
                }
                else
                {
                    // Если текст пустой, отменяем редактирование и выводим сообщение
                    CancelNodeEdit(e, "Нельзя задать пустое имя узла.");
                }
            }
            else
            {
                // Если e.Label == null, значит редактирование было отменено пользователем (например, нажатием Esc)
                CancelNodeEdit(e, null); // Сообщение не нужно, т.к. пользователь отменил
            }
        }

        private void CancelNodeEdit(NodeLabelEditEventArgs e, string message)
        {
            e.CancelEdit = true;
            e.Node.EndEdit(true); // true = отменить изменения
            if (!string.IsNullOrEmpty(message))
            {
                MessageBox.Show(message, "Переименование", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        public void RefreshView()
        {
            treeView1.Nodes.Clear(); // Очищаем TreeView
            _drivesLoaded = false; // Сбрасываем флаг загрузки дисков
            ChildForm_Load(this, EventArgs.Empty); // Повторно вызываем загрузку дисков
            MessageBox.Show("Дерево каталогов обновлено.", "Обновление", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void ExpandAllNodes()
        {
            treeView1.ExpandAll();
            MessageBox.Show("Все узлы развернуты.", "Развернуть все", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void CollapseAllNodes()
        {
            treeView1.CollapseAll();
            MessageBox.Show("Все узлы свернуты.", "Свернуть все", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        public void AddNode(string newNodeName)
        {
            AddNodeToTreeView(newNodeName);
        }

        private void AddNodeToTreeView(string newNodeName)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null)
            {
                // 1. Получаем путь к выбранной папке
                string selectedPath = GetFullPath(selectedNode);

                // 2. Создаем новый подкаталог в файловой системе
                string newFolderPath = Path.Combine(selectedPath, newNodeName);

                try
                {
                    Directory.CreateDirectory(newFolderPath);

                    // 3. Добавляем новый узел в TreeView
                    TreeNode newNode = new TreeNode(newNodeName);
                    selectedNode.Nodes.Add(newNode);
                    selectedNode.Expand();

                    // 4. Выбираем и начинаем редактирование нового узла
                    treeView1.SelectedNode = newNode;
                    treeView1.LabelEdit = true;
                    newNode.BeginEdit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Не удалось создать папку: " + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Выберите родительский узел для добавления.", "Добавление узла", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
        private string GetFullPath(TreeNode node)
        {
            string path = node.Text;
            TreeNode parent = node.Parent;
            while (parent != null)
            {
                path = Path.Combine(parent.Text, path);
                parent = parent.Parent;
            }
            return path;
        }

        private string GetNodePath(TreeNode node)
        {
            string path = node.Text;
            TreeNode parent = node.Parent;
            while (parent != null)
            {
                path = parent.Text + "/" + path;
                parent = parent.Parent;
            }
            return path;
        }

        private TreeNode FindNodeByPath(TreeNodeCollection nodes, string path)
        {
            string[] parts = path.Split('/');
            TreeNode currentNode = null;

            TreeNodeCollection currentNodes = nodes;
            for (int i = 0; i < parts.Length; i++)
            {
                bool found = false;
                foreach (TreeNode node in currentNodes)
                {
                    if (node.Text == parts[i])
                    {
                        currentNode = node;
                        currentNodes = node.Nodes;
                        found = true;
                        break;
                    }
                }
                if (!found) return null;
            }

            return currentNode;
        }

        public TreeViewState CaptureTreeViewState()
        {
            TreeViewState state = new TreeViewState();
            state.Nodes = CaptureNodes(treeView1.Nodes);

            // Сохраняем путь к выбранному узлу
            if (treeView1.SelectedNode != null)
            {
                state.SelectedNodePath = GetNodePath(treeView1.SelectedNode);
            }

            return state;
        }

        private List<TreeNodeData> CaptureNodes(TreeNodeCollection nodes)
        {
            List<TreeNodeData> dataList = new List<TreeNodeData>();
            foreach (TreeNode node in nodes)
            {
                TreeNodeData data = new TreeNodeData
                {
                    Text = node.Text,
                    IsExpanded = node.IsExpanded,
                    Children = CaptureNodes(node.Nodes)
                };
                dataList.Add(data);
            }
            return dataList;
        }

        public void RecordTreeViewState()
        {
            BeforeTreeViewChange();
        }

        private void BeforeTreeViewChange()
        {
            // Очищаем redoStack, так как новая операция делает redo невозможным
            redoStack.Clear();
            // Сохраняем текущее состояние в undoStack
            undoStack.Push(CaptureTreeViewState());

            // Обновляем состояние кнопок "Вперед" и "Назад"
            UpdateUndoRedoButtons();
        }

        public void RestoreTreeViewState(TreeView treeView, TreeViewState state)
        {
            treeView.Nodes.Clear();
            RestoreNodes(treeView.Nodes, state.Nodes);

            // Восстанавливаем выбранный узел
            if (!string.IsNullOrEmpty(state.SelectedNodePath))
            {
                TreeNode selectedNode = FindNodeByPath(treeView.Nodes, state.SelectedNodePath);
                if (selectedNode != null)
                {
                    treeView.SelectedNode = selectedNode;
                }
            }
        }

        private void RestoreNodes(TreeNodeCollection nodes, List<TreeNodeData> dataList)
        {
            foreach (TreeNodeData data in dataList)
            {
                TreeNode node = new TreeNode(data.Text);
                //node.IsExpanded = data.IsExpanded; // Неправильно!
                nodes.Add(node);
                RestoreNodes(node.Nodes, data.Children);

                if (data.IsExpanded)
                {
                    node.Expand(); // Правильно: разворачиваем узел, если нужно
                }
            }
        }
        // Рекурсивный метод для сбора текста всех узлов (включая дочерние)
        private void CollectNodeText(TreeNode node, List<string> lines)
        {
            lines.Add(node.Text); // Добавляем текст текущего узла

            foreach (TreeNode childNode in node.Nodes)
            {
                CollectNodeText(childNode, lines); // Рекурсивно вызываем для дочерних узлов
            }
        }

        private void PopulateNodes(TreeNode parentNode, string path, int depth)
        {
            if (depth <= 0)
            {
                Console.WriteLine("PopulateNodes: depth limit reached, returning");
                return;
            }

            Console.WriteLine("PopulateNodes: path = " + path + ", depth = " + depth);

            try
            {
                PopulateDirectories(parentNode, path, depth);
                PopulateFiles(parentNode, path);
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine("UnauthorizedAccessException in PopulateNodes: " + ex.Message + ", path = " + path);
            }
            catch (IOException ex)
            {
                Console.WriteLine("IOException in PopulateNodes: " + ex.Message + ", path = " + path);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception in PopulateNodes: " + ex.Message + ", path = " + path);
            }
        }

        private void PopulateDirectories(TreeNode parentNode, string path, int depth)
        {
            string[] directories = Directory.GetDirectories(path);
            foreach (string directory in directories)
            {
                TreeNode node = CreateDirectoryNode(directory);
                parentNode.Nodes.Add(node);
            }
        }

        private TreeNode CreateDirectoryNode(string directory)
        {
            TreeNode node = new TreeNode(Path.GetFileName(directory));
            node.Tag = directory;

            // Указывает, что у 
            // This will trigger the NodeMouseClick event when the user expands the node
            try
            {
                if (Directory.GetDirectories(directory).Length > 0 || Directory.GetFiles(directory).Length > 0)
                {
                    node.Nodes.Add(new TreeNode("Загрузка..."));
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Console.WriteLine($"UnauthorizedAccessException while checking directory {directory}: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"IOException while checking directory {directory}: {ex.Message}");
            }
            return node;
        }

        private void PopulateFiles(TreeNode parentNode, string path)
        {
            string[] files = Directory.GetFiles(path);
            foreach (string file in files)
            {
                TreeNode node = new TreeNode(Path.GetFileName(file));
                node.Tag = file;
                parentNode.Nodes.Add(node);
            }
        }
        private void Undo()
        {
            if (undoStack.Count > 0)
            {
                PerformUndo();
            }
        }

        private void PerformUndo()
        {
            // Сохраняем текущее состояние в redoStack
            redoStack.Push(CaptureTreeViewState());
            // Восстанавливаем предыдущее состояние из undoStack
            TreeViewState previousState = undoStack.Pop();
            RestoreTreeViewState(treeView1, previousState);

            // Обновляем состояние кнопок "Вперед" и "Назад"
            UpdateUndoRedoButtons();
        }

        private void Redo()
        {
            if (redoStack.Count > 0)
            {
                PerformRedo();
            }
        }

        private void PerformRedo()
        {
            // Сохраняем текущее состояние в undoStack
            undoStack.Push(CaptureTreeViewState());
            // Восстанавливаем следующее состояние из redoStack
            TreeViewState nextState = redoStack.Pop();
            RestoreTreeViewState(treeView1, nextState);

            // Обновляем состояние кнопок "Вперед" и "Назад"
            UpdateUndoRedoButtons();
        }

        private void UpdateUndoRedoButtons()
        {
            buttonUndo.Enabled = undoStack.Count > 0;
            buttonRedo.Enabled = redoStack.Count > 0;
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            Undo(); // Вызываем метод Undo, который мы реализуем позже
        }

        private void buttonRedo_Click(object sender, EventArgs e)
        {
            Redo(); // Вызываем метод Redo, который мы реализуем позже
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 1. Сохраняем состояние дерева перед переименованием
            RecordTreeViewState();

            // 2. Вызываем метод RenameNode()
            RenameNode();

            // 3. Сохраняем состояние дерева после переименования
            RecordTreeViewState();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyNode(); // Копирование не меняет структуру, поэтому undo/redo не требуется
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Вырезание подготавливает к вставке, поэтому состояние запоминать не нужно
            CutNode();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 1. Сохраняем состояние дерева перед вставкой
            RecordTreeViewState();

            // 2. Вызываем метод PasteNode()
            PasteNode();

            // 3. Сохраняем состояние дерева после вставки
            RecordTreeViewState();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 1. Сохраняем состояние дерева до удаления узла
            RecordTreeViewState();

            // 2. Вызываем метод DeleteNode()
            DeleteNode();

            // 3. Сохраняем состояние дерева после удаления узла
            RecordTreeViewState();
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 1. Спрашиваем имя для нового узла
            string newNodeName = Microsoft.VisualBasic.Interaction.InputBox("Введите имя нового узла:", "Добавление узла", "Новый узел");

            // Проверяем, что пользователь не нажал "Отмена" или не ввел пустое имя
            if (!string.IsNullOrEmpty(newNodeName))
            {
                // 2. Сохраняем состояние дерева для undo/redo
                RecordTreeViewState();

                // 3. Вызываем метод в ChildForm для добавления узла
                AddNode(newNodeName);

                // 4. Записываем состояние дерева для undo/redo
                RecordTreeViewState(); // <--- ВАЖНО!
            }
        }
        private void treeView1_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                treeView1.SelectedNode = e.Node;
            }
        }
    }
}