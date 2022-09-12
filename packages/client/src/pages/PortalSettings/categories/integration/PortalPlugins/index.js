import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import Button from "@docspace/components/button";
import Box from "@docspace/components/box";
import Link from "@docspace/components/link";
import api from "@docspace/common/api";

import toastr from "@docspace/components/toast/toastr";

import EmptyFolderContainer from "SRC_DIR/components/EmptyContainer/EmptyContainer";
import { initPlugin } from "SRC_DIR/helpers/plugins";

import PluginList from "./sub-components/plugin-list";

const linkStyles = {
  isHovered: true,
  type: "action",
  fontWeight: "600",
  className: "empty-folder_link",
  display: "flex",
};

const StyledContainer = styled.div`
  width: 100%;
  height: 100%;

  display: flex;

  flex-direction: column;

  .plugins__upload-button {
    width: 110px;

    margin-bottom: 24px;
  }

  .custom-plugin-input {
    display: none;
  }
`;

const PortalPlugins = ({
  t,
  setDocumentTitle,
  theme,
  withDelete,
  withUpload,
}) => {
  const [plugins, setPlugins] = React.useState(null);
  const inputPluginElement = React.useRef(null);

  setDocumentTitle(`Portal plugins`);

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
        <EmptyFolderContainer
          headerText={t("FilesSettings:ConnectEmpty")}
          descriptionText={t("Upload plugins here")}
          style={{ gridColumnGap: "39px" }}
          buttonStyle={{ marginTop: "16px" }}
          imageSrc="/static/images/empty_screen_alt.svg"
          buttons={
            <>
              {withUpload && (
                <div className="empty-folder_container-links empty-connect_container-links">
                  <img
                    className="empty-folder_container_plus-image"
                    src="images/plus.svg"
                    onClick={onUploadPluginClick}
                    alt="plus_icon"
                  />
                  <Box className="flex-wrapper_container">
                    <Link {...linkStyles} onClick={onUploadPluginClick}>
                      {t("Article:Upload")}
                    </Link>
                  </Box>
                </div>
              )}
            </>
          }
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
})(
  withTranslation([
    "Settings",
    "Common",
    "PeopleTranslations",
    "People",
    "Article",
    "FilesSettings",
  ])(observer(PortalPlugins))
);
