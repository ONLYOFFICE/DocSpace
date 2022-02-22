import React from "react";
import EmptyScreenContainer from "@appserver/components/empty-screen-container";

const ForbiddenPage = ({ t }) => {
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
