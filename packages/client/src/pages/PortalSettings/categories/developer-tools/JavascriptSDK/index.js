import React, { useState, useEffect } from "react";
import { withTranslation } from "react-i18next";
import debounce from "lodash.debounce";
import styled from "styled-components";
import Box from "@docspace/components/box";
import TextInput from "@docspace/components/text-input";
import Textarea from "@docspace/components/textarea";
import Label from "@docspace/components/label";
import Text from "@docspace/components/text";
import Checkbox from "@docspace/components/checkbox";
import ComboBox from "@docspace/components/combobox";
import TabContainer from "@docspace/components/tabs-container";
import SelectFolderInput from "client/SelectFolderInput";
import { tablet } from "@docspace/components/utils/device";
import { objectToGetParams, loadScript } from "@docspace/common/utils";
import { inject, observer } from "mobx-react";
import { isMobileOnly } from "react-device-detect";
import BreakpointWarning from "SRC_DIR/components/BreakpointWarning";
import Loaders from "@docspace/common/components/Loaders";
import HelpButton from "@docspace/components/help-button";
import Link from "@docspace/components/link";

const Controls = styled(Box)`
  min-width: 350px;
  max-width: 350px;
  display: flex;
  flex-direction: column;
  gap: 16px;

  .label {
    min-width: fit-content;
  }
`;

const CategoryHeader = styled.div`
  margin-top: 40px;
  margin-bottom: 24px;
  font-size: 16px;
  font-style: normal;
  font-weight: 700;
  line-height: 22px;
`;

const CategorySubHeader = styled.div`
  margin-top: 8px;
  margin-bottom: 8px;
  font-size: 15px;
  font-style: normal;
  font-weight: 600;
  line-height: 16px;
`;

const CategoryDescription = styled(Box)`
  max-width: 700px;
`;

const ControlsGroup = styled(Box)`
  display: flex;
  flex-direction: column;
  gap: 8px;
`;

const Frame = styled(Box)`
  margin-top: 16px;
  position: relative;

  ${(props) =>
    props.targetId &&
    `
    #${props.targetId} {
      position: absolute;
      border-radius: 6px;
      min-width: ${props.width ? props.width : "100%"};
      min-height: ${props.height ? props.height : "400px"};
    }
  `}
`;

const Container = styled(Box)`
  width: 100%;
  display: flex;
  flex-direction: row-reverse;
  justify-content: flex-end;
  gap: 16px;

  @media ${tablet} {
    flex-direction: column;
  }
`;

const RowContainer = styled(Box)`
  flex-direction: row;
  display: flex;
  gap: 8px;

  ${(props) =>
    props.combo &&
    `
      height: 32px;
      align-items: center;
    `}
`;

const ColumnContainer = styled(Box)`
  flex-direction: column;
  display: flex;
  gap: 8px;
`;

const Preview = styled(Box)`
  margin-top: 24px;
  min-width: 660px;
  flex-direction: row;
`;

const PortalIntegration = (props) => {
  const { t, setDocumentTitle, currentColorScheme, sdkLink } = props;

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

  const dataDimensions = [
    { key: "percent", label: "%", default: true },
    { key: "pixel", label: "px" },
  ];

  const [sortBy, setSortBy] = useState(dataSortBy[0]);
  const [sortOrder, setSortOrder] = useState(dataSortOrder[0]);
  const [widthDimension, setWidthDimension] = useState(dataDimensions[0]);
  const [heightDimension, setHeightDimension] = useState(dataDimensions[1]);
  const [width, setWidth] = useState("100");
  const [height, setHeight] = useState("600");
  const [withSubfolders, setWithSubfolders] = useState(true);
  const [folderPanelVisible, setFolderPanelVisible] = useState(false);

  const [config, setConfig] = useState({
    width: `${width}${widthDimension.label}`,
    height: `${height}${heightDimension.label}`,
    frameId: "ds-frame",
    showHeader: true,
    showTitle: true,
    showMenu: true,
    showFilter: true,
  });

  const params = objectToGetParams(config);

  const frameId = config.frameId || "ds-frame";

  const destroyFrame = () => {
    window.DocSpace?.SDK.frames[frameId]?.destroyFrame();
  };

  const loadFrame = debounce(() => {
    const script = document.getElementById("integration");

    if (script) {
      script.remove();
    }

    const params = objectToGetParams(config);

    loadScript(`${scriptUrl}${params}`, "integration", () =>
      window.DocSpace.SDK.initFrame(config)
    );
  }, 500);

  useEffect(() => {
    loadFrame();
    return () => destroyFrame();
  });

  const onChangeTab = () => {
    loadFrame();
  };

  const onChangeWidth = (e) => {
    setConfig((config) => {
      return { ...config, width: `${e.target.value}${widthDimension.label}` };
    });

    setWidth(e.target.value);
  };

  const onChangeHeight = (e) => {
    setConfig((config) => {
      return { ...config, height: `${e.target.value}${heightDimension.label}` };
    });

    setHeight(e.target.value);
  };

  const onChangeFolderId = (id) => {
    setConfig((config) => {
      return { ...config, id };
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

  const onChangeWidthDimension = (item) => {
    setConfig((config) => {
      return { ...config, width: `${width}${item.label}` };
    });

    setWidthDimension(item);
  };

  const onChangeHeightDimension = (item) => {
    setConfig((config) => {
      return { ...config, height: `${height}${item.label}` };
    });

    setHeightDimension(item);
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

  const onChangeShowMenu = (e) => {
    setConfig((config) => {
      return { ...config, showMenu: !config.showMenu };
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

  const onClickFolderInput = () => {
    setFolderPanelVisible(true);
  };

  const onCloseFolderInput = () => {
    setFolderPanelVisible(false);
  };

  const codeBlock = `<div id="${frameId}">Fallback text</div>\n<script src="${scriptUrl}${params}"></script>`;

  const preview = (
    <Frame width={width} height={width} targetId={frameId}>
      <Box id={frameId}></Box>
      <Loaders.Rectangle height={height} borderRadius="6px" />
    </Frame>
  );

  const code = (
    <>
      <CategorySubHeader>{t("CopyWindowCode")}</CategorySubHeader>
      <Textarea value={codeBlock} />
    </>
  );

  const dataTabs = [
    {
      key: "preview",
      title: t("Common:Preview"),
      content: preview,
    },
    {
      key: "code",
      title: t("Code"),
      content: code,
    },
  ];

  return (
    <>
      {isMobileOnly ? (
        <BreakpointWarning sectionName={t("JavascriptSdk")} />
      ) : (
        <Box>
          <CategoryDescription>
            {t("SDKDescription")}
            <Link
              color={currentColorScheme?.main?.accent}
              fontSize="12px"
              fontWeight="400"
              onClick={() => window.open(sdkLink, "_blank")}
            >
              {t("APILink")}.
            </Link>
          </CategoryDescription>
          <CategoryHeader>{t("CreateSampleHeader")}</CategoryHeader>
          <Container>
            <Preview>
              <TabContainer onSelect={onChangeTab} elements={dataTabs} />
            </Preview>
            <Controls>
              <CategorySubHeader>{t("CustomizingDisplay")}</CategorySubHeader>
              <ControlsGroup>
                <Label className="label" text={t("EmbeddingPanel:Width")} />
                <RowContainer combo>
                  <TextInput
                    onChange={onChangeWidth}
                    placeholder={t("EnterWidth")}
                    value={width}
                  />
                  <ComboBox
                    size="content"
                    scaled={false}
                    scaledOptions={true}
                    onSelect={onChangeWidthDimension}
                    options={dataDimensions}
                    selectedOption={widthDimension}
                    displaySelectedOption
                    directionY="bottom"
                  />
                </RowContainer>
              </ControlsGroup>
              <ControlsGroup>
                <Label className="label" text={t("EmbeddingPanel:Height")} />
                <RowContainer combo>
                  <TextInput
                    onChange={onChangeHeight}
                    placeholder={t("EnterHeight")}
                    value={height}
                  />
                  <ComboBox
                    size="content"
                    scaled={false}
                    scaledOptions={true}
                    onSelect={onChangeHeightDimension}
                    options={dataDimensions}
                    selectedOption={heightDimension}
                    displaySelectedOption
                    directionY="bottom"
                  />
                </RowContainer>
              </ControlsGroup>
              <ControlsGroup>
                <Label className="label" text={t("FrameId")} />
                <TextInput
                  scale={true}
                  onChange={onChangeFrameId}
                  placeholder={t("EnterId")}
                  value={config.frameId}
                />
              </ControlsGroup>
              <CategorySubHeader>{t("InterfaceElements")}</CategorySubHeader>
              <Checkbox
                label={t("Menu")}
                onChange={onChangeShowMenu}
                isChecked={config.showMenu}
              />
              <Checkbox
                label={t("Header")}
                onChange={onChangeShowHeader}
                isChecked={config.showHeader}
              />
              <Checkbox
                label={t("Filter")}
                onChange={onChangeShowFilter}
                isChecked={config.showFilter}
              />
              <RowContainer>
                <Checkbox
                  label={t("Title")}
                  onChange={onChangeShowTitle}
                  isChecked={config.showTitle}
                />
                <Text color="gray">{`(${t("MobileOnly")})`}</Text>
              </RowContainer>
              <CategorySubHeader>{t("DataDisplay")}</CategorySubHeader>
              <ControlsGroup>
                <Box
                  style={{
                    display: "inline-flex",
                    alignItems: "center",
                    gap: "8px",
                  }}
                >
                  <Label className="label" text={t("RoomOrFolder")} />
                  <HelpButton
                    offsetRight={0}
                    size={12}
                    tooltipContent={
                      <Text fontSize="12px">
                        {t("RoomOrFolderDescription")}
                      </Text>
                    }
                  />
                </Box>
                <SelectFolderInput
                  onSelectFolder={onChangeFolderId}
                  onClose={onCloseFolderInput}
                  onClickInput={onClickFolderInput}
                  isPanelVisible={folderPanelVisible}
                  filteredType="exceptSortedByTags"
                />
              </ControlsGroup>
              <CategorySubHeader>{t("AdvancedDisplay")}</CategorySubHeader>
              <ControlsGroup>
                <Label className="label" text={t("SearchTerm")} />
                <ColumnContainer>
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
                </ColumnContainer>
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
              <ControlsGroup>
                <Box
                  style={{
                    display: "inline-flex",
                    alignItems: "center",
                    gap: "8px",
                  }}
                >
                  <Label className="label" text={t("ItemsCount")} />
                  <HelpButton
                    offsetRight={0}
                    size={12}
                    tooltipContent={
                      <Text fontSize="12px">{t("ItemsCountDescription")}</Text>
                    }
                  />
                </Box>
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
                  isDisabled={!config.count}
                />
              </ControlsGroup>
            </Controls>
          </Container>
        </Box>
      )}
    </>
  );
};

export default inject(({ setup, auth }) => {
  const { settingsStore, setDocumentTitle } = auth;
  const { theme, currentColorScheme, sdkLink } = settingsStore;

  return {
    theme,
    setDocumentTitle,
    currentColorScheme,
    sdkLink,
  };
})(
  withTranslation(["JavascriptSdk", "Files", "EmbeddingPanel", "Common"])(
    observer(PortalIntegration)
  )
);
