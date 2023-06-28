import React from "react";

import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import api from "@docspace/common/api";

import toastr from "@docspace/components/toast/toastr";
import Button from "@docspace/components/button";

import PluginList from "./sub-components/plugin-list";
import EmptyContainer from "./sub-components/empty-container";

import { StyledContainer } from "./StyledWebPlugins";

const WebPlugins = ({
  setDocumentTitle,
  theme,
  withDelete,
  withUpload,

  plugins,
  initPlugin,
  changePluginStatus,
  uninstallPlugin,
}) => {
  const [pluginList, setPluginList] = React.useState(null);

  const inputPluginElement = React.useRef(null);

  const { t } = useTranslation([
    "Settings",
    "Common",
    "PeopleTranslations",
    "People",
    "Article",
    "FilesSettings",
  ]);

  setDocumentTitle("Web plugins");

  const addPlugin = (plugin) => {
    setPluginList((value) => {
      if (value) return [...value, plugin];

      return [plugin];
    });
  };

  const uploadPlugin = async (files) => {
    if (!files) return;

    let formData = new FormData();

    for (let index in Object.keys(files)) {
      formData.append(files[index].name, files[index]);
    }

    try {
      const plugin = await api.plugins.uploadPlugin(formData);

      if (plugin.error) return toastr.error(plugin.error);

      if (plugin) {
        initPlugin(plugin, addPlugin);
      }
    } catch (e) {
      console.log(e);
    }
  };

  const onInput = (e) => {
    uploadPlugin(e.target.files);
    e.target.value = null;
  };

  const onUploadPluginClick = () => {
    withUpload && inputPluginElement.current.click();
  };

  const onActivate = (id, status) => {
    changePluginStatus && changePluginStatus(id, status);
    setPluginList((val) => {
      const newPlugins = val;

      const idx = newPlugins.findIndex((plugin) => +plugin.id === +id);

      if (idx > -1) {
        newPlugins[idx].isActive = status === "true";
      }

      return [...newPlugins];
    });
  };

  const onDelete = (id) => {
    uninstallPlugin && uninstallPlugin(id);
    setPluginList((val) => {
      const newPlugins = val.filter((plugin) => +plugin.id !== +id);

      if (newPlugins.length === 0) return [];

      return [...newPlugins];
    });
  };

  React.useEffect(() => {
    const newPlugins = [];

    Array.from(plugins, ([key, value]) => newPlugins.push(value));

    setPluginList(newPlugins);
  }, [plugins]);

  return (
    <StyledContainer>
      {withUpload && !!pluginList?.length && (
        <Button
          className={"plugins__upload-button"}
          size={"small"}
          label={t("Article:Upload")}
          primary
          onClick={onUploadPluginClick}
        />
      )}
      {!!pluginList?.length ? (
        <PluginList
          plugins={pluginList}
          onActivate={onActivate}
          onDelete={onDelete}
          theme={theme}
          t={t}
          withDelete={withDelete}
          changePluginStatus={changePluginStatus}
          uninstallPlugin={uninstallPlugin}
        />
      ) : (
        <EmptyContainer
          t={t}
          withUpload={withUpload}
          onUploadPluginClick={onUploadPluginClick}
        />
      )}
      <input
        ref={inputPluginElement}
        id="customPluginInput"
        className="custom-plugin-input"
        type="file"
        accept=".js"
        onInput={onInput}
      />
    </StyledContainer>
  );
};

export default inject(({ auth, pluginStore }) => {
  const { settingsStore, setDocumentTitle } = auth;
  const { theme, pluginOptions } = settingsStore;

  const withUpload = pluginOptions.includes("upload");
  const withDelete = pluginOptions.includes("delete");

  const { plugins, initPlugin, changePluginStatus, uninstallPlugin } =
    pluginStore;

  return {
    theme,
    setDocumentTitle,
    withUpload,
    withDelete,

    plugins,
    initPlugin,
    changePluginStatus,
    uninstallPlugin,
  };
})(observer(WebPlugins));
