import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import Box from "@docspace/components/box";
import TextInput from "@docspace/components/text-input";
import Textarea from "@docspace/components/textarea";
import Label from "@docspace/components/label";
import Checkbox from "@docspace/components/checkbox";
import Button from "@docspace/components/button";
import ComboBox from "@docspace/components/combobox";
import Heading from "@docspace/components/heading";
import { tablet } from "@docspace/components/utils/device";
import { objectToGetParams, loadScript } from "@docspace/common/utils";
import { inject, observer } from "mobx-react";
import { isMobile } from "react-device-detect";
import BreakpointWarning from "SRC_DIR/components/BreakpointWarning";

const Controls = styled(Box)`
  width: 500px;
  display: flex;
  flex-direction: column;
  gap: 16px;

  @media ${tablet} {
    width: 100%;
  }

  .label {
    min-width: fit-content;
  }
`;

const ControlsGroup = styled(Box)`
  display: flex;
  flex-direction: column;
  gap: 8px;
`;

const Frame = styled(Box)`
  margin-top: 16px;

  > div {
    border: 1px dashed gray;
    border-radius: 3px;
    min-width: 100%;
    min-height: 400px;
  }
`;

const Buttons = styled(Box)`
  margin-top: 16px;
  button {
    margin-right: 16px;
  }
`;

const Container = styled(Box)`
  width: 100%;
  display: flex;
  gap: 16px;
`;

const Preview = styled(Box)`
  width: 50%;
  flex-direction: row;

  .frameStyle {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
    justify-content: center;
    align-items: center;
  }
`;

const PortalIntegration = (props) => {
  const { t, setDocumentTitle } = props;

  setDocumentTitle(`Portal integration`);

  const scriptUrl = `${window.location.origin}/static/scripts/api.js`;

  const dataSortBy = [
    { key: "DateAndTime", label: "Last modified date", default: true },
    { key: "AZ", label: "Title" },
    { key: "Type", label: "Type" },
    { key: "Size", label: "Size" },
    { key: "DateAndTimeCreation", label: "Creation date" },
    { key: "Author", label: "Author" },
  ];

  const dataFilterType = [
    { key: 0, label: "None", default: true },
    { key: 1, label: "Files" },
    { key: 2, label: "Folders" },
    { key: 3, label: "Documents" },
    { key: 4, label: "Presentations" },
    { key: 5, label: "Spreadsheets" },
    { key: 7, label: "Images" },
    { key: 8, label: "By user" },
    { key: 9, label: "By department" },
    { key: 10, label: "Archive" },
    { key: 11, label: "By Extension" },
    { key: 12, label: "Media" },
  ];

  const dataSortOrder = [
    { key: "descending", label: "Descending", default: true },
    { key: "ascending", label: "Ascending" },
  ];

  const dataDisplayType = [
    { key: "row", label: "Row", default: true },
    { key: "table", label: "Table" },
    { key: "tile", label: "Tile" },
  ];

  const [config, setConfig] = useState({
    width: "100%",
    height: "400px",
    frameId: "ds-frame",
    showHeader: false,
    showTitle: true,
    showArticle: false,
    showFilter: false,
  });

  const [sortBy, setSortBy] = useState(dataSortBy[0]);
  const [sortOrder, setSortOrder] = useState(dataSortOrder[0]);
  const [filterType, setFilterType] = useState(dataFilterType[0]);
  const [displayType, setDisplayType] = useState(dataDisplayType[0]);
  const [withSubfolders, setWithSubfolders] = useState(false);

  const params = objectToGetParams(config);

  const frameId = config.frameId || "ds-frame";

  const destroyFrame = () => {
    DocSpace.destroyFrame();
  };

  const loadFrame = () => {
    const script = document.getElementById("integration");

    if (script) {
      destroyFrame();
      script.remove();
    }

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

  const onChangeFolderId = (e) => {
    setConfig((config) => {
      return { ...config, folder: e.target.value };
    });
  };

  const onChangeFrameId = (e) => {
    setConfig((config) => {
      return { ...config, frameId: e.target.value };
    });
  };

  const onChangeWithSubfolders = (e) => {
    setConfig((config) => {
      return { ...config, withSubfolders: !withSubfolders };
    });

    setWithSubfolders(!withSubfolders);
  };

  const onChangeSortBy = (item) => {
    setConfig((config) => {
      return { ...config, sortby: item.key };
    });

    setSortBy(item);
  };

  const onChangeSortOrder = (item) => {
    setConfig((config) => {
      return { ...config, sortorder: item.key };
    });

    setSortOrder(item);
  };

  const onChangeFilterType = (item) => {
    setConfig((config) => {
      return { ...config, filterType: item.key };
    });

    setFilterType(item);
  };

  const onChangeDisplayType = (item) => {
    setConfig((config) => {
      return { ...config, viewAs: item.key };
    });

    setDisplayType(item);
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

  const onChangeCount = (e) => {
    setConfig((config) => {
      return { ...config, count: e.target.value };
    });
  };

  const onChangePage = (e) => {
    setConfig((config) => {
      return { ...config, page: e.target.value };
    });
  };

  const onChangeSearch = (e) => {
    setConfig((config) => {
      return { ...config, search: e.target.value };
    });
  };

  const onChangeAuthor = (e) => {
    setConfig((config) => {
      return { ...config, authorType: e.target.value };
    });
  };

  const codeBlock = `<div id="${frameId}">Fallback text</div>\n<script src="${scriptUrl}${params}"></script>`;

  return (
    <>
      {isMobile ? (
        <BreakpointWarning sectionName={t("Settings:Branding")} />
      ) : (
        <Container>
          <Controls>
            <Heading level={1} size="small">
              Frame options
            </Heading>
            <ControlsGroup>
              <Label className="label" text="Frame id" />
              <TextInput
                scale={true}
                onChange={onChangeFrameId}
                placeholder="Frame id"
                value={config.frameId}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text="Frame width" />
              <TextInput
                scale={true}
                onChange={onChangeWidth}
                placeholder="Frame width"
                value={config.width}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text="Frame height" />
              <TextInput
                scale={true}
                onChange={onChangeHeight}
                placeholder="Frame height"
                value={config.height}
              />
            </ControlsGroup>
            {/* <ControlsGroup>
          <Label className="label" text="Display mode: " />
          <ComboBox
            scale={true}
            onSelect={onChangeDisplayType}
            options={dataDisplayType}
            scaled={true}
            selectedOption={displayType}
            displaySelectedOption
          />
        </ControlsGroup> */}
            <Checkbox
              label="Show header"
              onChange={onChangeShowHeader}
              isChecked={config.showHeader}
            />
            <Checkbox
              label="Show title"
              onChange={onChangeShowTitle}
              isChecked={config.showTitle}
            />
            <Checkbox
              label="Show article"
              onChange={onChangeShowArticle}
              isChecked={config.showArticle}
            />
            <Checkbox
              label="Show filter"
              onChange={onChangeShowFilter}
              isChecked={config.showFilter}
            />
            <Heading level={1} size="small">
              Filter options
            </Heading>
            <ControlsGroup>
              <Label className="label" text="Folder id" />
              <TextInput
                scale={true}
                onChange={onChangeFolderId}
                placeholder="Folder id"
                value={config.folder}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text="Items count" />
              <TextInput
                scale={true}
                onChange={onChangeCount}
                placeholder="Items count"
                value={config.count}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text="Page" />
              <TextInput
                scale={true}
                onChange={onChangePage}
                placeholder="Page"
                value={config.page}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text="Search term" />
              <Box
                style={{ flexDirection: "row", display: "flex", gap: "16px" }}
              >
                <TextInput
                  scale={true}
                  onChange={onChangeSearch}
                  placeholder="Search term"
                  value={config.search}
                />
                <Checkbox
                  label="With subfolders"
                  onChange={onChangeWithSubfolders}
                  isChecked={withSubfolders}
                />
              </Box>
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text="Author" />
              <TextInput
                scale={true}
                onChange={onChangeAuthor}
                placeholder="Author"
                value={config.authorType}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text="Filter type:" />
              <ComboBox
                onSelect={onChangeFilterType}
                options={dataFilterType}
                scaled={true}
                selectedOption={filterType}
                displaySelectedOption
                directionY="top"
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text="Sort by:" />
              <ComboBox
                onSelect={onChangeSortBy}
                options={dataSortBy}
                scaled={true}
                selectedOption={sortBy}
                displaySelectedOption
                directionY="top"
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text="Sort order:" />
              <ComboBox
                onSelect={onChangeSortOrder}
                options={dataSortOrder}
                scaled={true}
                selectedOption={sortOrder}
                displaySelectedOption
                directionY="top"
              />
            </ControlsGroup>
          </Controls>
          <Preview>
            <Frame>
              <Box id={frameId} className="frameStyle">
                Frame content
              </Box>
            </Frame>

            <Buttons>
              <Button
                primary
                size="normal"
                label={t("Common:Preview")}
                onClick={loadFrame}
              />
              <Button
                primary
                size="normal"
                label="Destroy"
                onClick={destroyFrame}
              />
            </Buttons>

            <Heading level={1} size="xsmall">
              Paste this code block on page:
            </Heading>

            <Textarea value={codeBlock} />
          </Preview>
        </Container>
      )}
    </>
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
