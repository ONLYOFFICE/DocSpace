import React from "react";
import { useTranslation } from "react-i18next";

import EmptyScreenContainer from "@docspace/components/empty-screen-container";

const ForbiddenPage = () => {
  const { t } = useTranslation("SingleSignOn");

  return (
    <EmptyScreenContainer
      descriptionText={t("ForbiddenPageDescription")}
      headerText={t("ForbiddenPageHeader")}
      imageAlt="Empty screen image"
      imageSrc="images/empty_screen_privacy.png"
    />
  );
};

export default ForbiddenPage;
