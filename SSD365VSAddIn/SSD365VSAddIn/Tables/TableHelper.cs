﻿using Microsoft.Dynamics.AX.Metadata.MetaModel;
using Microsoft.Dynamics.Framework.Tools.Extensibility;
using Microsoft.Dynamics.Framework.Tools.MetaModel.Automation.Tables;
using Microsoft.Dynamics.Framework.Tools.MetaModel.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Dynamics.AX.Server.Core.Service;

namespace SSD365VSAddIn.Tables
{
    class TableHelper
    {
        public static string CreateTableExtension(ITable table)
        {
            AxTableExtension axExtension;
            axExtension = TableHelper.GetFirstExtension(table.Name);
            if(axExtension != null)
            {
                // Add existing extension to project & quit
                Common.CommonUtil.AddElementToProject(axExtension);
                return axExtension.Name;
            }

            var name = table.Name;// + Common.Constants.DotEXTENSION;
            name = Common.CommonUtil.GetNextTableExtension(name);

            // Find current model
            //Create menu item in the right model
            var metaModelProviders = ServiceLocator.GetService<IMetaModelProviders>() as IMetaModelProviders;


            //Create an extension object
            axExtension = new AxTableExtension() { Name = name };
            //var tableExts = metaModelProviders.CurrentMetadataProvider.TableExtensions.Common.CommonUtil.GetCurrentModel().Name);

            Common.CommonUtil.GetMetaModelProviders()
                .CurrentMetadataProvider
                .TableExtensions.Create(axExtension, Common.CommonUtil.GetCurrentModelSaveInfo());

            // Add to project
            Common.CommonUtil.AddElementToProject(axExtension);

            return name;
        }

        public static AxTableExtension GetFirstExtension(string name)
        {
            // Find current model
            var metaModelService = Common.CommonUtil.GetModelSaveService();

            var extensionNames = metaModelService.GetTableExtensionNames()
                                    .ToList()
                                    .Where(tableExtName => tableExtName.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
                                    .ToList();

            if(extensionNames == null)
            {
                return null;
            }

            var currentModel = Common.CommonUtil.GetCurrentModel();

            foreach (var extName in extensionNames)
            {
                var extModels = metaModelService.GetTableExtensionModelInfo(extName).ToList();
                if (extModels != null)
                {
                    foreach (var model in extModels)
                    {
                        if (model.Module.Equals(currentModel.Module, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return metaModelService.GetTableExtension(extName);
                        }
                    }
                }
            }
            //if(String.IsNullOrEmpty(extensionName) == false)
            //{
            //    var extension = metaModelService.GetTableExtension(extensionName);
            //    return extension;
            //}

            return null;
        }
    }
}
