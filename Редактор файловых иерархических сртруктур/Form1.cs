using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Редактор_файловых_иерархических_сртруктур
{
    public partial class Form1 : Form
    {
        private int documentCount = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void createMenuItem_Click(object sender, EventArgs e)
        {
            CreateNewMDIChild();
        }

        private void openMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        private void saveMenuItem_Click(object sender, EventArgs e)
        {
            SaveFile();
        }

        private void saveAsMenuItem_Click(object sender, EventArgs e)
        {
            SaveAsToolStripMenuItemClick();
        }

        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            ExitToolStripMenuItemClick();
        }

        private void copyMenuItem_Click(object sender, EventArgs e)
        {
            CopyToolStripMenuItemClick();
        }

        private void cutMenuItem_Click(object sender, EventArgs e)
        {
            CutToolStripMenuItemClick();
        }

        private void pasteMenuItem_Click(object sender, EventArgs e)
        {
            PasteToolStripMenuItemClick();
        }

        private void deleteMenuItem_Click(object sender, EventArgs e)
        {
            DeleteToolStripMenuItemClick();
        }

        private void renameMenuItem_Click(object sender, EventArgs e)
        {
            RenameNode();
        }

        private void refreshMenuItem_Click(object sender, EventArgs e)
        {
            RefreshToolStripMenuItemClick();
        }

        private void expandAllMenuItem_Click(object sender, EventArgs e)
        {
            ExpandAllToolStripMenuItemClick();
        }

        private void collapseAllMenuItem_Click(object sender, EventArgs e)
        {
            CollapseAllToolStripMenuItemClick();
        }

        private void addMenuItem_Click(object sender, EventArgs e)
        {
            AddToolStripMenuItemClick();
        }

        private void CreateNewMDIChild()
        {
            ChildForm newMDIChild = new ChildForm();
            newMDIChild.MdiParent = this;
            newMDIChild.Text = "Новый редактор";
            newMDIChild.Show();
        }

        private void OpenFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Файлы иерархии (*.hier)|*.hier|Все файлы (*.*)|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (ActiveMdiChild is ChildForm childForm)
                {
                    childForm.LoadHierarchyFromFile(openFileDialog.FileName); // Вызываем метод LoadHierarchyFromFile
                }
            }
        }

        private void SaveFile()
        {
            if (ActiveMdiChild is ChildForm childForm)
            {
                if (string.IsNullOrEmpty(childForm.CurrentFilePath)) // Проверяем, есть ли уже открытый файл
                {
                    SaveAsFile(childForm);
                }
                else
                {
                    SaveExistingFile(childForm);
                }
            }
        }

        private void SaveAsFile(ChildForm childForm)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Файлы иерархии (*.hier)|*.hier|Все файлы (*.*)|*.*";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string savedFilePath;
                childForm.SaveHierarchyToFile(saveFileDialog.FileName, out savedFilePath);
                if (!string.IsNullOrEmpty(savedFilePath))
                {
                    childForm.CurrentFilePath = savedFilePath; // Обновляем _currentFilePath
                    childForm.Text = "Редактор - " + Path.GetFileName(savedFilePath); // Обновляем заголовок
                }
            }
        }

        private void SaveExistingFile(ChildForm childForm)
        {
            string savedFilePath;
            childForm.SaveHierarchyToFile(childForm.CurrentFilePath, out savedFilePath);

            if (!string.IsNullOrEmpty(savedFilePath))
            {
                childForm.Text = "Редактор - " + Path.GetFileName(savedFilePath); // Обновляем заголовок
            }
        }

        private void RenameNode()
        {
            // Получаем активное дочернее окно
            ChildForm activeChild = (ChildForm)this.ActiveMdiChild;
            if (activeChild != null)
            {
                activeChild.RecordTreeViewState();
                // Вызываем метод RenameNode() в активном дочернем окне
                activeChild.RenameNode();
            }
            else
            {
                MessageBox.Show("Нет активного окна для переименования.", "Переименование", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void SaveAsToolStripMenuItemClick()
        {
            if (ActiveMdiChild is ChildForm childForm)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "Файлы иерархии (*.hier)|*.hier|Все файлы (*.*)|*.*";
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string savedFilePath;
                    childForm.SaveHierarchyToFile(saveFileDialog.FileName, out savedFilePath);

                    if (!string.IsNullOrEmpty(savedFilePath))
                    {
                        childForm.CurrentFilePath = savedFilePath; // Обновляем _currentFilePath
                        childForm.Text = "Редактор - " + Path.GetFileName(savedFilePath); // Обновляем заголовок
                    }
                }
            }
        }

        private void ExitToolStripMenuItemClick()
        {
            Application.Exit();
        }

        private void CopyToolStripMenuItemClick()
        {
            if (ActiveMdiChild is ChildForm childForm)
            {
                childForm.CopyNode(); // Вызываем метод CopyNode() из ChildForm
            }
        }

        private void CutToolStripMenuItemClick()
        {
            if (ActiveMdiChild is ChildForm childForm)
            {
                childForm.CutNode(); // Вызываем метод CutNode() из ChildForm
            }
        }

        private void PasteToolStripMenuItemClick()
        {
            if (ActiveMdiChild is ChildForm childForm)
            {
                // 1. Сохраняем состояние дерева до вставки
                childForm.RecordTreeViewState();

                // 2. Вызываем метод PasteNode()
                childForm.PasteNode();

                // 3. Сохраняем состояние дерева после вставки
                childForm.RecordTreeViewState();
            }
        }

        private void DeleteToolStripMenuItemClick()
        {
            if (ActiveMdiChild is ChildForm childForm)
            {
                // 1. Сохраняем состояние дерева до удаления узла
                childForm.RecordTreeViewState();

                // 2. Вызываем метод DeleteNode()
                childForm.DeleteNode();

                // 3. Сохраняем состояние дерева после удаления узла
                childForm.RecordTreeViewState();
            }
        }
        private void RefreshToolStripMenuItemClick()
        {
            if (ActiveMdiChild is ChildForm childForm)
            {
                // **Перед** обновлением состояния, сохраняем текущее состояние дерева
                childForm.RecordTreeViewState();

                childForm.RefreshView(); // Вызываем метод RefreshView() из ChildForm

                // **После** обновления состояния, *снова* сохраняем состояние дерева
                // Это нужно, так как RefreshView() изменил состояние дерева
                childForm.RecordTreeViewState();
            }
        }

        private void ExpandAllToolStripMenuItemClick()
        {
            if (ActiveMdiChild is ChildForm childForm)
            {
                childForm.ExpandAllNodes(); // Вызываем метод ExpandAllNodes() из ChildForm
            }
        }

        private void CollapseAllToolStripMenuItemClick()
        {
            if (ActiveMdiChild is ChildForm childForm)
            {
                childForm.CollapseAllNodes(); // Вызываем метод CollapseAllNodes() из ChildForm
            }
        }

        private void AddToolStripMenuItemClick()
        {
            // 1. Получаем ссылку на активный ChildForm
            ChildForm activeChild = (ChildForm)this.ActiveMdiChild;

            // 2. Проверяем, что активный ChildForm существует
            if (activeChild != null)
            {
                // 3. Спрашиваем имя для нового узла
                string newNodeName = Microsoft.VisualBasic.Interaction.InputBox("Введите имя нового узла:", "Добавление узла", "Новый узел");

                // Проверяем, что пользователь не нажал "Отмена" или не ввел пустое имя
                if (!string.IsNullOrEmpty(newNodeName))
                {
                    // 4. Вызываем метод в ChildForm для добавления узла
                    activeChild.AddNode(newNodeName);

                    // 5. Записываем состояние дерева для undo/redo
                    activeChild.RecordTreeViewState(); // <--- ВАЖНО!
                }
            }
            else
            {
                MessageBox.Show("Нет активного окна для добавления узла.", "Добавление узла", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
