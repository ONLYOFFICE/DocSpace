import React from "react";
import IconButton from "@appserver/components/icon-button";
import { withTranslation } from "react-i18next";
import { withRouter } from "react-router-dom";
import { StyledHeadline, StyledContainer } from "./StyledGallery";

const SectionHeaderContent = ({ t, history }) => {
  const onBackToFiles = () => history.goBack();

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
