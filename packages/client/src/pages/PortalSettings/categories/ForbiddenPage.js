import ManageAccessRightsReactSvgUrl from "PUBLIC_DIR/images/manage.access.rights.react.svg?url";
import React from "react";
import { useTranslation } from "react-i18next";

import EmptyScreenContainer from "@docspace/components/empty-screen-container";

const ForbiddenPage = () => {
  const { t } = useTranslation("Settings");

  return (
    <EmptyScreenContainer
      descriptionText={t("ForbiddenPageDescription")}
      headerText={t("ForbiddenPageHeader")}
      imageAlt="Empty screen image"
      imageSrc={ManageAccessRightsReactSvgUrl}
    />
  );
};

export default ForbiddenPage;
