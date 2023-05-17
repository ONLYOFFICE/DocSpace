import PanelReactSvgUrl from "PUBLIC_DIR/images/panel.react.svg?url";
import ArrowPathReactSvgUrl from "PUBLIC_DIR/images/arrow.path.react.svg?url";
import React from "react";
import { inject, observer } from "mobx-react";
import IconButton from "@docspace/components/icon-button";
import { withTranslation } from "react-i18next";
import { useNavigate, useParams } from "react-router-dom";
import {
  StyledHeadline,
  StyledContainer,
  StyledInfoPanelToggleWrapper,
} from "./StyledGallery";
import config from "PACKAGE_FILE";
import FilesFilter from "@docspace/common/api/files/filter";
import { combineUrl } from "@docspace/common/utils";
import { getCategoryUrl } from "SRC_DIR/helpers/utils";

const SectionHeaderContent = (props) => {
  const {
    t,

    isInfoPanelVisible,
    setIsInfoPanelVisible,
    setGallerySelected,
    categoryType,
  } = props;

  const navigate = useNavigate();
  const params = useParams();

  const onBackToFiles = () => {
    setGallerySelected(null);

    const filter = FilesFilter.getDefault();

    filter.folder = params.folderId;

    const filterParamsStr = filter.toUrlParams();

    const url = getCategoryUrl(categoryType, filter.folder);

    const pathname = `${url}?${filterParamsStr}`;

    navigate(
      combineUrl(window.DocSpaceConfig?.proxy?.url, config.homepage, pathname)
    );
  };

  const toggleInfoPanel = () => {
    setIsInfoPanelVisible(!isInfoPanelVisible);
  };

  return (
    <StyledContainer>
      <IconButton
        iconName={ArrowPathReactSvgUrl}
        size="17"
        isFill
        onClick={onBackToFiles}
        className="arrow-button"
      />

      <StyledHeadline type="content" truncate>
        {t("Common:OFORMsGallery")}
      </StyledHeadline>
      <StyledInfoPanelToggleWrapper isInfoPanelVisible={isInfoPanelVisible}>
        <div className="info-panel-toggle-bg">
          <IconButton
            className="info-panel-toggle"
            iconName={PanelReactSvgUrl}
            size="16"
            isFill={true}
            onClick={toggleInfoPanel}
            title={t("Common:InfoPanel")}
          />
        </div>
      </StyledInfoPanelToggleWrapper>
    </StyledContainer>
  );
};

export default inject(({ auth, filesStore, oformsStore }) => {
  const { isVisible, setIsVisible } = auth.infoPanelStore;
  const { categoryType } = filesStore;
  const { setGallerySelected } = oformsStore;
  return {
    isInfoPanelVisible: isVisible,
    setIsInfoPanelVisible: setIsVisible,
    setGallerySelected,
    categoryType,
  };
})(withTranslation("Common")(observer(SectionHeaderContent)));
