import React, { useState, useEffect } from "react";
import PropTypes from "prop-types";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Box from "@appserver/components/box";
import TextInput from "@appserver/components/text-input";
import Label from "@appserver/components/label";
import Text from "@appserver/components/text";
import Checkbox from "@appserver/components/checkbox";
import Button from "@appserver/components/button";
import toastr from "@appserver/components/toast/toastr";
import { tablet, mobile } from "@appserver/components/utils/device";
import {
  showLoader,
  hideLoader,
  objectToGetParams,
  loadScript,
} from "@appserver/common/utils";
import { inject, observer } from "mobx-react";
import { Base } from "@appserver/components/themes";

const Controls = styled(Box)`
  width: 300px;
`;

const ControlsGroup = styled(Box)`
  display: flex;
  align-items: center;
`;

const Parameter = styled(TextInput)`
  margin: 8px;
`;

const Frame = styled(Box)`
  margin-top: 16px;
`;

const CodeBox = styled(Box)`
  margin: 16px 0px;
  padding: 16px;
  border: 1px solid black;
  border-radius: 10px;
`;

const Code = styled(Text)`
  font-family: monospace;
`;

const PortalIntegration = (props) => {
  const { t, setDocumentTitle } = props;

  const [config, setConfig] = useState({ withSubfolders: true });

  const scriptUrl = "http://192.168.1.60:8092/static/scripts/ds-api.js";

  setDocumentTitle(`Portal integration`);

  const loadFrame = () => {
    const script = document.getElementById("integration");

    if (script) script.remove();

    const params = objectToGetParams(config);

    loadScript(`${scriptUrl}${params}`, "integration");
  };

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

  const onChangeSrc = (e) => {
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

  const params = objectToGetParams(config);

  const frameId = config.frameId || "ds-frame";

  return (
    <Box>
      <Controls>
        <ControlsGroup>
          <Parameter
            onChange={onChangeFrameId}
            placeholder="Enter frame id"
            value={config.frameId}
          />
        </ControlsGroup>

        <ControlsGroup>
          <Parameter
            onChange={onChangeWidth}
            placeholder="Enter frame width"
            value={config.width}
          />
        </ControlsGroup>

        <ControlsGroup>
          <Parameter
            onChange={onChangeHeight}
            placeholder="Enter frame height"
            value={config.height}
          />
        </ControlsGroup>

        <ControlsGroup>
          <Parameter
            onChange={onChangeSrc}
            placeholder="Enter folder id"
            value={config.folderId}
          />
          <Checkbox
            label="With subfolders"
            onChange={onChangeWithSubfolders}
            isChecked={config.withSubfolders}
          />
        </ControlsGroup>
      </Controls>
      <Text>Paste this code block on page:</Text>
      <CodeBox>
        <Code>
          {'<div id="'}
          {frameId}
          {'">Fallback text</div>'}
        </Code>
        <Code>
          {'<script src="'}
          {scriptUrl}
          {params}
          {'"></script>'}
        </Code>
      </CodeBox>
      <Button primary size="normal" label="Preview" onClick={loadFrame} />
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
