﻿using Microsoft.Dynamics.AX.Metadata.MetaModel;
using Microsoft.Dynamics.Framework.Tools.Extensibility;
using Microsoft.Dynamics.Framework.Tools.MetaModel.Automation.Forms;
using Microsoft.Dynamics.AX.Server.Core.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SSD365VSAddIn.Forms
{
    class FormHelper
    {
        public static string CreateExtension(IForm form)
        {
            AxFormExtension axExtension = FormHelper.GetFirstExtension(form.Name);
            if (axExtension != null)
            {
                // Add existing extension to project & quit
                Common.CommonUtil.AddElementToProject(axExtension);
                return axExtension.Name;
            }
            var name = form.Name;// + Common.Constants.DotEXTENSION;
            name = Common.CommonUtil.GetNextFormExtension(name);

            //Create an extension object
            axExtension = new AxFormExtension() { Name = name };
            //var tableExts = metaModelProviders.CurrentMetadataProvider.TableExtensions.Common.CommonUtil.GetCurrentModel().Name);

            Common.CommonUtil.GetMetaModelProviders()
                .CurrentMetadataProvider
                .FormExtensions
                .Create(axExtension, Common.CommonUtil.GetCurrentModelSaveInfo());

            // Add to project
            Common.CommonUtil.AddElementToProject(axExtension);

            return name;
        }

        public static AxFormExtension GetFirstExtension(string name)
        {
            // Find current model
            var metaModelService = Common.CommonUtil.GetModelSaveService();

            var extensionNames = metaModelService.MetadataProvider.FormExtensions.GetPrimaryKeys().ToList()
                                    .Where(extName => extName.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
                                    .ToList();


            if (extensionNames == null)
            {
                return null;
            }

            var currentModel = Common.CommonUtil.GetCurrentModel();
            foreach (var extName in extensionNames)
            {
                var extModels = metaModelService.GetFormExtensionModelInfo(extName).ToList();
                if (extModels != null)
                {
                    foreach (var model in extModels)
                    {
                        if (model.Module.Equals(currentModel.Module, StringComparison.InvariantCultureIgnoreCase))
                        {
                            return metaModelService.GetFormExtension(extName);
                        }
                    }
                }
            }

            return null;
        }
    }
}
