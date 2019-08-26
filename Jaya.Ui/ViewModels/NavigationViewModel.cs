﻿using Jaya.Ui.Models;
using Jaya.Ui.Services;
using System;

namespace Jaya.Ui.ViewModels
{
    public class NavigationViewModel : ViewModelBase
    {
        readonly ConfigurationService _configService;
        readonly Subscription<DirectoryModel> _onDirectoryChanged;

        public NavigationViewModel()
        {
            _configService = GetService<ConfigurationService>();
            _onDirectoryChanged = EventAggregator.Subscribe<DirectoryModel>(OnDirectoryChanged);

            Node = new TreeNodeModel(null, null);
            Populate(Node);
        }

        ~NavigationViewModel()
        {
            EventAggregator.UnSubscribe(_onDirectoryChanged);
        }

        public PaneConfigModel PaneConfig => _configService.PaneConfiguration;

        public ApplicationConfigModel ApplicationConfig => _configService.ApplicationConfiguration;

        public TreeNodeModel Node { get; }

        public TreeNodeModel SelectedNode
        {
            get => Get<TreeNodeModel>();
            set
            {
                Set(value);

                if (value == null)
                    return;

                if (value.FileSystemObject != null)
                    EventAggregator.Publish(value.FileSystemObject as DirectoryModel);
            }
        }

        void OnDirectoryChanged(DirectoryModel directory)
        {
            
        }

        void OnNodeExpanded(TreeNodeModel node, bool isExpaded)
        {
            if (!isExpaded)
                return;

            if (node.IsHavingDummyChild)
                Populate(node);
        }

        void Populate(TreeNodeModel node)
        {
            if (node == null)
                throw new ArgumentNullException(nameof(node));

            if (node.IsExpanded && !node.IsHavingDummyChild)
                return;

            node.RemoveDummyChild();

            if (node.Service == null)
            {
                foreach (var service in GetService<ProviderService>().Services)
                {
                    var child = new TreeNodeModel(service, null);
                    child.Label = service.Name;
                    child.ImagePath = service.ImagePath;
                    child.NodeExpanded += OnNodeExpanded;
                    node.Children.Add(child);

                    child.AddDummyChild();
                }
            }
            else if (node.Provider == null)
            {
                var provider = node.Service.GetDefaultProvider();

                var child = new TreeNodeModel(node.Service, provider);
                child.Label = provider.Name;
                child.ImagePath = provider.ImagePath;
                child.NodeExpanded += OnNodeExpanded;
                node.Children.Add(child);

                child.AddDummyChild();
            }
            else
            {
                var currentDirectory = node.Service.GetDirectory(node.Provider, node.FileSystemObject?.Path);
                foreach (var directory in currentDirectory.Directories)
                {
                    var child = new TreeNodeModel(node.Service, node.Provider);
                    child.Label = directory.Name;
                    child.FileSystemObject = directory;
                    child.NodeExpanded += OnNodeExpanded;
                    node.Children.Add(child);

                    child.AddDummyChild();
                }
            }
        }
    }
}
