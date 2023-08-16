import React from "react";
import { withTranslation } from "react-i18next";
import styled from "styled-components";
import { inject, observer } from "mobx-react";
import { isMobileOnly } from "react-device-detect";
import { smallTablet } from "@docspace/components/utils/device";
import Button from "@docspace/components/button";
import EmptyScreenContainer from "@docspace/components/empty-screen-container";
import ConfirmWrapper from "../../../../Confirm/ConfirmWrapper";

import ApiSvgUrl from "PUBLIC_DIR/images/settings.api.svg?url";
import ApiDarkSvgUrl from "PUBLIC_DIR/images/settings.api.dark.svg?url";

const EmptyContainer = styled(EmptyScreenContainer)`
  .ec-header {
    font-size: 23px;
  }

  .ec-image {
    margin-right: 22px;
    margin-top: 22px;

    @media ${smallTablet} {
      margin-bottom: 22px;
    }
  }

  .ec-desc {
    font-size: 13px;
    margin-top: 12px;
    margin-bottom: 22px;
  }
`;

const Api = (props) => {
  const { t, setDocumentTitle, theme, apiBasicLink } = props;

  const imgSrc = theme.isBase ? ApiSvgUrl : ApiDarkSvgUrl;

  setDocumentTitle(t("Api"));

  return (
    <ConfirmWrapper height="100%">
      <EmptyContainer
        buttons={
          <Button
            label={t("Common:LearnMore")}
            primary
            size="normal"
            minwidth="135px"
            onClick={() => window.open(apiBasicLink, "_blank")}
            scale={isMobileOnly}
          />
        }
        descriptionText={t("ApiPageDescription")}
        headerText={t("ApiPageHeader")}
        imageAlt={t("ApiPageHeader")}
        imageSrc={imgSrc}
      />
    </ConfirmWrapper>
  );
};

export default inject(({ auth }) => {
  const { settingsStore, setDocumentTitle } = auth;
  const { theme, apiBasicLink } = settingsStore;

  return {
    theme,
    setDocumentTitle,
    apiBasicLink,
  };
})(withTranslation(["Settings", "Common"])(observer(Api)));
