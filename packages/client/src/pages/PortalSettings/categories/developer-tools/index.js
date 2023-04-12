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

  setDocumentTitle(t("JavascriptSdk"));

  const scriptUrl = `${window.location.origin}/static/scripts/api.js`;

  const dataSortBy = [
    { key: "DateAndTime", label: t("Common:LastModifiedDate"), default: true },
    { key: "AZ", label: t("Common:Title") },
    { key: "Type", label: t("Common:Type") },
    { key: "Size", label: t("Common:Size") },
    { key: "DateAndTimeCreation", label: t("Files:ByCreation") },
    { key: "Author", label: t("Files:ByAuthor") },
  ];

  const dataSortOrder = [
    { key: "descending", label: t("Descending"), default: true },
    { key: "ascending", label: t("Ascending") },
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
        <BreakpointWarning sectionName={t("JavascriptSdk")} />
      ) : (
        <Container>
          <Controls>
            <Heading level={1} size="small">
              {t("WindowParameters")}
            </Heading>
            <ControlsGroup>
              <Label className="label" text={t("FrameId")} />
              <TextInput
                scale={true}
                onChange={onChangeFrameId}
                placeholder={t("EnterId")}
                value={config.frameId}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text={t("EmbeddingPanel:Width")} />
              <TextInput
                scale={true}
                onChange={onChangeWidth}
                placeholder={t("EnterWidth")}
                value={config.width}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text={t("EmbeddingPanel:Height")} />
              <TextInput
                scale={true}
                onChange={onChangeHeight}
                placeholder={t("EnterHeight")}
                value={config.height}
              />
            </ControlsGroup>
            <Checkbox
              label={t("Header")}
              onChange={onChangeShowHeader}
              isChecked={config.showHeader}
            />
            <Checkbox
              label={t("Common:Title")}
              onChange={onChangeShowTitle}
              isChecked={config.showTitle}
            />
            <Checkbox
              label={t("Menu")}
              onChange={onChangeShowArticle}
              isChecked={config.showArticle}
            />
            <Checkbox
              label={t("Files:Filter")}
              onChange={onChangeShowFilter}
              isChecked={config.showFilter}
            />
            <Heading level={1} size="small">
              {t("DataDisplay")}
            </Heading>
            <ControlsGroup>
              <Label className="label" text={t("FolderId")} />
              <TextInput
                scale={true}
                onChange={onChangeFolderId}
                placeholder={t("EnterId")}
                value={config.folder}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text={t("ItemsCount")} />
              <TextInput
                scale={true}
                onChange={onChangeCount}
                placeholder={t("EnterCount")}
                value={config.count}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text={t("Page")} />
              <TextInput
                scale={true}
                onChange={onChangePage}
                placeholder={t("EnterPage")}
                value={config.page}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text={t("SearchTerm")} />
              <Box
                style={{ flexDirection: "row", display: "flex", gap: "16px" }}
              >
                <TextInput
                  scale={true}
                  onChange={onChangeSearch}
                  placeholder={t("Common:Search")}
                  value={config.search}
                />
                <Checkbox
                  label={t("Files:WithSubfolders")}
                  onChange={onChangeWithSubfolders}
                  isChecked={withSubfolders}
                />
              </Box>
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text={t("Files:ByAuthor")} />
              <TextInput
                scale={true}
                onChange={onChangeAuthor}
                placeholder={t("EnterName")}
                value={config.authorType}
              />
            </ControlsGroup>
            <ControlsGroup>
              <Label className="label" text={t("Common:SortBy")} />
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
              <Label className="label" text={t("SortOrder")} />
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
                {t("Common:Preview")}
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
                label={t("Destroy")}
                onClick={destroyFrame}
              />
            </Buttons>

            <Heading level={1} size="xsmall">
              {t("CopyWindowCode")}
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
})(
  withTranslation(["JavascriptSdk", "Files", "EmbeddingPanel", "Common"])(
    observer(PortalIntegration)
  )
);
