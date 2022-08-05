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
      imageSrc="/static/images/manage.access.rights.react.svg"
    />
  );
};

export default ForbiddenPage;
