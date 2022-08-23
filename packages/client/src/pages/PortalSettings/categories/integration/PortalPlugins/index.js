import React from "react";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { withTranslation } from "react-i18next";

import UploadButton from "./sub-components/upload-button";
import PluginList from "./sub-components/plugin-list";

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

const PortalPlugins = ({ t, setDocumentTitle, theme }) => {
  const [plugins, setPlugins] = React.useState(null);

  setDocumentTitle(`Portal plugins`);

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

        console.log(newPlugins);

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
      <UploadButton t={t} addPlugin={addPlugin} />
      {plugins && (
        <PluginList
          plugins={plugins}
          onActivate={onActivate}
          onDelete={onDelete}
          theme={theme}
        />
      )}
    </StyledContainer>
  );
};

export default inject(({ auth }) => {
  const { settingsStore, setDocumentTitle } = auth;
  const { theme } = settingsStore;

  return {
    theme,
    setDocumentTitle,
  };
})(withTranslation(["Settings", "Common"])(observer(PortalPlugins)));
