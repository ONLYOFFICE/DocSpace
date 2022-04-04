import React from "react";
import IconButton from "@appserver/components/icon-button";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router-dom";
import { AppServerConfig } from "@appserver/common/constants";
import { StyledHeadline, StyledContainer } from "./StyledGallery";
import config from "../../../package.json";
import FilesFilter from "@appserver/common/api/files/filter";
import { combineUrl } from "@appserver/common/utils";

const SectionHeaderContent = (props) => {
  const { t, history, match } = props;

  const onBackToFiles = () => {
    const filter = FilesFilter.getDefault();
    filter.folder = match.params.fileId;
    const urlFilter = filter.toUrlParams();

    history.push(
      combineUrl(
        AppServerConfig.proxyURL,
        config.homepage,
        `/filter?${urlFilter}`
      )
    );
  };

  return (
    <StyledContainer>
      <IconButton
        iconName="/static/images/arrow.path.react.svg"
        size="17"
        isFill
        onClick={onBackToFiles}
        className="arrow-button"
      />

      <StyledHeadline type="content" truncate>
        {t("Common:OFORMsGallery")}
      </StyledHeadline>
    </StyledContainer>
  );
};

export default withTranslation("Common")(withRouter(SectionHeaderContent));
