import React from "react";

import { inject, observer } from "mobx-react";
import { useTranslation } from "react-i18next";

import api from "@docspace/common/api";

import toastr from "@docspace/components/toast/toastr";
import Button from "@docspace/components/button";

import { initPlugin } from "SRC_DIR/helpers/plugins";

import PluginList from "./sub-components/plugin-list";
import EmptyContainer from "./sub-components/empty-container";

import { StyledContainer } from "./StyledWebPlugins";

const WebPlugins = ({ setDocumentTitle, theme, withDelete, withUpload }) => {
  const [plugins, setPlugins] = React.useState(null);

  const inputPluginElement = React.useRef(null);

  const { t } = useTranslation([
    "Settings",
    "Common",
    "PeopleTranslations",
    "People",
    "Article",
    "FilesSettings",
  ]);

  setDocumentTitle(t("PortalIntegration"));

  const uploadPlugin = React.useCallback(
    async (files) => {
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

        // addPlugin(plugin);
      } catch (e) {
        console.log(e);
      }
    },
    [addPlugin]
  );

  const onInput = React.useCallback(
    (e) => {
      uploadPlugin(e.target.files);
      e.target.value = null;
    },
    [uploadPlugin]
  );

  const onUploadPluginClick = React.useCallback(() => {
    withUpload && inputPluginElement.current.click();
  }, [inputPluginElement.current, withUpload]);

  const onActivate = React.useCallback(
    (id, status) => {
      setPlugins((val) => {
        const newPlugins = val;

        const idx = newPlugins.findIndex((plugin) => +plugin.id === +id);

        if (idx > -1) {
          newPlugins[idx].isActive = status === "true";
        }

        return [...newPlugins];
      });
    },
    [plugins]
  );

  const onDelete = React.useCallback(
    (id) => {
      setPlugins((val) => {
        const newPlugins = val.filter((plugin) => +plugin.id !== +id);

        if (newPlugins.length === 0) return [];

        return [...newPlugins];
      });
    },
    [plugins]
  );

  const addPlugin = React.useCallback((plugin) => {
    setPlugins((value) => {
      if (value) return [...value, plugin];

      return [plugin];
    });
  }, []);

  React.useEffect(() => {
    const newPlugins = [];

    Array.from(window.PluginStore.plugins, ([key, value]) =>
      newPlugins.push(value)
    );

    setPlugins(newPlugins);
  }, [window.PluginStore]);

  return (
    <StyledContainer>
      {withUpload && !!plugins?.length && (
        <Button
          className={"plugins__upload-button"}
          size={"small"}
          label={t("Article:Upload")}
          primary
          onClick={onUploadPluginClick}
        />
      )}
      {!!plugins?.length ? (
        <PluginList
          plugins={plugins}
          onActivate={onActivate}
          onDelete={onDelete}
          theme={theme}
          t={t}
          withDelete={withDelete}
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

export default inject(({ auth }) => {
  const { settingsStore, setDocumentTitle } = auth;
  const { theme, pluginOptions } = settingsStore;

  const withUpload = pluginOptions.includes("upload");
  const withDelete = pluginOptions.includes("delete");

  return {
    theme,
    setDocumentTitle,
    withUpload,
    withDelete,
  };
})(observer(WebPlugins));
