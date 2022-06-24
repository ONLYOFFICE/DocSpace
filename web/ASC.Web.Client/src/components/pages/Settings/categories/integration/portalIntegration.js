import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Box from "@appserver/components/box";
import TextInput from "@appserver/components/text-input";
import Textarea from "@appserver/components/textarea";
import Checkbox from "@appserver/components/checkbox";
import Button from "@appserver/components/button";
import ComboBox from "@appserver/components/combobox";
import Heading from "@appserver/components/heading";
import toastr from "@appserver/components/toast/toastr";
import { tablet } from "@appserver/components/utils/device";
import { objectToGetParams, loadScript } from "@appserver/common/utils";
import { inject, observer } from "mobx-react";

const Controls = styled(Box)`
  width: 500px;

  @media ${tablet} {
    width: 100%;
  }
`;

const ControlsGroup = styled(Box)`
  display: flex;
  align-items: center;
`;

const Parameter = styled(TextInput)`
  margin-bottom: 16px;
  margin-right: 8px;
`;

const ParameterCheckbox = styled(Checkbox)`
  margin-bottom: 16px;
`;

const ParameterComboBox = styled(ComboBox)`
  margin-bottom: 16px;
`;

const Frame = styled(Box)`
  margin-top: 16px;
`;

const Buttons = styled(Box)`
  margin-top: 16px;
  button {
    margin-right: 16px;
  }
`;

const PortalIntegration = (props) => {
  const { t, setDocumentTitle } = props;

  setDocumentTitle(`Portal integration`);

  const scriptUrl = `${window.location.origin}/static/scripts/ds-api.js`;

  const dataSortBy = [
    { key: "AZ", label: "Title", default: true },
    { key: "Type", label: "Type" },
    { key: "Size", label: "Size" },
    { key: "DateAndTimeCreation", label: "Creation date" },
    { key: "DateAndTime", label: "Last modified date" },
    { key: "Author", label: "Author" },
  ];

  const dataSortOrder = [
    { key: "ascending", label: "Ascending", default: true },
    { key: "descending", label: "Descending" },
  ];

  const [config, setConfig] = useState({
    withSubfolders: true,
    showHeader: false,
    showTitle: true,
    showArticle: false,
    showFilter: false,
  });

  const [sortBy, setSortBy] = useState(dataSortBy[0]);
  const [sortOrder, setSortOrder] = useState(dataSortOrder[0]);

  const params = objectToGetParams(config);

  const frameId = config.frameId || "ds-frame";

  const loadFrame = () => {
    const script = document.getElementById("integration");

    if (script) script.remove();

    const params = objectToGetParams(config);

    loadScript(`${scriptUrl}${params}`, "integration");
  };

  const destroyFrame = () => {
    DocSpace.destroyFrame();
  };

  const showMessage = (message) => {
    const data = message.data;

    if (data.frameId === frameId) {
      toastr.success(data.message, "Frame message");
    }
  };

  useEffect(() => {
    window.addEventListener("message", showMessage, false);

    return () => window.removeEventListener("message", showMessage, false);
  }, [showMessage]);

  const onChangeWidth = (e) => {
    setConfig((config) => {
      return { ...config, width: e.target.value };
    });
  };

  const onChangeHeight = (e) => {
    setConfig((config) => {
      return { ...config, height: e.target.value };
    });
  };

  const onChangeFolderId = (e) => {
    setConfig((config) => {
      return { ...config, folderId: e.target.value };
    });
  };

  const onChangeFrameId = (e) => {
    setConfig((config) => {
      return { ...config, frameId: e.target.value };
    });
  };

  const onChangeWithSubfolders = (e) => {
    setConfig((config) => {
      return { ...config, withSubfolders: !config.withSubfolders };
    });
  };

  const onChangeSortBy = (item) => {
    setConfig((config) => {
      return { ...config, sortBy: item.key };
    });

    setSortBy(item);
  };

  const onChangeSortOrder = (item) => {
    setConfig((config) => {
      return { ...config, sortOrder: item.key };
    });

    setSortOrder(item);
  };

  const onChangeShowHeader = (e) => {
    setConfig((config) => {
      return { ...config, showHeader: !config.showHeader };
    });
  };

  const onChangeShowTitle = () => {
    setConfig((config) => {
      return { ...config, showTitle: !config.showTitle };
    });
  };

  const onChangeShowArticle = (e) => {
    setConfig((config) => {
      return { ...config, showArticle: !config.showArticle };
    });
  };

  const onChangeShowFilter = (e) => {
    setConfig((config) => {
      return { ...config, showFilter: !config.showFilter };
    });
  };

  const codeBlock = `<div id="${frameId}">Fallback text</div>\n<script src="${scriptUrl}${params}"></script>`;

  return (
    <Box>
      <Controls>
        <Heading level={1} size="small">
          Frame options
        </Heading>
        <ControlsGroup>
          <Parameter
            onChange={onChangeFrameId}
            placeholder="Frame id"
            value={config.frameId}
          />
        </ControlsGroup>

        <ControlsGroup>
          <Parameter
            onChange={onChangeWidth}
            placeholder="Frame width"
            value={config.width}
          />
        </ControlsGroup>

        <ControlsGroup>
          <Parameter
            onChange={onChangeHeight}
            placeholder="Frame height"
            value={config.height}
          />
        </ControlsGroup>

        <ParameterCheckbox
          label="Show header"
          onChange={onChangeShowHeader}
          isChecked={config.showHeader}
        />

        <ParameterCheckbox
          label="Show title"
          onChange={onChangeShowTitle}
          isChecked={config.showTitle}
        />

        <ParameterCheckbox
          label="Show article"
          onChange={onChangeShowArticle}
          isChecked={config.showArticle}
        />

        <ParameterCheckbox
          label="Show filter"
          onChange={onChangeShowFilter}
          isChecked={config.showFilter}
        />

        <Heading level={1} size="small">
          Filter options
        </Heading>

        <ControlsGroup>
          <Parameter
            onChange={onChangeFolderId}
            placeholder="Folder id"
            value={config.folderId}
          />
          <ParameterCheckbox
            label="With subfolders"
            onChange={onChangeWithSubfolders}
            isChecked={config.withSubfolders}
          />
        </ControlsGroup>

        <ParameterComboBox
          onSelect={onChangeSortBy}
          options={dataSortBy}
          scaled={true}
          selectedOption={sortBy}
          displaySelectedOption
        />

        <ParameterComboBox
          onSelect={onChangeSortOrder}
          options={dataSortOrder}
          scaled={true}
          selectedOption={sortOrder}
          displaySelectedOption
        />
      </Controls>
      <Heading level={1} size="xsmall">
        Paste this code block on page:
      </Heading>

      <Textarea value={codeBlock} />

      <Buttons>
        <Button primary size="normal" label="Preview" onClick={loadFrame} />
        <Button primary size="normal" label="Destroy" onClick={destroyFrame} />
      </Buttons>

      <Frame>
        <Box id={frameId}>Frame content</Box>
      </Frame>
    </Box>
  );
};

export default inject(({ setup, auth }) => {
  const { settingsStore, setDocumentTitle } = auth;
  const { theme } = settingsStore;

  return {
    theme,
    setDocumentTitle,
  };
})(withTranslation(["Settings", "Common"])(observer(PortalIntegration)));
